-- ============================================================
-- portal.Certifications — Professional certifications
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."Certifications" (
    "Id"                  UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"            UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "ProfileId"           UUID         NOT NULL REFERENCES portal."JobSeekerProfiles"("Id") ON DELETE CASCADE,
    "Name"                VARCHAR(200) NOT NULL,
    "IssuingOrganization" VARCHAR(200) NOT NULL,
    "IssueDate"           DATE,
    "ExpiryDate"          DATE,
    "CredentialId"        VARCHAR(200),
    "CredentialUrl"       VARCHAR(500),
    "DoesNotExpire"       BOOLEAN      NOT NULL DEFAULT FALSE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_Certifications_ProfileId
    ON portal."Certifications" ("ProfileId");

CALL apply_ModifiedOn_trigger('portal', 'Certifications');
