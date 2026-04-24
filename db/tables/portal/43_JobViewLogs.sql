-- ============================================================
-- portal.JobViewLogs — Deduplicated job detail view tracking
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."JobViewLogs" (
    "Id"           UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"     UUID         NOT NULL,
    "JobPostingId" UUID         NOT NULL REFERENCES portal."JobPostings"("Id") ON DELETE CASCADE,
    "UserId"       UUID         REFERENCES auth."Users"("Id"),
    "IpAddress"    VARCHAR(45),
    "ViewedAt"     TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "SourcePage"   VARCHAR(200)   -- 'Search', 'Home', 'Direct', 'Alert', 'Email'
);

CREATE INDEX IF NOT EXISTS idx_JobViewLogs_JobPostingId_ViewedAt
    ON portal."JobViewLogs" ("JobPostingId", "ViewedAt" DESC);

CREATE INDEX IF NOT EXISTS idx_JobViewLogs_UserId
    ON portal."JobViewLogs" ("UserId") WHERE "UserId" IS NOT NULL;
