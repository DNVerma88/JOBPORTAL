using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace JobPortal.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}

public sealed class ForgotPasswordCommandHandler(
    IApplicationDbContext db,
    ITenantService tenantService,
    IEmailService emailService,
    IConfiguration configuration)
    : IRequestHandler<ForgotPasswordCommand>
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(15);

    public async Task Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var tenantId = tenantService.GetRequiredTenantId();
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        // Silently succeed if user not found to prevent email enumeration
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.NormalizedEmail == normalizedEmail && u.IsActive, ct);

        if (user is null)
            return;

        var pepper = configuration["Security:PasswordPepper"]
            ?? throw new InvalidOperationException("PasswordPepper is not configured.");
        var frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:3000";

        var expiresEpoch = DateTimeOffset.UtcNow.Add(TokenLifetime).ToUnixTimeSeconds();
        var payload = $"{user.Id}|{tenantId}|{expiresEpoch}";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var keyBytes = Encoding.UTF8.GetBytes(pepper);

        var hmac = HMACSHA256.HashData(keyBytes, payloadBytes);

        var token = $"{Convert.ToBase64String(payloadBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_')}" +
                    $".{Convert.ToBase64String(hmac).TrimEnd('=').Replace('+', '-').Replace('/', '_')}";

        var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(token)}";

        var emailMessage = new EmailMessage(
            To: user.Email,
            Subject: "Reset your JobPortal password",
            HtmlBody: $"""
                <p>Hi {user.FirstName},</p>
                <p>We received a request to reset your password. Click the link below within 15 minutes:</p>
                <p><a href="{resetLink}">Reset Password</a></p>
                <p>If you did not request this, you can safely ignore this email.</p>
                """);

        await emailService.SendAsync(emailMessage, ct);
    }
}
