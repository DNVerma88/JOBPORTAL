-- ============================================================
-- portal.CandidatePipelines — Candidate stage movement log
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."CandidatePipelines" (
    "Id"            UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"      UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "ApplicationId" UUID        NOT NULL REFERENCES portal."JobApplications"("Id") ON DELETE CASCADE,
    "StageId"       UUID        NOT NULL REFERENCES portal."CandidatePipelineStages"("Id"),
    "MovedBy"       UUID        NOT NULL REFERENCES auth."Users"("Id"),
    "MovedAt"       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "Notes"         TEXT
);

CREATE INDEX IF NOT EXISTS idx_CandidatePipelines_ApplicationId
    ON portal."CandidatePipelines" ("ApplicationId");

CREATE INDEX IF NOT EXISTS idx_CandidatePipelines_StageId
    ON portal."CandidatePipelines" ("StageId");
