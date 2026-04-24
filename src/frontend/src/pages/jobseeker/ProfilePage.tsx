import { useState, useCallback } from 'react'
import {
  Box,
  Grid,
  Typography,
  Avatar,
  Chip,
  IconButton,
  Button,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  LinearProgress,
  FormControlLabel,
  Switch,
  Tooltip,
} from '@mui/material'
import {
  EditOutlined,
  AddOutlined,
  DeleteOutlined,
  PersonOutlined,
  CloudUploadOutlined,
  LinkOutlined,
  WorkHistoryOutlined,
  SchoolOutlined,
} from '@mui/icons-material'
import { useDropzone } from 'react-dropzone'
import {
  useProfile,
  useUpdateProfile,
  useUploadResume,
  useAddWorkExperience,
  useDeleteWorkExperience,
  useAddEducation,
  useDeleteEducation,
  type WorkExperience,
  type Education,
} from '@/features/profile/profileApi'
import { PageHeader, SectionCard, LoadingSpinner, EmptyState } from '@/components/ui'
import { formatDate } from '@/utils/formatters'

// ── Resume dropzone ─────────────────────────────────────────────

function ResumeDropzone() {
  const upload = useUploadResume()
  const onDrop = useCallback((files: File[]) => {
    if (files[0]) upload.mutate(files[0])
  }, [upload])
  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: { 'application/pdf': ['.pdf'], 'application/msword': ['.doc'], 'application/vnd.openxmlformats-officedocument.wordprocessingml.document': ['.docx'] },
    maxFiles: 1,
    maxSize: 5 * 1024 * 1024,
  })

  return (
    <Box
      {...getRootProps()}
      sx={{
        border: '2px dashed',
        borderColor: isDragActive ? 'primary.main' : 'divider',
        borderRadius: 2,
        p: 3,
        textAlign: 'center',
        cursor: 'pointer',
        bgcolor: isDragActive ? 'primary.50' : 'background.default',
        transition: 'all 0.2s',
        '&:hover': { borderColor: 'primary.main', bgcolor: 'primary.50' },
      }}
    >
      <input {...getInputProps()} />
      {upload.isPending ? (
        <LinearProgress sx={{ mt: 1 }} />
      ) : (
        <>
          <CloudUploadOutlined sx={{ fontSize: 36, color: 'text.disabled', mb: 1 }} />
          <Typography variant="body2" color="text.secondary">
            {isDragActive ? 'Drop your resume here' : 'Drag & drop or click to upload resume (PDF, DOC, DOCX — max 5 MB)'}
          </Typography>
        </>
      )}
    </Box>
  )
}

// ── Work Experience dialog ──────────────────────────────────────

function WorkExperienceDialog({ open, onClose }: { open: boolean; onClose: () => void }) {
  const add = useAddWorkExperience()
  const [form, setForm] = useState<Omit<WorkExperience, 'id'>>({
    jobTitle: '', companyName: '', location: '', startDate: '', endDate: '', isCurrent: false, description: '',
  })
  const set = (k: keyof typeof form, v: unknown) => setForm((f) => ({ ...f, [k]: v }))

  const handleSubmit = () => {
    add.mutate({ ...form, endDate: form.isCurrent ? undefined : form.endDate || undefined }, {
      onSuccess: () => { onClose(); setForm({ jobTitle: '', companyName: '', location: '', startDate: '', endDate: '', isCurrent: false, description: '' }) },
    })
  }

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add Work Experience</DialogTitle>
      <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
        <TextField label="Job Title *" value={form.jobTitle} onChange={(e) => set('jobTitle', e.target.value)} size="small" />
        <TextField label="Company *" value={form.companyName} onChange={(e) => set('companyName', e.target.value)} size="small" />
        <TextField label="Location" value={form.location} onChange={(e) => set('location', e.target.value)} size="small" />
        <Grid container spacing={1}>
          <Grid size={6}><TextField label="Start Date *" type="month" value={form.startDate} onChange={(e) => set('startDate', e.target.value)} size="small" fullWidth slotProps={{ inputLabel: { shrink: true } }} /></Grid>
          <Grid size={6}><TextField label="End Date" type="month" value={form.endDate} onChange={(e) => set('endDate', e.target.value)} size="small" fullWidth disabled={form.isCurrent} slotProps={{ inputLabel: { shrink: true } }} /></Grid>
        </Grid>
        <FormControlLabel control={<Switch checked={form.isCurrent} onChange={(e) => set('isCurrent', e.target.checked)} />} label="Currently working here" />
        <TextField label="Description" multiline rows={3} value={form.description} onChange={(e) => set('description', e.target.value)} size="small" />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button variant="contained" onClick={handleSubmit} disabled={!form.jobTitle || !form.companyName || !form.startDate || add.isPending}>
          {add.isPending ? 'Adding…' : 'Add'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

// ── Education dialog ─────────────────────────────────────────────

function EducationDialog({ open, onClose }: { open: boolean; onClose: () => void }) {
  const add = useAddEducation()
  const [form, setForm] = useState<Omit<Education, 'id'>>({ degree: '', fieldOfStudy: '', institutionName: '', startYear: new Date().getFullYear(), endYear: undefined, grade: '', description: '' })
  const set = (k: keyof typeof form, v: unknown) => setForm((f) => ({ ...f, [k]: v }))

  const handleSubmit = () => {
    add.mutate(form, { onSuccess: () => { onClose(); setForm({ degree: '', fieldOfStudy: '', institutionName: '', startYear: new Date().getFullYear(), endYear: undefined, grade: '', description: '' }) } })
  }

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add Education</DialogTitle>
      <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
        <TextField label="Degree *" value={form.degree} onChange={(e) => set('degree', e.target.value)} size="small" />
        <TextField label="Field of Study *" value={form.fieldOfStudy} onChange={(e) => set('fieldOfStudy', e.target.value)} size="small" />
        <TextField label="Institution *" value={form.institutionName} onChange={(e) => set('institutionName', e.target.value)} size="small" />
        <Grid container spacing={1}>
          <Grid size={6}><TextField label="Start Year *" type="number" value={form.startYear} onChange={(e) => set('startYear', parseInt(e.target.value))} size="small" fullWidth /></Grid>
          <Grid size={6}><TextField label="End Year" type="number" value={form.endYear ?? ''} onChange={(e) => set('endYear', e.target.value ? parseInt(e.target.value) : undefined)} size="small" fullWidth /></Grid>
        </Grid>
        <TextField label="Grade / GPA" value={form.grade} onChange={(e) => set('grade', e.target.value)} size="small" />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button variant="contained" onClick={handleSubmit} disabled={!form.degree || !form.fieldOfStudy || !form.institutionName || add.isPending}>
          {add.isPending ? 'Adding…' : 'Add'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

// ── Page ───────────────────────────────────────────────────────

export function ProfilePage() {
  const { data: profile, isLoading } = useProfile()
  const updateProfile = useUpdateProfile()
  const deleteExp = useDeleteWorkExperience()
  const deleteEdu = useDeleteEducation()

  const [editOpen, setEditOpen] = useState(false)
  const [workOpen, setWorkOpen] = useState(false)
  const [eduOpen, setEduOpen] = useState(false)
  const [editForm, setEditForm] = useState({ headline: '', about: '', phone: '', linkedInUrl: '', portfolioUrl: '', isOpenToWork: false })

  const openEdit = () => {
    setEditForm({
      headline: profile?.headline ?? '',
      about: profile?.about ?? '',
      phone: profile?.phone ?? '',
      linkedInUrl: profile?.linkedInUrl ?? '',
      portfolioUrl: profile?.portfolioUrl ?? '',
      isOpenToWork: profile?.isOpenToWork ?? false,
    })
    setEditOpen(true)
  }

  const handleSaveProfile = () => {
    updateProfile.mutate(editForm, { onSuccess: () => setEditOpen(false) })
  }

  if (isLoading) return <LoadingSpinner />

  return (
    <Box>
      <PageHeader
        title="My Profile"
        actions={
          <Button variant="outlined" startIcon={<EditOutlined />} onClick={openEdit}>
            Edit Profile
          </Button>
        }
      />

      <Grid container spacing={3}>
        {/* Left column */}
        <Grid size={{ xs: 12, md: 4 }}>
          {/* Photo + basic info */}
          <SectionCard sx={{ mb: 2, textAlign: 'center' }}>
            <Avatar
              src={profile?.profilePictureUrl}
              sx={{ width: 96, height: 96, mx: 'auto', mb: 2, bgcolor: 'primary.main', fontSize: 36 }}
            >
              {profile ? `${profile.firstName[0]}${profile.lastName[0]}` : <PersonOutlined />}
            </Avatar>
            <Typography variant="h6" sx={{ fontWeight: 700 }}>
              {profile ? `${profile.firstName} ${profile.lastName}` : '—'}
            </Typography>
            {profile?.headline && <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>{profile.headline}</Typography>}
            {profile?.isOpenToWork && (
              <Chip label="Open to Work" color="success" size="small" sx={{ mt: 1 }} />
            )}
            {profile?.linkedInUrl && (
              <Box sx={{ mt: 1.5 }}>
                <Button size="small" startIcon={<LinkOutlined />} href={profile.linkedInUrl} target="_blank" rel="noopener noreferrer">
                  LinkedIn
                </Button>
              </Box>
            )}
          </SectionCard>

          {/* Resume */}
          <SectionCard title="Resume" sx={{ mb: 2 }}>
            {profile?.resumeUrl ? (
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1.5 }}>
                <Typography variant="body2">resume.pdf</Typography>
                <Button
                  size="small"
                  href={profile.resumeUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  View
                </Button>
              </Box>
            ) : null}
            <ResumeDropzone />
          </SectionCard>

          {/* Skills */}
          <SectionCard title="Skills">
            {profile?.skills.length ? (
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.75 }}>
                {profile.skills.map((s: string) => <Chip key={s} label={s} size="small" />)}
              </Box>
            ) : (
              <Typography variant="body2" color="text.secondary">No skills added yet.</Typography>
            )}
          </SectionCard>
        </Grid>

        {/* Right column */}
        <Grid size={{ xs: 12, md: 8 }}>
          {/* About */}
          <SectionCard title="About" sx={{ mb: 2 }}>
            <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap' }}>
              {profile?.about ?? 'No summary added yet.'}
            </Typography>
          </SectionCard>

          {/* Work experience */}
          <SectionCard
            title="Work Experience"
            action={
              <IconButton size="small" onClick={() => setWorkOpen(true)} aria-label="add work experience">
                <AddOutlined />
              </IconButton>
            }
            sx={{ mb: 2 }}
          >
            {!profile?.workExperiences.length ? (
              <EmptyState icon={<WorkHistoryOutlined />} title="No work experience" description="Add your work history to strengthen your profile." />
            ) : (
              profile.workExperiences.map((exp: import('@/features/profile/profileApi').WorkExperience) => (
                <Box key={exp.id} sx={{ mb: 2, '&:last-child': { mb: 0 } }}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <Box>
                      <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>{exp.jobTitle}</Typography>
                      <Typography variant="body2" color="text.secondary">
                        {exp.companyName}{exp.location ? ` · ${exp.location}` : ''}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        {formatDate(exp.startDate, 'MMM yyyy')} – {exp.isCurrent ? 'Present' : exp.endDate ? formatDate(exp.endDate, 'MMM yyyy') : ''}
                      </Typography>
                      {exp.description && <Typography variant="body2" sx={{ mt: 0.5, whiteSpace: 'pre-wrap' }}>{exp.description}</Typography>}
                    </Box>
                    <Tooltip title="Remove">
                      <IconButton size="small" onClick={() => deleteExp.mutate(exp.id)} sx={{ color: 'error.main' }}>
                        <DeleteOutlined fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  </Box>
                </Box>
              ))
            )}
          </SectionCard>

          {/* Education */}
          <SectionCard
            title="Education"
            action={
              <IconButton size="small" onClick={() => setEduOpen(true)} aria-label="add education">
                <AddOutlined />
              </IconButton>
            }
          >
            {!profile?.educations.length ? (
              <EmptyState icon={<SchoolOutlined />} title="No education added" description="Add your academic background." />
            ) : (
              profile.educations.map((edu: import('@/features/profile/profileApi').Education) => (
                <Box key={edu.id} sx={{ mb: 2, '&:last-child': { mb: 0 } }}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <Box>
                      <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>{edu.degree} in {edu.fieldOfStudy}</Typography>
                      <Typography variant="body2" color="text.secondary">{edu.institutionName}</Typography>
                      <Typography variant="caption" color="text.secondary">
                        {edu.startYear} – {edu.endYear ?? 'Present'}
                        {edu.grade ? ` · ${edu.grade}` : ''}
                      </Typography>
                    </Box>
                    <Tooltip title="Remove">
                      <IconButton size="small" onClick={() => deleteEdu.mutate(edu.id)} sx={{ color: 'error.main' }}>
                        <DeleteOutlined fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  </Box>
                </Box>
              ))
            )}
          </SectionCard>
        </Grid>
      </Grid>

      {/* Edit profile dialog */}
      <Dialog open={editOpen} onClose={() => setEditOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Edit Profile</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
          <TextField label="Professional Headline" value={editForm.headline} onChange={(e) => setEditForm((f) => ({ ...f, headline: e.target.value }))} size="small" placeholder="e.g. Senior React Developer" />
          <TextField label="About" multiline rows={4} value={editForm.about} onChange={(e) => setEditForm((f) => ({ ...f, about: e.target.value }))} size="small" placeholder="Tell employers about yourself…" />
          <TextField label="Phone" value={editForm.phone} onChange={(e) => setEditForm((f) => ({ ...f, phone: e.target.value }))} size="small" />
          <TextField label="LinkedIn URL" value={editForm.linkedInUrl} onChange={(e) => setEditForm((f) => ({ ...f, linkedInUrl: e.target.value }))} size="small" />
          <TextField label="Portfolio / Website" value={editForm.portfolioUrl} onChange={(e) => setEditForm((f) => ({ ...f, portfolioUrl: e.target.value }))} size="small" />
          <FormControlLabel
            control={<Switch checked={editForm.isOpenToWork} onChange={(e) => setEditForm((f) => ({ ...f, isOpenToWork: e.target.checked }))} />}
            label="Open to work opportunities"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSaveProfile} disabled={updateProfile.isPending}>
            {updateProfile.isPending ? 'Saving…' : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>

      <WorkExperienceDialog open={workOpen} onClose={() => setWorkOpen(false)} />
      <EducationDialog open={eduOpen} onClose={() => setEduOpen(false)} />
    </Box>
  )
}
