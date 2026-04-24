-- ============================================================
-- portal.SkillAssessmentResults — Assessment results per user
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."SkillAssessmentResults" (
    "Id"           UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"     UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "AssessmentId" UUID        NOT NULL REFERENCES portal."SkillAssessments"("Id"),
    "UserId"       UUID        NOT NULL REFERENCES auth."Users"("Id"),
    "Score"        SMALLINT    NOT NULL,
    "TotalScore"   SMALLINT    NOT NULL,
    "Percentage"   SMALLINT    NOT NULL,
    "IsPassed"     BOOLEAN     NOT NULL,
    "TimeTaken"    SMALLINT,               -- seconds
    "Answers"      JSONB,                  -- detailed answer log
    "CompletedAt"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ExpiresAt"    TIMESTAMPTZ,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

-- Unique passed result per user per assessment
CREATE UNIQUE INDEX IF NOT EXISTS idx_SkillAssessmentResults_AssessmentId_UserId_Passed
    ON portal."SkillAssessmentResults" ("AssessmentId", "UserId")
    WHERE "IsPassed" = TRUE;

CREATE INDEX IF NOT EXISTS idx_SkillAssessmentResults_UserId
    ON portal."SkillAssessmentResults" ("UserId");

CREATE INDEX IF NOT EXISTS idx_SkillAssessmentResults_AssessmentId
    ON portal."SkillAssessmentResults" ("AssessmentId");

CALL apply_ModifiedOn_trigger('portal', 'SkillAssessmentResults');
