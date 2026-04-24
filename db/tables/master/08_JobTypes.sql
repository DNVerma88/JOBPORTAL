-- ============================================================
-- master.JobTypes — Employment type lookup
-- ============================================================

CREATE TABLE IF NOT EXISTS master."JobTypes" (
    "Id"        UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Code"      VARCHAR(50) NOT NULL,
    "Name"      VARCHAR(100) NOT NULL,
    "IsActive"  BOOLEAN     NOT NULL DEFAULT TRUE,
    "SortOrder" INT         NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_JobTypes_Code
    ON master."JobTypes" ("Code");
