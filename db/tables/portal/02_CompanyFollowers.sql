-- ============================================================
-- portal.CompanyFollowers — Job seeker follows company
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."CompanyFollowers" (
    "Id"        UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"  UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "CompanyId" UUID        NOT NULL REFERENCES portal."Companies"("Id") ON DELETE CASCADE,
    "UserId"    UUID        NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "CreatedOn" TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_CompanyFollowers_CompanyId_UserId
    ON portal."CompanyFollowers" ("CompanyId", "UserId");

CREATE INDEX IF NOT EXISTS idx_CompanyFollowers_UserId
    ON portal."CompanyFollowers" ("UserId");

CREATE INDEX IF NOT EXISTS idx_CompanyFollowers_CompanyId
    ON portal."CompanyFollowers" ("CompanyId");
