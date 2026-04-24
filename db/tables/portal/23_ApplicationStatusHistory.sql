-- ============================================================
-- portal.ApplicationStatusHistory — Immutable audit of status changes
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."ApplicationStatusHistory" (
    "Id"            UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"      UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "ApplicationId" UUID          NOT NULL REFERENCES portal."JobApplications"("Id") ON DELETE CASCADE,
    "FromStatus"    portal."ApplicationStatus",
    "ToStatus"      portal."ApplicationStatus" NOT NULL,
    "Notes"         TEXT,
    "ChangedBy"     UUID          NOT NULL REFERENCES auth."Users"("Id"),
    "ChangedOn"     TIMESTAMPTZ   NOT NULL DEFAULT NOW()
    -- Immutable — no audit update columns
);

CREATE INDEX IF NOT EXISTS idx_ApplicationStatusHistory_ApplicationId
    ON portal."ApplicationStatusHistory" ("ApplicationId");

CREATE INDEX IF NOT EXISTS idx_ApplicationStatusHistory_ApplicationId_ChangedOn
    ON portal."ApplicationStatusHistory" ("ApplicationId", "ChangedOn" DESC);
