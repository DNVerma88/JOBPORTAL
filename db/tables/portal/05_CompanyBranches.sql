-- ============================================================
-- portal.CompanyBranches — Office / branch locations
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."CompanyBranches" (
    "Id"             UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"       UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "CompanyId"      UUID         NOT NULL REFERENCES portal."Companies"("Id") ON DELETE CASCADE,
    "Name"           VARCHAR(200) NOT NULL,
    "Address"        TEXT         NOT NULL,
    "CityId"         UUID         REFERENCES master."Cities"("Id"),
    "StateId"        UUID         REFERENCES master."States"("Id"),
    "CountryId"      UUID         REFERENCES master."Countries"("Id"),
    "Email"          VARCHAR(320),
    "Phone"          VARCHAR(20),
    "IsHeadquarters" BOOLEAN      NOT NULL DEFAULT FALSE,
    "IsActive"       BOOLEAN      NOT NULL DEFAULT TRUE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_CompanyBranches_CompanyId
    ON portal."CompanyBranches" ("CompanyId");

CALL apply_ModifiedOn_trigger('portal', 'CompanyBranches');
