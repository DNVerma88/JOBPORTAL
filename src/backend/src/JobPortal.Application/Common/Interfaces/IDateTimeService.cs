namespace JobPortal.Application.Common.Interfaces;

public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }
    DateOnly UtcToday { get; }
}
