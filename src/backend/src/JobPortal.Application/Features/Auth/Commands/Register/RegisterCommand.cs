using MediatR;

namespace JobPortal.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    /// <summary>"JobSeeker" or "Recruiter". Defaults to JobSeeker.</summary>
    string Role = "JobSeeker",
    /// <summary>Required when Role = "Recruiter". Creates a new tenant.</summary>
    string? CompanyName = null,
    /// <summary>URL slug for the new company tenant. Auto-derived from CompanyName if omitted.</summary>
    string? TenantSlug = null) : IRequest<RegisterResponse>;

public sealed record RegisterResponse(Guid UserId, string Email, string FullName);
