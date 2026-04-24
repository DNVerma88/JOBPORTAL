# JobPortal

> **Multi-Tenant SaaS Job Portal** — Naukri / Hirist style platform  
> React 19 PWA · .NET 10 Clean Architecture API · PostgreSQL · Redis · Docker

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Architecture Overview](#2-architecture-overview)
3. [Tech Stack](#3-tech-stack)
4. [Backend Architecture](#4-backend-architecture)
5. [Frontend Architecture](#5-frontend-architecture)
6. [Database Architecture](#6-database-architecture)
7. [Infrastructure & DevOps](#7-infrastructure--devops)
8. [Security](#8-security)
9. [Modules & Features](#9-modules--features)
10. [Getting Started](#10-getting-started)
11. [Project Structure](#11-project-structure)
12. [Roadmap](#12-roadmap)

---

## 1. Project Overview

JobPortal is a **production-grade, multi-tenant SaaS job platform** that connects:

| Actor | Role |
|---|---|
| **Job Seekers** | Register, build profiles, upload resumes, search & apply for jobs |
| **Employers / Recruiters** | Post jobs, search candidate database, manage ATS pipeline |
| **Hiring Managers** | Review shortlisted candidates, schedule interviews, submit feedback |
| **Tenant Admins** | Manage their organisation, branding, team members, billing |
| **Super Admins** | Manage all tenants, subscriptions, platform config, audit logs |
| **Public / Guests** | Browse public job listings without logging in |

### Key Design Goals

- **Multi-tenancy** — Row-Level Security in PostgreSQL; each tenant's data is fully isolated
- **PWA** — Installable, offline-capable React frontend
- **Scalable** — Redis cache, cursor-based pagination, connection pooling
- **Secure** — JWT RS256 + refresh tokens, Argon2id password hashing, OWASP Top 10 compliance
- **Real-time** — SignalR for live notifications

---

## 2. Architecture Overview

```
┌──────────────────────────────────────────────────────────┐
│                        Browser / PWA                      │
│            React 19 + Vite + MUI v9 (port 3000)          │
└────────────────────────────┬─────────────────────────────┘
                             │ HTTPS / WebSocket (SignalR)
┌────────────────────────────▼─────────────────────────────┐
│                    .NET 10 REST API                        │
│         Clean Architecture — CQRS + MediatR              │
│              Versioned: /api/v1/...  (port 5000)          │
└──────┬──────────────────────────────────┬────────────────┘
       │                                  │
┌──────▼──────┐                  ┌────────▼────────┐
│ PostgreSQL  │                  │     Redis        │
│ (port 5433) │                  │  (port 6380)     │
│  5 schemas  │                  │  Cache + Sessions│
└─────────────┘                  └──────────────────┘
```

### Communication Flow

1. The React SPA communicates with the API over HTTPS. Axios handles JWT injection and silent token refresh.
2. The API processes commands/queries via MediatR pipelines (validation → authorization → handler → response).
3. EF Core 10 maps domain entities to PostgreSQL using Npgsql. Row-Level Security enforces tenant isolation at DB level.
4. Redis caches hot data (job listings, master data) and stores rate-limit counters.
5. SignalR pushes real-time notifications to connected clients via `NotificationHub`.
6. Email is dispatched asynchronously via SMTP (MailHog in dev, configurable SMTP in production).

---

## 3. Tech Stack

### Backend

| Layer | Technology |
|---|---|
| Runtime | .NET 10 (C# 13) |
| Web Framework | ASP.NET Core 10 |
| Architecture | Clean Architecture, CQRS, DDD |
| Mediator | MediatR 12 |
| ORM | Entity Framework Core 10 |
| DB Driver | Npgsql 10 (PostgreSQL) |
| Auth | JWT RS256 + Refresh Tokens, ASP.NET Identity (custom) |
| Password Hashing | Argon2id (Konscious.Security.Cryptography) |
| Real-time | SignalR |
| Cache | Redis (StackExchange.Redis) |
| Email | SMTP (MailKit) |
| Validation | FluentValidation |
| Logging | Serilog + structured logs |
| API Docs | Scalar / OpenAPI |

### Frontend

| Layer | Technology |
|---|---|
| Framework | React 19 |
| Build Tool | Vite 6 |
| UI Library | MUI v9 (Material UI) |
| State Management | Zustand |
| Server State | TanStack Query v5 |
| Forms | React Hook Form + Zod validation |
| Routing | React Router v7 |
| HTTP Client | Axios (with JWT interceptors) |
| PWA | vite-plugin-pwa |
| Language | TypeScript 5 |

### Database & Infrastructure

| Component | Technology |
|---|---|
| Primary DB | PostgreSQL 16 |
| Cache / Sessions | Redis 7 |
| Dev SMTP | MailHog |
| Containerisation | Docker + Docker Compose |
| Web Server | Nginx (frontend) |
| Secret Generation | Custom .NET tool (`tools/gen-secrets`) |

---

## 4. Backend Architecture

The backend follows **Clean Architecture** with strict dependency rules (outer layers depend on inner, never the reverse).

### Layer Structure

```
src/backend/src/
├── JobPortal.Domain/          ← Core business logic (no dependencies)
├── JobPortal.Application/     ← Use cases: Commands, Queries, Handlers
├── JobPortal.Infrastructure/  ← EF Core, Redis, Email, SignalR, File Storage
└── JobPortal.API/             ← ASP.NET Core: Controllers, Middleware, DI
```

#### Domain Layer (`JobPortal.Domain`)

Pure C# — no framework dependencies.

```
Domain/
├── Entities/
│   ├── Auth/        → Tenant, User, Role, Permission, RefreshToken,
│   │                  UserSession, AuditLog
│   ├── Portal/      → JobPosting, JobApplication, JobSeekerProfile,
│   │                  Company, CandidatePipeline, CandidatePipelineStage,
│   │                  InterviewSchedule, OfferLetter, SavedJob,
│   │                  JobAlert, Notification, WorkExperience, Education
│   ├── Billing/     → TenantSubscription, SubscriptionInvoice,
│   │                  PaymentTransaction, JobCredit, ResumeViewCredit
│   ├── Master/      → Country, State, City, Industry, JobCategory,
│   │                  Skill, JobType, ExperienceLevel, EducationLevel
│   └── Config/      → TenantSettings, GlobalSettings, EmailTemplate,
│                       FeatureFlag, Announcement, ContentPage
├── Enums/           → ApplicationStatus, JobStatus, PipelineStage, etc.
├── Events/          → Domain events (DomainEventBase, IHasDomainEvents)
├── Exceptions/      → Domain exceptions
└── Common/          → BaseEntity, IAuditableEntity, IHasTenant
```

#### Application Layer (`JobPortal.Application`)

CQRS via MediatR. Each feature folder contains Commands, Queries, Validators, and DTOs.

```
Application/
├── Features/
│   ├── Auth/        → Login, Register, RefreshToken, Logout,
│   │                  ForgotPassword, ResetPassword, VerifyEmail,
│   │                  TwoFactor
│   ├── Jobs/        → CreateJob, UpdateJob, DeleteJob, GetJob,
│   │                  ListJobs, SearchJobs, FeaturedJobs
│   ├── Applications/→ Apply, WithdrawApplication, GetApplication,
│   │                  ListApplications, UpdateApplicationStatus
│   ├── Candidates/  → SearchCandidates, GetCandidateProfile
│   ├── Pipeline/    → GetPipeline, MoveCandidateStage, BulkMove
│   ├── Companies/   → CreateCompany, UpdateCompany, GetCompany
│   ├── Interviews/  → ScheduleInterview, UpdateInterview, GetInterview
│   ├── Offers/      → CreateOffer, UpdateOffer, GetOffer
│   ├── Billing/     → GetSubscription, GetInvoices, GetCredits
│   ├── Notifications→ GetNotifications, MarkAsRead, BroadcastToTenant
│   ├── JobAlerts/   → CreateAlert, DeleteAlert, ListAlerts
│   ├── SavedJobs/   → SaveJob, UnsaveJob, ListSavedJobs
│   ├── Dashboard/   → GetJobSeekerDashboard, GetRecruiterDashboard
│   ├── Admin/       → TenantManagement, GlobalSettings, AuditLogs
│   ├── Master/      → Countries, States, Cities, Industries,
│   │                  Categories, Skills
│   └── Config/      → TenantSettings, EmailTemplates, FeatureFlags
├── Common/
│   ├── Behaviours/  → ValidationBehaviour, LoggingBehaviour,
│   │                  PerformanceBehaviour, AuthorizationBehaviour
│   ├── Interfaces/  → IApplicationDbContext, ICurrentUserService,
│   │                  ICacheService, IEmailService, IFileService,
│   │                  INotificationService, ITenantService
│   └── Models/      → Result<T>, PagedList<T>, PaginationParams
└── EventHandlers/   → Domain event handlers
```

#### Infrastructure Layer (`JobPortal.Infrastructure`)

```
Infrastructure/
├── Persistence/
│   ├── ApplicationDbContext.cs    → EF Core DbContext
│   ├── Configurations/            → Fluent API entity configs
│   └── Interceptors/              → Audit, soft-delete interceptors
├── Identity/                      → JWT generation, Argon2id hashing
├── Services/
│   ├── CurrentUserService         → Extracts user from HttpContext
│   ├── TenantService              → Resolves current tenant
│   ├── RedisCacheService          → IDistributedCache wrapper
│   ├── EmailService               → SMTP via MailKit
│   ├── LocalFileStorageService    → Resume/logo uploads
│   ├── SignalRNotificationService → Push via NotificationHub
│   └── DateTimeService            → UTC clock abstraction
├── Hubs/
│   ├── NotificationHub            → SignalR hub
│   └── NotificationUserIdProvider → Maps user ID to SignalR connection
└── Middleware/                    → Tenant resolution middleware
```

#### API Layer (`JobPortal.API`)

```
API/
├── Controllers/v1/
│   ├── AuthController             → /api/v1/auth
│   ├── JobsController             → /api/v1/jobs
│   ├── ApplicationsController     → /api/v1/applications
│   ├── CandidatesController       → /api/v1/candidates
│   ├── CompaniesController        → /api/v1/companies
│   ├── PipelineController         → /api/v1/pipeline
│   ├── InterviewsController       → /api/v1/interviews
│   ├── OffersController           → /api/v1/offers
│   ├── BillingController          → /api/v1/billing
│   ├── NotificationsController    → /api/v1/notifications
│   ├── JobAlertsController        → /api/v1/job-alerts
│   ├── SavedJobsController        → /api/v1/saved-jobs
│   ├── DashboardController        → /api/v1/dashboard
│   ├── MasterController           → /api/v1/master
│   ├── ConfigController           → /api/v1/config
│   └── AdminController            → /api/v1/admin
├── Middlewares/
│   ├── ExceptionHandlingMiddleware
│   └── RequestLoggingMiddleware
├── Extensions/                    → DI registration extension methods
└── Program.cs                     → App bootstrap
```

### MediatR Pipeline

Every request flows through:

```
Request → ValidationBehaviour (FluentValidation)
        → LoggingBehaviour
        → PerformanceBehaviour
        → AuthorizationBehaviour
        → Handler → Response
```

### Authentication Flow

```
1. POST /api/v1/auth/login
   → Argon2id verify password
   → Issue JWT (RS256, 15 min) + Refresh Token (SHA-256 hashed, 7 days)
   → Store refresh token hash in DB

2. POST /api/v1/auth/refresh
   → Validate refresh token
   → Rotate: issue new JWT + new Refresh Token
   → Invalidate old refresh token

3. POST /api/v1/auth/logout
   → Revoke refresh token in DB
```

---

## 5. Frontend Architecture

### Structure

```
src/frontend/src/
├── api/
│   └── apiClient.ts          → Axios instance: JWT injection + silent refresh
├── features/                 → Feature-sliced modules (co-located logic)
│   ├── auth/
│   │   ├── hooks/            → useAuth.ts (mutations), authApi.ts (raw calls)
│   │   └── store/            → authStore.ts (Zustand: user + refreshToken)
│   ├── jobs/
│   ├── applications/
│   ├── pipeline/
│   ├── candidates/
│   ├── companies/
│   ├── billing/
│   ├── notifications/
│   └── ...
├── pages/
│   ├── public/               → LandingPage, JobSearchPage, JobDetailPage
│   ├── auth/                 → LoginPage, RegisterPage,
│   │                           ForgotPasswordPage, ResetPasswordPage
│   ├── jobseeker/            → Dashboard, Profile, Applications, SavedJobs
│   ├── recruiter/            → Dashboard, PostJob, Pipeline, Candidates
│   ├── company/              → CompanyProfile
│   ├── admin/                → TenantManagement, Settings
│   ├── billing/              → SubscriptionPage
│   ├── interviews/
│   ├── offers/
│   └── notifications/
├── components/
│   ├── ui/                   → Button, FormTextField, FormSelect,
│   │                           FormCheckbox, LoadingSpinner,
│   │                           ErrorAlert, ConfirmDialog
│   └── layout/               → TopNav, MainLayout, AuthLayout
├── router/
│   └── index.tsx             → React Router v7: RequireAuth, RequireGuest guards
├── theme/
│   └── index.ts              → lightTheme / darkTheme (MUI v9 tokens)
├── types/
│   └── index.ts              → All shared TypeScript types / interfaces
├── hooks/                    → Shared custom hooks
├── store/                    → Global Zustand stores
├── utils/                    → Helper functions
└── lib/                      → Third-party wrappers
```

### State Management Strategy

| Concern | Tool |
|---|---|
| Server state (API data, caching, refetch) | TanStack Query v5 |
| Auth state (user, tokens) | Zustand (persisted to localStorage) |
| UI / ephemeral state | React local state |

### Routing & Guards

```
Public routes   → /jobs, /companies, /login, /register
RequireGuest    → Redirects authenticated users away from /login, /register
RequireAuth     → Redirects unauthenticated users to /login
  Role guards   → /admin/* (Super Admin), /recruiter/* (Recruiter/Employer)
```

### API Client (Axios)

- Attaches `Authorization: Bearer <token>` to every request
- On 401 response → silently calls `/auth/refresh` → retries original request
- On refresh failure → clears auth store → redirects to login

---

## 6. Database Architecture

### Overview

PostgreSQL 16 with **5 dedicated schemas** for clean separation of concerns.

### Schemas

| Schema | Purpose | Key Tables |
|---|---|---|
| `auth` | Identity, access control, sessions | `Tenants`, `Users`, `Roles`, `Permissions`, `RolePermissions`, `UserRoles`, `RefreshTokens`, `UserSessions`, `AuditLogs`, `PasswordResetTokens`, `EmailVerifications`, `TwoFactorSettings` |
| `master` | Reference / lookup data | `Countries`, `States`, `Cities`, `Industries`, `JobCategories`, `JobSubCategories`, `Skills`, `JobTypes`, `ExperienceLevels`, `EducationLevels`, `LanguageMaster` |
| `portal` | Core job portal data | `Companies`, `JobPostings`, `JobSeekerProfiles`, `WorkExperiences`, `Educations`, `JobApplications`, `SavedJobs`, `JobAlerts`, `CandidatePipelines`, `PipelineStages`, `InterviewSchedules`, `OfferLetters`, `Notifications` |
| `billing` | Subscriptions & payments | `TenantSubscriptions`, `SubscriptionInvoices`, `PaymentTransactions`, `JobCredits`, `ResumeViewCredits` |
| `config` | Platform & tenant config | `TenantSettings`, `GlobalSettings`, `EmailTemplates`, `FeatureFlags`, `Announcements`, `ContentPages` |

### Multi-Tenancy

All tenant-scoped tables have a `tenant_id UUID NOT NULL` column.  
**Row-Level Security (RLS)** is enabled on every portal/billing/config table:

```sql
-- Example: JobPostings RLS policy
CREATE POLICY tenant_isolation ON portal.JobPostings
  USING (tenant_id = current_setting('app.current_tenant_id')::uuid);
```

The API sets `SET LOCAL app.current_tenant_id = '<id>'` at the start of each request via EF Core interceptors.

### Indexing Strategy

- B-tree indexes on all foreign keys and common filter columns
- Full-text search indexes (`tsvector`) on `JobPostings` (title, description, skills)
- Composite indexes for pagination queries (e.g., `(tenant_id, status, created_at)`)
- Partial indexes on active/non-deleted rows

### Schema Conventions

- Primary keys: `UUID` (generated as `gen_random_uuid()`)
- Audit columns: `created_at TIMESTAMPTZ`, `updated_at TIMESTAMPTZ`, `created_by UUID`, `updated_by UUID`
- Soft-delete: `deleted_at TIMESTAMPTZ` on applicable tables
- Naming: `PascalCase` tables, `snake_case` columns

---

## 7. Infrastructure & DevOps

### Docker Compose Services

| Service | Image | Port |
|---|---|---|
| `db` | Custom PostgreSQL 16 (with init scripts) | 5433 |
| `redis` | redis:7-alpine | 6380 |
| `api` | .NET 10 API (multi-stage build) | 5000 |
| `frontend` | React/Nginx (multi-stage build) | 3000 |
| `mailhog` | mailhog/mailhog (dev profile only) | 8025 (UI) |

### Starting the Stack

```bash
# 1. Generate secrets (JWT RSA keys + pepper)
cd tools/gen-secrets
dotnet run

# 2. Copy and fill environment file
cp .env.example .env   # set POSTGRES_PASSWORD, REDIS_PASSWORD, etc.

# 3. Start all services (dev — includes MailHog)
docker compose --profile dev up -d --build

# 4. Check MailHog web UI at http://localhost:8025
```

### Environment Variables

| Variable | Description |
|---|---|
| `POSTGRES_DB` | Database name (default: `jobportal`) |
| `POSTGRES_USER` | DB user (default: `jpuser`) |
| `POSTGRES_PASSWORD` | DB password (**required**) |
| `REDIS_PASSWORD` | Redis auth password (**required**) |
| `JWT_ISSUER` | JWT issuer URL |
| `JWT_AUDIENCE` | JWT audience URL |
| `SMTP_HOST` | SMTP server host |
| `SMTP_PORT` | SMTP server port |

### Secret Generation Tool

`tools/gen-secrets` is a .NET utility that generates:
- RSA 2048-bit key pair for JWT RS256 signing
- Argon2id pepper value for password hashing

Output is written to `src/backend/src/JobPortal.API/appsettings.Secrets.json` (git-ignored, mounted as Docker volume).

---

## 8. Security

### Authentication & Authorization

- **JWT RS256** — asymmetric signing; private key never leaves the API server
- **Refresh Token Rotation** — every refresh issues a new token and revokes the old one
- **Argon2id** password hashing with server-side pepper
- **Two-Factor Authentication** (TOTP) support
- **Role-Based Access Control (RBAC)** — Roles + Permissions + UserRoles (DB-driven)

### OWASP Top 10 Mitigations

| Risk | Mitigation |
|---|---|
| Broken Access Control | RLS policies, JWT claims validation, role guards |
| Cryptographic Failures | RS256 JWT, Argon2id, HTTPS enforced |
| Injection | EF Core parameterised queries, FluentValidation input validation |
| Insecure Design | Clean Architecture separates concerns; domain invariants enforced |
| Security Misconfiguration | Secrets never in image; env vars; separate appsettings.Secrets.json |
| Vulnerable Components | Nuget + npm audits, pinned versions |
| Authentication Failures | Token rotation, revocation, rate limiting (Redis) |
| Integrity Failures | RSA-signed JWTs; Docker image build from source |
| Logging / Monitoring | Serilog structured logs, AuditLogs table, request logging middleware |
| SSRF | No user-controlled HTTP destinations |

### API Security

- **CORS** — allowlist-based (configured per environment)
- **Rate Limiting** — Redis-backed sliding window
- **Input Validation** — FluentValidation on all commands
- **Exception Handling** — Global middleware returns RFC 7807 problem details (no stack traces in production)

---

## 9. Modules & Features

### Public Portal

- Landing page with hero job search, featured jobs, top companies
- Job search with full-text search + filters (location, salary, type, experience, industry)
- SEO-friendly job URLs
- Company profiles and listings

### Job Seeker Portal

- Profile: personal info, headline, work experience, education, skills, certifications, languages
- Resume upload (PDF/DOC) and built-in resume builder
- One-click apply with custom cover letter
- Application tracker (Kanban or list view) with status timeline
- Saved jobs wishlist
- Job alerts (keyword + filters, email: instant/daily/weekly)
- In-app and push notifications (PWA)

### Employer / Recruiter Portal

- Company profile management (logo, about, culture)
- Job posting management: rich text description, custom application questions, auto-expire, featured/sponsored toggle
- ATS pipeline: per-job Kanban board (New → Screening → Interview → Offer → Hired/Rejected)
- Candidate search (full-text on resume database, subscription-gated)
- Interview scheduling with calendar invites and multi-round feedback
- Offer letter management
- Team management (invite users, assign roles)
- Reports & analytics: time-to-hire, application funnel, source analytics

### Super Admin Panel

- Tenant management (create, suspend, delete tenants)
- Subscription plan management
- Global configuration and settings
- Content management (email templates, announcements, content pages)
- Platform analytics and KPIs
- Audit log viewer

---

## 10. Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (with Compose v2)
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (for running gen-secrets or local dev)
- [Node.js 22+](https://nodejs.org/) (for local frontend dev)

### Quick Start (Docker)

```bash
# Clone the repo
git clone https://github.com/DNVerma88/JOBPORTAL.git
cd JOBPORTAL

# Generate JWT keys and pepper
cd tools/gen-secrets
dotnet run
cd ../..

# Set required environment variables
$env:POSTGRES_PASSWORD="your_strong_password"
$env:REDIS_PASSWORD="your_redis_password"

# Start all services (with MailHog for email testing)
docker compose --profile dev up -d --build
```

| Service | URL |
|---|---|
| Frontend | http://localhost:3000 |
| API | http://localhost:5000 |
| API Docs (Scalar) | http://localhost:5000/scalar |
| MailHog (dev) | http://localhost:8025 |
| PostgreSQL | localhost:5433 |
| Redis | localhost:6380 |

### Local Development (without Docker)

**Backend:**
```bash
cd src/backend
dotnet restore
dotnet build
cd src/JobPortal.API
dotnet run
```

**Frontend:**
```bash
cd src/frontend
npm install
npm run dev
```

---

## 11. Project Structure

```
JobPortal/
├── db/                        ← PostgreSQL init scripts
│   ├── schemas/               → Extensions, schemas, types, functions
│   ├── tables/                → auth/, master/, portal/, billing/, config/
│   ├── indexes/               → Index definitions per schema
│   ├── rls/                   → Row-Level Security policies
│   └── seed/                  → Reference data seeds
├── docs/
│   └── PROJECT_PLAN.md        → Full product requirements & planning
├── src/
│   ├── backend/
│   │   └── src/
│   │       ├── JobPortal.Domain/
│   │       ├── JobPortal.Application/
│   │       ├── JobPortal.Infrastructure/
│   │       └── JobPortal.API/
│   └── frontend/
│       └── src/               → React 19 + Vite application
├── tools/
│   └── gen-secrets/           → Secret generation utility
└── docker-compose.yml
```

---

## 12. Roadmap

| Phase | Status |
|---|---|
| .NET 10 backend — Clean Architecture, all layers, 0 build errors | ✅ Complete |
| PostgreSQL — 100 SQL files: schemas, tables, indexes, RLS, seeds | ✅ Complete |
| Frontend scaffold — Vite + React 19 + MUI v9 + PWA, auth pages | ✅ Complete |
| Job search & listing pages | 🔲 Planned |
| Job detail page | 🔲 Planned |
| Recruiter dashboard (post jobs, manage applications) | 🔲 Planned |
| Job Seeker profile page | 🔲 Planned |
| ATS Kanban board (CandidatePipelines) | 🔲 Planned |
| Real-time notifications (SignalR) | 🔲 Planned |
| Subscription & billing pages | 🔲 Planned |
| Resume builder | 🔲 Planned |
| Interview scheduling | 🔲 Planned |
| Super Admin panel | 🔲 Planned |

---

## License

This project is private and proprietary.

---

*Built with .NET 10, React 19, PostgreSQL 16, and Redis 7.*
