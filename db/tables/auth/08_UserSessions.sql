-- ============================================================
-- auth.UserSessions — Active user sessions
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."UserSessions" (
    "Id"             UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"       UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"         UUID         NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "SessionToken"   VARCHAR(512) NOT NULL,
    "DeviceType"     VARCHAR(50),               -- 'web', 'mobile', 'desktop'
    "DeviceInfo"     VARCHAR(500),
    "IpAddress"      VARCHAR(45),
    "UserAgent"      TEXT,
    "IsActive"       BOOLEAN      NOT NULL DEFAULT TRUE,
    "LastActivityAt" TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "ExpiresAt"      TIMESTAMPTZ  NOT NULL,
    -- Audit columns
    "CreatedBy"      UUID         NOT NULL,
    "CreatedOn"      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "ModifiedBy"     UUID,
    "ModifiedOn"     TIMESTAMPTZ,
    "IsDeleted"      BOOLEAN      NOT NULL DEFAULT FALSE,
    "DeletedBy"      UUID,
    "DeletedOn"      TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_UserSessions_SessionToken
    ON auth."UserSessions" ("SessionToken");

CREATE INDEX IF NOT EXISTS idx_UserSessions_UserId
    ON auth."UserSessions" ("UserId");

CREATE INDEX IF NOT EXISTS idx_UserSessions_UserId_IsActive
    ON auth."UserSessions" ("UserId", "IsActive") WHERE "IsActive" = TRUE;

CREATE INDEX IF NOT EXISTS idx_UserSessions_ExpiresAt
    ON auth."UserSessions" ("ExpiresAt") WHERE "IsActive" = TRUE;

CALL apply_ModifiedOn_trigger('auth', 'UserSessions');
