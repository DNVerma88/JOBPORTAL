-- ============================================================
-- portal.Awards — Honors and awards per seeker
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."Awards" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"    UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "ProfileId"   UUID         NOT NULL REFERENCES portal."JobSeekerProfiles"("Id") ON DELETE CASCADE,
    "Title"       VARCHAR(200) NOT NULL,
    "Issuer"      VARCHAR(200),
    "IssueDate"   DATE,
    "Description" TEXT,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_Awards_ProfileId
    ON portal."Awards" ("ProfileId");

CALL apply_ModifiedOn_trigger('portal', 'Awards');
