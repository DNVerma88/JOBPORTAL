-- ============================================================
-- portal.JobPostingSkills — Skills required for a job
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."JobPostingSkills" (
    "Id"           UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"     UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "JobPostingId" UUID        NOT NULL REFERENCES portal."JobPostings"("Id") ON DELETE CASCADE,
    "SkillId"      UUID        NOT NULL REFERENCES master."Skills"("Id"),
    "Proficiency"  VARCHAR(50),   -- 'Beginner', 'Intermediate', 'Expert'
    "IsRequired"   BOOLEAN     NOT NULL DEFAULT TRUE
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_JobPostingSkills_JobPostingId_SkillId
    ON portal."JobPostingSkills" ("JobPostingId", "SkillId");

CREATE INDEX IF NOT EXISTS idx_JobPostingSkills_JobPostingId
    ON portal."JobPostingSkills" ("JobPostingId");

CREATE INDEX IF NOT EXISTS idx_JobPostingSkills_SkillId
    ON portal."JobPostingSkills" ("SkillId");
