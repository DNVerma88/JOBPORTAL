-- ============================================================
-- master.EducationLevels — Education level lookup
-- ============================================================

CREATE TABLE IF NOT EXISTS master."EducationLevels" (
    "Id"        UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Code"      VARCHAR(50) NOT NULL,
    "Name"      VARCHAR(100) NOT NULL,
    "SortOrder" INT         NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_EducationLevels_Code
    ON master."EducationLevels" ("Code");
