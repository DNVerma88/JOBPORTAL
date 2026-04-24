// ── Pagination ────────────────────────────────────────────────
export interface PagedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface ApiResponse<T> {
  data: T
  message?: string
  success: boolean
}

// ── Auth ──────────────────────────────────────────────────────
export interface LoginRequest {
  email: string
  password: string
  rememberMe?: boolean
}

export interface RegisterRequest {
  firstName: string
  lastName: string
  email: string
  password: string
  confirmPassword: string
  role: 'JobSeeker' | 'Recruiter'
  tenantSlug?: string   // required for Recruiter
  companyName?: string  // required for Recruiter
}

export interface AuthTokens {
  accessToken: string
  refreshToken: string
  expiresIn: number
}

export interface UserProfile {
  id: string
  firstName: string
  lastName: string
  email: string
  role: string
  tenantId: string
  avatarUrl?: string
}

// ── Jobs ──────────────────────────────────────────────────────
export type JobType = 'FullTime' | 'PartTime' | 'Contract' | 'Internship' | 'Freelance'
export type WorkMode = 'Remote' | 'OnSite' | 'Hybrid'
export type ExperienceLevel = 'Fresher' | 'Junior' | 'Mid' | 'Senior' | 'Lead' | 'Executive'
export type JobStatus = 'Draft' | 'Published' | 'Paused' | 'Closed' | 'Expired'

export interface JobSummary {
  id: string
  title: string
  companyName: string
  companyLogoUrl?: string
  location: string
  cityName: string
  jobType: JobType
  workMode: WorkMode
  experienceLevel: ExperienceLevel
  salaryMin?: number
  salaryMax?: number
  currency: string
  skills: string[]
  postedAt: string
  expiresAt: string
  applicationsCount: number
  openingsCount?: number
  isSaved: boolean
  // Recruiter-facing additional fields (present when calling /jobs/my)
  status?: JobStatus
  slug?: string
  companyId?: string
  isUrgent?: boolean
  isFeatured?: boolean
  isRemote?: boolean
  publishedAt?: string
  createdOn?: string
}

export interface JobSearchParams {
  keyword?: string
  location?: string
  cityId?: string
  jobType?: JobType
  workMode?: WorkMode
  experienceLevel?: ExperienceLevel
  categoryId?: string
  industryId?: string
  salaryMin?: number
  salaryMax?: number
  skills?: string[]
  pageNumber?: number
  pageSize?: number
  sortBy?: 'Relevance' | 'DatePosted' | 'Salary'
}

// ── Applications ──────────────────────────────────────────────
export type ApplicationStatus =
  | 'Applied'
  | 'UnderReview'
  | 'Shortlisted'
  | 'Assessment'
  | 'InterviewScheduled'
  | 'OfferExtended'
  | 'Hired'
  | 'Rejected'
  | 'Withdrawn'

export interface ApplicationSummary {
  id: string
  jobTitle: string
  companyName: string
  appliedAt: string
  status: ApplicationStatus
  lastUpdatedAt: string
  applicantName?: string
  applicantId?: string
}

// ── Notifications ─────────────────────────────────────────────
export type NotificationChannel = 'InApp' | 'Email' | 'Push' | 'SMS'

export interface Notification {
  id: string
  type: string
  title: string
  body: string
  channel: NotificationChannel
  isRead: boolean
  createdAt: string
  data?: Record<string, unknown>
}
