-- ============================================================
-- master.Skills — Global skills dictionary
-- ============================================================

CREATE TABLE IF NOT EXISTS master."Skills" (
    "Id"             UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Name"           VARCHAR(100) NOT NULL,
    "NormalizedName" VARCHAR(100) NOT NULL,
    "Slug"           VARCHAR(100) NOT NULL,
    "Category"       VARCHAR(50),   -- 'Technical', 'Soft', 'Domain'
    "IsActive"       BOOLEAN      NOT NULL DEFAULT TRUE,
    "UsageCount"     INT          NOT NULL DEFAULT 0,
    "CreatedOn"      TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_Skills_NormalizedName
    ON master."Skills" ("NormalizedName");

CREATE UNIQUE INDEX IF NOT EXISTS idx_Skills_Slug
    ON master."Skills" ("Slug");

-- GIN trigram for fuzzy skill search
CREATE INDEX IF NOT EXISTS idx_Skills_Name_Trgm
    ON master."Skills" USING GIN ("Name" gin_trgm_ops);

CREATE INDEX IF NOT EXISTS idx_Skills_UsageCount
    ON master."Skills" ("UsageCount" DESC);
