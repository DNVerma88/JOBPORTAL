import { useState } from 'react'
import {
  Box, Tab, Tabs, Stack, Button, Chip, IconButton, Dialog, DialogTitle, DialogContent,
  DialogActions, TextField, Switch, FormControlLabel, Paper, Grid, Typography,
  CircularProgress, Alert
} from '@mui/material'
import { Add as AddIcon, Edit as EditIcon, CheckCircle, Cancel } from '@mui/icons-material'
import {
  useSkills, useJobCategories, useIndustries,
  useCreateMasterItem, useToggleMasterItem,
} from '@/features/config/configApi'
import { useAdminSubscriptionPlans, useCreatePlan, useUpdatePlan } from '@/features/billing/billingApi'
import { PageHeader, StatusChip, SearchInput } from '@/components/ui'

function SkillsTab() {
  const { data: skills = [], isLoading } = useSkills()
  const createItem = useCreateMasterItem()
  const toggleItem = useToggleMasterItem()
  const [search, setSearch] = useState('')
  const [open, setOpen] = useState(false)
  const [name, setName] = useState('')

  const filtered = (skills as any[]).filter(s => !search || s.name.toLowerCase().includes(search.toLowerCase()))

  const handleCreate = () => {
    createItem.mutate({ endpoint: '/master/skills', body: { name } }, {
      onSuccess: () => { setOpen(false); setName('') },
    })
  }

  return (
    <Box>
      <Stack direction="row" spacing={2} sx={{ mb: 2, alignItems: 'center' }}>
        <SearchInput value={search} onChange={setSearch} placeholder="Search skills..." />
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpen(true)} size="small">
          Add Skill
        </Button>
      </Stack>

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress /></Box>
      ) : (
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
          {filtered.map((s: any) => (
            <Chip
              key={s.id}
              label={s.name}
              color={s.isActive ? 'default' : 'error'}
              variant={s.isActive ? 'filled' : 'outlined'}
              onDelete={() => toggleItem.mutate({ endpoint: '/master/skills', id: s.id, isActive: !s.isActive })}
              deleteIcon={s.isActive ? <Cancel /> : <CheckCircle />}
            />
          ))}
          {filtered.length === 0 && <Typography color="text.secondary">No skills found</Typography>}
        </Box>
      )}

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Add Skill</DialogTitle>
        <DialogContent>
          <TextField
            label="Skill Name" value={name} fullWidth sx={{ mt: 1 }}
            onChange={e => setName(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreate} disabled={!name || createItem.isPending}>Add</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

function CategoriesTab() {
  const { data: categories = [], isLoading } = useJobCategories()
  const createItem = useCreateMasterItem()
  const [open, setOpen] = useState(false)
  const [form, setForm] = useState({ name: '', slug: '' })

  const handleCreate = () => {
    createItem.mutate({ endpoint: '/master/categories', body: form }, {
      onSuccess: () => { setOpen(false); setForm({ name: '', slug: '' }) },
    })
  }

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: "flex-end" }}>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpen(true)} size="small">
          Add Category
        </Button>
      </Stack>

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress /></Box>
      ) : (
        <Grid container spacing={2}>
          {(categories as any[]).map((c) => (
            <Grid size={{ xs: 12, sm: 6, md: 4 }} key={c.id}>
              <Paper sx={{ p: 2 }} variant="outlined">
                <Stack direction="row" sx={{ justifyContent: 'space-between' }}>
                  <Box>
                    <Typography variant="body1" sx={{ fontWeight: 600 }}>{c.name}</Typography>
                    <Chip label={c.slug} size="small" variant="outlined" />
                    {c.jobCount != null && (
                      <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>
                        {c.jobCount} jobs
                      </Typography>
                    )}
                  </Box>
                  <StatusChip status={c.isActive ? 'Active' : 'Inactive'} />
                </Stack>
              </Paper>
            </Grid>
          ))}
          {(categories as any[]).length === 0 && (
            <Grid size={12}>
              <Alert severity="info">No categories found.</Alert>
            </Grid>
          )}
        </Grid>
      )}

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Add Category</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField label="Name" value={form.name} fullWidth onChange={e => setForm(f => ({ ...f, name: e.target.value }))} />
            <TextField label="Slug" value={form.slug} fullWidth onChange={e => setForm(f => ({ ...f, slug: e.target.value }))} />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreate} disabled={!form.name || createItem.isPending}>Add</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

function IndustriesTab() {
  const { data: industries = [], isLoading } = useIndustries()
  const createItem = useCreateMasterItem()
  const toggleItem = useToggleMasterItem()
  const [open, setOpen] = useState(false)
  const [name, setName] = useState('')

  const handleCreate = () => {
    createItem.mutate({ endpoint: '/master/industries', body: { name } }, {
      onSuccess: () => { setOpen(false); setName('') },
    })
  }

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: "flex-end" }}>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpen(true)} size="small">
          Add Industry
        </Button>
      </Stack>

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress /></Box>
      ) : (
        <Stack spacing={1}>
          {(industries as any[]).map((ind) => (
            <Paper key={ind.id} sx={{ p: 1.5, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }} variant="outlined">
              <Typography>{ind.name}</Typography>
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <StatusChip status={ind.isActive ? 'Active' : 'Inactive'} />
                <Button
                  size="small"
                  color={ind.isActive ? 'warning' : 'success'}
                  onClick={() => toggleItem.mutate({ endpoint: '/master/industries', id: ind.id, isActive: !ind.isActive })}
                >
                  {ind.isActive ? 'Disable' : 'Enable'}
                </Button>
              </Stack>
            </Paper>
          ))}
          {(industries as any[]).length === 0 && <Alert severity="info">No industries found.</Alert>}
        </Stack>
      )}

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Add Industry</DialogTitle>
        <DialogContent>
          <TextField
            label="Industry Name" value={name} fullWidth sx={{ mt: 1 }}
            onChange={e => setName(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreate} disabled={!name || createItem.isPending}>Add</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

function SubscriptionPlansTab() {
  const { data: plans = [], isLoading } = useAdminSubscriptionPlans()
  const createPlan = useCreatePlan()
  const updatePlan = useUpdatePlan()
  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState<any | null>(null)
  const [form, setForm] = useState({ name: '', price: '', billingCycle: 'Monthly', maxJobs: '', maxUsers: '', isActive: true })

  const handleOpen = (plan?: any) => {
    if (plan) {
      setEditing(plan)
      setForm({ name: plan.name, price: String(plan.price), billingCycle: plan.billingCycle, maxJobs: String(plan.maxJobs ?? ''), maxUsers: String(plan.maxUsers ?? ''), isActive: plan.isActive })
    } else {
      setEditing(null)
      setForm({ name: '', price: '', billingCycle: 'Monthly', maxJobs: '', maxUsers: '', isActive: true })
    }
    setOpen(true)
  }

  const handleSave = () => {
    const payload = { ...form, price: Number(form.price), maxJobs: Number(form.maxJobs), maxUsers: Number(form.maxUsers) }
    if (editing) {
      updatePlan.mutate({ id: editing.id, ...payload }, { onSuccess: () => setOpen(false) })
    } else {
      createPlan.mutate(payload, { onSuccess: () => setOpen(false) })
    }
  }

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: "flex-end" }}>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => handleOpen()}>New Plan</Button>
      </Stack>

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress /></Box>
      ) : (
        <Grid container spacing={2}>
          {(plans as any[]).map((p) => (
            <Grid size={{ xs: 12, sm: 6, md: 4 }} key={p.id}>
              <Paper sx={{ p: 3 }} variant="outlined">
                <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start' }}>
                  <Box>
                    <Typography variant="h6">{p.name}</Typography>
                    <Typography variant="h5" color="primary" sx={{ fontWeight: 700 }}>
                      ${p.monthlyPrice ?? p.annualPrice ?? '—'}<Typography component="span" variant="caption">/mo</Typography>
                    </Typography>
                    <Typography variant="body2" color="text.secondary">Max Jobs: {p.maxJobPostings ?? '∞'}</Typography>
                    <Typography variant="body2" color="text.secondary">Max Users: {p.maxUsers ?? '∞'}</Typography>
                  </Box>
                  <Stack spacing={1} sx={{ alignItems: 'flex-end' }}>
                    <StatusChip status={p.isActive ? 'Active' : 'Inactive'} />
                    <IconButton size="small" onClick={() => handleOpen(p)}><EditIcon fontSize="small" /></IconButton>
                  </Stack>
                </Stack>
              </Paper>
            </Grid>
          ))}
          {(plans as any[]).length === 0 && (
            <Grid size={12}><Alert severity="info">No subscription plans defined yet.</Alert></Grid>
          )}
        </Grid>
      )}

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editing ? 'Edit Plan' : 'Create Plan'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField label="Plan Name" value={form.name} fullWidth onChange={e => setForm(f => ({ ...f, name: e.target.value }))} />
            <Stack direction="row" spacing={2}>
              <TextField label="Price ($)" value={form.price} type="number" fullWidth onChange={e => setForm(f => ({ ...f, price: e.target.value }))} />
              <TextField
                select label="Billing Cycle" value={form.billingCycle} fullWidth
                onChange={e => setForm(f => ({ ...f, billingCycle: e.target.value }))}
                slotProps={{ select: { native: true } }}
              >
                <option value="Monthly">Monthly</option>
                <option value="Annually">Annually</option>
              </TextField>
            </Stack>
            <Stack direction="row" spacing={2}>
              <TextField label="Max Jobs" value={form.maxJobs} type="number" fullWidth onChange={e => setForm(f => ({ ...f, maxJobs: e.target.value }))} />
              <TextField label="Max Users" value={form.maxUsers} type="number" fullWidth onChange={e => setForm(f => ({ ...f, maxUsers: e.target.value }))} />
            </Stack>
            <FormControlLabel
              control={<Switch checked={form.isActive} onChange={e => setForm(f => ({ ...f, isActive: e.target.checked }))} />}
              label="Active"
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSave} disabled={!form.name || !form.price}>
            {editing ? 'Update Plan' : 'Create Plan'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default function MasterDataPage() {
  const [tab, setTab] = useState(0)

  return (
    <Box>
      <PageHeader title="Master Data" subtitle="Manage platform reference data and subscription plans" />
      <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 3 }}>
        <Tab label="Skills" />
        <Tab label="Job Categories" />
        <Tab label="Industries" />
        <Tab label="Subscription Plans" />
      </Tabs>
      {tab === 0 && <SkillsTab />}
      {tab === 1 && <CategoriesTab />}
      {tab === 2 && <IndustriesTab />}
      {tab === 3 && <SubscriptionPlansTab />}
    </Box>
  )
}
