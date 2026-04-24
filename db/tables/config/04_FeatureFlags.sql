-- ============================================================
-- config.FeatureFlags — Feature toggles per tenant / globally
-- ============================================================

CREATE TABLE IF NOT EXISTS config."FeatureFlags" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"    UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "FlagKey"     VARCHAR(100) NOT NULL,
    "IsEnabled"   BOOLEAN      NOT NULL DEFAULT FALSE,
    "Conditions"  JSONB,                   -- percentage rollout or user conditions
    "Description" VARCHAR(300),
    "IsGlobal"    BOOLEAN      NOT NULL DEFAULT FALSE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_FeatureFlags_TenantId_FlagKey
    ON config."FeatureFlags" ("TenantId", "FlagKey") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_FeatureFlags_TenantId
    ON config."FeatureFlags" ("TenantId");

CALL apply_ModifiedOn_trigger('config', 'FeatureFlags');
