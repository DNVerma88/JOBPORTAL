using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Auth.Commands.ForgotPassword;
using JobPortal.Domain.Entities.Auth;
using JobPortal.Domain.Entities.Master;
using JobPortal.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace JobPortal.Application.Features.Admin.AdminFeatures;

// ══════════════════════════════════════════════════════════════════════════════
//  USERS
// ══════════════════════════════════════════════════════════════════════════════

public sealed record GetUsersQuery(int PageNumber = 1, int PageSize = 20, string? Search = null)
    : IRequest<PagedList<AdminUserDto>>;

public sealed record AdminUserDto(
    Guid Id, string Email, string FirstName, string LastName,
    bool IsActive, bool IsEmailVerified, DateTimeOffset? LastLoginOn, DateTimeOffset CreatedOn,
    string? Role);

public sealed class GetUsersQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetUsersQuery, PagedList<AdminUserDto>>
{
    public async Task<PagedList<AdminUserDto>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var query = db.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(u => u.Email.Contains(request.Search) ||
                u.FirstName.Contains(request.Search) || u.LastName.Contains(request.Search));

        var total = await query.CountAsync(ct);
        var users = await query
            .OrderByDescending(u => u.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var userIds = users.Select(u => u.Id).ToList();
        // Get the first role per user (join UserRoles → Roles)
        var roleMap = await db.UserRoles
            .Where(ur => userIds.Contains(ur.UserId))
            .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
            .GroupBy(x => x.UserId)
            .Select(g => new { UserId = g.Key, Role = g.First().Name })
            .ToDictionaryAsync(x => x.UserId, x => x.Role, ct);

        var items = users.Select(u =>
        {
            roleMap.TryGetValue(u.Id, out var role);
            return new AdminUserDto(u.Id, u.Email, u.FirstName, u.LastName,
                u.IsActive, u.IsEmailVerified, u.LastLoginOn, u.CreatedOn, role);
        }).ToList();

        return PagedList<AdminUserDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

public sealed record SuspendUserCommand(Guid UserId) : IRequest;

public sealed class SuspendUserCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<SuspendUserCommand>
{
    public async Task Handle(SuspendUserCommand request, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct)
            ?? throw new KeyNotFoundException($"User {request.UserId} not found.");
        // Suspend by forcing IsActive = false via reflection or domain method
        typeof(User).GetProperty("IsActive")!.SetValue(user, false);
        typeof(User).GetProperty("ModifiedBy")!.SetValue(user, currentUser.UserId ?? Guid.Empty);
        typeof(User).GetProperty("ModifiedOn")!.SetValue(user, DateTimeOffset.UtcNow);
        await db.SaveChangesAsync(ct);
    }
}

public sealed record ActivateUserCommand(Guid UserId) : IRequest;

public sealed class ActivateUserCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<ActivateUserCommand>
{
    public async Task Handle(ActivateUserCommand request, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct)
            ?? throw new KeyNotFoundException($"User {request.UserId} not found.");
        typeof(User).GetProperty("IsActive")!.SetValue(user, true);
        typeof(User).GetProperty("ModifiedBy")!.SetValue(user, currentUser.UserId ?? Guid.Empty);
        typeof(User).GetProperty("ModifiedOn")!.SetValue(user, DateTimeOffset.UtcNow);
        await db.SaveChangesAsync(ct);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
//  TENANTS
// ══════════════════════════════════════════════════════════════════════════════

public sealed record GetTenantsQuery(int PageNumber = 1, int PageSize = 20, string? Search = null)
    : IRequest<PagedList<TenantDto>>;

public sealed record TenantDto(Guid Id, string Name, string Slug, bool IsActive, DateTimeOffset CreatedOn);

public sealed class GetTenantsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetTenantsQuery, PagedList<TenantDto>>
{
    public async Task<PagedList<TenantDto>> Handle(GetTenantsQuery request, CancellationToken ct)
    {
        var query = db.Tenants.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(t => t.Name.Contains(request.Search) || t.Slug.Contains(request.Search));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TenantDto(t.Id, t.Name, t.Slug, t.IsActive, t.CreatedOn))
            .ToListAsync(ct);

        return PagedList<TenantDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

public sealed record CreateTenantCommand(
    string Name,
    string Slug,
    string ContactEmail,
    string? ContactPhone = null,
    string? Address = null,
    string? Country = null,
    bool IsActive = true) : IRequest<Guid>;

public sealed class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100).Matches(@"^[a-z0-9\-]+$");
        RuleFor(x => x.ContactEmail).NotEmpty().MaximumLength(320).EmailAddress();
        RuleFor(x => x.ContactPhone).MaximumLength(20).When(x => x.ContactPhone is not null);
    }
}

public sealed class CreateTenantCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<CreateTenantCommand, Guid>
{
    public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        // For a new tenant, TenantId is self-referential (the tenant owns itself)
        var newId = Guid.NewGuid();
        var created = Tenant.Create(newId, request.Name, request.Slug, request.ContactEmail, userId);
        db.Tenants.Add(created);
        await db.SaveChangesAsync(ct);
        return created.Id;
    }
}

public sealed record UpdateTenantStatusCommand(Guid TenantId, bool IsActive) : IRequest;

public sealed class UpdateTenantStatusCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UpdateTenantStatusCommand>
{
    public async Task Handle(UpdateTenantStatusCommand request, CancellationToken ct)
    {
        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == request.TenantId, ct)
            ?? throw new KeyNotFoundException($"Tenant {request.TenantId} not found.");
        typeof(Tenant).GetProperty("IsActive")!.SetValue(tenant, request.IsActive);
        typeof(Tenant).GetProperty("ModifiedBy")!.SetValue(tenant, currentUser.UserId ?? Guid.Empty);
        typeof(Tenant).GetProperty("ModifiedOn")!.SetValue(tenant, DateTimeOffset.UtcNow);
        await db.SaveChangesAsync(ct);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
//  ROLES & PERMISSIONS
// ══════════════════════════════════════════════════════════════════════════════

public sealed record GetRolesQuery : IRequest<List<RoleDto>>;

public sealed record RoleDto(Guid Id, string Name, string? Description, bool IsSystemRole, DateTimeOffset CreatedOn);

public sealed class GetRolesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken ct)
        => await db.Roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto(r.Id, r.Name, r.Description, r.IsSystemRole, r.CreatedOn))
            .ToListAsync(ct);
}

public sealed record GetRolePermissionsQuery(Guid RoleId) : IRequest<List<PermissionDto>>;

public sealed record PermissionDto(Guid Id, string Name, string Resource, string Action, string? Description);

public sealed class GetRolePermissionsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetRolePermissionsQuery, List<PermissionDto>>
{
    public async Task<List<PermissionDto>> Handle(GetRolePermissionsQuery request, CancellationToken ct)
    {
        var permIds = await db.RolePermissions
            .Where(rp => rp.RoleId == request.RoleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync(ct);

        return await db.Permissions
            .Where(p => permIds.Contains(p.Id))
            .Select(p => new PermissionDto(p.Id, p.Name, p.Resource, p.Action, p.Description))
            .ToListAsync(ct);
    }
}

public sealed record UpdateRolePermissionsCommand(Guid RoleId, List<Guid> PermissionIds) : IRequest;

public sealed class UpdateRolePermissionsCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UpdateRolePermissionsCommand>
{
    public async Task Handle(UpdateRolePermissionsCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var existing = await db.RolePermissions
            .Where(rp => rp.RoleId == request.RoleId)
            .ToListAsync(ct);

        // Remove old
        foreach (var rp in existing)
            rp.SoftDelete(userId);

        // Add new
        foreach (var permId in request.PermissionIds)
        {
            var rp = RolePermission.Create(tenantId, request.RoleId, permId, userId);
            db.RolePermissions.Add(rp);
        }

        await db.SaveChangesAsync(ct);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
//  AUDIT LOGS
// ══════════════════════════════════════════════════════════════════════════════

public sealed record GetAuditLogsQuery(int PageNumber = 1, int PageSize = 50, string? EntityType = null, string? Action = null)
    : IRequest<PagedList<AuditLogDto>>;

public sealed record AuditLogDto(Guid Id, Guid? UserId, string? UserEmail, string Action, string EntityType,
    Guid? EntityId, string? IpAddress, DateTimeOffset CreatedOn);

public sealed class GetAuditLogsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAuditLogsQuery, PagedList<AuditLogDto>>
{
    public async Task<PagedList<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken ct)
    {
        var query = db.AuditLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.EntityType))
            query = query.Where(a => a.EntityType == request.EntityType);
        if (!string.IsNullOrWhiteSpace(request.Action))
            query = query.Where(a => a.Action == request.Action);

        var total = await query.CountAsync(ct);
        var logItems = await query
            .OrderByDescending(a => a.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var userIds = logItems.Where(a => a.UserId.HasValue).Select(a => a.UserId!.Value).Distinct().ToList();
        var emailMap = await db.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Email, ct);

        var items = logItems.Select(a =>
        {
            string? email = a.UserId.HasValue && emailMap.TryGetValue(a.UserId.Value, out var e) ? e : null;
            return new AuditLogDto(a.Id, a.UserId, email, a.Action, a.EntityType, a.EntityId, a.IpAddress, a.CreatedOn);
        }).ToList();

        return PagedList<AuditLogDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
//  SESSIONS
// ══════════════════════════════════════════════════════════════════════════════

public sealed record GetSessionsQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedList<SessionDto>>;

public sealed record SessionDto(Guid Id, Guid UserId, string? DeviceType, string? IpAddress,
    bool IsActive, DateTimeOffset LastActivityAt, DateTimeOffset ExpiresAt, DateTimeOffset CreatedOn);

public sealed class GetSessionsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetSessionsQuery, PagedList<SessionDto>>
{
    public async Task<PagedList<SessionDto>> Handle(GetSessionsQuery request, CancellationToken ct)
    {
        var query = db.UserSessions.Where(s => s.IsActive);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(s => s.LastActivityAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SessionDto(s.Id, s.UserId, s.DeviceType, s.IpAddress,
                s.IsActive, s.LastActivityAt, s.ExpiresAt, s.CreatedOn))
            .ToListAsync(ct);

        return PagedList<SessionDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

public sealed record DeleteSessionCommand(Guid SessionId) : IRequest;

public sealed class DeleteSessionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<DeleteSessionCommand>
{
    public async Task Handle(DeleteSessionCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var session = await db.UserSessions.FirstOrDefaultAsync(s => s.Id == request.SessionId, ct)
            ?? throw new KeyNotFoundException($"Session {request.SessionId} not found.");
        session.Deactivate(userId);
        await db.SaveChangesAsync(ct);
    }
}

// ══════════════════════════════════════════════════════════════════════════════════
//  PERMISSIONS (ALL)
// ══════════════════════════════════════════════════════════════════════════════════

public sealed record GetAllPermissionsQuery : IRequest<List<PermissionDto>>;

public sealed class GetAllPermissionsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllPermissionsQuery, List<PermissionDto>>
{
    public async Task<List<PermissionDto>> Handle(GetAllPermissionsQuery request, CancellationToken ct)
        => await db.Permissions
            .OrderBy(p => p.Resource).ThenBy(p => p.Action)
            .Select(p => new PermissionDto(p.Id, p.Name, p.Resource, p.Action, p.Description))
            .ToListAsync(ct);
}

// ══════════════════════════════════════════════════════════════════════════════════
//  CREATE USER (admin)
// ══════════════════════════════════════════════════════════════════════════════════

public sealed record CreateAdminUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    Guid RoleId
) : IRequest<Guid>;

public sealed class CreateAdminUserCommandValidator : AbstractValidator<CreateAdminUserCommand>
{
    public CreateAdminUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.RoleId).NotEmpty();
    }
}

public sealed class CreateAdminUserCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IPasswordService passwordService)
    : IRequestHandler<CreateAdminUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateAdminUserCommand request, CancellationToken ct)
    {
        var adminId = currentUser.GetRequiredUserId();
        var tenantId = currentUser.GetRequiredTenantId();

        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        if (await db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, ct))
            throw new DuplicateEntityException(nameof(User), "Email");

        var passwordHash = passwordService.Hash(request.Password);
        var user = User.Create(tenantId, request.Email, passwordHash, request.FirstName, request.LastName, adminId);
        db.Users.Add(user);

        var userRole = UserRole.Create(tenantId, user.Id, request.RoleId, adminId);
        db.UserRoles.Add(userRole);

        await db.SaveChangesAsync(ct);
        return user.Id;
    }
}

// ══════════════════════════════════════════════════════════════════════════════════
//  SUBSCRIPTION PLAN CRUD
// ══════════════════════════════════════════════════════════════════════════════════

public sealed record CreateSubscriptionPlanCommand(
    string Tier,
    string Name,
    string? Description,
    decimal PriceMonthly,
    decimal PriceAnnually,
    string CurrencyCode,
    int? MaxJobPostings,
    int? MaxUsers,
    int? MaxResumeViews,
    int JobPostingDurationDays,
    bool IsActive,
    int SortOrder
) : IRequest<Guid>;

public sealed class CreateSubscriptionPlanCommandValidator : AbstractValidator<CreateSubscriptionPlanCommand>
{
    public CreateSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.Tier).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PriceMonthly).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PriceAnnually).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.JobPostingDurationDays).GreaterThan(0);
    }
}

public sealed class CreateSubscriptionPlanCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateSubscriptionPlanCommand, Guid>
{
    public async Task<Guid> Handle(CreateSubscriptionPlanCommand request, CancellationToken ct)
    {
        var plan = new SubscriptionPlan
        {
            Id = Guid.NewGuid(),
            Tier = request.Tier,
            Name = request.Name,
            Description = request.Description,
            PriceMonthly = request.PriceMonthly,
            PriceAnnually = request.PriceAnnually,
            CurrencyCode = request.CurrencyCode,
            MaxJobPostings = request.MaxJobPostings,
            MaxUsers = request.MaxUsers,
            MaxResumeViews = request.MaxResumeViews,
            JobPostingDurationDays = request.JobPostingDurationDays,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder,
            CreatedOn = DateTimeOffset.UtcNow
        };
        db.SubscriptionPlans.Add(plan);
        await db.SaveChangesAsync(ct);
        return plan.Id;
    }
}

public sealed record UpdateSubscriptionPlanCommand(
    Guid Id,
    string Tier,
    string Name,
    string? Description,
    decimal PriceMonthly,
    decimal PriceAnnually,
    string CurrencyCode,
    int? MaxJobPostings,
    int? MaxUsers,
    int? MaxResumeViews,
    int JobPostingDurationDays,
    bool IsActive,
    int SortOrder
) : IRequest;

public sealed class UpdateSubscriptionPlanCommandValidator : AbstractValidator<UpdateSubscriptionPlanCommand>
{
    public UpdateSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.Tier).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PriceMonthly).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PriceAnnually).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.JobPostingDurationDays).GreaterThan(0);
    }
}

public sealed class UpdateSubscriptionPlanCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateSubscriptionPlanCommand>
{
    public async Task Handle(UpdateSubscriptionPlanCommand request, CancellationToken ct)
    {
        var plan = await db.SubscriptionPlans.FindAsync([request.Id], ct)
            ?? throw new EntityNotFoundException(nameof(SubscriptionPlan), request.Id);

        plan.Tier = request.Tier;
        plan.Name = request.Name;
        plan.Description = request.Description;
        plan.PriceMonthly = request.PriceMonthly;
        plan.PriceAnnually = request.PriceAnnually;
        plan.CurrencyCode = request.CurrencyCode;
        plan.MaxJobPostings = request.MaxJobPostings;
        plan.MaxUsers = request.MaxUsers;
        plan.MaxResumeViews = request.MaxResumeViews;
        plan.JobPostingDurationDays = request.JobPostingDurationDays;
        plan.IsActive = request.IsActive;
        plan.SortOrder = request.SortOrder;

        await db.SaveChangesAsync(ct);
    }
}

// ── Admin Reset User Password ──────────────────────────────────────────────────
public sealed record AdminResetUserPasswordCommand(Guid UserId) : IRequest;

public sealed class AdminResetUserPasswordCommandHandler(
    IApplicationDbContext db,
    IEmailService emailService,
    IConfiguration configuration)
    : IRequestHandler<AdminResetUserPasswordCommand>
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(15);

    public async Task Handle(AdminResetUserPasswordCommand request, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([request.UserId], ct)
            ?? throw new EntityNotFoundException(nameof(User), request.UserId);

        var pepper = configuration["Security:PasswordPepper"]
            ?? throw new InvalidOperationException("PasswordPepper is not configured.");
        var frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:3000";

        var expiresEpoch = DateTimeOffset.UtcNow.Add(TokenLifetime).ToUnixTimeSeconds();
        var payload = $"{user.Id}|{user.TenantId}|{expiresEpoch}";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var keyBytes = Encoding.UTF8.GetBytes(pepper);
        var hmac = HMACSHA256.HashData(keyBytes, payloadBytes);
        var token = $"{Convert.ToBase64String(payloadBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_')}" +
                    $".{Convert.ToBase64String(hmac).TrimEnd('=').Replace('+', '-').Replace('/', '_')}";
        var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(token)}";

        await emailService.SendAsync(new EmailMessage(
            To: user.Email,
            Subject: "Your JobPortal password has been reset by an administrator",
            HtmlBody: $"""
                <p>Hi {user.FirstName},</p>
                <p>An administrator has initiated a password reset for your account. Click the link below within 15 minutes:</p>
                <p><a href="{resetLink}">Reset Password</a></p>
                """), ct);
    }
}

// ── Delete Subscription Plan ───────────────────────────────────────────────────
public sealed record DeleteSubscriptionPlanCommand(Guid Id) : IRequest;

public sealed class DeleteSubscriptionPlanCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteSubscriptionPlanCommand>
{
    public async Task Handle(DeleteSubscriptionPlanCommand request, CancellationToken ct)
    {
        var plan = await db.SubscriptionPlans.FindAsync([request.Id], ct)
            ?? throw new EntityNotFoundException(nameof(SubscriptionPlan), request.Id);
        db.SubscriptionPlans.Remove(plan);
        await db.SaveChangesAsync(ct);
    }
}

// ── Create Role ────────────────────────────────────────────────────────────────
public sealed record CreateRoleCommand(string Name, string? Description) : IRequest<Guid>;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public sealed class CreateRoleCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<CreateRoleCommand, Guid>
{
    public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var tenantId = currentUser.TenantId ?? Guid.Empty;
        var userId = currentUser.UserId ?? Guid.Empty;
        var role = Role.Create(tenantId, request.Name, request.Description, false, userId);
        db.Roles.Add(role);
        await db.SaveChangesAsync(ct);
        return role.Id;
    }
}

// ── Update Role ────────────────────────────────────────────────────────────────
public sealed record UpdateRoleCommand(Guid Id, string Name, string? Description) : IRequest;

public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateRoleCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UpdateRoleCommand>
{
    public async Task Handle(UpdateRoleCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var role = await db.Roles.FindAsync([request.Id], ct)
            ?? throw new EntityNotFoundException(nameof(Role), request.Id);
        if (role.IsSystemRole) throw new InvalidOperationException("Cannot modify a system role.");
        role.Update(request.Name, request.Description, userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Delete Role ────────────────────────────────────────────────────────────────
public sealed record DeleteRoleCommand(Guid Id) : IRequest;

public sealed class DeleteRoleCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteRoleCommand>
{
    public async Task Handle(DeleteRoleCommand request, CancellationToken ct)
    {
        var role = await db.Roles.FindAsync([request.Id], ct)
            ?? throw new EntityNotFoundException(nameof(Role), request.Id);
        if (role.IsSystemRole) throw new InvalidOperationException("Cannot delete a system role.");
        db.Roles.Remove(role);
        await db.SaveChangesAsync(ct);
    }
}

// ── Update User (Admin) ────────────────────────────────────────────────────────
public sealed record UpdateAdminUserCommand(Guid Id, string FirstName, string LastName, Guid? RoleId) : IRequest;

public sealed class UpdateAdminUserCommandValidator : AbstractValidator<UpdateAdminUserCommand>
{
    public UpdateAdminUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateAdminUserCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateAdminUserCommand>
{
    public async Task Handle(UpdateAdminUserCommand request, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([request.Id], ct)
            ?? throw new EntityNotFoundException(nameof(User), request.Id);
        user.UpdateProfile(request.FirstName, request.LastName, user.ProfilePictureUrl, user.Gender, user.DateOfBirth, request.Id);

        if (request.RoleId.HasValue)
        {
            var existingRole = await db.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == request.Id, ct);
            if (existingRole is not null)
                db.UserRoles.Remove(existingRole);
            var newRole = UserRole.Create(user.TenantId, request.Id, request.RoleId.Value, request.Id);
            db.UserRoles.Add(newRole);
        }

        await db.SaveChangesAsync(ct);
    }
}

// ── Update Tenant ──────────────────────────────────────────────────────────────
public sealed record UpdateTenantCommand(Guid Id, string Name, string? CustomDomain) : IRequest;

public sealed class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public sealed class UpdateTenantCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateTenantCommand>
{
    public async Task Handle(UpdateTenantCommand request, CancellationToken ct)
    {
        var tenant = await db.Tenants.FindAsync([request.Id], ct)
            ?? throw new EntityNotFoundException(nameof(Tenant), request.Id);
        tenant.Update(request.Name, request.CustomDomain, null, Guid.Empty);
        await db.SaveChangesAsync(ct);
    }
}
