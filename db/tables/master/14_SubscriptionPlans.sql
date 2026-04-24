-- ============================================================
-- master.SubscriptionPlans — Subscription tier definitions
-- ============================================================

CREATE TABLE IF NOT EXISTS master."SubscriptionPlans" (
    "Id"                     UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Tier"                   VARCHAR(50)               NOT NULL,
    "Name"                   VARCHAR(100) NOT NULL,
    "Description"            TEXT,
    "PriceMonthly"           NUMERIC(10,2) NOT NULL DEFAULT 0,
    "PriceAnnually"          NUMERIC(10,2) NOT NULL DEFAULT 0,
    "CurrencyCode"           CHAR(3)      NOT NULL DEFAULT 'USD',
    "MaxJobPostings"         INT,         -- NULL = unlimited
    "MaxUsers"               INT,
    "MaxResumeViews"         INT,
    "JobPostingDurationDays" INT          NOT NULL DEFAULT 30,
    "IsActive"               BOOLEAN      NOT NULL DEFAULT TRUE,
    "SortOrder"              INT          NOT NULL DEFAULT 0,
    "CreatedOn"              TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_SubscriptionPlans_Tier
    ON master."SubscriptionPlans" ("Tier");
