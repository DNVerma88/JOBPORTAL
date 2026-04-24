-- ============================================================
-- auth.Tenants — SaaS tenant registry (no TenantId self-ref)
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."Tenants" (
    "Id"               UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Name"             VARCHAR(200) NOT NULL,
    "Slug"             VARCHAR(100) NOT NULL,
    "CustomDomain"      VARCHAR(255),
    "LogoUrl"          VARCHAR(500),
    "IsActive"         BOOLEAN      NOT NULL DEFAULT TRUE,
    "TrialEndsOn"      TIMESTAMPTZ,
    "SubscriptionTier" VARCHAR(50)            NOT NULL DEFAULT 'Free',
    "ContactEmail"     VARCHAR(320) NOT NULL,
    "ContactPhone"     VARCHAR(20),
    "Address"          TEXT,
    "Country"          VARCHAR(100),
    -- Audit columns
    "CreatedBy"        UUID         NOT NULL,
    "CreatedOn"        TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "ModifiedBy"       UUID,
    "ModifiedOn"       TIMESTAMPTZ,
    "IsDeleted"        BOOLEAN      NOT NULL DEFAULT FALSE,
    "DeletedBy"        UUID,
    "DeletedOn"        TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_Tenants_Slug
    ON auth."Tenants" ("Slug") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_Tenants_IsActive
    ON auth."Tenants" ("IsActive");

CREATE INDEX IF NOT EXISTS idx_Tenants_CustomDomain
    ON auth."Tenants" ("CustomDomain") WHERE "CustomDomain" IS NOT NULL;

CALL apply_ModifiedOn_trigger('auth', 'Tenants');
