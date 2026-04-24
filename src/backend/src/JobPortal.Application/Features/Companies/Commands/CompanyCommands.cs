using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Portal;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Companies.Commands;

public sealed record UpdateMyCompanyCommand(
    string Name,
    string? Description,
    string? TagLine,
    string? WebsiteUrl,
    string? LogoUrl,
    string? CoverImageUrl,
    int? FoundedYear,
    string? CompanySize,
    Guid? IndustryId,
    Guid? CountryId,
    Guid? CityId,
    string? HeadquartersAddress,
    string? LinkedInUrl,
    string? TwitterUrl,
    string? GlassdoorUrl
) : IRequest<Guid>;

public sealed class UpdateMyCompanyCommandValidator : AbstractValidator<UpdateMyCompanyCommand>
{
    public UpdateMyCompanyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public sealed class UpdateMyCompanyCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateMyCompanyCommand, Guid>
{
    public async Task<Guid> Handle(UpdateMyCompanyCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var company = await db.Companies.FirstOrDefaultAsync(c => c.TenantId == tenantId, ct);
        if (company is null)
        {
            var slug = request.Name.Trim().ToLowerInvariant().Replace(" ", "-");
            company = Company.Create(tenantId, request.Name, slug, userId);
            db.Companies.Add(company);
        }

        company.Update(request.Name, request.Description, request.TagLine, request.WebsiteUrl,
            request.LogoUrl, request.CoverImageUrl, request.FoundedYear, request.CompanySize,
            request.IndustryId, request.CountryId, request.CityId, request.HeadquartersAddress,
            request.LinkedInUrl, request.TwitterUrl, request.GlassdoorUrl, userId);

        await db.SaveChangesAsync(ct);
        return company.Id;
    }
}
