using JobPortal.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Jobs.Queries.GetJob;

public sealed record GetJobQuery(Guid Id) : IRequest<JobDetailDto?>;

public sealed record JobDetailDto(
    Guid Id,
    string Title,
    string Slug,
    Guid CompanyId,
    string? CompanyName,
    string Description,
    string? Responsibilities,
    string? Requirements,
    string? NiceToHave,
    string JobType,
    string WorkMode,
    string ExperienceLevel,
    short? MinExperienceYears,
    short? MaxExperienceYears,
    decimal? MinSalary,
    decimal? MaxSalary,
    string? CurrencyCode,
    bool IsSalaryHidden,
    Guid? CategoryId,
    Guid? CountryId,
    Guid? CityId,
    string Status,
    int OpeningsCount,
    int ApplicationsCount,
    bool IsUrgent,
    bool IsFeatured,
    bool IsRemote,
    string? ApplicationEmail,
    string? ApplicationUrl,
    DateTimeOffset? PublishedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset CreatedOn
);

public sealed class GetJobQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetJobQuery, JobDetailDto?>
{
    public async Task<JobDetailDto?> Handle(GetJobQuery request, CancellationToken ct)
    {
        var job = await db.JobPostings.FirstOrDefaultAsync(j => j.Id == request.Id, ct);
        if (job is null) return null;

        var company = await db.Companies.FirstOrDefaultAsync(c => c.Id == job.CompanyId, ct);

        return new JobDetailDto(
            job.Id, job.Title, job.Slug, job.CompanyId, company?.Name,
            job.Description, job.Responsibilities, job.Requirements, job.NiceToHave,
            job.JobType.ToString(), job.WorkMode.ToString(), job.ExperienceLevel.ToString(),
            job.MinExperienceYears, job.MaxExperienceYears,
            job.MinSalary, job.MaxSalary, job.CurrencyCode, job.IsSalaryHidden,
            job.CategoryId, job.CountryId, job.CityId,
            job.Status.ToString(), job.OpeningsCount, job.ApplicationsCount,
            job.IsUrgent, job.IsFeatured, job.IsRemote,
            job.ApplicationEmail, job.ApplicationUrl,
            job.PublishedAt, job.ExpiresAt, job.CreatedOn);
    }
}
