-- ============================================================
-- portal.InterviewSchedules — Interview booking records
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."InterviewSchedules" (
    "Id"              UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"        UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "ApplicationId"   UUID          NOT NULL REFERENCES portal."JobApplications"("Id"),
    "Title"           VARCHAR(200)  NOT NULL,
    "Description"     TEXT,
    "ScheduledAt"     TIMESTAMPTZ   NOT NULL,
    "DurationMinutes" SMALLINT      NOT NULL DEFAULT 60,
    "InterviewType"   portal."InterviewType" NOT NULL,
    "MeetingLink"     VARCHAR(500),
    "Location"        TEXT,
    "IsConfirmed"     BOOLEAN       NOT NULL DEFAULT FALSE,
    "IsCancelled"     BOOLEAN       NOT NULL DEFAULT FALSE,
    "CancelledAt"     TIMESTAMPTZ,
    "CancelledReason" TEXT,
    "ReminderSentAt"  TIMESTAMPTZ,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_InterviewSchedules_ApplicationId
    ON portal."InterviewSchedules" ("ApplicationId");

CREATE INDEX IF NOT EXISTS idx_InterviewSchedules_ScheduledAt
    ON portal."InterviewSchedules" ("TenantId", "ScheduledAt");

CALL apply_ModifiedOn_trigger('portal', 'InterviewSchedules');
