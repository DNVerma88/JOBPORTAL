-- ============================================================
-- portal.SavedSearches — Saved job search queries
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."SavedSearches" (
    "Id"             UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"       UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"         UUID         NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "Name"           VARCHAR(200) NOT NULL,
    "SearchParams"   JSONB        NOT NULL,
    "AlertEnabled"   BOOLEAN      NOT NULL DEFAULT FALSE,
    "AlertFrequency" VARCHAR(20)  DEFAULT 'Daily',
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_SavedSearches_UserId
    ON portal."SavedSearches" ("UserId");

CALL apply_ModifiedOn_trigger('portal', 'SavedSearches');
