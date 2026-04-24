-- ============================================================
-- auth.RefreshTokens — JWT refresh token store (SHA-256 hashed)
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."RefreshTokens" (
    "Id"            UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"      UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"        UUID         NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "TokenHash"     VARCHAR(512) NOT NULL,   -- SHA-256 hash, never stored plaintext
    "DeviceInfo"    VARCHAR(500),
    "IpAddress"     VARCHAR(45),
    "ExpiresOn"      TIMESTAMPTZ  NOT NULL,
    "IsRevoked"     BOOLEAN      NOT NULL DEFAULT FALSE,
    "RevokedOn"     TIMESTAMPTZ,
    "RevokedReason" VARCHAR(200),
    "RevokedBy"     UUID,
    -- Audit columns
    "CreatedBy"     UUID         NOT NULL,
    "CreatedOn"     TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "ModifiedBy"    UUID,
    "ModifiedOn"    TIMESTAMPTZ,
    "IsDeleted"     BOOLEAN      NOT NULL DEFAULT FALSE,
    "DeletedBy"     UUID,
    "DeletedOn"     TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_RefreshTokens_TokenHash
    ON auth."RefreshTokens" ("TokenHash");

CREATE INDEX IF NOT EXISTS idx_RefreshTokens_UserId
    ON auth."RefreshTokens" ("UserId");

CREATE INDEX IF NOT EXISTS idx_RefreshTokens_UserId_IsRevoked
    ON auth."RefreshTokens" ("UserId", "IsRevoked") WHERE "IsRevoked" = FALSE;

CREATE INDEX IF NOT EXISTS idx_RefreshTokens_ExpiresOn_Active
    ON auth."RefreshTokens" ("ExpiresOn") WHERE "IsRevoked" = FALSE;

CALL apply_ModifiedOn_trigger('auth', 'RefreshTokens');
