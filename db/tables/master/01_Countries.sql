-- ============================================================
-- master.Countries — ISO 3166-1 country list
-- ============================================================

CREATE TABLE IF NOT EXISTS master."Countries" (
    "Id"           UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Code"         CHAR(2)     NOT NULL,       -- ISO 3166-1 alpha-2
    "Code3"        CHAR(3),                    -- ISO 3166-1 alpha-3
    "Name"         VARCHAR(100) NOT NULL,
    "PhoneCode"    VARCHAR(10),
    "CurrencyCode" VARCHAR(10),
    "IsActive"     BOOLEAN     NOT NULL DEFAULT TRUE,
    "SortOrder"    INT         NOT NULL DEFAULT 0,
    "CreatedOn"    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_Countries_Code
    ON master."Countries" ("Code");

CREATE INDEX IF NOT EXISTS idx_Countries_IsActive
    ON master."Countries" ("IsActive");
