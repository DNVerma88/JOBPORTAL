-- ============================================================
-- portal.OfferLetters — Offer letters sent to candidates
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."OfferLetters" (
    "Id"              UUID          NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"        UUID          NOT NULL REFERENCES auth."Tenants"("Id"),
    "ApplicationId"   UUID          NOT NULL REFERENCES portal."JobApplications"("Id"),
    "OfferedBy"       UUID          NOT NULL REFERENCES auth."Users"("Id"),
    "OfferDate"       DATE          NOT NULL,
    "JoiningDate"     DATE,
    "OfferSalary"     NUMERIC(15,2) NOT NULL,
    "CurrencyCode"    CHAR(3)       NOT NULL,
    "PositionTitle"   VARCHAR(200)  NOT NULL,
    "Department"      VARCHAR(200),
    "Location"        TEXT,
    "OfferFileUrl"    VARCHAR(500),
    "ExpiresAt"       TIMESTAMPTZ,
    "Status"          VARCHAR(50)   NOT NULL DEFAULT 'Sent',  -- 'Sent', 'Accepted', 'Declined', 'Expired', 'Revoked'
    "CandidateResponse" TEXT,
    "RespondedAt"     TIMESTAMPTZ,
    "IsRevoked"       BOOLEAN       NOT NULL DEFAULT FALSE,
    "RevokedAt"       TIMESTAMPTZ,
    "RevokedReason"   TEXT,
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_OfferLetters_ApplicationId
    ON portal."OfferLetters" ("ApplicationId");

CREATE INDEX IF NOT EXISTS idx_OfferLetters_TenantId_Status
    ON portal."OfferLetters" ("TenantId", "Status");

CALL apply_ModifiedOn_trigger('portal', 'OfferLetters');
