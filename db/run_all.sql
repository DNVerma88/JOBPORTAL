-- ============================================================
-- JobPortal — Master DB Orchestration Script
-- Run with: psql -U <user> -d <database> -f run_all.sql
-- ============================================================

\echo '============================================================'
\echo ' JobPortal Database Setup'
\echo '============================================================'

BEGIN;

-- ── 1. Extensions ─────────────────────────────────────────────
\echo '[1/8] Installing extensions...'
\i schemas/00_extensions.sql

-- ── 2. Schemas ────────────────────────────────────────────────
\echo '[2/8] Creating schemas...'
\i schemas/01_schemas.sql

-- ── 3. Enum Types ─────────────────────────────────────────────
\echo '[3/8] Creating enum types...'
\i schemas/02_types.sql

-- ── 4. Functions & Triggers ───────────────────────────────────
\echo '[4/8] Creating functions & triggers...'
\i schemas/03_functions.sql

-- ── 5. Tables ─────────────────────────────────────────────────
\echo '[5/8] Creating tables...'

-- auth schema
\echo '  [auth] Tenants...'
\i tables/auth/01_Tenants.sql
\echo '  [auth] Users...'
\i tables/auth/02_Users.sql
\echo '  [auth] Roles...'
\i tables/auth/03_Roles.sql
\echo '  [auth] Permissions...'
\i tables/auth/04_Permissions.sql
\echo '  [auth] RolePermissions...'
\i tables/auth/05_RolePermissions.sql
\echo '  [auth] UserRoles...'
\i tables/auth/06_UserRoles.sql
\echo '  [auth] RefreshTokens...'
\i tables/auth/07_RefreshTokens.sql
\echo '  [auth] UserSessions...'
\i tables/auth/08_UserSessions.sql
\echo '  [auth] TwoFactorSettings...'
\i tables/auth/09_TwoFactorSettings.sql
\echo '  [auth] AuditLogs...'
\i tables/auth/10_AuditLogs.sql
\echo '  [auth] PasswordResetTokens...'
\i tables/auth/11_PasswordResetTokens.sql
\echo '  [auth] EmailVerifications...'
\i tables/auth/12_EmailVerifications.sql

-- master schema
\echo '  [master] Countries...'
\i tables/master/01_Countries.sql
\echo '  [master] States...'
\i tables/master/02_States.sql
\echo '  [master] Cities...'
\i tables/master/03_Cities.sql
\echo '  [master] Industries...'
\i tables/master/04_Industries.sql
\echo '  [master] JobCategories...'
\i tables/master/05_JobCategories.sql
\echo '  [master] JobSubCategories...'
\i tables/master/06_JobSubCategories.sql
\echo '  [master] Skills...'
\i tables/master/07_Skills.sql
\echo '  [master] JobTypes...'
\i tables/master/08_JobTypes.sql
\echo '  [master] ExperienceLevels...'
\i tables/master/09_ExperienceLevels.sql
\echo '  [master] EducationLevels...'
\i tables/master/10_EducationLevels.sql
\echo '  [master] LanguageMaster...'
\i tables/master/11_LanguageMaster.sql
\echo '  [master] CurrencyMaster...'
\i tables/master/12_CurrencyMaster.sql
\echo '  [master] NotificationTemplates...'
\i tables/master/13_NotificationTemplates.sql
\echo '  [master] SubscriptionPlans...'
\i tables/master/14_SubscriptionPlans.sql
\echo '  [master] SubscriptionFeatures...'
\i tables/master/15_SubscriptionFeatures.sql

-- portal schema
\echo '  [portal] Companies...'
\i tables/portal/01_Companies.sql
\i tables/portal/02_CompanyFollowers.sql
\i tables/portal/03_CompanyCultures.sql
\i tables/portal/04_CompanyReviews.sql
\i tables/portal/05_CompanyBranches.sql
\echo '  [portal] JobPostings...'
\i tables/portal/06_JobPostings.sql
\i tables/portal/07_JobPostingSkills.sql
\i tables/portal/08_JobPostingQuestions.sql
\i tables/portal/09_JobPostingBenefits.sql
\i tables/portal/10_SavedJobs.sql
\echo '  [portal] JobSeekerProfiles...'
\i tables/portal/11_JobSeekerProfiles.sql
\i tables/portal/12_WorkExperiences.sql
\i tables/portal/13_Educations.sql
\i tables/portal/14_Certifications.sql
\i tables/portal/15_Projects.sql
\i tables/portal/16_Languages.sql
\i tables/portal/17_Awards.sql
\i tables/portal/18_SocialLinks.sql
\i tables/portal/19_Resumes.sql
\i tables/portal/20_ResumeFiles.sql
\echo '  [portal] Applications...'
\i tables/portal/21_JobApplications.sql
\i tables/portal/22_ApplicationAnswers.sql
\i tables/portal/23_ApplicationStatusHistory.sql
\i tables/portal/24_ApplicationNotes.sql
\i tables/portal/25_CandidatePipelineStages.sql
\i tables/portal/26_CandidatePipelines.sql
\echo '  [portal] Interviews...'
\i tables/portal/27_InterviewSchedules.sql
\i tables/portal/28_InterviewRounds.sql
\i tables/portal/29_InterviewFeedbacks.sql
\i tables/portal/30_OfferLetters.sql
\echo '  [portal] Messaging & Notifications...'
\i tables/portal/31_MessageThreads.sql
\i tables/portal/32_Messages.sql
\i tables/portal/33_JobAlerts.sql
\i tables/portal/34_Notifications.sql
\i tables/portal/35_NotificationPreferences.sql
\echo '  [portal] Saved Searches & Candidate Lists...'
\i tables/portal/36_SavedSearches.sql
\i tables/portal/37_CandidateSavedLists.sql
\i tables/portal/38_CandidateSavedListMembers.sql
\echo '  [portal] Skill Assessments...'
\i tables/portal/39_SkillAssessments.sql
\i tables/portal/40_SkillAssessmentQuestions.sql
\i tables/portal/41_SkillAssessmentResults.sql
\echo '  [portal] Activity Logs...'
\i tables/portal/42_UserActivityLogs.sql
\i tables/portal/43_JobViewLogs.sql
\i tables/portal/44_ApplicationSourceLogs.sql

-- billing schema
\echo '  [billing] Subscriptions & Payments...'
\i tables/billing/01_TenantSubscriptions.sql
\i tables/billing/02_SubscriptionInvoices.sql
\i tables/billing/03_PaymentTransactions.sql
\i tables/billing/04_JobCredits.sql
\i tables/billing/05_ResumeViewCredits.sql

-- config schema
\echo '  [config] Settings & Configuration...'
\i tables/config/01_TenantSettings.sql
\i tables/config/02_GlobalSettings.sql
\i tables/config/03_EmailTemplates.sql
\i tables/config/04_FeatureFlags.sql
\i tables/config/05_Announcements.sql
\i tables/config/06_ContentPages.sql

-- ── 6. Indexes ────────────────────────────────────────────────
\echo '[6/8] Creating indexes...'
\i indexes/auth_indexes.sql
\i indexes/master_indexes.sql
\i indexes/portal_indexes.sql
\i indexes/billing_indexes.sql
\i indexes/config_indexes.sql

-- ── 7. Row Level Security ─────────────────────────────────────
\echo '[7/8] Applying Row Level Security...'
\i rls/row_level_security.sql

-- ── 8. Seed Data ──────────────────────────────────────────────
\echo '[8/8] Loading seed data...'
\i seed/01_seed_countries.sql
\i seed/02_seed_states.sql
\i seed/03_seed_cities.sql
\i seed/04_seed_industries.sql
\i seed/05_seed_job_categories.sql
\i seed/06_seed_skills.sql
\i seed/07_seed_roles_permissions.sql
\i seed/08_seed_subscription_plans.sql
\i seed/09_seed_global_settings.sql
\i seed/10_seed_admin_user.sql

COMMIT;

\echo ''
\echo '============================================================'
\echo ' JobPortal database setup complete!'
\echo '============================================================'
