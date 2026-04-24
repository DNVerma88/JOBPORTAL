using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Companies.Queries;

// ── List Companies ────────────────────────────────────────────────────────────
public sealed record GetCompaniesQuery(int PageNumber = 1, int PageSize = 20, string? Search = null)
    : IRequest<PagedList<CompanySummaryDto>>;

public sealed record CompanySummaryDto(
    Guid Id, string Name, string Slug, string? LogoUrl, string? WebsiteUrl,
    string? CompanySize, bool IsVerified, bool IsActive, int TotalReviews, int TotalFollowers,
    DateTimeOffset CreatedOn);

public sealed class GetCompaniesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetCompaniesQuery, PagedList<CompanySummaryDto>>
{
    public async Task<PagedList<CompanySummaryDto>> Handle(GetCompaniesQuery request, CancellationToken ct)
    {
        var query = db.Companies.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Name.Contains(request.Search));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(c => c.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CompanySummaryDto(c.Id, c.Name, c.Slug, c.LogoUrl, c.WebsiteUrl,
                c.CompanySize, c.IsVerified, c.IsActive, c.TotalReviews, c.TotalFollowers, c.CreatedOn))
            .ToListAsync(ct);

        return PagedList<CompanySummaryDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

// ── Get My Company ────────────────────────────────────────────────────────────
public sealed record GetMyCompanyQuery : IRequest<CompanyDetailDto?>;

public sealed record CompanyDetailDto(
    Guid Id, string Name, string Slug, string? LogoUrl, string? CoverImageUrl,
    string? WebsiteUrl, string? Description, string? TagLine, int? FoundedYear,
    string? CompanySize, Guid? IndustryId, Guid? CountryId, Guid? CityId,
    string? HeadquartersAddress, bool IsVerified, bool IsActive,
    string? LinkedInUrl, string? TwitterUrl, string? GlassdoorUrl,
    string? IndustryName);

public sealed class GetMyCompanyQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetMyCompanyQuery, CompanyDetailDto?>
{
    public async Task<CompanyDetailDto?> Handle(GetMyCompanyQuery request, CancellationToken ct)
    {
        var tenantId = currentUser.TenantId ?? Guid.Empty;
        var company = await db.Companies.FirstOrDefaultAsync(c => c.TenantId == tenantId, ct);
        if (company is null) return null;

        string? industryName = null;
        if (company.IndustryId.HasValue)
            industryName = await db.Industries.Where(i => i.Id == company.IndustryId.Value).Select(i => i.Name).FirstOrDefaultAsync(ct);

        return new CompanyDetailDto(company.Id, company.Name, company.Slug, company.LogoUrl,
            company.CoverImageUrl, company.WebsiteUrl, company.Description, company.TagLine,
            company.FoundedYear, company.CompanySize, company.IndustryId, company.CountryId,
            company.CityId, company.HeadquartersAddress, company.IsVerified, company.IsActive,
            company.LinkedInUrl, company.TwitterUrl, company.GlassdoorUrl, industryName);
    }
}

// ── Get Company By Id ────────────────────────────────────────────────────────────────
public sealed record GetCompanyByIdQuery(Guid Id) : IRequest<CompanyDetailDto?>;

public sealed class GetCompanyByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetCompanyByIdQuery, CompanyDetailDto?>
{
    public async Task<CompanyDetailDto?> Handle(GetCompanyByIdQuery request, CancellationToken ct)
    {
        var company = await db.Companies.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (company is null) return null;

        string? industryName = null;
        if (company.IndustryId.HasValue)
            industryName = await db.Industries.Where(i => i.Id == company.IndustryId.Value).Select(i => i.Name).FirstOrDefaultAsync(ct);

        return new CompanyDetailDto(company.Id, company.Name, company.Slug, company.LogoUrl,
            company.CoverImageUrl, company.WebsiteUrl, company.Description, company.TagLine,
            company.FoundedYear, company.CompanySize, company.IndustryId, company.CountryId,
            company.CityId, company.HeadquartersAddress, company.IsVerified, company.IsActive,
            company.LinkedInUrl, company.TwitterUrl, company.GlassdoorUrl, industryName);
    }
}
