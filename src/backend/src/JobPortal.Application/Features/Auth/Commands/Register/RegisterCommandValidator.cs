using FluentValidation;

namespace JobPortal.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(128)
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("First name contains invalid characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Last name contains invalid characters.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{6,14}$").WithMessage("Phone number is invalid.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Role)
            .Must(r => r == "JobSeeker" || r == "Recruiter")
            .WithMessage("Role must be 'JobSeeker' or 'Recruiter'.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required for Recruiter registration.")
            .MaximumLength(200)
            .When(x => x.Role == "Recruiter");

        RuleFor(x => x.TenantSlug)
            .MaximumLength(100)
            .Matches(@"^[a-z0-9\-]+$").WithMessage("Slug may only contain lowercase letters, numbers, and hyphens.")
            .When(x => x.Role == "Recruiter" && !string.IsNullOrWhiteSpace(x.TenantSlug));
    }
}
