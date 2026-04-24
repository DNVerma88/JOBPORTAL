using JobPortal.Domain.Entities.Auth;
using JobPortal.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortal.Infrastructure.Persistence.Configurations.Auth;

public sealed class UserSessionConfiguration : BaseEntityConfiguration<UserSession>
{
    public override void Configure(EntityTypeBuilder<UserSession> builder)
    {
        base.Configure(builder);
        builder.ToTable("UserSessions", "auth");
        builder.Property(e => e.SessionToken).IsRequired().HasMaxLength(512);
        builder.Property(e => e.DeviceType).HasMaxLength(50);
        builder.Property(e => e.DeviceInfo).HasMaxLength(500);
        builder.Property(e => e.IpAddress).HasMaxLength(45);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.HasIndex(e => e.SessionToken).IsUnique().HasDatabaseName("idx_UserSessions_SessionToken");
        builder.HasIndex(e => e.UserId).HasDatabaseName("idx_UserSessions_UserId");
    }
}

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "auth");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Action).IsRequired().HasMaxLength(100);
        builder.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(e => e.IpAddress).HasMaxLength(45);
        builder.Property(e => e.CorrelationId).HasMaxLength(100);
        builder.HasIndex(e => e.TenantId).HasDatabaseName("idx_AuditLogs_TenantId");
    }
}
