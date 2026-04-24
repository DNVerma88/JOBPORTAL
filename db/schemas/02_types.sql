-- ============================================================
-- JobPortal Database - Custom ENUM Types
-- ============================================================

-- ── Portal Enums ─────────────────────────────────────────────
DO $$ BEGIN
    CREATE TYPE portal."ApplicationStatus" AS ENUM (
        'Pending', 'Screening', 'ShortListed', 'InterviewScheduled',
        'Interviewed', 'OfferSent', 'Hired', 'Rejected', 'Withdrawn', 'OnHold'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE portal."JobPostingStatus" AS ENUM (
        'Draft', 'Published', 'Paused', 'Closed', 'Expired', 'Archived'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE portal."JobType" AS ENUM (
        'FullTime', 'PartTime', 'Contract', 'Freelance', 'Internship', 'Temporary'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE portal."WorkMode" AS ENUM (
        'OnSite', 'Remote', 'Hybrid'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE portal."ExperienceLevel" AS ENUM (
        'EntryLevel', 'MidLevel', 'Senior', 'Lead', 'Manager', 'Executive'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE portal."Gender" AS ENUM (
        'Male', 'Female', 'NonBinary', 'PreferNotToSay'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE portal."ProfileVisibility" AS ENUM (
        'Public', 'Private', 'RecruiterOnly'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE portal."InterviewType" AS ENUM (
        'Phone', 'Video', 'InPerson', 'Technical', 'HR'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE portal."NotificationType" AS ENUM (
        'JobAlert', 'ApplicationStatus', 'Message', 'InterviewInvite',
        'OfferLetter', 'SystemAlert', 'Announcement'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE portal."NotificationChannel" AS ENUM (
        'InApp', 'Email', 'SMS', 'Push'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE portal."NotificationDeliveryScope" AS ENUM (
        'Public', 'Tenant', 'Private', 'Role'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

-- ── Billing Enums ─────────────────────────────────────────────
DO $$ BEGIN
    CREATE TYPE billing."SubscriptionTier" AS ENUM (
        'Free', 'Basic', 'Professional', 'Enterprise'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE billing."PaymentStatus" AS ENUM (
        'Pending', 'Completed', 'Failed', 'Refunded', 'Cancelled'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;
