-- ============================================================
-- Config schema — Additional indexes
-- ============================================================

CREATE INDEX IF NOT EXISTS idx_TenantSettings_TenantId_Key
    ON config."TenantSettings" ("TenantId", "Key")
    WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_FeatureFlags_TenantId_FlagKey_IsEnabled
    ON config."FeatureFlags" ("TenantId", "FlagKey", "IsEnabled")
    WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_Announcements_StartsAt_EndsAt_Active
    ON config."Announcements" ("StartsAt", "EndsAt", "IsActive")
    WHERE "IsActive" = TRUE AND "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_ContentPages_TenantId_Slug_Published
    ON config."ContentPages" ("TenantId", "Slug", "IsPublished")
    WHERE "IsDeleted" = FALSE;
