-- ============================================================
-- portal.Messages — Individual messages in a thread
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."Messages" (
    "Id"             UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"       UUID        NOT NULL REFERENCES auth."Tenants"("Id"),
    "ThreadId"       UUID        NOT NULL REFERENCES portal."MessageThreads"("Id") ON DELETE CASCADE,
    "SenderUserId"   UUID        NOT NULL REFERENCES auth."Users"("Id"),
    "Body"           TEXT        NOT NULL,
    "IsRead"         BOOLEAN     NOT NULL DEFAULT FALSE,
    "ReadAt"         TIMESTAMPTZ,
    "AttachmentUrl"  VARCHAR(500),
    "AttachmentName" VARCHAR(300),
    -- Audit columns
    "CreatedBy"  UUID        NOT NULL,
    "CreatedOn"  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ModifiedBy" UUID,
    "ModifiedOn" TIMESTAMPTZ,
    "IsDeleted"  BOOLEAN     NOT NULL DEFAULT FALSE,
    "DeletedBy"  UUID,
    "DeletedOn"  TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_Messages_ThreadId_CreatedOn
    ON portal."Messages" ("ThreadId", "CreatedOn" ASC);

CREATE INDEX IF NOT EXISTS idx_Messages_SenderUserId
    ON portal."Messages" ("SenderUserId");

CALL apply_ModifiedOn_trigger('portal', 'Messages');
