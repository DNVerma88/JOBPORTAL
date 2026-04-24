using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using JobPortal.Domain.Entities.Portal;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Offers.OfferFeatures;

// ── Get Offers ────────────────────────────────────────────────────────────────
public sealed record GetOffersQuery(int PageNumber = 1, int PageSize = 20, Guid? ApplicationId = null, Guid? ApplicantId = null)
    : IRequest<PagedList<OfferDto>>;

public sealed record OfferDto(
    Guid Id, Guid ApplicationId, DateOnly OfferDate, DateOnly? JoiningDate,
    decimal OfferSalary, string CurrencyCode, string PositionTitle,
    string? Department, string Status, DateTimeOffset? ExpiresAt, DateTimeOffset CreatedOn,
    string? CandidateName, string? CandidateEmail, string? JobTitle);

public sealed class GetOffersQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetOffersQuery, PagedList<OfferDto>>
{
    public async Task<PagedList<OfferDto>> Handle(GetOffersQuery request, CancellationToken ct)
    {
        var query = db.OfferLetters.AsQueryable();
        if (request.ApplicationId.HasValue)
            query = query.Where(o => o.ApplicationId == request.ApplicationId.Value);

        if (request.ApplicantId.HasValue)
        {
            var appIds = await db.JobApplications
                .Where(a => a.ApplicantId == request.ApplicantId.Value)
                .Select(a => a.Id)
                .ToListAsync(ct);
            query = query.Where(o => appIds.Contains(o.ApplicationId));
        }

        var total = await query.CountAsync(ct);
        var offers = await query
            .OrderByDescending(o => o.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var appIds2 = offers.Select(o => o.ApplicationId).Distinct().ToList();
        var applications = await db.JobApplications
            .Where(a => appIds2.Contains(a.Id))
            .ToListAsync(ct);

        var userIds = applications.Select(a => a.ApplicantId).Distinct().ToList();
        var users = await db.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email })
            .ToListAsync(ct);

        var jobIds = applications.Select(a => a.JobPostingId).Distinct().ToList();
        var jobs = await db.JobPostings
            .Where(j => jobIds.Contains(j.Id))
            .Select(j => new { j.Id, j.Title })
            .ToListAsync(ct);

        var appMap = applications.ToDictionary(a => a.Id);
        var userMap = users.ToDictionary(u => u.Id);
        var jobMap = jobs.ToDictionary(j => j.Id);

        var items = offers.Select(o =>
        {
            appMap.TryGetValue(o.ApplicationId, out var app);
            var user = app is not null && userMap.TryGetValue(app.ApplicantId, out var u) ? u : null;
            var job = app is not null && jobMap.TryGetValue(app.JobPostingId, out var j) ? j : null;
            return new OfferDto(o.Id, o.ApplicationId, o.OfferDate, o.JoiningDate,
                o.OfferSalary, o.CurrencyCode, o.PositionTitle, o.Department,
                o.Status.ToString(), o.ExpiresAt, o.CreatedOn,
                user is not null ? $"{user.FirstName} {user.LastName}" : null,
                user?.Email, job?.Title);
        }).ToList();

        return PagedList<OfferDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

// ── Create Offer ──────────────────────────────────────────────────────────────
public sealed record CreateOfferCommand(
    Guid ApplicationId,
    DateOnly OfferDate,
    DateOnly? JoiningDate,
    decimal OfferSalary,
    string CurrencyCode,
    string PositionTitle,
    string? Department,
    string? Location,
    DateTimeOffset? ExpiresAt
) : IRequest<Guid>;

public sealed class CreateOfferCommandValidator : AbstractValidator<CreateOfferCommand>
{
    public CreateOfferCommandValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.PositionTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OfferSalary).GreaterThan(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
    }
}

public sealed class CreateOfferCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateOfferCommand, Guid>
{
    public async Task<Guid> Handle(CreateOfferCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var offer = OfferLetter.Create(tenantId, request.ApplicationId, userId,
            request.OfferDate, request.OfferSalary, request.CurrencyCode,
            request.PositionTitle, request.Department, request.Location,
            request.JoiningDate, request.ExpiresAt, userId);

        db.OfferLetters.Add(offer);
        await db.SaveChangesAsync(ct);
        return offer.Id;
    }
}

// ── Respond to Offer (candidate accept/decline) ───────────────────────────────
public sealed record RespondToOfferCommand(Guid Id, OfferLetterStatus Response, string? Message) : IRequest;

public sealed class RespondToOfferCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<RespondToOfferCommand>
{
    public async Task Handle(RespondToOfferCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var offer = await db.OfferLetters.FirstOrDefaultAsync(o => o.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Offer {request.Id} not found.");

        if (request.Response != OfferLetterStatus.Accepted && request.Response != OfferLetterStatus.Declined)
            throw new InvalidOperationException("Response must be Accepted or Declined.");

        offer.UpdateStatus(request.Response, request.Message, userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Revoke Offer (recruiter) ──────────────────────────────────────────────────
public sealed record RevokeOfferCommand(Guid Id, string? Reason) : IRequest;

public sealed class RevokeOfferCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<RevokeOfferCommand>
{
    public async Task Handle(RevokeOfferCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var offer = await db.OfferLetters.FirstOrDefaultAsync(o => o.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Offer {request.Id} not found.");

        offer.UpdateStatus(OfferLetterStatus.Revoked, request.Reason, userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Get Offer By ID ────────────────────────────────────────────────────────────
public sealed record GetOfferByIdQuery(Guid Id) : IRequest<OfferDto?>;

public sealed class GetOfferByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetOfferByIdQuery, OfferDto?>
{
    public async Task<OfferDto?> Handle(GetOfferByIdQuery request, CancellationToken ct)
    {
        var offer = await db.OfferLetters.FirstOrDefaultAsync(o => o.Id == request.Id, ct);
        if (offer is null) return null;

        var app = await db.JobApplications.FirstOrDefaultAsync(a => a.Id == offer.ApplicationId, ct);
        string? candidateName = null; string? candidateEmail = null; string? jobTitle = null;
        if (app is not null)
        {
            var user = await db.Users.Where(u => u.Id == app.ApplicantId).Select(u => new { u.FirstName, u.LastName, u.Email }).FirstOrDefaultAsync(ct);
            var job = await db.JobPostings.Where(j => j.Id == app.JobPostingId).Select(j => new { j.Title }).FirstOrDefaultAsync(ct);
            if (user is not null) { candidateName = $"{user.FirstName} {user.LastName}"; candidateEmail = user.Email; }
            jobTitle = job?.Title;
        }
        return new OfferDto(offer.Id, offer.ApplicationId, offer.OfferDate, offer.JoiningDate,
            offer.OfferSalary, offer.CurrencyCode, offer.PositionTitle, offer.Department,
            offer.Status.ToString(), offer.ExpiresAt, offer.CreatedOn,
            candidateName, candidateEmail, jobTitle);
    }
}

// ── Update Offer ───────────────────────────────────────────────────────────────
public sealed record UpdateOfferCommand(
    Guid Id, DateOnly OfferDate, DateOnly? JoiningDate, decimal OfferSalary,
    string CurrencyCode, string PositionTitle, string? Department,
    string? Location, DateTimeOffset? ExpiresAt) : IRequest;

public sealed class UpdateOfferCommandValidator : AbstractValidator<UpdateOfferCommand>
{
    public UpdateOfferCommandValidator()
    {
        RuleFor(x => x.PositionTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OfferSalary).GreaterThan(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
    }
}

public sealed class UpdateOfferCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UpdateOfferCommand>
{
    public async Task Handle(UpdateOfferCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var offer = await db.OfferLetters.FirstOrDefaultAsync(o => o.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Offer {request.Id} not found.");
        offer.Update(request.OfferDate, request.JoiningDate, request.OfferSalary, request.CurrencyCode,
            request.PositionTitle, request.Department, request.Location, request.ExpiresAt, userId);
        await db.SaveChangesAsync(ct);
    }
}
