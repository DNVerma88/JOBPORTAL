-- ============================================================
-- portal.Resumes — Uploaded resume metadata
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."Resumes" (
    "Id"          UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"    UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "ProfileId"   UUID        NOT NULL REFERENCES portal."JobSeekerProfiles"("Id") ON DELETE CASCADE,
    "Name"        VARCHAR(200) NOT NULL,   -- e.g. "My Software Dev Resume"
    "IsDefault"   BOOLEAN     NOT NULL DEFAULT FALSE,
    "ParsedData"  JSONB,                   -- AI-parsed resume data
    "ParsedAt"    TIMESTAMPTZ,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_Resumes_ProfileId
    ON portal."Resumes" ("ProfileId");

CALL apply_ModifiedOn_trigger('portal', 'Resumes');
