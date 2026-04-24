import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import { apiClient } from '@/api/apiClient'
import type { PagedResult } from '@/types'

// ── Types ──────────────────────────────────────────────────────

export interface Company {
  id: string
  tenantId: string
  name: string
  slug: string
  logoUrl?: string
  coverImageUrl?: string
  websiteUrl?: string
  description?: string
  tagLine?: string
  industryId?: string
  industryName?: string
  /** @deprecated use industryName */
  industry?: string
  companySize?: string
  foundedYear?: number
  headquartersAddress?: string
  /** @deprecated use headquartersAddress */
  headquartersCity?: string
  isVerified: boolean
  isActive: boolean
  linkedInUrl?: string
  twitterUrl?: string
  glasssdoorUrl?: string
  followersCount: number
  activeJobCount: number
  rating?: number
  reviewCount: number
}

export interface CompanyBranch {
  id: string
  companyId: string
  name: string
  cityName: string
  countryName: string
  address?: string
  isHeadquarters: boolean
}

export interface CompanyReview {
  id: string
  companyId: string
  reviewerName: string
  rating: number
  title?: string
  pros?: string
  cons?: string
  isAnonymous: boolean
  createdOn: string
}

export interface InterviewSchedule {
  id: string
  applicationId: string
  candidateName: string
  candidateEmail: string
  jobTitle: string
  scheduledOn: string
  durationMinutes: number
  interviewType: 'Phone' | 'Video' | 'InPerson'
  status: 'Scheduled' | 'Completed' | 'Cancelled' | 'Rescheduled'
  meetingLink?: string
  location?: string
  notes?: string
}

export interface InterviewFeedback {
  id: string
  scheduleId: string
  interviewerId: string
  rating: number
  recommendation: 'StrongHire' | 'Hire' | 'NoHire' | 'StrongNoHire'
  feedback: string
  createdOn: string
}

export interface OfferLetter {
  id: string
  applicationId: string
  candidateName: string
  jobTitle: string
  offeredSalary: number
  currency: string
  startDate: string
  expiryDate: string
  status: 'Draft' | 'Sent' | 'Accepted' | 'Declined' | 'Expired'
  createdOn: string
}

export interface CandidatePipelineStage {
  id: string
  name: string
  order: number
  color: string
  candidateCount: number
}

export interface PipelineCandidate {
  id: string
  candidateName: string
  candidateEmail: string
  jobTitle: string
  appliedOn: string
  stageId: string
  avatarUrl?: string
}

export interface JobAlert {
  id: string
  userId: string
  keyword?: string
  location?: string
  jobType?: string
  workMode?: string
  experienceLevel?: string
  frequency: 'Daily' | 'Weekly' | 'Instant'
  isActive: boolean
  createdOn: string
}

// ── Query keys ─────────────────────────────────────────────────

const KEYS = {
  companies: (params?: Record<string, unknown>) => ['companies', params] as const,
  company: (id: string) => ['companies', id] as const,
  myCompany: () => ['companies', 'mine'] as const,
  branches: (companyId: string) => ['companies', companyId, 'branches'] as const,
  reviews: (companyId: string) => ['companies', companyId, 'reviews'] as const,
  interviews: () => ['recruiter', 'interviews'] as const,
  offerLetters: () => ['recruiter', 'offers'] as const,
  pipeline: () => ['recruiter', 'pipeline'] as const,
  jobAlerts: () => ['job-alerts'] as const,
}

// ── Company hooks ──────────────────────────────────────────────

export function useCompanies(params?: Record<string, unknown>) {
  return useQuery({
    queryKey: KEYS.companies(params),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<Company>>('/companies', { params })
      return data
    },
    placeholderData: (prev) => prev,
  })
}

export function useMyCompany() {
  return useQuery({
    queryKey: KEYS.myCompany(),
    queryFn: async () => {
      const { data } = await apiClient.get<Company>('/companies/mine')
      return data
    },
  })
}

export function useCompanyById(id: string) {
  return useQuery({
    queryKey: KEYS.company(id),
    queryFn: async () => {
      const { data } = await apiClient.get<Company>(`/companies/${id}`)
      return data
    },
    enabled: !!id,
  })
}

export function useUpdateCompany() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: Partial<Company>) => apiClient.put('/companies/mine', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.myCompany() })
      enqueueSnackbar('Company profile updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update company', { variant: 'error' }),
  })
}

export function useCompanyBranches(companyId: string) {
  return useQuery({
    queryKey: KEYS.branches(companyId),
    queryFn: async () => {
      const { data } = await apiClient.get<CompanyBranch[]>(`/companies/${companyId}/branches`)
      return data
    },
    enabled: !!companyId,
  })
}

export function useCompanyReviews(companyId: string) {
  return useQuery({
    queryKey: KEYS.reviews(companyId),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<CompanyReview>>(`/companies/${companyId}/reviews`)
      return data
    },
    enabled: !!companyId,
  })
}

// ── Interview hooks ────────────────────────────────────────────

export function useInterviews() {
  return useQuery({
    queryKey: KEYS.interviews(),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<InterviewSchedule>>('/interviews')
      return data
    },
  })
}

export function useScheduleInterview() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: Omit<InterviewSchedule, 'id' | 'candidateName' | 'candidateEmail' | 'jobTitle' | 'status'>) =>
      apiClient.post('/interviews', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.interviews() })
      enqueueSnackbar('Interview scheduled', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to schedule interview', { variant: 'error' }),
  })
}

export function useUpdateInterviewStatus() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, status }: { id: string; status: InterviewSchedule['status'] }) =>
      apiClient.patch(`/interviews/${id}/status`, { status }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.interviews() })
      enqueueSnackbar('Interview updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update interview', { variant: 'error' }),
  })
}

// ── Offer letter hooks ─────────────────────────────────────────

export function useOfferLetters() {
  return useQuery({
    queryKey: KEYS.offerLetters(),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<OfferLetter>>('/offers')
      return data
    },
  })
}

export function useCreateOffer() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: Omit<OfferLetter, 'id' | 'candidateName' | 'jobTitle' | 'status' | 'createdOn'>) =>
      apiClient.post('/offers', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.offerLetters() })
      enqueueSnackbar('Offer letter sent', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to create offer', { variant: 'error' }),
  })
}

// ── Pipeline hooks ─────────────────────────────────────────────

export function useHiringPipeline() {
  return useQuery({
    queryKey: KEYS.pipeline(),
    queryFn: async () => {
      const { data } = await apiClient.get<{ stages: CandidatePipelineStage[]; candidates: PipelineCandidate[] }>('/pipeline')
      return data
    },
  })
}

export function useMoveCandidateStage() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ candidateId, stageId }: { candidateId: string; stageId: string }) =>
      apiClient.patch(`/pipeline/candidates/${candidateId}/stage`, { stageId }),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.pipeline() }),
  })
}

// ── Job alert hooks ────────────────────────────────────────────

export function useJobAlerts() {
  return useQuery({
    queryKey: KEYS.jobAlerts(),
    queryFn: async () => {
      const { data } = await apiClient.get<JobAlert[]>('/job-alerts')
      return data
    },
  })
}

export function useCreateJobAlert() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: Omit<JobAlert, 'id' | 'userId' | 'createdOn'>) => apiClient.post('/job-alerts', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.jobAlerts() })
      enqueueSnackbar('Job alert created', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to create alert', { variant: 'error' }),
  })
}

export function useDeleteJobAlert() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`/job-alerts/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.jobAlerts() })
      enqueueSnackbar('Alert removed', { variant: 'info' })
    },
    onError: () => enqueueSnackbar('Failed to remove alert', { variant: 'error' }),
  })
}

export function useToggleJobAlert() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, isActive }: { id: string; isActive: boolean }) =>
      apiClient.patch(`/job-alerts/${id}/toggle`, { isActive }),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.jobAlerts() }),
  })
}
