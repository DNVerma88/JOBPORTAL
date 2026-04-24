-- ============================================================
-- master.NotificationTemplates — Email/push template definitions
-- ============================================================

CREATE TABLE IF NOT EXISTS master."NotificationTemplates" (
    "Id"         UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Key"        VARCHAR(100) NOT NULL,   -- 'email.verification', 'job.alert', etc.
    "Channel"    portal."NotificationChannel" NOT NULL,
    "Subject"    VARCHAR(300),
    "BodyHtml"   TEXT,
    "BodyText"   TEXT,
    "Variables"  JSONB,                   -- expected vars: {"name": "string"}
    "IsActive"   BOOLEAN      NOT NULL DEFAULT TRUE,
    "CreatedOn"  TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "ModifiedOn" TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_NotificationTemplates_Key
    ON master."NotificationTemplates" ("Key");

CREATE INDEX IF NOT EXISTS idx_NotificationTemplates_Channel
    ON master."NotificationTemplates" ("Channel");
