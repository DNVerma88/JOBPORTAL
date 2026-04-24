-- ============================================================
-- JobPortal Database - PostgreSQL Extensions
-- Run this FIRST as superuser
-- ============================================================

CREATE EXTENSION IF NOT EXISTS "pgcrypto";      -- gen_random_uuid(), crypt()
CREATE EXTENSION IF NOT EXISTS "pg_trgm";       -- fuzzy text search
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";     -- uuid_generate_v4() fallback
CREATE EXTENSION IF NOT EXISTS "unaccent";      -- accent-insensitive search
