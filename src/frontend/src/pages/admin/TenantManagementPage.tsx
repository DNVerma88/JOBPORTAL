import { useState } from 'react'
import {
  Box, Button, Dialog, DialogTitle, DialogContent, DialogActions,
  Stack, Chip
} from '@mui/material'
import { Add as AddIcon } from '@mui/icons-material'
import type { GridColDef } from '@mui/x-data-grid'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useTenants, useCreateTenant, useToggleTenantStatus } from '@/features/admin/adminApi'
import type { Tenant } from '@/features/admin/adminApi'
import { PageHeader, DataTable, StatusChip, SearchInput, FormTextField } from '@/components/ui'

const tenantSchema = z.object({
  name: z.string().min(1, 'Required'),
  slug: z.string().min(2, 'Min 2 chars').regex(/^[a-z0-9-]+$/, 'Only lowercase letters, numbers, hyphens'),
  contactEmail: z.string().email('Invalid email'),
})
type TenantForm = z.infer<typeof tenantSchema>

export default function TenantManagementPage() {
  const [search, setSearch] = useState('')
  const [open, setOpen] = useState(false)

  const { data, isLoading } = useTenants()
  const createTenant = useCreateTenant()
  const toggleStatus = useToggleTenantStatus()

  const { control, handleSubmit, reset } = useForm<TenantForm>({
    resolver: zodResolver(tenantSchema),
    defaultValues: { name: '', slug: '', contactEmail: '' },
  })

  const allRows = Array.isArray(data) ? data : (data as any)?.items ?? []
  const filtered = allRows.filter((t: Tenant) =>
    !search || t.name.toLowerCase().includes(search.toLowerCase()) || t.slug.includes(search.toLowerCase())
  )

  const onSubmit = (values: TenantForm) => {
    createTenant.mutate(values, { onSuccess: () => { reset(); setOpen(false) } })
  }

  const columns: GridColDef<Tenant>[] = [
    { field: 'name', headerName: 'Tenant Name', flex: 1, minWidth: 160 },
    {
      field: 'slug', headerName: 'Slug', width: 140,
      renderCell: p => <Chip label={p.value} size="small" variant="outlined" />,
    },
    { field: 'planName', headerName: 'Plan', width: 120, renderCell: p => p.value ?? '—' },
    { field: 'userCount', headerName: 'Users', width: 80, renderCell: p => p.value ?? 0 },
    { field: 'jobCount', headerName: 'Jobs', width: 80, renderCell: p => p.value ?? 0 },
    {
      field: 'isActive', headerName: 'Status', width: 110,
      renderCell: p => <StatusChip status={p.value ? 'Active' : 'Inactive'} />,
    },
    {
      field: 'createdOn', headerName: 'Created', width: 110,
      renderCell: p => new Date(p.value).toLocaleDateString(),
    },
    {
      field: 'actions', headerName: 'Actions', width: 130, sortable: false,
      renderCell: p => (
        <Button
          size="small"
          color={p.row.isActive ? 'warning' : 'success'}
          onClick={() => toggleStatus.mutate({ id: p.row.id, active: !p.row.isActive })}
        >
          {p.row.isActive ? 'Deactivate' : 'Activate'}
        </Button>
      ),
    },
  ]

  return (
    <Box>
      <PageHeader
        title="Tenant Management"
        subtitle="Manage organisations using this platform"
        actions={<Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpen(true)}>New Tenant</Button>}
      />

      <Box sx={{ mb: 2 }}>
        <SearchInput value={search} onChange={setSearch} placeholder="Search tenants..." />
      </Box>

      <DataTable columns={columns} rows={filtered} loading={isLoading} />

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create New Tenant</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <FormTextField name="name" label="Organisation Name" control={control} />
            <FormTextField name="slug" label="URL Slug" control={control} />
            <FormTextField name="contactEmail" label="Contact Email" control={control} type="email" />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => { reset(); setOpen(false) }}>Cancel</Button>
          <Button variant="contained" onClick={handleSubmit(onSubmit)} disabled={createTenant.isPending}>
            {createTenant.isPending ? 'Creating...' : 'Create Tenant'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

