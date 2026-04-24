-- ============================================================
-- master.Industries — Industry verticals
-- ============================================================

CREATE TABLE IF NOT EXISTS master."Industries" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Name"        VARCHAR(150) NOT NULL,
    "Description" VARCHAR(500),
    "IconUrl"     VARCHAR(500),
    "IsActive"    BOOLEAN      NOT NULL DEFAULT TRUE,
    "SortOrder"   INT          NOT NULL DEFAULT 0,
    "CreatedOn"   TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_Industries_Name
    ON master."Industries" ("Name");

CREATE INDEX IF NOT EXISTS idx_Industries_IsActive
    ON master."Industries" ("IsActive");
