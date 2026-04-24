import { useAuthStore } from '@/features/auth/store/authStore'
import { ROLES } from '@/utils/constants'

export function usePermissions() {
  const user = useAuthStore((s) => s.user)
  const role = user?.role ?? ''

  return {
    role,
    isSuperAdmin: role === ROLES.SUPER_ADMIN,
    isTenantAdmin: role === ROLES.TENANT_ADMIN,
    isAdmin: role === ROLES.SUPER_ADMIN || role === ROLES.TENANT_ADMIN,
    isRecruiter: ([ROLES.RECRUITER, ROLES.HIRING_MANAGER, ROLES.TENANT_ADMIN, ROLES.SUPER_ADMIN] as string[]).includes(role),
    isHiringManager: role === ROLES.HIRING_MANAGER,
    isJobSeeker: role === ROLES.JOB_SEEKER,
    can: (requiredRoles: string[]) => requiredRoles.includes(role),
  }
}
