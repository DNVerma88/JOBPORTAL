using JobPortal.Application.Common.Interfaces;

namespace JobPortal.Infrastructure.Services;

public sealed class DateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public DateOnly UtcToday => DateOnly.FromDateTime(DateTime.UtcNow);
}
