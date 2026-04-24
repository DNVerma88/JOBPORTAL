-- ============================================================
-- portal.Educations — Academic history per seeker
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."Educations" (
    "Id"               UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"         UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "ProfileId"        UUID         NOT NULL REFERENCES portal."JobSeekerProfiles"("Id") ON DELETE CASCADE,
    "Degree"           VARCHAR(200) NOT NULL,
    "FieldOfStudy"     VARCHAR(200) NOT NULL,
    "Institution"      VARCHAR(300) NOT NULL,
    "EducationLevelId" UUID         REFERENCES master."EducationLevels"("Id"),
    "StartYear"        SMALLINT,
    "EndYear"          SMALLINT,
    "IsCurrent"        BOOLEAN      NOT NULL DEFAULT FALSE,
    "Grade"            VARCHAR(50),
    "Description"      TEXT,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_Educations_ProfileId
    ON portal."Educations" ("ProfileId");

CALL apply_ModifiedOn_trigger('portal', 'Educations');
