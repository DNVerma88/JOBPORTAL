using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Jobs.Commands.UpdateJobStatus;

public sealed record UpdateJobStatusCommand(Guid Id, JobPostingStatus Status) : IRequest;

public sealed class UpdateJobStatusCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateJobStatusCommand>
{
    public async Task Handle(UpdateJobStatusCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var job = await db.JobPostings.FirstOrDefaultAsync(j => j.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Job {request.Id} not found.");

        if (request.Status == JobPostingStatus.Published)
            job.Publish(userId);
        else
            job.UpdateStatus(request.Status, userId);

        await db.SaveChangesAsync(ct);
    }
}
