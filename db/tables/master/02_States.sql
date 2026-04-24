-- ============================================================
-- master.States — States / provinces by country
-- ============================================================

CREATE TABLE IF NOT EXISTS master."States" (
    "Id"        UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "CountryId" UUID         NOT NULL REFERENCES master."Countries"("Id"),
    "Code"      VARCHAR(10)  NOT NULL,
    "Name"      VARCHAR(100) NOT NULL,
    "IsActive"  BOOLEAN      NOT NULL DEFAULT TRUE,
    "CreatedOn" TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_States_CountryId_Code
    ON master."States" ("CountryId", "Code");

CREATE INDEX IF NOT EXISTS idx_States_CountryId
    ON master."States" ("CountryId");
