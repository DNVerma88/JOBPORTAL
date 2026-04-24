import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import { apiClient } from '@/api/apiClient'

// ── Types ──────────────────────────────────────────────────────

export interface WorkExperience {
  id: string
  jobTitle: string
  companyName: string
  location?: string
  startDate: string
  endDate?: string
  isCurrent: boolean
  description?: string
}

export interface Education {
  id: string
  degree: string
  fieldOfStudy: string
  institutionName: string
  startYear: number
  endYear?: number
  grade?: string
  description?: string
}

export interface CandidateProfile {
  id: string
  userId: string
  firstName: string
  lastName: string
  email: string
  phone?: string
  headline?: string
  about?: string
  city?: string
  country?: string
  profilePictureUrl?: string
  resumeUrl?: string
  linkedInUrl?: string
  portfolioUrl?: string
  skills: string[]
  workExperiences: WorkExperience[]
  educations: Education[]
  totalExperienceYears: number
  isOpenToWork: boolean
}

export interface UpdateProfileRequest {
  firstName?: string
  lastName?: string
  phone?: string
  headline?: string
  about?: string
  city?: string
  country?: string
  linkedInUrl?: string
  portfolioUrl?: string
  skills?: string[]
  isOpenToWork?: boolean
}

// ── Query keys ─────────────────────────────────────────────────

const KEYS = {
  me: () => ['profile', 'me'] as const,
  workExperiences: () => ['profile', 'work-experiences'] as const,
  educations: () => ['profile', 'educations'] as const,
}

// ── Hooks ──────────────────────────────────────────────────────

export function useProfile() {
  return useQuery({
    queryKey: KEYS.me(),
    queryFn: async () => {
      const { data } = await apiClient.get<CandidateProfile>('/candidates/me')
      return data
    },
  })
}

export function useUpdateProfile() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: UpdateProfileRequest) => apiClient.put('/candidates/me', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.me() })
      enqueueSnackbar('Profile updated', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to update profile', { variant: 'error' }),
  })
}

export interface UpdateUserProfileRequest {
  firstName: string
  lastName: string
  gender?: number
  dateOfBirth?: string
  profilePictureUrl?: string
}

export function useUpdateUserProfile() {
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: UpdateUserProfileRequest) => apiClient.put('/auth/profile', body),
    onSuccess: () => enqueueSnackbar('Account details updated', { variant: 'success' }),
    onError: () => enqueueSnackbar('Failed to update account details', { variant: 'error' }),
  })
}

export function useUploadResume() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (file: File) => {
      const form = new FormData()
      form.append('file', file)
      return apiClient.post<{ resumeUrl: string }>('/candidates/me/resume', form, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.me() })
      enqueueSnackbar('Resume uploaded', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to upload resume', { variant: 'error' }),
  })
}

export function useAddWorkExperience() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: Omit<WorkExperience, 'id'>) =>
      apiClient.post('/candidates/me/work-experiences', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.me() })
      qc.invalidateQueries({ queryKey: KEYS.workExperiences() })
      enqueueSnackbar('Work experience added', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to add experience', { variant: 'error' }),
  })
}

export function useDeleteWorkExperience() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`/candidates/me/work-experiences/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.me() })
      qc.invalidateQueries({ queryKey: KEYS.workExperiences() })
      enqueueSnackbar('Work experience removed', { variant: 'info' })
    },
    onError: () => enqueueSnackbar('Failed to remove experience', { variant: 'error' }),
  })
}

export function useAddEducation() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (body: Omit<Education, 'id'>) =>
      apiClient.post('/candidates/me/educations', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.me() })
      qc.invalidateQueries({ queryKey: KEYS.educations() })
      enqueueSnackbar('Education added', { variant: 'success' })
    },
    onError: () => enqueueSnackbar('Failed to add education', { variant: 'error' }),
  })
}

export function useDeleteEducation() {
  const qc = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`/candidates/me/educations/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.me() })
      qc.invalidateQueries({ queryKey: KEYS.educations() })
      enqueueSnackbar('Education removed', { variant: 'info' })
    },
    onError: () => enqueueSnackbar('Failed to remove education', { variant: 'error' }),
  })
}

export function useWorkExperiences() {
  return useQuery({
    queryKey: KEYS.workExperiences(),
    queryFn: async () => {
      const { data } = await apiClient.get<WorkExperience[]>('/candidates/me/work-experiences')
      return data ?? []
    },
  })
}

export function useEducations() {
  return useQuery({
    queryKey: KEYS.educations(),
    queryFn: async () => {
      const { data } = await apiClient.get<Education[]>('/candidates/me/educations')
      return data ?? []
    },
  })
}

export function useCandidateById(profileId: string | undefined) {
  return useQuery({
    queryKey: ['candidate', profileId],
    queryFn: async () => {
      const { data } = await apiClient.get(`/candidates/${profileId}`)
      return data
    },
    enabled: !!profileId,
  })
}
