import { useState } from 'react'
import {
  Box, Tab, Tabs, Stack, Switch, FormControlLabel, Paper, Typography,
  TextField, Button, Chip, IconButton, Dialog, DialogTitle, DialogContent,
  DialogActions, Alert
} from '@mui/material'
import { Edit as EditIcon, Delete as DeleteIcon, Add as AddIcon } from '@mui/icons-material'
import {
  useGlobalSettings, useUpdateSetting,
  useEmailTemplates, useUpdateEmailTemplate,
  useFeatureFlags, useToggleFeatureFlag,
  useAnnouncements, useCreateAnnouncement, useDeleteAnnouncement,
} from '@/features/config/configApi'
import { PageHeader, SectionCard, StatusChip } from '@/components/ui'

function SettingsTab() {
  const { data: settings = [], isLoading } = useGlobalSettings()
  const updateSetting = useUpdateSetting()
  const [edited, setEdited] = useState<Record<string, string>>({})

  const grouped = (settings as any[]).reduce((acc: Record<string, any[]>, s) => {
    if (!acc[s.group]) acc[s.group] = []
    acc[s.group].push(s)
    return acc
  }, {})

  return (
    <Box>
      {isLoading ? (
        <Typography color="text.secondary">Loading settings…</Typography>
      ) : (
        <Stack spacing={3}>
          {Object.entries(grouped).map(([group, items]) => (
            <SectionCard key={group} title={group}>
              <Stack spacing={2}>
                {(items as any[]).map((s) => (
                  <Stack key={s.key} direction="row" spacing={2} sx={{ alignItems: 'center' }}>
                    <Box sx={{ flex: 1 }}>
                      <Typography variant="body2" sx={{ fontWeight: 600 }}>{s.key}</Typography>
                      {s.description && <Typography variant="caption" color="text.secondary">{s.description}</Typography>}
                    </Box>
                    <TextField
                      size="small"
                      defaultValue={s.value}
                      onChange={e => setEdited(prev => ({ ...prev, [s.key]: e.target.value }))}
                      sx={{ width: 240 }}
                    />
                    <Button
                      size="small"
                      variant="contained"
                      disabled={!edited[s.key] || updateSetting.isPending}
                      onClick={() => updateSetting.mutate({ key: s.key, value: edited[s.key] })}
                    >
                      Save
                    </Button>
                  </Stack>
                ))}
              </Stack>
            </SectionCard>
          ))}
          {Object.keys(grouped).length === 0 && (
            <Alert severity="info">No settings configured yet. Settings will appear here once the API is available.</Alert>
          )}
        </Stack>
      )}
    </Box>
  )
}

function EmailTemplatesTab() {
  const { data: templates = [] } = useEmailTemplates()
  const updateTemplate = useUpdateEmailTemplate()
  const [editing, setEditing] = useState<any | null>(null)
  const [subject, setSubject] = useState('')
  const [body, setBody] = useState('')

  const startEdit = (t: any) => { setEditing(t); setSubject(t.subject); setBody(t.body) }
  const handleSave = () => {
    if (!editing) return
    updateTemplate.mutate({ id: editing.id, subject, body }, { onSuccess: () => setEditing(null) })
  }

  return (
    <Box>
      {(templates as any[]).length === 0 ? (
        <Alert severity="info">No email templates found. They will appear here once the API is configured.</Alert>
      ) : (
        <Stack spacing={2}>
          {(templates as any[]).map((t) => (
            <Paper key={t.id} sx={{ p: 2 }} variant="outlined">
              <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
                <Box>
                  <Typography variant="body1" sx={{ fontWeight: 600 }}>{t.name}</Typography>
                  <Typography variant="body2" color="text.secondary">{t.subject}</Typography>
                  {t.variables && (
                    <Typography variant="caption" color="text.secondary">Variables: {t.variables}</Typography>
                  )}
                </Box>
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <StatusChip status={t.isActive ? 'Active' : 'Inactive'} />
                  <IconButton size="small" onClick={() => startEdit(t)}><EditIcon fontSize="small" /></IconButton>
                </Stack>
              </Stack>
            </Paper>
          ))}
        </Stack>
      )}

      <Dialog open={!!editing} onClose={() => setEditing(null)} maxWidth="md" fullWidth>
        <DialogTitle>Edit Template: {editing?.name}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField label="Subject" value={subject} onChange={e => setSubject(e.target.value)} fullWidth />
            <TextField
              label="Body (HTML)"
              value={body}
              onChange={e => setBody(e.target.value)}
              multiline
              rows={10}
              fullWidth
              slotProps={{ htmlInput: { style: { fontFamily: 'monospace', fontSize: 13 } } }}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditing(null)}>Cancel</Button>
          <Button variant="contained" onClick={handleSave} disabled={updateTemplate.isPending}>
            {updateTemplate.isPending ? 'Saving…' : 'Save Template'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

function FeatureFlagsTab() {
  const { data: flags = [] } = useFeatureFlags()
  const toggle = useToggleFeatureFlag()

  return (
    <Box>
      {(flags as any[]).length === 0 ? (
        <Alert severity="info">No feature flags configured. They will appear here once the API is available.</Alert>
      ) : (
        <Stack spacing={2}>
          {(flags as any[]).map((f) => (
            <Paper key={f.id} sx={{ p: 2 }} variant="outlined">
              <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
                <Box>
                  <Typography variant="body1" sx={{ fontWeight: 600 }}>{f.name}</Typography>
                  <Chip label={f.key} size="small" variant="outlined" sx={{ mr: 1 }} />
                  {f.description && <Typography variant="caption" color="text.secondary">{f.description}</Typography>}
                </Box>
                <FormControlLabel
                  control={
                    <Switch
                      checked={f.isEnabled}
                      onChange={e => toggle.mutate({ id: f.id, isEnabled: e.target.checked })}
                      disabled={toggle.isPending}
                    />
                  }
                  label={f.isEnabled ? 'Enabled' : 'Disabled'}
                />
              </Stack>
            </Paper>
          ))}
        </Stack>
      )}
    </Box>
  )
}

function AnnouncementsTab() {
  const { data: announcements = [] } = useAnnouncements()
  const createAnnouncement = useCreateAnnouncement()
  const deleteAnnouncement = useDeleteAnnouncement()
  const [open, setOpen] = useState(false)
  const [form, setForm] = useState({ title: '', content: '', type: 'Info' as 'Info' | 'Warning' | 'Critical', isActive: true })

  const handleCreate = () => {
    createAnnouncement.mutate(form, { onSuccess: () => { setOpen(false); setForm({ title: '', content: '', type: 'Info', isActive: true }) } })
  }

  const typeColor = (type: string) =>
    type === 'Critical' ? 'error' : type === 'Warning' ? 'warning' : 'info'

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: "flex-end" }}>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpen(true)}>
          New Announcement
        </Button>
      </Stack>

      {(announcements as any[]).length === 0 ? (
        <Alert severity="info">No announcements yet.</Alert>
      ) : (
        <Stack spacing={2}>
          {(announcements as any[]).map((a) => (
            <Alert
              key={a.id}
              severity={typeColor(a.type) as any}
              action={
                <IconButton size="small" onClick={() => deleteAnnouncement.mutate(a.id)}>
                  <DeleteIcon fontSize="small" />
                </IconButton>
              }
            >
              <Typography variant="body2" sx={{ fontWeight: 600 }}>{a.title}</Typography>
              <Typography variant="body2">{a.content}</Typography>
              {a.endsOn && <Typography variant="caption">Expires: {new Date(a.endsOn).toLocaleDateString()}</Typography>}
            </Alert>
          ))}
        </Stack>
      )}

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create Announcement</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label="Title" value={form.title} fullWidth
              onChange={e => setForm(f => ({ ...f, title: e.target.value }))}
            />
            <TextField
              label="Content" value={form.content} multiline rows={4} fullWidth
              onChange={e => setForm(f => ({ ...f, content: e.target.value }))}
            />
            <TextField
              select label="Type" value={form.type} fullWidth
              onChange={e => setForm(f => ({ ...f, type: e.target.value as any }))}
            >
              <option value="Info">Info</option>
              <option value="Warning">Warning</option>
              <option value="Critical">Critical</option>
            </TextField>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreate} disabled={createAnnouncement.isPending || !form.title}>
            {createAnnouncement.isPending ? 'Creating…' : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default function SystemSettingsPage() {
  const [tab, setTab] = useState(0)

  return (
    <Box>
      <PageHeader title="System Settings" subtitle="Configure global platform settings and behaviours" />
      <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 3 }}>
        <Tab label="General Settings" />
        <Tab label="Email Templates" />
        <Tab label="Feature Flags" />
        <Tab label="Announcements" />
      </Tabs>
      {tab === 0 && <SettingsTab />}
      {tab === 1 && <EmailTemplatesTab />}
      {tab === 2 && <FeatureFlagsTab />}
      {tab === 3 && <AnnouncementsTab />}
    </Box>
  )
}
