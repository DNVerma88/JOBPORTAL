-- ============================================================
-- auth.Roles — RBAC roles scoped per tenant
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."Roles" (
    "Id"             UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"       UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "Name"           VARCHAR(100) NOT NULL,
    "NormalizedName" VARCHAR(100) NOT NULL,
    "Description"    VARCHAR(500),
    "IsSystemRole"   BOOLEAN      NOT NULL DEFAULT FALSE,
    "IsActive"       BOOLEAN      NOT NULL DEFAULT TRUE,
    -- Audit columns
    "CreatedBy"      UUID         NOT NULL,
    "CreatedOn"      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "ModifiedBy"     UUID,
    "ModifiedOn"     TIMESTAMPTZ,
    "IsDeleted"      BOOLEAN      NOT NULL DEFAULT FALSE,
    "DeletedBy"      UUID,
    "DeletedOn"      TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_Roles_TenantId_NormalizedName
    ON auth."Roles" ("TenantId", "NormalizedName") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_Roles_TenantId
    ON auth."Roles" ("TenantId");

CALL apply_ModifiedOn_trigger('auth', 'Roles');
