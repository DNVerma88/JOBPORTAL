import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import { apiClient } from '@/api/apiClient'
import type { PagedResult } from '@/types'

// ── Types ──────────────────────────────────────────────────────

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

// ── Query keys ─────────────────────────────────────────────────

const KEYS = {
  interviews: (params?: Record<string, unknown>) => ['recruiter', 'interviews', params] as const,
}

// ── Hooks ──────────────────────────────────────────────────────

export function useInterviews(params?: Record<string, unknown>) {
  return useQuery({
    queryKey: KEYS.interviews(params),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<InterviewSchedule>>('/interviews', { params })
      return data
    },
    placeholderData: (prev) => prev,
  })
}

export function useScheduleInterview() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: Omit<InterviewSchedule, 'id' | 'candidateName' | 'candidateEmail' | 'jobTitle' | 'status'>) =>
      apiClient.post('/interviews', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['recruiter', 'interviews'] })
      enqueueSnackbar('Interview scheduled', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to schedule interview', { variant: 'error' }),
  })
}

export function useUpdateInterviewStatus() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, status, cancelledReason }: { id: string; status: InterviewSchedule['status']; cancelledReason?: string }) =>
      apiClient.patch(`/interviews/${id}/status`, {
        isCancelled: status === 'Cancelled',
        cancelledReason: status === 'Cancelled' ? (cancelledReason ?? null) : null,
      }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['recruiter', 'interviews'] })
      enqueueSnackbar('Interview updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update interview', { variant: 'error' }),
  })
}

export function useRescheduleInterview() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, scheduledOn, durationMinutes, meetingLink, location }: {
      id: string; scheduledOn: string; durationMinutes: number; meetingLink?: string; location?: string
    }) => apiClient.put(`/interviews/${id}`, { scheduledOn, durationMinutes, meetingLink, location }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['recruiter', 'interviews'] })
      enqueueSnackbar('Interview rescheduled', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to reschedule', { variant: 'error' }),
  })
}
