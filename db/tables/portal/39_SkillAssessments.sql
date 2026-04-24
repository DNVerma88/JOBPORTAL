-- ============================================================
-- portal.SkillAssessments — Quiz / assessment definitions
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."SkillAssessments" (
    "Id"              UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"        UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "SkillId"         UUID        REFERENCES master."Skills"("Id"),
    "Title"           VARCHAR(300) NOT NULL,
    "Description"     TEXT,
    "DurationMinutes" SMALLINT    NOT NULL DEFAULT 30,
    "TotalQuestions"  SMALLINT    NOT NULL DEFAULT 10,
    "PassingScore"    SMALLINT    NOT NULL DEFAULT 70,
    "IsActive"        BOOLEAN     NOT NULL DEFAULT TRUE,
    "IsPublic"        BOOLEAN     NOT NULL DEFAULT TRUE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_SkillAssessments_TenantId_SkillId
    ON portal."SkillAssessments" ("TenantId", "SkillId");

CREATE INDEX IF NOT EXISTS idx_SkillAssessments_IsActive_IsPublic
    ON portal."SkillAssessments" ("IsActive", "IsPublic");

CALL apply_ModifiedOn_trigger('portal', 'SkillAssessments');
