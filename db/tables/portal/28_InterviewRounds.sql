-- ============================================================
-- portal.InterviewRounds — Multiple rounds per interview schedule
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."InterviewRounds" (
    "Id"           UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"     UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "ScheduleId"   UUID         NOT NULL REFERENCES portal."InterviewSchedules"("Id") ON DELETE CASCADE,
    "RoundNumber"  SMALLINT     NOT NULL,
    "Title"        VARCHAR(200) NOT NULL,
    "InterviewerId" UUID        REFERENCES auth."Users"("Id"),
    "StartedAt"    TIMESTAMPTZ,
    "EndedAt"      TIMESTAMPTZ,
    "Status"       VARCHAR(50)  NOT NULL DEFAULT 'Scheduled',  -- 'Scheduled', 'Completed', 'Cancelled', 'NoShow'
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_InterviewRounds_ScheduleId
    ON portal."InterviewRounds" ("ScheduleId");

CREATE INDEX IF NOT EXISTS idx_InterviewRounds_InterviewerId
    ON portal."InterviewRounds" ("InterviewerId");

CALL apply_ModifiedOn_trigger('portal', 'InterviewRounds');
