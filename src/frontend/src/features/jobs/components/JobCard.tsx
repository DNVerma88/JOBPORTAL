import {
  Card,
  CardContent,
  CardActionArea,
  Box,
  Typography,
  Chip,
  Avatar,
  IconButton,
  Tooltip,
  Divider,
} from '@mui/material'
import {
  LocationOnOutlined,
  BookmarkOutlined,
  BookmarkBorderOutlined,
  PeopleOutlined,
} from '@mui/icons-material'
import { useNavigate } from 'react-router-dom'
import type { JobSummary } from '@/types'
import { JOB_TYPE_LABELS, WORK_MODE_LABELS } from '@/utils/constants'
import { formatSalary, formatRelativeTime } from '@/utils/formatters'
import { useSaveJob } from '@/features/jobs/jobsApi'

interface JobCardProps {
  job: JobSummary
  showActions?: boolean
}

export function JobCard({ job, showActions = true }: JobCardProps) {
  const navigate = useNavigate()
  const saveJob = useSaveJob()

  const handleSave = (e: React.MouseEvent) => {
    e.stopPropagation()
    saveJob.mutate({ id: job.id, saved: job.isSaved })
  }

  return (
    <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <CardActionArea onClick={() => navigate(`/jobs/${job.id}`)} sx={{ flex: 1 }}>
        <CardContent sx={{ p: 2.5 }}>
          {/* Header */}
          <Box sx={{ display: 'flex', gap: 1.5, mb: 1.5 }}>
            <Avatar
              src={job.companyLogoUrl}
              variant="rounded"
              sx={{ width: 44, height: 44, bgcolor: 'primary.50', color: 'primary.main', fontWeight: 700 }}
            >
              {job.companyName[0]}
            </Avatar>
            <Box sx={{ flex: 1, minWidth: 0 }}>
              <Typography variant="subtitle1" sx={{ fontWeight: 600 }} noWrap>
                {job.title}
              </Typography>
              <Typography variant="body2" color="text.secondary" noWrap>
                {job.companyName}
              </Typography>
            </Box>
            {showActions && (
              <Tooltip title={job.isSaved ? 'Remove from saved' : 'Save job'}>
                <IconButton
                  size="small"
                  onClick={handleSave}
                  sx={{ color: job.isSaved ? 'primary.main' : 'text.disabled', mt: -0.5, mr: -0.5 }}
                  aria-label={job.isSaved ? 'unsave job' : 'save job'}
                >
                  {job.isSaved ? <BookmarkOutlined /> : <BookmarkBorderOutlined />}
                </IconButton>
              </Tooltip>
            )}
          </Box>

          {/* Tags */}
          <Box sx={{ display: 'flex', gap: 0.75, flexWrap: 'wrap', mb: 1.5 }}>
            <Chip label={JOB_TYPE_LABELS[job.jobType] ?? job.jobType} size="small" variant="outlined" />
            <Chip label={WORK_MODE_LABELS[job.workMode] ?? job.workMode} size="small" variant="outlined" />
            <Chip label={job.experienceLevel} size="small" variant="outlined" />
          </Box>

          {/* Location & salary */}
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mb: 0.5 }}>
            <LocationOnOutlined sx={{ fontSize: 16, color: 'text.disabled' }} />
            <Typography variant="body2" color="text.secondary">
              {job.location}
            </Typography>
          </Box>
          <Typography variant="body2" sx={{ fontWeight: 600, mb: 1.5 }} color="primary.main">
            {formatSalary(job.salaryMin, job.salaryMax, job.currency)}
          </Typography>

          <Divider sx={{ mb: 1.5 }} />

          {/* Footer */}
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <PeopleOutlined sx={{ fontSize: 14, color: 'text.disabled' }} />
              <Typography variant="caption" color="text.secondary">
                {job.applicationsCount} applicant{job.applicationsCount !== 1 ? 's' : ''}
              </Typography>
            </Box>
            <Typography variant="caption" color="text.secondary">
              {formatRelativeTime(job.postedAt)}
            </Typography>
          </Box>
        </CardContent>
      </CardActionArea>
    </Card>
  )
}
