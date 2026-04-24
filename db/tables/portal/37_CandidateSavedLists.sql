-- ============================================================
-- portal.CandidateSavedLists — Recruiter candidate shortlists
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."CandidateSavedLists" (
    "Id"              UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"        UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "Name"            VARCHAR(200) NOT NULL,
    "Description"     TEXT,
    "CreatedByUserId" UUID         NOT NULL REFERENCES auth."Users"("Id"),
    "IsShared"        BOOLEAN      NOT NULL DEFAULT FALSE,
    "Color"           VARCHAR(7),
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_CandidateSavedLists_TenantId
    ON portal."CandidateSavedLists" ("TenantId");

CREATE INDEX IF NOT EXISTS idx_CandidateSavedLists_CreatedByUserId
    ON portal."CandidateSavedLists" ("CreatedByUserId");

CALL apply_ModifiedOn_trigger('portal', 'CandidateSavedLists');
