using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Auth;
using JobPortal.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using DomainRefreshToken = JobPortal.Domain.Entities.Auth.RefreshToken;

namespace JobPortal.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordService passwordService,
    IJwtTokenService jwtTokenService,
    ITenantService tenantService)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetRequiredTenantId();
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.NormalizedEmail == normalizedEmail, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(User), request.Email);

        if (user.IsLockedOut())
            throw new UnauthorizedDomainException("Account is temporarily locked. Please try again later.");

        if (!passwordService.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin(MaxFailedAttempts, LockoutDuration);
            await dbContext.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedDomainException("Invalid email or password.");
        }

        if (!user.IsEmailVerified)
            throw new UnauthorizedDomainException("Email address has not been verified. Please check your inbox.");

        // Fetch roles and permissions for JWT claims
        var roles = await dbContext.UserRoles
            .Where(ur => ur.UserId == user.Id && ur.TenantId == tenantId)
            .Join(dbContext.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name)
            .ToListAsync(cancellationToken);

        var permissions = await dbContext.RolePermissions
            .Where(rp => roles.Contains(rp.Role!.Name) && rp.TenantId == tenantId)
            .Select(rp => rp.Permission!.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        var tokenPair = jwtTokenService.GenerateTokenPair(user, roles, permissions);

        // Hash the refresh token before storing
        var refreshTokenHash = Convert.ToHexString(
            SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(tokenPair.RefreshToken)));

        var refreshToken = DomainRefreshToken.Create(
            tenantId,
            user.Id,
            refreshTokenHash,
            tokenPair.ExpiresOn.AddDays(7),
            request.DeviceInfo,
            request.IpAddress,
            user.Id);

        // Track the session
        var sessionToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var session = UserSession.Create(
            tenantId,
            user.Id,
            sessionToken,
            deviceType: null,
            deviceInfo: request.DeviceInfo,
            ipAddress: request.IpAddress,
            userAgent: request.DeviceInfo,
            expiresAt: tokenPair.ExpiresOn.AddDays(7),
            createdBy: user.Id);

        user.RecordSuccessfulLogin();

        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await dbContext.UserSessions.AddAsync(session, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            tokenPair.AccessToken,
            tokenPair.RefreshToken,
            tokenPair.ExpiresOn,
            user.Id,
            user.FullName,
            user.Email,
            roles.AsReadOnly());
    }
}
