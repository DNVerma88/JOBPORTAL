-- ============================================================
-- JobPortal Database - Schema Definitions
-- ============================================================

CREATE SCHEMA IF NOT EXISTS auth;       -- Authentication & Authorization
CREATE SCHEMA IF NOT EXISTS master;     -- Shared lookup / reference data
CREATE SCHEMA IF NOT EXISTS portal;     -- Core job portal business data
CREATE SCHEMA IF NOT EXISTS billing;    -- Subscriptions, invoices, payments
CREATE SCHEMA IF NOT EXISTS config;     -- Platform & tenant configuration
