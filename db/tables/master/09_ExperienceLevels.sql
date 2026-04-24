-- ============================================================
-- master.ExperienceLevels — Experience level lookup
-- ============================================================

CREATE TABLE IF NOT EXISTS master."ExperienceLevels" (
    "Id"        UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Code"      VARCHAR(50) NOT NULL,
    "Name"      VARCHAR(100) NOT NULL,
    "MinYears"  INT,
    "MaxYears"  INT,
    "SortOrder" INT         NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_ExperienceLevels_Code
    ON master."ExperienceLevels" ("Code");
