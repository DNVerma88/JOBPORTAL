-- ============================================================
-- portal.UserActivityLogs — Immutable analytics event log
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."UserActivityLogs" (
    "Id"           UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"     UUID         NOT NULL,
    "UserId"       UUID         REFERENCES auth."Users"("Id"),
    "ActivityType" VARCHAR(100) NOT NULL,   -- 'JobView', 'ProfileView', 'Search', 'Apply'
    "EntityId"     UUID,
    "EntityType"   VARCHAR(100),
    "IpAddress"    VARCHAR(45),
    "UserAgent"    TEXT,
    "SessionId"    VARCHAR(200),
    "Data"         JSONB,
    "CreatedOn"    TIMESTAMPTZ  NOT NULL DEFAULT NOW()
    -- Immutable log — no audit columns, no ModifiedOn trigger
);

CREATE INDEX IF NOT EXISTS idx_UserActivityLogs_TenantId_CreatedOn
    ON portal."UserActivityLogs" ("TenantId", "CreatedOn" DESC);

CREATE INDEX IF NOT EXISTS idx_UserActivityLogs_UserId
    ON portal."UserActivityLogs" ("UserId") WHERE "UserId" IS NOT NULL;

CREATE INDEX IF NOT EXISTS idx_UserActivityLogs_EntityType_EntityId
    ON portal."UserActivityLogs" ("EntityType", "EntityId") WHERE "EntityId" IS NOT NULL;

CREATE INDEX IF NOT EXISTS idx_UserActivityLogs_ActivityType
    ON portal."UserActivityLogs" ("TenantId", "ActivityType");
