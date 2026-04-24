import { useParams, useNavigate } from 'react-router-dom'
import {
  Box, Avatar, Button, Chip, CircularProgress, Divider,
  Grid, Paper, Stack, Typography, Alert,
} from '@mui/material'
import {
  Language as WebIcon, People as PeopleIcon,
  Verified as VerifiedIcon, Work as WorkIcon,
  ArrowBack as BackIcon,
} from '@mui/icons-material'
import { useCompanyById } from '@/features/company/companyApi'
import { SectionCard } from '@/components/ui'

export function CompanyDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data: company, isLoading, isError } = useCompanyById(id ?? '')

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 8 }}>
        <CircularProgress />
      </Box>
    )
  }

  if (isError || !company) {
    return (
      <Box>
        <Button startIcon={<BackIcon />} onClick={() => navigate(-1)} sx={{ mb: 2 }}>
          Back
        </Button>
        <Alert severity="error">Company not found or could not be loaded.</Alert>
      </Box>
    )
  }

  return (
    <Box>
      <Button startIcon={<BackIcon />} onClick={() => navigate(-1)} sx={{ mb: 2 }}>
        Back to Companies
      </Button>

      {/* Header card */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={3} sx={{ alignItems: { sm: 'center' } }}>
          <Avatar
            src={(company as any).logoUrl}
            alt={company.name}
            variant="rounded"
            sx={{ width: 88, height: 88, fontSize: 32 }}
          >
            {company.name?.[0]}
          </Avatar>

          <Box sx={{ flex: 1 }}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap', mb: 0.5 }}>
              <Typography variant="h5" sx={{ fontWeight: 700 }}>
                {company.name}
              </Typography>
              {company.isVerified && (
                <VerifiedIcon color="primary" fontSize="small" titleAccess="Verified company" />
              )}
            </Stack>

            <Stack direction="row" spacing={2} sx={{ flexWrap: 'wrap', mb: 1 }}>
              {company.companySize && (
                <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
                  <PeopleIcon sx={{ fontSize: 16, color: 'text.secondary' }} />
                  <Typography variant="body2" color="text.secondary">{company.companySize} employees</Typography>
                </Stack>
              )}
              {(company as any).founded && (
                <Typography variant="body2" color="text.secondary">
                  Founded {(company as any).founded}
                </Typography>
              )}
              {company.websiteUrl && (
                <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
                  <WebIcon sx={{ fontSize: 16, color: 'text.secondary' }} />
                  <Typography
                    variant="body2"
                    component="a"
                    href={company.websiteUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    sx={{ color: 'primary.main', textDecoration: 'none', '&:hover': { textDecoration: 'underline' } }}
                  >
                    {company.websiteUrl.replace(/^https?:\/\//, '')}
                  </Typography>
                </Stack>
              )}
            </Stack>
          </Box>

          <Button
            variant="contained"
            startIcon={<WorkIcon />}
            onClick={() => navigate(`/jobs?companyId=${company.id}`)}
          >
            View Jobs
          </Button>
        </Stack>
      </Paper>

      <Grid container spacing={3}>
        {/* About */}
        {company.description && (
          <Grid size={{ xs: 12, md: 8 }}>
            <SectionCard title="About">
              <Typography variant="body2" sx={{ whiteSpace: 'pre-line', lineHeight: 1.7 }}>
                {company.description}
              </Typography>
            </SectionCard>
          </Grid>
        )}

        {/* Details sidebar */}
        <Grid size={{ xs: 12, md: company.description ? 4 : 12 }}>
          <SectionCard title="Company Details">
            <Stack spacing={2} divider={<Divider />}>
              {company.industry && (
                <Box>
                  <Typography variant="caption" color="text.secondary">Industry</Typography>
                  <Typography variant="body2" sx={{ fontWeight: 500 }}>{company.industry}</Typography>
                </Box>
              )}
              {company.companySize && (
                <Box>
                  <Typography variant="caption" color="text.secondary">Company Size</Typography>
                  <Typography variant="body2" sx={{ fontWeight: 500 }}>{company.companySize} employees</Typography>
                </Box>
              )}
              {company.isVerified && (
                <Box>
                  <Chip icon={<VerifiedIcon />} label="Verified Company" color="primary" size="small" />
                </Box>
              )}
            </Stack>
          </SectionCard>
        </Grid>
      </Grid>
    </Box>
  )
}

export default CompanyDetailPage
