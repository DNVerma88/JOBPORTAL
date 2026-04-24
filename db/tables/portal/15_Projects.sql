-- ============================================================
-- portal.Projects — Portfolio projects per seeker
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."Projects" (
    "Id"            UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"      UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "ProfileId"     UUID         NOT NULL REFERENCES portal."JobSeekerProfiles"("Id") ON DELETE CASCADE,
    "Title"         VARCHAR(200) NOT NULL,
    "Description"   TEXT,
    "ProjectUrl"    VARCHAR(500),
    "RepositoryUrl" VARCHAR(500),
    "StartDate"     DATE,
    "EndDate"       DATE,
    "IsCurrent"     BOOLEAN      NOT NULL DEFAULT FALSE,
    "TechStack"     TEXT[],
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_Projects_ProfileId
    ON portal."Projects" ("ProfileId");

CALL apply_ModifiedOn_trigger('portal', 'Projects');
