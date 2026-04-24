-- ============================================================
-- master.JobSubCategories — Sub-categories per job category
-- ============================================================

CREATE TABLE IF NOT EXISTS master."JobSubCategories" (
    "Id"         UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "CategoryId" UUID         NOT NULL REFERENCES master."JobCategories"("Id"),
    "Name"       VARCHAR(150) NOT NULL,
    "Slug"       VARCHAR(150) NOT NULL,
    "IsActive"   BOOLEAN      NOT NULL DEFAULT TRUE,
    "SortOrder"  INT          NOT NULL DEFAULT 0,
    "CreatedOn"  TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_JobSubCategories_Slug
    ON master."JobSubCategories" ("Slug");

CREATE INDEX IF NOT EXISTS idx_JobSubCategories_CategoryId
    ON master."JobSubCategories" ("CategoryId");
