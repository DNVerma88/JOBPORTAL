using JobPortal.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortal.Infrastructure.Persistence.Configurations;

/// <summary>
/// Shared base configuration applied to all entities inheriting BaseEntity.
/// Sets up common audit columns, soft delete column, and xmin rowversion for optimistic concurrency.
/// </summary>
public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        // Multi-tenancy
        builder.Property(e => e.TenantId).IsRequired();
        builder.HasIndex(e => e.TenantId).HasDatabaseName($"idx_{typeof(TEntity).Name}s_TenantId");

        // Audit
        builder.Property(e => e.CreatedBy).IsRequired();
        builder.Property(e => e.CreatedOn).IsRequired();
        builder.Property(e => e.ModifiedBy).IsRequired(false);
        builder.Property(e => e.ModifiedOn).IsRequired(false);

        // Soft delete
        builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(e => e.DeletedBy).IsRequired(false);
        builder.Property(e => e.DeletedOn).IsRequired(false);
        builder.HasIndex(e => e.IsDeleted).HasDatabaseName($"idx_{typeof(TEntity).Name}s_IsDeleted");

        // Optimistic concurrency via PostgreSQL xmin system column
        builder.Property(e => e.RecordVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();
    }
}
