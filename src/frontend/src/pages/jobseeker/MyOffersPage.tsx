import { useState } from 'react'
import {
  Box, Button, Chip, Dialog, DialogActions, DialogContent,
  DialogTitle, Stack, TextField, Typography,
} from '@mui/material'
import type { GridColDef, GridRenderCellParams } from '@mui/x-data-grid'
import { useOfferLetters, useRespondToOffer } from '@/features/offers/offersApi'
import { useAuthStore } from '@/features/auth/store/authStore'
import { PageHeader, DataTable } from '@/components/ui'

const STATUS_COLORS: Record<string, 'success' | 'error' | 'warning' | 'default' | 'info'> = {
  Sent: 'info',
  Accepted: 'success',
  Declined: 'error',
  Expired: 'warning',
  Revoked: 'error',
}

export function MyOffersPage() {
  const user = useAuthStore((s) => s.user)
  const { data, isLoading } = useOfferLetters({ applicantId: user?.id })
  const respondToOffer = useRespondToOffer()

  const [dialogOffer, setDialogOffer] = useState<{ id: string; response: 'Accepted' | 'Declined' } | null>(null)
  const [message, setMessage] = useState('')

  const rows = (data as any)?.items ?? data ?? []

  const handleRespond = () => {
    if (!dialogOffer) return
    respondToOffer.mutate(
      { id: dialogOffer.id, response: dialogOffer.response, message: message || undefined },
      { onSettled: () => { setDialogOffer(null); setMessage('') } },
    )
  }

  const columns: GridColDef[] = [
    {
      field: 'positionTitle',
      headerName: 'Position',
      flex: 1.5,
      minWidth: 180,
    },
    {
      field: 'offerSalary',
      headerName: 'Offered Salary',
      width: 160,
      renderCell: ({ row, value }: GridRenderCellParams) =>
        `${row.currencyCode ?? 'USD'} ${Number(value).toLocaleString()}`,
    },
    {
      field: 'offerDate',
      headerName: 'Offer Date',
      width: 120,
      renderCell: ({ value }: GridRenderCellParams) =>
        value ? new Date(value as string).toLocaleDateString() : '—',
    },
    {
      field: 'joiningDate',
      headerName: 'Joining Date',
      width: 120,
      renderCell: ({ value }: GridRenderCellParams) =>
        value ? new Date(value as string).toLocaleDateString() : '—',
    },
    {
      field: 'expiresAt',
      headerName: 'Expires',
      width: 120,
      renderCell: ({ value }: GridRenderCellParams) =>
        value ? new Date(value as string).toLocaleDateString() : '—',
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 110,
      renderCell: ({ value }: GridRenderCellParams) => (
        <Chip label={value} size="small" color={STATUS_COLORS[value as string] ?? 'default'} />
      ),
    },
    {
      field: 'actions',
      headerName: '',
      width: 200,
      sortable: false,
      renderCell: ({ row }: GridRenderCellParams) =>
        row.status === 'Sent' ? (
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', height: '100%' }}>
            <Button
              size="small"
              color="success"
              variant="outlined"
              onClick={() => setDialogOffer({ id: row.id, response: 'Accepted' })}
            >
              Accept
            </Button>
            <Button
              size="small"
              color="error"
              variant="outlined"
              onClick={() => setDialogOffer({ id: row.id, response: 'Declined' })}
            >
              Decline
            </Button>
          </Stack>
        ) : null,
    },
  ]

  return (
    <Box>
      <PageHeader
        title="My Offers"
        subtitle="View and respond to job offers extended to you"
      />

      <DataTable
        rows={rows}
        columns={columns}
        loading={isLoading}
        getRowId={(r) => r.id}
        emptyTitle="No offers yet"
        emptyDescription="Keep applying! Offers will appear here when extended to you."
      />

      <Dialog open={Boolean(dialogOffer)} onClose={() => setDialogOffer(null)} maxWidth="xs" fullWidth>
        <DialogTitle>
          {dialogOffer?.response === 'Accepted' ? 'Accept Offer' : 'Decline Offer'}
        </DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            {dialogOffer?.response === 'Accepted'
              ? 'Are you sure you want to accept this offer? Leave a message for the recruiter (optional).'
              : 'Please let the recruiter know why you are declining (optional).'}
          </Typography>
          <TextField
            fullWidth
            multiline
            rows={3}
            label="Message (optional)"
            value={message}
            onChange={(e) => setMessage(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOffer(null)}>Cancel</Button>
          <Button
            variant="contained"
            color={dialogOffer?.response === 'Accepted' ? 'success' : 'error'}
            onClick={handleRespond}
            loading={respondToOffer.isPending}
          >
            {dialogOffer?.response === 'Accepted' ? 'Accept Offer' : 'Decline Offer'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default MyOffersPage
