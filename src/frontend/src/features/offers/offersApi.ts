import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import { apiClient } from '@/api/apiClient'
import type { PagedResult } from '@/types'

// ── Types ──────────────────────────────────────────────────────

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

// ── Query keys ─────────────────────────────────────────────────

const KEYS = {
  offers: (params?: Record<string, unknown>) => ['recruiter', 'offers', params] as const,
}

// ── Hooks ──────────────────────────────────────────────────────

export function useOfferLetters(params?: Record<string, unknown>) {
  return useQuery({
    queryKey: KEYS.offers(params),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<OfferLetter>>('/offers', { params })
      return data
    },
    placeholderData: (prev) => prev,
  })
}

export function useCreateOffer() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: Omit<OfferLetter, 'id' | 'candidateName' | 'jobTitle' | 'status' | 'createdOn'>) =>
      apiClient.post('/offers', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['recruiter', 'offers'] })
      enqueueSnackbar('Offer letter sent', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to create offer', { variant: 'error' }),
  })
}

export function useRespondToOffer() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, response, message }: { id: string; response: 'Accepted' | 'Declined'; message?: string }) =>
      apiClient.patch(`/offers/${id}/respond`, { response, message }),
    onSuccess: (_, { response }) => {
      qc.invalidateQueries({ queryKey: ['recruiter', 'offers'] })
      enqueueSnackbar(`Offer ${response.toLowerCase()} successfully`, { variant: response === 'Accepted' ? 'success' : 'info' })
    },
    onError: () => enqueueSnackbar('Failed to respond to offer', { variant: 'error' }),
  })
}

export function useRevokeOffer() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, reason }: { id: string; reason?: string }) =>
      apiClient.patch(`/offers/${id}/revoke`, { reason }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['recruiter', 'offers'] })
      enqueueSnackbar('Offer revoked', { variant: 'warning' })
    },
    onError: () => enqueueSnackbar('Failed to revoke offer', { variant: 'error' }),
  })
}

export function useOfferById(id: string) {
  return useQuery({
    queryKey: ['recruiter', 'offers', id],
    queryFn: async () => {
      const { data } = await apiClient.get(`/offers/${id}`)
      return data
    },
    enabled: !!id,
  })
}

export function useUpdateOffer() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, ...body }: { id: string; [key: string]: unknown }) =>
      apiClient.put(`/offers/${id}`, body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['recruiter', 'offers'] })
      enqueueSnackbar('Offer updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update offer', { variant: 'error' }),
  })
}
