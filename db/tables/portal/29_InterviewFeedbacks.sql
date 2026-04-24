-- ============================================================
-- portal.InterviewFeedbacks — Interviewer feedback per round
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."InterviewFeedbacks" (
    "Id"                    UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"              UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "RoundId"               UUID        NOT NULL REFERENCES portal."InterviewRounds"("Id") ON DELETE CASCADE,
    "InterviewerId"         UUID        NOT NULL REFERENCES auth."Users"("Id"),
    "OverallRating"         SMALLINT    CHECK ("OverallRating" BETWEEN 1 AND 5),
    "Recommendation"        VARCHAR(50),   -- 'StrongHire', 'Hire', 'NoHire', 'StrongNoHire'
    "TechnicalSkills"       SMALLINT    CHECK ("TechnicalSkills" BETWEEN 1 AND 5),
    "CommunicationSkills"   SMALLINT    CHECK ("CommunicationSkills" BETWEEN 1 AND 5),
    "ProblemSolving"        SMALLINT    CHECK ("ProblemSolving" BETWEEN 1 AND 5),
    "CultureFit"            SMALLINT    CHECK ("CultureFit" BETWEEN 1 AND 5),
    "Notes"                 TEXT,
    "IsSharedWithCandidate" BOOLEAN     NOT NULL DEFAULT FALSE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_InterviewFeedbacks_RoundId_InterviewerId
    ON portal."InterviewFeedbacks" ("RoundId", "InterviewerId");

CREATE INDEX IF NOT EXISTS idx_InterviewFeedbacks_RoundId
    ON portal."InterviewFeedbacks" ("RoundId");

CALL apply_ModifiedOn_trigger('portal', 'InterviewFeedbacks');
