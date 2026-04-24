-- ============================================================
-- auth.Permissions — Granular permissions per tenant
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."Permissions" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"    UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "Key"         VARCHAR(200) NOT NULL,   -- e.g. 'jobs.create', 'candidates.view'
    "Name"        VARCHAR(200) NOT NULL,
    "Description" VARCHAR(500),
    "Resource"    VARCHAR(100) NOT NULL,   -- 'jobs', 'candidates', 'reports'
    "Action"      VARCHAR(50)  NOT NULL,   -- 'create', 'read', 'update', 'delete'
    "IsActive"    BOOLEAN      NOT NULL DEFAULT TRUE,
    -- Audit columns
    "CreatedBy"   UUID         NOT NULL,
    "CreatedOn"   TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "ModifiedBy"  UUID,
    "ModifiedOn"  TIMESTAMPTZ,
    "IsDeleted"   BOOLEAN      NOT NULL DEFAULT FALSE,
    "DeletedBy"   UUID,
    "DeletedOn"   TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_Permissions_TenantId_Key
    ON auth."Permissions" ("TenantId", "Key") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_Permissions_TenantId
    ON auth."Permissions" ("TenantId");

CREATE INDEX IF NOT EXISTS idx_Permissions_Resource
    ON auth."Permissions" ("TenantId", "Resource");

CALL apply_ModifiedOn_trigger('auth', 'Permissions');
