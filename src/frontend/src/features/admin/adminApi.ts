import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import { apiClient } from '@/api/apiClient'
import type { PagedResult } from '@/types'

// ── Types ──────────────────────────────────────────────────────

export interface User {
  id: string
  firstName: string
  lastName: string
  email: string
  role: string
  tenantId: string
  isEmailVerified: boolean
  isActive: boolean
  isSuspended: boolean
  createdOn: string
  lastLoginOn?: string
  profilePictureUrl?: string
}

export interface CreateUserRequest {
  firstName: string
  lastName: string
  email: string
  password: string
  roleId: string
  tenantId?: string
}

export interface UserFilters {
  search?: string
  role?: string
  isActive?: boolean
  pageNumber?: number
  pageSize?: number
}

export interface Tenant {
  id: string
  name: string
  slug: string
  customDomain?: string
  isActive: boolean
  planName?: string
  createdOn: string
  userCount?: number
  jobCount?: number
  trialEndsOn?: string
}

export interface Role {
  id: string
  name: string
  description?: string
  permissionCount: number
}

export interface Permission {
  id: string
  name: string
  description?: string
  module: string
}

export interface AuditLog {
  id: string
  userId: string
  userEmail: string
  action: string
  entityType: string
  entityId?: string
  ipAddress?: string
  userAgent?: string
  createdOn: string
  changes?: string
}

export interface Session {
  id: string
  userId: string
  userEmail: string
  ipAddress?: string
  deviceInfo?: string
  createdOn: string
  lastActivityOn: string
  isActive: boolean
}

// ── Query keys ─────────────────────────────────────────────────

const KEYS = {
  users: (f?: UserFilters) => ['admin', 'users', f] as const,
  user: (id: string) => ['admin', 'users', id] as const,
  tenants: () => ['admin', 'tenants'] as const,
  tenant: (id: string) => ['admin', 'tenants', id] as const,
  roles: () => ['admin', 'roles'] as const,
  permissions: () => ['admin', 'permissions'] as const,
  auditLogs: (f?: Record<string, unknown>) => ['admin', 'audit', f] as const,
  sessions: () => ['admin', 'sessions'] as const,
}

// ── User hooks ─────────────────────────────────────────────────

export function useUsers(filters?: UserFilters) {
  return useQuery({
    queryKey: KEYS.users(filters),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<User>>('/admin/users', { params: filters })
      return data
    },
    placeholderData: (prev) => prev,
  })
}

export function useCreateUser() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: CreateUserRequest) => apiClient.post('/admin/users', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin', 'users'] })
      enqueueSnackbar('User created', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to create user', { variant: 'error' }),
  })
}

export function useToggleUserStatus() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, suspend }: { id: string; suspend: boolean }) =>
      apiClient.patch(`/admin/users/${id}/${suspend ? 'suspend' : 'activate'}`, {}),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin', 'users'] })
      enqueueSnackbar('User status updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update user', { variant: 'error' }),
  })
}

export function useResetUserPassword() {
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (id: string) => apiClient.post(`/admin/users/${id}/reset-password`, {}),
    onSuccess: () => enqueueSnackbar('Password reset email sent', { variant: 'success' }),
    onError: () => enqueueSnackbar('Failed to reset password', { variant: 'error' }),
  })
}

// ── Tenant hooks ───────────────────────────────────────────────

export function useTenants() {
  return useQuery({
    queryKey: KEYS.tenants(),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<Tenant>>('/admin/tenants')
      return data
    },
  })
}

export function useCreateTenant() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: { name: string; slug: string; contactEmail: string }) =>
      apiClient.post('/admin/tenants', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.tenants() })
      enqueueSnackbar('Tenant created', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to create tenant', { variant: 'error' }),
  })
}

export function useToggleTenantStatus() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, active }: { id: string; active: boolean }) =>
      apiClient.patch(`/admin/tenants/${id}/status`, { isActive: active }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.tenants() })
      enqueueSnackbar('Tenant status updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update tenant', { variant: 'error' }),
  })
}

// ── Role hooks ─────────────────────────────────────────────────

export function useRoles() {
  return useQuery({
    queryKey: KEYS.roles(),
    queryFn: async () => {
      const { data } = await apiClient.get<Role[]>('/admin/roles')
      return data
    },
  })
}

export function usePermissions() {
  return useQuery({
    queryKey: KEYS.permissions(),
    queryFn: async () => {
      const { data } = await apiClient.get<Permission[]>('/admin/permissions')
      return data
    },
  })
}

export function useRolePermissions(roleId: string) {
  return useQuery({
    queryKey: ['admin', 'roles', roleId, 'permissions'],
    queryFn: async () => {
      const { data } = await apiClient.get<string[]>(`/admin/roles/${roleId}/permissions`)
      return data
    },
    enabled: !!roleId,
  })
}

export function useUpdateRolePermissions() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ roleId, permissionIds }: { roleId: string; permissionIds: string[] }) =>
      apiClient.put(`/admin/roles/${roleId}/permissions`, { permissionIds }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin', 'roles'] })
      enqueueSnackbar('Permissions updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update permissions', { variant: 'error' }),
  })
}

// ── Audit log hooks ────────────────────────────────────────────

export function useAuditLogs(filters?: Record<string, unknown>) {
  return useQuery({
    queryKey: KEYS.auditLogs(filters),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<AuditLog>>('/admin/audit-logs', { params: filters })
      return data
    },
    placeholderData: (prev) => prev,
  })
}

export function useSessions() {
  return useQuery({
    queryKey: KEYS.sessions(),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<Session>>('/admin/sessions')
      return data
    },
  })
}

export function useRevokeSession() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`/admin/sessions/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.sessions() })
      enqueueSnackbar('Session revoked', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to revoke session', { variant: 'error' }),
  })
}

// ── Role CRUD hooks ─────────────────────────────────────────────────────────────

export function useCreateRole() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: { name: string; description?: string }) => apiClient.post('/admin/roles', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.roles() })
      enqueueSnackbar('Role created', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to create role', { variant: 'error' }),
  })
}

export function useUpdateRole() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, ...body }: { id: string; name: string; description?: string }) =>
      apiClient.put(`/admin/roles/${id}`, body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.roles() })
      enqueueSnackbar('Role updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update role', { variant: 'error' }),
  })
}

export function useDeleteRole() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`/admin/roles/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.roles() })
      enqueueSnackbar('Role deleted', { variant: 'info' })
    },
    onError: () => enqueueSnackbar('Failed to delete role', { variant: 'error' }),
  })
}

// ── User + Tenant edit hooks ────────────────────────────────────────────────────

export function useUpdateUser() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, ...body }: { id: string; firstName: string; lastName: string; roleId?: string }) =>
      apiClient.put(`/admin/users/${id}`, body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin', 'users'] })
      enqueueSnackbar('User updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update user', { variant: 'error' }),
  })
}

export function useUpdateTenant() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, ...body }: { id: string; name: string; customDomain?: string }) =>
      apiClient.put(`/admin/tenants/${id}`, body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.tenants() })
      enqueueSnackbar('Tenant updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update tenant', { variant: 'error' }),
  })
}

export function useUpdateAnnouncement() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ id, ...body }: { id: string; title: string; body: string; type: string; isGlobal: boolean; isActive: boolean; startsAt: string; endsAt?: string }) =>
      apiClient.put(`/admin/announcements/${id}`, body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['config', 'announcements'] })
      enqueueSnackbar('Announcement updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update announcement', { variant: 'error' }),
  })
}

export function useDeleteSubscriptionPlan() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`/admin/subscription-plans/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['master', 'subscription-plans'] })
      enqueueSnackbar('Plan deleted', { variant: 'info' })
    },
    onError: () => enqueueSnackbar('Failed to delete plan', { variant: 'error' }),
  })
}
