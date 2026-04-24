-- ============================================================
-- Portal schema — Additional composite/perf indexes
-- ============================================================

-- Job postings: tenant + status + publish date (most common listing)
CREATE INDEX IF NOT EXISTS idx_JobPostings_TenantId_Status_PublishedAt_Desc
    ON portal."JobPostings" ("TenantId", "Status", "PublishedAt" DESC)
    WHERE "IsDeleted" = FALSE;

-- Job postings: country + city + status for location filtering
CREATE INDEX IF NOT EXISTS idx_JobPostings_CountryId_CityId_Status
    ON portal."JobPostings" ("CountryId", "CityId", "Status")
    WHERE "IsDeleted" = FALSE;

-- Job postings: IsRemote filter
CREATE INDEX IF NOT EXISTS idx_JobPostings_IsRemote_Status
    ON portal."JobPostings" ("IsRemote", "Status")
    WHERE "IsRemote" = TRUE AND "IsDeleted" = FALSE;

-- Applications: job + status (recruiter dashboard)
CREATE INDEX IF NOT EXISTS idx_JobApplications_JobPostingId_Status_CreatedOn
    ON portal."JobApplications" ("JobPostingId", "Status", "CreatedOn" DESC)
    WHERE "IsDeleted" = FALSE;

-- Applications: applicant view (job seeker dashboard)
CREATE INDEX IF NOT EXISTS idx_JobApplications_ApplicantId_Status
    ON portal."JobApplications" ("ApplicantId", "Status", "CreatedOn" DESC)
    WHERE "IsDeleted" = FALSE;

-- Notifications: unread count by user
CREATE INDEX IF NOT EXISTS idx_Notifications_UserId_IsRead
    ON portal."Notifications" ("UserId", "IsRead", "CreatedOn" DESC)
    WHERE "IsDeleted" = FALSE;

-- Messages: thread conversation fetch
CREATE INDEX IF NOT EXISTS idx_Messages_ThreadId_CreatedOn_Asc
    ON portal."Messages" ("ThreadId", "CreatedOn" ASC)
    WHERE "IsDeleted" = FALSE;

-- Open-to-work candidate search (recruiter search candidates)
CREATE INDEX IF NOT EXISTS idx_JobSeekerProfiles_OpenToWork_Country_City
    ON portal."JobSeekerProfiles" ("TenantId", "CountryId", "CityId")
    WHERE "IsOpenToWork" = TRUE AND "ProfileVisibility" != 'Private' AND "IsDeleted" = FALSE;

-- Skill assessments: passed results
CREATE INDEX IF NOT EXISTS idx_SkillAssessmentResults_UserId_IsPassed
    ON portal."SkillAssessmentResults" ("UserId", "IsPassed", "CompletedAt" DESC);

-- GIN for full-text search (if not already from table file)
CREATE INDEX IF NOT EXISTS idx_JobPostings_SearchVector_GIN
    ON portal."JobPostings" USING GIN ("SearchVector");
