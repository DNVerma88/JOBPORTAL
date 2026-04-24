using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Portal;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Jobs.Commands.CreateJob;

public sealed record CreateJobCommand(
    Guid CompanyId,
    string Title,
    string Description,
    string? Responsibilities,
    string? Requirements,
    string? NiceToHave,
    JobType JobType,
    WorkMode WorkMode,
    ExperienceLevel ExperienceLevel,
    short? MinExperienceYears,
    short? MaxExperienceYears,
    decimal? MinSalary,
    decimal? MaxSalary,
    string? CurrencyCode,
    bool IsSalaryHidden,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? CityId,
    Guid? StateId,
    Guid? CountryId,
    short OpeningsCount,
    DateTimeOffset? ExpiresAt,
    bool IsUrgent,
    bool IsFeatured,
    bool IsRemote,
    string? ApplicationEmail,
    string? ApplicationUrl
) : IRequest<Guid>;

public sealed class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.OpeningsCount).GreaterThan((short)0);
    }
}

public sealed class CreateJobCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateJobCommand, Guid>
{
    public async Task<Guid> Handle(CreateJobCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var slug = GenerateSlug(request.Title);
        // Ensure slug uniqueness
        var count = await db.JobPostings.CountAsync(j => j.Slug.StartsWith(slug), ct);
        if (count > 0) slug = $"{slug}-{count}";

        var job = JobPosting.Create(tenantId, request.CompanyId, request.Title, slug,
            request.Description, request.JobType, request.WorkMode, request.ExperienceLevel, userId);

        job.Update(request.Title, request.Description, request.Responsibilities, request.Requirements,
            request.NiceToHave, request.JobType, request.WorkMode, request.ExperienceLevel,
            request.MinExperienceYears, request.MaxExperienceYears, request.MinSalary, request.MaxSalary,
            request.CurrencyCode, request.IsSalaryHidden, request.CategoryId, request.SubCategoryId,
            request.CityId, request.StateId, request.CountryId, request.OpeningsCount,
            request.ExpiresAt, request.IsUrgent, request.IsFeatured, request.IsRemote,
            request.ApplicationEmail, request.ApplicationUrl, userId);

        db.JobPostings.Add(job);
        await db.SaveChangesAsync(ct);
        return job.Id;
    }

    private static string GenerateSlug(string title) =>
        System.Text.RegularExpressions.Regex.Replace(
            title.Trim().ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
}
