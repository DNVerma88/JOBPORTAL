using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Entities.Auth;

/// <summary>
/// Represents a SaaS tenant (company / agency using the platform).
/// </summary>
public sealed class Tenant : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty; // subdomain
    public string? CustomDomain { get; private set; }
    public string? LogoUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset? TrialEndsOn { get; private set; }
    public SubscriptionTier SubscriptionTier { get; private set; } = SubscriptionTier.Free;
    public string ContactEmail { get; private set; } = string.Empty;
    public string? ContactPhone { get; private set; }
    public string? Address { get; private set; }
    public string? Country { get; private set; }

    private Tenant() { } // EF Core

    public static Tenant Create(Guid tenantId, string name, string slug, string contactEmail, Guid createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(contactEmail);

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId, // self-referential for super-admin isolation
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            ContactEmail = contactEmail.Trim().ToLowerInvariant(),
            SubscriptionTier = SubscriptionTier.Free,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

        return tenant;
    }

    public void Update(string name, string? customDomain, string? logoUrl, Guid modifiedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        CustomDomain = customDomain?.Trim();
        LogoUrl = logoUrl?.Trim();
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void Deactivate(Guid modifiedBy)
    {
        IsActive = false;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
