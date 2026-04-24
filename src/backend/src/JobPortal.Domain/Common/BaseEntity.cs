namespace JobPortal.Domain.Common;

/// <summary>
/// Base entity with full SaaS audit + multi-tenant + optimistic concurrency support.
/// All domain entities must inherit from this class.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    // ── Multi-tenancy ──────────────────────────────────────────────────────
    public Guid TenantId { get; set; }

    // ── Audit ──────────────────────────────────────────────────────────────
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }

    // ── Soft Delete ────────────────────────────────────────────────────────
    public bool IsDeleted { get; set; }
    public Guid? DeletedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }

    // ── Optimistic Concurrency (maps to PostgreSQL xmin syscolumn) ─────────
    public uint RecordVersion { get; set; }

    // ── Domain Events ──────────────────────────────────────────────────────
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public void SoftDelete(Guid deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedOn = DateTimeOffset.UtcNow;
    }
}
