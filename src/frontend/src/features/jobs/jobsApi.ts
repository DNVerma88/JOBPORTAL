import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import { apiClient } from '@/api/apiClient'
import type { PagedResult, JobSummary, JobSearchParams, JobStatus } from '@/types'

// ── Types ──────────────────────────────────────────────────────

export interface JobDetail extends JobSummary {
  description: string
  requirements: string
  responsibilities?: string
  benefits?: string
  companyDescription?: string
  companyWebsite?: string
  companySize?: string
  companyIndustry?: string
  categoryName: string
}

export interface CreateJobRequest {
  title: string
  description: string
  requirements: string
  responsibilities?: string
  benefits?: string
  categoryId: string
  jobType: string
  workMode: string
  experienceLevel: string
  location: string
  cityId?: string
  salaryMin?: number
  salaryMax?: number
  currency?: string
  isSalaryHidden?: boolean
  skills: string[]
  expiresAt: string
}

// ── Query keys ─────────────────────────────────────────────────

const KEYS = {
  list: (params: JobSearchParams) => ['jobs', params] as const,
  detail: (id: string) => ['jobs', id] as const,
  myJobs: () => ['my-jobs'] as const,
}

// ── Hooks ──────────────────────────────────────────────────────

export function useJobs(params: JobSearchParams) {
  return useQuery({
    queryKey: KEYS.list(params),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<JobSummary>>('/jobs', { params })
      return data
    },
    placeholderData: (prev) => prev,
  })
}

export function useJob(id: string) {
  return useQuery({
    queryKey: KEYS.detail(id),
    queryFn: async () => {
      const { data } = await apiClient.get<JobDetail>(`/jobs/${id}`)
      return data
    },
    enabled: !!id,
  })
}

export function useMyJobs() {
  return useQuery({
    queryKey: KEYS.myJobs(),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<JobSummary>>('/jobs/my')
      return data
    },
  })
}

export function useCreateJob() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: CreateJobRequest) => apiClient.post<{ id: string }>('/jobs', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.myJobs() })
      enqueueSnackbar('Job posted successfully', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to post job', { variant: 'error' }),
  })
}

export function useUpdateJobStatus() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, status }: { id: string; status: JobStatus }) =>
      apiClient.patch(`/jobs/${id}/status`, { status }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.myJobs() })
      enqueueSnackbar('Job status updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update status', { variant: 'error' }),
  })
}

export function useSaveJob() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, saved }: { id: string; saved: boolean }) =>
      saved ? apiClient.delete(`/jobs/${id}/save`) : apiClient.post(`/jobs/${id}/save`, {}),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['jobs'] })
      qc.invalidateQueries({ queryKey: ['saved-jobs'] })
    },
  })
}

export function useSavedJobs() {
  return useQuery({
    queryKey: ['saved-jobs'],
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<JobSummary>>('/jobs/saved')
      return data
    },
  })
}

export function useDeleteJob() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`/jobs/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['jobs'] })
      enqueueSnackbar('Job deleted', { variant: 'info' })
    },
    onError: () => enqueueSnackbar('Failed to delete job', { variant: 'error' }),
  })
}
