using MediatR;

namespace JobPortal.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password,
    string? DeviceInfo,
    string? IpAddress) : IRequest<LoginResponse>;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresOn,
    Guid UserId,
    string FullName,
    string Email,
    IReadOnlyList<string> Roles);
