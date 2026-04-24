-- ============================================================
-- billing.PaymentTransactions — Payment gateway transactions
-- ============================================================

CREATE TABLE IF NOT EXISTS billing."PaymentTransactions" (
    "Id"                   UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"             UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "InvoiceId"            UUID          REFERENCES billing."SubscriptionInvoices"("Id"),
    "Amount"               NUMERIC(10,2) NOT NULL,
    "CurrencyCode"         CHAR(3)       NOT NULL,
    "PaymentMethod"        VARCHAR(50),   -- 'Card', 'UPI', 'NetBanking', 'Wallet'
    "Gateway"              VARCHAR(50),   -- 'Stripe', 'Razorpay'
    "GatewayTransactionId" VARCHAR(200),
    "GatewayResponse"      JSONB,
    "Status"               billing."PaymentStatus" NOT NULL DEFAULT 'Pending',
    "FailureReason"        TEXT,
    "ProcessedAt"          TIMESTAMPTZ,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_PaymentTransactions_GatewayTransactionId
    ON billing."PaymentTransactions" ("GatewayTransactionId")
    WHERE "GatewayTransactionId" IS NOT NULL;

CREATE INDEX IF NOT EXISTS idx_PaymentTransactions_TenantId_Status
    ON billing."PaymentTransactions" ("TenantId", "Status");

CREATE INDEX IF NOT EXISTS idx_PaymentTransactions_InvoiceId
    ON billing."PaymentTransactions" ("InvoiceId");

CALL apply_ModifiedOn_trigger('billing', 'PaymentTransactions');
