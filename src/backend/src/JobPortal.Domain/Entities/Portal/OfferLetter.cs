using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Entities.Portal;

public sealed class OfferLetter : BaseEntity, IAggregateRoot
{
    public Guid ApplicationId { get; private set; }
    public Guid OfferedBy { get; private set; }
    public DateOnly OfferDate { get; private set; }
    public DateOnly? JoiningDate { get; private set; }
    public decimal OfferSalary { get; private set; }
    public string CurrencyCode { get; private set; } = "USD";
    public string PositionTitle { get; private set; } = string.Empty;
    public string? Department { get; private set; }
    public string? Location { get; private set; }
    public string? OfferFileUrl { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public OfferLetterStatus Status { get; private set; } = OfferLetterStatus.Sent;
    public string? CandidateResponse { get; private set; }
    public DateTimeOffset? RespondedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? RevokedReason { get; private set; }

    private OfferLetter() { }

    public static OfferLetter Create(Guid tenantId, Guid applicationId, Guid offeredBy,
        DateOnly offerDate, decimal offerSalary, string currencyCode, string positionTitle,
        string? department, string? location, DateOnly? joiningDate,
        DateTimeOffset? expiresAt, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ApplicationId = applicationId,
            OfferedBy = offeredBy,
            OfferDate = offerDate,
            JoiningDate = joiningDate,
            OfferSalary = offerSalary,
            CurrencyCode = currencyCode,
            PositionTitle = positionTitle.Trim(),
            Department = department?.Trim(),
            Location = location?.Trim(),
            ExpiresAt = expiresAt,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void UpdateStatus(OfferLetterStatus status, string? candidateResponse, Guid modifiedBy)
    {
        Status = status;
        CandidateResponse = candidateResponse?.Trim();
        RespondedAt = DateTimeOffset.UtcNow;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void Update(DateOnly offerDate, DateOnly? joiningDate, decimal offerSalary,
        string currencyCode, string positionTitle, string? department,
        string? location, DateTimeOffset? expiresAt, Guid modifiedBy)
    {
        OfferDate = offerDate;
        JoiningDate = joiningDate;
        OfferSalary = offerSalary;
        CurrencyCode = currencyCode;
        PositionTitle = positionTitle.Trim();
        Department = department?.Trim();
        Location = location?.Trim();
        ExpiresAt = expiresAt;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
