using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Entities.Billing;

public sealed class SubscriptionInvoice : BaseEntity
{
    public Guid SubscriptionId { get; private set; }
    public string InvoiceNumber { get; private set; } = string.Empty;
    public DateOnly BillingPeriodStart { get; private set; }
    public DateOnly BillingPeriodEnd { get; private set; }
    public decimal Amount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string CurrencyCode { get; private set; } = "USD";
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public DateOnly DueDate { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }
    public string? ExternalInvoiceId { get; private set; }
    public string? InvoiceFileUrl { get; private set; }
    public string? Notes { get; private set; }

    private SubscriptionInvoice() { }

    public static SubscriptionInvoice Create(Guid tenantId, Guid subscriptionId, string invoiceNumber,
        DateOnly periodStart, DateOnly periodEnd, decimal amount, decimal taxAmount,
        string currencyCode, DateOnly dueDate, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SubscriptionId = subscriptionId,
            InvoiceNumber = invoiceNumber,
            BillingPeriodStart = periodStart,
            BillingPeriodEnd = periodEnd,
            Amount = amount,
            TaxAmount = taxAmount,
            TotalAmount = amount + taxAmount,
            CurrencyCode = currencyCode,
            DueDate = dueDate,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void MarkPaid(Guid modifiedBy)
    {
        Status = PaymentStatus.Completed;
        PaidAt = DateTimeOffset.UtcNow;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
