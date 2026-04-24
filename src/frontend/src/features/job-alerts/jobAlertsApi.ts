import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import { apiClient } from '@/api/apiClient'

// ── Types ──────────────────────────────────────────────────────

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
  alerts: () => ['job-alerts'] as const,
}

// ── Hooks ──────────────────────────────────────────────────────

export function useJobAlerts() {
  return useQuery({
    queryKey: KEYS.alerts(),
    queryFn: async () => {
      const { data } = await apiClient.get<{ items: JobAlert[] }>('/job-alerts')
      return data.items ?? []
    },
  })
}

export function useCreateJobAlert() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: Omit<JobAlert, 'id' | 'userId' | 'createdOn'>) =>
      apiClient.post('/job-alerts', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.alerts() })
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
      qc.invalidateQueries({ queryKey: KEYS.alerts() })
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
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.alerts() }),
  })
}

export function useUpdateJobAlert() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, ...body }: { id: string; frequency: string; keyword?: string; jobType?: string; workMode?: string }) =>
      apiClient.put(`/job-alerts/${id}`, body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.alerts() })
      enqueueSnackbar('Alert updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update alert', { variant: 'error' }),
  })
}
