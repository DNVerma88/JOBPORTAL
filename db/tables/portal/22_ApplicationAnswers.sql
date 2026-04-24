-- ============================================================
-- portal.ApplicationAnswers — Answers to custom job questions
-- ============================================================

CREATE TABLE IF NOT EXISTS portal."ApplicationAnswers" (
    "Id"            UUID  NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "TenantId"      UUID  NOT NULL REFERENCES auth."Tenants"("Id"),
    "ApplicationId" UUID  NOT NULL REFERENCES portal."JobApplications"("Id") ON DELETE CASCADE,
    "QuestionId"    UUID  NOT NULL REFERENCES portal."JobPostingQuestions"("Id"),
    "AnswerText"    TEXT,
    "AnswerData"    JSONB   -- for MCQ selections
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_ApplicationAnswers_ApplicationId_QuestionId
    ON portal."ApplicationAnswers" ("ApplicationId", "QuestionId");

CREATE INDEX IF NOT EXISTS idx_ApplicationAnswers_ApplicationId
    ON portal."ApplicationAnswers" ("ApplicationId");
