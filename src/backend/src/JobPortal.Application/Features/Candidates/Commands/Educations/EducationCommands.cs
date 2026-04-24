using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Portal;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Candidates.Commands.Educations;

// ── DTOs ──────────────────────────────────────────────────────────────────────
public sealed record EducationDto(
    Guid Id,
    string Degree,
    string FieldOfStudy,
    string Institution,
    short? StartYear,
    short? EndYear,
    bool IsCurrent,
    string? Grade,
    string? Description);

// ── Get ───────────────────────────────────────────────────────────────────────
public sealed record GetEducationsQuery : IRequest<List<EducationDto>>;

public sealed class GetEducationsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetEducationsQuery, List<EducationDto>>
{
    public async Task<List<EducationDto>> Handle(GetEducationsQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var profile = await db.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        if (profile is null) return [];

        return await db.Educations
            .Where(e => e.ProfileId == profile.Id)
            .OrderByDescending(e => e.EndYear ?? 9999)
            .Select(e => new EducationDto(e.Id, e.Degree, e.FieldOfStudy, e.Institution,
                e.StartYear, e.EndYear, e.IsCurrent, e.Grade, e.Description))
            .ToListAsync(ct);
    }
}

// ── Add ───────────────────────────────────────────────────────────────────────
public sealed record AddEducationCommand(
    string Degree,
    string FieldOfStudy,
    string Institution,
    short? StartYear,
    short? EndYear,
    bool IsCurrent,
    string? Grade,
    string? Description,
    Guid? EducationLevelId
) : IRequest<Guid>;

public sealed class AddEducationCommandValidator : AbstractValidator<AddEducationCommand>
{
    public AddEducationCommandValidator()
    {
        RuleFor(x => x.Degree).NotEmpty().MaximumLength(200);
        RuleFor(x => x.FieldOfStudy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Institution).NotEmpty().MaximumLength(300);
    }
}

public sealed class AddEducationCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<AddEducationCommand, Guid>
{
    public async Task<Guid> Handle(AddEducationCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var profile = await db.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct)
            ?? throw new KeyNotFoundException("Candidate profile not found.");

        var edu = Education.Create(
            tenantId, profile.Id,
            request.Degree, request.FieldOfStudy, request.Institution,
            request.StartYear, request.EndYear, request.IsCurrent,
            request.Grade, request.Description,
            request.EducationLevelId,
            userId);

        db.Educations.Add(edu);
        await db.SaveChangesAsync(ct);
        return edu.Id;
    }
}

// ── Delete ────────────────────────────────────────────────────────────────────
public sealed record DeleteEducationCommand(Guid Id) : IRequest;

public sealed class DeleteEducationCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<DeleteEducationCommand>
{
    public async Task Handle(DeleteEducationCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;

        var edu = await db.Educations.FirstOrDefaultAsync(e => e.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Education record not found.");

        var profile = await db.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct)
            ?? throw new UnauthorizedAccessException();
        if (edu.ProfileId != profile.Id) throw new UnauthorizedAccessException();

        edu.SoftDelete(userId);
        await db.SaveChangesAsync(ct);
    }
}
