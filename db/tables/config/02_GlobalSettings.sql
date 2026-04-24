-- ============================================================
-- config.GlobalSettings — Platform-wide key-value config
-- ============================================================

CREATE TABLE IF NOT EXISTS config."GlobalSettings" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Key"         VARCHAR(200) NOT NULL,
    "Value"       TEXT         NOT NULL,
    "DataType"    VARCHAR(20)  NOT NULL DEFAULT 'String',
    "Description" VARCHAR(500),
    "IsPublic"    BOOLEAN      NOT NULL DEFAULT FALSE,   -- readable by unauthenticated clients
    "ModifiedBy"  UUID,
    "ModifiedOn"  TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_GlobalSettings_Key
    ON config."GlobalSettings" ("Key");

CREATE INDEX IF NOT EXISTS idx_GlobalSettings_IsPublic
    ON config."GlobalSettings" ("IsPublic");
