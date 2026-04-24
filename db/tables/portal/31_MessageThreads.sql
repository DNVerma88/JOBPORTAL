-- ============================================================
-- portal.MessageThreads — Recruiter ↔ Candidate conversation threads
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."MessageThreads" (
    "Id"                  UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"            UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "JobPostingId"        UUID         REFERENCES portal."JobPostings"("Id"),
    "ApplicationId"       UUID         REFERENCES portal."JobApplications"("Id"),
    "RecruiterUserId"     UUID         NOT NULL REFERENCES auth."Users"("Id"),
    "CandidateUserId"     UUID         NOT NULL REFERENCES auth."Users"("Id"),
    "LastMessageAt"       TIMESTAMPTZ,
    "LastMessagePreview"  VARCHAR(300),
    "UnreadByRecruiter"   INT          NOT NULL DEFAULT 0,
    "UnreadByCandidate"   INT          NOT NULL DEFAULT 0,
    "IsArchived"          BOOLEAN      NOT NULL DEFAULT FALSE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

-- One thread per application
CREATE UNIQUE INDEX IF NOT EXISTS idx_MessageThreads_ApplicationId
    ON portal."MessageThreads" ("ApplicationId")
    WHERE "ApplicationId" IS NOT NULL AND "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_MessageThreads_TenantId
    ON portal."MessageThreads" ("TenantId");

CREATE INDEX IF NOT EXISTS idx_MessageThreads_RecruiterUserId_LastMessageAt
    ON portal."MessageThreads" ("RecruiterUserId", "LastMessageAt" DESC);

CREATE INDEX IF NOT EXISTS idx_MessageThreads_CandidateUserId_LastMessageAt
    ON portal."MessageThreads" ("CandidateUserId", "LastMessageAt" DESC);

CALL apply_ModifiedOn_trigger('portal', 'MessageThreads');
