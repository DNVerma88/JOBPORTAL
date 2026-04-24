using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Billing;

public sealed class JobCredit : BaseEntity
{
    public int TotalCredits { get; private set; }
    public int UsedCredits { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public string? Source { get; private set; }
    public Guid? InvoiceId { get; private set; }

    public int AvailableCredits => TotalCredits - UsedCredits;

    private JobCredit() { }

    public static JobCredit Create(Guid tenantId, int totalCredits, string? source,
        Guid? invoiceId, DateTimeOffset? expiresAt, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            TotalCredits = totalCredits,
            Source = source,
            InvoiceId = invoiceId,
            ExpiresAt = expiresAt,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public bool ConsumeCredit(Guid modifiedBy)
    {
        if (AvailableCredits <= 0) return false;
        UsedCredits++;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
        return true;
    }

    public void AddCredits(int quantity, Guid modifiedBy)
    {
        TotalCredits += quantity;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
