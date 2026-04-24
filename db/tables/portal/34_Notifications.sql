-- ============================================================
-- portal.Notifications — In-app notification feed
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."Notifications" (
    "Id"            UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"      UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "UserId"        UUID          NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "Title"         VARCHAR(300)  NOT NULL,
    "Body"          TEXT          NOT NULL,
    "Type"          portal."NotificationType" NOT NULL,
    "DeliveryScope" portal."NotificationDeliveryScope" NOT NULL DEFAULT 'Private',
    "Channel"       portal."NotificationChannel" NOT NULL DEFAULT 'InApp',
    "IsRead"        BOOLEAN       NOT NULL DEFAULT FALSE,
    "ReadAt"        TIMESTAMPTZ,
    "ActionUrl"     VARCHAR(500),
    "Data"          JSONB,
    "ExpiresAt"     TIMESTAMPTZ,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_Notifications_UserId_IsRead_CreatedOn
    ON portal."Notifications" ("UserId", "IsRead", "CreatedOn" DESC);

CREATE INDEX IF NOT EXISTS idx_Notifications_TenantId_CreatedOn
    ON portal."Notifications" ("TenantId", "CreatedOn" DESC);

CREATE INDEX IF NOT EXISTS idx_Notifications_ExpiresAt
    ON portal."Notifications" ("ExpiresAt") WHERE "ExpiresAt" IS NOT NULL;

CALL apply_ModifiedOn_trigger('portal', 'Notifications');
