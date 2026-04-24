-- ============================================================
-- portal.JobApplications — Core application record
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."JobApplications" (
    "Id"             UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"       UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "JobPostingId"   UUID          NOT NULL REFERENCES portal."JobPostings"("Id"),
    "ApplicantId"    UUID          NOT NULL REFERENCES auth."Users"("Id"),
    "ProfileId"      UUID          REFERENCES portal."JobSeekerProfiles"("Id"),
    "ResumeId"       UUID          REFERENCES portal."Resumes"("Id"),
    "CoverLetter"    TEXT,
    "ExpectedSalary" NUMERIC(15,2),
    "Status"         portal."ApplicationStatus" NOT NULL DEFAULT 'Pending',
    "AppliedVia"     VARCHAR(50),   -- 'Portal', 'Email', 'LinkedIn', 'Referral'
    "ReferralCode"   VARCHAR(50),
    "IsViewed"       BOOLEAN       NOT NULL DEFAULT FALSE,
    "ViewedAt"       TIMESTAMPTZ,
    "Source"         VARCHAR(100),
    "CustomData"     JSONB,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_JobApplications_JobPostingId_ApplicantId
    ON portal."JobApplications" ("JobPostingId", "ApplicantId") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_JobApplications_TenantId
    ON portal."JobApplications" ("TenantId");

CREATE INDEX IF NOT EXISTS idx_JobApplications_JobPostingId_Status
    ON portal."JobApplications" ("JobPostingId", "Status");

CREATE INDEX IF NOT EXISTS idx_JobApplications_ApplicantId_CreatedOn
    ON portal."JobApplications" ("ApplicantId", "CreatedOn" DESC);

CREATE INDEX IF NOT EXISTS idx_JobApplications_Status
    ON portal."JobApplications" ("TenantId", "Status");

CALL apply_ModifiedOn_trigger('portal', 'JobApplications');
