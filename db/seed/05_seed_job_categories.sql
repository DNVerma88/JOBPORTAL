-- ============================================================
-- Seed: master.JobCategories & master.JobSubCategories
-- NOTE: These inserts are included in 04_seed_industries.sql
--       since categories reference industry IDs defined there.
--       This file exists as a placeholder to maintain the
--       numbered sequence and can be extended with additional
--       cross-industry categories.
-- ============================================================

-- ── Additional cross-industry categories ──────────────────────
-- (Base set defined at end of 04_seed_industries.sql)

-- No-op — data seeded in 04_seed_industries.sql after INSERT INTO master."Industries"
SELECT 'Job categories and sub-categories seeded in 04_seed_industries.sql' AS note;
