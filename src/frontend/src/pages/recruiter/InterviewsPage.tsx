import { useState } from 'react'
import {
  Box, Button, Dialog, DialogTitle, DialogContent, DialogActions,
  Stack, Chip, Typography, TextField, Alert, CircularProgress,
  FormControl, InputLabel, Select, MenuItem,
} from '@mui/material'
import { Add as AddIcon, Videocam, LocationOn, Phone } from '@mui/icons-material'
import { useForm, Controller } from 'react-hook-form'
import type { GridColDef } from '@mui/x-data-grid'
import { useInterviews, useScheduleInterview, useUpdateInterviewStatus } from '@/features/interviews/interviewsApi'
import { useAllApplications } from '@/features/applications/applicationsApi'
import { PageHeader, DataTable, StatusChip, FormTextField, FormSelect } from '@/components/ui'

const typeIcons: Record<string, React.ReactNode> = {
  Video: <Videocam fontSize="small" />,
  InPerson: <LocationOn fontSize="small" />,
  Phone: <Phone fontSize="small" />,
}

export default function InterviewsPage() {
  const [filter, setFilter] = useState<'upcoming' | 'past' | 'all'>('upcoming')
  const [open, setOpen] = useState(false)

  const { data: interviews = [], isLoading } = useInterviews()
  const scheduleInterview = useScheduleInterview()
  const updateStatus = useUpdateInterviewStatus()
  const { data: applicationsData } = useAllApplications()
  const applications = applicationsData?.items ?? []

  const { control, handleSubmit, reset } = useForm({
    defaultValues: {
      applicationId: '',
      scheduledAt: '',
      durationMinutes: '60',
      interviewType: 'Video',
      location: '',
      meetingLink: '',
      notes: '',
    },
  })

  const onSubmit = (values: any) => {
    scheduleInterview.mutate(
      { ...values, durationMinutes: Number(values.durationMinutes) },
      { onSuccess: () => { reset(); setOpen(false) } }
    )
  }

  const columns: GridColDef<any>[] = [
    {
      field: 'candidateName', headerName: 'Candidate', flex: 1, minWidth: 160,
      renderCell: p => (
        <Box>
          <Typography variant="body2" sx={{ fontWeight: 600 }}>{p.row.candidateName}</Typography>
          <Typography variant="caption" color="text.secondary">{p.row.jobTitle}</Typography>
        </Box>
      ),
    },
    {
      field: 'scheduledOn', headerName: 'Date & Time', width: 180,
      renderCell: p => new Date(p.value).toLocaleString(),
    },
    {
      field: 'interviewType', headerName: 'Type', width: 130,
      renderCell: p => (
        <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
          {typeIcons[p.value]}
          <Typography variant="body2">{p.value}</Typography>
        </Stack>
      ),
    },
    { field: 'durationMinutes', headerName: 'Duration', width: 100, renderCell: p => `${p.value} min` },
    {
      field: 'status', headerName: 'Status', width: 120,
      renderCell: p => <StatusChip status={p.value} />,
    },
    {
      field: 'actions', headerName: 'Actions', width: 200, sortable: false,
      renderCell: p => p.row.status === 'Scheduled' ? (
        <Stack direction="row" spacing={0.5}>
          <Button
            size="small" color="success"
            onClick={() => updateStatus.mutate({ id: p.row.id, status: 'Completed' })}
          >
            Done
          </Button>
          <Button
            size="small" color="warning"
            onClick={() => updateStatus.mutate({ id: p.row.id, status: 'Cancelled' })}
          >
            Cancel
          </Button>
        </Stack>
      ) : null,
    },
  ]

  return (
    <Box>
      <PageHeader
        title="Interviews"
        subtitle="Schedule and track candidate interviews"
        actions={<Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpen(true)}>Schedule Interview</Button>}
      />

      <Stack direction="row" spacing={1} sx={{ mb: 2 }}>
        {(['upcoming', 'past', 'all'] as const).map(f => (
          <Chip
            key={f}
            label={f.charAt(0).toUpperCase() + f.slice(1)}
            onClick={() => setFilter(f)}
            color={filter === f ? 'primary' : 'default'}
            variant={filter === f ? 'filled' : 'outlined'}
          />
        ))}
      </Stack>

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 6 }}><CircularProgress /></Box>
      ) : (interviews as any[]).length === 0 ? (
        <Alert severity="info">No {filter === 'all' ? '' : filter} interviews found.</Alert>
      ) : (
        <DataTable columns={columns} rows={interviews as any[]} loading={false} />
      )}

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Schedule Interview</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <Controller
              name="applicationId"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth>
                  <InputLabel>Application</InputLabel>
                  <Select {...field} label="Application">
                    {applications.map(a => (
                      <MenuItem key={a.id} value={a.id}>
                        {a.applicantName} — {a.jobTitle}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              )}
            />
            <Controller
              name="scheduledAt"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  type="datetime-local"
                  label="Date & Time"
                  fullWidth
                  slotProps={{ inputLabel: { shrink: true } }}
                />
              )}
            />
            <Stack direction="row" spacing={2}>
              <FormSelect
                name="interviewType"
                label="Interview Type"
                control={control}
                options={[
                  { value: 'Video', label: 'Video Call' },
                  { value: 'Phone', label: 'Phone' },
                  { value: 'InPerson', label: 'In Person' },
                ]}
              />
              <FormTextField name="durationMinutes" label="Duration (minutes)" control={control} type="number" />
            </Stack>
            <FormTextField name="meetingLink" label="Meeting Link" control={control} placeholder="https://meet.google.com/..." />
            <FormTextField name="location" label="Location" control={control} placeholder="Office address (for in-person)" />
            <FormTextField name="notes" label="Notes for candidate" control={control} multiline rows={2} />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => { reset(); setOpen(false) }}>Cancel</Button>
          <Button variant="contained" onClick={handleSubmit(onSubmit)} disabled={scheduleInterview.isPending}>
            {scheduleInterview.isPending ? 'Scheduling…' : 'Schedule'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
