using JobPortal.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortal.Infrastructure.Persistence.Configurations.Auth;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants", "auth");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        // auth.Tenants has no TenantId column — Tenant IS the root of tenancy.
        builder.Ignore(e => e.TenantId);

        // Audit columns present in the table
        builder.Property(e => e.CreatedBy).IsRequired();
        builder.Property(e => e.CreatedOn).IsRequired();
        builder.Property(e => e.ModifiedBy).IsRequired(false);
        builder.Property(e => e.ModifiedOn).IsRequired(false);

        // Soft delete
        builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(e => e.DeletedBy).IsRequired(false);
        builder.Property(e => e.DeletedOn).IsRequired(false);

        // Optimistic concurrency via PostgreSQL system column xmin
        builder.Property(e => e.RecordVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Slug).IsRequired().HasMaxLength(100);
        builder.Property(e => e.CustomDomain).HasMaxLength(253);
        builder.Property(e => e.LogoUrl).HasMaxLength(2000);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(e => e.ContactEmail).IsRequired().HasMaxLength(320);
        builder.Property(e => e.ContactPhone).HasMaxLength(20);
        builder.Property(e => e.Address);
        builder.Property(e => e.Country).HasMaxLength(100);
        builder.Property(e => e.SubscriptionTier)
            .IsRequired()
            .HasDefaultValue(JobPortal.Domain.Enums.SubscriptionTier.Free)
            .HasConversion<string>();

        builder.HasIndex(e => e.Slug)
            .IsUnique()
            .HasDatabaseName("uq_Tenants_Slug");
        builder.HasIndex(e => e.IsDeleted)
            .HasDatabaseName("idx_Tenants_IsDeleted");
    }
}

public sealed class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users", "auth");

        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.Property(e => e.NormalizedEmail).IsRequired().HasMaxLength(256);
        builder.Property(e => e.PasswordHash).IsRequired().HasMaxLength(512);
        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.PhoneNumber).HasMaxLength(20);
        builder.Property(e => e.ProfilePictureUrl).HasMaxLength(2000);
        builder.Property(e => e.IsEmailVerified).IsRequired().HasDefaultValue(false);
        builder.Property(e => e.IsPhoneVerified).IsRequired().HasDefaultValue(false);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(e => e.IsTwoFactorEnabled).IsRequired().HasDefaultValue(false);
        builder.Property(e => e.FailedLoginAttempts).IsRequired().HasDefaultValue(0);
        builder.Property(e => e.Gender).HasConversion<int>();

        // Composite unique: one email per tenant
        builder.HasIndex(e => new { e.TenantId, e.NormalizedEmail })
            .IsUnique()
            .HasDatabaseName("uq_Users_TenantId_NormalizedEmail");

        builder.HasIndex(e => e.NormalizedEmail)
            .HasDatabaseName("idx_Users_NormalizedEmail");
    }
}

public sealed class RoleConfiguration : BaseEntityConfiguration<Role>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        builder.ToTable("Roles", "auth");

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.NormalizedName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);

        builder.HasIndex(e => new { e.TenantId, e.NormalizedName })
            .IsUnique()
            .HasDatabaseName("uq_Roles_TenantId_NormalizedName");
    }
}

public sealed class PermissionConfiguration : BaseEntityConfiguration<Permission>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.ToTable("Permissions", "auth");

        builder.Property(e => e.Name).IsRequired().HasMaxLength(150);
        builder.Property(e => e.Resource).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Action).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);

        builder.HasIndex(e => new { e.TenantId, e.Name })
            .IsUnique()
            .HasDatabaseName("uq_Permissions_TenantId_Name");
    }
}

public sealed class UserRoleConfiguration : BaseEntityConfiguration<UserRole>
{
    public override void Configure(EntityTypeBuilder<UserRole> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserRoles", "auth");

        builder.HasIndex(e => new { e.TenantId, e.UserId, e.RoleId })
            .IsUnique()
            .HasDatabaseName("uq_UserRoles_TenantId_UserId_RoleId");

        builder.HasIndex(e => e.UserId).HasDatabaseName("idx_UserRoles_UserId");
        builder.HasIndex(e => e.RoleId).HasDatabaseName("idx_UserRoles_RoleId");

        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class RolePermissionConfiguration : BaseEntityConfiguration<RolePermission>
{
    public override void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        base.Configure(builder);

        builder.ToTable("RolePermissions", "auth");

        builder.HasIndex(e => new { e.TenantId, e.RoleId, e.PermissionId })
            .IsUnique()
            .HasDatabaseName("uq_RolePermissions_TenantId_RoleId_PermissionId");

        builder.HasIndex(e => e.RoleId).HasDatabaseName("idx_RolePermissions_RoleId");
        builder.HasIndex(e => e.PermissionId).HasDatabaseName("idx_RolePermissions_PermissionId");

        builder.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Permission).WithMany().HasForeignKey(e => e.PermissionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.ToTable("RefreshTokens", "auth");

        builder.Property(e => e.TokenHash).IsRequired().HasMaxLength(512);
        builder.Property(e => e.ExpiresOn).IsRequired();
        builder.Property(e => e.DeviceInfo).HasMaxLength(500);
        builder.Property(e => e.IpAddress).HasMaxLength(45); // IPv6 max
        builder.Property(e => e.RevokedReason).HasMaxLength(500);

        builder.HasIndex(e => e.TokenHash).HasDatabaseName("idx_RefreshTokens_TokenHash");
        builder.HasIndex(e => new { e.TenantId, e.UserId }).HasDatabaseName("idx_RefreshTokens_TenantId_UserId");
        builder.HasIndex(e => e.ExpiresOn).HasDatabaseName("idx_RefreshTokens_ExpiresOn");
    }
}
