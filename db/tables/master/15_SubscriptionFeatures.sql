-- ============================================================
-- master.SubscriptionFeatures — Feature toggles per plan
-- ============================================================

CREATE TABLE IF NOT EXISTS master."SubscriptionFeatures" (
    "Id"           UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "PlanId"       UUID         NOT NULL REFERENCES master."SubscriptionPlans"("Id") ON DELETE CASCADE,
    "FeatureKey"   VARCHAR(100) NOT NULL,
    "FeatureValue" VARCHAR(200),
    "IsEnabled"    BOOLEAN      NOT NULL DEFAULT TRUE
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_SubscriptionFeatures_PlanId_FeatureKey
    ON master."SubscriptionFeatures" ("PlanId", "FeatureKey");

CREATE INDEX IF NOT EXISTS idx_SubscriptionFeatures_PlanId
    ON master."SubscriptionFeatures" ("PlanId");
