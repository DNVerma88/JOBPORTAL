using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Common;
using JobPortal.Domain.Entities.Auth;
using JobPortal.Domain.Entities.Billing;
using JobPortal.Domain.Entities.Config;
using JobPortal.Domain.Entities.Master;
using JobPortal.Domain.Entities.Portal;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JobPortal.Infrastructure.Persistence;

/// <summary>
/// Main EF Core DbContext.
/// - Multi-tenant isolation via global query filters on TenantId + IsDeleted.
/// - Audit fields auto-populated by AuditInterceptor.
/// - PostgreSQL xmin column mapped as rowversion for optimistic concurrency.
/// </summary>
public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ICurrentUserService currentUserService,
    ITenantService tenantService)
    : DbContext(options), IApplicationDbContext
{
    // ── Auth ──────────────────────────────────────────────────────────────
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // ── Portal ────────────────────────────────────────────────────────────────
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<JobSeekerProfile> JobSeekerProfiles => Set<JobSeekerProfile>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<SavedJob> SavedJobs => Set<SavedJob>();
    public DbSet<InterviewSchedule> InterviewSchedules => Set<InterviewSchedule>();
    public DbSet<OfferLetter> OfferLetters => Set<OfferLetter>();
    public DbSet<JobAlert> JobAlerts => Set<JobAlert>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<CandidatePipelineStage> CandidatePipelineStages => Set<CandidatePipelineStage>();
    public DbSet<CandidatePipeline> CandidatePipelines => Set<CandidatePipeline>();

    // ── Billing ───────────────────────────────────────────────────────────────
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<SubscriptionInvoice> SubscriptionInvoices => Set<SubscriptionInvoice>();
    public DbSet<JobCredit> JobCredits => Set<JobCredit>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();

    // ── Portal sub-resources ─────────────────────────────────────────────────────────────────
    public DbSet<WorkExperience> WorkExperiences => Set<WorkExperience>();
    public DbSet<Education> Educations => Set<Education>();

    // ── Config ────────────────────────────────────────────────────────────── ────────────────────────────────────────────────────────────────
    public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
    public DbSet<Announcement> Announcements => Set<Announcement>();

    // ── Master ────────────────────────────────────────────────────────────────
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Industry> Industries => Set<Industry>();
    public DbSet<JobCategory> JobCategories => Set<JobCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration classes from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global query filters: multi-tenancy + soft delete
        ApplyGlobalFilters(modelBuilder);
    }

    // Exposed as a property so the LINQ expression tree captures `this` (per DbContext
    // instance) rather than a baked-in constant value. EF Core evaluates this per-query.
    private Guid? CurrentTenantId => tenantService.TenantId;

    private void ApplyGlobalFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (!typeof(BaseEntity).IsAssignableFrom(clrType))
                continue;

            // Some entities intentionally omit TenantId or IsDeleted columns.
            bool hasTenantId = entityType.FindProperty(nameof(BaseEntity.TenantId)) != null;
            bool hasIsDeleted = entityType.FindProperty(nameof(BaseEntity.IsDeleted)) != null;

            modelBuilder.Entity(clrType)
                .HasQueryFilter(BuildGlobalFilter(clrType, hasTenantId, hasIsDeleted));
        }
    }

    private System.Linq.Expressions.LambdaExpression BuildGlobalFilter(Type entityType, bool hasTenantId, bool hasIsDeleted)
    {
        var param = System.Linq.Expressions.Expression.Parameter(entityType, "e");

        System.Linq.Expressions.Expression? body = null;

        if (hasIsDeleted)
        {
            var isDeletedProp = System.Linq.Expressions.Expression.Property(param, nameof(BaseEntity.IsDeleted));
            body = System.Linq.Expressions.Expression.Not(isDeletedProp);
        }

        if (hasTenantId)
        {
            // this.CurrentTenantId  (evaluated per DbContext instance, not compiled-in constant)
            var dbCtx = System.Linq.Expressions.Expression.Constant(this, typeof(ApplicationDbContext));
            var currentTenantIdExpr = System.Linq.Expressions.Expression.Property(dbCtx, nameof(CurrentTenantId));

            // !CurrentTenantId.HasValue  (SuperAdmin bypass: null = see all tenants)
            var hasValueProp = System.Linq.Expressions.Expression.Property(currentTenantIdExpr, nameof(Nullable<Guid>.HasValue));
            var tenantIdBypass = System.Linq.Expressions.Expression.Not(hasValueProp);

            // e.TenantId == CurrentTenantId.Value
            var tenantIdProp = System.Linq.Expressions.Expression.Property(param, nameof(BaseEntity.TenantId));
            var tenantValue = System.Linq.Expressions.Expression.Property(currentTenantIdExpr, nameof(Nullable<Guid>.Value));
            var tenantEquals = System.Linq.Expressions.Expression.Equal(tenantIdProp, tenantValue);

            // !CurrentTenantId.HasValue || e.TenantId == CurrentTenantId.Value
            var tenantFilter = System.Linq.Expressions.Expression.OrElse(tenantIdBypass, tenantEquals);
            body = body is null ? tenantFilter
                : System.Linq.Expressions.Expression.AndAlso(tenantFilter, body);
        }

        // If no filter clauses apply (e.g. lightweight join table with no audit cols),
        // use a constant true so EF still has a valid filter expression.
        body ??= System.Linq.Expressions.Expression.Constant(true);

        return System.Linq.Expressions.Expression.Lambda(body, param);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var userId = currentUserService.UserId ?? Guid.Empty;
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedOn = now;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedBy = userId;
                    entry.Entity.ModifiedOn = now;
                    // Prevent overwriting immutable fields
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    entry.Property(e => e.CreatedOn).IsModified = false;
                    break;
            }
        }
    }
}
