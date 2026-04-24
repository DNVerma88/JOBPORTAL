using JobPortal.Domain.Entities.Auth;

namespace JobPortal.Application.Common.Interfaces;

public record TokenPair(string AccessToken, string RefreshToken, DateTimeOffset ExpiresOn);

public interface IJwtTokenService
{
    TokenPair GenerateTokenPair(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions);
    bool ValidateToken(string token, out Guid userId, out Guid tenantId);
}

public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
