using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Portal;
using MediatR;

namespace JobPortal.Application.Features.Applications.Commands.ApplyForJob;

public sealed record ApplyForJobCommand(
    Guid JobPostingId,
    Guid? ResumeId,
    string? CoverLetter,
    decimal? ExpectedSalary,
    string? AppliedVia
) : IRequest<Guid>;

public sealed class ApplyForJobCommandValidator : AbstractValidator<ApplyForJobCommand>
{
    public ApplyForJobCommandValidator()
    {
        RuleFor(x => x.JobPostingId).NotEmpty();
    }
}

public sealed class ApplyForJobCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<ApplyForJobCommand, Guid>
{
    public async Task<Guid> Handle(ApplyForJobCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var application = JobApplication.Create(
            tenantId, request.JobPostingId, userId,
            null, request.ResumeId, request.CoverLetter,
            request.ExpectedSalary, request.AppliedVia ?? "Portal", userId);

        db.JobApplications.Add(application);
        await db.SaveChangesAsync(ct);
        return application.Id;
    }
}
