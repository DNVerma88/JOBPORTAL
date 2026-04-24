import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm, Controller } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import {
  Box,
  Grid,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  InputAdornment,
  FormControlLabel,
  Switch,
  Typography,
  Autocomplete,
} from '@mui/material'
import { useCreateJob } from '@/features/jobs/jobsApi'
import { useJobCategories } from '@/features/config/configApi'
import { PageHeader, SectionCard, Button } from '@/components/ui'
import { JOB_TYPE_OPTIONS, WORK_MODE_OPTIONS, EXP_LEVEL_OPTIONS } from '@/utils/constants'
import type { CreateJobRequest } from '@/features/jobs/jobsApi'

const schema = z.object({
  title: z.string().min(3, 'Title is required'),
  description: z.string().min(50, 'Description must be at least 50 characters'),
  requirements: z.string().min(20, 'Requirements are required'),
  responsibilities: z.string().optional(),
  benefits: z.string().optional(),
  categoryId: z.string().min(1, 'Category is required'),
  jobType: z.string().min(1, 'Job type is required'),
  workMode: z.string().min(1, 'Work mode is required'),
  experienceLevel: z.string().min(1, 'Experience level is required'),
  location: z.string().min(2, 'Location is required'),
  salaryMin: z.number().optional(),
  salaryMax: z.number().optional(),
  currency: z.string().optional(),
  isSalaryHidden: z.boolean().optional(),
  skills: z.array(z.string()).min(1, 'Add at least one skill'),
  expiresAt: z.string().min(1, 'Expiry date is required'),
})

type FormData = z.infer<typeof schema>

const POPULAR_SKILLS = [
  'JavaScript', 'TypeScript', 'React', 'Node.js', 'Python', 'Java', 'C#', '.NET',
  'SQL', 'PostgreSQL', 'MongoDB', 'Docker', 'Kubernetes', 'AWS', 'Azure', 'CI/CD',
  'REST API', 'GraphQL', 'Git', 'Agile', 'Scrum',
]

export function PostJobPage() {
  const navigate = useNavigate()
  const createJob = useCreateJob()
  const { data: categories = [] } = useJobCategories()
  const [isSalaryHidden, setIsSalaryHidden] = useState(false)

  const { control, register, handleSubmit, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      currency: 'USD',
      isSalaryHidden: false,
      skills: [],
    },
  })

  const onSubmit = (data: FormData) => {
    const payload: CreateJobRequest = {
      ...data,
      isSalaryHidden,
    }
    createJob.mutate(payload, {
      onSuccess: () => navigate('/jobs'),
    })
  }

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
      <PageHeader
        title="Post a Job"
        subtitle="Fill in the details below to create a new job listing"
        actions={
          <Box sx={{ display: 'flex', gap: 1 }}>
            <Button variant="outlined" onClick={() => navigate(-1)}>Cancel</Button>
            <Button variant="contained" type="submit" loading={createJob.isPending}>
              Publish Job
            </Button>
          </Box>
        }
      />

      <Grid container spacing={3}>
        {/* Basic Info */}
        <Grid size={{ xs: 12, md: 8 }}>
          <SectionCard title="Job Details" sx={{ mb: 3 }}>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2.5 }}>
              <TextField
                label="Job Title *"
                {...register('title')}
                error={!!errors.title}
                helperText={errors.title?.message}
                placeholder="e.g. Senior React Developer"
                size="small"
              />
              <TextField
                label="Description *"
                {...register('description')}
                error={!!errors.description}
                helperText={errors.description?.message}
                multiline
                rows={6}
                placeholder="Describe the role, team, and what success looks like…"
              />
              <TextField
                label="Responsibilities"
                {...register('responsibilities')}
                multiline
                rows={4}
                placeholder="List the key responsibilities (optional)"
              />
              <TextField
                label="Requirements *"
                {...register('requirements')}
                error={!!errors.requirements}
                helperText={errors.requirements?.message}
                multiline
                rows={4}
                placeholder="Skills, qualifications, and years of experience required…"
              />
              <TextField
                label="Benefits"
                {...register('benefits')}
                multiline
                rows={3}
                placeholder="Health insurance, stock options, remote work, etc. (optional)"
              />
            </Box>
          </SectionCard>

          {/* Skills */}
          <SectionCard title="Required Skills">
            <Controller
              name="skills"
              control={control}
              render={({ field }) => (
                <Autocomplete<string, true, false, true>
                  multiple
                  freeSolo
                  options={POPULAR_SKILLS}
                  value={field.value}
                  onChange={(_, v) => field.onChange(v as string[])}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      size="small"
                      placeholder="Type a skill and press Enter"
                      error={!!errors.skills}
                      helperText={errors.skills?.message ?? 'Select from suggestions or type custom skills'}
                    />
                  )}
                />
              )}
            />
          </SectionCard>
        </Grid>

        {/* Right sidebar */}
        <Grid size={{ xs: 12, md: 4 }}>
          <SectionCard title="Job Meta" sx={{ mb: 3 }}>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
              <Controller
                name="jobType"
                control={control}
                render={({ field }) => (
                  <FormControl size="small" error={!!errors.jobType}>
                    <InputLabel>Job Type *</InputLabel>
                    <Select label="Job Type *" {...field} value={field.value ?? ''}>
                      {JOB_TYPE_OPTIONS.map((o) => <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>)}
                    </Select>
                  </FormControl>
                )}
              />
              <Controller
                name="workMode"
                control={control}
                render={({ field }) => (
                  <FormControl size="small" error={!!errors.workMode}>
                    <InputLabel>Work Mode *</InputLabel>
                    <Select label="Work Mode *" {...field} value={field.value ?? ''}>
                      {WORK_MODE_OPTIONS.map((o) => <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>)}
                    </Select>
                  </FormControl>
                )}
              />
              <Controller
                name="experienceLevel"
                control={control}
                render={({ field }) => (
                  <FormControl size="small" error={!!errors.experienceLevel}>
                    <InputLabel>Experience Level *</InputLabel>
                    <Select label="Experience Level *" {...field} value={field.value ?? ''}>
                      {EXP_LEVEL_OPTIONS.map((o) => <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>)}
                    </Select>
                  </FormControl>
                )}
              />
              <Controller
                name="categoryId"
                control={control}
                render={({ field }) => (
                  <FormControl size="small" error={!!errors.categoryId}>
                    <InputLabel>Category *</InputLabel>
                    <Select label="Category *" {...field} value={field.value ?? ''}>
                      {(categories as any[]).map((c: any) => (
                        <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                )}
              />
              <TextField
                label="Location *"
                {...register('location')}
                error={!!errors.location}
                helperText={errors.location?.message}
                size="small"
                placeholder="e.g. New York, NY or Remote"
              />
              <TextField
                label="Listing Expires On *"
                type="date"
                {...register('expiresAt')}
                error={!!errors.expiresAt}
                helperText={errors.expiresAt?.message}
                size="small"
                slotProps={{ inputLabel: { shrink: true } }}
              />
            </Box>
          </SectionCard>

          <SectionCard title="Compensation">
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
              <Grid container spacing={1}>
                <Grid size={6}>
                  <TextField
                    label="Min Salary"
                    type="number"
                    {...register('salaryMin', { valueAsNumber: true })}
                    size="small"
                    fullWidth
                    slotProps={{ input: { startAdornment: <InputAdornment position="start">$</InputAdornment> } }}
                    disabled={isSalaryHidden}
                  />
                </Grid>
                <Grid size={6}>
                  <TextField
                    label="Max Salary"
                    type="number"
                    {...register('salaryMax', { valueAsNumber: true })}
                    size="small"
                    fullWidth
                    slotProps={{ input: { startAdornment: <InputAdornment position="start">$</InputAdornment> } }}
                    disabled={isSalaryHidden}
                  />
                </Grid>
              </Grid>
              <FormControlLabel
                control={<Switch checked={isSalaryHidden} onChange={(e) => setIsSalaryHidden(e.target.checked)} size="small" />}
                label={<Typography variant="body2">Hide salary from applicants</Typography>}
              />
            </Box>
          </SectionCard>
        </Grid>
      </Grid>
    </Box>
  )
}
