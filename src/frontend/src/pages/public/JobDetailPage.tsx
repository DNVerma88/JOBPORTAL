import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import {
  Box,
  Grid,
  Typography,
  Avatar,
  Button,
  Chip,
  Divider,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
} from '@mui/material'
import {
  LocationOnOutlined,
  WorkOutlined,
  AttachMoneyOutlined,
  PeopleOutlined,
  BookmarkOutlined,
  BookmarkBorderOutlined,
  ArrowBackOutlined,
} from '@mui/icons-material'
import { useJob } from '@/features/jobs/jobsApi'
import { useApplyJob } from '@/features/applications/applicationsApi'
import { useSaveJob } from '@/features/jobs/jobsApi'
import { useAuthStore } from '@/features/auth/store/authStore'
import { LoadingSpinner, SectionCard, EmptyState } from '@/components/ui'
import { JOB_TYPE_LABELS, WORK_MODE_LABELS, EXP_LEVEL_LABELS } from '@/utils/constants'
import { formatSalary, formatDate } from '@/utils/formatters'

export function JobDetailPage() {
  const { id = '' } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data: job, isLoading, isError } = useJob(id)
  const applyJob = useApplyJob()
  const saveJob = useSaveJob()
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated)

  const [applyOpen, setApplyOpen] = useState(false)
  const [coverLetter, setCoverLetter] = useState('')

  const handleApply = () => {
    applyJob.mutate({ jobId: id, coverLetter: coverLetter || undefined }, {
      onSuccess: () => setApplyOpen(false),
    })
  }

  if (isLoading) return <LoadingSpinner />
  if (isError || !job) {
    return (
      <EmptyState
        icon={<WorkOutlined />}
        title="Job not found"
        description="This job may have been removed or expired."
        action={<Button variant="outlined" onClick={() => navigate('/jobs')}>Browse Jobs</Button>}
      />
    )
  }

  return (
    <Box>
      <Button startIcon={<ArrowBackOutlined />} onClick={() => navigate(-1)} sx={{ mb: 2 }}>
        Back to Jobs
      </Button>

      <Grid container spacing={3}>
        {/* Main content */}
        <Grid size={{ xs: 12, md: 8 }}>
          <SectionCard>
            {/* Job header */}
            <Box sx={{ display: 'flex', gap: 2, mb: 3, alignItems: 'flex-start' }}>
              <Avatar
                src={job.companyLogoUrl}
                variant="rounded"
                sx={{ width: 64, height: 64, bgcolor: 'primary.50', color: 'primary.main', fontSize: 24, fontWeight: 700 }}
              >
                {job.companyName[0]}
              </Avatar>
              <Box sx={{ flex: 1 }}>
                <Typography variant="h5" sx={{ fontWeight: 700 }}>{job.title}</Typography>
                <Typography variant="body1" color="text.secondary">{job.companyName}</Typography>
              </Box>
            </Box>

            {/* Meta chips */}
            <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mb: 3 }}>
              <Chip icon={<LocationOnOutlined />} label={job.location} size="small" />
              <Chip icon={<WorkOutlined />} label={JOB_TYPE_LABELS[job.jobType as import('@/types').JobType] ?? job.jobType} size="small" />
              <Chip label={WORK_MODE_LABELS[job.workMode as import('@/types').WorkMode] ?? job.workMode} size="small" />
              <Chip label={EXP_LEVEL_LABELS[job.experienceLevel as import('@/types').ExperienceLevel] ?? job.experienceLevel} size="small" />
              {(job.salaryMin || job.salaryMax) && (
                <Chip icon={<AttachMoneyOutlined />} label={formatSalary(job.salaryMin, job.salaryMax, job.currency)} size="small" color="success" />
              )}
            </Box>

            <Divider sx={{ mb: 3 }} />

            {/* Description */}
            <Typography variant="subtitle1" sx={{ fontWeight: 600 }} gutterBottom>Job Description</Typography>
            <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap', mb: 3 }}>{job.description}</Typography>

            {job.responsibilities && (
              <>
                <Typography variant="subtitle1" sx={{ fontWeight: 600 }} gutterBottom>Responsibilities</Typography>
                <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap', mb: 3 }}>{job.responsibilities}</Typography>
              </>
            )}

            <Typography variant="subtitle1" sx={{ fontWeight: 600 }} gutterBottom>Requirements</Typography>
            <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap', mb: 3 }}>{job.requirements}</Typography>

            {job.benefits && (
              <>
                <Typography variant="subtitle1" sx={{ fontWeight: 600 }} gutterBottom>Benefits</Typography>
                <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap', mb: 3 }}>{job.benefits}</Typography>
              </>
            )}

            {/* Skills */}
            {job.skills.length > 0 && (
              <>
                <Divider sx={{ mb: 2 }} />
                <Typography variant="subtitle1" sx={{ fontWeight: 600 }} gutterBottom>Required Skills</Typography>
                <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
                  {job.skills.map((s: string) => <Chip key={s} label={s} size="small" color="primary" variant="outlined" />)}
                </Box>
              </>
            )}
          </SectionCard>
        </Grid>

        {/* Sidebar */}
        <Grid size={{ xs: 12, md: 4 }}>
          <SectionCard title="Apply Now" sx={{ mb: 2 }}>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
              <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                <Typography variant="caption" color="text.secondary">Posted</Typography>
                <Typography variant="caption">{formatDate(job.postedAt)}</Typography>
              </Box>
              <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                <Typography variant="caption" color="text.secondary">Deadline</Typography>
                <Typography variant="caption">{formatDate(job.expiresAt)}</Typography>
              </Box>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                <PeopleOutlined sx={{ fontSize: 16, color: 'text.disabled' }} />
                <Typography variant="caption" color="text.secondary">
                  {job.applicationsCount} applicant{job.applicationsCount !== 1 ? 's' : ''}
                </Typography>
              </Box>
              <Divider />
              {isAuthenticated ? (
                <>
                  <Button variant="contained" fullWidth onClick={() => setApplyOpen(true)}>
                    Apply Now
                  </Button>
                  <Button
                    variant="outlined"
                    fullWidth
                    startIcon={job.isSaved ? <BookmarkOutlined /> : <BookmarkBorderOutlined />}
                    onClick={() => saveJob.mutate({ id: job.id, saved: job.isSaved })}
                  >
                    {job.isSaved ? 'Saved' : 'Save Job'}
                  </Button>
                </>
              ) : (
                <Button variant="contained" fullWidth onClick={() => navigate('/login')}>
                  Sign in to Apply
                </Button>
              )}
            </Box>
          </SectionCard>

          <SectionCard title="About the Company">
            <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
              {job.companyDescription ?? 'No description provided.'}
            </Typography>
            {job.companySize && (
              <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>
                Size: {job.companySize}
              </Typography>
            )}
            {job.companyIndustry && (
              <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>
                Industry: {job.companyIndustry}
              </Typography>
            )}
          </SectionCard>
        </Grid>
      </Grid>

      {/* Apply dialog */}
      <Dialog open={applyOpen} onClose={() => setApplyOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Apply for {job.title}</DialogTitle>
        <DialogContent>
          <TextField
            label="Cover Letter (optional)"
            multiline
            rows={6}
            fullWidth
            value={coverLetter}
            onChange={(e) => setCoverLetter(e.target.value)}
            sx={{ mt: 1 }}
            placeholder="Tell the recruiter why you're a great fit…"
            slotProps={{ htmlInput: { maxLength: 2000 } }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setApplyOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleApply}
            disabled={applyJob.isPending}
          >
            {applyJob.isPending ? 'Submitting…' : 'Submit Application'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
