-- ============================================================
-- portal.NotificationPreferences — User notification channel preferences
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."NotificationPreferences" (
    "Id"               UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"         UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"           UUID        NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "NotificationType" portal."NotificationType" NOT NULL,
    "Channel"          portal."NotificationChannel" NOT NULL,
    "IsEnabled"        BOOLEAN     NOT NULL DEFAULT TRUE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_NotificationPreferences_UserId_Type_Channel
    ON portal."NotificationPreferences" ("UserId", "NotificationType", "Channel");

CREATE INDEX IF NOT EXISTS idx_NotificationPreferences_UserId
    ON portal."NotificationPreferences" ("UserId");

CALL apply_ModifiedOn_trigger('portal', 'NotificationPreferences');
