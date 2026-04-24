-- ============================================================
-- portal.JobPostingBenefits — Benefits listed per job
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."JobPostingBenefits" (
    "Id"           UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"     UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "JobPostingId" UUID        NOT NULL REFERENCES portal."JobPostings"("Id") ON DELETE CASCADE,
    "Benefit"      VARCHAR(200) NOT NULL,
    "SortOrder"    SMALLINT    NOT NULL DEFAULT 0
);

CREATE INDEX IF NOT EXISTS idx_JobPostingBenefits_JobPostingId
    ON portal."JobPostingBenefits" ("JobPostingId");
