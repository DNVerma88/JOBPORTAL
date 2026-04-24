import { Box, Grid } from '@mui/material'
import { AssignmentOutlined } from '@mui/icons-material'
import { useMyApplications, useWithdrawApplication } from '@/features/applications/applicationsApi'
import { ApplicationCard } from '@/features/applications/components/ApplicationCard'
import { PageHeader, EmptyState, LoadingSpinner, Button } from '@/components/ui'
import { useNavigate } from 'react-router-dom'

export function MyApplicationsPage() {
  const navigate = useNavigate()
  const { data, isLoading } = useMyApplications()
  const withdraw = useWithdrawApplication()

  return (
    <Box>
      <PageHeader title="My Applications" subtitle={data ? `${data.totalCount} application${data.totalCount !== 1 ? 's' : ''}` : ''} />

      {isLoading ? (
        <LoadingSpinner />
      ) : !data?.items.length ? (
        <EmptyState
          icon={<AssignmentOutlined />}
          title="No applications yet"
          description="Start applying to jobs and track your progress here."
          action={
            <Button variant="contained" onClick={() => navigate('/jobs')}>
              Browse Jobs
            </Button>
          }
        />
      ) : (
        <Grid container spacing={2}>
          {data.items.map((app: import('@/types').ApplicationSummary) => (
            <Grid key={app.id} size={{ xs: 12, sm: 6, md: 4 }}>
              <ApplicationCard
                application={app}
                onWithdraw={(id) => withdraw.mutate(id)}
              />
            </Grid>
          ))}
        </Grid>
      )}
    </Box>
  )
}
