namespace JobPortal.Application.Common.Interfaces;

public record EmailMessage(
    string To,
    string Subject,
    string HtmlBody,
    string? PlainTextBody = null,
    IReadOnlyList<string>? Cc = null);

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
    Task SendTemplatedAsync(string templateKey, string to, IDictionary<string, string> variables, CancellationToken cancellationToken = default);
}
