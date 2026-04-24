import { useState } from 'react'
import {
  Box,
  Typography,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Avatar,
} from '@mui/material'
import type { GridColDef } from '@mui/x-data-grid'
import { useMyJobs } from '@/features/jobs/jobsApi'
import { useJobApplications, useUpdateApplicationStatus } from '@/features/applications/applicationsApi'
import { PageHeader, SectionCard, DataTable, StatusChip } from '@/components/ui'
import type { ApplicationStatus, JobSummary } from '@/types'
import { formatDate } from '@/utils/formatters'
import type { ApplicationDetail } from '@/features/applications/applicationsApi'

const STATUS_OPTIONS: ApplicationStatus[] = [
  'Applied', 'UnderReview', 'Shortlisted', 'Assessment', 'InterviewScheduled',
  'OfferExtended', 'Hired', 'Rejected',
]

const columns: GridColDef<ApplicationDetail>[] = [
  {
    field: 'candidateName',
    headerName: 'Candidate',
    flex: 1.5,
    minWidth: 160,
    renderCell: ({ row }) => (
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <Avatar sx={{ width: 28, height: 28, fontSize: 12, bgcolor: 'primary.main' }}>
          {row.candidateName[0]}
        </Avatar>
        <Box>
          <Typography variant="body2" sx={{ fontWeight: 500 }}>{row.candidateName}</Typography>
          <Typography variant="caption" color="text.secondary">{row.candidateEmail}</Typography>
        </Box>
      </Box>
    ),
  },
  {
    field: 'appliedAt',
    headerName: 'Applied',
    width: 120,
    renderCell: ({ value }) => <Typography variant="body2">{formatDate(value as string)}</Typography>,
  },
  {
    field: 'status',
    headerName: 'Status',
    width: 160,
    renderCell: ({ value }) => <StatusChip status={value as ApplicationStatus} type="application" />,
  },
]

export function ManageApplicationsPage() {
  const { data: jobs } = useMyJobs()
  const [selectedJobId, setSelectedJobId] = useState('')
  const { data: applications, isLoading } = useJobApplications(selectedJobId)
  const updateStatus = useUpdateApplicationStatus()

  const rows = applications?.items ?? []

  const cols: GridColDef<ApplicationDetail>[] = [
    ...columns,
    {
      field: 'actions',
      headerName: 'Change Status',
      width: 200,
      sortable: false,
      renderCell: ({ row }) => (
        <FormControl size="small" fullWidth>
          <Select
            value={row.status}
            onChange={(e) => updateStatus.mutate({ id: row.id, status: e.target.value as ApplicationStatus })}
          >
            {STATUS_OPTIONS.map((s) => <MenuItem key={s} value={s}>{s}</MenuItem>)}
          </Select>
        </FormControl>
      ),
    },
  ]

  return (
    <Box>
      <PageHeader title="Manage Applications" subtitle="Review and update the status of job applicants" />

      <SectionCard
        title="Filter by Job"
        sx={{ mb: 3 }}
      >
        <FormControl size="small" sx={{ minWidth: 320 }}>
          <InputLabel>Select a job</InputLabel>
          <Select
            label="Select a job"
            value={selectedJobId}
            onChange={(e) => setSelectedJobId(e.target.value)}
          >
            <MenuItem value="">All jobs</MenuItem>
            {jobs?.items.map((j: JobSummary) => (
              <MenuItem key={j.id} value={j.id}>{j.title}</MenuItem>
            ))}
          </Select>
        </FormControl>
      </SectionCard>

      <SectionCard title={`Applications${selectedJobId ? ` (${rows.length})` : ''}`} noPadding>
        <DataTable
          rows={rows}
          columns={cols}
          loading={isLoading}
          emptyTitle="No applications yet"
          emptyDescription="Once candidates apply to your jobs, they will appear here."
        />
      </SectionCard>
    </Box>
  )
}
