-- ============================================================
-- master.Cities — Cities by state / country
-- ============================================================

CREATE TABLE IF NOT EXISTS master."Cities" (
    "Id"        UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "StateId"   UUID         NOT NULL REFERENCES master."States"("Id"),
    "CountryId" UUID         NOT NULL REFERENCES master."Countries"("Id"),
    "Name"      VARCHAR(150) NOT NULL,
    "IsMetro"   BOOLEAN      NOT NULL DEFAULT FALSE,
    "IsActive"  BOOLEAN      NOT NULL DEFAULT TRUE,
    "CreatedOn" TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_Cities_StateId
    ON master."Cities" ("StateId");

CREATE INDEX IF NOT EXISTS idx_Cities_CountryId
    ON master."Cities" ("CountryId");

-- GIN trigram index for fuzzy city search
CREATE INDEX IF NOT EXISTS idx_Cities_Name_Trgm
    ON master."Cities" USING GIN ("Name" gin_trgm_ops);
