-- ============================================================
-- portal.JobPostings — Core job listing table
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."JobPostings" (
    "Id"                 UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"           UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "CompanyId"          UUID          NOT NULL REFERENCES portal."Companies"("Id"),
    "Title"              VARCHAR(300)  NOT NULL,
    "Slug"               VARCHAR(350)  NOT NULL,
    "Description"        TEXT          NOT NULL,
    "Responsibilities"   TEXT,
    "Requirements"       TEXT,
    "NiceToHave"         TEXT,
    "JobType"            portal."JobType"          NOT NULL,
    "WorkMode"           portal."WorkMode"         NOT NULL DEFAULT 'OnSite',
    "ExperienceLevel"    portal."ExperienceLevel"  NOT NULL,
    "MinExperienceYears" SMALLINT,
    "MaxExperienceYears" SMALLINT,
    "MinSalary"          NUMERIC(15,2),
    "MaxSalary"          NUMERIC(15,2),
    "CurrencyCode"       CHAR(3),
    "IsSalaryHidden"     BOOLEAN       NOT NULL DEFAULT FALSE,
    "CategoryId"         UUID          REFERENCES master."JobCategories"("Id"),
    "SubCategoryId"      UUID          REFERENCES master."JobSubCategories"("Id"),
    "CityId"             UUID          REFERENCES master."Cities"("Id"),
    "StateId"            UUID          REFERENCES master."States"("Id"),
    "CountryId"          UUID          REFERENCES master."Countries"("Id"),
    "Status"             portal."JobPostingStatus" NOT NULL DEFAULT 'Draft',
    "OpeningsCount"      SMALLINT      NOT NULL DEFAULT 1,
    "ApplicationsCount"  INT           NOT NULL DEFAULT 0,
    "ViewsCount"         INT           NOT NULL DEFAULT 0,
    "PublishedAt"        TIMESTAMPTZ,
    "ExpiresAt"          TIMESTAMPTZ,
    "IsUrgent"           BOOLEAN       NOT NULL DEFAULT FALSE,
    "IsFeatured"         BOOLEAN       NOT NULL DEFAULT FALSE,
    "IsRemote"           BOOLEAN       NOT NULL DEFAULT FALSE,
    "ApplicationEmail"   VARCHAR(320),
    "ApplicationUrl"     VARCHAR(500),
    "SearchVector"       TSVECTOR,     -- Updated by trigger
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_JobPostings_TenantId_Slug
    ON portal."JobPostings" ("TenantId", "Slug") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_JobPostings_TenantId_Status_PublishedAt
    ON portal."JobPostings" ("TenantId", "Status", "PublishedAt" DESC);

CREATE INDEX IF NOT EXISTS idx_JobPostings_CompanyId
    ON portal."JobPostings" ("CompanyId");

CREATE INDEX IF NOT EXISTS idx_JobPostings_CategoryId_Status
    ON portal."JobPostings" ("CategoryId", "Status");

CREATE INDEX IF NOT EXISTS idx_JobPostings_CountryId_Status
    ON portal."JobPostings" ("CountryId", "Status");

CREATE INDEX IF NOT EXISTS idx_JobPostings_CityId_Status
    ON portal."JobPostings" ("CityId", "Status");

CREATE INDEX IF NOT EXISTS idx_JobPostings_IsFeatured
    ON portal."JobPostings" ("IsFeatured", "Status") WHERE "IsFeatured" = TRUE;

CREATE INDEX IF NOT EXISTS idx_JobPostings_ExpiresAt
    ON portal."JobPostings" ("ExpiresAt") WHERE "Status" = 'Published';

-- GIN index for full-text search
CREATE INDEX IF NOT EXISTS idx_JobPostings_SearchVector
    ON portal."JobPostings" USING GIN ("SearchVector");

CALL apply_ModifiedOn_trigger('portal', 'JobPostings');

-- Trigger to update tsvector on insert/update
CREATE OR REPLACE TRIGGER trg_JobPostings_SearchVector
    BEFORE INSERT OR UPDATE OF "Title", "Description", "Requirements"
    ON portal."JobPostings"
    FOR EACH ROW EXECUTE FUNCTION portal.update_JobPostings_SearchVector();
