using JobPortal.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace JobPortal.Infrastructure.Services;

public sealed class EmailService(
    IConfiguration configuration,
    ILogger<EmailService> logger) : IEmailService
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(
            configuration["Email:SenderName"] ?? "JobPortal",
            configuration["Email:SenderAddress"] ?? "noreply@jobportal.com"));
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));
        mimeMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message.HtmlBody,
            TextBody = message.PlainTextBody
        };
        mimeMessage.Body = bodyBuilder.ToMessageBody();

        if (message.Cc is { Count: > 0 })
        {
            foreach (var cc in message.Cc)
                mimeMessage.Cc.Add(MailboxAddress.Parse(cc));
        }

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(
                configuration["Email:SmtpHost"] ?? "localhost",
                int.Parse(configuration["Email:SmtpPort"] ?? "587"),
                SecureSocketOptions.StartTlsWhenAvailable,
                cancellationToken);

            var user = configuration["Email:SmtpUser"];
            var pass = configuration["Email:SmtpPassword"];
            if (!string.IsNullOrWhiteSpace(user))
                await client.AuthenticateAsync(user, pass, cancellationToken);

            await client.SendAsync(mimeMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {To} with subject {Subject}", message.To, message.Subject);
            throw;
        }
    }

    public async Task SendTemplatedAsync(
        string templateKey,
        string to,
        IDictionary<string, string> variables,
        CancellationToken cancellationToken = default)
    {
        // Template resolution will be wired to DB in a future phase.
        // For now, build a simple message.
        var subject = templateKey.Replace("-", " ").ToUpperInvariant();
        var body = variables.Aggregate(
            $"<p>Template: {templateKey}</p>",
            (current, kv) => current + $"<p>{kv.Key}: {kv.Value}</p>");

        await SendAsync(new EmailMessage(to, subject, body), cancellationToken);
    }
}
