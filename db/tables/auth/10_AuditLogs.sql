-- ============================================================
-- auth.AuditLogs — Immutable write-action audit trail
-- ============================================================

CREATE TABLE IF NOT EXISTS auth."AuditLogs" (
    "Id"            UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"      UUID         NOT NULL,
    "UserId"        UUID,                        -- NULL for system/background actions
    "Action"        VARCHAR(100) NOT NULL,        -- 'Created', 'Updated', 'Deleted', 'Login'
    "EntityType"    VARCHAR(100) NOT NULL,        -- 'User', 'JobPosting', etc.
    "EntityId"      UUID,
    "OldValues"     JSONB,
    "NewValues"     JSONB,
    "IpAddress"     VARCHAR(45),
    "UserAgent"     TEXT,
    "CorrelationId" VARCHAR(100),
    "CreatedOn"     TIMESTAMPTZ  NOT NULL DEFAULT NOW()
    -- Intentionally no ModifiedOn: audit logs are immutable
);

CREATE INDEX IF NOT EXISTS idx_AuditLogs_TenantId
    ON auth."AuditLogs" ("TenantId");

CREATE INDEX IF NOT EXISTS idx_AuditLogs_UserId
    ON auth."AuditLogs" ("UserId") WHERE "UserId" IS NOT NULL;

CREATE INDEX IF NOT EXISTS idx_AuditLogs_EntityType_EntityId
    ON auth."AuditLogs" ("EntityType", "EntityId");

CREATE INDEX IF NOT EXISTS idx_AuditLogs_TenantId_CreatedOn
    ON auth."AuditLogs" ("TenantId", "CreatedOn" DESC);
