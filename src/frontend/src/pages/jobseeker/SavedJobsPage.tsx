import { Box, Grid } from '@mui/material'
import { BookmarkOutlined } from '@mui/icons-material'
import { useSavedJobs } from '@/features/jobs/jobsApi'
import { JobCard } from '@/features/jobs/components/JobCard'
import { PageHeader, EmptyState, LoadingSpinner, Button } from '@/components/ui'
import { useNavigate } from 'react-router-dom'

export function SavedJobsPage() {
  const navigate = useNavigate()
  const { data, isLoading } = useSavedJobs()

  return (
    <Box>
      <PageHeader title="Saved Jobs" subtitle={data ? `${data.totalCount} saved job${data.totalCount !== 1 ? 's' : ''}` : ''} />

      {isLoading ? (
        <LoadingSpinner />
      ) : !data?.items.length ? (
        <EmptyState
          icon={<BookmarkOutlined />}
          title="No saved jobs"
          description="Bookmark jobs you're interested in to review them later."
          action={
            <Button variant="contained" onClick={() => navigate('/jobs')}>
              Find Jobs
            </Button>
          }
        />
      ) : (
        <Grid container spacing={2.5}>
          {data.items.map((job: import('@/types').JobSummary) => (
            <Grid key={job.id} size={{ xs: 12, sm: 6, md: 4 }}>
              <JobCard job={job} />
            </Grid>
          ))}
        </Grid>
      )}
    </Box>
  )
}
