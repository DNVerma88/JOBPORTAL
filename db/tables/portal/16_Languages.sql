-- ============================================================
-- portal.Languages — Languages known per seeker
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."Languages" (
    "Id"          UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"    UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "ProfileId"   UUID        NOT NULL REFERENCES portal."JobSeekerProfiles"("Id") ON DELETE CASCADE,
    "LanguageId"  UUID        NOT NULL REFERENCES master."LanguageMaster"("Id"),
    "Proficiency" VARCHAR(50) NOT NULL   -- 'Native', 'Fluent', 'Professional', 'Basic'
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_Languages_ProfileId_LanguageId
    ON portal."Languages" ("ProfileId", "LanguageId");

CREATE INDEX IF NOT EXISTS idx_Languages_ProfileId
    ON portal."Languages" ("ProfileId");
