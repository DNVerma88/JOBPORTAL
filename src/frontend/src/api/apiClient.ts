/**
 * Axios instance pre-configured with:
 * - Base URL from env
 * - JWT Bearer token injection
 * - Automatic token refresh on 401
 * - Structured error normalization
 */
import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios'
import { useAuthStore } from '@/features/auth/store/authStore'

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? '/api/v1',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: false,
})

// ── Request interceptor: inject access token + active tenant ──
apiClient.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const { accessToken, activeTenantId } = useAuthStore.getState()
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`
  }
  const tenantId = activeTenantId ?? (import.meta.env.VITE_TENANT_ID ?? '00000000-0000-0000-0000-000000000001')
  config.headers['X-Tenant-Id'] = tenantId
  return config
})

// ── Response interceptor: handle 401 / token refresh ─────────
let isRefreshing = false
let pendingQueue: Array<{ resolve: (t: string) => void; reject: (e: unknown) => void }> = []

const processQueue = (error: unknown, token: string | null) => {
  pendingQueue.forEach((p) => (error ? p.reject(error) : p.resolve(token!)))
  pendingQueue = []
}

apiClient.interceptors.response.use(
  (response) => {
    // Unwrap the ApiResponse<T> envelope automatically:
    // Server always returns { success, data, message, errors, ... }
    // so response.data.data contains the actual payload.
    const body = response.data
    if (body !== null && typeof body === 'object' && 'success' in body && 'data' in body) {
      response.data = body.data
    }
    return response
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean }

    // Don't attempt token refresh on auth endpoints — let the caller handle the error directly
    const isAuthEndpoint = originalRequest.url?.includes('/auth/login') ||
      originalRequest.url?.includes('/auth/register') ||
      originalRequest.url?.includes('/auth/refresh-token')

    if (error.response?.status === 401 && !originalRequest._retry && !isAuthEndpoint) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          pendingQueue.push({
            resolve: (token) => {
              originalRequest.headers.Authorization = `Bearer ${token}`
              resolve(apiClient(originalRequest))
            },
            reject,
          })
        })
      }

      originalRequest._retry = true
      isRefreshing = true

      try {
        // Refresh token is stored in HttpOnly cookie — send no body, withCredentials=true
        // Must include X-Tenant-Id so the backend TenantService can resolve the tenant.
        const tenantId = useAuthStore.getState().activeTenantId
          ?? (import.meta.env.VITE_TENANT_ID ?? '00000000-0000-0000-0000-000000000001')
        const { data } = await axios.post<{ data: { accessToken: string; expiresOn: string } }>(
          `${import.meta.env.VITE_API_URL ?? '/api/v1'}/auth/refresh-token`,
          null,
          { withCredentials: true, headers: { 'X-Tenant-Id': tenantId } },
        )

        const newToken = data.data.accessToken
        useAuthStore.getState().setAccessToken(newToken)
        processQueue(null, newToken)
        originalRequest.headers.Authorization = `Bearer ${newToken}`
        return apiClient(originalRequest)
      } catch (refreshError) {
        processQueue(refreshError, null)
        useAuthStore.getState().logout()
        return Promise.reject(refreshError)
      } finally {
        isRefreshing = false
      }
    }

    return Promise.reject(error)
  },
)

export type ApiError = {
  status: number
  message: string
  errors?: Record<string, string[]>
}

export const extractApiError = (error: unknown): ApiError => {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as { message?: string; errors?: Record<string, string[]> } | undefined
    return {
      status: error.response?.status ?? 0,
      message: data?.message ?? error.message,
      errors: data?.errors,
    }
  }
  return { status: 0, message: 'An unexpected error occurred' }
}
