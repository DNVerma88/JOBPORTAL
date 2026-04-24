-- ============================================================
-- billing.SubscriptionInvoices — Invoice records
-- ============================================================

CREATE TABLE IF NOT EXISTS billing."SubscriptionInvoices" (
    "Id"                   UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"             UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "SubscriptionId"       UUID          NOT NULL REFERENCES billing."TenantSubscriptions"("Id"),
    "InvoiceNumber"        VARCHAR(50)   NOT NULL,
    "BillingPeriodStart"   DATE          NOT NULL,
    "BillingPeriodEnd"     DATE          NOT NULL,
    "Amount"               NUMERIC(10,2) NOT NULL,
    "TaxAmount"            NUMERIC(10,2) NOT NULL DEFAULT 0,
    "TotalAmount"          NUMERIC(10,2) NOT NULL,
    "CurrencyCode"         CHAR(3)       NOT NULL,
    "Status"               billing."PaymentStatus" NOT NULL DEFAULT 'Pending',
    "DueDate"              DATE          NOT NULL,
    "PaidAt"               TIMESTAMPTZ,
    "ExternalInvoiceId"    VARCHAR(200),
    "InvoiceFileUrl"       VARCHAR(500),
    "Notes"                TEXT,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_SubscriptionInvoices_InvoiceNumber
    ON billing."SubscriptionInvoices" ("InvoiceNumber");

CREATE INDEX IF NOT EXISTS idx_SubscriptionInvoices_TenantId_Status
    ON billing."SubscriptionInvoices" ("TenantId", "Status");

CREATE INDEX IF NOT EXISTS idx_SubscriptionInvoices_SubscriptionId
    ON billing."SubscriptionInvoices" ("SubscriptionId");

CREATE INDEX IF NOT EXISTS idx_SubscriptionInvoices_DueDate
    ON billing."SubscriptionInvoices" ("DueDate") WHERE "Status" = 'Pending';

CALL apply_ModifiedOn_trigger('billing', 'SubscriptionInvoices');
