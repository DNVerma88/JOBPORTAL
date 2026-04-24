using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Config;

public sealed class TenantSetting : BaseEntity
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string DataType { get; private set; } = "String";
    public string? Description { get; private set; }
    public bool IsEncrypted { get; private set; }

    private TenantSetting() { }

    public static TenantSetting Create(Guid tenantId, string key, string value,
        string dataType, string? description, bool isEncrypted, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Key = key.Trim(),
            Value = value,
            DataType = dataType,
            Description = description?.Trim(),
            IsEncrypted = isEncrypted,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void UpdateValue(string value, Guid modifiedBy)
    {
        Value = value;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}

public sealed class EmailTemplate : BaseEntity
{
    public string TemplateKey { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string BodyHtml { get; private set; } = string.Empty;
    public string? BodyText { get; private set; }
    public bool IsGlobal { get; private set; }
    public bool IsActive { get; private set; } = true;

    private EmailTemplate() { }

    public static EmailTemplate Create(Guid tenantId, string templateKey, string subject,
        string bodyHtml, string? bodyText, bool isGlobal, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            TemplateKey = templateKey.Trim().ToLowerInvariant(),
            Subject = subject.Trim(),
            BodyHtml = bodyHtml,
            BodyText = bodyText,
            IsGlobal = isGlobal,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void Update(string subject, string bodyHtml, string? bodyText, Guid modifiedBy)
    {
        Subject = subject.Trim();
        BodyHtml = bodyHtml;
        BodyText = bodyText;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}

public sealed class FeatureFlag : BaseEntity
{
    public string FlagKey { get; private set; } = string.Empty;
    public bool IsEnabled { get; private set; }
    public string? Description { get; private set; }
    public bool IsGlobal { get; private set; }

    private FeatureFlag() { }

    public static FeatureFlag Create(Guid tenantId, string flagKey, bool isEnabled,
        string? description, bool isGlobal, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            FlagKey = flagKey.Trim().ToLowerInvariant(),
            IsEnabled = isEnabled,
            Description = description?.Trim(),
            IsGlobal = isGlobal,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void Toggle(bool isEnabled, Guid modifiedBy)
    {
        IsEnabled = isEnabled;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}

public sealed class Announcement : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public string Type { get; private set; } = "Info";
    public bool IsGlobal { get; private set; }
    public DateTimeOffset StartsAt { get; private set; }
    public DateTimeOffset? EndsAt { get; private set; }
    public string[]? TargetRoles { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Announcement() { }

    public static Announcement Create(Guid tenantId, string title, string body, string type,
        bool isGlobal, DateTimeOffset startsAt, DateTimeOffset? endsAt,
        string[]? targetRoles, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = title.Trim(),
            Body = body.Trim(),
            Type = type,
            IsGlobal = isGlobal,
            StartsAt = startsAt,
            EndsAt = endsAt,
            TargetRoles = targetRoles,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void Update(string title, string body, string type, bool isGlobal, bool isActive,
        DateTimeOffset startsAt, DateTimeOffset? endsAt, Guid modifiedBy)
    {
        Title = title.Trim();
        Body = body.Trim();
        Type = type;
        IsGlobal = isGlobal;
        IsActive = isActive;
        StartsAt = startsAt;
        EndsAt = endsAt;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
