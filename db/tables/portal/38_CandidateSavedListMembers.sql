-- ============================================================
-- portal.CandidateSavedListMembers — Candidates in saved lists
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."CandidateSavedListMembers" (
    "Id"               UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"         UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "ListId"           UUID        NOT NULL REFERENCES portal."CandidateSavedLists"("Id") ON DELETE CASCADE,
    "CandidateUserId"  UUID        NOT NULL REFERENCES auth."Users"("Id") ON DELETE CASCADE,
    "Notes"            TEXT,
    "AddedAt"          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "AddedBy"          UUID        NOT NULL REFERENCES auth."Users"("Id")
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_CandidateSavedListMembers_ListId_CandidateUserId
    ON portal."CandidateSavedListMembers" ("ListId", "CandidateUserId");

CREATE INDEX IF NOT EXISTS idx_CandidateSavedListMembers_ListId
    ON portal."CandidateSavedListMembers" ("ListId");

CREATE INDEX IF NOT EXISTS idx_CandidateSavedListMembers_CandidateUserId
    ON portal."CandidateSavedListMembers" ("CandidateUserId");
