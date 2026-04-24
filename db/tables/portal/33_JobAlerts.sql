-- ============================================================
-- portal.JobAlerts — Job seeker alert subscriptions
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."JobAlerts" (
    "Id"               UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"         UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"           UUID        NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "Name"             VARCHAR(200) NOT NULL,
    "Keywords"         TEXT[],
    "CategoryIds"      UUID[],
    "CityIds"          UUID[],
    "JobTypes"         TEXT[],
    "WorkModes"        TEXT[],
    "MinSalary"        NUMERIC(15,2),
    "MaxSalary"        NUMERIC(15,2),
    "ExperienceLevels" TEXT[],
    "Frequency"        VARCHAR(20)  NOT NULL DEFAULT 'Daily',   -- 'Instant', 'Daily', 'Weekly'
    "IsActive"         BOOLEAN      NOT NULL DEFAULT TRUE,
    "LastSentAt"       TIMESTAMPTZ,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_JobAlerts_UserId_IsActive
    ON portal."JobAlerts" ("UserId", "IsActive");

CREATE INDEX IF NOT EXISTS idx_JobAlerts_Frequency_IsActive
    ON portal."JobAlerts" ("Frequency", "IsActive") WHERE "IsActive" = TRUE;

CALL apply_ModifiedOn_trigger('portal', 'JobAlerts');
