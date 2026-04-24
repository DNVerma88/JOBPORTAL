-- ============================================================
-- auth.RolePermissions — Role ↔ Permission mapping
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."RolePermissions" (
    "Id"           UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"     UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "RoleId"       UUID        NOT NULL REFERENCES auth."Roles"("Id") ON DELETE CASCADE,
    "PermissionId" UUID        NOT NULL REFERENCES auth."Permissions"("Id") ON DELETE CASCADE,
    "CreatedBy"    UUID        NOT NULL,
    "CreatedOn"   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy"  UUID,
    "ModifiedOn"  TIMESTAMPTZ,
    "IsDeleted"   BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"   UUID,
    "DeletedOn"   TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_RolePermissions_RoleId_PermissionId
    ON auth."RolePermissions" ("RoleId", "PermissionId");

CREATE INDEX IF NOT EXISTS idx_RolePermissions_TenantId
    ON auth."RolePermissions" ("TenantId");

CREATE INDEX IF NOT EXISTS idx_RolePermissions_RoleId
    ON auth."RolePermissions" ("RoleId");

CREATE INDEX IF NOT EXISTS idx_RolePermissions_PermissionId
    ON auth."RolePermissions" ("PermissionId");
