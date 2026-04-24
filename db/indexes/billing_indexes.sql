-- ============================================================
-- Billing schema — Additional indexes
-- ============================================================

CREATE INDEX IF NOT EXISTS idx_TenantSubscriptions_TenantId_Status
    ON billing."TenantSubscriptions" ("TenantId", "Status");

CREATE INDEX IF NOT EXISTS idx_TenantSubscriptions_EndDate
    ON billing."TenantSubscriptions" ("EndDate")
    WHERE "Status" = 'Active';

CREATE INDEX IF NOT EXISTS idx_PaymentTransactions_TenantId_Gateway_Status
    ON billing."PaymentTransactions" ("TenantId", "Gateway", "Status");

CREATE INDEX IF NOT EXISTS idx_SubscriptionInvoices_TenantId_DueDate
    ON billing."SubscriptionInvoices" ("TenantId", "DueDate" DESC);

CREATE INDEX IF NOT EXISTS idx_JobCredits_TenantId_Expires
    ON billing."JobCredits" ("TenantId", "ExpiresAt")
    WHERE "IsDeleted" = FALSE;
