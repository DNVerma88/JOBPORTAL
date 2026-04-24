using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Master;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Master.MasterFeatures;

// ── Skills ────────────────────────────────────────────────────────────────────
public sealed record GetSkillsQuery(string? Search = null, int Limit = 50) : IRequest<List<SkillDto>>;

public sealed record SkillDto(Guid Id, string Name, string Slug, string? Category, int UsageCount);

public sealed class GetSkillsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetSkillsQuery, List<SkillDto>>
{
    public async Task<List<SkillDto>> Handle(GetSkillsQuery request, CancellationToken ct)
    {
        var query = db.Skills.Where(s => s.IsActive);
        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(s => s.Name.Contains(request.Search));

        return await query
            .OrderByDescending(s => s.UsageCount)
            .Take(request.Limit)
            .Select(s => new SkillDto(s.Id, s.Name, s.Slug, s.Category, s.UsageCount))
            .ToListAsync(ct);
    }
}

// ── Industries ────────────────────────────────────────────────────────────────
public sealed record GetIndustriesQuery : IRequest<List<IndustryDto>>;

public sealed record IndustryDto(Guid Id, string Name, string? Description, string? IconUrl, int SortOrder);

public sealed class GetIndustriesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetIndustriesQuery, List<IndustryDto>>
{
    public async Task<List<IndustryDto>> Handle(GetIndustriesQuery request, CancellationToken ct)
        => await db.Industries
            .Where(i => i.IsActive)
            .OrderBy(i => i.SortOrder).ThenBy(i => i.Name)
            .Select(i => new IndustryDto(i.Id, i.Name, i.Description, i.IconUrl, i.SortOrder))
            .ToListAsync(ct);
}

// ── Job Categories ────────────────────────────────────────────────────────────
public sealed record GetJobCategoriesQuery(Guid? IndustryId = null) : IRequest<List<JobCategoryDto>>;

public sealed record JobCategoryDto(Guid Id, Guid? IndustryId, string Name, string Slug, string? IconUrl, int SortOrder);

public sealed class GetJobCategoriesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetJobCategoriesQuery, List<JobCategoryDto>>
{
    public async Task<List<JobCategoryDto>> Handle(GetJobCategoriesQuery request, CancellationToken ct)
    {
        var query = db.JobCategories.Where(c => c.IsActive);
        if (request.IndustryId.HasValue)
            query = query.Where(c => c.IndustryId == request.IndustryId.Value);

        return await query
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
            .Select(c => new JobCategoryDto(c.Id, c.IndustryId, c.Name, c.Slug, c.IconUrl, c.SortOrder))
            .ToListAsync(ct);
    }
}

// ── Subscription Plans ────────────────────────────────────────────────────────
public sealed record GetSubscriptionPlansQuery : IRequest<List<SubscriptionPlanDto>>;

public sealed record SubscriptionPlanDto(
    Guid Id, string Tier, string Name, string? Description,
    decimal PriceMonthly, decimal PriceAnnually, string CurrencyCode,
    int? MaxJobPostings, int? MaxUsers, int JobPostingDurationDays, bool IsActive, int SortOrder);

public sealed class GetSubscriptionPlansQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetSubscriptionPlansQuery, List<SubscriptionPlanDto>>
{
    public async Task<List<SubscriptionPlanDto>> Handle(GetSubscriptionPlansQuery request, CancellationToken ct)
        => await db.SubscriptionPlans
            .Where(p => p.IsActive)
            .OrderBy(p => p.SortOrder)
            .Select(p => new SubscriptionPlanDto(p.Id, p.Tier, p.Name, p.Description,
                p.PriceMonthly, p.PriceAnnually, p.CurrencyCode,
                p.MaxJobPostings, p.MaxUsers, p.JobPostingDurationDays, p.IsActive, p.SortOrder))
            .ToListAsync(ct);
}

// ── Skill CRUD ────────────────────────────────────────────────────────────────
public sealed record CreateSkillCommand(string Name, string? Category) : IRequest<Guid>;

public sealed class CreateSkillCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateSkillCommand, Guid>
{
    public async Task<Guid> Handle(CreateSkillCommand request, CancellationToken ct)
    {
        var skill = new Skill
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            NormalizedName = request.Name.Trim().ToUpperInvariant(),
            Slug = request.Name.Trim().ToLowerInvariant().Replace(" ", "-"),
            Category = request.Category?.Trim(),
        };
        db.Skills.Add(skill);
        await db.SaveChangesAsync(ct);
        return skill.Id;
    }
}

public sealed record UpdateSkillCommand(Guid Id, string Name, string? Category, bool IsActive) : IRequest;

public sealed class UpdateSkillCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateSkillCommand>
{
    public async Task Handle(UpdateSkillCommand request, CancellationToken ct)
    {
        var skill = await db.Skills.FindAsync([request.Id], ct)
            ?? throw new KeyNotFoundException($"Skill {request.Id} not found.");
        skill.Name = request.Name.Trim();
        skill.NormalizedName = request.Name.Trim().ToUpperInvariant();
        skill.Slug = request.Name.Trim().ToLowerInvariant().Replace(" ", "-");
        skill.Category = request.Category?.Trim();
        skill.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
    }
}

public sealed record DeleteSkillCommand(Guid Id) : IRequest;

public sealed class DeleteSkillCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteSkillCommand>
{
    public async Task Handle(DeleteSkillCommand request, CancellationToken ct)
    {
        var skill = await db.Skills.FindAsync([request.Id], ct)
            ?? throw new KeyNotFoundException($"Skill {request.Id} not found.");
        db.Skills.Remove(skill);
        await db.SaveChangesAsync(ct);
    }
}

// ── Industry CRUD ─────────────────────────────────────────────────────────────
public sealed record CreateIndustryCommand(string Name, string? Description, int SortOrder) : IRequest<Guid>;

public sealed class CreateIndustryCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateIndustryCommand, Guid>
{
    public async Task<Guid> Handle(CreateIndustryCommand request, CancellationToken ct)
    {
        var industry = new Industry
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            SortOrder = request.SortOrder,
        };
        db.Industries.Add(industry);
        await db.SaveChangesAsync(ct);
        return industry.Id;
    }
}

public sealed record UpdateIndustryCommand(Guid Id, string Name, string? Description, int SortOrder, bool IsActive) : IRequest;

public sealed class UpdateIndustryCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateIndustryCommand>
{
    public async Task Handle(UpdateIndustryCommand request, CancellationToken ct)
    {
        var industry = await db.Industries.FindAsync([request.Id], ct)
            ?? throw new KeyNotFoundException($"Industry {request.Id} not found.");
        industry.Name = request.Name.Trim();
        industry.Description = request.Description?.Trim();
        industry.SortOrder = request.SortOrder;
        industry.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
    }
}

public sealed record DeleteIndustryCommand(Guid Id) : IRequest;

public sealed class DeleteIndustryCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteIndustryCommand>
{
    public async Task Handle(DeleteIndustryCommand request, CancellationToken ct)
    {
        var industry = await db.Industries.FindAsync([request.Id], ct)
            ?? throw new KeyNotFoundException($"Industry {request.Id} not found.");
        db.Industries.Remove(industry);
        await db.SaveChangesAsync(ct);
    }
}

// ── Job Category CRUD ─────────────────────────────────────────────────────────
public sealed record CreateJobCategoryCommand(string Name, Guid? IndustryId, int SortOrder) : IRequest<Guid>;

public sealed class CreateJobCategoryCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateJobCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateJobCategoryCommand request, CancellationToken ct)
    {
        var category = new JobCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = request.Name.Trim().ToLowerInvariant().Replace(" ", "-"),
            IndustryId = request.IndustryId,
            SortOrder = request.SortOrder,
        };
        db.JobCategories.Add(category);
        await db.SaveChangesAsync(ct);
        return category.Id;
    }
}

public sealed record UpdateJobCategoryCommand(Guid Id, string Name, Guid? IndustryId, int SortOrder, bool IsActive) : IRequest;

public sealed class UpdateJobCategoryCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateJobCategoryCommand>
{
    public async Task Handle(UpdateJobCategoryCommand request, CancellationToken ct)
    {
        var category = await db.JobCategories.FindAsync([request.Id], ct)
            ?? throw new KeyNotFoundException($"Job category {request.Id} not found.");
        category.Name = request.Name.Trim();
        category.Slug = request.Name.Trim().ToLowerInvariant().Replace(" ", "-");
        category.IndustryId = request.IndustryId;
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
    }
}

public sealed record DeleteJobCategoryCommand(Guid Id) : IRequest;

public sealed class DeleteJobCategoryCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteJobCategoryCommand>
{
    public async Task Handle(DeleteJobCategoryCommand request, CancellationToken ct)
    {
        var category = await db.JobCategories.FindAsync([request.Id], ct)
            ?? throw new KeyNotFoundException($"Job category {request.Id} not found.");
        db.JobCategories.Remove(category);
        await db.SaveChangesAsync(ct);
    }
}
