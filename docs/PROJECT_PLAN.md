# Job Portal - Comprehensive Project Plan
> SaaS Job Portal (Naukri / Hirist style) — React + Vite PWA · .NET 10 API · PostgreSQL

---

## Table of Contents
1. [Project Vision](#1-project-vision)
2. [User Roles & Actors](#2-user-roles--actors)
3. [All Modules Overview](#3-all-modules-overview)
4. [Detailed Module Breakdown](#4-detailed-module-breakdown)
5. [Technical Architecture](#5-technical-architecture)
6. [Database Design Conventions](#6-database-design-conventions)
7. [Database Schema — All Tables](#7-database-schema--all-tables)
8. [Backend Project Structure](#8-backend-project-structure)
9. [Frontend Project Structure](#9-frontend-project-structure)
10. [API Design Standards](#10-api-design-standards)
11. [Security Strategy (OWASP)](#11-security-strategy-owasp)
12. [Performance Strategy](#12-performance-strategy)
13. [Implementation Phases & Roadmap](#13-implementation-phases--roadmap)
14. [Technology Stack & Packages](#14-technology-stack--packages)

---

## 1. Project Vision

Build a **multi-tenant SaaS Job Portal** that connects:
- **Job Seekers** — create profiles, upload resumes, search/apply for jobs
- **Employers / Recruiters** — post jobs, search candidates, manage pipelines
- **Super Admin** — manage tenants, plans, billing, platform settings

The platform must support **millions of concurrent users** with high availability,  
be **PWA-enabled** (offline capability, installable), and follow **OWASP/SOLID/KISS** at all layers.

---

## 2. User Roles & Actors

| Role | Description |
|------|-------------|
| **Super Admin** | Platform owner. Manages tenants, subscriptions, global config |
| **Tenant Admin** | Company/agency admin. Manages their org users, branding, billing |
| **Recruiter / HR** | Posts jobs, manages applications, communicates with candidates |
| **Hiring Manager** | Reviews shortlisted candidates, provides feedback |
| **Job Seeker** | Registers, builds profile, searches & applies for jobs |
| **Guest / Public** | Browses public job listings without logging in |

---

## 3. All Modules Overview

### A. Public Portal (Guest-facing)
| # | Module |
|---|--------|
| 1 | Landing Page |
| 2 | Job Search & Browse |
| 3 | Job Detail Page |
| 4 | Company Listing & Profile |
| 5 | Login / Register (Job Seeker & Employer) |
| 6 | Forgot / Reset Password |

### B. Job Seeker Portal
| # | Module |
|---|--------|
| 7 | Dashboard |
| 8 | Profile Management |
| 9 | Resume Builder & Upload |
| 10 | Job Search & Filters |
| 11 | Job Application |
| 12 | Application Tracker |
| 13 | Saved Jobs / Wishlist |
| 14 | Job Alerts & Notifications |
| 15 | Skills Assessment |
| 16 | Interview Preparation Tips |
| 17 | Chat / Messaging with Recruiter |
| 18 | Privacy Settings |
| 19 | Account Settings |

### C. Employer / Recruiter Portal
| # | Module |
|---|--------|
| 20 | Employer Dashboard |
| 21 | Company Profile Management |
| 22 | Job Posting Management |
| 23 | Candidate Search (Resume Database) |
| 24 | Application Pipeline / ATS |
| 25 | Interview Scheduling |
| 26 | Team & User Management |
| 27 | Candidate Shortlisting & Tracking |
| 28 | Offer Letter Management |
| 29 | Job Sponsorship / Featured Jobs |
| 30 | Reports & Analytics |
| 31 | Recruiter Messaging / Chat |
| 32 | Subscription & Billing |

### D. Super Admin Panel
| # | Module |
|---|--------|
| 33 | Platform Dashboard |
| 34 | Tenant Management |
| 35 | Subscription Plan Management |
| 36 | Billing & Payment Management |
| 37 | Global Configuration & Settings |
| 38 | Content Management (CMS) |
| 39 | Job Category & Skills Management |
| 40 | Audit Logs & Activity Monitoring |
| 41 | Reports & Statistics |
| 42 | Email / Notification Templates |
| 43 | Announcement & Banners |

### E. Cross-Cutting Modules
| # | Module |
|---|--------|
| 44 | Authentication & Authorization (JWT + Refresh) |
| 45 | Multi-Tenancy Engine |
| 46 | Notification System (In-App, Email, Push) |
| 47 | File Management (Resume, Logos, Docs) |
| 48 | Search Engine (Elasticsearch / Full-text) |
| 49 | Audit Trail |
| 50 | GDPR / Privacy / Data Consent |
| 51 | SEO & Social Sharing |
| 52 | PWA (Offline + Installable) |

---

## 4. Detailed Module Breakdown

---

### MODULE 1: Landing Page
**Purpose:** Marketing & entry point for all users  
**Features:**
- Hero banner with job search bar (keyword + location)
- Featured job listings
- Top companies carousel
- Job categories grid
- How it works section
- Testimonials
- Newsletter subscription
- Footer with sitemap

---

### MODULE 2–5: Public Job Search & Company Browse
**Features:**
- Keyword search with autocomplete
- Filters: location, salary, experience, job type (full-time/part-time/contract/remote), industry, posted date
- Pagination (cursor-based for performance)
- Sort by: relevance, date, salary
- Job card: title, company, location, salary range, tags, posted date
- Company profile: logo, about, culture, reviews, open positions
- SEO-friendly URLs (`/jobs/react-developer-mumbai`)

---

### MODULE 7: Job Seeker Dashboard
**Features:**
- Profile completion percentage widget
- Recommended jobs (based on skills/preferences)
- Recent applications status
- Job alerts count
- Profile views count
- Quick actions: Update Resume, Search Jobs, View Applications

---

### MODULE 8: Profile Management
**Features:**
- Personal info (name, photo, phone, location, DOB)
- Professional headline & summary
- Work experience (multi-entry: company, title, start/end, description, current)
- Education (multi-entry: institution, degree, field, year)
- Skills (tags with proficiency level)
- Certifications & Awards
- Languages
- Social links (LinkedIn, GitHub, Portfolio)
- Resume visibility toggle (public/private/searchable)
- Profile URL (slug)

---

### MODULE 9: Resume Builder & Upload
**Features:**
- Upload PDF/DOC resume
- Resume parser (extract name, skills, experience)
- Built-in resume builder with templates
- Download as PDF
- Multiple resume versions
- Resume completeness score

---

### MODULE 10-11: Job Search & Application
**Features:**
- Advanced filter panel + full-text search
- Save search queries
- One-click apply
- Apply with custom cover letter
- Question-based application forms (custom questions per job)
- Captcha on application (anti-spam)
- Application status tracking (Applied → Reviewed → Shortlisted → Interview → Offered → Rejected)

---

### MODULE 12: Application Tracker
**Features:**
- Kanban board or list view of all applications
- Status badges with colors
- Notes per application
- Timeline of communication

---

### MODULE 14: Job Alerts & Notifications
**Features:**
- Create alert by keyword + location + filters
- Email frequency: instant / daily / weekly
- In-app notification bell
- Push notifications (PWA)
- Manage/unsubscribe alerts

---

### MODULE 20: Employer Dashboard
**Features:**
- Active jobs count
- Applications received today/week
- Candidate pipeline overview (kanban summary)
- Profile views of company
- Subscription status / job slots remaining
- Recent activity feed

---

### MODULE 22: Job Posting Management
**Features:**
- Create/Edit/Delete job posts
- Job details: title, description (rich text), location (remote/hybrid/onsite), salary range, job type, experience level, skills required
- Custom application questions
- Opening/closing date
- Status: Draft / Active / Paused / Closed / Expired
- Duplicate job
- Auto-expire after N days
- Featured/sponsored job toggle
- Preview before publish

---

### MODULE 23: Candidate Search (Resume Database)
**Features:**
- Full-text search by skills, title, location, experience
- Advanced filters
- Saved candidate lists
- View/download resume (subscription-gated)
- Bulk export
- Candidate shortlisting

---

### MODULE 24: Application Pipeline / ATS
**Features:**
- Per-job kanban board: New → Screening → Interview → Offer → Hired/Rejected
- Bulk status change
- Add interview notes per candidate
- Assign application to team member
- Rejection reason (logged)
- Communication log per application

---

### MODULE 25: Interview Scheduling
**Features:**
- Schedule interview (date, time, type: online/in-person)
- Send calendar invite via email
- Interview feedback form (rating per criteria)
- Multi-round interview tracking

---

### MODULE 30: Reports & Analytics (Employer)
**Features:**
- Jobs posted vs filled
- Time to hire
- Applications funnel (source → apply → interview → hire)
- Source analytics (where applicants came from)
- Export to CSV/Excel

---

### MODULE 33: Super Admin Dashboard
**Features:**
- Platform KPIs: total tenants, active jobs, registered users, applications today
- Revenue overview
- Recent sign-ups
- System health indicators

---

### MODULE 34: Tenant Management
**Features:**
- Create / suspend / delete tenant
- Per-tenant config (custom domain, branding, features)
- Impersonate tenant admin
- Usage statistics per tenant

---

### MODULE 35: Subscription Plans
**Features:**
- Create/edit plans: name, price, features, limits (jobs slots, resume views, users)
- Free / Basic / Professional / Enterprise tiers
- Assign plan to tenant
- Grace period on expiry

---

### MODULE 44: Authentication & Authorization
**Features:**
- Register with email/phone + OTP verification
- Social login: Google, LinkedIn
- JWT access token (15 min) + Refresh token (7 days, rotating)
- Role-based access control (RBAC)
- Permission-based guards
- 2FA (TOTP via authenticator app)
- Account lockout after N failed attempts
- Password strength enforcement
- Token revocation (logout / all sessions)
- Remember me functionality

---

### MODULE 45: Multi-Tenancy
**Strategy:** Shared database, tenant isolation via `TenantId` column  
**Features:**
- Tenant resolution via HTTP header / subdomain / JWT claim
- EF Core global query filters for TenantId + IsDeleted
- Tenant context injected via middleware
- Row-level security at database level (PostgreSQL RLS optional)

---

### MODULE 46: Notification System
**Channels:**
- In-app real-time via **SignalR** (`/hubs/notifications`)
- Email (SMTP / SendGrid)
- Push (Web Push via PWA service worker)

**SignalR Delivery Model:**

| Type | Scope | Mechanism | Example |
|------|-------|-----------|--------|
| **Public** | All connected users | `Clients.All.SendAsync(...)` | Platform-wide announcement, maintenance notice |
| **Tenant-wide** | All users of a tenant | `Clients.Group(tenantId).SendAsync(...)` | Tenant admin broadcasts |
| **Private** | Single user | `Clients.User(userId).SendAsync(...)` | Application status update, new message, interview invite |
| **Role-based** | All users with a role | `Clients.Group(roleGroupKey).SendAsync(...)` | Alerts to all recruiters in a tenant |

**Hub Design (`NotificationHub`):**
- Client connects and authenticates via JWT Bearer token over WebSocket
- On connect: user is added to their personal group (`user:{userId}`) and tenant group (`tenant:{tenantId}`)
- Server-side `IUserIdProvider` maps JWT `sub` claim → SignalR user identity
- Supports fallback transports: WebSocket → Server-Sent Events → Long Polling
- Hub methods: `MarkAsRead(notificationId)`, `MarkAllAsRead()`
- Client-side events: `ReceiveNotification`, `NotificationCountUpdated`

**Notification Service Flow:**
```
Business Event (e.g. ApplicationStatusChanged)
  → Domain Event raised
  → NotificationEventHandler (MediatR INotificationHandler)
  → INotificationService.SendAsync(notification)
      ├─ Persist to Notifications table
      ├─ SignalR dispatch (private/public based on notification type)
      ├─ Email dispatch (if user preference allows)
      └─ Web Push dispatch (if subscribed)
```

**Notification Types (discriminated by `NotificationType` enum):**
- `SystemAnnouncement` → Public (all users)
- `TenantAnnouncement` → Tenant-group
- `ApplicationStatusChanged` → Private (job seeker)
- `NewApplicationReceived` → Private (recruiter)
- `InterviewScheduled` → Private (candidate + interviewer)
- `NewMessage` → Private (recipient)
- `JobAlertMatch` → Private (job seeker)
- `OfferLetterSent` → Private (candidate)
- `SubscriptionExpiring` → Private (tenant admin)

**Features:**
- Templated notifications (Liquid templates stored in `master.NotificationTemplates`)
- Notification preferences per user per channel (in-app / email / push)
- Unread count badge updated in real-time via `NotificationCountUpdated` event
- Mark as read / mark all as read (persisted + real-time count sync)
- Notification history with pagination
- Stale connection handling: notifications persisted to DB so they are delivered on next login if user was offline
- Redis backplane (`Microsoft.AspNetCore.SignalR.StackExchangeRedis`) for horizontal scaling across multiple API instances

---

### MODULE 47: File Management
**Features:**
- File upload to Azure Blob / AWS S3 / local with pre-signed URLs
- Virus scan on upload
- File type/size validation
- CDN delivery for public assets
- Soft-delete files on record delete

---

### MODULE 50: GDPR / Privacy
**Features:**
- Consent banner on first visit
- Data export request
- Account deletion request (right to erasure)
- Cookie management
- Privacy policy & ToS pages

---

## 5. Technical Architecture

```
┌──────────────────────────────────────────────────┐
│           React + Vite PWA (MUI)                 │
│  React Router v6 · React Hook Form · Zod         │
│  React Query (TanStack) · Zustand · Notistack    │
└────────────────┬─────────────────────────────────┘
                 │ HTTPS / REST / WebSocket (SignalR)
┌────────────────▼─────────────────────────────────┐
│         .NET 10 Web API (Clean Architecture)     │
│  ┌───────────┬──────────────┬──────────────────┐ │
│  │  API Layer│ Application  │   Domain         │ │
│  │  Controllers│ CQRS/MediatR│ Entities/Rules  │ │
│  │  Middlewares│ FluentValid │ Value Objects    │ │
│  │  Auth/JWT  │ AutoMapper   │ Domain Events    │ │
│  └───────────┴──────────────┴──────────────────┘ │
│  ┌─────────────────────────────────────────────┐  │
│  │         Infrastructure Layer                │  │
│  │  EF Core (Npgsql) · Repository · UoW        │  │
│  │  Redis Cache · Email · File Storage         │  │
│  │  Elasticsearch · SignalR Hub                │  │
│  └─────────────────────────────────────────────┘  │
└────────────────┬─────────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────────┐
│   PostgreSQL (Primary)  │  Redis (Cache/Session)  │
│   Elasticsearch (Search)│  Blob Storage (Files)   │
└──────────────────────────────────────────────────┘
```

**Architecture Pattern:** Clean Architecture  
**API Pattern:** CQRS with MediatR  
**Validation:** FluentValidation (backend) + Zod (frontend)  
**Logging:** Serilog → structured JSON → Seq / Elasticsearch  
**Caching:** Redis (distributed cache, session store)  
**Real-time:** SignalR for notifications and messaging  

---

## 6. Database Design Conventions

### Standard Audit Columns (ALL tables must have):
```sql
TenantId        UUID        NOT NULL  -- Multi-tenancy
CreatedBy       UUID        NOT NULL  -- FK to Users
CreatedOn       TIMESTAMPTZ NOT NULL DEFAULT NOW()
ModifiedBy      UUID        NULL
ModifiedOn      TIMESTAMPTZ NULL
RecordVersion   BYTEA       NOT NULL  -- PostgreSQL xmin for optimistic concurrency
IsDeleted       BOOLEAN     NOT NULL DEFAULT FALSE  -- Soft delete
DeletedBy       UUID        NULL
DeletedOn       TIMESTAMPTZ NULL
```

### Naming Conventions (PascalCase throughout):
- **Schemas:** lowercase (`auth`, `master`, `portal`, `billing`, `config`)
- **Tables:** PascalCase, plural — `JobPostings`, `Users`, `RefreshTokens`
- **Columns:** PascalCase — `FirstName`, `TenantId`, `CreatedOn`, `IsDeleted`
- **Primary Keys:** `Id UUID DEFAULT gen_random_uuid()`
- **Foreign Keys:** `{Entity}Id` — e.g., `JobPostingId`, `TenantId`, `CreatedBy`
- **Indexes:** `idx_{TableName}_{ColumnName(s)}` — e.g., `idx_JobPostings_TenantId_Status`
- **Unique Constraints:** `uq_{TableName}_{ColumnName(s)}`
- **Check Constraints:** `chk_{TableName}_{ColumnName}_{rule}`
- **Sequences:** `seq_{TableName}_{ColumnName}` (rarely needed with UUID PKs)
- **Stored Procedures / Functions:** `fn_{schema}_{purpose}` — e.g., `fn_portal_search_jobs`
- **Triggers:** `trg_{TableName}_{event}` — e.g., `trg_Users_UpdateModifiedOn`
- **Views:** PascalCase, prefixed `vw_` — e.g., `vw_ActiveJobListings`
- All FK columns **must** be indexed
- No abbreviations (use `Description` not `Desc`, `IsDeleted` not `IsDel`)

### PostgreSQL Specifics:
- Use `gen_random_uuid()` for UUID generation
- Use `xmin` system column for optimistic concurrency (`RecordVersion`)
- Enable Row Level Security (RLS) on sensitive tables
- Use `TIMESTAMPTZ` (not TIMESTAMP) for all datetime fields
- Use `TEXT` over `VARCHAR(n)` unless constraint needed

---

## 7. Database Schema — All Tables

### Schema: `auth`
```
Tenants               -- SaaS tenant registry
Users                 -- All users across tenants
UserRoles             -- User ↔ Role mapping
Roles                 -- RBAC roles per tenant
Permissions           -- Granular permissions
RolePermissions       -- Role ↔ Permission mapping
RefreshTokens         -- JWT refresh token store
UserSessions          -- Active sessions
TwoFactorSettings     -- 2FA config per user
AuditLogs             -- All write-action audit trail
PasswordResetTokens   -- Password reset links
EmailVerifications    -- Email OTP verification
```

### Schema: `master`
```
Countries
States
Cities
Industries
JobCategories          -- Software, Finance, etc.
JobSubCategories       -- Frontend Dev, DBA, etc.
Skills                 -- Global skills dictionary
JobTypes               -- Full-time, Part-time, Contract, Freelance, Internship
ExperienceLevels       -- Entry, Mid, Senior, Lead, Executive
EducationLevels        -- High School, Bachelors, Masters, PhD
LanguageMaster         -- English, Hindi, etc.
CurrencyMaster         -- USD, INR, etc.
NotificationTemplates  -- Email/push templates
SubscriptionPlans      -- Free/Basic/Pro/Enterprise
SubscriptionFeatures   -- Features per plan
```

### Schema: `portal`
```
Companies              -- Employer company profiles
CompanyFollowers       -- JobSeeker follows Company
CompanyCultures        -- Culture tags per company
CompanyReviews         -- Employee reviews
CompanyBranches        -- Office locations

JobPostings            -- Core job listing
JobPostingSkills       -- Skills required for job
JobPostingQuestions    -- Custom application questions
JobPostingBenefits     -- Benefits listed per job
SavedJobs              -- JobSeeker saved/bookmarked jobs

JobSeekerProfiles      -- Extended profile data
WorkExperiences        -- Employment history
Educations             -- Academic history
Certifications         -- Professional certifications
Projects               -- Portfolio projects
Languages              -- Languages known per seeker
Awards                 -- Honors / awards
SocialLinks            -- GitHub, LinkedIn, etc.

Resumes                -- Uploaded resume metadata
ResumeFiles            -- File store references

JobApplications        -- Application records
ApplicationAnswers     -- Answers to custom questions
ApplicationStatusHistory  -- Trail of status changes
ApplicationNotes       -- Recruiter internal notes

CandidatePipelines     -- Kanban stages per job
CandidatePipelineStages

InterviewSchedules     -- Interview bookings
InterviewRounds        -- Multiple rounds per application
InterviewFeedbacks     -- Interviewer feedback per round

OfferLetters           -- Offer sent to candidate

Messages               -- Recruiter ↔ Candidate chat
MessageThreads         -- Conversation thread

JobAlerts              -- Job seeker alert subscriptions
Notifications          -- In-app notification feed
NotificationPreferences -- User notification settings

SavedSearches          -- Saved job search queries
CandidateSavedLists    -- Recruiter saved candidate lists
CandidateSavedListMembers

SkillAssessments       -- Quiz/assessment definitions
SkillAssessmentQuestions
SkillAssessmentResults -- Job seeker results

UserActivityLogs       -- Page views, clicks for analytics
JobViewLogs            -- Job detail views (deduplicated)
ApplicationSourceLogs  -- Where did applicants come from
```

### Schema: `billing`
```
TenantSubscriptions    -- Active subscription per tenant
SubscriptionInvoices   -- Invoice records
PaymentTransactions    -- Payment gateway transactions
JobCredits             -- Credits system for job posting slots
ResumeViewCredits      -- Credits for viewing resumes
```

### Schema: `config`
```
TenantSettings         -- Per-tenant config (theme, domain, features)
GlobalSettings         -- Platform-wide key-value config
EmailTemplates         -- Overridable email templates per tenant
FeatureFlags           -- Feature toggle per tenant / globally
Announcements          -- Admin broadcast banners
ContentPages           -- CMS pages (About, Privacy, ToS)
```

---

## 8. Backend Project Structure

```
JobPortal.sln
├── src/
│   ├── JobPortal.Domain/
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs            -- Id, TenantId, audit fields
│   │   │   ├── IAggregateRoot.cs
│   │   │   ├── IDomainEvent.cs
│   │   │   └── ValueObject.cs
│   │   ├── Entities/
│   │   │   ├── Auth/                    -- User, Role, Tenant, etc.
│   │   │   ├── Portal/                  -- Job, Application, Profile, etc.
│   │   │   ├── Master/                  -- Skills, Industries, etc.
│   │   │   └── Billing/                 -- Subscription, Invoice, etc.
│   │   ├── Enums/
│   │   ├── Events/                      -- Domain events
│   │   └── Exceptions/                  -- Domain exceptions
│   │
│   ├── JobPortal.Application/
│   │   ├── Common/
│   │   │   ├── Behaviors/
│   │   │   │   ├── ValidationBehavior.cs    -- FluentValidation pipeline
│   │   │   │   ├── LoggingBehavior.cs
│   │   │   │   ├── PerformanceBehavior.cs   -- Slow query warning
│   │   │   │   └── AuthorizationBehavior.cs
│   │   │   ├── Interfaces/
│   │   │   │   ├── ICurrentUserService.cs
│   │   │   │   ├── ITenantService.cs
│   │   │   │   ├── IDateTimeService.cs
│   │   │   │   ├── IEmailService.cs
│   │   │   │   ├── IFileStorageService.cs
│   │   │   │   ├── ICacheService.cs
│   │   │   │   └── INotificationService.cs
│   │   │   └── Models/
│   │   │       ├── PagedList.cs
│   │   │       ├── Result.cs            -- Result<T> pattern
│   │   │       └── ApiResponse.cs
│   │   ├── Features/
│   │   │   ├── Auth/
│   │   │   │   ├── Commands/            -- Login, Register, RefreshToken, etc.
│   │   │   │   └── Queries/
│   │   │   ├── Jobs/
│   │   │   │   ├── Commands/
│   │   │   │   └── Queries/
│   │   │   ├── Applications/
│   │   │   ├── Profiles/
│   │   │   ├── Companies/
│   │   │   ├── Candidates/
│   │   │   ├── Notifications/
│   │   │   ├── Master/
│   │   │   └── Admin/
│   │   └── Mappings/                    -- AutoMapper profiles
│   │
│   ├── JobPortal.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Configurations/          -- IEntityTypeConfiguration per entity
│   │   │   ├── Migrations/
│   │   │   ├── Repositories/
│   │   │   │   ├── GenericRepository.cs
│   │   │   │   └── {Entity}Repository.cs
│   │   │   ├── UnitOfWork.cs
│   │   │   └── Interceptors/
│   │   │       └── AuditInterceptor.cs  -- Auto-fill audit fields
│   │   ├── Services/
│   │   │   ├── CurrentUserService.cs
│   │   │   ├── TenantService.cs
│   │   │   ├── EmailService.cs
│   │   │   ├── FileStorageService.cs
│   │   │   ├── CacheService.cs (Redis)
│   │   │   ├── NotificationService.cs
│   │   │   └── SearchService.cs (Elasticsearch)
│   │   ├── Identity/
│   │   │   ├── JwtTokenService.cs
│   │   │   └── PasswordService.cs
│   │   └── Hubs/
│   │       └── Hubs/
│   │           ├── NotificationHub.cs           -- SignalR hub (/hubs/notifications)
│   │           ├── INotificationHubClient.cs    -- Strongly-typed client contract
│   │           └── NotificationUserIdProvider.cs -- Maps JWT sub → SignalR userId
│   │
│   └── JobPortal.API/
│       ├── Controllers/
│       │   ├── v1/
│       │   │   ├── AuthController.cs
│       │   │   ├── JobsController.cs
│       │   │   ├── ApplicationsController.cs
│       │   │   ├── ProfilesController.cs
│       │   │   ├── CompaniesController.cs
│       │   │   ├── CandidatesController.cs
│       │   │   ├── NotificationsController.cs
│       │   │   ├── MasterController.cs
│       │   │   └── AdminController.cs
│       ├── Middlewares/
│       │   ├── TenantMiddleware.cs       -- Resolves TenantId
│       │   ├── ExceptionMiddleware.cs    -- Global error handler
│       │   ├── CorrelationIdMiddleware.cs
│       │   └── RateLimitingMiddleware.cs
│       ├── Filters/
│       │   └── ValidationActionFilter.cs
│       ├── Extensions/
│       │   ├── ServiceCollectionExtensions.cs
│       │   ├── ApplicationBuilderExtensions.cs
│       │   └── SwaggerExtensions.cs
│       └── Program.cs
│
└── tests/
    ├── JobPortal.UnitTests/
    ├── JobPortal.IntegrationTests/
    └── JobPortal.ArchitectureTests/

db/
├── schemas/
│   ├── 00_extensions.sql              -- pgcrypto, pg_trgm, uuid-ossp
│   ├── 01_schemas.sql                 -- CREATE SCHEMA auth, master, portal, billing, config
│   ├── 02_types.sql                   -- Custom ENUMs (ApplicationStatus, JobType, etc.)
│   └── 03_functions.sql               -- Shared DB functions & triggers
├── tables/
│   ├── auth/
│   │   ├── 01_Tenants.sql
│   │   ├── 02_Users.sql
│   │   ├── 03_Roles.sql
│   │   ├── 04_Permissions.sql
│   │   ├── 05_RolePermissions.sql
│   │   ├── 06_UserRoles.sql
│   │   ├── 07_RefreshTokens.sql
│   │   ├── 08_UserSessions.sql
│   │   ├── 09_TwoFactorSettings.sql
│   │   ├── 10_AuditLogs.sql
│   │   ├── 11_PasswordResetTokens.sql
│   │   └── 12_EmailVerifications.sql
│   ├── master/
│   │   ├── 01_Countries.sql
│   │   ├── 02_States.sql
│   │   ├── 03_Cities.sql
│   │   ├── 04_Industries.sql
│   │   ├── 05_JobCategories.sql
│   │   ├── 06_JobSubCategories.sql
│   │   ├── 07_Skills.sql
│   │   ├── 08_JobTypes.sql
│   │   ├── 09_ExperienceLevels.sql
│   │   ├── 10_EducationLevels.sql
│   │   ├── 11_LanguageMaster.sql
│   │   ├── 12_CurrencyMaster.sql
│   │   ├── 13_NotificationTemplates.sql
│   │   ├── 14_SubscriptionPlans.sql
│   │   └── 15_SubscriptionFeatures.sql
│   ├── portal/
│   │   ├── 01_Companies.sql
│   │   ├── 02_CompanyFollowers.sql
│   │   ├── 03_CompanyCultures.sql
│   │   ├── 04_CompanyReviews.sql
│   │   ├── 05_CompanyBranches.sql
│   │   ├── 06_JobPostings.sql
│   │   ├── 07_JobPostingSkills.sql
│   │   ├── 08_JobPostingQuestions.sql
│   │   ├── 09_JobPostingBenefits.sql
│   │   ├── 10_SavedJobs.sql
│   │   ├── 11_JobSeekerProfiles.sql
│   │   ├── 12_WorkExperiences.sql
│   │   ├── 13_Educations.sql
│   │   ├── 14_Certifications.sql
│   │   ├── 15_Projects.sql
│   │   ├── 16_Languages.sql
│   │   ├── 17_Awards.sql
│   │   ├── 18_SocialLinks.sql
│   │   ├── 19_Resumes.sql
│   │   ├── 20_ResumeFiles.sql
│   │   ├── 21_JobApplications.sql
│   │   ├── 22_ApplicationAnswers.sql
│   │   ├── 23_ApplicationStatusHistory.sql
│   │   ├── 24_ApplicationNotes.sql
│   │   ├── 25_CandidatePipelineStages.sql
│   │   ├── 26_CandidatePipelines.sql
│   │   ├── 27_InterviewSchedules.sql
│   │   ├── 28_InterviewRounds.sql
│   │   ├── 29_InterviewFeedbacks.sql
│   │   ├── 30_OfferLetters.sql
│   │   ├── 31_MessageThreads.sql
│   │   ├── 32_Messages.sql
│   │   ├── 33_JobAlerts.sql
│   │   ├── 34_Notifications.sql
│   │   ├── 35_NotificationPreferences.sql
│   │   ├── 36_SavedSearches.sql
│   │   ├── 37_CandidateSavedLists.sql
│   │   ├── 38_CandidateSavedListMembers.sql
│   │   ├── 39_SkillAssessments.sql
│   │   ├── 40_SkillAssessmentQuestions.sql
│   │   ├── 41_SkillAssessmentResults.sql
│   │   ├── 42_UserActivityLogs.sql
│   │   ├── 43_JobViewLogs.sql
│   │   └── 44_ApplicationSourceLogs.sql
│   ├── billing/
│   │   ├── 01_TenantSubscriptions.sql
│   │   ├── 02_SubscriptionInvoices.sql
│   │   ├── 03_PaymentTransactions.sql
│   │   ├── 04_JobCredits.sql
│   │   └── 05_ResumeViewCredits.sql
│   └── config/
│       ├── 01_TenantSettings.sql
│       ├── 02_GlobalSettings.sql
│       ├── 03_EmailTemplates.sql
│       ├── 04_FeatureFlags.sql
│       ├── 05_Announcements.sql
│       └── 06_ContentPages.sql
├── indexes/
│   ├── auth_indexes.sql
│   ├── master_indexes.sql
│   ├── portal_indexes.sql
│   ├── billing_indexes.sql
│   └── config_indexes.sql
├── rls/
│   └── row_level_security.sql         -- PostgreSQL RLS policies
├── seed/
│   ├── 01_seed_countries.sql
│   ├── 02_seed_states.sql
│   ├── 03_seed_cities.sql
│   ├── 04_seed_industries.sql
│   ├── 05_seed_job_categories.sql
│   ├── 06_seed_skills.sql
│   ├── 07_seed_roles_permissions.sql
│   ├── 08_seed_subscription_plans.sql
│   └── 09_seed_global_settings.sql
├── migrations/                        -- EF Core managed (generated)
└── run_all.sql                        -- Master script: runs all in order
```

---

## 9. Frontend Project Structure

```
src/
├── assets/                     -- images, fonts, icons
├── components/                 -- Generic/shared components
│   ├── ui/
│   │   ├── AppTextField.tsx        -- Wraps MUI TextField
│   │   ├── AppSelect.tsx           -- Wraps MUI Select
│   │   ├── AppAutocomplete.tsx     -- Wraps MUI Autocomplete
│   │   ├── AppDatePicker.tsx       -- Wraps MUI DatePicker
│   │   ├── AppDataGrid.tsx         -- Wraps MUI DataGrid
│   │   ├── AppButton.tsx           -- Wraps MUI Button
│   │   ├── AppDialog.tsx           -- Wraps MUI Dialog
│   │   ├── ConfirmDialog.tsx       -- Generic confirmation dialog
│   │   ├── AppChip.tsx             -- Wraps MUI Chip
│   │   ├── AppBadge.tsx            -- Wraps MUI Badge
│   │   ├── AppCard.tsx             -- Wraps MUI Card
│   │   ├── AppSkeleton.tsx         -- Loading skeletons
│   │   ├── AppFileUpload.tsx       -- File upload with preview
│   │   ├── AppRichTextEditor.tsx   -- Rich text (TipTap/Quill)
│   │   ├── AppStatusBadge.tsx      -- Colored status chip
│   │   └── AppPageHeader.tsx       -- Page title + breadcrumb
│   ├── form/
│   │   ├── AppForm.tsx             -- react-hook-form wrapper
│   │   ├── FormTextField.tsx       -- RHF-connected TextField
│   │   ├── FormSelect.tsx
│   │   ├── FormAutocomplete.tsx
│   │   ├── FormDatePicker.tsx
│   │   └── FormFileUpload.tsx
│   └── layout/
│       ├── AppLayout.tsx
│       ├── AuthLayout.tsx
│       ├── Sidebar.tsx
│       ├── Topbar.tsx
│       └── Footer.tsx
│
├── hooks/
│   ├── useUnsavedChanges.ts        -- Prompt when leaving dirty form
│   ├── useNotification.ts          -- Wrapper for notistack/snackbar
│   ├── useConfirmDialog.ts         -- Programmatic confirm dialog
│   ├── useDebounce.ts
│   ├── usePagination.ts
│   ├── useAuth.ts
│   └── useTenant.ts
│
├── services/                       -- API calls (axios-based)
│   ├── apiClient.ts                -- Axios instance with interceptors
│   ├── authService.ts
│   ├── jobService.ts
│   ├── applicationService.ts
│   ├── profileService.ts
│   ├── companyService.ts
│   ├── candidateService.ts
│   ├── notificationService.ts
│   └── masterService.ts
│
├── store/                          -- Zustand stores
│   ├── authStore.ts
│   ├── tenantStore.ts
│   └── uiStore.ts
│
├── pages/
│   ├── public/
│   │   ├── HomePage/
│   │   ├── JobSearchPage/
│   │   ├── JobDetailPage/
│   │   ├── CompanyListPage/
│   │   └── CompanyDetailPage/
│   ├── auth/
│   │   ├── LoginPage/
│   │   ├── RegisterPage/
│   │   ├── ForgotPasswordPage/
│   │   └── ResetPasswordPage/
│   ├── seeker/
│   │   ├── DashboardPage/
│   │   ├── ProfilePage/
│   │   ├── ResumeBuilderPage/
│   │   ├── ApplicationsPage/
│   │   ├── SavedJobsPage/
│   │   ├── AlertsPage/
│   │   └── SettingsPage/
│   ├── employer/
│   │   ├── DashboardPage/
│   │   ├── CompanyProfilePage/
│   │   ├── JobPostingsPage/
│   │   ├── JobPostingFormPage/
│   │   ├── CandidateSearchPage/
│   │   ├── ApplicationPipelinePage/
│   │   ├── InterviewsPage/
│   │   ├── TeamPage/
│   │   ├── ReportsPage/
│   │   └── BillingPage/
│   └── admin/
│       ├── DashboardPage/
│       ├── TenantsPage/
│       ├── SubscriptionPlansPage/
│       ├── SkillsPage/
│       ├── IndustriesPage/
│       ├── AuditLogsPage/
│       └── SettingsPage/
│
├── router/
│   ├── index.tsx                   -- React Router v6 setup
│   ├── ProtectedRoute.tsx
│   ├── RoleGuard.tsx
│   └── routes.ts
│
├── theme/
│   ├── theme.ts                    -- MUI theme customization
│   └── palette.ts
│
├── utils/
│   ├── formatters.ts              -- Date, currency, number formatters
│   ├── validators.ts              -- Zod schemas
│   ├── constants.ts
│   └── helpers.ts
│
├── types/
│   ├── auth.types.ts
│   ├── job.types.ts
│   ├── application.types.ts
│   ├── profile.types.ts
│   └── common.types.ts
│
├── i18n/                          -- Internationalization (future)
├── sw.ts                          -- Service Worker (PWA)
├── App.tsx
├── main.tsx
└── vite-env.d.ts
```

---

## 10. API Design Standards

### URL Structure
```
/api/v1/{resource}
/api/v1/{resource}/{id}
/api/v1/{resource}/{id}/{sub-resource}
```

Examples:
```
GET    /api/v1/jobs                        -- List jobs (paginated, filtered)
POST   /api/v1/jobs                        -- Create job posting
GET    /api/v1/jobs/{id}                   -- Get job detail
PUT    /api/v1/jobs/{id}                   -- Update job
DELETE /api/v1/jobs/{id}                   -- Soft delete job
GET    /api/v1/jobs/{id}/applications      -- List applications for a job
POST   /api/v1/jobs/{id}/applications      -- Apply to job
PATCH  /api/v1/applications/{id}/status    -- Update application status
```

### Response Envelope
```json
{
  "success": true,
  "data": { ... },
  "errors": null,
  "message": "Job created successfully",
  "traceId": "abc-123",
  "timestamp": "2026-04-08T12:00:00Z"
}
```

### Error Response
```json
{
  "success": false,
  "data": null,
  "errors": [
    { "field": "Title", "message": "Title is required" }
  ],
  "message": "Validation failed",
  "traceId": "abc-123",
  "timestamp": "2026-04-08T12:00:00Z"
}
```

### Pagination (Cursor-based for large datasets)
```json
{
  "items": [...],
  "totalCount": 50000,
  "pageSize": 20,
  "pageNumber": 1,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### HTTP Status Codes
| Code | Usage |
|------|-------|
| 200 | Success GET/PUT/PATCH |
| 201 | Created POST |
| 204 | No Content DELETE |
| 400 | Validation error |
| 401 | Unauthenticated |
| 403 | Forbidden (role/permission) |
| 404 | Not found |
| 409 | Conflict (duplicate, concurrency) |
| 422 | Business rule violation |
| 429 | Rate limit exceeded |
| 500 | Internal server error |

---

## 11. Security Strategy (OWASP Top 10)

### A01 – Broken Access Control
- JWT-based authentication with short expiry (15 min)
- Rotating refresh tokens stored in HttpOnly cookies
- TenantId in every query (multi-tenant isolation)
- Resource-level ownership checks (user can only edit own records)
- RBAC + permission-based authorization attributes
- Global query filters prevent cross-tenant data access

### A02 – Cryptographic Failures
- **Passwords hashed with Argon2id** (memory: 64 MB, iterations: 3, parallelism: 4) — OWASP recommended winner of Password Hashing Competition; resistant to GPU/ASIC brute force
- Argon2id parameters stored alongside hash so they can be upgraded transparently
- Pepper added to hash input (application-side secret from env var) for defense-in-depth against DB-only leaks
- JWT signed with RS256 (asymmetric, 4096-bit key pair)
- HTTPS enforced (HSTS headers, `max-age=31536000; includeSubDomains`)
- Sensitive PII fields encrypted at rest using AES-256-GCM via application-layer encryption before storing
- No PII in logs, query strings, or error responses
- Refresh tokens are SHA-256 hashed before stored in DB (only plaintext sent to client once)

### A03 – Injection
- EF Core parameterized queries (no raw SQL interpolation)
- FluentValidation on all inputs
- Input sanitization for rich text (HtmlSanitizer)
- Zod validation on frontend before submission

### A04 – Insecure Design
- Threat modeling per module
- Defense in depth: validate at API + DB level
- Principle of least privilege for DB users
- Separate connection strings per environment

### A05 – Security Misconfiguration
- Environment-specific config via environment variables (not appsettings)
- Swagger disabled in production
- Detailed errors only in development
- Security headers middleware (CSP, X-Frame-Options, etc.)

### A06 – Vulnerable Components
- Dependabot / Renovate for dependency updates
- NuGet package audit in CI pipeline
- npm audit in frontend CI

### A07 – Identity & Auth Failures
- Account lockout (5 failed attempts → 15-min lock)
- 2FA support (TOTP)
- OTP for email/phone verification
- Session invalidation on password change
- Concurrent session limits

### A08 – Data Integrity Failures
- RecordVersion (xmin) for optimistic concurrency
- CSRF protection (SameSite=Strict cookies)
- Content-type validation on file uploads
- Signed URLs for file access

### A09 – Logging & Monitoring Failures
- Serilog structured logging with correlation IDs
- Log all auth events (login, logout, failed attempts)
- Audit trail table for all data mutations
- Alerting on anomalous patterns

### A10 – SSRF
- Whitelist allowed external domains
- No user-controlled URL fetching
- Network egress restrictions

---

## 12. Performance Strategy

### Database
- Connection pooling (PgBouncer or Npgsql built-in)
- Read replicas for heavy read queries
- Proper indexes on: TenantId, UserId, CompanyId, Status, CreatedOn, IsDeleted
- Composite indexes for common filter combinations
- Avoid N+1 queries: use `.Include()` judiciously or split queries
- Pagination: use cursor-based pagination for large result sets
- Compiled EF queries for hot paths
- Database-level search (pg_trgm) as fallback; Elasticsearch for production scale

### Caching
- Redis for: JWT blacklist, user sessions, master data (skills, industries), rate limit counters
- Response caching for public API endpoints (job listings, company profiles)
- Cache invalidation strategy: event-driven cache busting

### API
- Response compression (Brotli + GZip)
- Async/await throughout — no synchronous blocking
- CancellationToken propagation everywhere
- Streaming for large file downloads

### Frontend
- React.lazy + Suspense for code splitting per route
- Virtual scrolling for large lists (react-window / MUI DataGrid virtualization)
- Memoization with React.memo, useMemo, useCallback
- TanStack Query for server state caching and deduplication
- Optimistic UI updates for mutations
- Image lazy loading + WebP format
- PWA: cache shell + API responses in service worker

### Infrastructure
- CDN for static assets
- Load balancer with sticky sessions (for SignalR)
- Horizontal scaling — stateless API
- Background jobs via Hangfire (email sending, alerts, cleanup)

---

## 13. Implementation Phases & Roadmap

### Phase 1 — Foundation (Weeks 1–4)
- [ ] Initialize Solution (.NET 10 Clean Architecture)
- [ ] Initialize Frontend (React + Vite + PWA + MUI)
- [ ] Generic base entities, audit infrastructure
- [ ] **Create all DB scripts** (`db/` folder: schemas, tables, indexes, RLS, seed data)
- [ ] Database setup (PostgreSQL + EF Core + Migrations)
- [ ] Multi-tenancy engine (middleware + EF global filters)
- [ ] Authentication: Register, Login, JWT, Refresh Token
- [ ] Role/Permission system
- [ ] API response envelope + global exception middleware
- [ ] Serilog setup
- [ ] Frontend: Generic UI components (MUI wrappers)
- [ ] Frontend: Auth pages (Login, Register)

### Phase 2 — Core Job Portal (Weeks 5–10)
- [ ] Master data (Skills, Industries, Job Types, Countries/Cities)
- [ ] Company profile management (Employer)
- [ ] Job Posting CRUD
- [ ] Public job search (with filters)
- [ ] Job Seeker profile (work experience, education, skills)
- [ ] Resume upload & storage
- [ ] Job application flow
- [ ] Application status tracking
- [ ] Email notifications (application received, status change)

### Phase 3 — Advanced Features (Weeks 11–16)
- [ ] Candidate search (Employer)
- [ ] Application Pipeline / ATS (Kanban)
- [ ] Interview scheduling
- [ ] Job alerts (cron job)
- [ ] In-app notifications (SignalR)
- [ ] Messaging (Recruiter ↔ Candidate)
- [ ] Company reviews
- [ ] Saved jobs / candidates

### Phase 4 — SaaS & Admin (Weeks 17–21)
- [ ] Super Admin panel
- [ ] Tenant management
- [ ] Subscription plans
- [ ] Billing integration (Stripe / Razorpay)
- [ ] Job credits / resume view credits
- [ ] Reports & analytics
- [ ] Feature flags per tenant

### Phase 5 — Polish & Scale (Weeks 22–26)
- [ ] Elasticsearch integration
- [ ] Redis caching layer
- [ ] PWA push notifications
- [ ] 2FA
- [ ] GDPR features (data export, deletion)
- [ ] Performance testing & optimization
- [ ] SEO (SSG/SSR consideration via Vite SSR or separate Next.js)
- [ ] Security audit

---

## 14. Technology Stack & Packages

### Backend (.NET 10)
```
Microsoft.AspNetCore                   -- Web API framework
MediatR                                -- CQRS mediator
FluentValidation.AspNetCore            -- Validation
AutoMapper                             -- Object mapping
Microsoft.EntityFrameworkCore          -- ORM
Npgsql.EntityFrameworkCore.PostgreSQL  -- PostgreSQL driver
StackExchange.Redis                    -- Redis client
Serilog.AspNetCore                     -- Structured logging
Serilog.Sinks.Elasticsearch            -- Log sink
Microsoft.AspNetCore.SignalR           -- Real-time hub
Microsoft.AspNetCore.SignalR.StackExchangeRedis  -- Redis backplane for multi-instance scaling
Hangfire.PostgreSql                    -- Background jobs
AWSSDK.S3 / Azure.Storage.Blobs        -- File storage
MailKit                                -- Email
System.IdentityModel.Tokens.Jwt        -- JWT
Isopoh.Cryptography.Argon2             -- Argon2id password hashing (OWASP recommended)
Swashbuckle.AspNetCore                 -- Swagger
Elastic.Clients.Elasticsearch          -- Elasticsearch
HtmlSanitizerAspNetCore                -- XSS prevention
Polly                                  -- Resilience / retry
```

### Frontend (React / Vite)
```
react@latest + react-dom
react-router-dom v6
@mui/material + @mui/x-data-grid + @mui/x-date-pickers
@emotion/react + @emotion/styled
@tanstack/react-query          -- Server state management
react-hook-form                -- Form management
zod + @hookform/resolvers      -- Schema validation
zustand                        -- Client state management
axios                          -- HTTP client
notistack                      -- Snackbar / toast notifications
react-dropzone                 -- File uploads
@tiptap/react                  -- Rich text editor
recharts                       -- Charts / analytics
date-fns                       -- Date utilities
vite-plugin-pwa                -- PWA plugin
workbox-*                      -- Service worker
react-window                   -- Virtual lists
typescript
eslint + prettier              -- Lint & format
```

---

## Notes & Decisions

1. **Tenant Resolution:** Use `X-Tenant-Id` HTTP header (or JWT claim for logged-in users). Public endpoints use subdomain resolution.
2. **Concurrency:** EF Core uses `xmin` (PostgreSQL system column) mapped as `RowVersion` for optimistic concurrency.
3. **File Storage:** Abstract `IFileStorageService` — start with local disk in dev, swap to S3/Azure Blob in prod.
4. **Search:** Start with PostgreSQL full-text search (`to_tsvector` + `pg_trgm`). Introduce Elasticsearch in Phase 5.
5. **Email:** Use `IEmailService` abstraction backed by MailKit/SMTP in dev, SendGrid in production.
6. **Soft Delete:** Applied globally via EF query filter `WHERE IsDeleted = FALSE`. Call `SoftDelete()` instead of `Remove()`.
7. **SemVer API:** Version all APIs from day 1 (`/api/v1/`) to allow non-breaking evolution.
8. **Password Hashing:** Argon2id with `memoryCost=65536` (64 MB), `iterations=3`, `parallelism=4`, `hashLength=32`. Parameters are versioned so cost factors can be increased over time without invalidating existing hashes.
9. **DB Scripts:** All SQL scripts live in `db/` at solution root. Numbered prefix ensures deterministic execution order. `run_all.sql` is the master entry point for fresh environments. EF Core `Migrations/` folder handles incremental schema changes in CI/CD — both approaches co-exist (scripts for DBA review, migrations for automation).
10. **PascalCase DB Convention:** Enforced via EF Core `UseQuotedIdentifiers()` on NpgsqlModelBuilderExtensions and `ToTable("TableName", schema: "schemaname")` in all entity configurations. PostgreSQL defaults to lowercase identifiers — quoting ensures PascalCase is preserved exactly.
