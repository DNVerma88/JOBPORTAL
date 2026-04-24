import { useState } from 'react'
import {
  Box, Chip, Avatar, Stack, Typography, Button,
  Dialog, DialogTitle, DialogContent, DialogActions,
  LinearProgress, Tooltip,
} from '@mui/material'
import {
  WorkOutlined,
  OpenInNewOutlined,
  DescriptionOutlined,
} from '@mui/icons-material'
import type { GridColDef } from '@mui/x-data-grid'
import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/api/apiClient'
import type { PagedResult } from '@/types'
import { PageHeader, DataTable, SearchInput } from '@/components/ui'

// ── Types ──────────────────────────────────────────────────────

interface CandidateSummary {
  profileId: string
  userId: string
  fullName: string
  headline?: string
  currentJobTitle?: string
  currentCompany?: string
  totalExperienceYears?: number
  isOpenToWork: boolean
  profileCompletionPct: number
  resumeUrl?: string
}

// ── Hook ───────────────────────────────────────────────────────

function useCandidates(search: string, openToWork?: boolean) {
  return useQuery({
    queryKey: ['candidates', search, openToWork],
    queryFn: async () => {
      const params = new URLSearchParams({ pageNumber: '1', pageSize: '50' })
      if (search) params.set('search', search)
      if (openToWork !== undefined) params.set('isOpenToWork', String(openToWork))
      const { data } = await apiClient.get<PagedResult<CandidateSummary>>(`/candidates?${params}`)
      if (Array.isArray(data)) return data
      if (data && Array.isArray((data as any).items)) return (data as any).items as CandidateSummary[]
      return [] as CandidateSummary[]
    },
  })
}

// ── Page ───────────────────────────────────────────────────────

export default function CandidatesPage() {
  const [search, setSearch] = useState('')
  const [openToWorkFilter, setOpenToWorkFilter] = useState<boolean | undefined>(undefined)
  const [selected, setSelected] = useState<CandidateSummary | null>(null)

  const { data: candidates = [], isLoading } = useCandidates(search, openToWorkFilter)

  const columns: GridColDef<CandidateSummary>[] = [
    {
      field: 'fullName',
      headerName: 'Candidate',
      flex: 1,
      minWidth: 180,
      renderCell: (p) => (
        <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center', height: '100%' }}>
          <Avatar sx={{ width: 32, height: 32, fontSize: 13 }}>
            {p.value?.charAt(0)}
          </Avatar>
          <Box>
            <Typography variant="body2" sx={{ fontWeight: 600, lineHeight: 1.2 }}>{p.value}</Typography>
            {p.row.headline && (
              <Typography variant="caption" color="text.secondary" noWrap>{p.row.headline}</Typography>
            )}
          </Box>
        </Stack>
      ),
    },
    {
      field: 'currentJobTitle',
      headerName: 'Current Role',
      flex: 1,
      minWidth: 160,
      renderCell: (p) => (
        <Box>
          <Typography variant="body2">{p.value ?? '—'}</Typography>
          {p.row.currentCompany && (
            <Typography variant="caption" color="text.secondary">{p.row.currentCompany}</Typography>
          )}
        </Box>
      ),
    },
    {
      field: 'totalExperienceYears',
      headerName: 'Experience',
      width: 110,
      renderCell: (p) => p.value != null ? `${p.value} yr${p.value === 1 ? '' : 's'}` : '—',
    },
    {
      field: 'profileCompletionPct',
      headerName: 'Profile',
      width: 130,
      renderCell: (p) => (
        <Box sx={{ width: '100%' }}>
          <Typography variant="caption" sx={{ mb: 0.25, display: 'block' }}>{p.value}%</Typography>
          <LinearProgress variant="determinate" value={p.value} sx={{ borderRadius: 1, height: 5 }} />
        </Box>
      ),
    },
    {
      field: 'isOpenToWork',
      headerName: 'Status',
      width: 140,
      renderCell: (p) => p.value
        ? <Chip label="Open to work" color="success" size="small" />
        : <Chip label="Not looking" size="small" variant="outlined" />,
    },
    {
      field: 'actions',
      headerName: '',
      width: 110,
      sortable: false,
      renderCell: (p) => (
        <Stack direction="row" spacing={0.5}>
          <Tooltip title="View profile">
            <Button size="small" onClick={() => setSelected(p.row)}>
              <OpenInNewOutlined fontSize="small" />
            </Button>
          </Tooltip>
          {p.row.resumeUrl && (
            <Tooltip title="Download resume">
              <Button size="small" href={p.row.resumeUrl} target="_blank" rel="noopener noreferrer">
                <DescriptionOutlined fontSize="small" />
              </Button>
            </Tooltip>
          )}
        </Stack>
      ),
    },
  ]

  return (
    <Box>
      <PageHeader
        title="Candidates"
        subtitle="Browse job seekers who are open to new opportunities"
      />

      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }}>
        <SearchInput value={search} onChange={setSearch} placeholder="Search by name, title, or company…" />
        <Stack direction="row" spacing={1} sx={{ flexShrink: 0 }}>
          <Chip
            label="All"
            onClick={() => setOpenToWorkFilter(undefined)}
            color={openToWorkFilter === undefined ? 'primary' : 'default'}
            variant={openToWorkFilter === undefined ? 'filled' : 'outlined'}
            clickable
          />
          <Chip
            label="Open to work"
            icon={<WorkOutlined />}
            onClick={() => setOpenToWorkFilter(true)}
            color={openToWorkFilter === true ? 'success' : 'default'}
            variant={openToWorkFilter === true ? 'filled' : 'outlined'}
            clickable
          />
        </Stack>
      </Stack>

      <DataTable
        columns={columns}
        rows={candidates}
        loading={isLoading}
        getRowId={(r) => r.profileId}
        rowHeight={64}
      />

      {/* Candidate quick-view dialog */}
      <Dialog open={!!selected} onClose={() => setSelected(null)} maxWidth="sm" fullWidth>
        <DialogTitle>
          <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
            <Avatar sx={{ width: 48, height: 48 }}>{selected?.fullName?.charAt(0)}</Avatar>
            <Box>
              <Typography variant="h6">{selected?.fullName}</Typography>
              <Typography variant="body2" color="text.secondary">{selected?.headline ?? selected?.currentJobTitle ?? '—'}</Typography>
            </Box>
          </Stack>
        </DialogTitle>
        <DialogContent dividers>
          <Stack spacing={1.5}>
            {selected?.currentJobTitle && (
              <Typography variant="body2"><strong>Current role:</strong> {selected.currentJobTitle}{selected.currentCompany ? ` at ${selected.currentCompany}` : ''}</Typography>
            )}
            {selected?.totalExperienceYears != null && (
              <Typography variant="body2"><strong>Experience:</strong> {selected.totalExperienceYears} year{selected.totalExperienceYears === 1 ? '' : 's'}</Typography>
            )}
            <Typography variant="body2">
              <strong>Status:</strong>{' '}
              {selected?.isOpenToWork ? <Chip label="Open to work" color="success" size="small" /> : <Chip label="Not looking" size="small" variant="outlined" />}
            </Typography>
            <Typography variant="body2"><strong>Profile completion:</strong> {selected?.profileCompletionPct}%</Typography>
          </Stack>
        </DialogContent>
        <DialogActions>
          {selected?.resumeUrl && (
            <Button href={selected.resumeUrl} target="_blank" rel="noopener noreferrer" startIcon={<DescriptionOutlined />}>
              Download Resume
            </Button>
          )}
          <Button onClick={() => setSelected(null)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
