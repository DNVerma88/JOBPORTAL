using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Billing.BillingFeatures;

// ── Get Subscription ──────────────────────────────────────────────────────────
public sealed record GetSubscriptionQuery : IRequest<SubscriptionDto?>;

public sealed record SubscriptionDto(
    Guid Id, Guid PlanId, string Tier, string Status,
    DateOnly StartDate, DateOnly? EndDate, bool IsAutoRenew,
    string BillingCycle, decimal Amount, string CurrencyCode);

public sealed class GetSubscriptionQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetSubscriptionQuery, SubscriptionDto?>
{
    public async Task<SubscriptionDto?> Handle(GetSubscriptionQuery request, CancellationToken ct)
    {
        var tenantId = currentUser.TenantId ?? Guid.Empty;
        var sub = await db.TenantSubscriptions
            .Where(s => s.TenantId == tenantId && s.Status == "Active")
            .FirstOrDefaultAsync(ct);

        if (sub is null) return null;
        return new SubscriptionDto(sub.Id, sub.PlanId, sub.Tier.ToString(), sub.Status,
            sub.StartDate, sub.EndDate, sub.IsAutoRenew, sub.BillingCycle, sub.Amount, sub.CurrencyCode);
    }
}

// ── Get Invoices ──────────────────────────────────────────────────────────────
public sealed record GetInvoicesQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedList<InvoiceDto>>;

public sealed record InvoiceDto(
    Guid Id, string InvoiceNumber, DateOnly BillingPeriodStart, DateOnly BillingPeriodEnd,
    decimal TotalAmount, string CurrencyCode, string Status, DateOnly DueDate,
    DateTimeOffset? PaidAt, DateTimeOffset CreatedOn);

public sealed class GetInvoicesQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetInvoicesQuery, PagedList<InvoiceDto>>
{
    public async Task<PagedList<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken ct)
    {
        var tenantId = currentUser.TenantId ?? Guid.Empty;
        var query = db.SubscriptionInvoices.Where(i => i.TenantId == tenantId);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(i => i.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new InvoiceDto(i.Id, i.InvoiceNumber, i.BillingPeriodStart, i.BillingPeriodEnd,
                i.TotalAmount, i.CurrencyCode, i.Status.ToString(), i.DueDate, i.PaidAt, i.CreatedOn))
            .ToListAsync(ct);

        return PagedList<InvoiceDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

// ── Get Credits ───────────────────────────────────────────────────────────────
public sealed record GetCreditsQuery : IRequest<List<JobCreditDto>>;

public sealed record JobCreditDto(
    Guid Id, int TotalCredits, int UsedCredits, int AvailableCredits,
    string? Source, DateTimeOffset? ExpiresAt);

public sealed class GetCreditsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetCreditsQuery, List<JobCreditDto>>
{
    public async Task<List<JobCreditDto>> Handle(GetCreditsQuery request, CancellationToken ct)
    {
        var tenantId = currentUser.TenantId ?? Guid.Empty;
        return await db.JobCredits
            .Where(c => c.TenantId == tenantId)
            .OrderByDescending(c => c.CreatedOn)
            .Select(c => new JobCreditDto(c.Id, c.TotalCredits, c.UsedCredits,
                c.TotalCredits - c.UsedCredits, c.Source, c.ExpiresAt))
            .ToListAsync(ct);
    }
}

// ── Change Plan ───────────────────────────────────────────────────────────────
public sealed record ChangePlanCommand(Guid PlanId, string BillingCycle) : IRequest;

public sealed class ChangePlanCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<ChangePlanCommand>
{
    public async Task Handle(ChangePlanCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var plan = await db.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == request.PlanId, ct)
            ?? throw new KeyNotFoundException($"Plan {request.PlanId} not found.");

        var amount = request.BillingCycle == "Annual" ? plan.PriceAnnually : plan.PriceMonthly;
        if (!Enum.TryParse<SubscriptionTier>(plan.Tier, out var tier))
            throw new InvalidOperationException("Invalid plan tier.");

        var sub = await db.TenantSubscriptions
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Status == "Active", ct);

        if (sub is not null)
            sub.ChangePlan(plan.Id, tier, amount, request.BillingCycle, userId);
        else
        {
            sub = Domain.Entities.Billing.TenantSubscription.Create(
                tenantId, plan.Id, tier, DateOnly.FromDateTime(DateTime.UtcNow),
                amount, plan.CurrencyCode, request.BillingCycle, userId);
            db.TenantSubscriptions.Add(sub);
        }

        await db.SaveChangesAsync(ct);
    }
}

// ── Cancel Subscription ────────────────────────────────────────────────────────
public sealed record CancelSubscriptionCommand(string? Reason) : IRequest;

public sealed class CancelSubscriptionCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<CancelSubscriptionCommand>
{
    public async Task Handle(CancelSubscriptionCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var sub = await db.TenantSubscriptions
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Status == "Active", ct)
            ?? throw new KeyNotFoundException("No active subscription found.");

        sub.Cancel(request.Reason, userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Reactivate Subscription ────────────────────────────────────────────────────
public sealed record ReactivateSubscriptionCommand : IRequest;

public sealed class ReactivateSubscriptionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<ReactivateSubscriptionCommand>
{
    public async Task Handle(ReactivateSubscriptionCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var sub = await db.TenantSubscriptions
            .Where(s => s.TenantId == tenantId)
            .OrderByDescending(s => s.CreatedOn)
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException("No subscription found.");

        sub.Reactivate(userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Purchase Credits ───────────────────────────────────────────────────────────
public sealed record PurchaseCreditsCommand(int Quantity) : IRequest;

public sealed class PurchaseCreditsCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<PurchaseCreditsCommand>
{
    public async Task Handle(PurchaseCreditsCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        if (request.Quantity <= 0) throw new ArgumentException("Quantity must be positive.");

        var credit = await db.JobCredits.FirstOrDefaultAsync(c => c.TenantId == tenantId, ct);
        if (credit is null)
        {
            credit = Domain.Entities.Billing.JobCredit.Create(tenantId, request.Quantity, "Purchase", null, null, userId);
            db.JobCredits.Add(credit);
        }
        else
            credit.AddCredits(request.Quantity, userId);

        await db.SaveChangesAsync(ct);
    }
}

// ── Get Payment Transactions ──────────────────────────────────────────────────
public sealed record GetTransactionsQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedList<TransactionDto>>;

public sealed record TransactionDto(
    Guid Id,
    Guid? InvoiceId,
    decimal Amount,
    string CurrencyCode,
    string? PaymentMethod,
    string? Gateway,
    string? GatewayTransactionId,
    string Status,
    string? FailureReason,
    DateTimeOffset? ProcessedAt,
    DateTimeOffset CreatedOn);

public sealed class GetTransactionsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetTransactionsQuery, PagedList<TransactionDto>>
{
    public async Task<PagedList<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken ct)
    {
        var tenantId = currentUser.TenantId ?? Guid.Empty;
        var query = db.PaymentTransactions.Where(t => t.TenantId == tenantId);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TransactionDto(
                t.Id, t.InvoiceId, t.Amount, t.CurrencyCode,
                t.PaymentMethod, t.Gateway, t.GatewayTransactionId,
                t.Status.ToString(), t.FailureReason, t.ProcessedAt, t.CreatedOn))
            .ToListAsync(ct);

        return PagedList<TransactionDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}
