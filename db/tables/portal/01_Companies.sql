-- ============================================================
-- portal.Companies — Employer company profiles
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."Companies" (
    "Id"                  UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"            UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "Name"                VARCHAR(200)  NOT NULL,
    "Slug"                VARCHAR(200)  NOT NULL,
    "LogoUrl"             VARCHAR(500),
    "CoverImageUrl"       VARCHAR(500),
    "WebsiteUrl"          VARCHAR(500),
    "Description"         TEXT,
    "TagLine"             VARCHAR(300),
    "FoundedYear"         INT,
    "CompanySize"         VARCHAR(50),  -- '1-10', '11-50', '51-200', '201-500', '501-1000', '1000+'
    "IndustryId"          UUID          REFERENCES master."Industries"("Id"),
    "CountryId"           UUID          REFERENCES master."Countries"("Id"),
    "CityId"              UUID          REFERENCES master."Cities"("Id"),
    "HeadquartersAddress" TEXT,
    "IsVerified"          BOOLEAN       NOT NULL DEFAULT FALSE,
    "IsActive"            BOOLEAN       NOT NULL DEFAULT TRUE,
    "LinkedInUrl"         VARCHAR(500),
    "TwitterUrl"          VARCHAR(500),
    "GlassdoorUrl"        VARCHAR(500),
    "AverageRating"       NUMERIC(3,2),
    "TotalReviews"        INT           NOT NULL DEFAULT 0,
    "TotalFollowers"      INT           NOT NULL DEFAULT 0,
    -- Audit columns
    "CreatedBy"     UUID          NOT NULL,
    "CreatedOn"     TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
    "ModifiedBy"    UUID,
    "ModifiedOn"    TIMESTAMPTZ,
    "IsDeleted"     BOOLEAN       NOT NULL DEFAULT FALSE,
    "DeletedBy"     UUID,
    "DeletedOn"     TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_Companies_TenantId_Slug
    ON portal."Companies" ("TenantId", "Slug") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_Companies_TenantId
    ON portal."Companies" ("TenantId");

CREATE INDEX IF NOT EXISTS idx_Companies_IndustryId
    ON portal."Companies" ("IndustryId");

CREATE INDEX IF NOT EXISTS idx_Companies_CountryId
    ON portal."Companies" ("CountryId");

CREATE INDEX IF NOT EXISTS idx_Companies_CityId
    ON portal."Companies" ("CityId");

-- GIN trigram for fuzzy company name search
CREATE INDEX IF NOT EXISTS idx_Companies_Name_Trgm
    ON portal."Companies" USING GIN ("Name" gin_trgm_ops);

CALL apply_ModifiedOn_trigger('portal', 'Companies');
