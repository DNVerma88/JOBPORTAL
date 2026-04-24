-- ============================================================
-- master.CurrencyMaster — ISO 4217 currency reference
-- ============================================================

CREATE TABLE IF NOT EXISTS master."CurrencyMaster" (
    "Id"       UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Code"     CHAR(3)     NOT NULL,     -- ISO 4217: 'USD', 'INR', 'EUR'
    "Name"     VARCHAR(100) NOT NULL,
    "Symbol"   VARCHAR(10)  NOT NULL,
    "IsActive" BOOLEAN     NOT NULL DEFAULT TRUE
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_CurrencyMaster_Code
    ON master."CurrencyMaster" ("Code");
