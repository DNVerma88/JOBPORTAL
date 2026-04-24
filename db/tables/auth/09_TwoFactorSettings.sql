-- ============================================================
-- auth.TwoFactorSettings — 2FA config per user
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."TwoFactorSettings" (
    "Id"         UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"   UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"     UUID        NOT NULL UNIQUE REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "IsEnabled"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "Method"     VARCHAR(20) NOT NULL DEFAULT 'TOTP',   -- 'TOTP', 'SMS', 'Email'
    "Secret"     VARCHAR(256),                           -- TOTP secret (encrypted at app level)
    "BackupCodes" TEXT[],                                -- Encrypted backup codes
    "LastUsedAt" TIMESTAMPTZ,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_TwoFactorSettings_TenantId
    ON auth."TwoFactorSettings" ("TenantId");

CALL apply_ModifiedOn_trigger('auth', 'TwoFactorSettings');
