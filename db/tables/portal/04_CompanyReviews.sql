-- ============================================================
-- portal.CompanyReviews — Employee company reviews
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."CompanyReviews" (
    "Id"              UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"        UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "CompanyId"       UUID        NOT NULL REFERENCES portal."Companies"("Id"),
    "UserId"          UUID        NOT NULL REFERENCES auth."Users"("Id"),
    "IsAnonymous"     BOOLEAN     NOT NULL DEFAULT FALSE,
    "Title"           VARCHAR(200) NOT NULL,
    "ProsText"        TEXT,
    "ConsText"        TEXT,
    "OverallRating"   SMALLINT    NOT NULL CHECK ("OverallRating" BETWEEN 1 AND 5),
    "WorkLifeBalance" SMALLINT    CHECK ("WorkLifeBalance" BETWEEN 1 AND 5),
    "Compensation"    SMALLINT    CHECK ("Compensation" BETWEEN 1 AND 5),
    "JobSecurity"     SMALLINT    CHECK ("JobSecurity" BETWEEN 1 AND 5),
    "Management"      SMALLINT    CHECK ("Management" BETWEEN 1 AND 5),
    "CultureRating"   SMALLINT    CHECK ("CultureRating" BETWEEN 1 AND 5),
    "IsVerified"      BOOLEAN     NOT NULL DEFAULT FALSE,
    "IsApproved"      BOOLEAN     NOT NULL DEFAULT FALSE,
    "EmploymentType"  VARCHAR(50),        -- 'Current', 'Former'
    "WorkedFrom"      DATE,
    "WorkedUntil"     DATE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_CompanyReviews_CompanyId
    ON portal."CompanyReviews" ("CompanyId");

CREATE INDEX IF NOT EXISTS idx_CompanyReviews_UserId
    ON portal."CompanyReviews" ("UserId");

CREATE INDEX IF NOT EXISTS idx_CompanyReviews_CompanyId_OverallRating
    ON portal."CompanyReviews" ("CompanyId", "OverallRating") WHERE "IsApproved" = TRUE;

CALL apply_ModifiedOn_trigger('portal', 'CompanyReviews');
