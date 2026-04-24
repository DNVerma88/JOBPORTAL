using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Events.Portal;

public sealed record ApplicationStatusChangedEvent(
    Guid ApplicationId,
    Guid TenantId,
    Guid JobSeekerId,
    Guid RecruiterId,
    ApplicationStatus OldStatus,
    ApplicationStatus NewStatus) : IDomainEvent;

public sealed record JobPostingPublishedEvent(
    Guid JobPostingId,
    Guid TenantId,
    Guid CompanyId) : IDomainEvent;

public sealed record NewApplicationReceivedEvent(
    Guid ApplicationId,
    Guid TenantId,
    Guid JobPostingId,
    Guid RecruiterId,
    Guid JobSeekerId) : IDomainEvent;
