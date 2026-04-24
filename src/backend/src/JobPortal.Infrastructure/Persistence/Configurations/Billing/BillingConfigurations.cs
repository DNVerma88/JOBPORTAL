using JobPortal.Domain.Entities.Billing;
using JobPortal.Domain.Enums;
using JobPortal.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortal.Infrastructure.Persistence.Configurations.Billing;

public sealed class TenantSubscriptionConfiguration : BaseEntityConfiguration<TenantSubscription>
{
    public override void Configure(EntityTypeBuilder<TenantSubscription> builder)
    {
        base.Configure(builder);
        builder.ToTable("TenantSubscriptions", "billing");
        builder.Property(e => e.Status).IsRequired().HasMaxLength(50);
        builder.Property(e => e.BillingCycle).IsRequired().HasMaxLength(20);
        builder.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3);
        builder.Property(e => e.ExternalSubscriptionId).HasMaxLength(200);
        builder.Property(e => e.Tier).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.IsAutoRenew).HasDefaultValue(true);
        builder.HasIndex(e => e.TenantId).HasDatabaseName("idx_TenantSubscriptions_TenantId");
    }
}

public sealed class SubscriptionInvoiceConfiguration : BaseEntityConfiguration<SubscriptionInvoice>
{
    public override void Configure(EntityTypeBuilder<SubscriptionInvoice> builder)
    {
        base.Configure(builder);
        builder.ToTable("SubscriptionInvoices", "billing");
        builder.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
        builder.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3);
        builder.Property(e => e.InvoiceFileUrl).HasMaxLength(500);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.ExternalInvoiceId).HasMaxLength(200);
        builder.HasIndex(e => e.InvoiceNumber).IsUnique().HasDatabaseName("uq_SubscriptionInvoices_InvoiceNumber");
        builder.HasIndex(e => new { e.TenantId, e.Status }).HasDatabaseName("idx_SubscriptionInvoices_TenantId_Status");
    }
}

public sealed class JobCreditConfiguration : BaseEntityConfiguration<JobCredit>
{
    public override void Configure(EntityTypeBuilder<JobCredit> builder)
    {
        base.Configure(builder);
        builder.ToTable("JobCredits", "billing");
        builder.Property(e => e.Source).HasMaxLength(100);
        builder.Property(e => e.TotalCredits).HasDefaultValue(0);
        builder.Property(e => e.UsedCredits).HasDefaultValue(0);
        // AvailableCredits is a C# computed property (TotalCredits - UsedCredits) — not mapped to DB column
        builder.Ignore(e => e.AvailableCredits);
        builder.HasIndex(e => e.TenantId).HasDatabaseName("idx_JobCredits_TenantId");
    }
}

public sealed class PaymentTransactionConfiguration : BaseEntityConfiguration<PaymentTransaction>
{
    public override void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        base.Configure(builder);
        builder.ToTable("PaymentTransactions", "billing");
        builder.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3);
        builder.Property(e => e.PaymentMethod).HasMaxLength(50);
        builder.Property(e => e.Gateway).HasMaxLength(50);
        builder.Property(e => e.GatewayTransactionId).HasMaxLength(200);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(e => new { e.TenantId, e.Status }).HasDatabaseName("idx_PaymentTransactions_TenantId_Status");
    }
}
