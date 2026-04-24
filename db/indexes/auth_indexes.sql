-- ============================================================
-- Auth schema — Additional composite/partial indexes
-- ============================================================

-- Active refresh tokens by user (most common lookup)
CREATE INDEX IF NOT EXISTS idx_RefreshTokens_UserId_Active_Expires
    ON auth."RefreshTokens" ("UserId", "ExpiresOn" DESC)
    WHERE "IsRevoked" = FALSE AND "IsDeleted" = FALSE;

-- Active sessions by user
CREATE INDEX IF NOT EXISTS idx_UserSessions_UserId_Active
    ON auth."UserSessions" ("UserId", "LastActivityAt" DESC)
    WHERE "IsActive" = TRUE AND "IsDeleted" = FALSE;

-- Audit log — entity type + date range analytic queries
CREATE INDEX IF NOT EXISTS idx_AuditLogs_TenantId_EntityType_CreatedOn
    ON auth."AuditLogs" ("TenantId", "EntityType", "CreatedOn" DESC);

-- Users: fast lockout check by email
CREATE INDEX IF NOT EXISTS idx_Users_TenantId_NormalizedEmail_IsActive
    ON auth."Users" ("TenantId", "NormalizedEmail", "IsActive")
    WHERE "IsDeleted" = FALSE;
