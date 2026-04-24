-- ============================================================
-- Row Level Security (RLS) — Tenant isolation policies
-- ============================================================
-- NOTE: RLS is enforced at the DB level as a defence-in-depth layer.
-- The application also enforces TenantId filtering via EF Core global
-- query filters. Both layers together prevent data cross-contamination.
--
-- Usage (from application connection):
--   SELECT auth.set_tenant_context('tenant-uuid-here');
--   -- All subsequent queries on RLS-protected tables are automatically filtered.
--
-- For migrations / admin operations use bypass role:
--   SET LOCAL app.bypass_rls = 'true';
-- ============================================================

-- ── Helper function: set tenant context ──────────────────────
CREATE OR REPLACE FUNCTION auth.set_tenant_context(p_tenant_id UUID)
RETURNS VOID AS $$
BEGIN
    PERFORM set_config('app.current_tenant_id', p_tenant_id::TEXT, TRUE);
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- ── Helper function: bypass RLS check ────────────────────────
CREATE OR REPLACE FUNCTION auth.bypass_rls()
RETURNS BOOLEAN AS $$
BEGIN
    RETURN COALESCE(
        current_setting('app.bypass_rls', TRUE)::BOOLEAN,
        FALSE
    );
EXCEPTION
    WHEN others THEN RETURN FALSE;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- ── auth schema ───────────────────────────────────────────────
ALTER TABLE auth."Users"           ENABLE ROW LEVEL SECURITY;
ALTER TABLE auth."Roles"           ENABLE ROW LEVEL SECURITY;
ALTER TABLE auth."Permissions"     ENABLE ROW LEVEL SECURITY;
ALTER TABLE auth."UserRoles"       ENABLE ROW LEVEL SECURITY;
ALTER TABLE auth."RolePermissions" ENABLE ROW LEVEL SECURITY;
ALTER TABLE auth."RefreshTokens"   ENABLE ROW LEVEL SECURITY;

CREATE POLICY pol_Users_tenant_isolation ON auth."Users"
    USING (
        "TenantId" = current_setting('app.current_tenant_id', TRUE)::UUID
        OR auth.bypass_rls()
    );

CREATE POLICY pol_Roles_tenant_isolation ON auth."Roles"
    USING (
        "TenantId" = current_setting('app.current_tenant_id', TRUE)::UUID
        OR auth.bypass_rls()
    );

CREATE POLICY pol_Permissions_tenant_isolation ON auth."Permissions"
    USING (
        "TenantId" = current_setting('app.current_tenant_id', TRUE)::UUID
        OR auth.bypass_rls()
    );

CREATE POLICY pol_UserRoles_tenant_isolation ON auth."UserRoles"
    USING (
        "TenantId" = current_setting('app.current_tenant_id', TRUE)::UUID
        OR auth.bypass_rls()
    );

CREATE POLICY pol_RolePermissions_tenant_isolation ON auth."RolePermissions"
    USING (
        "TenantId" = current_setting('app.current_tenant_id', TRUE)::UUID
        OR auth.bypass_rls()
    );

CREATE POLICY pol_RefreshTokens_tenant_isolation ON auth."RefreshTokens"
    USING (
        "TenantId" = current_setting('app.current_tenant_id', TRUE)::UUID
        OR auth.bypass_rls()
    );

-- ── portal schema ─────────────────────────────────────────────
ALTER TABLE portal."Companies"        ENABLE ROW LEVEL SECURITY;
ALTER TABLE portal."JobPostings"      ENABLE ROW LEVEL SECURITY;
ALTER TABLE portal."JobApplications"  ENABLE ROW LEVEL SECURITY;

CREATE POLICY pol_Companies_tenant_isolation ON portal."Companies"
    USING (
        "TenantId" = current_setting('app.current_tenant_id', TRUE)::UUID
        OR auth.bypass_rls()
    );

CREATE POLICY pol_JobPostings_tenant_isolation ON portal."JobPostings"
    USING (
        "TenantId" = current_setting('app.current_tenant_id', TRUE)::UUID
        OR auth.bypass_rls()
        OR "Status" = 'Published'   -- published jobs are visible cross-tenant
    );

CREATE POLICY pol_JobApplications_tenant_isolation ON portal."JobApplications"
    USING (
        "TenantId" = current_setting('app.current_tenant_id', TRUE)::UUID
        OR auth.bypass_rls()
    );

-- ── billing schema ────────────────────────────────────────────
ALTER TABLE billing."TenantSubscriptions" ENABLE ROW LEVEL SECURITY;

CREATE POLICY pol_TenantSubscriptions_tenant_isolation ON billing."TenantSubscriptions"
    USING (
        "TenantId" = current_setting('app.current_tenant_id', TRUE)::UUID
        OR auth.bypass_rls()
    );
