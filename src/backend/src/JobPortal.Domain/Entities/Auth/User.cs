using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;
using JobPortal.Domain.Events.Auth;

namespace JobPortal.Domain.Entities.Auth;

/// <summary>
/// Platform user across all tenants. Password hash uses Argon2id.
/// </summary>
public sealed class User : BaseEntity, IAggregateRoot
{
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? ProfilePictureUrl { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsPhoneVerified { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsTwoFactorEnabled { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTimeOffset? LockedUntil { get; private set; }
    public DateTimeOffset? LastLoginOn { get; private set; }
    public Gender Gender { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }

    private User() { } // EF Core

    public static User Create(
        Guid tenantId,
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        Guid createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email.Trim().ToLowerInvariant(),
            NormalizedEmail = email.Trim().ToUpperInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.TenantId, user.Email));
        return user;
    }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateProfile(string firstName, string lastName, string? profilePictureUrl, Gender gender, DateOnly? dateOfBirth, Guid modifiedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        ProfilePictureUrl = profilePictureUrl;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void ChangePasswordHash(string newPasswordHash, Guid modifiedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);
        PasswordHash = newPasswordHash;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void VerifyEmail(Guid modifiedBy)
    {
        IsEmailVerified = true;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public bool IsLockedOut() =>
        LockedUntil.HasValue && LockedUntil.Value > DateTimeOffset.UtcNow;

    public void RecordFailedLogin(int maxAttempts, TimeSpan lockoutDuration)
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= maxAttempts)
            LockedUntil = DateTimeOffset.UtcNow.Add(lockoutDuration);
    }

    public void RecordSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;
        LastLoginOn = DateTimeOffset.UtcNow;
    }

    public void EnableTwoFactor(Guid modifiedBy)
    {
        IsTwoFactorEnabled = true;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
