using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace JobPortal.Application.Features.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Token,
    string NewPassword) : IRequest;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}

public sealed class ResetPasswordCommandHandler(
    IApplicationDbContext db,
    IPasswordService passwordService,
    IConfiguration configuration)
    : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var pepper = configuration["Security:PasswordPepper"]
            ?? throw new InvalidOperationException("PasswordPepper is not configured.");

        // Parse token: base64url(payload).base64url(signature)
        var parts = request.Token.Split('.');
        if (parts.Length != 2)
            throw new UnauthorizedDomainException("Invalid or expired password reset token.");

        byte[] payloadBytes;
        byte[] receivedSig;
        try
        {
            payloadBytes = Convert.FromBase64String(PadBase64(parts[0].Replace('-', '+').Replace('_', '/')));
            receivedSig  = Convert.FromBase64String(PadBase64(parts[1].Replace('-', '+').Replace('_', '/')));
        }
        catch
        {
            throw new UnauthorizedDomainException("Invalid or expired password reset token.");
        }

        // Verify HMAC signature
        var keyBytes = Encoding.UTF8.GetBytes(pepper);
        var expectedSig = HMACSHA256.HashData(keyBytes, payloadBytes);

        if (!CryptographicOperations.FixedTimeEquals(expectedSig, receivedSig))
            throw new UnauthorizedDomainException("Invalid or expired password reset token.");

        // Parse payload: userId|tenantId|expiresEpoch
        var payload = Encoding.UTF8.GetString(payloadBytes);
        var payloadParts = payload.Split('|');
        if (payloadParts.Length != 3
            || !Guid.TryParse(payloadParts[0], out var userId)
            || !Guid.TryParse(payloadParts[1], out var tenantId)
            || !long.TryParse(payloadParts[2], out var expiresEpoch))
        {
            throw new UnauthorizedDomainException("Invalid or expired password reset token.");
        }

        // Check expiry
        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiresEpoch)
            throw new UnauthorizedDomainException("Password reset token has expired. Please request a new one.");

        var user = await db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId && u.IsActive && !u.IsDeleted, ct)
            ?? throw new UnauthorizedDomainException("Invalid or expired password reset token.");

        var newHash = passwordService.Hash(request.NewPassword);
        user.ChangePasswordHash(newHash, userId);

        // Revoke all existing refresh tokens to force re-login on all devices
        var tokens = await db.RefreshTokens
            .IgnoreQueryFilters()
            .Where(t => t.UserId == userId && t.TenantId == tenantId && !t.IsRevoked)
            .ToListAsync(ct);

        foreach (var token in tokens)
            token.Revoke("Password reset", userId);

        await db.SaveChangesAsync(ct);
    }

    private static string PadBase64(string base64)
    {
        return (base64.Length % 4) switch
        {
            2 => base64 + "==",
            3 => base64 + "=",
            _ => base64,
        };
    }
}
