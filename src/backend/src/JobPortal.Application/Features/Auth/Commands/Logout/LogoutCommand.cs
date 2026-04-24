using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace JobPortal.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand : IRequest;

public sealed class LogoutCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext)
    : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var userId = currentUser.GetRequiredUserId();

        // Try to revoke the specific refresh token from the cookie
        var rawToken = httpContext.HttpContext?.Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(rawToken))
        {
            var hash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken)));
            var token = await db.RefreshTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.TokenHash == hash && !t.IsRevoked, ct);
            if (token is not null)
                token.Revoke("Logout", userId);
        }

        await db.SaveChangesAsync(ct);
    }
}
