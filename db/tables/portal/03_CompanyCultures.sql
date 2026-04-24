-- ============================================================
-- portal.CompanyCultures — Culture tags per company
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."CompanyCultures" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"    UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "CompanyId"   UUID         NOT NULL REFERENCES portal."Companies"("Id") ON DELETE CASCADE,
    "Tag"         VARCHAR(100) NOT NULL,
    "Description" VARCHAR(300),
    -- Audit columns
    "CreatedBy"   UUID         NOT NULL,
    "CreatedOn"   TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "ModifiedBy"  UUID,
    "ModifiedOn"  TIMESTAMPTZ,
    "IsDeleted"   BOOLEAN      NOT NULL DEFAULT FALSE,
    "DeletedBy"   UUID,
    "DeletedOn"   TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_CompanyCultures_CompanyId_Tag
    ON portal."CompanyCultures" ("CompanyId", "Tag") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_CompanyCultures_CompanyId
    ON portal."CompanyCultures" ("CompanyId");

CALL apply_ModifiedOn_trigger('portal', 'CompanyCultures');
