import { Grid, Box, Typography } from '@mui/material'
import {
  WorkOutlined,
  AssignmentOutlined,
  BookmarkOutlined,
  VisibilityOutlined,
  PeopleOutlined,
  PostAddOutlined,
  CheckCircleOutlined,
  HourglassEmptyOutlined,
} from '@mui/icons-material'
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip as ChartTooltip,
  ResponsiveContainer,
} from 'recharts'
import { useAuthStore } from '@/features/auth/store/authStore'
import { usePermissions } from '@/hooks/usePermissions'
import { useDashboardStats, useApplicationTrend } from '@/features/dashboard/dashboardApi'
import { PageHeader, StatCard, SectionCard, EmptyState } from '@/components/ui'
import { formatNumber } from '@/utils/formatters'

// ── JobSeeker Dashboard ────────────────────────────────────────

function JobSeekerDashboard() {
  const { data: stats } = useDashboardStats()
  const { data: trend } = useApplicationTrend()

  return (
    <>
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StatCard
            title="Total Applications"
            value={stats?.totalApplications != null ? formatNumber(stats.totalApplications) : '—'}
            icon={<AssignmentOutlined />}
            color="primary"
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StatCard
            title="Active Applications"
            value={stats?.activeApplications != null ? formatNumber(stats.activeApplications) : '—'}
            icon={<HourglassEmptyOutlined />}
            color="info"
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StatCard
            title="Saved Jobs"
            value={stats?.savedJobs != null ? formatNumber(stats.savedJobs) : '—'}
            icon={<BookmarkOutlined />}
            color="warning"
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StatCard
            title="Profile Views"
            value={stats?.profileViews != null ? formatNumber(stats.profileViews) : '—'}
            icon={<VisibilityOutlined />}
            color="success"
          />
        </Grid>
      </Grid>

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 8 }}>
          <SectionCard title="Application Activity">
            {trend?.length ? (
              <Box sx={{ height: 260 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <AreaChart data={trend} margin={{ top: 4, right: 4, left: -20, bottom: 0 }}>
                    <defs>
                      <linearGradient id="appGrad" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="5%" stopColor="#3b82f6" stopOpacity={0.2} />
                        <stop offset="95%" stopColor="#3b82f6" stopOpacity={0} />
                      </linearGradient>
                    </defs>
                    <CartesianGrid strokeDasharray="3 3" stroke="rgba(0,0,0,0.06)" />
                    <XAxis dataKey="date" tick={{ fontSize: 12 }} />
                    <YAxis tick={{ fontSize: 12 }} />
                    <ChartTooltip />
                    <Area type="monotone" dataKey="count" stroke="#3b82f6" fill="url(#appGrad)" strokeWidth={2} />
                  </AreaChart>
                </ResponsiveContainer>
              </Box>
            ) : (
              <EmptyState title="No activity yet" description="Start applying to jobs to see your activity here." />
            )}
          </SectionCard>
        </Grid>
        <Grid size={{ xs: 12, md: 4 }}>
          <SectionCard title="Quick Tips">
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
              {[
                'Complete your profile to stand out',
                'Upload your latest resume',
                'Add a professional headline',
                'Set job alerts for your target roles',
              ].map((tip) => (
                <Box key={tip} sx={{ display: 'flex', gap: 1, alignItems: 'flex-start' }}>
                  <CheckCircleOutlined sx={{ fontSize: 18, color: 'success.main', mt: 0.1, flexShrink: 0 }} />
                  <Typography variant="body2">{tip}</Typography>
                </Box>
              ))}
            </Box>
          </SectionCard>
        </Grid>
      </Grid>
    </>
  )
}

// ── Recruiter / Admin Dashboard ────────────────────────────────

function RecruiterDashboard() {
  const { data: stats } = useDashboardStats()

  return (
    <>
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StatCard
            title="Active Jobs"
            value={stats?.activeJobs != null ? formatNumber(stats.activeJobs) : '—'}
            icon={<WorkOutlined />}
            color="primary"
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StatCard
            title="Total Applications"
            value={stats?.totalApplications != null ? formatNumber(stats.totalApplications) : '—'}
            icon={<AssignmentOutlined />}
            color="info"
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StatCard
            title="Pending Reviews"
            value={stats?.pendingReviews != null ? formatNumber(stats.pendingReviews) : '—'}
            icon={<HourglassEmptyOutlined />}
            color="warning"
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StatCard
            title="Hired This Month"
            value={stats?.hiredThisMonth != null ? formatNumber(stats.hiredThisMonth) : '—'}
            icon={<PeopleOutlined />}
            color="success"
          />
        </Grid>
      </Grid>

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 7 }}>
          <SectionCard title="Hiring Pipeline">
            <EmptyState
              icon={<PostAddOutlined />}
              title="Post your first job"
              description="Once you post jobs and receive applications, your hiring pipeline will appear here."
            />
          </SectionCard>
        </Grid>
        <Grid size={{ xs: 12, md: 5 }}>
          <SectionCard title="Recent Activity">
            <EmptyState title="No recent activity" description="Application updates will appear here." />
          </SectionCard>
        </Grid>
      </Grid>
    </>
  )
}

// ── Page ───────────────────────────────────────────────────────

export function DashboardPage() {
  const user = useAuthStore((s) => s.user)
  const { isJobSeeker } = usePermissions()

  return (
    <Box>
      <PageHeader
        title={`Welcome back, ${user?.firstName ?? 'there'} 👋`}
        subtitle={isJobSeeker ? 'Track your job search progress' : 'Monitor your hiring pipeline'}
      />
      {isJobSeeker ? <JobSeekerDashboard /> : <RecruiterDashboard />}
    </Box>
  )
}
