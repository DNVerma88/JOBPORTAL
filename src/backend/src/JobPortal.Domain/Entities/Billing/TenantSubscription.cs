using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Entities.Billing;

public sealed class TenantSubscription : BaseEntity, IAggregateRoot
{
    public Guid PlanId { get; private set; }
    public SubscriptionTier Tier { get; private set; }
    public string Status { get; private set; } = "Active";
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public DateOnly? TrialEndsAt { get; private set; }
    public bool IsAutoRenew { get; private set; } = true;
    public string BillingCycle { get; private set; } = "Monthly";
    public decimal Amount { get; private set; }
    public string CurrencyCode { get; private set; } = "USD";
    public DateTimeOffset? CancelledAt { get; private set; }
    public string? CancelledReason { get; private set; }
    public string? ExternalSubscriptionId { get; private set; }

    private TenantSubscription() { }

    public static TenantSubscription Create(Guid tenantId, Guid planId, SubscriptionTier tier,
        DateOnly startDate, decimal amount, string currencyCode, string billingCycle, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            PlanId = planId,
            Tier = tier,
            StartDate = startDate,
            Amount = amount,
            CurrencyCode = currencyCode,
            BillingCycle = billingCycle,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void Cancel(string? reason, Guid modifiedBy)
    {
        Status = "Cancelled";
        CancelledAt = DateTimeOffset.UtcNow;
        CancelledReason = reason?.Trim();
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void ChangePlan(Guid newPlanId, SubscriptionTier newTier, decimal newAmount,
        string billingCycle, Guid modifiedBy)
    {
        PlanId = newPlanId;
        Tier = newTier;
        Amount = newAmount;
        BillingCycle = billingCycle;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void Reactivate(Guid modifiedBy)
    {
        Status = "Active";
        CancelledAt = null;
        CancelledReason = null;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
