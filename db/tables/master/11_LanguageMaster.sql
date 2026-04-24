-- ============================================================
-- master.LanguageMaster — Language reference data (BCP 47)
-- ============================================================

CREATE TABLE IF NOT EXISTS master."LanguageMaster" (
    "Id"         UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Code"       CHAR(5)     NOT NULL,     -- BCP 47: 'en', 'hi', 'fr', 'zh-CN'
    "Name"       VARCHAR(100) NOT NULL,
    "NativeName" VARCHAR(100),
    "IsActive"   BOOLEAN     NOT NULL DEFAULT TRUE,
    "SortOrder"  INT         NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_LanguageMaster_Code
    ON master."LanguageMaster" ("Code");
