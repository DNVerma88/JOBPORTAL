-- ============================================================
-- Seed: Default SuperAdmin user
-- Tenant:  00000000-0000-0000-0000-000000000001 (System)
-- Email:   admin@jobportal.io
-- Password: Admin@12345!
-- ============================================================

DO $$
DECLARE
    v_system_tenant UUID   := '00000000-0000-0000-0000-000000000001'::UUID;
    v_admin_id      UUID   := '00000001-0000-0000-0000-000000000001'::UUID;
    v_superadmin_role UUID := '00000009-0000-0000-0000-000000000001'::UUID;
BEGIN
    INSERT INTO auth."Users" (
        "Id", "TenantId",
        "Email", "NormalizedEmail",
        "PasswordHash",
        "FirstName", "LastName",
        "IsEmailVerified", "IsActive", "IsSuperAdmin",
        "CreatedBy", "CreatedOn", "IsDeleted"
    ) VALUES (
        v_admin_id, v_system_tenant,
        'admin@jobportal.io', 'ADMIN@JOBPORTAL.IO',
        '$argon2id$v=19$m=65536,t=3,p=4$5FhHYmLdVNwdMSE5QmY4mA$x3/hgpHSADch0slQsVAAb3bPT1cRWoilhM05n8c3I/4',
        'System', 'Admin',
        TRUE, TRUE, TRUE,
        v_admin_id, NOW(), FALSE
    ) ON CONFLICT DO NOTHING;

    -- Assign SuperAdmin role
    INSERT INTO auth."UserRoles" (
        "Id", "TenantId", "UserId", "RoleId",
        "CreatedBy", "CreatedOn", "IsDeleted"
    ) VALUES (
        gen_random_uuid(), v_system_tenant, v_admin_id, v_superadmin_role,
        v_admin_id, NOW(), FALSE
    ) ON CONFLICT DO NOTHING;
END $$;
