import { apiClient } from '@/api/apiClient'
import { useMutation } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import type { LoginRequest, RegisterRequest } from '@/types'

export interface LoginApiResponse {
  accessToken: string
  refreshToken: string
  expiresOn: string
  userId: string
  fullName: string
  email: string
  roles: string[]
}

export interface RegisterApiResponse {
  userId: string
  email: string
  fullName: string
}

export const authApi = {
  login: (data: LoginRequest) =>
    apiClient.post<LoginApiResponse>('/auth/login', data),

  register: (data: RegisterRequest) =>
    apiClient.post<RegisterApiResponse>('/auth/register', data),

  logout: () =>
    apiClient.post('/auth/logout'),

  refreshToken: () =>
    apiClient.post<{ accessToken: string; expiresOn: string }>('/auth/refresh-token'),

  changePassword: (currentPassword: string, newPassword: string) =>
    apiClient.post('/auth/change-password', { currentPassword, newPassword }),

  forgotPassword: (email: string) =>
    apiClient.post('/auth/forgot-password', { email }),

  resetPassword: (token: string, password: string, _confirmPassword: string) =>
    apiClient.post('/auth/reset-password', { token, newPassword: password }),

  verifyEmail: (token: string) =>
    apiClient.post('/auth/verify-email', { token }),

  resendVerification: (email: string) =>
    apiClient.post('/auth/resend-verification', { email }),
}

// ── React-Query hooks ─────────────────────────────────────────

export function useChangePassword() {
  const { enqueueSnackbar } = useSnackbar()
  return useMutation({
    mutationFn: ({ currentPassword, newPassword }: { currentPassword: string; newPassword: string }) =>
      authApi.changePassword(currentPassword, newPassword),
    onSuccess: () =>
      enqueueSnackbar('Password changed successfully. Please log in again on other devices.', { variant: 'success' }),
    onError: (err: any) => {
      const msg = err?.response?.data?.message ?? 'Failed to change password.'
      enqueueSnackbar(msg, { variant: 'error' })
    },
  })
}
