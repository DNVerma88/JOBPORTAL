-- ============================================================
-- auth.PasswordResetTokens — Secure password reset links
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."PasswordResetTokens" (
    "Id"        UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"  UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"    UUID         NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "TokenHash" VARCHAR(512) NOT NULL,   -- SHA-256 hash of the reset token
    "ExpiresAt" TIMESTAMPTZ  NOT NULL,
    "IsUsed"    BOOLEAN      NOT NULL DEFAULT FALSE,
    "UsedAt"    TIMESTAMPTZ,
    "IpAddress" VARCHAR(45),
    "CreatedOn" TIMESTAMPTZ  NOT NULL DEFAULT NOW()
    -- Immutable after creation, no ModifiedOn needed
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_PasswordResetTokens_TokenHash
    ON auth."PasswordResetTokens" ("TokenHash");

CREATE INDEX IF NOT EXISTS idx_PasswordResetTokens_UserId
    ON auth."PasswordResetTokens" ("UserId");

CREATE INDEX IF NOT EXISTS idx_PasswordResetTokens_ExpiresAt
    ON auth."PasswordResetTokens" ("ExpiresAt") WHERE "IsUsed" = FALSE;
