using JobPortal.Domain.Entities.Config;
using JobPortal.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortal.Infrastructure.Persistence.Configurations.Config;

public sealed class TenantSettingConfiguration : BaseEntityConfiguration<TenantSetting>
{
    public override void Configure(EntityTypeBuilder<TenantSetting> builder)
    {
        base.Configure(builder);
        builder.ToTable("TenantSettings", "config");
        builder.Property(e => e.Key).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Value).IsRequired();
        builder.Property(e => e.DataType).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.HasIndex(e => new { e.TenantId, e.Key }).IsUnique().HasDatabaseName("uq_TenantSettings_TenantId_Key");
    }
}

public sealed class EmailTemplateConfiguration : BaseEntityConfiguration<EmailTemplate>
{
    public override void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        base.Configure(builder);
        builder.ToTable("EmailTemplates", "config");
        builder.Property(e => e.TemplateKey).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Subject).IsRequired().HasMaxLength(300);
        builder.Property(e => e.BodyHtml).IsRequired();
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.HasIndex(e => new { e.TenantId, e.TemplateKey }).IsUnique().HasDatabaseName("uq_EmailTemplates_TenantId_TemplateKey");
    }
}

public sealed class FeatureFlagConfiguration : BaseEntityConfiguration<FeatureFlag>
{
    public override void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        base.Configure(builder);
        builder.ToTable("FeatureFlags", "config");
        builder.Property(e => e.FlagKey).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(300);
        builder.HasIndex(e => new { e.TenantId, e.FlagKey }).IsUnique().HasDatabaseName("uq_FeatureFlags_TenantId_FlagKey");
    }
}

public sealed class AnnouncementConfiguration : BaseEntityConfiguration<Announcement>
{
    public override void Configure(EntityTypeBuilder<Announcement> builder)
    {
        base.Configure(builder);
        builder.ToTable("Announcements", "config");
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Body).IsRequired();
        builder.Property(e => e.Type).IsRequired().HasMaxLength(50);
        builder.Property(e => e.TargetRoles).HasColumnType("text[]");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
    }
}
