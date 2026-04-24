import { useState } from 'react'
import {
  Box, Grid, Paper, Typography, Stack, Avatar, Chip, Rating,
  CircularProgress, Alert, InputAdornment, TextField
} from '@mui/material'
import { Search, LocationOn, People, Work } from '@mui/icons-material'
import { useCompanies } from '@/features/company/companyApi'
import { PageHeader } from '@/components/ui'
import { useNavigate } from 'react-router-dom'

function CompanyCard({ company }: { company: any }) {
  const navigate = useNavigate()

  return (
    <Paper
      sx={{ p: 2.5, cursor: 'pointer', transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 4 } }}
      onClick={() => navigate(`/companies/${company.id}`)}
    >
      <Stack spacing={2}>
        <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
          <Avatar
            src={company.logoUrl}
            sx={{ width: 56, height: 56, bgcolor: 'primary.main', fontSize: 22, fontWeight: 700 }}
          >
            {company.name?.[0]}
          </Avatar>
          <Box sx={{ flex: 1, minWidth: 0 }}>
            <Typography variant="subtitle1" sx={{ fontWeight: 700 }} noWrap>{company.name}</Typography>
            {company.industry && (
              <Chip label={company.industry} size="small" sx={{ mt: 0.25 }} />
            )}
          </Box>
        </Stack>

        {company.description && (
          <Typography variant="body2" color="text.secondary" sx={{
            display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden',
          }}>
            {company.description}
          </Typography>
        )}

        <Stack direction="row" spacing={2} sx={{ flexWrap: 'wrap' }}>
          {company.location && (
            <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
              <LocationOn sx={{ fontSize: 14, color: 'text.secondary' }} />
              <Typography variant="caption" color="text.secondary">{company.location}</Typography>
            </Stack>
          )}
          {company.size && (
            <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
              <People sx={{ fontSize: 14, color: 'text.secondary' }} />
              <Typography variant="caption" color="text.secondary">{company.size}</Typography>
            </Stack>
          )}
          {company.activeJobCount != null && (
            <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
              <Work sx={{ fontSize: 14, color: 'text.secondary' }} />
              <Typography variant="caption" color="text.secondary">{company.activeJobCount} open jobs</Typography>
            </Stack>
          )}
        </Stack>

        {company.averageRating != null && (
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <Rating value={company.averageRating} precision={0.5} readOnly size="small" />
            <Typography variant="caption" color="text.secondary">({company.reviewCount ?? 0})</Typography>
          </Stack>
        )}
      </Stack>
    </Paper>
  )
}

export default function BrowseCompaniesPage() {
  const [search, setSearch] = useState('')
  const { data: companiesResult, isLoading } = useCompanies()
  const companies = companiesResult?.items ?? []

  const filtered = companies.filter(c =>
    !search || c.name.toLowerCase().includes(search.toLowerCase()) ||
    c.industry?.toLowerCase().includes(search.toLowerCase()) ||
    [c.headquartersCity, c.headquartersAddress].filter(Boolean).join(' ').toLowerCase().includes(search.toLowerCase())
  )

  return (
    <Box>
      <PageHeader title="Companies" subtitle="Explore organisations and find your ideal workplace" />

      <TextField
        fullWidth
        placeholder="Search by company name, industry or location…"
        value={search}
        onChange={e => setSearch(e.target.value)}
        sx={{ mb: 3 }}
        slotProps={{
          input: {
            startAdornment: (
              <InputAdornment position="start">
                <Search />
              </InputAdornment>
            ),
          },
        }}
      />

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 8 }}><CircularProgress /></Box>
      ) : filtered.length === 0 ? (
        <Alert severity="info">
          {search ? `No companies found matching "${search}"` : 'No companies listed yet.'}
        </Alert>
      ) : (
        <>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            {filtered.length} {filtered.length === 1 ? 'company' : 'companies'} found
          </Typography>
          <Grid container spacing={2}>
            {filtered.map((company: any) => (
              <Grid size={{ xs: 12, sm: 6, md: 4 }} key={company.id}>
                <CompanyCard company={company} />
              </Grid>
            ))}
          </Grid>
        </>
      )}
    </Box>
  )
}
