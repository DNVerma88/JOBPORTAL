import { useState } from 'react'
import {
  Box, Button, Dialog, DialogTitle, DialogContent, DialogActions,
  Stack, Typography, Alert, CircularProgress, TextField,
  FormControl, InputLabel, Select, MenuItem,
} from '@mui/material'
import { Add as AddIcon } from '@mui/icons-material'
import { useForm, Controller } from 'react-hook-form'
import type { GridColDef } from '@mui/x-data-grid'
import { useOfferLetters, useCreateOffer } from '@/features/offers/offersApi'
import { useAllApplications } from '@/features/applications/applicationsApi'
import { PageHeader, DataTable, StatusChip, FormTextField, FormSelect } from '@/components/ui'

export default function OfferLettersPage() {
  const [open, setOpen] = useState(false)
  const { data: offers = [], isLoading } = useOfferLetters()
  const createOffer = useCreateOffer()
  const { data: applicationsData } = useAllApplications()
  const applications = applicationsData?.items ?? []

  const { control, handleSubmit, reset, setValue } = useForm({
    defaultValues: {
      applicationId: '',
      positionTitle: '',
      offerSalary: '',
      currencyCode: 'USD',
      joiningDate: '',
      expiresAt: '',
      department: '',
    },
  })

  const onSubmit = (values: any) => {
    createOffer.mutate(
      { ...values, offerSalary: Number(values.offerSalary) },
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
      field: 'salary', headerName: 'Offered Salary', width: 160,
      renderCell: p => `${p.row.currency ?? 'USD'} ${p.value?.toLocaleString() ?? '—'}`,
    },
    {
      field: 'joiningDate', headerName: 'Joining Date', width: 140,
      renderCell: p => p.value ? new Date(p.value).toLocaleDateString() : '—',
    },
    {
      field: 'expiresOn', headerName: 'Offer Expires', width: 160,
      renderCell: p => {
        if (!p.value) return '—'
        const expired = new Date(p.value) < new Date()
        return (
          <Typography variant="body2" color={expired ? 'error.main' : 'inherit'}>
            {new Date(p.value).toLocaleDateString()}{expired ? ' (Expired)' : ''}
          </Typography>
        )
      },
    },
    {
      field: 'status', headerName: 'Status', width: 120,
      renderCell: p => <StatusChip status={p.value} />,
    },
    { field: 'sentOn', headerName: 'Sent On', width: 130, renderCell: p => p.value ? new Date(p.value).toLocaleDateString() : '—' },
  ]

  return (
    <Box>
      <PageHeader
        title="Offer Letters"
        subtitle="Manage job offers sent to candidates"
        actions={<Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpen(true)}>Create Offer</Button>}
      />

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 6 }}><CircularProgress /></Box>
      ) : (offers as any[]).length === 0 ? (
        <Alert severity="info">No offer letters yet. Create an offer for a successful candidate.</Alert>
      ) : (
        <DataTable columns={columns} rows={offers as any[]} loading={false} />
      )}

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create Offer Letter</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <Controller
              name="applicationId"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth>
                  <InputLabel>Application</InputLabel>
                  <Select
                    {...field}
                    label="Application"
                    onChange={e => {
                      field.onChange(e)
                      const app = applications.find(a => a.id === e.target.value)
                      if (app) setValue('positionTitle', app.jobTitle ?? '')
                    }}
                  >
                    {applications.map(a => (
                      <MenuItem key={a.id} value={a.id}>
                        {a.applicantName} — {a.jobTitle}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              )}
            />
            <FormTextField name="positionTitle" label="Position Title *" control={control} />
            <Stack direction="row" spacing={2}>
              <FormTextField name="offerSalary" label="Offered Salary" control={control} type="number" />
              <FormSelect
                name="currencyCode"
                label="Currency"
                control={control}
                options={[
                  { value: 'USD', label: 'USD' },
                  { value: 'EUR', label: 'EUR' },
                  { value: 'GBP', label: 'GBP' },
                  { value: 'INR', label: 'INR' },
                ]}
              />
            </Stack>
            <Controller
              name="joiningDate"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  type="date"
                  label="Proposed Joining Date"
                  fullWidth
                  slotProps={{ inputLabel: { shrink: true } }}
                />
              )}
            />
            <Controller
              name="expiresAt"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  type="date"
                  label="Offer Expiry Date"
                  fullWidth
                  slotProps={{ inputLabel: { shrink: true } }}
                />
              )}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => { reset(); setOpen(false) }}>Cancel</Button>
          <Button variant="contained" onClick={handleSubmit(onSubmit)} disabled={createOffer.isPending}>
            {createOffer.isPending ? 'Creating…' : 'Send Offer'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
