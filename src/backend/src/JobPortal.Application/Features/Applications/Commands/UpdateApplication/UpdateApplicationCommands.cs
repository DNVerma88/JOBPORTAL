using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Applications.Commands.WithdrawApplication;

public sealed record WithdrawApplicationCommand(Guid Id) : IRequest;

public sealed class WithdrawApplicationCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<WithdrawApplicationCommand>
{
    public async Task Handle(WithdrawApplicationCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var application = await db.JobApplications.FirstOrDefaultAsync(a => a.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Application {request.Id} not found.");

        application.Withdraw(userId);
        await db.SaveChangesAsync(ct);
    }
}

public sealed record UpdateApplicationStatusCommand(Guid Id, ApplicationStatus Status) : IRequest;

public sealed class UpdateApplicationStatusCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateApplicationStatusCommand>
{
    public async Task Handle(UpdateApplicationStatusCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var application = await db.JobApplications.FirstOrDefaultAsync(a => a.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Application {request.Id} not found.");

        application.UpdateStatus(request.Status, userId);
        await db.SaveChangesAsync(ct);
    }
}
