using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Entities.Billing;

public sealed class PaymentTransaction : BaseEntity
{
    public Guid? InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public string CurrencyCode { get; private set; } = "USD";
    public string? PaymentMethod { get; private set; }
    public string? Gateway { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string? FailureReason { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }

    private PaymentTransaction() { }
}
