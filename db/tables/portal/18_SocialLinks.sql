-- ============================================================
-- portal.SocialLinks — Social profile links per seeker
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."SocialLinks" (
    "Id"        UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"  UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "ProfileId" UUID         NOT NULL REFERENCES portal."JobSeekerProfiles"("Id") ON DELETE CASCADE,
    "Platform"  VARCHAR(50)  NOT NULL,   -- 'LinkedIn', 'GitHub', 'Twitter', 'Behance', 'Dribbble'
    "Url"       VARCHAR(500) NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_SocialLinks_ProfileId_Platform
    ON portal."SocialLinks" ("ProfileId", "Platform");

CREATE INDEX IF NOT EXISTS idx_SocialLinks_ProfileId
    ON portal."SocialLinks" ("ProfileId");
