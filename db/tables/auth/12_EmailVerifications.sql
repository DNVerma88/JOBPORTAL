-- ============================================================
-- auth.EmailVerifications — Email OTP / link verification
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."EmailVerifications" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"    UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"      UUID         NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "Email"       VARCHAR(320) NOT NULL,
    "TokenHash"   VARCHAR(512) NOT NULL,    -- SHA-256 hash of OTP or link token
    "OtpCode"     VARCHAR(10),              -- 6-digit OTP (stored hashed via TokenHash)
    "ExpiresAt"   TIMESTAMPTZ  NOT NULL,
    "IsVerified"  BOOLEAN      NOT NULL DEFAULT FALSE,
    "VerifiedAt"  TIMESTAMPTZ,
    "Attempts"    INT          NOT NULL DEFAULT 0,
    "CreatedOn"   TIMESTAMPTZ  NOT NULL DEFAULT NOW()
    -- Immutable after creation
);

CREATE INDEX IF NOT EXISTS idx_EmailVerifications_UserId
    ON auth."EmailVerifications" ("UserId");

CREATE INDEX IF NOT EXISTS idx_EmailVerifications_Email
    ON auth."EmailVerifications" ("Email");

CREATE UNIQUE INDEX IF NOT EXISTS idx_EmailVerifications_TokenHash
    ON auth."EmailVerifications" ("TokenHash");

CREATE INDEX IF NOT EXISTS idx_EmailVerifications_ExpiresAt
    ON auth."EmailVerifications" ("ExpiresAt") WHERE "IsVerified" = FALSE;
