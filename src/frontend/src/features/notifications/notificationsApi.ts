import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/api/apiClient'
import type { PagedResult, Notification } from '@/types'

const KEYS = {
  list: () => ['notifications'] as const,
  unreadCount: () => ['notifications', 'unread-count'] as const,
}

export function useNotifications() {
  return useQuery({
    queryKey: KEYS.list(),
    queryFn: async () => {
      const { data } = await apiClient.get<PagedResult<Notification>>('/notifications')
      return data
    },
  })
}

export function useUnreadCount() {
  return useQuery({
    queryKey: KEYS.unreadCount(),
    queryFn: async () => {
      const { data } = await apiClient.get<{ count: number }>('/notifications/unread-count')
      return data?.count ?? 0
    },
    refetchInterval: 30_000,
  })
}

export function useMarkAsRead() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => apiClient.patch(`/notifications/${id}/read`, {}),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.list() })
      qc.invalidateQueries({ queryKey: KEYS.unreadCount() })
    },
  })
}

export function useMarkAllAsRead() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: () => apiClient.post('/notifications/read-all', {}),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.list() })
      qc.invalidateQueries({ queryKey: KEYS.unreadCount() })
    },
  })
}

export function useDeleteNotification() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`/notifications/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.list() })
      qc.invalidateQueries({ queryKey: KEYS.unreadCount() })
    },
  })
}
