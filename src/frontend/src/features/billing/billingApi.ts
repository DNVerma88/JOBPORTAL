import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import { apiClient } from '@/api/apiClient'
import type { PagedResult } from '@/types'

// ── Types ──────────────────────────────────────────────────────

export interface SubscriptionPlan {
  id: string
  name: string
  description?: string
  monthlyPrice: number
  annualPrice: number
  currency: string
  maxJobPostings: number
  maxUsers: number
  maxResumeViews: number
  isActive: boolean
  isPopular?: boolean
  features: SubscriptionFeature[]
}

export interface SubscriptionFeature {
  id: string
  planId: string
  featureName: string
  isIncluded: boolean
  limit?: number
}

export interface TenantSubscription {
  id: string
  tenantId: string
  planId: string
  planName: string
  status: 'Active' | 'Trial' | 'Expired' | 'Cancelled' | 'PastDue'
  startDate: string
  endDate?: string
  trialEndsOn?: string
  billingCycle: 'Monthly' | 'Annual'
  amount: number
  currency: string
  autoRenew: boolean
}

export interface Invoice {
  id: string
  subscriptionId: string
  invoiceNumber: string
  billingPeriodStart: string
  billingPeriodEnd: string
  amount: number
  taxAmount: number
  totalAmount: number
  currency: string
  currencyCode: string
  status: 'Paid' | 'Pending' | 'Failed' | 'Voided' | 'Completed'
  dueDate: string
  paidAt?: string
  externalInvoiceId?: string
  invoiceFileUrl?: string
}

export interface PaymentTransaction {
  id: string
  invoiceId?: string
  amount: number
  currency: string
  method: string
  status: 'Success' | 'Failed' | 'Pending' | 'Refunded'
  transactionRef?: string
  createdOn: string
}

export interface JobCredit {
  id: string
  tenantId: string
  totalCredits: number
  usedCredits: number
  availableCredits: number
  expiresOn?: string
}

// ── Query keys ─────────────────────────────────────────────────

const KEYS = {
  plans: () => ['billing', 'plans'] as const,
  subscription: () => ['billing', 'subscription'] as const,
  invoices: () => ['billing', 'invoices'] as const,
  transactions: () => ['billing', 'transactions'] as const,
  credits: () => ['billing', 'credits'] as const,
}

// ── Hooks ──────────────────────────────────────────────────────

export function useSubscriptionPlans() {
  return useQuery({
    queryKey: KEYS.plans(),
    queryFn: async () => {
      const { data } = await apiClient.get<SubscriptionPlan[]>('/master/subscription-plans')
      return data
    },
  })
}

export function useCurrentSubscription() {
  return useQuery({
    queryKey: KEYS.subscription(),
    queryFn: async () => {
      const { data } = await apiClient.get<TenantSubscription>('/billing/subscription')
      return data
    },
  })
}

export function useInvoices() {
  return useQuery({
    queryKey: KEYS.invoices(),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<Invoice>>('/billing/invoices')
      return data
    },
  })
}

export function usePaymentTransactions() {
  return useQuery({
    queryKey: KEYS.transactions(),
    queryFn: async () => {
      const { data } = await apiClient.get<{ items: PaymentTransaction[] }>('/billing/transactions')
      return data?.items ?? []
    },
  })
}

export function useJobCredits() {
  return useQuery({
    queryKey: KEYS.credits(),
    queryFn: async () => {
      const { data } = await apiClient.get<JobCredit[]>('/billing/credits')
      return data
    },
  })
}

export function useChangePlan() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ planId, billingCycle }: { planId: string; billingCycle: 'Monthly' | 'Annual' }) =>
      apiClient.post('/billing/subscription/change', { planId, billingCycle }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.subscription() })
      enqueueSnackbar('Plan updated successfully', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to change plan', { variant: 'error' }),
  })
}

export function useCancelSubscription() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: () => apiClient.post('/billing/subscription/cancel', {}),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.subscription() })
      enqueueSnackbar('Subscription cancelled', { variant: 'info' })
    },
    onError: () => enqueueSnackbar('Failed to cancel subscription', { variant: 'error' }),
  })
}

export function useAdminSubscriptionPlans() {
  return useQuery({
    queryKey: ['admin', 'plans'],
    queryFn: async () => {
      const { data } = await apiClient.get<SubscriptionPlan[]>('/master/subscription-plans')
      return data
    },
  })
}

export function useCreatePlan() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: Partial<SubscriptionPlan>) => apiClient.post('/admin/subscription-plans', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin', 'plans'] })
      enqueueSnackbar('Plan created', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to create plan', { variant: 'error' }),
  })
}

export function useUpdatePlan() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, ...body }: Partial<SubscriptionPlan> & { id: string }) =>
      apiClient.put(`/admin/subscription-plans/${id}`, body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin', 'plans'] })
      enqueueSnackbar('Plan updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update plan', { variant: 'error' }),
  })
}

export function useReactivateSubscription() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: () => apiClient.post('/billing/subscription/reactivate', {}),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['billing', 'subscription'] })
      enqueueSnackbar('Subscription reactivated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to reactivate subscription', { variant: 'error' }),
  })
}

export function usePurchaseCredits() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (quantity: number) => apiClient.post('/billing/credits/purchase', { quantity }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['billing', 'credits'] })
      enqueueSnackbar('Credits purchased', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to purchase credits', { variant: 'error' }),
  })
}
