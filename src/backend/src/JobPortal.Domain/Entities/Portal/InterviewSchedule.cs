using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Entities.Portal;

public sealed class InterviewSchedule : BaseEntity, IAggregateRoot
{
    public Guid ApplicationId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTimeOffset ScheduledAt { get; private set; }
    public short DurationMinutes { get; private set; } = 60;
    public InterviewType InterviewType { get; private set; }
    public string? MeetingLink { get; private set; }
    public string? Location { get; private set; }
    public bool IsConfirmed { get; private set; }
    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public string? CancelledReason { get; private set; }
    public DateTimeOffset? ReminderSentAt { get; private set; }

    private InterviewSchedule() { }

    public static InterviewSchedule Create(Guid tenantId, Guid applicationId, string title,
        DateTimeOffset scheduledAt, short durationMinutes, InterviewType type,
        string? meetingLink, string? location, string? description, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ApplicationId = applicationId,
            Title = title.Trim(),
            Description = description?.Trim(),
            ScheduledAt = scheduledAt,
            DurationMinutes = durationMinutes,
            InterviewType = type,
            MeetingLink = meetingLink?.Trim(),
            Location = location?.Trim(),
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void UpdateStatus(bool isCancelled, string? cancelledReason, Guid modifiedBy)
    {
        IsCancelled = isCancelled;
        if (isCancelled)
        {
            CancelledAt = DateTimeOffset.UtcNow;
            CancelledReason = cancelledReason?.Trim();
        }
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void Reschedule(DateTimeOffset scheduledAt, short durationMinutes,
        string? meetingLink, string? location, Guid modifiedBy)
    {
        ScheduledAt = scheduledAt;
        DurationMinutes = durationMinutes;
        MeetingLink = meetingLink?.Trim();
        Location = location?.Trim();
        IsCancelled = false;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
