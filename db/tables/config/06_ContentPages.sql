-- ============================================================
-- config.ContentPages — CMS pages (About, Privacy, ToS)
-- ============================================================

CREATE TABLE IF NOT EXISTS config."ContentPages" (
    "Id"              UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"        UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "Slug"            VARCHAR(200) NOT NULL,   -- 'about', 'privacy', 'terms'
    "Title"           VARCHAR(300) NOT NULL,
    "Content"         TEXT         NOT NULL,
    "MetaTitle"       VARCHAR(200),
    "MetaDescription" VARCHAR(500),
    "IsPublished"     BOOLEAN      NOT NULL DEFAULT FALSE,
    "PublishedAt"     TIMESTAMPTZ,
    "IsGlobal"        BOOLEAN      NOT NULL DEFAULT FALSE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_ContentPages_TenantId_Slug
    ON config."ContentPages" ("TenantId", "Slug") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_ContentPages_TenantId_IsPublished
    ON config."ContentPages" ("TenantId", "IsPublished");

CALL apply_ModifiedOn_trigger('config', 'ContentPages');
