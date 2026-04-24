-- ============================================================
-- Seed: auth.Roles & auth.Permissions for SYSTEM tenant
-- SYSTEM_TENANT_ID = '00000000-0000-0000-0000-000000000001'
-- ============================================================

DO $$
DECLARE
    v_system_tenant UUID := '00000000-0000-0000-0000-000000000001'::UUID;
BEGIN
    -- ── Ensure system tenant exists ────────────────────────────
    INSERT INTO auth."Tenants" (
        "Id", "Name", "Slug", "IsActive", "SubscriptionTier",
        "ContactEmail", "CreatedBy",
        "CreatedOn", "ModifiedOn", "IsDeleted"
    ) VALUES (
        v_system_tenant, 'System', 'system', TRUE, 'Enterprise',
        'system@jobportal.io', v_system_tenant,
        NOW(), NOW(), FALSE
    ) ON CONFLICT DO NOTHING;

    -- ── Permissions ────────────────────────────────────────────
    INSERT INTO auth."Permissions" (
        "Id", "TenantId", "Key", "Name", "Resource", "Action",
        "Description", "CreatedBy", "CreatedOn", "IsDeleted"
    ) VALUES
        ('00000008-0000-0000-0000-000000000001', v_system_tenant, 'jobs.create',         'Create Jobs',           'Jobs',        'Create',   'Create job postings',              v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000002', v_system_tenant, 'jobs.read',           'View Jobs',             'Jobs',        'Read',     'View job postings',                v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000003', v_system_tenant, 'jobs.update',         'Edit Jobs',             'Jobs',        'Update',   'Edit job postings',                v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000004', v_system_tenant, 'jobs.delete',         'Delete Jobs',           'Jobs',        'Delete',   'Delete job postings',              v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000005', v_system_tenant, 'jobs.publish',        'Publish Jobs',          'Jobs',        'Publish',  'Publish/unpublish job postings',   v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000006', v_system_tenant, 'candidates.view',     'View Candidates',       'Candidates',  'Read',     'View candidate profiles',          v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000007', v_system_tenant, 'candidates.export',   'Export Candidates',     'Candidates',  'Export',   'Export candidate data',            v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000008', v_system_tenant, 'applications.view',   'View Applications',     'Applications','Read',     'View applications',                v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000009', v_system_tenant, 'applications.manage', 'Manage Applications',   'Applications','Manage',   'Manage application pipeline',      v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000010', v_system_tenant, 'reports.view',        'View Reports',          'Reports',     'Read',     'View analytics & reports',         v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000011', v_system_tenant, 'settings.manage',     'Manage Settings',       'Settings',    'Manage',   'Manage tenant settings',           v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000012', v_system_tenant, 'users.manage',        'Manage Users',          'Users',       'Manage',   'Manage team members',              v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000013', v_system_tenant, 'billing.view',        'View Billing',          'Billing',     'Read',     'View billing information',         v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000014', v_system_tenant, 'billing.manage',      'Manage Billing',        'Billing',     'Manage',   'Manage subscriptions & payments',  v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000015', v_system_tenant, 'companies.manage',    'Manage Companies',      'Companies',   'Manage',   'Manage company profiles',          v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000016', v_system_tenant, 'interviews.schedule', 'Schedule Interviews',   'Interviews',  'Schedule', 'Schedule interviews',              v_system_tenant, NOW(), FALSE),
        ('00000008-0000-0000-0000-000000000017', v_system_tenant, 'offers.manage',       'Manage Offers',         'Offers',      'Manage',   'Send and manage offer letters',    v_system_tenant, NOW(), FALSE)
    ON CONFLICT DO NOTHING;

    -- ── Roles ──────────────────────────────────────────────────
    INSERT INTO auth."Roles" (
        "Id", "TenantId", "Name", "NormalizedName", "Description",
        "IsSystemRole", "CreatedBy", "CreatedOn", "IsDeleted"
    ) VALUES
        ('00000009-0000-0000-0000-000000000001', v_system_tenant, 'SuperAdmin',    'SUPERADMIN',    'Full system access',                    TRUE, v_system_tenant, NOW(), FALSE),
        ('00000009-0000-0000-0000-000000000002', v_system_tenant, 'TenantAdmin',   'TENANTADMIN',   'Tenant-level admin (all permissions)',   TRUE, v_system_tenant, NOW(), FALSE),
        ('00000009-0000-0000-0000-000000000003', v_system_tenant, 'Recruiter',     'RECRUITER',     'Post jobs and manage applications',     TRUE, v_system_tenant, NOW(), FALSE),
        ('00000009-0000-0000-0000-000000000004', v_system_tenant, 'JobSeeker',     'JOBSEEKER',     'Apply for jobs and manage profile',     TRUE, v_system_tenant, NOW(), FALSE),
        ('00000009-0000-0000-0000-000000000005', v_system_tenant, 'HiringManager', 'HIRINGMANAGER', 'Review applications and give feedback', TRUE, v_system_tenant, NOW(), FALSE)
    ON CONFLICT DO NOTHING;

    -- ── Role → Permission mapping ──────────────────────────────
    -- TenantAdmin gets all permissions
    INSERT INTO auth."RolePermissions" ("TenantId", "RoleId", "PermissionId", "CreatedBy", "CreatedOn")
    SELECT v_system_tenant, '00000009-0000-0000-0000-000000000002', "Id", v_system_tenant, NOW()
    FROM   auth."Permissions"
    WHERE  "TenantId" = v_system_tenant
    ON CONFLICT DO NOTHING;

    -- Recruiter: jobs, candidates, applications, interviews, offers
    INSERT INTO auth."RolePermissions" ("TenantId", "RoleId", "PermissionId", "CreatedBy", "CreatedOn")
    SELECT v_system_tenant, '00000009-0000-0000-0000-000000000003', "Id", v_system_tenant, NOW()
    FROM   auth."Permissions"
    WHERE  "TenantId" = v_system_tenant
      AND  "Key" IN (
               'jobs.create','jobs.read','jobs.update','jobs.delete','jobs.publish',
               'candidates.view','applications.view','applications.manage',
               'interviews.schedule','offers.manage','companies.manage','reports.view'
           )
    ON CONFLICT DO NOTHING;

    -- HiringManager: view candidates, manage applications, interviews, feedback
    INSERT INTO auth."RolePermissions" ("TenantId", "RoleId", "PermissionId", "CreatedBy", "CreatedOn")
    SELECT v_system_tenant, '00000009-0000-0000-0000-000000000005', "Id", v_system_tenant, NOW()
    FROM   auth."Permissions"
    WHERE  "TenantId" = v_system_tenant
      AND  "Key" IN (
               'jobs.read','candidates.view','applications.view','applications.manage',
               'interviews.schedule','reports.view'
           )
    ON CONFLICT DO NOTHING;

    -- JobSeeker: only read jobs
    INSERT INTO auth."RolePermissions" ("TenantId", "RoleId", "PermissionId", "CreatedBy", "CreatedOn")
    SELECT v_system_tenant, '00000009-0000-0000-0000-000000000004', "Id", v_system_tenant, NOW()
    FROM   auth."Permissions"
    WHERE  "TenantId" = v_system_tenant
      AND  "Key" = 'jobs.read'
    ON CONFLICT DO NOTHING;
END $$;
