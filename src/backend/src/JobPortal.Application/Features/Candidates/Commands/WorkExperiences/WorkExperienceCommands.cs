using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Portal;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Candidates.Commands.WorkExperiences;

// ── DTOs ──────────────────────────────────────────────────────────────────────
public sealed record WorkExperienceDto(
    Guid Id,
    string JobTitle,
    string CompanyName,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsCurrent,
    string? Description,
    string[]? Skills);

// ── Get ───────────────────────────────────────────────────────────────────────
public sealed record GetWorkExperiencesQuery : IRequest<List<WorkExperienceDto>>;

public sealed class GetWorkExperiencesQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetWorkExperiencesQuery, List<WorkExperienceDto>>
{
    public async Task<List<WorkExperienceDto>> Handle(GetWorkExperiencesQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var profile = await db.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        if (profile is null) return [];

        return await db.WorkExperiences
            .Where(w => w.ProfileId == profile.Id)
            .OrderByDescending(w => w.StartDate)
            .Select(w => new WorkExperienceDto(w.Id, w.JobTitle, w.CompanyName,
                w.StartDate, w.EndDate, w.IsCurrent, w.Description, w.Skills))
            .ToListAsync(ct);
    }
}

// ── Add ───────────────────────────────────────────────────────────────────────
public sealed record AddWorkExperienceCommand(
    string JobTitle,
    string CompanyName,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsCurrent,
    string? Description,
    string[]? Skills,
    Guid? CityId,
    Guid? CountryId,
    Guid? IndustryId
) : IRequest<Guid>;

public sealed class AddWorkExperienceCommandValidator : AbstractValidator<AddWorkExperienceCommand>
{
    public AddWorkExperienceCommandValidator()
    {
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
    }
}

public sealed class AddWorkExperienceCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<AddWorkExperienceCommand, Guid>
{
    public async Task<Guid> Handle(AddWorkExperienceCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var profile = await db.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct)
            ?? throw new KeyNotFoundException("Candidate profile not found.");

        var exp = WorkExperience.Create(
            tenantId, profile.Id,
            request.JobTitle, request.CompanyName,
            request.StartDate, request.EndDate, request.IsCurrent,
            request.Description, request.Skills,
            request.CityId, request.CountryId, request.IndustryId,
            userId);

        db.WorkExperiences.Add(exp);
        await db.SaveChangesAsync(ct);
        return exp.Id;
    }
}

// ── Delete ────────────────────────────────────────────────────────────────────
public sealed record DeleteWorkExperienceCommand(Guid Id) : IRequest;

public sealed class DeleteWorkExperienceCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<DeleteWorkExperienceCommand>
{
    public async Task Handle(DeleteWorkExperienceCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;

        var exp = await db.WorkExperiences.FirstOrDefaultAsync(w => w.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Work experience not found.");

        // Verify ownership via profile
        var profile = await db.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct)
            ?? throw new UnauthorizedAccessException();
        if (exp.ProfileId != profile.Id) throw new UnauthorizedAccessException();

        exp.SoftDelete(userId);
        await db.SaveChangesAsync(ct);
    }
}
