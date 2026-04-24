using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword) : IRequest;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        RuleFor(x => x).Must(x => x.CurrentPassword != x.NewPassword)
            .WithMessage("New password must differ from the current password.");
    }
}

public sealed class ChangePasswordCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IPasswordService passwordService)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = currentUser.GetRequiredUserId();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new EntityNotFoundException("User", userId);

        if (!passwordService.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedDomainException("Current password is incorrect.");

        var newHash = passwordService.Hash(request.NewPassword);
        user.ChangePasswordHash(newHash, userId);

        // Revoke all existing refresh tokens to force re-login on other devices
        var tokens = await db.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ToListAsync(ct);

        foreach (var token in tokens)
            token.Revoke("Password changed", userId);

        await db.SaveChangesAsync(ct);
    }
}
