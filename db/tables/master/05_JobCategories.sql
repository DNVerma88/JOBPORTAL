-- ============================================================
-- master.JobCategories — Job category (e.g. Software Development)
-- ============================================================

CREATE TABLE IF NOT EXISTS master."JobCategories" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "IndustryId"  UUID         REFERENCES master."Industries"("Id"),
    "Name"        VARCHAR(150) NOT NULL,
    "Slug"        VARCHAR(150) NOT NULL,
    "Description" VARCHAR(500),
    "IconUrl"     VARCHAR(500),
    "IsActive"    BOOLEAN      NOT NULL DEFAULT TRUE,
    "SortOrder"   INT          NOT NULL DEFAULT 0,
    "CreatedOn"   TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_JobCategories_Slug
    ON master."JobCategories" ("Slug");

CREATE INDEX IF NOT EXISTS idx_JobCategories_IndustryId
    ON master."JobCategories" ("IndustryId");

CREATE INDEX IF NOT EXISTS idx_JobCategories_IsActive
    ON master."JobCategories" ("IsActive");
