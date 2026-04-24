using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Portal;

public sealed class Company : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? LogoUrl { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string? Description { get; private set; }
    public string? TagLine { get; private set; }
    public int? FoundedYear { get; private set; }
    public string? CompanySize { get; private set; }
    public Guid? IndustryId { get; private set; }
    public Guid? CountryId { get; private set; }
    public Guid? CityId { get; private set; }
    public string? HeadquartersAddress { get; private set; }
    public bool IsVerified { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? LinkedInUrl { get; private set; }
    public string? TwitterUrl { get; private set; }
    public string? GlassdoorUrl { get; private set; }
    public decimal? AverageRating { get; private set; }
    public int TotalReviews { get; private set; }
    public int TotalFollowers { get; private set; }

    private Company() { }

    public static Company Create(Guid tenantId, string name, string slug, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void Update(string name, string? description, string? tagLine, string? websiteUrl,
        string? logoUrl, string? coverImageUrl, int? foundedYear, string? companySize,
        Guid? industryId, Guid? countryId, Guid? cityId, string? headquartersAddress,
        string? linkedInUrl, string? twitterUrl, string? glassdoorUrl, Guid modifiedBy)
    {
        Name = name.Trim();
        Description = description?.Trim();
        TagLine = tagLine?.Trim();
        WebsiteUrl = websiteUrl?.Trim();
        LogoUrl = logoUrl;
        CoverImageUrl = coverImageUrl;
        FoundedYear = foundedYear;
        CompanySize = companySize;
        IndustryId = industryId;
        CountryId = countryId;
        CityId = cityId;
        HeadquartersAddress = headquartersAddress?.Trim();
        LinkedInUrl = linkedInUrl?.Trim();
        TwitterUrl = twitterUrl?.Trim();
        GlassdoorUrl = glassdoorUrl?.Trim();
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
