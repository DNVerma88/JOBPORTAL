-- ============================================================
-- portal.JobSeekerProfiles — Extended job seeker profile
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."JobSeekerProfiles" (
    "Id"                    UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"              UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"                UUID          NOT NULL UNIQUE REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "Headline"              VARCHAR(300),
    "Summary"               TEXT,
    "CurrentJobTitle"       VARCHAR(200),
    "CurrentCompany"        VARCHAR(200),
    "TotalExperienceYears"  SMALLINT,
    "DateOfBirth"           DATE,
    "Gender"                portal."Gender",
    "Nationality"           VARCHAR(100),
    "CountryId"             UUID          REFERENCES master."Countries"("Id"),
    "CityId"                UUID          REFERENCES master."Cities"("Id"),
    "Address"               TEXT,
    "ProfileVisibility"     portal."ProfileVisibility" NOT NULL DEFAULT 'Public',
    "IsOpenToWork"          BOOLEAN       NOT NULL DEFAULT TRUE,
    "ExpectedSalaryMin"     NUMERIC(15,2),
    "ExpectedSalaryMax"     NUMERIC(15,2),
    "PreferredCurrencyCode" CHAR(3),
    "NoticePeriodDays"      SMALLINT,
    "LinkedInUrl"           VARCHAR(500),
    "GitHubUrl"             VARCHAR(500),
    "PortfolioUrl"          VARCHAR(500),
    "ProfileCompletionPct"  SMALLINT      NOT NULL DEFAULT 0,
    "IsVerified"            BOOLEAN       NOT NULL DEFAULT FALSE,
    "LastActiveAt"          TIMESTAMPTZ,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_JobSeekerProfiles_TenantId
    ON portal."JobSeekerProfiles" ("TenantId");

CREATE INDEX IF NOT EXISTS idx_JobSeekerProfiles_CountryId
    ON portal."JobSeekerProfiles" ("CountryId");

CREATE INDEX IF NOT EXISTS idx_JobSeekerProfiles_CityId
    ON portal."JobSeekerProfiles" ("CityId");

CREATE INDEX IF NOT EXISTS idx_JobSeekerProfiles_IsOpenToWork
    ON portal."JobSeekerProfiles" ("TenantId", "CountryId", "CityId")
    WHERE "IsOpenToWork" = TRUE AND "IsDeleted" = FALSE;

CALL apply_ModifiedOn_trigger('portal', 'JobSeekerProfiles');
