import { useState } from 'react'
import {
  Box, Button, Dialog, DialogTitle, DialogContent, DialogActions,
  Stack, Typography, Switch, FormControlLabel, Chip, Alert, CircularProgress,
  Grid, Paper
} from '@mui/material'
import { Add as AddIcon, Delete as DeleteIcon, NotificationsActive } from '@mui/icons-material'
import { useJobAlerts, useCreateJobAlert, useDeleteJobAlert, useToggleJobAlert } from '@/features/job-alerts/jobAlertsApi'
import { PageHeader, FormTextField, FormSelect } from '@/components/ui'
import { useForm } from 'react-hook-form'

const frequencyColors: Record<string, 'default' | 'primary' | 'secondary'> = {
  Instant: 'primary',
  Daily: 'secondary',
  Weekly: 'default',
}

export default function JobAlertsPage() {
  const [open, setOpen] = useState(false)
  const { data: alerts = [], isLoading } = useJobAlerts()
  const createAlert = useCreateJobAlert()
  const deleteAlert = useDeleteJobAlert()
  const toggleAlert = useToggleJobAlert()

  const { control, handleSubmit, reset } = useForm({
    defaultValues: {
      keyword: '',
      location: '',
      jobType: '',
      workMode: '',
      frequency: 'Daily',
    },
  })

  const onSubmit = (values: any) => {
    createAlert.mutate(values, { onSuccess: () => { reset(); setOpen(false) } })
  }

  return (
    <Box>
      <PageHeader
        title="Job Alerts"
        subtitle="Get notified when matching jobs are posted"
        actions={<Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpen(true)}>Create Alert</Button>}
      />

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 6 }}><CircularProgress /></Box>
      ) : (alerts as any[]).length === 0 ? (
        <Alert severity="info" icon={<NotificationsActive />}>
          No job alerts set up yet. Create an alert to be notified when matching jobs are posted.
        </Alert>
      ) : (
        <Grid container spacing={2}>
          {(alerts as any[]).map((alert) => (
            <Grid size={{ xs: 12, sm: 6, md: 4 }} key={alert.id}>
              <Paper
                sx={{
                  p: 2,
                  border: '1px solid',
                  borderColor: alert.isActive ? 'primary.light' : 'divider',
                  opacity: alert.isActive ? 1 : 0.65,
                }}
              >
                <Stack spacing={1.5}>
                  <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <Box>
                      <Typography variant="subtitle2" sx={{ fontWeight: 700 }}>
                        {alert.keyword || 'All Jobs'}
                      </Typography>
                      {alert.location && (
                        <Typography variant="body2" color="text.secondary">{alert.location}</Typography>
                      )}
                    </Box>
                    <Chip
                      label={alert.frequency}
                      size="small"
                      color={frequencyColors[alert.frequency] ?? 'default'}
                    />
                  </Stack>

                  <Stack direction="row" spacing={0.75} sx={{ flexWrap: 'wrap' }}>
                    {alert.jobType && <Chip label={alert.jobType} size="small" variant="outlined" />}
                    {alert.workMode && <Chip label={alert.workMode} size="small" variant="outlined" />}
                  </Stack>

                  <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
                    <FormControlLabel
                      control={
                        <Switch
                          size="small"
                          checked={alert.isActive}
                          onChange={e => toggleAlert.mutate({ id: alert.id, isActive: e.target.checked })}
                        />
                      }
                      label={
                        <Typography variant="caption">{alert.isActive ? 'Active' : 'Paused'}</Typography>
                      }
                    />
                    <Button
                      size="small"
                      color="error"
                      startIcon={<DeleteIcon />}
                      onClick={() => deleteAlert.mutate(alert.id)}
                    >
                      Delete
                    </Button>
                  </Stack>
                </Stack>
              </Paper>
            </Grid>
          ))}
        </Grid>
      )}

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Create Job Alert</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <FormTextField name="keyword" label="Keywords" control={control} placeholder="e.g. React Developer" />
            <FormTextField name="location" label="Location" control={control} placeholder="City or Remote" />
            <FormSelect
              name="jobType"
              label="Job Type"
              control={control}
              options={[
                { value: '', label: 'Any' },
                { value: 'FullTime', label: 'Full Time' },
                { value: 'PartTime', label: 'Part Time' },
                { value: 'Contract', label: 'Contract' },
                { value: 'Freelance', label: 'Freelance' },
                { value: 'Internship', label: 'Internship' },
              ]}
            />
            <FormSelect
              name="workMode"
              label="Work Mode"
              control={control}
              options={[
                { value: '', label: 'Any' },
                { value: 'Remote', label: 'Remote' },
                { value: 'Hybrid', label: 'Hybrid' },
                { value: 'OnSite', label: 'On-Site' },
              ]}
            />
            <FormSelect
              name="frequency"
              label="Alert Frequency"
              control={control}
              options={[
                { value: 'Instant', label: 'Instant (as posted)' },
                { value: 'Daily', label: 'Daily digest' },
                { value: 'Weekly', label: 'Weekly digest' },
              ]}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => { reset(); setOpen(false) }}>Cancel</Button>
          <Button variant="contained" onClick={handleSubmit(onSubmit)} disabled={createAlert.isPending}>
            {createAlert.isPending ? 'Creating…' : 'Create Alert'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
