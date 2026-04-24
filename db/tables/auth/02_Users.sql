-- ============================================================
-- auth.Users — All users across all tenants
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."Users" (
    "Id"                  UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"            UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "Email"               VARCHAR(320) NOT NULL,
    "NormalizedEmail"     VARCHAR(320) NOT NULL,
    "PasswordHash"        VARCHAR(512) NOT NULL,
    "FirstName"           VARCHAR(100) NOT NULL,
    "LastName"            VARCHAR(100) NOT NULL,
    "PhoneNumber"         VARCHAR(20),
    "ProfilePictureUrl"   VARCHAR(2000),
    "IsEmailVerified"     BOOLEAN      NOT NULL DEFAULT FALSE,
    "IsPhoneVerified"     BOOLEAN      NOT NULL DEFAULT FALSE,
    "IsActive"            BOOLEAN      NOT NULL DEFAULT TRUE,
    "IsSuperAdmin"        BOOLEAN      NOT NULL DEFAULT FALSE,
    "Gender"              SMALLINT     NOT NULL DEFAULT 0,
    "DateOfBirth"         DATE,
    "FailedLoginAttempts" INT          NOT NULL DEFAULT 0,
    "LockedUntil"         TIMESTAMPTZ,
    "LastLoginOn"         TIMESTAMPTZ,
    "LastLoginIp"         VARCHAR(45),
    "IsTwoFactorEnabled"  BOOLEAN      NOT NULL DEFAULT FALSE,
    "TwoFactorSecret"     VARCHAR(256),
    "RefreshTokenVersion" INT          NOT NULL DEFAULT 0,
    -- Audit columns
    "CreatedBy"           UUID         NOT NULL,
    "CreatedOn"           TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "ModifiedBy"          UUID,
    "ModifiedOn"          TIMESTAMPTZ,
    "IsDeleted"           BOOLEAN      NOT NULL DEFAULT FALSE,
    "DeletedBy"           UUID,
    "DeletedOn"           TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_Users_TenantId_NormalizedEmail
    ON auth."Users" ("TenantId", "NormalizedEmail") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_Users_TenantId
    ON auth."Users" ("TenantId");

CREATE INDEX IF NOT EXISTS idx_Users_NormalizedEmail
    ON auth."Users" ("NormalizedEmail");

CREATE INDEX IF NOT EXISTS idx_Users_IsActive
    ON auth."Users" ("TenantId", "IsActive");

CALL apply_ModifiedOn_trigger('auth', 'Users');
