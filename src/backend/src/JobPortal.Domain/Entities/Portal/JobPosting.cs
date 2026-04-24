using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Entities.Portal;

public sealed class JobPosting : BaseEntity, IAggregateRoot
{
    public Guid CompanyId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Responsibilities { get; private set; }
    public string? Requirements { get; private set; }
    public string? NiceToHave { get; private set; }
    public JobType JobType { get; private set; }
    public WorkMode WorkMode { get; private set; }
    public ExperienceLevel ExperienceLevel { get; private set; }
    public short? MinExperienceYears { get; private set; }
    public short? MaxExperienceYears { get; private set; }
    public decimal? MinSalary { get; private set; }
    public decimal? MaxSalary { get; private set; }
    public string? CurrencyCode { get; private set; }
    public bool IsSalaryHidden { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Guid? SubCategoryId { get; private set; }
    public Guid? CityId { get; private set; }
    public Guid? StateId { get; private set; }
    public Guid? CountryId { get; private set; }
    public JobPostingStatus Status { get; private set; } = JobPostingStatus.Draft;
    public short OpeningsCount { get; private set; } = 1;
    public int ApplicationsCount { get; private set; }
    public int ViewsCount { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public bool IsUrgent { get; private set; }
    public bool IsFeatured { get; private set; }
    public bool IsRemote { get; private set; }
    public string? ApplicationEmail { get; private set; }
    public string? ApplicationUrl { get; private set; }

    private JobPosting() { }

    public static JobPosting Create(Guid tenantId, Guid companyId, string title, string slug,
        string description, JobType jobType, WorkMode workMode, ExperienceLevel level, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CompanyId = companyId,
            Title = title.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            Description = description.Trim(),
            JobType = jobType,
            WorkMode = workMode,
            ExperienceLevel = level,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void Update(string title, string description, string? responsibilities, string? requirements,
        string? niceToHave, JobType jobType, WorkMode workMode, ExperienceLevel level,
        short? minExp, short? maxExp, decimal? minSalary, decimal? maxSalary, string? currencyCode,
        bool isSalaryHidden, Guid? categoryId, Guid? subCategoryId, Guid? cityId, Guid? stateId,
        Guid? countryId, short openingsCount, DateTimeOffset? expiresAt, bool isUrgent, bool isFeatured,
        bool isRemote, string? applicationEmail, string? applicationUrl, Guid modifiedBy)
    {
        Title = title.Trim();
        Description = description.Trim();
        Responsibilities = responsibilities?.Trim();
        Requirements = requirements?.Trim();
        NiceToHave = niceToHave?.Trim();
        JobType = jobType;
        WorkMode = workMode;
        ExperienceLevel = level;
        MinExperienceYears = minExp;
        MaxExperienceYears = maxExp;
        MinSalary = minSalary;
        MaxSalary = maxSalary;
        CurrencyCode = currencyCode;
        IsSalaryHidden = isSalaryHidden;
        CategoryId = categoryId;
        SubCategoryId = subCategoryId;
        CityId = cityId;
        StateId = stateId;
        CountryId = countryId;
        OpeningsCount = openingsCount;
        ExpiresAt = expiresAt;
        IsUrgent = isUrgent;
        IsFeatured = isFeatured;
        IsRemote = isRemote;
        ApplicationEmail = applicationEmail?.Trim();
        ApplicationUrl = applicationUrl?.Trim();
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void Publish(Guid modifiedBy)
    {
        Status = JobPostingStatus.Published;
        PublishedAt = DateTimeOffset.UtcNow;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void UpdateStatus(JobPostingStatus status, Guid modifiedBy)
    {
        Status = status;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
