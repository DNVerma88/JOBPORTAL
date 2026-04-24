using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Auth;
using JobPortal.Domain.Entities.Portal;
using JobPortal.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace JobPortal.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordService passwordService,
    IEmailService emailService,
    IConfiguration configuration)
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    // Well-known system tenant that hosts all global roles / job seekers
    private static readonly Guid SystemTenantId = new("00000000-0000-0000-0000-000000000001");

    // Well-known role IDs seeded in 07_seed_roles_permissions.sql
    private static readonly Guid JobSeekerRoleId  = new("00000009-0000-0000-0000-000000000004");
    private static readonly Guid TenantAdminRoleId = new("00000009-0000-0000-0000-000000000002");

    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(24);

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        // ── Determine the tenant for this user ────────────────────────────
        Guid tenantId;
        if (request.Role == "Recruiter")
        {
            // Recruiter → create (or reuse) their own company tenant
            tenantId = await ResolveRecruiterTenantAsync(request, cancellationToken);
        }
        else
        {
            // JobSeeker → always lives on the system tenant
            tenantId = SystemTenantId;
        }

        // ── Duplicate email check within the tenant ───────────────────────
        var emailExists = await dbContext.Users
            .AnyAsync(u => u.TenantId == tenantId && u.NormalizedEmail == normalizedEmail, cancellationToken);

        if (emailExists)
            throw new DuplicateEntityException(nameof(User), "Email");

        // ── Create user ───────────────────────────────────────────────────
        var passwordHash = passwordService.Hash(request.Password);
        var user = User.Create(tenantId, request.Email, passwordHash, request.FirstName, request.LastName, Guid.Empty);
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        // ── Assign role ───────────────────────────────────────────────────
        // Use the system-tenant role record; UserRole.TenantId scopes it to this tenant.
        var roleId = request.Role == "Recruiter" ? TenantAdminRoleId : JobSeekerRoleId;

        // Ensure the role exists on the system tenant (it's seeded there)
        var roleExists = await dbContext.Roles
            .IgnoreQueryFilters()
            .AnyAsync(r => r.Id == roleId && !r.IsDeleted, cancellationToken);

        if (roleExists)
        {
            var userRole = UserRole.Create(tenantId, user.Id, roleId, user.Id);
            dbContext.UserRoles.Add(userRole);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        // ── Create role-specific resources ────────────────────────────────
        if (request.Role == "JobSeeker")
        {
            // Auto-create an empty profile so the Profile page always has a record
            var profile = JobSeekerProfile.Create(tenantId, user.Id, user.Id);
            dbContext.JobSeekerProfiles.Add(profile);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else if (request.Role == "Recruiter")
        {
            // Auto-create the Company record for the new tenant so Company Profile page works
            var companySlug = string.IsNullOrWhiteSpace(request.TenantSlug)
                ? GenerateSlug(request.CompanyName!)
                : request.TenantSlug.Trim().ToLowerInvariant();

            var companyExists = await dbContext.Companies
                .IgnoreQueryFilters()
                .AnyAsync(c => c.TenantId == tenantId, cancellationToken);

            if (!companyExists)
            {
                var company = Company.Create(tenantId, request.CompanyName!, companySlug, user.Id);
                dbContext.Companies.Add(company);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        // ── Send verification email ───────────────────────────────────────
        try
        {
            var pepper = configuration["Security:PasswordPepper"] ?? string.Empty;
            var frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:3000";
            var expiresEpoch = DateTimeOffset.UtcNow.Add(TokenLifetime).ToUnixTimeSeconds();
            var payload = $"{user.Id}|{tenantId}|{expiresEpoch}";
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var hmac = HMACSHA256.HashData(Encoding.UTF8.GetBytes(pepper), payloadBytes);
            var token = $"{Convert.ToBase64String(payloadBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_')}" +
                        $".{Convert.ToBase64String(hmac).TrimEnd('=').Replace('+', '-').Replace('/', '_')}";
            var verifyLink = $"{frontendUrl}/verify-email?token={Uri.EscapeDataString(token)}";

            await emailService.SendAsync(new EmailMessage(
                To: user.Email,
                Subject: "Verify your JobPortal email address",
                HtmlBody: $"""
                    <p>Hi {user.FirstName},</p>
                    <p>Welcome to JobPortal! Please verify your email by clicking the link below:</p>
                    <p><a href="{verifyLink}">Verify Email</a></p>
                    <p>This link expires in 24 hours.</p>
                    """), cancellationToken);
        }
        catch
        {
            // Never fail registration because of email sending
        }

        return new RegisterResponse(user.Id, user.Email, user.FullName);
    }

    /// <summary>
    /// For Recruiter registration: create a new tenant for their company, or find an existing
    /// one by slug. Returns the resolved tenant ID.
    /// </summary>
    private async Task<Guid> ResolveRecruiterTenantAsync(RegisterCommand request, CancellationToken ct)
    {
        var companyName = request.CompanyName!.Trim();

        // Derive slug from company name if not provided
        var slug = string.IsNullOrWhiteSpace(request.TenantSlug)
            ? GenerateSlug(companyName)
            : request.TenantSlug.Trim().ToLowerInvariant();

        // Reuse existing tenant with same slug (idempotent)
        var existing = await dbContext.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Slug == slug && !t.IsDeleted, ct);

        if (existing is not null)
            return existing.Id;

        // Create a new tenant scoped to itself
        var newId = Guid.NewGuid();
        var tenant = Tenant.Create(newId, companyName, slug, request.Email, SystemTenantId /* created by system */);
        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync(ct);
        return tenant.Id;
    }

    private static string GenerateSlug(string name)
    {
        var slug = name.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s\-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-").Trim('-');
        return slug.Length > 80 ? slug[..80] : slug;
    }
}
