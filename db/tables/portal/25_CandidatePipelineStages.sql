-- ============================================================
-- portal.CandidatePipelineStages — Stages for Kanban board
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."CandidatePipelineStages" (
    "Id"           UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"     UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "JobPostingId" UUID         NOT NULL REFERENCES portal."JobPostings"("Id") ON DELETE CASCADE,
    "Name"         VARCHAR(100) NOT NULL,
    "Color"        VARCHAR(7),
    "SortOrder"    SMALLINT     NOT NULL DEFAULT 0,
    "IsDefault"    BOOLEAN      NOT NULL DEFAULT FALSE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_CandidatePipelineStages_JobPostingId
    ON portal."CandidatePipelineStages" ("JobPostingId");

CALL apply_ModifiedOn_trigger('portal', 'CandidatePipelineStages');
