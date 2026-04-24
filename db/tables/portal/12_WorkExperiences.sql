-- ============================================================
-- portal.WorkExperiences — Employment history per seeker
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."WorkExperiences" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"    UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "ProfileId"   UUID         NOT NULL REFERENCES portal."JobSeekerProfiles"("Id") ON DELETE CASCADE,
    "JobTitle"    VARCHAR(200) NOT NULL,
    "CompanyName" VARCHAR(200) NOT NULL,
    "CompanyId"   UUID         REFERENCES portal."Companies"("Id"),
    "IndustryId"  UUID         REFERENCES master."Industries"("Id"),
    "CityId"      UUID         REFERENCES master."Cities"("Id"),
    "CountryId"   UUID         REFERENCES master."Countries"("Id"),
    "StartDate"   DATE         NOT NULL,
    "EndDate"     DATE,
    "IsCurrent"   BOOLEAN      NOT NULL DEFAULT FALSE,
    "Description" TEXT,
    "Skills"      TEXT[],      -- denormalized skill names for display speed
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_WorkExperiences_ProfileId
    ON portal."WorkExperiences" ("ProfileId");

CALL apply_ModifiedOn_trigger('portal', 'WorkExperiences');
