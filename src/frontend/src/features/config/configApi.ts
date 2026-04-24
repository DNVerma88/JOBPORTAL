import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import { apiClient } from '@/api/apiClient'

// ── Types ──────────────────────────────────────────────────────

export interface GlobalSetting {
  key: string
  value: string
  description?: string
  group: string
  isPublic: boolean
}

export interface EmailTemplate {
  id: string
  name: string
  subject: string
  body: string
  variables?: string
  isActive: boolean
}

export interface FeatureFlag {
  id: string
  name: string
  key: string
  description?: string
  isEnabled: boolean
  tenantId?: string
}

export interface Announcement {
  id: string
  title: string
  content: string
  type: 'Info' | 'Warning' | 'Critical'
  targetRole?: string
  isActive: boolean
  startsOn?: string
  endsOn?: string
  createdOn: string
}

export interface MasterSkill {
  id: string
  name: string
  category?: string
  isActive: boolean
}

export interface JobCategory {
  id: string
  name: string
  slug: string
  iconUrl?: string
  jobCount?: number
  isActive: boolean
}

export interface Industry {
  id: string
  name: string
  isActive: boolean
}

// ── Query keys ─────────────────────────────────────────────────

const KEYS = {
  settings: () => ['config', 'settings'] as const,
  emailTemplates: () => ['config', 'email-templates'] as const,
  featureFlags: () => ['config', 'feature-flags'] as const,
  announcements: () => ['config', 'announcements'] as const,
  skills: () => ['master', 'skills'] as const,
  categories: () => ['master', 'categories'] as const,
  industries: () => ['master', 'industries'] as const,
}

// ── Settings hooks ─────────────────────────────────────────────

export function useGlobalSettings() {
  return useQuery({
    queryKey: KEYS.settings(),
    queryFn: async () => {
      const { data } = await apiClient.get<any>('/admin/settings')
      // Backend may return PagedList or plain array
      if (Array.isArray(data)) return data as GlobalSetting[]
      if (data && Array.isArray(data.items)) return data.items as GlobalSetting[]
      return [] as GlobalSetting[]
    },
  })
}

export function useUpdateSetting() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ key, value }: { key: string; value: string }) =>
      apiClient.put(`/admin/settings/${key}`, { value }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.settings() })
      enqueueSnackbar('Setting saved', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to save setting', { variant: 'error' }),
  })
}

// ── Email template hooks ───────────────────────────────────────

export function useEmailTemplates() {
  return useQuery({
    queryKey: KEYS.emailTemplates(),
    queryFn: async () => {
      const { data } = await apiClient.get<any>('/admin/email-templates')
      if (Array.isArray(data)) return data as EmailTemplate[]
      if (data && Array.isArray(data.items)) return data.items as EmailTemplate[]
      return [] as EmailTemplate[]
    },
  })
}

export function useUpdateEmailTemplate() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, ...body }: Partial<EmailTemplate> & { id: string }) =>
      apiClient.put(`/admin/email-templates/${id}`, body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.emailTemplates() })
      enqueueSnackbar('Template updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update template', { variant: 'error' }),
  })
}

// ── Feature flag hooks ─────────────────────────────────────────

export function useFeatureFlags() {
  return useQuery({
    queryKey: KEYS.featureFlags(),
    queryFn: async () => {
      const { data } = await apiClient.get<any>('/admin/feature-flags')
      if (Array.isArray(data)) return data as FeatureFlag[]
      if (data && Array.isArray(data.items)) return data.items as FeatureFlag[]
      return [] as FeatureFlag[]
    },
  })
}

export function useToggleFeatureFlag() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, isEnabled }: { id: string; isEnabled: boolean }) =>
      apiClient.patch(`/admin/feature-flags/${id}`, { isEnabled }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.featureFlags() })
      enqueueSnackbar('Feature flag updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update feature flag', { variant: 'error' }),
  })
}

// ── Announcement hooks ─────────────────────────────────────────

export function useAnnouncements() {
  return useQuery({
    queryKey: KEYS.announcements(),
    queryFn: async () => {
      const { data } = await apiClient.get<any>('/admin/announcements')
      if (Array.isArray(data)) return data as Announcement[]
      if (data && Array.isArray(data.items)) return data.items as Announcement[]
      return [] as Announcement[]
    },
  })
}

export function useCreateAnnouncement() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: Omit<Announcement, 'id' | 'createdOn'>) => apiClient.post('/admin/announcements', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.announcements() })
      enqueueSnackbar('Announcement created', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to create announcement', { variant: 'error' }),
  })
}

export function useDeleteAnnouncement() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`/admin/announcements/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.announcements() })
      enqueueSnackbar('Announcement deleted', { variant: 'info' })
    },
    onError: () => enqueueSnackbar('Failed to delete announcement', { variant: 'error' }),
  })
}

// ── Master data hooks ──────────────────────────────────────────

export function useSkills() {
  return useQuery({
    queryKey: KEYS.skills(),
    queryFn: async () => {
      const { data } = await apiClient.get<MasterSkill[]>('/master/skills')
      return data
    },
  })
}

export function useJobCategories() {
  return useQuery({
    queryKey: KEYS.categories(),
    queryFn: async () => {
      const { data } = await apiClient.get<JobCategory[]>('/master/categories')
      return data
    },
  })
}

export function useIndustries() {
  return useQuery({
    queryKey: KEYS.industries(),
    queryFn: async () => {
      const { data } = await apiClient.get<Industry[]>('/master/industries')
      return data
    },
  })
}

export function useCreateMasterItem() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ endpoint, body }: { endpoint: string; body: Record<string, unknown> }) =>
      apiClient.post(endpoint, body),
    onSuccess: (_, { endpoint }) => {
      qc.invalidateQueries({ queryKey: ['master'] })
      const label = endpoint.includes('skill') ? 'Skill' : endpoint.includes('categor') ? 'Category' : 'Item'
      enqueueSnackbar(`${label} created`, { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to create item', { variant: 'error' }),
  })
}

export function useToggleMasterItem() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ endpoint, id, isActive }: { endpoint: string; id: string; isActive: boolean }) =>
      apiClient.patch(`${endpoint}/${id}`, { isActive }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['master'] })
      enqueueSnackbar('Status updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update item', { variant: 'error' }),
  })
}
