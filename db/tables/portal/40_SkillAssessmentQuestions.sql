-- ============================================================
-- portal.SkillAssessmentQuestions — Questions in an assessment
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."SkillAssessmentQuestions" (
    "Id"            UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"      UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "AssessmentId"  UUID        NOT NULL REFERENCES portal."SkillAssessments"("Id") ON DELETE CASCADE,
    "QuestionText"  TEXT        NOT NULL,
    "Options"       JSONB       NOT NULL,   -- ["A", "B", "C", "D"]
    "CorrectAnswer" VARCHAR(10) NOT NULL,
    "Explanation"   TEXT,
    "Difficulty"    VARCHAR(20) NOT NULL DEFAULT 'Medium',  -- 'Easy', 'Medium', 'Hard'
    "Points"        SMALLINT    NOT NULL DEFAULT 1,
    "SortOrder"     SMALLINT    NOT NULL DEFAULT 0,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_SkillAssessmentQuestions_AssessmentId
    ON portal."SkillAssessmentQuestions" ("AssessmentId");

CALL apply_ModifiedOn_trigger('portal', 'SkillAssessmentQuestions');
