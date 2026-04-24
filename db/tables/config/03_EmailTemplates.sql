-- ============================================================
-- config.EmailTemplates — Overridable email templates per tenant
-- ============================================================

CREATE TABLE IF NOT EXISTS config."EmailTemplates" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"    UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "TemplateKey" VARCHAR(100) NOT NULL,   -- 'email.verification', 'password.reset'
    "Subject"     VARCHAR(300) NOT NULL,
    "BodyHtml"    TEXT         NOT NULL,
    "BodyText"    TEXT,
    "Variables"   JSONB,                   -- {"name": "string", "otp": "string"}
    "IsGlobal"    BOOLEAN      NOT NULL DEFAULT FALSE,
    "IsActive"    BOOLEAN      NOT NULL DEFAULT TRUE,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_EmailTemplates_TenantId_TemplateKey
    ON config."EmailTemplates" ("TenantId", "TemplateKey") WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_EmailTemplates_TenantId
    ON config."EmailTemplates" ("TenantId");

CALL apply_ModifiedOn_trigger('config', 'EmailTemplates');
