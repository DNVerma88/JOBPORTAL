-- ============================================================
-- auth.UserRoles — User ↔ Role mapping
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."UserRoles" (
    "Id"        UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"  UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"    UUID        NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "RoleId"    UUID        NOT NULL REFERENCES auth."Roles"("Id") ON DELETE CASCADE,
    "CreatedBy" UUID        NOT NULL,
    "CreatedOn" TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_UserRoles_UserId_RoleId
    ON auth."UserRoles" ("UserId", "RoleId");

CREATE INDEX IF NOT EXISTS idx_UserRoles_TenantId
    ON auth."UserRoles" ("TenantId");

CREATE INDEX IF NOT EXISTS idx_UserRoles_UserId
    ON auth."UserRoles" ("UserId");

CREATE INDEX IF NOT EXISTS idx_UserRoles_RoleId
    ON auth."UserRoles" ("RoleId");
