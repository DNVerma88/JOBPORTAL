using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace JobPortal.Application.Features.Auth.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(string Token) : IRequest;

public sealed class VerifyEmailCommandHandler(
    IApplicationDbContext db,
    IConfiguration configuration)
    : IRequestHandler<VerifyEmailCommand>
{
    public async Task Handle(VerifyEmailCommand request, CancellationToken ct)
    {
        var pepper = configuration["Security:PasswordPepper"]
            ?? throw new InvalidOperationException("PasswordPepper is not configured.");

        var parts = request.Token.Split('.');
        if (parts.Length != 2)
            throw new UnauthorizedDomainException("Invalid or expired verification link.");

        byte[] payloadBytes;
        byte[] receivedSig;
        try
        {
            payloadBytes = Convert.FromBase64String(PadBase64(parts[0].Replace('-', '+').Replace('_', '/')));
            receivedSig  = Convert.FromBase64String(PadBase64(parts[1].Replace('-', '+').Replace('_', '/')));
        }
        catch
        {
            throw new UnauthorizedDomainException("Invalid or expired verification link.");
        }

        var keyBytes = Encoding.UTF8.GetBytes(pepper);
        var expectedSig = HMACSHA256.HashData(keyBytes, payloadBytes);
        if (!CryptographicOperations.FixedTimeEquals(expectedSig, receivedSig))
            throw new UnauthorizedDomainException("Invalid or expired verification link.");

        var payload = Encoding.UTF8.GetString(payloadBytes);
        var payloadParts = payload.Split('|');
        if (payloadParts.Length != 3
            || !Guid.TryParse(payloadParts[0], out var userId)
            || !Guid.TryParse(payloadParts[1], out var tenantId)
            || !long.TryParse(payloadParts[2], out var expiresEpoch))
        {
            throw new UnauthorizedDomainException("Invalid or expired verification link.");
        }

        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiresEpoch)
            throw new UnauthorizedDomainException("Verification link has expired. Please request a new one.");

        var user = await db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId && !u.IsDeleted, ct)
            ?? throw new UnauthorizedDomainException("Invalid verification link.");

        if (user.IsEmailVerified)
            return; // idempotent - already verified

        user.VerifyEmail(userId);
        await db.SaveChangesAsync(ct);
    }

    private static string PadBase64(string base64) =>
        (base64.Length % 4) switch { 2 => base64 + "==", 3 => base64 + "=", _ => base64 };
}

public sealed record ResendVerificationCommand(string Email) : IRequest;

public sealed class ResendVerificationCommandHandler(
    IApplicationDbContext db,
    ITenantService tenantService,
    IEmailService emailService,
    IConfiguration configuration)
    : IRequestHandler<ResendVerificationCommand>
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(24);

    public async Task Handle(ResendVerificationCommand request, CancellationToken ct)
    {
        var tenantId = tenantService.GetRequiredTenantId();
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.NormalizedEmail == normalizedEmail && u.IsActive, ct);

        if (user is null || user.IsEmailVerified)
            return; // silently succeed

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

        var verificationLink = $"{frontendUrl}/verify-email?token={Uri.EscapeDataString(token)}";

        await emailService.SendAsync(new EmailMessage(
            To: user.Email,
            Subject: "Verify your JobPortal email address",
            HtmlBody: $"""
                <p>Hi {user.FirstName},</p>
                <p>Please verify your email address by clicking the link below. It expires in 24 hours:</p>
                <p><a href="{verificationLink}">Verify Email</a></p>
                <p>If you did not create an account, you can safely ignore this email.</p>
                """), ct);
    }
}
