import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Box, Button, Chip, Menu, MenuItem, Stack, Typography } from '@mui/material'
import { Add as AddIcon, MoreVert as MoreVertIcon } from '@mui/icons-material'
import type { GridColDef, GridRenderCellParams } from '@mui/x-data-grid'
import { useMyJobs, useUpdateJobStatus } from '@/features/jobs/jobsApi'
import { PageHeader, DataTable, SearchInput } from '@/components/ui'
import type { JobSummary, JobStatus } from '@/types'
import { formatDate } from '@/utils/formatters'

const STATUS_COLORS: Record<string, 'success' | 'warning' | 'error' | 'default' | 'info'> = {
  Published: 'success',
  Draft: 'default',
  Paused: 'warning',
  Closed: 'error',
  Expired: 'error',
}

function JobActions({ row }: { row: JobSummary }) {
  const [anchor, setAnchor] = useState<null | HTMLElement>(null)
  const navigate = useNavigate()
  const updateStatus = useUpdateJobStatus()

  const open = Boolean(anchor)

  const changeStatus = (status: JobStatus) => {
    updateStatus.mutate({ id: row.id, status })
    setAnchor(null)
  }

  return (
    <>
      <Button
        size="small"
        endIcon={<MoreVertIcon />}
        onClick={(e) => setAnchor(e.currentTarget)}
      >
        Actions
      </Button>
      <Menu anchorEl={anchor} open={open} onClose={() => setAnchor(null)}>
        <MenuItem onClick={() => { navigate(`/jobs/${row.id}`); setAnchor(null) }}>View</MenuItem>
        <MenuItem onClick={() => { navigate(`/post-job?edit=${row.id}`); setAnchor(null) }}>Edit</MenuItem>
        {row.status !== 'Published' && (
          <MenuItem onClick={() => changeStatus('Published')}>Publish</MenuItem>
        )}
        {row.status === 'Published' && (
          <MenuItem onClick={() => changeStatus('Paused')}>Pause</MenuItem>
        )}
        {row.status !== 'Closed' && (
          <MenuItem onClick={() => changeStatus('Closed')} sx={{ color: 'error.main' }}>Close</MenuItem>
        )}
      </Menu>
    </>
  )
}

const columns: GridColDef<JobSummary>[] = [
  {
    field: 'title',
    headerName: 'Job Title',
    flex: 2,
    minWidth: 200,
  },
  {
    field: 'companyName',
    headerName: 'Company',
    flex: 1,
    minWidth: 140,
  },
  {
    field: 'jobType',
    headerName: 'Type',
    width: 110,
    renderCell: ({ value }: GridRenderCellParams) => (
      <Chip label={value} size="small" variant="outlined" />
    ),
  },
  {
    field: 'status',
    headerName: 'Status',
    width: 110,
    renderCell: ({ value }: GridRenderCellParams) => (
      <Chip
        label={value}
        size="small"
        color={STATUS_COLORS[value as string] ?? 'default'}
      />
    ),
  },
  {
    field: 'applicationsCount',
    headerName: 'Applications',
    width: 120,
    type: 'number',
    renderCell: ({ value }: GridRenderCellParams) => (
      <Typography variant="body2" sx={{ fontWeight: 600 }}>{value ?? 0}</Typography>
    ),
  },
  {
    field: 'openingsCount',
    headerName: 'Openings',
    width: 100,
    type: 'number',
  },
  {
    field: 'expiresAt',
    headerName: 'Expires',
    width: 110,
    renderCell: ({ value }: GridRenderCellParams) =>
      value ? (
        <Typography variant="body2">{formatDate(value as string)}</Typography>
      ) : (
        <Typography variant="body2" color="text.secondary">—</Typography>
      ),
  },
  {
    field: 'actions',
    headerName: '',
    width: 130,
    sortable: false,
    renderCell: ({ row }: GridRenderCellParams<JobSummary>) => <JobActions row={row} />,
  },
]

export function ManageJobsPage() {
  const navigate = useNavigate()
  const [search, setSearch] = useState('')
  const [statusFilter, setStatusFilter] = useState<string>('')

  const { data, isLoading } = useMyJobs()
  const rows: JobSummary[] = (data as any)?.items ?? data ?? []

  const filtered = rows.filter((j) => {
    const matchesSearch = !search || j.title.toLowerCase().includes(search.toLowerCase())
    const matchesStatus = !statusFilter || j.status === statusFilter
    return matchesSearch && matchesStatus
  })

  const statusCounts = {
    total: rows.length,
    published: rows.filter((j) => j.status === 'Published').length,
    draft: rows.filter((j) => j.status === 'Draft').length,
    paused: rows.filter((j) => j.status === 'Paused').length,
  }

  return (
    <Box>
      <PageHeader
        title="My Job Postings"
        subtitle="Manage all job postings you've created"
        actions={
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate('/post-job')}>
            Post New Job
          </Button>
        }
      />

      {/* Summary chips */}
      <Stack direction="row" spacing={1} sx={{ mb: 2 }}>
        <Chip label={`Total: ${statusCounts.total}`} variant="outlined" onClick={() => setStatusFilter('')} color={!statusFilter ? 'primary' : 'default'} />
        <Chip label={`Published: ${statusCounts.published}`} variant="outlined" onClick={() => setStatusFilter('Published')} color={statusFilter === 'Published' ? 'success' : 'default'} />
        <Chip label={`Draft: ${statusCounts.draft}`} variant="outlined" onClick={() => setStatusFilter('Draft')} color={statusFilter === 'Draft' ? 'primary' : 'default'} />
        <Chip label={`Paused: ${statusCounts.paused}`} variant="outlined" onClick={() => setStatusFilter('Paused')} color={statusFilter === 'Paused' ? 'warning' : 'default'} />
      </Stack>

      <SearchInput
        value={search}
        onChange={setSearch}
        placeholder="Search by job title..."
        sx={{ mb: 2, maxWidth: 360 }}
      />

      <DataTable
        rows={filtered}
        columns={columns}
        loading={isLoading}
        getRowId={(r) => r.id}
        emptyTitle="No job postings found"
        emptyDescription="Post your first job to get started!"
      />
    </Box>
  )
}

export default ManageJobsPage
