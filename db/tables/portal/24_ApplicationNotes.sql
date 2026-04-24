-- ============================================================
-- portal.ApplicationNotes — Recruiter internal notes
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."ApplicationNotes" (
    "Id"            UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"      UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "ApplicationId" UUID        NOT NULL REFERENCES portal."JobApplications"("Id") ON DELETE CASCADE,
    "Note"          TEXT        NOT NULL,
    "IsPrivate"     BOOLEAN     NOT NULL DEFAULT TRUE,
    "PinnedAt"      TIMESTAMPTZ,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_ApplicationNotes_ApplicationId
    ON portal."ApplicationNotes" ("ApplicationId");

CALL apply_ModifiedOn_trigger('portal', 'ApplicationNotes');
