import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/api/apiClient'

export interface DashboardStats {
  // JobSeeker
  totalApplications?: number
  activeApplications?: number
  savedJobs?: number
  profileViews?: number
  // Recruiter / Admin
  totalJobs?: number
  activeJobs?: number
  totalCandidates?: number
  pendingReviews?: number
  hiredThisMonth?: number
}

export interface ApplicationTrend {
  date: string
  count: number
}

export function useDashboardStats() {
  return useQuery({
    queryKey: ['dashboard-stats'],
    queryFn: async () => {
      const { data } = await apiClient.get<DashboardStats>('/dashboard/stats')
      return data
    },
  })
}

export function useApplicationTrend() {
  return useQuery({
    queryKey: ['application-trend'],
    queryFn: async () => {
      const { data } = await apiClient.get<ApplicationTrend[]>('/dashboard/application-trend')
      return data
    },
  })
}
