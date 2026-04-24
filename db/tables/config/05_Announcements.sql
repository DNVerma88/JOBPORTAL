-- ============================================================
-- config.Announcements — Admin broadcast banners
-- ============================================================

CREATE TABLE IF NOT EXISTS config."Announcements" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"    UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "Title"       VARCHAR(300) NOT NULL,
    "Body"        TEXT         NOT NULL,
    "Type"        VARCHAR(50)  NOT NULL DEFAULT 'Info',   -- 'Info', 'Warning', 'Critical', 'Success'
    "IsGlobal"    BOOLEAN      NOT NULL DEFAULT FALSE,
    "StartsAt"    TIMESTAMPTZ  NOT NULL,
    "EndsAt"      TIMESTAMPTZ,
    "TargetRoles" TEXT[],                  -- NULL = all roles
    "IsActive"    BOOLEAN      NOT NULL DEFAULT TRUE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_Announcements_TenantId_IsActive
    ON config."Announcements" ("TenantId", "IsActive");

CREATE INDEX IF NOT EXISTS idx_Announcements_StartsAt_EndsAt
    ON config."Announcements" ("StartsAt", "EndsAt") WHERE "IsActive" = TRUE;

CALL apply_ModifiedOn_trigger('config', 'Announcements');
