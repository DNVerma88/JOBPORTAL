using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using JobPortal.Domain.Entities.Config;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Config.ConfigFeatures;

// ══════════════════════════════════════════════════════════════════════════════
//  SETTINGS
// ══════════════════════════════════════════════════════════════════════════════

public sealed record GetSettingQuery(string Key) : IRequest<TenantSettingDto?>;

public sealed record TenantSettingDto(Guid Id, string Key, string Value, string DataType, string? Description, string? Group);

public sealed record GetAllSettingsQuery : IRequest<List<TenantSettingDto>>;

public sealed class GetAllSettingsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetAllSettingsQuery, List<TenantSettingDto>>
{
    public async Task<List<TenantSettingDto>> Handle(GetAllSettingsQuery request, CancellationToken ct)
    {
        var tenantId = currentUser.TenantId ?? Guid.Empty;
        return await db.TenantSettings
            .Where(s => s.TenantId == tenantId)
            .OrderBy(s => s.Key)
            .Select(s => new TenantSettingDto(s.Id, s.Key, s.Value, s.DataType, s.Description, null))
            .ToListAsync(ct);
    }
}

public sealed class GetSettingQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetSettingQuery, TenantSettingDto?>
{
    public async Task<TenantSettingDto?> Handle(GetSettingQuery request, CancellationToken ct)
    {
        var tenantId = currentUser.TenantId ?? Guid.Empty;
        var setting = await db.TenantSettings
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Key == request.Key, ct);

        if (setting is null) return null;
        return new TenantSettingDto(setting.Id, setting.Key, setting.Value, setting.DataType, setting.Description, null);
    }
}

public sealed record UpdateSettingCommand(string Key, string Value) : IRequest;

public sealed class UpdateSettingCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UpdateSettingCommand>
{
    public async Task Handle(UpdateSettingCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var setting = await db.TenantSettings
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Key == request.Key, ct);

        if (setting is null)
        {
            setting = TenantSetting.Create(tenantId, request.Key, request.Value, "String", null, false, userId);
            db.TenantSettings.Add(setting);
        }
        else
            setting.UpdateValue(request.Value, userId);

        await db.SaveChangesAsync(ct);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
//  EMAIL TEMPLATES
// ══════════════════════════════════════════════════════════════════════════════

public sealed record GetEmailTemplatesQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedList<EmailTemplateDto>>;

public sealed record EmailTemplateDto(Guid Id, string TemplateKey, string Subject, bool IsGlobal, bool IsActive, DateTimeOffset CreatedOn);

public sealed class GetEmailTemplatesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetEmailTemplatesQuery, PagedList<EmailTemplateDto>>
{
    public async Task<PagedList<EmailTemplateDto>> Handle(GetEmailTemplatesQuery request, CancellationToken ct)
    {
        var total = await db.EmailTemplates.CountAsync(ct);
        var items = await db.EmailTemplates
            .OrderBy(e => e.TemplateKey)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EmailTemplateDto(e.Id, e.TemplateKey, e.Subject, e.IsGlobal, e.IsActive, e.CreatedOn))
            .ToListAsync(ct);

        return PagedList<EmailTemplateDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

public sealed record UpdateEmailTemplateCommand(Guid Id, string Subject, string BodyHtml, string? BodyText) : IRequest;

public sealed class UpdateEmailTemplateCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UpdateEmailTemplateCommand>
{
    public async Task Handle(UpdateEmailTemplateCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var template = await db.EmailTemplates.FirstOrDefaultAsync(t => t.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Email template {request.Id} not found.");
        template.Update(request.Subject, request.BodyHtml, request.BodyText, userId);
        await db.SaveChangesAsync(ct);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
//  FEATURE FLAGS
// ══════════════════════════════════════════════════════════════════════════════

public sealed record GetFeatureFlagsQuery : IRequest<List<FeatureFlagDto>>;

public sealed record FeatureFlagDto(Guid Id, string FlagKey, bool IsEnabled, string? Description, bool IsGlobal);

public sealed class GetFeatureFlagsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetFeatureFlagsQuery, List<FeatureFlagDto>>
{
    public async Task<List<FeatureFlagDto>> Handle(GetFeatureFlagsQuery request, CancellationToken ct)
        => await db.FeatureFlags
            .OrderBy(f => f.FlagKey)
            .Select(f => new FeatureFlagDto(f.Id, f.FlagKey, f.IsEnabled, f.Description, f.IsGlobal))
            .ToListAsync(ct);
}

public sealed record ToggleFeatureFlagCommand(Guid Id, bool IsEnabled) : IRequest;

public sealed class ToggleFeatureFlagCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<ToggleFeatureFlagCommand>
{
    public async Task Handle(ToggleFeatureFlagCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var flag = await db.FeatureFlags.FirstOrDefaultAsync(f => f.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Feature flag {request.Id} not found.");
        flag.Toggle(request.IsEnabled, userId);
        await db.SaveChangesAsync(ct);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
//  ANNOUNCEMENTS
// ══════════════════════════════════════════════════════════════════════════════

public sealed record GetAnnouncementsQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedList<AnnouncementDto>>;

public sealed record AnnouncementDto(Guid Id, string Title, string Body, string Type,
    bool IsGlobal, bool IsActive, DateTimeOffset StartsAt, DateTimeOffset? EndsAt, DateTimeOffset CreatedOn);

public sealed class GetAnnouncementsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAnnouncementsQuery, PagedList<AnnouncementDto>>
{
    public async Task<PagedList<AnnouncementDto>> Handle(GetAnnouncementsQuery request, CancellationToken ct)
    {
        var total = await db.Announcements.CountAsync(ct);
        var items = await db.Announcements
            .OrderByDescending(a => a.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AnnouncementDto(a.Id, a.Title, a.Body, a.Type,
                a.IsGlobal, a.IsActive, a.StartsAt, a.EndsAt, a.CreatedOn))
            .ToListAsync(ct);

        return PagedList<AnnouncementDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

public sealed record CreateAnnouncementCommand(
    string Title, string Body, string Type, bool IsGlobal,
    DateTimeOffset StartsAt, DateTimeOffset? EndsAt, string[]? TargetRoles) : IRequest<Guid>;

public sealed class CreateAnnouncementCommandValidator : AbstractValidator<CreateAnnouncementCommand>
{
    public CreateAnnouncementCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Body).NotEmpty();
    }
}

public sealed class CreateAnnouncementCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<CreateAnnouncementCommand, Guid>
{
    public async Task<Guid> Handle(CreateAnnouncementCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var announcement = Announcement.Create(tenantId, request.Title, request.Body, request.Type,
            request.IsGlobal, request.StartsAt, request.EndsAt, request.TargetRoles, userId);

        db.Announcements.Add(announcement);
        await db.SaveChangesAsync(ct);
        return announcement.Id;
    }
}

public sealed record DeleteAnnouncementCommand(Guid Id) : IRequest;

public sealed class DeleteAnnouncementCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<DeleteAnnouncementCommand>
{
    public async Task Handle(DeleteAnnouncementCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var announcement = await db.Announcements.FirstOrDefaultAsync(a => a.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Announcement {request.Id} not found.");
        announcement.SoftDelete(userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Update Announcement ─────────────────────────────────────────────────────────
public sealed record UpdateAnnouncementCommand(
    Guid Id, string Title, string Body, string Type,
    bool IsGlobal, bool IsActive, DateTimeOffset StartsAt, DateTimeOffset? EndsAt) : IRequest;

public sealed class UpdateAnnouncementCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UpdateAnnouncementCommand>
{
    public async Task Handle(UpdateAnnouncementCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var announcement = await db.Announcements.FirstOrDefaultAsync(a => a.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Announcement {request.Id} not found.");
        announcement.Update(request.Title, request.Body, request.Type, request.IsGlobal,
            request.IsActive, request.StartsAt, request.EndsAt, userId);
        await db.SaveChangesAsync(ct);
    }
}
