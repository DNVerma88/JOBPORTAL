import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import { apiClient } from '@/api/apiClient'
import type { PagedResult, ApplicationSummary, ApplicationStatus } from '@/types'

// ── Types ──────────────────────────────────────────────────────

export interface ApplicationDetail extends ApplicationSummary {
  coverLetter?: string
  resumeUrl?: string
  notes?: string
  candidateId: string
  candidateName: string
  candidateEmail: string
  candidatePhone?: string
  jobId: string
}

export interface ApplyJobRequest {
  jobId: string
  coverLetter?: string
  resumeId?: string
}

// ── Query keys ─────────────────────────────────────────────────

const KEYS = {
  myApplications: () => ['my-applications'] as const,
  jobApplications: (jobId: string) => ['job-applications', jobId] as const,
  allApplications: (params?: Record<string, unknown>) => ['applications', params] as const,
  detail: (id: string) => ['applications', id] as const,
  stats: () => ['application-stats'] as const,
}

// ── Hooks ──────────────────────────────────────────────────────

export function useMyApplications() {
  return useQuery({
    queryKey: KEYS.myApplications(),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<ApplicationSummary>>('/applications/my')
      return data
    },
  })
}

export function useAllApplications(params?: { pageSize?: number; status?: string }) {
  return useQuery({
    queryKey: KEYS.allApplications(params),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<ApplicationSummary>>('/applications', {
        params: { pageSize: 100, ...params },
      })
      return data
    },
  })
}

export function useJobApplications(jobId: string) {
  return useQuery({
    queryKey: KEYS.jobApplications(jobId),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<ApplicationDetail>>(
        `/jobs/${jobId}/applications`,
      )
      return data
    },
    enabled: !!jobId,
  })
}

export function useApplication(id: string) {
  return useQuery({
    queryKey: KEYS.detail(id),
    queryFn: async () => {
      const { data } = await apiClient.get<ApplicationDetail>(`/applications/${id}`)
      return data
    },
    enabled: !!id,
  })
}

export function useApplyJob() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: ApplyJobRequest) => apiClient.post('/applications', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.myApplications() })
      enqueueSnackbar('Application submitted!', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to apply', { variant: 'error' }),
  })
}

export function useWithdrawApplication() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (id: string) => apiClient.patch(`/applications/${id}/withdraw`, {}),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.myApplications() })
      enqueueSnackbar('Application withdrawn', { variant: 'info' })
    },
    onError: () => enqueueSnackbar('Failed to withdraw', { variant: 'error' }),
  })
}

export function useUpdateApplicationStatus() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, status }: { id: string; status: ApplicationStatus }) =>
      apiClient.patch(`/applications/${id}/status`, { status }),
    onSuccess: (_, { id }) => {
      qc.invalidateQueries({ queryKey: KEYS.detail(id) })
      qc.invalidateQueries({ queryKey: ['job-applications'] })
      enqueueSnackbar('Status updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update status', { variant: 'error' }),
  })
}

export function useApplicationStats() {
  return useQuery({
    queryKey: KEYS.stats(),
    queryFn: async () => {
      const { data } = await apiClient.get<{
        total: number
        underReview: number
        shortlisted: number
        offered: number
      }>('/applications/stats')
      return data
    },
  })
}
