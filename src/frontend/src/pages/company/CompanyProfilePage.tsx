import { useState } from 'react'
import {
  Box, Grid, Paper, Typography, Stack, Button, Avatar, Dialog,
  DialogTitle, DialogContent, DialogActions, Chip, Alert, CircularProgress
} from '@mui/material'
import { Edit as EditIcon, Add as AddIcon, LocationOn, Language } from '@mui/icons-material'
import { useForm } from 'react-hook-form'
import { useMyCompany, useUpdateCompany, useCompanyBranches } from '@/features/company/companyApi'
import { PageHeader, SectionCard, FormTextField } from '@/components/ui'

function BranchCard({ branch }: { branch: any }) {
  return (
    <Paper variant="outlined" sx={{ p: 2 }}>
      <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>{branch.name}</Typography>
      <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center', mt: 0.5 }}>
        <LocationOn sx={{ fontSize: 14, color: 'text.secondary' }} />
        <Typography variant="body2" color="text.secondary">{branch.city}, {branch.country}</Typography>
      </Stack>
      {branch.isHeadquarters && (
        <Chip label="HQ" size="small" color="primary" sx={{ mt: 0.5 }} />
      )}
    </Paper>
  )
}

export default function CompanyProfilePage() {
  const { data: company, isLoading } = useMyCompany()
  const { data: branches = [] } = useCompanyBranches(company?.id ?? '')
  const updateCompany = useUpdateCompany()
  const [editOpen, setEditOpen] = useState(false)

  const { control, handleSubmit, reset } = useForm({
    defaultValues: {
      name: company?.name ?? '',
      description: company?.description ?? '',
      websiteUrl: company?.websiteUrl ?? '',
      companySize: company?.companySize ?? '',
      headquartersAddress: company?.headquartersAddress ?? '',
    },
  })

  const openEdit = () => {
    reset({
      name: company?.name ?? '',
      description: company?.description ?? '',
      websiteUrl: company?.websiteUrl ?? '',
      companySize: company?.companySize ?? '',
      headquartersAddress: company?.headquartersAddress ?? '',
    })
    setEditOpen(true)
  }

  const onSubmit = (values: any) => {
    if (!company?.id) return
    updateCompany.mutate({ id: company.id, ...values }, { onSuccess: () => setEditOpen(false) })
  }

  if (isLoading) {
    return <Box sx={{ display: 'flex', justifyContent: 'center', p: 8 }}><CircularProgress /></Box>
  }

  return (
    <Box>
      <PageHeader
        title="Company Profile"
        subtitle="Manage your organisation's public presence"
        actions={<Button variant="contained" startIcon={<EditIcon />} onClick={openEdit}>Edit Profile</Button>}
      />

      {!company ? (
        <Alert severity="info">
          No company profile yet. Complete your profile to attract top talent.
          <Button sx={{ ml: 2 }} variant="contained" onClick={() => setEditOpen(true)}>Create Profile</Button>
        </Alert>
      ) : (
        <Grid container spacing={3}>
          {/* Hero */}
          <Grid size={12}>
            <Paper sx={{ p: 3 }}>
              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={3} sx={{ alignItems: { xs: 'flex-start', sm: 'center' } }}>
                <Avatar
                  src={company.logoUrl}
                  sx={{ width: 80, height: 80, bgcolor: 'primary.main', fontSize: 32 }}
                >
                  {company.name?.[0]}
                </Avatar>
                <Box sx={{ flex: 1 }}>
                  <Typography variant="h4" sx={{ fontWeight: 700 }}>{company.name}</Typography>
                  <Stack direction="row" spacing={2} sx={{ mt: 1, flexWrap: 'wrap' }}>
                    {(company.industryName || company.industry) && <Chip label={company.industryName ?? company.industry} size="small" />}
                    {company.companySize && <Chip label={company.companySize} size="small" variant="outlined" />}
                    {(company.headquartersAddress || company.headquartersCity) && (
                      <Box sx={{ display: 'flex', gap: 0.5, alignItems: 'center' }}>
                        <LocationOn sx={{ fontSize: 16, color: 'text.secondary' }} />
                        <Typography variant="body2" color="text.secondary">{company.headquartersAddress ?? company.headquartersCity}</Typography>
                      </Box>
                    )}
                    {company.websiteUrl && (
                      <Box sx={{ display: 'flex', gap: 0.5, alignItems: 'center' }}>
                        <Language sx={{ fontSize: 16, color: 'text.secondary' }} />
                        <Typography
                          variant="body2"
                          component="a"
                          href={company.websiteUrl}
                          target="_blank"
                          rel="noopener noreferrer"
                          color="primary"
                        >
                          {company.websiteUrl}
                        </Typography>
                      </Box>
                    )}
                  </Stack>
                </Box>
                <Stack direction="row" spacing={2}>
                  <Box sx={{ textAlign: 'center' }}>
                    <Typography variant="h5" sx={{ fontWeight: 700 }}>{company.followersCount ?? 0}</Typography>
                    <Typography variant="caption" color="text.secondary">Followers</Typography>
                  </Box>
                  <Box sx={{ textAlign: 'center' }}>
                    <Typography variant="h5" sx={{ fontWeight: 700 }}>{company.activeJobCount ?? 0}</Typography>
                    <Typography variant="caption" color="text.secondary">Open Jobs</Typography>
                  </Box>
                </Stack>
              </Stack>
            </Paper>
          </Grid>

          {/* About */}
          <Grid size={{ xs: 12, md: 8 }}>
            <SectionCard title="About">
              {company.description ? (
                <Typography variant="body1" sx={{ lineHeight: 1.8 }}>{company.description}</Typography>
              ) : (
                <Typography color="text.secondary">No description added yet.</Typography>
              )}
            </SectionCard>
          </Grid>

          {/* Branches */}
          <Grid size={{ xs: 12, md: 4 }}>
            <SectionCard
              title="Office Locations"
              action={<Button size="small" startIcon={<AddIcon />}>Add</Button>}
            >
              <Stack spacing={1.5}>
                {(branches as any[]).length > 0 ? (
                  (branches as any[]).map((b) => <BranchCard key={b.id} branch={b} />)
                ) : (
                  <Typography color="text.secondary" variant="body2">No locations added yet.</Typography>
                )}
              </Stack>
            </SectionCard>
          </Grid>
        </Grid>
      )}

      {/* Edit Dialog */}
      <Dialog open={editOpen} onClose={() => setEditOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{company ? 'Edit Company Profile' : 'Create Company Profile'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <FormTextField name="name" label="Company Name" control={control} />
            <FormTextField
              name="description"
              label="About"
              control={control}
              multiline
              rows={4}
            />
            <FormTextField name="websiteUrl" label="Website URL" control={control} />
            <FormTextField name="companySize" label="Company Size" control={control} placeholder="e.g. 50-200" />
            <FormTextField name="headquartersAddress" label="Headquarters Address" control={control} />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSubmit(onSubmit)} disabled={updateCompany.isPending}>
            {updateCompany.isPending ? 'Saving…' : 'Save Profile'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
