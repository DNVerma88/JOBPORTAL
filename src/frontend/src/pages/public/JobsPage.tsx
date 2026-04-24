import { useState } from 'react'
import { Box, Grid, FormControl, InputLabel, Select, MenuItem, Button, Pagination } from '@mui/material'
import { TuneOutlined, WorkOutlined } from '@mui/icons-material'
import { useJobs } from '@/features/jobs/jobsApi'
import { JobCard } from '@/features/jobs/components/JobCard'
import { PageHeader, SearchInput, EmptyState, LoadingSpinner } from '@/components/ui'
import { JOB_TYPE_OPTIONS, WORK_MODE_OPTIONS, EXP_LEVEL_OPTIONS } from '@/utils/constants'
import { useDebounce } from '@/hooks/useDebounce'
import type { JobSearchParams } from '@/types'

const INITIAL_PARAMS: JobSearchParams = { pageNumber: 1, pageSize: 12 }

export function JobsPage() {
  const [keyword, setKeyword] = useState('')
  const [params, setParams] = useState<JobSearchParams>(INITIAL_PARAMS)
  const debouncedKeyword = useDebounce(keyword)

  const effectiveParams: JobSearchParams = {
    ...params,
    keyword: debouncedKeyword || undefined,
  }

  const { data, isLoading } = useJobs(effectiveParams)

  const handleFilter = (key: keyof JobSearchParams, value: string) => {
    setParams((p) => ({ ...p, [key]: value || undefined, pageNumber: 1 }))
  }

  const handleReset = () => {
    setKeyword('')
    setParams(INITIAL_PARAMS)
  }

  return (
    <Box>
      <PageHeader title="Find Jobs" subtitle={data ? `${data.totalCount.toLocaleString()} jobs available` : 'Browse all opportunities'} />

      {/* Filters */}
      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1.5, mb: 3, alignItems: 'center' }}>
        <SearchInput
          value={keyword}
          onChange={setKeyword}
          placeholder="Search job title, skills…"
          sx={{ flex: { xs: '1 1 100%', sm: '1 1 280px' } }}
        />
        <FormControl size="small" sx={{ minWidth: 140 }}>
          <InputLabel>Job Type</InputLabel>
          <Select label="Job Type" value={params.jobType ?? ''} onChange={(e) => handleFilter('jobType', e.target.value)}>
            <MenuItem value="">All</MenuItem>
            {JOB_TYPE_OPTIONS.map((o) => <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>)}
          </Select>
        </FormControl>
        <FormControl size="small" sx={{ minWidth: 140 }}>
          <InputLabel>Work Mode</InputLabel>
          <Select label="Work Mode" value={params.workMode ?? ''} onChange={(e) => handleFilter('workMode', e.target.value)}>
            <MenuItem value="">All</MenuItem>
            {WORK_MODE_OPTIONS.map((o) => <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>)}
          </Select>
        </FormControl>
        <FormControl size="small" sx={{ minWidth: 160 }}>
          <InputLabel>Experience</InputLabel>
          <Select label="Experience" value={params.experienceLevel ?? ''} onChange={(e) => handleFilter('experienceLevel', e.target.value)}>
            <MenuItem value="">All</MenuItem>
            {EXP_LEVEL_OPTIONS.map((o) => <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>)}
          </Select>
        </FormControl>
        <Button startIcon={<TuneOutlined />} size="small" onClick={handleReset} color="inherit">
          Reset
        </Button>
      </Box>

      {/* Results */}
      {isLoading ? (
        <LoadingSpinner />
      ) : !data?.items.length ? (
        <EmptyState
          icon={<WorkOutlined />}
          title="No jobs found"
          description="Try adjusting your filters or search terms."
        />
      ) : (
        <>
          <Grid container spacing={2.5} sx={{ mb: 3 }}>
            {data.items.map((job: import('@/types').JobSummary) => (
              <Grid key={job.id} size={{ xs: 12, sm: 6, md: 4 }}>
                <JobCard job={job} />
              </Grid>
            ))}
          </Grid>

          {data.totalPages > 1 && (
            <Box sx={{ display: 'flex', justifyContent: 'center' }}>
              <Pagination
                count={data.totalPages}
                page={params.pageNumber ?? 1}
                onChange={(_, p) => setParams((prev) => ({ ...prev, pageNumber: p }))}
                color="primary"
              />
            </Box>
          )}
        </>
      )}
    </Box>
  )
}
