-- ============================================================
-- billing.TenantSubscriptions — Active subscription per tenant
-- ============================================================

CREATE TABLE IF NOT EXISTS billing."TenantSubscriptions" (
    "Id"                     UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"               UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "PlanId"                 UUID          NOT NULL REFERENCES master."SubscriptionPlans"("Id"),
    "Tier"                   VARCHAR(50)               NOT NULL DEFAULT 'Free',
    "Status"                 VARCHAR(50)   NOT NULL DEFAULT 'Active',  -- 'Active', 'Cancelled', 'Expired', 'PastDue', 'Trial'
    "StartDate"              DATE          NOT NULL,
    "EndDate"                DATE,
    "TrialEndsAt"            DATE,
    "IsAutoRenew"            BOOLEAN       NOT NULL DEFAULT TRUE,
    "BillingCycle"           VARCHAR(20)   NOT NULL DEFAULT 'Monthly',  -- 'Monthly', 'Annual'
    "Amount"                 NUMERIC(10,2) NOT NULL,
    "CurrencyCode"           CHAR(3)       NOT NULL DEFAULT 'USD',
    "CancelledAt"            TIMESTAMPTZ,
    "CancelledReason"        TEXT,
    "ExternalSubscriptionId" VARCHAR(200),  -- Stripe/Razorpay subscription ID
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

-- Only one active subscription per tenant
CREATE UNIQUE INDEX IF NOT EXISTS idx_TenantSubscriptions_TenantId_Active
    ON billing."TenantSubscriptions" ("TenantId")
    WHERE "Status" = 'Active' AND "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS idx_TenantSubscriptions_TenantId_Status
    ON billing."TenantSubscriptions" ("TenantId", "Status");

CALL apply_ModifiedOn_trigger('billing', 'TenantSubscriptions');
