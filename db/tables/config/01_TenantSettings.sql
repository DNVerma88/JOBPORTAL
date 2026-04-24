-- ============================================================
-- config.TenantSettings — Per-tenant key-value configuration
-- ============================================================

CREATE TABLE IF NOT EXISTS config."TenantSettings" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"    UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "Key"         VARCHAR(200) NOT NULL,
    "Value"       TEXT         NOT NULL,
    "DataType"    VARCHAR(20)  NOT NULL DEFAULT 'String',   -- 'String', 'Number', 'Boolean', 'Json'
    "Description" VARCHAR(500),
    "IsEncrypted" BOOLEAN      NOT NULL DEFAULT FALSE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_TenantSettings_TenantId_Key
    ON config."TenantSettings" ("TenantId", "Key") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_TenantSettings_TenantId
    ON config."TenantSettings" ("TenantId");

CALL apply_ModifiedOn_trigger('config', 'TenantSettings');
