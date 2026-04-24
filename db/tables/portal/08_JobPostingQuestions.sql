-- ============================================================
-- portal.JobPostingQuestions — Custom application questions
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."JobPostingQuestions" (
    "Id"           UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"     UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "JobPostingId" UUID        NOT NULL REFERENCES portal."JobPostings"("Id") ON DELETE CASCADE,
    "QuestionText" TEXT        NOT NULL,
    "QuestionType" VARCHAR(50) NOT NULL,   -- 'Text', 'YesNo', 'MCQ', 'Number'
    "Options"      JSONB,                  -- for MCQ: ["Option A", "Option B"]
    "IsRequired"   BOOLEAN     NOT NULL DEFAULT FALSE,
    "SortOrder"    SMALLINT    NOT NULL DEFAULT 0,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_JobPostingQuestions_JobPostingId
    ON portal."JobPostingQuestions" ("JobPostingId");

CALL apply_ModifiedOn_trigger('portal', 'JobPostingQuestions');
