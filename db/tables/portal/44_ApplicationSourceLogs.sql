-- ============================================================
-- portal.ApplicationSourceLogs — UTM / source tracking per application
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."ApplicationSourceLogs" (
    "Id"            UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"      UUID         NOT NULL,
    "ApplicationId" UUID         NOT NULL REFERENCES portal."JobApplications"("Id") ON DELETE CASCADE,
    "Source"        VARCHAR(100) NOT NULL,   -- 'Portal', 'LinkedIn', 'Indeed', 'Referral'
    "ReferralCode"  VARCHAR(100),
    "UtmSource"     VARCHAR(200),
    "UtmMedium"     VARCHAR(200),
    "UtmCampaign"   VARCHAR(200),
    "LoggedAt"      TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_ApplicationSourceLogs_ApplicationId
    ON portal."ApplicationSourceLogs" ("ApplicationId");

CREATE INDEX IF NOT EXISTS idx_ApplicationSourceLogs_TenantId_Source
    ON portal."ApplicationSourceLogs" ("TenantId", "Source");
