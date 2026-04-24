using JobPortal.Domain.Entities.Auth;
using JobPortal.Domain.Entities.Billing;
using JobPortal.Domain.Entities.Config;
using JobPortal.Domain.Entities.Master;
using JobPortal.Domain.Entities.Portal;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // ── Auth ──────────────────────────────────────────────────────────────
    DbSet<Tenant> Tenants { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<UserSession> UserSessions { get; }
    DbSet<AuditLog> AuditLogs { get; }

    // ── Portal ────────────────────────────────────────────────────────────
    DbSet<Company> Companies { get; }
    DbSet<JobPosting> JobPostings { get; }
    DbSet<JobSeekerProfile> JobSeekerProfiles { get; }
    DbSet<JobApplication> JobApplications { get; }
    DbSet<SavedJob> SavedJobs { get; }
    DbSet<InterviewSchedule> InterviewSchedules { get; }
    DbSet<OfferLetter> OfferLetters { get; }
    DbSet<JobAlert> JobAlerts { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<CandidatePipelineStage> CandidatePipelineStages { get; }
    DbSet<CandidatePipeline> CandidatePipelines { get; }

    // ── Billing ───────────────────────────────────────────────────────────
    DbSet<TenantSubscription> TenantSubscriptions { get; }
    DbSet<SubscriptionInvoice> SubscriptionInvoices { get; }
    DbSet<JobCredit> JobCredits { get; }
    DbSet<PaymentTransaction> PaymentTransactions { get; }

    // ── Portal sub-resources ──────────────────────────────────────────────────────────────────────
    DbSet<WorkExperience> WorkExperiences { get; }
    DbSet<Education> Educations { get; }

    // ── Config ────────────────────────────────────────────────────────────
    DbSet<TenantSetting> TenantSettings { get; }
    DbSet<EmailTemplate> EmailTemplates { get; }
    DbSet<FeatureFlag> FeatureFlags { get; }
    DbSet<Announcement> Announcements { get; }

    // ── Master ────────────────────────────────────────────────────────────
    DbSet<SubscriptionPlan> SubscriptionPlans { get; }
    DbSet<Skill> Skills { get; }
    DbSet<Industry> Industries { get; }
    DbSet<JobCategory> JobCategories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
