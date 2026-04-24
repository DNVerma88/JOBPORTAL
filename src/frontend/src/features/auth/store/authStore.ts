import { create } from 'zustand'
import { persist, createJSONStorage } from 'zustand/middleware'
import type { UserProfile } from '@/types'

const SYSTEM_TENANT_ID = '00000000-0000-0000-0000-000000000001'

interface AuthState {
  accessToken: string | null
  user: UserProfile | null
  isAuthenticated: boolean
  activeTenantId: string
  activeTenantName: string

  setAccessToken: (accessToken: string) => void
  setUser: (user: UserProfile) => void
  switchTenant: (tenantId: string, tenantName: string) => void
  logout: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      accessToken: null,
      user: null,
      isAuthenticated: false,
      activeTenantId: SYSTEM_TENANT_ID,
      activeTenantName: 'System',

      setAccessToken: (accessToken) =>
        set({ accessToken, isAuthenticated: true }),

      setUser: (user) => set({ user }),

      switchTenant: (tenantId, tenantName) => set({ activeTenantId: tenantId, activeTenantName: tenantName }),

      logout: () =>
        set({ accessToken: null, user: null, isAuthenticated: false, activeTenantId: SYSTEM_TENANT_ID, activeTenantName: 'System' }),
    }),
    {
      name: 'auth-storage',
      storage: createJSONStorage(() => localStorage),
      partialize: (state) => ({
        user: state.user,
        isAuthenticated: state.isAuthenticated,
        activeTenantId: state.activeTenantId,
        activeTenantName: state.activeTenantName,
      }),
    },
  ),
)
