-- ============================================================
-- portal.ResumeFiles — Physical file references per resume
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."ResumeFiles" (
    "Id"              UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"        UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "ResumeId"        UUID          NOT NULL REFERENCES portal."Resumes"("Id") ON DELETE CASCADE,
    "FileName"        VARCHAR(300)  NOT NULL,
    "StoredFileName"  VARCHAR(500)  NOT NULL,   -- UUID-based storage filename
    "FileSize"        BIGINT        NOT NULL,
    "ContentType"     VARCHAR(100)  NOT NULL,   -- 'application/pdf', etc.
    "StorageProvider" VARCHAR(50)   NOT NULL,   -- 'local', 'azure', 's3'
    "StoragePath"     VARCHAR(1000) NOT NULL,
    "UploadedAt"      TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_ResumeFiles_ResumeId
    ON portal."ResumeFiles" ("ResumeId");

CALL apply_ModifiedOn_trigger('portal', 'ResumeFiles');
