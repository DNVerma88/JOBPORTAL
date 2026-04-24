import type { ApplicationStatus, JobStatus, JobType, WorkMode, ExperienceLevel } from '@/types'

export const APP_STATUS_LABELS: Record<ApplicationStatus, string> = {
  Applied: 'Applied',
  UnderReview: 'Under Review',
  Shortlisted: 'Shortlisted',
  Assessment: 'Assessment',
  InterviewScheduled: 'Interview Scheduled',
  OfferExtended: 'Offer Extended',
  Hired: 'Hired',
  Rejected: 'Rejected',
  Withdrawn: 'Withdrawn',
}

export const APP_STATUS_COLORS: Record<ApplicationStatus, 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning'> = {
  Applied: 'default',
  UnderReview: 'info',
  Shortlisted: 'primary',
  Assessment: 'secondary',
  InterviewScheduled: 'warning',
  OfferExtended: 'success',
  Hired: 'success',
  Rejected: 'error',
  Withdrawn: 'default',
}

export const JOB_STATUS_LABELS: Record<JobStatus, string> = {
  Draft: 'Draft',
  Published: 'Published',
  Paused: 'Paused',
  Closed: 'Closed',
  Expired: 'Expired',
}

export const JOB_STATUS_COLORS: Record<JobStatus, 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning'> = {
  Draft: 'default',
  Published: 'success',
  Paused: 'warning',
  Closed: 'default',
  Expired: 'error',
}

export const JOB_TYPE_LABELS: Record<JobType, string> = {
  FullTime: 'Full-time',
  PartTime: 'Part-time',
  Contract: 'Contract',
  Internship: 'Internship',
  Freelance: 'Freelance',
}

export const WORK_MODE_LABELS: Record<WorkMode, string> = {
  Remote: 'Remote',
  OnSite: 'On-site',
  Hybrid: 'Hybrid',
}

export const EXP_LEVEL_LABELS: Record<ExperienceLevel, string> = {
  Fresher: 'Fresher (0–1 yr)',
  Junior: 'Junior (1–3 yrs)',
  Mid: 'Mid (3–5 yrs)',
  Senior: 'Senior (5–8 yrs)',
  Lead: 'Lead (8–12 yrs)',
  Executive: 'Executive (12+ yrs)',
}

export const JOB_TYPE_OPTIONS = Object.entries(JOB_TYPE_LABELS).map(([v, l]) => ({ value: v, label: l }))
export const WORK_MODE_OPTIONS = Object.entries(WORK_MODE_LABELS).map(([v, l]) => ({ value: v, label: l }))
export const EXP_LEVEL_OPTIONS = Object.entries(EXP_LEVEL_LABELS).map(([v, l]) => ({ value: v, label: l }))

export const ROLES = {
  SUPER_ADMIN: 'SuperAdmin',
  TENANT_ADMIN: 'TenantAdmin',
  RECRUITER: 'Recruiter',
  HIRING_MANAGER: 'HiringManager',
  JOB_SEEKER: 'JobSeeker',
} as const

export const PAGE_SIZE_OPTIONS = [10, 25, 50, 100]
export const DEFAULT_PAGE_SIZE = 25
