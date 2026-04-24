using JobPortal.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Jobs.Commands.DeleteJob;

public sealed record DeleteJobCommand(Guid Id) : IRequest;

public sealed class DeleteJobCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<DeleteJobCommand>
{
    public async Task Handle(DeleteJobCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var job = await db.JobPostings.FirstOrDefaultAsync(j => j.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Job {request.Id} not found.");
        job.SoftDelete(userId);
        await db.SaveChangesAsync(ct);
    }
}
