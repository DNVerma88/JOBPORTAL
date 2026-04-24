using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Enums;
using JobPortal.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Auth.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(
    string FirstName,
    string LastName,
    Gender Gender,
    DateOnly? DateOfBirth,
    string? ProfilePictureUrl
) : IRequest;

public sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateProfileCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateProfileCommand>
{
    public async Task Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var userId = currentUser.GetRequiredUserId();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new EntityNotFoundException("User", userId);

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.ProfilePictureUrl,
            request.Gender,
            request.DateOfBirth,
            userId);

        await db.SaveChangesAsync(ct);
    }
}
