import { useState } from 'react'
import {
  Box, Paper, Stack, Typography, Avatar, Chip, CircularProgress, Alert,
  IconButton, Tooltip, FormControl, InputLabel, Select, MenuItem, Button,
  Dialog, DialogTitle, DialogContent, DialogActions, TextField,
  Divider, List, ListItem, ListItemText, ListItemSecondaryAction,
} from '@mui/material'
import {
  OpenInNew, SettingsOutlined, EditOutlined, DeleteOutlined, AddOutlined,
} from '@mui/icons-material'
import { useHiringPipeline, useMoveCandidateStage, useCreatePipelineStage, useUpdatePipelineStage, useDeletePipelineStage } from '@/features/pipeline/pipelineApi'
import type { CandidatePipelineStage } from '@/features/pipeline/pipelineApi'
import { useMyJobs } from '@/features/jobs/jobsApi'
import { PageHeader, SectionCard } from '@/components/ui'
import type { JobSummary } from '@/types'

const STAGE_COLORS = [
  { label: 'Blue', value: '#3b82f6' },
  { label: 'Purple', value: '#8b5cf6' },
  { label: 'Amber', value: '#f59e0b' },
  { label: 'Green', value: '#10b981' },
  { label: 'Red', value: '#ef4444' },
  { label: 'Pink', value: '#ec4899' },
  { label: 'Cyan', value: '#06b6d4' },
  { label: 'Slate', value: '#64748b' },
]

interface StageFormState {
  name: string
  color: string
  sortOrder: string
}

const DEFAULT_FORM: StageFormState = { name: '', color: '#3b82f6', sortOrder: '' }

interface ManageStagesDialogProps {
  open: boolean
  onClose: () => void
  stages: CandidatePipelineStage[]
  jobPostingId: string
}

function ManageStagesDialog({ open, onClose, stages, jobPostingId }: ManageStagesDialogProps) {
  const [form, setForm] = useState<StageFormState>(DEFAULT_FORM)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [confirmDeleteId, setConfirmDeleteId] = useState<string | null>(null)

  const createStage = useCreatePipelineStage()
  const updateStage = useUpdatePipelineStage()
  const deleteStage = useDeletePipelineStage()

  const isBusy = createStage.isPending || updateStage.isPending || deleteStage.isPending

  const handleEdit = (stage: CandidatePipelineStage) => {
    setEditingId(stage.id)
    setForm({ name: stage.name, color: stage.color ?? '#3b82f6', sortOrder: String(stage.order) })
  }

  const handleCancelEdit = () => {
    setEditingId(null)
    setForm(DEFAULT_FORM)
  }

  const handleSave = () => {
    const payload = {
      name: form.name.trim(),
      color: form.color,
      sortOrder: Number(form.sortOrder) || (stages.length + 1),
    }
    if (!payload.name) return

    if (editingId) {
      updateStage.mutate({ id: editingId, ...payload }, {
        onSuccess: () => { setEditingId(null); setForm(DEFAULT_FORM) },
      })
    } else {
      createStage.mutate({ jobPostingId, ...payload }, {
        onSuccess: () => setForm(DEFAULT_FORM),
      })
    }
  }

  const handleDelete = (id: string) => {
    deleteStage.mutate(id, { onSuccess: () => setConfirmDeleteId(null) })
  }

  return (
    <>
      <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
        <DialogTitle>Manage Pipeline Stages</DialogTitle>
        <DialogContent sx={{ px: 2 }}>
          <List dense disablePadding>
            {stages.length === 0 && (
              <Typography variant="body2" color="text.secondary" sx={{ py: 2, textAlign: 'center' }}>
                No stages yet. Add one below.
              </Typography>
            )}
            {stages.map((stage) => (
              <ListItem key={stage.id} disableGutters sx={{ pr: 10 }}>
                <Box
                  sx={{ width: 12, height: 12, borderRadius: '50%', bgcolor: stage.color, mr: 1.5, flexShrink: 0 }}
                />
                <ListItemText
                  primary={stage.name}
                  secondary={`Order: ${stage.order}`}
                />
                <ListItemSecondaryAction>
                  <IconButton size="small" onClick={() => handleEdit(stage)} disabled={isBusy}>
                    <EditOutlined fontSize="small" />
                  </IconButton>
                  <IconButton size="small" color="error" onClick={() => setConfirmDeleteId(stage.id)} disabled={isBusy || stage.candidateCount > 0}>
                    <DeleteOutlined fontSize="small" />
                  </IconButton>
                </ListItemSecondaryAction>
              </ListItem>
            ))}
          </List>

          <Divider sx={{ my: 2 }} />

          <Typography variant="subtitle2" sx={{ mb: 1.5 }}>
            {editingId ? 'Edit Stage' : 'Add New Stage'}
          </Typography>
          <Stack spacing={2}>
            <TextField
              size="small"
              label="Stage Name *"
              value={form.name}
              onChange={(e) => setForm(f => ({ ...f, name: e.target.value }))}
              fullWidth
            />
            <Stack direction="row" spacing={2}>
              <FormControl size="small" fullWidth>
                <InputLabel>Color</InputLabel>
                <Select
                  label="Color"
                  value={form.color}
                  onChange={(e) => setForm(f => ({ ...f, color: e.target.value }))}
                  renderValue={(v) => (
                    <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                      <Box sx={{ width: 14, height: 14, borderRadius: '50%', bgcolor: v }} />
                      <span>{STAGE_COLORS.find(c => c.value === v)?.label ?? v}</span>
                    </Stack>
                  )}
                >
                  {STAGE_COLORS.map(c => (
                    <MenuItem key={c.value} value={c.value}>
                      <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                        <Box sx={{ width: 14, height: 14, borderRadius: '50%', bgcolor: c.value }} />
                        <span>{c.label}</span>
                      </Stack>
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
              <TextField
                size="small"
                label="Order"
                type="number"
                value={form.sortOrder}
                onChange={(e) => setForm(f => ({ ...f, sortOrder: e.target.value }))}
                sx={{ width: 110 }}
                placeholder={String(stages.length + 1)}
              />
            </Stack>
            <Stack direction="row" spacing={1}>
              <Button
                variant="contained"
                size="small"
                startIcon={editingId ? <EditOutlined /> : <AddOutlined />}
                onClick={handleSave}
                disabled={!form.name.trim() || isBusy}
              >
                {editingId ? 'Update Stage' : 'Add Stage'}
              </Button>
              {editingId && (
                <Button size="small" onClick={handleCancelEdit}>Cancel</Button>
              )}
            </Stack>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose}>Close</Button>
        </DialogActions>
      </Dialog>

      {/* Confirm delete dialog */}
      <Dialog open={!!confirmDeleteId} onClose={() => setConfirmDeleteId(null)} maxWidth="xs" fullWidth>
        <DialogTitle>Delete Stage?</DialogTitle>
        <DialogContent>
          <Typography variant="body2">
            This will permanently delete the stage. Only empty stages can be deleted.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirmDeleteId(null)}>Cancel</Button>
          <Button
            color="error"
            variant="contained"
            onClick={() => confirmDeleteId && handleDelete(confirmDeleteId)}
            disabled={deleteStage.isPending}
          >
            {deleteStage.isPending ? 'Deleting…' : 'Delete'}
          </Button>
        </DialogActions>
      </Dialog>
    </>
  )
}

interface CandidateCardProps {
  candidate: any
  stages: any[]
  onMove: (candidateId: string, newStageId: string) => void
}

function CandidateCard({ candidate, stages, onMove }: CandidateCardProps) {
  const currentIdx = stages.findIndex(s => s.id === candidate.currentStageId)
  const nextStage = stages[currentIdx + 1]

  return (
    <Paper
      sx={{ p: 1.5, mb: 1, cursor: 'grab', '&:hover': { boxShadow: 3 } }}
      draggable
    >
      <Stack direction="row" spacing={1} sx={{ alignItems: 'flex-start' }}>
        <Avatar sx={{ width: 32, height: 32, fontSize: 13, bgcolor: 'primary.main' }}>
          {candidate.candidateName?.[0] ?? 'C'}
        </Avatar>
        <Box sx={{ flex: 1, minWidth: 0 }}>
          <Typography variant="body2" sx={{ fontWeight: 600 }} noWrap>{candidate.candidateName}</Typography>
          <Typography variant="caption" color="text.secondary" noWrap>{candidate.jobTitle}</Typography>
          {candidate.appliedOn && (
            <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>
              Applied {new Date(candidate.appliedOn).toLocaleDateString()}
            </Typography>
          )}
        </Box>
        {nextStage && (
          <Tooltip title={`Move to ${nextStage.name}`}>
            <IconButton size="small" onClick={() => onMove(candidate.id, nextStage.id)}>
              <OpenInNew sx={{ fontSize: 14 }} />
            </IconButton>
          </Tooltip>
        )}
      </Stack>
    </Paper>
  )
}

export default function HiringPipelinePage() {
  const [selectedJobId, setSelectedJobId] = useState<string>('')
  const [manageOpen, setManageOpen] = useState(false)
  const { data: jobsData } = useMyJobs()
  const jobs = jobsData?.items ?? []

  const { data, isLoading } = useHiringPipeline(selectedJobId || undefined)
  const moveCandidateStage = useMoveCandidateStage()

  const stages: CandidatePipelineStage[] = data?.stages ?? []
  const candidates: any[] = data?.candidates ?? []

  const stageMap = stages.reduce((acc: Record<string, any[]>, stage) => {
    acc[stage.id] = candidates.filter(c => c.currentStageId === stage.id)
    return acc
  }, {})

  const handleMove = (candidateId: string, newStageId: string) => {
    moveCandidateStage.mutate({ candidateId, stageId: newStageId })
  }

  return (
    <Box>
      <PageHeader title="Hiring Pipeline" subtitle="Track candidates through your recruitment stages" />

      <SectionCard sx={{ mb: 3 }}>
        <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
          <FormControl size="small" sx={{ minWidth: 320 }}>
            <InputLabel>Select a Job</InputLabel>
            <Select
              label="Select a Job"
              value={selectedJobId}
              onChange={(e) => setSelectedJobId(e.target.value)}
            >
              <MenuItem value="">— Select a job posting —</MenuItem>
              {jobs.map((j: JobSummary) => (
                <MenuItem key={j.id} value={j.id}>{j.title}</MenuItem>
              ))}
            </Select>
          </FormControl>
          <Button
            variant="outlined"
            size="small"
            startIcon={<SettingsOutlined />}
            onClick={() => setManageOpen(true)}
            disabled={!selectedJobId}
          >
            Manage Stages
          </Button>
        </Stack>
      </SectionCard>

      {!selectedJobId ? (
        <Alert severity="info">Select a job posting above to view its hiring pipeline.</Alert>
      ) : isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 8 }}><CircularProgress /></Box>
      ) : stages.length === 0 ? (
        <Alert severity="info" action={
          <Button color="inherit" size="small" onClick={() => setManageOpen(true)}>Add Stages</Button>
        }>
          No pipeline stages configured yet. Click "Add Stages" to set up your hiring funnel.
        </Alert>
      ) : (
        <Box sx={{ overflowX: 'auto' }}>
          <Stack
            direction="row"
            spacing={2}
            sx={{ minWidth: stages.length * 260 + 'px', pb: 2, alignItems: 'flex-start' }}
          >
            {stages.map((stage) => {
              const stageCandidates = stageMap[stage.id] ?? []
              return (
                <Paper
                  key={stage.id}
                  sx={{ minWidth: 240, maxWidth: 260, p: 1.5, bgcolor: 'background.default', flex: '0 0 auto' }}
                >
                  <Stack direction="row" sx={{ alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                    <Stack direction="row" spacing={0.75} sx={{ alignItems: 'center' }}>
                      <Box sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: stage.color }} />
                      <Typography variant="subtitle2" sx={{ fontWeight: 700 }}>{stage.name}</Typography>
                    </Stack>
                    <Chip label={stageCandidates.length} size="small" color="primary" />
                  </Stack>

                  <Box sx={{ minHeight: 200 }}>
                    {stageCandidates.length === 0 ? (
                      <Box sx={{
                        border: '2px dashed',
                        borderColor: 'divider',
                        borderRadius: 1,
                        p: 3,
                        textAlign: 'center',
                      }}>
                        <Typography variant="caption" color="text.disabled">No candidates</Typography>
                      </Box>
                    ) : stageCandidates.map((c: any) => (
                      <CandidateCard
                        key={c.id}
                        candidate={c}
                        stages={stages}
                        onMove={handleMove}
                      />
                    ))}
                  </Box>
                </Paper>
              )
            })}
          </Stack>
        </Box>
      )}

      {selectedJobId && stages.length > 0 && (
        <Stack direction="row" spacing={2} sx={{ mt: 3, flexWrap: 'wrap' }}>
          <Chip label={`Total candidates: ${candidates.length}`} />
          {stages.map((s) => (
            <Chip
              key={s.id}
              label={`${s.name}: ${(stageMap[s.id] ?? []).length}`}
              variant="outlined"
              size="small"
              sx={{ borderColor: s.color, color: s.color }}
            />
          ))}
        </Stack>
      )}

      {selectedJobId && (
        <ManageStagesDialog
          open={manageOpen}
          onClose={() => setManageOpen(false)}
          stages={stages}
          jobPostingId={selectedJobId}
        />
      )}
    </Box>
  )
}
