-- ============================================================
-- JobPortal Database - Shared Functions & Triggers
-- ============================================================

-- ── Trigger function: auto-set ModifiedOn on UPDATE ──────────
CREATE OR REPLACE FUNCTION update_ModifiedOn()
RETURNS TRIGGER AS $$
BEGIN
    NEW."ModifiedOn" = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- ── Helper: attach ModifiedOn trigger to a table ─────────────
CREATE OR REPLACE PROCEDURE apply_ModifiedOn_trigger(
    p_schema TEXT,
    p_table  TEXT
)
LANGUAGE plpgsql AS $$
DECLARE
    v_trigger_name TEXT;
    v_sql          TEXT;
BEGIN
    v_trigger_name := 'trg_' || p_table || '_ModifiedOn';

    v_sql := FORMAT(
        'CREATE OR REPLACE TRIGGER %I
         BEFORE UPDATE ON %I.%I
         FOR EACH ROW EXECUTE FUNCTION update_ModifiedOn()',
        v_trigger_name, p_schema, p_table
    );

    EXECUTE v_sql;
END;
$$;

-- ── JobPostings full-text search vector update trigger ────────
CREATE OR REPLACE FUNCTION portal.update_JobPostings_SearchVector()
RETURNS TRIGGER AS $$
BEGIN
    NEW."SearchVector" =
        setweight(to_tsvector('english', COALESCE(NEW."Title", '')), 'A') ||
        setweight(to_tsvector('english', COALESCE(NEW."Description", '')), 'B') ||
        setweight(to_tsvector('english', COALESCE(NEW."Requirements", '')), 'C');
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
