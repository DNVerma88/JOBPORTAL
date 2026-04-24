using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JobPortal.Infrastructure.Identity;

/// <summary>
/// JWT token service using RS256 (asymmetric RSA signing).
/// Keys loaded from configuration (PEM format or file paths).
/// </summary>
public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    private const int AccessTokenExpiryMinutes = 15;

    public TokenPair GenerateTokenPair(
        User user,
        IReadOnlyList<string> roles,
        IReadOnlyList<string> permissions)
    {
        var privateKeyPem = configuration["Jwt:PrivateKey"]
            ?? throw new InvalidOperationException("Jwt:PrivateKey is not configured.");

        var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);

        var signingCredentials = new SigningCredentials(
            new RsaSecurityKey(rsa) { KeyId = "job-portal-key-1" },
            SecurityAlgorithms.RsaSha256);

        var claims = BuildClaims(user, roles, permissions);
        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(AccessTokenExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresOn.UtcDateTime,
            signingCredentials: signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = GenerateRefreshToken();

        return new TokenPair(accessToken, refreshToken, expiresOn);
    }

    public bool ValidateToken(string token, out Guid userId, out Guid tenantId)
    {
        userId = Guid.Empty;
        tenantId = Guid.Empty;

        try
        {
            var publicKeyPem = configuration["Jwt:PublicKey"]
                ?? throw new InvalidOperationException("Jwt:PublicKey is not configured.");

            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParams, out _);

            var userIdStr = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var tenantIdStr = principal.FindFirstValue("tenant_id");

            if (!Guid.TryParse(userIdStr, out userId)) return false;
            if (!Guid.TryParse(tenantIdStr, out tenantId)) return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static List<Claim> BuildClaims(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new("tenant_id", user.TenantId.ToString()),
            new("given_name", user.FirstName),
            new("family_name", user.LastName)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        foreach (var permission in permissions)
            claims.Add(new Claim("permission", permission));

        return claims;
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
