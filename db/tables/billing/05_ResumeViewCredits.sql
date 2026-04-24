-- ============================================================
-- billing.ResumeViewCredits — Resume view credit balance per tenant
-- ============================================================

CREATE TABLE IF NOT EXISTS billing."ResumeViewCredits" (
    "Id"               UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"         UUID         NOT NULL REFERENCES auth."Tenants"("Id"),
    "TotalCredits"     INT          NOT NULL DEFAULT 0,
    "UsedCredits"      INT          NOT NULL DEFAULT 0,
    "AvailableCredits" INT          GENERATED ALWAYS AS ("TotalCredits" - "UsedCredits") STORED,
    "ExpiresAt"        TIMESTAMPTZ,
    "Source"           VARCHAR(100),
    "InvoiceId"        UUID         REFERENCES billing."SubscriptionInvoices"("Id"),
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_ResumeViewCredits_TenantId
    ON billing."ResumeViewCredits" ("TenantId");

CALL apply_ModifiedOn_trigger('billing', 'ResumeViewCredits');
