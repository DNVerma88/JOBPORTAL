-- ============================================================
-- portal.SavedJobs — Job seeker bookmarked jobs
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."SavedJobs" (
    "Id"           UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"     UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"       UUID        NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "JobPostingId" UUID        NOT NULL REFERENCES portal."JobPostings"("Id") ON DELETE CASCADE,
    "CreatedOn"    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_SavedJobs_UserId_JobPostingId
    ON portal."SavedJobs" ("UserId", "JobPostingId");

CREATE INDEX IF NOT EXISTS idx_SavedJobs_UserId
    ON portal."SavedJobs" ("UserId");
