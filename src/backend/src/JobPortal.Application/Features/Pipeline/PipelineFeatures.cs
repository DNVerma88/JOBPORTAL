using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Portal;
using JobPortal.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Pipeline.PipelineFeatures;

// ── Get Pipeline (stages + candidates for a job) ──────────────────────────────
public sealed record GetPipelineQuery(Guid JobPostingId) : IRequest<PipelineResponse>;

public sealed record PipelineStageDto(Guid Id, string Name, string? Color, short SortOrder, bool IsDefault, List<PipelineCandidateDto> Candidates);

public sealed record PipelineCandidateDto(
    Guid ApplicationId, Guid ApplicantId, string? ApplicantName, string Status,
    Guid? CurrentStageId, DateTimeOffset CreatedOn);

public sealed record PipelineResponse(List<PipelineStageDto> Stages);

public sealed class GetPipelineQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPipelineQuery, PipelineResponse>
{
    public async Task<PipelineResponse> Handle(GetPipelineQuery request, CancellationToken ct)
    {
        var stages = await db.CandidatePipelineStages
            .Where(s => s.JobPostingId == request.JobPostingId)
            .OrderBy(s => s.SortOrder)
            .ToListAsync(ct);

        var applications = await db.JobApplications
            .Where(a => a.JobPostingId == request.JobPostingId)
            .ToListAsync(ct);

        var userIds = applications.Select(a => a.ApplicantId).Distinct().ToList();
        var users = await db.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, Name = u.FirstName + " " + u.LastName })
            .ToListAsync(ct);
        var userMap = users.ToDictionary(u => u.Id, u => u.Name);

        // Get latest pipeline entry per application for current stage
        var appIds = applications.Select(a => a.Id).ToList();
        var pipelines = await db.CandidatePipelines
            .Where(p => appIds.Contains(p.ApplicationId))
            .OrderByDescending(p => p.MovedAt)
            .ToListAsync(ct);

        var latestStagePerApp = pipelines
            .GroupBy(p => p.ApplicationId)
            .ToDictionary(g => g.Key, g => g.First().StageId);

        var result = stages.Select(stage => new PipelineStageDto(
            stage.Id, stage.Name, stage.Color, stage.SortOrder, stage.IsDefault,
            applications
                .Where(a => latestStagePerApp.TryGetValue(a.Id, out var sid) ? sid == stage.Id : stage.IsDefault)
                .Select(a => new PipelineCandidateDto(
                    a.Id, a.ApplicantId,
                    userMap.TryGetValue(a.ApplicantId, out var name) ? name : null,
                    a.Status.ToString(),
                    latestStagePerApp.TryGetValue(a.Id, out var stageId) ? stageId : null,
                    a.CreatedOn))
                .ToList()
        )).ToList();

        return new PipelineResponse(result);
    }
}

// ── Move Candidate to Stage ────────────────────────────────────────────────────
public sealed record MoveCandidateStageCommand(Guid ApplicationId, Guid StageId, string? Notes) : IRequest;

public sealed class MoveCandidateStageCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<MoveCandidateStageCommand>
{
    public async Task Handle(MoveCandidateStageCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var pipeline = CandidatePipeline.Create(tenantId, request.ApplicationId,
            request.StageId, userId, request.Notes, userId);

        db.CandidatePipelines.Add(pipeline);
        await db.SaveChangesAsync(ct);
    }
}

// ── Create Pipeline Stage ─────────────────────────────────────────────────────────
public sealed record CreatePipelineStageCommand(
    Guid JobPostingId,
    string Name,
    string? Color,
    short SortOrder,
    bool IsDefault = false
) : IRequest<Guid>;

public sealed class CreatePipelineStageCommandValidator : AbstractValidator<CreatePipelineStageCommand>
{
    public CreatePipelineStageCommandValidator()
    {
        RuleFor(x => x.JobPostingId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Color).MaximumLength(20).When(x => x.Color != null);
    }
}

public sealed class CreatePipelineStageCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<CreatePipelineStageCommand, Guid>
{
    public async Task<Guid> Handle(CreatePipelineStageCommand request, CancellationToken ct)
    {
        var userId = currentUser.GetRequiredUserId();
        var tenantId = currentUser.GetRequiredTenantId();

        var stage = CandidatePipelineStage.Create(tenantId, request.JobPostingId, request.Name,
            request.Color, request.SortOrder, request.IsDefault, userId);

        db.CandidatePipelineStages.Add(stage);
        await db.SaveChangesAsync(ct);
        return stage.Id;
    }
}

// ── Update Pipeline Stage ─────────────────────────────────────────────────────────
public sealed record UpdatePipelineStageCommand(
    Guid Id,
    string Name,
    string? Color,
    short SortOrder
) : IRequest;

public sealed class UpdatePipelineStageCommandValidator : AbstractValidator<UpdatePipelineStageCommand>
{
    public UpdatePipelineStageCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Color).MaximumLength(20).When(x => x.Color != null);
    }
}

public sealed class UpdatePipelineStageCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdatePipelineStageCommand>
{
    public async Task Handle(UpdatePipelineStageCommand request, CancellationToken ct)
    {
        var userId = currentUser.GetRequiredUserId();
        var stage = await db.CandidatePipelineStages.FirstOrDefaultAsync(s => s.Id == request.Id, ct)
            ?? throw new EntityNotFoundException(nameof(CandidatePipelineStage), request.Id);

        stage.Update(request.Name, request.Color, request.SortOrder, userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Delete Pipeline Stage ─────────────────────────────────────────────────────────
public sealed record DeletePipelineStageCommand(Guid Id) : IRequest;

public sealed class DeletePipelineStageCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<DeletePipelineStageCommand>
{
    public async Task Handle(DeletePipelineStageCommand request, CancellationToken ct)
    {
        var userId = currentUser.GetRequiredUserId();
        var stage = await db.CandidatePipelineStages.FirstOrDefaultAsync(s => s.Id == request.Id, ct)
            ?? throw new EntityNotFoundException(nameof(CandidatePipelineStage), request.Id);

        stage.SoftDelete(userId);
        await db.SaveChangesAsync(ct);
    }
}
