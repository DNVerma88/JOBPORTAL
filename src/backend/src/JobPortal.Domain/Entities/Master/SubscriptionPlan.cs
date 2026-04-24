namespace JobPortal.Domain.Entities.Master;

/// <summary>Master entity — no TenantId, no soft-delete; uses direct table query.</summary>
public sealed class SubscriptionPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Tier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PriceMonthly { get; set; }
    public decimal PriceAnnually { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int? MaxJobPostings { get; set; }
    public int? MaxUsers { get; set; }
    public int? MaxResumeViews { get; set; }
    public int JobPostingDurationDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
}
