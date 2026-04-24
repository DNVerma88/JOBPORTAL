import { useState } from 'react'
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  List, ListItemButton, ListItemText, ListItemIcon,
  Button, CircularProgress, Alert, Chip, Typography, Box, TextField,
  InputAdornment,
} from '@mui/material'
import { ApartmentOutlined, CheckCircle, Search } from '@mui/icons-material'
import { useTenants } from '@/features/admin/adminApi'
import { useAuthStore } from '@/features/auth/store/authStore'
import { useQueryClient } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'

interface SwitchTenantDialogProps {
  open: boolean
  onClose: () => void
}

export function SwitchTenantDialog({ open, onClose }: SwitchTenantDialogProps) {
  const { data, isLoading, isError } = useTenants()
  const { activeTenantId, switchTenant } = useAuthStore()
  const queryClient = useQueryClient()
  const { enqueueSnackbar } = useSnackbar()
  const [search, setSearch] = useState('')

  const tenants = Array.isArray(data) ? data : data?.items ?? []

  const filtered = tenants.filter(t =>
    !search || t.name.toLowerCase().includes(search.toLowerCase()) || t.slug.toLowerCase().includes(search.toLowerCase())
  )

  const handleSwitch = (tenantId: string, tenantName: string) => {
    if (tenantId === activeTenantId) { onClose(); return }
    switchTenant(tenantId, tenantName)
    queryClient.clear()
    enqueueSnackbar(`Switched to "${tenantName}"`, { variant: 'success' })
    onClose()
  }

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>
        <Typography variant="h6" sx={{ fontWeight: 700 }}>Switch Tenant</Typography>
        <Typography variant="body2" color="text.secondary">
          Select a tenant to view and manage its data
        </Typography>
      </DialogTitle>

      <DialogContent sx={{ pt: 1, px: 2, pb: 0 }}>
        <TextField
          fullWidth
          size="small"
          placeholder="Search tenants…"
          value={search}
          onChange={e => setSearch(e.target.value)}
          sx={{ mb: 1 }}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start"><Search fontSize="small" /></InputAdornment>
              ),
            },
          }}
        />

        {isLoading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
            <CircularProgress size={28} />
          </Box>
        ) : isError ? (
          <Alert severity="error">Failed to load tenants.</Alert>
        ) : filtered.length === 0 ? (
          <Alert severity="info">No tenants found.</Alert>
        ) : (
          <List disablePadding>
            {filtered.map(tenant => {
              const isActive = tenant.id === activeTenantId
              return (
                <ListItemButton
                  key={tenant.id}
                  selected={isActive}
                  onClick={() => handleSwitch(tenant.id, tenant.name)}
                  sx={{
                    borderRadius: 1,
                    mb: 0.5,
                    border: '1px solid',
                    borderColor: isActive ? 'primary.main' : 'divider',
                    '&.Mui-selected': {
                      bgcolor: 'primary.50',
                      '&:hover': { bgcolor: 'primary.100' },
                    },
                  }}
                >
                  <ListItemIcon sx={{ minWidth: 36 }}>
                    <ApartmentOutlined color={isActive ? 'primary' : 'action'} />
                  </ListItemIcon>
                  <ListItemText
                    primary={
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Typography variant="body2" sx={{ fontWeight: isActive ? 700 : 400 }}>
                          {tenant.name}
                        </Typography>
                        {!tenant.isActive && (
                          <Chip label="Inactive" size="small" color="default" />
                        )}
                      </Box>
                    }
                    secondary={
                      <Typography variant="caption" color="text.secondary">
                        {tenant.slug}
                        {tenant.planName ? ` · ${tenant.planName}` : ''}
                        {tenant.userCount != null ? ` · ${tenant.userCount} users` : ''}
                      </Typography>
                    }
                  />
                  {isActive && <CheckCircle color="primary" sx={{ fontSize: 18 }} />}
                </ListItemButton>
              )
            })}
          </List>
        )}
      </DialogContent>

      <DialogActions sx={{ px: 2, py: 1.5 }}>
        <Button onClick={onClose}>Close</Button>
      </DialogActions>
    </Dialog>
  )
}
