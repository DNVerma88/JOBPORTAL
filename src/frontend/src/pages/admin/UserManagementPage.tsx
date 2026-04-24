import { useState } from 'react'
import {
  Box, Button, Dialog, DialogTitle, DialogContent, DialogActions,
  FormControl, InputLabel, Select, MenuItem, Stack, Chip
} from '@mui/material'
import { Add as AddIcon, LockReset } from '@mui/icons-material'
import type { GridColDef } from '@mui/x-data-grid'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useUsers, useCreateUser, useToggleUserStatus, useResetUserPassword } from '@/features/admin/adminApi'
import type { User } from '@/features/admin/adminApi'
import { PageHeader, DataTable, StatusChip, SearchInput } from '@/components/ui'
import { FormTextField, FormSelect } from '@/components/ui'

const inviteSchema = z.object({
  firstName: z.string().min(1, 'Required'),
  lastName: z.string().min(1, 'Required'),
  email: z.string().email('Invalid email'),
  roleId: z.string().min(1, 'Required'),
  password: z.string().min(8, 'Min 8 characters'),
})
type InviteForm = z.infer<typeof inviteSchema>

export default function UserManagementPage() {
  const [search, setSearch] = useState('')
  const [roleFilter, setRoleFilter] = useState('')
  const [activeFilter, setActiveFilter] = useState<boolean | undefined>()
  const [open, setOpen] = useState(false)

  const { data, isLoading } = useUsers({
    search: search || undefined,
    role: roleFilter || undefined,
    isActive: activeFilter,
  })

  const rows = Array.isArray(data) ? data : (data as any)?.items ?? []

  const createUser = useCreateUser()
  const toggleStatus = useToggleUserStatus()
  const resetPw = useResetUserPassword()

  const { control, handleSubmit, reset } = useForm<InviteForm>({
    resolver: zodResolver(inviteSchema),
    defaultValues: { firstName: '', lastName: '', email: '', roleId: '', password: '' },
  })

  const onSubmit = (values: InviteForm) => {
    createUser.mutate(values, { onSuccess: () => { reset(); setOpen(false) } })
  }

  const columns: GridColDef<User>[] = [
    {
      field: 'firstName', headerName: 'Name', flex: 1, minWidth: 160,
      renderCell: p => `${p.row.firstName} ${p.row.lastName}`,
    },
    { field: 'email', headerName: 'Email', flex: 1, minWidth: 200 },
    {
      field: 'role', headerName: 'Role', width: 140,
      renderCell: p => <Chip label={p.value} size="small" color="primary" variant="outlined" />,
    },
    {
      field: 'isActive', headerName: 'Status', width: 110,
      renderCell: p => <StatusChip status={p.value ? 'Active' : 'Inactive'} />,
    },
    {
      field: 'lastLoginOn', headerName: 'Last Login', width: 120,
      renderCell: p => p.value ? new Date(p.value).toLocaleDateString() : 'Never',
    },
    {
      field: 'actions', headerName: 'Actions', width: 200, sortable: false,
      renderCell: p => (
        <Box sx={{ display: 'flex', gap: 0.5, alignItems: 'center', height: '100%' }}>
          <Button
            size="small"
            color={p.row.isActive ? 'warning' : 'success'}
            onClick={() => toggleStatus.mutate({ id: p.row.id, suspend: p.row.isActive })}
          >
            {p.row.isActive ? 'Suspend' : 'Activate'}
          </Button>
          <Button size="small" color="secondary" startIcon={<LockReset />} onClick={() => resetPw.mutate(p.row.id)}>
            Reset
          </Button>
        </Box>
      ),
    },
  ]

  return (
    <Box>
      <PageHeader
        title="User Management"
        subtitle="Manage platform users and their roles"
        actions={<Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpen(true)}>Invite User</Button>}
      />

      <Box sx={{ display: 'flex', gap: 2, mb: 2, flexWrap: 'wrap' }}>
        <SearchInput value={search} onChange={setSearch} placeholder="Search by name or email" />
        <FormControl size="small" sx={{ minWidth: 160 }}>
          <InputLabel>Role</InputLabel>
          <Select value={roleFilter} label="Role" onChange={e => setRoleFilter(e.target.value)}>
            <MenuItem value="">All Roles</MenuItem>
            <MenuItem value="SuperAdmin">Super Admin</MenuItem>
            <MenuItem value="TenantAdmin">Tenant Admin</MenuItem>
            <MenuItem value="Recruiter">Recruiter</MenuItem>
            <MenuItem value="HiringManager">Hiring Manager</MenuItem>
            <MenuItem value="JobSeeker">Job Seeker</MenuItem>
          </Select>
        </FormControl>
        <FormControl size="small" sx={{ minWidth: 140 }}>
          <InputLabel>Status</InputLabel>
          <Select
            value={activeFilter === undefined ? '' : String(activeFilter)}
            label="Status"
            onChange={e => {
              const v = e.target.value
              setActiveFilter(v === '' ? undefined : v === 'true')
            }}
          >
            <MenuItem value="">All</MenuItem>
            <MenuItem value="true">Active</MenuItem>
            <MenuItem value="false">Inactive</MenuItem>
          </Select>
        </FormControl>
      </Box>

      <DataTable columns={columns} rows={rows} loading={isLoading} />

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Invite User</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <Box sx={{ display: 'flex', gap: 2 }}>
              <FormTextField name="firstName" label="First Name" control={control} />
              <FormTextField name="lastName" label="Last Name" control={control} />
            </Box>
            <FormTextField name="email" label="Email Address" control={control} type="email" />
            <FormTextField name="password" label="Temporary Password" control={control} type="password" />
            <FormSelect
              name="roleId"
              label="Role"
              control={control}
              options={[
                { value: 'tenant-admin', label: 'Tenant Admin' },
                { value: 'recruiter', label: 'Recruiter' },
                { value: 'hiring-manager', label: 'Hiring Manager' },
                { value: 'jobseeker', label: 'Job Seeker' },
              ]}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => { reset(); setOpen(false) }}>Cancel</Button>
          <Button variant="contained" onClick={handleSubmit(onSubmit)} disabled={createUser.isPending}>
            {createUser.isPending ? 'Inviting...' : 'Send Invite'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
