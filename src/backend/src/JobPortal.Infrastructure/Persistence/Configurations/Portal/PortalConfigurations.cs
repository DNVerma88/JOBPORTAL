using JobPortal.Domain.Entities.Portal;
using JobPortal.Domain.Enums;
using JobPortal.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortal.Infrastructure.Persistence.Configurations.Portal;

public sealed class CompanyConfiguration : BaseEntityConfiguration<Company>
{
    public override void Configure(EntityTypeBuilder<Company> builder)
    {
        base.Configure(builder);
        builder.ToTable("Companies", "portal");
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Slug).IsRequired().HasMaxLength(200);
        builder.Property(e => e.LogoUrl).HasMaxLength(500);
        builder.Property(e => e.CoverImageUrl).HasMaxLength(500);
        builder.Property(e => e.WebsiteUrl).HasMaxLength(500);
        builder.Property(e => e.TagLine).HasMaxLength(300);
        builder.Property(e => e.CompanySize).HasMaxLength(50);
        builder.Property(e => e.LinkedInUrl).HasMaxLength(500);
        builder.Property(e => e.TwitterUrl).HasMaxLength(500);
        builder.Property(e => e.GlassdoorUrl).HasMaxLength(500);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique().HasDatabaseName("uq_Companies_TenantId_Slug");
    }
}

public sealed class JobPostingConfiguration : BaseEntityConfiguration<JobPosting>
{
    public override void Configure(EntityTypeBuilder<JobPosting> builder)
    {
        base.Configure(builder);
        builder.ToTable("JobPostings", "portal");
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Slug).IsRequired().HasMaxLength(350);
        builder.Property(e => e.Description).IsRequired();
        builder.Property(e => e.CurrencyCode).HasMaxLength(3);
        builder.Property(e => e.ApplicationEmail).HasMaxLength(320);
        builder.Property(e => e.ApplicationUrl).HasMaxLength(500);
        builder.Property(e => e.JobType).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.WorkMode).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.ExperienceLevel).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.OpeningsCount).HasDefaultValue((short)1);
        builder.Property(e => e.IsSalaryHidden).HasDefaultValue(false);
        builder.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique().HasDatabaseName("uq_JobPostings_TenantId_Slug");
        builder.HasIndex(e => new { e.TenantId, e.Status }).HasDatabaseName("idx_JobPostings_TenantId_Status");
        builder.HasIndex(e => e.CompanyId).HasDatabaseName("idx_JobPostings_CompanyId");
    }
}

public sealed class JobSeekerProfileConfiguration : BaseEntityConfiguration<JobSeekerProfile>
{
    public override void Configure(EntityTypeBuilder<JobSeekerProfile> builder)
    {
        base.Configure(builder);
        builder.ToTable("JobSeekerProfiles", "portal");
        builder.Property(e => e.Headline).HasMaxLength(300);
        builder.Property(e => e.CurrentJobTitle).HasMaxLength(200);
        builder.Property(e => e.CurrentCompany).HasMaxLength(200);
        builder.Property(e => e.Nationality).HasMaxLength(100);
        builder.Property(e => e.PreferredCurrencyCode).HasMaxLength(3);
        builder.Property(e => e.LinkedInUrl).HasMaxLength(500);
        builder.Property(e => e.GitHubUrl).HasMaxLength(500);
        builder.Property(e => e.PortfolioUrl).HasMaxLength(500);
        builder.Property(e => e.ProfileVisibility).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.Gender).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.IsOpenToWork).HasDefaultValue(true);
        builder.Property(e => e.ProfileCompletionPct).HasDefaultValue((short)0);
        builder.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("uq_JobSeekerProfiles_UserId");
    }
}

public sealed class JobApplicationConfiguration : BaseEntityConfiguration<JobApplication>
{
    public override void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        base.Configure(builder);
        builder.ToTable("JobApplications", "portal");
        builder.Property(e => e.AppliedVia).HasMaxLength(50);
        builder.Property(e => e.ReferralCode).HasMaxLength(50);
        builder.Property(e => e.Source).HasMaxLength(100);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(e => new { e.JobPostingId, e.ApplicantId }).IsUnique().HasDatabaseName("uq_JobApplications_JobPosting_Applicant");
        builder.HasIndex(e => e.TenantId).HasDatabaseName("idx_JobApplications_TenantId");
        builder.HasIndex(e => new { e.ApplicantId }).HasDatabaseName("idx_JobApplications_ApplicantId");
    }
}

public sealed class SavedJobConfiguration : IEntityTypeConfiguration<SavedJob>
{
    public void Configure(EntityTypeBuilder<SavedJob> builder)
    {
        builder.ToTable("SavedJobs", "portal");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        // portal.SavedJobs is a lightweight bookmark table: has TenantId + CreatedOn
        // but no standard audit cols (CreatedBy, IsDeleted, etc.).
        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.ModifiedBy);
        builder.Ignore(e => e.ModifiedOn);
        builder.Ignore(e => e.IsDeleted);
        builder.Ignore(e => e.DeletedBy);
        builder.Ignore(e => e.DeletedOn);
        builder.Ignore(e => e.RecordVersion);

        builder.HasIndex(e => new { e.UserId, e.JobPostingId }).IsUnique().HasDatabaseName("uq_SavedJobs_UserId_JobPostingId");
        builder.HasIndex(e => e.UserId).HasDatabaseName("idx_SavedJobs_UserId");
    }
}

public sealed class InterviewScheduleConfiguration : BaseEntityConfiguration<InterviewSchedule>
{
    public override void Configure(EntityTypeBuilder<InterviewSchedule> builder)
    {
        base.Configure(builder);
        builder.ToTable("InterviewSchedules", "portal");
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.MeetingLink).HasMaxLength(500);
        builder.Property(e => e.DurationMinutes).HasDefaultValue((short)60);
        builder.Property(e => e.InterviewType).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(e => e.ApplicationId).HasDatabaseName("idx_InterviewSchedules_ApplicationId");
    }
}

public sealed class OfferLetterConfiguration : BaseEntityConfiguration<OfferLetter>
{
    public override void Configure(EntityTypeBuilder<OfferLetter> builder)
    {
        base.Configure(builder);
        builder.ToTable("OfferLetters", "portal");
        builder.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3);
        builder.Property(e => e.PositionTitle).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Department).HasMaxLength(200);
        builder.Property(e => e.OfferFileUrl).HasMaxLength(500);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(e => e.ApplicationId).HasDatabaseName("idx_OfferLetters_ApplicationId");
    }
}

public sealed class JobAlertConfiguration : BaseEntityConfiguration<JobAlert>
{
    public override void Configure(EntityTypeBuilder<JobAlert> builder)
    {
        base.Configure(builder);
        builder.ToTable("JobAlerts", "portal");
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Frequency).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.Keywords).HasColumnType("text[]");
        builder.Property(e => e.JobTypes).HasColumnType("text[]");
        builder.Property(e => e.WorkModes).HasColumnType("text[]");
        builder.Property(e => e.ExperienceLevels).HasColumnType("text[]");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.HasIndex(e => new { e.UserId, e.IsActive }).HasDatabaseName("idx_JobAlerts_UserId_IsActive");
    }
}

public sealed class NotificationConfiguration : BaseEntityConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);
        builder.ToTable("Notifications", "portal");
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Body).IsRequired();
        builder.Property(e => e.ActionUrl).HasMaxLength(500);
        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.Channel).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.DeliveryScope).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.IsRead).HasDefaultValue(false);
        builder.HasIndex(e => new { e.UserId, e.IsRead }).HasDatabaseName("idx_Notifications_UserId_IsRead");
    }
}

public sealed class CandidatePipelineStageConfiguration : BaseEntityConfiguration<CandidatePipelineStage>
{
    public override void Configure(EntityTypeBuilder<CandidatePipelineStage> builder)
    {
        base.Configure(builder);
        builder.ToTable("CandidatePipelineStages", "portal");
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Color).HasMaxLength(7);
        builder.Property(e => e.SortOrder).HasDefaultValue((short)0);
        builder.HasIndex(e => e.JobPostingId).HasDatabaseName("idx_CandidatePipelineStages_JobPostingId");
    }
}

public sealed class CandidatePipelineConfiguration : IEntityTypeConfiguration<CandidatePipeline>
{
    public void Configure(EntityTypeBuilder<CandidatePipeline> builder)
    {
        builder.ToTable("CandidatePipelines", "portal");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        // portal.CandidatePipelines is a lightweight movement-log table.
        // It has TenantId but NO standard audit columns (CreatedBy, IsDeleted, etc.).
        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.CreatedOn);
        builder.Ignore(e => e.ModifiedBy);
        builder.Ignore(e => e.ModifiedOn);
        builder.Ignore(e => e.IsDeleted);
        builder.Ignore(e => e.DeletedBy);
        builder.Ignore(e => e.DeletedOn);
        builder.Ignore(e => e.RecordVersion);

        builder.Property(e => e.Notes).HasMaxLength(2000);
        builder.HasIndex(e => e.ApplicationId).HasDatabaseName("idx_CandidatePipelines_ApplicationId");
        builder.HasIndex(e => e.StageId).HasDatabaseName("idx_CandidatePipelines_StageId");
    }
}

public sealed class WorkExperienceConfiguration : BaseEntityConfiguration<WorkExperience>
{
    public override void Configure(EntityTypeBuilder<WorkExperience> builder)
    {
        base.Configure(builder);
        builder.ToTable("WorkExperiences", "portal");
        builder.Property(e => e.JobTitle).IsRequired().HasMaxLength(200);
        builder.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.Skills).HasColumnType("text[]");
        builder.HasIndex(e => e.ProfileId).HasDatabaseName("idx_WorkExperiences_ProfileId");
    }
}

public sealed class EducationConfiguration : BaseEntityConfiguration<Education>
{
    public override void Configure(EntityTypeBuilder<Education> builder)
    {
        base.Configure(builder);
        builder.ToTable("Educations", "portal");
        builder.Property(e => e.Degree).IsRequired().HasMaxLength(200);
        builder.Property(e => e.FieldOfStudy).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Institution).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Grade).HasMaxLength(50);
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.HasIndex(e => e.ProfileId).HasDatabaseName("idx_Educations_ProfileId");
    }
}
