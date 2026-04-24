using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using DomainRefreshToken = JobPortal.Domain.Entities.Auth.RefreshToken;

namespace JobPortal.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand : IRequest<RefreshTokenResponse>;

public sealed record RefreshTokenResponse(
    string AccessToken,
    DateTimeOffset ExpiresOn);

public sealed class RefreshTokenCommandHandler(
    IApplicationDbContext db,
    IJwtTokenService jwtTokenService,
    Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext)
    : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var rawToken = httpContext.HttpContext?.Request.Cookies["refreshToken"]
            ?? throw new UnauthorizedDomainException("Refresh token not found.");

        var hash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken)));

        // Find the token ignoring global query filters (tenant + soft-delete).
        // The endpoint is anonymous so no tenantId is available from context yet;
        // we derive it from the stored token itself.
        var storedToken = await db.RefreshTokens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.TokenHash == hash && !t.IsDeleted, ct)
            ?? throw new UnauthorizedDomainException("Invalid refresh token.");

        if (!storedToken.IsActive())
            throw new UnauthorizedDomainException("Refresh token has expired or been revoked.");

        var tenantId = storedToken.TenantId;

        var user = await db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == storedToken.UserId && !u.IsDeleted, ct)
            ?? throw new UnauthorizedDomainException("User not found.");

        if (!user.IsActive)
            throw new UnauthorizedDomainException("Account is inactive.");

        var roles = await db.UserRoles
            .IgnoreQueryFilters()
            .Where(ur => ur.UserId == user.Id && ur.TenantId == tenantId && !ur.IsDeleted)
            .Join(
                db.Roles.IgnoreQueryFilters(),
                ur => ur.RoleId, r => r.Id,
                (_, r) => r.Name)
            .ToListAsync(ct);

        var permissions = await db.RolePermissions
            .IgnoreQueryFilters()
            .Where(rp => roles.Contains(rp.Role!.Name) && rp.TenantId == tenantId && !rp.IsDeleted)
            .Select(rp => rp.Permission!.Name)
            .Distinct()
            .ToListAsync(ct);

        var tokenPair = jwtTokenService.GenerateTokenPair(user, roles, permissions);

        // Rotate: revoke old token, issue new one
        storedToken.Revoke("Rotated", user.Id);

        var newHash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(tokenPair.RefreshToken)));
        var newRefreshToken = DomainRefreshToken.Create(
            tenantId,
            user.Id,
            newHash,
            tokenPair.ExpiresOn.AddDays(7),
            storedToken.DeviceInfo,
            storedToken.IpAddress,
            user.Id);

        await db.RefreshTokens.AddAsync(newRefreshToken, ct);
        await db.SaveChangesAsync(ct);

        // Set the rotated refresh token in cookie
        httpContext.HttpContext!.Response.Cookies.Append("refreshToken", tokenPair.RefreshToken, new Microsoft.AspNetCore.Http.CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return new RefreshTokenResponse(tokenPair.AccessToken, tokenPair.ExpiresOn);
    }
}
