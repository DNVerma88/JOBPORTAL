-- ============================================================
-- Master schema — Additional search-optimized indexes
-- ============================================================

-- GIN trigram on Skills.Name for fuzzy autocomplete
CREATE INDEX IF NOT EXISTS idx_Skills_Name_Trgm
    ON master."Skills" USING GIN ("Name" gin_trgm_ops);

-- GIN trigram on Cities.Name for fast city search
CREATE INDEX IF NOT EXISTS idx_Cities_Name_Trgm_Master
    ON master."Cities" USING GIN ("Name" gin_trgm_ops);

-- States by country
CREATE INDEX IF NOT EXISTS idx_States_CountryId_IsActive
    ON master."States" ("CountryId", "IsActive");

-- Cities by state and country
CREATE INDEX IF NOT EXISTS idx_Cities_StateId_CountryId
    ON master."Cities" ("StateId", "CountryId");

-- Job sub-categories by category
CREATE INDEX IF NOT EXISTS idx_JobSubCategories_CategoryId_IsActive
    ON master."JobSubCategories" ("CategoryId", "IsActive");

-- Notification templates by channel
CREATE INDEX IF NOT EXISTS idx_NotificationTemplates_Channel_IsActive
    ON master."NotificationTemplates" ("Channel", "IsActive");
