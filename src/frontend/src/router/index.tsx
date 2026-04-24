import { lazy, Suspense } from 'react'
import { createBrowserRouter, Navigate, Outlet } from 'react-router-dom'
import { useAuthStore } from '@/features/auth/store/authStore'
import { LoadingSpinner } from '@/components/ui'
import { AppLayout } from '@/components/layout'
import { ROLES } from '@/utils/constants'

// ── Auth pages ─────────────────────────────────────────────────
const LoginPage          = lazy(() => import('@/pages/auth/LoginPage').then((m) => ({ default: m.LoginPage })))
const RegisterPage       = lazy(() => import('@/pages/auth/RegisterPage').then((m) => ({ default: m.RegisterPage })))
const ForgotPasswordPage = lazy(() => import('@/pages/auth/ForgotPasswordPage').then((m) => ({ default: m.ForgotPasswordPage })))
const ResetPasswordPage  = lazy(() => import('@/pages/auth/ResetPasswordPage').then((m) => ({ default: m.ResetPasswordPage })))
const VerifyEmailPage    = lazy(() => import('@/pages/auth/VerifyEmailPage').then((m) => ({ default: m.VerifyEmailPage })))
const ResendVerificationPage = lazy(() => import('@/pages/auth/ResendVerificationPage').then((m) => ({ default: m.ResendVerificationPage })))

// ── Authenticated pages ────────────────────────────────────────
const DashboardPage         = lazy(() => import('@/pages/DashboardPage').then((m) => ({ default: m.DashboardPage })))
const NotificationsPage     = lazy(() => import('@/pages/NotificationsPage').then((m) => ({ default: m.NotificationsPage })))
const SettingsPage          = lazy(() => import('@/pages/SettingsPage').then((m) => ({ default: m.SettingsPage })))

// JobSeeker
const MyApplicationsPage    = lazy(() => import('@/pages/jobseeker/MyApplicationsPage').then((m) => ({ default: m.MyApplicationsPage })))
const SavedJobsPage         = lazy(() => import('@/pages/jobseeker/SavedJobsPage').then((m) => ({ default: m.SavedJobsPage })))
const ProfilePage           = lazy(() => import('@/pages/jobseeker/ProfilePage').then((m) => ({ default: m.ProfilePage })))
const JobAlertsPage         = lazy(() => import('@/pages/jobseeker/JobAlertsPage'))
const MyOffersPage          = lazy(() => import('@/pages/jobseeker/MyOffersPage').then((m) => ({ default: m.MyOffersPage })))

// Recruiter
const PostJobPage            = lazy(() => import('@/pages/recruiter/PostJobPage').then((m) => ({ default: m.PostJobPage })))
const ManageJobsPage         = lazy(() => import('@/pages/recruiter/ManageJobsPage').then((m) => ({ default: m.ManageJobsPage })))
const ManageApplicationsPage = lazy(() => import('@/pages/recruiter/ManageApplicationsPage').then((m) => ({ default: m.ManageApplicationsPage })))
const HiringPipelinePage     = lazy(() => import('@/pages/recruiter/HiringPipelinePage'))
const InterviewsPage         = lazy(() => import('@/pages/recruiter/InterviewsPage'))
const OfferLettersPage       = lazy(() => import('@/pages/recruiter/OfferLettersPage'))
const CandidatesPage         = lazy(() => import('@/pages/recruiter/CandidatesPage'))

// Company
const CompanyProfilePage     = lazy(() => import('@/pages/company/CompanyProfilePage'))
const CompanyDetailPage      = lazy(() => import('@/pages/public/CompanyDetailPage').then((m) => ({ default: m.CompanyDetailPage })))

// Public (accessible to all authenticated)
const JobsPage               = lazy(() => import('@/pages/public/JobsPage').then((m) => ({ default: m.JobsPage })))
const JobDetailPage          = lazy(() => import('@/pages/public/JobDetailPage').then((m) => ({ default: m.JobDetailPage })))
const BrowseCompaniesPage    = lazy(() => import('@/pages/public/BrowseCompaniesPage'))

// Admin
const UserManagementPage     = lazy(() => import('@/pages/admin/UserManagementPage'))
const TenantManagementPage   = lazy(() => import('@/pages/admin/TenantManagementPage'))
const RolesPermissionsPage   = lazy(() => import('@/pages/admin/RolesPermissionsPage'))
const AuditLogsPage          = lazy(() => import('@/pages/admin/AuditLogsPage'))
const SessionManagementPage  = lazy(() => import('@/pages/admin/SessionManagementPage').then((m) => ({ default: m.SessionManagementPage })))
const SystemSettingsPage     = lazy(() => import('@/pages/admin/SystemSettingsPage'))
const MasterDataPage         = lazy(() => import('@/pages/admin/MasterDataPage'))

// Billing
const SubscriptionPage       = lazy(() => import('@/pages/billing/SubscriptionPage'))
const BillingHistoryPage     = lazy(() => import('@/pages/billing/BillingHistoryPage'))
const JobCreditsPage         = lazy(() => import('@/pages/billing/JobCreditsPage'))

// ── Route guards ───────────────────────────────────────────────
function RequireAuth() {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated)
  if (!isAuthenticated) return <Navigate to="/login" replace />
  return <Outlet />
}

function RequireGuest() {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated)
  if (isAuthenticated) return <Navigate to="/" replace />
  return <Outlet />
}

function RequireRole({ allowed }: { allowed: string[] }) {
  const user = useAuthStore((s) => s.user)
  if (!user?.role || !allowed.includes(user.role)) return <Navigate to="/" replace />
  return <Outlet />
}

const S = ({ children }: { children: React.ReactNode }) => (
  <Suspense fallback={<LoadingSpinner fullPage />}>{children}</Suspense>
)

// ── Router ─────────────────────────────────────────────────────
export const router = createBrowserRouter([
  // Guest-only auth routes
  {
    element: <RequireGuest />,
    children: [
      { path: '/login',           element: <S><LoginPage /></S> },
      { path: '/register',        element: <S><RegisterPage /></S> },
      { path: '/forgot-password', element: <S><ForgotPasswordPage /></S> },
      { path: '/reset-password',  element: <S><ResetPasswordPage /></S> },
      { path: '/verify-email',    element: <S><VerifyEmailPage /></S> },
      { path: '/resend-verification', element: <S><ResendVerificationPage /></S> },
    ],
  },

  // Protected routes — wrapped in AppLayout (sidebar + topnav)
  {
    element: <RequireAuth />,
    children: [
      {
        element: <AppLayout />,
        children: [
          // Core — all authenticated users
          { path: '/',                        element: <S><DashboardPage /></S> },
          { path: '/notifications',           element: <S><NotificationsPage /></S> },
          { path: '/settings',               element: <S><SettingsPage /></S> },

          // Public browse — all authenticated
          { path: '/jobs',                   element: <S><JobsPage /></S> },
          { path: '/jobs/:id',              element: <S><JobDetailPage /></S> },
          { path: '/companies',             element: <S><BrowseCompaniesPage /></S> },
          { path: '/companies/:id',         element: <S><CompanyDetailPage /></S> },

          // JobSeeker-only routes
          {
            element: <RequireRole allowed={[ROLES.JOB_SEEKER]} />,
            children: [
              { path: '/my-applications',         element: <S><MyApplicationsPage /></S> },
              { path: '/saved-jobs',              element: <S><SavedJobsPage /></S> },
              { path: '/profile',                element: <S><ProfilePage /></S> },
              { path: '/job-alerts',             element: <S><JobAlertsPage /></S> },
              { path: '/my-offers',              element: <S><MyOffersPage /></S> },
            ],
          },

          // Recruiter / HiringManager / Admin routes
          {
            element: <RequireRole allowed={[ROLES.RECRUITER, ROLES.HIRING_MANAGER, ROLES.TENANT_ADMIN, ROLES.SUPER_ADMIN]} />,
            children: [
              { path: '/post-job',               element: <S><PostJobPage /></S> },
              { path: '/manage-jobs',            element: <S><ManageJobsPage /></S> },
              { path: '/applications',           element: <S><ManageApplicationsPage /></S> },
              { path: '/candidates',             element: <S><CandidatesPage /></S> },
              { path: '/pipeline',               element: <S><HiringPipelinePage /></S> },
              { path: '/interviews',             element: <S><InterviewsPage /></S> },
              { path: '/offers',                 element: <S><OfferLettersPage /></S> },
              { path: '/company',               element: <S><CompanyProfilePage /></S> },
            ],
          },

          // Admin-only routes (TenantAdmin + SuperAdmin)
          {
            element: <RequireRole allowed={[ROLES.TENANT_ADMIN, ROLES.SUPER_ADMIN]} />,
            children: [
              { path: '/admin/users',           element: <S><UserManagementPage /></S> },
              { path: '/admin/roles',           element: <S><RolesPermissionsPage /></S> },
              { path: '/admin/audit-logs',      element: <S><AuditLogsPage /></S> },
              { path: '/admin/sessions',        element: <S><SessionManagementPage /></S> },
              { path: '/admin/settings',        element: <S><SystemSettingsPage /></S> },
              { path: '/admin/master-data',     element: <S><MasterDataPage /></S> },
              { path: '/billing/subscription',  element: <S><SubscriptionPage /></S> },
              { path: '/billing/history',       element: <S><BillingHistoryPage /></S> },
              { path: '/billing/credits',       element: <S><JobCreditsPage /></S> },
            ],
          },

          // SuperAdmin-only routes
          {
            element: <RequireRole allowed={[ROLES.SUPER_ADMIN]} />,
            children: [
              { path: '/admin/tenants',         element: <S><TenantManagementPage /></S> },
            ],
          },
        ],
      },
    ],
  },

  // Catch-all
  { path: '*', element: <Navigate to="/" replace /> },
])

