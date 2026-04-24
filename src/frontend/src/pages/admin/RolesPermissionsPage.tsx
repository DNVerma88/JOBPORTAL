import { useState } from 'react'
import {
  Box, Grid, List, ListItemButton, ListItemText, Paper, Stack, Typography,
  Checkbox, FormControlLabel, Button, Divider, Chip, CircularProgress
} from '@mui/material'
import { useRoles, usePermissions, useRolePermissions, useUpdateRolePermissions } from '@/features/admin/adminApi'
import { PageHeader, SectionCard } from '@/components/ui'

export default function RolesPermissionsPage() {
  const [selectedRoleId, setSelectedRoleId] = useState<string | null>(null)
  const [localPerms, setLocalPerms] = useState<string[]>([])

  const { data: roles = [], isLoading: rolesLoading } = useRoles()
  const { data: permissions = [] } = usePermissions()
  const { data: rolePerms, isFetching } = useRolePermissions(selectedRoleId ?? '')
  const updatePerms = useUpdateRolePermissions()

  const handleSelectRole = (id: string) => {
    setSelectedRoleId(id)
    setLocalPerms([])
  }

  // Sync rolePerms into local state when loaded
  const effectivePerms = localPerms.length > 0 ? localPerms : (rolePerms?.map((p: any) => p.permissionId) ?? [])

  const togglePerm = (permId: string) => {
    setLocalPerms(prev => {
      const base = localPerms.length > 0 ? prev : (rolePerms?.map((p: any) => p.permissionId) ?? [])
      return base.includes(permId) ? base.filter(id => id !== permId) : [...base, permId]
    })
  }

  const grouped = permissions.reduce((acc: Record<string, any[]>, p: any) => {
    const module = p.module ?? 'General'
    if (!acc[module]) acc[module] = []
    acc[module].push(p)
    return acc
  }, {})

  const handleSave = () => {
    if (!selectedRoleId) return
    const perms = localPerms.length > 0 ? localPerms : effectivePerms
    updatePerms.mutate({ roleId: selectedRoleId, permissionIds: perms })
  }

  const selectedRole = roles.find((r: any) => r.id === selectedRoleId)

  return (
    <Box>
      <PageHeader title="Roles & Permissions" subtitle="Configure what each role can do in the system" />

      <Grid container spacing={2}>
        {/* Role list */}
        <Grid size={{ xs: 12, md: 3 }}>
          <SectionCard title="Roles">
            {rolesLoading ? (
              <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}><CircularProgress size={24} /></Box>
            ) : (
              <List disablePadding>
                {roles.map((role: any) => (
                  <ListItemButton
                    key={role.id}
                    selected={selectedRoleId === role.id}
                    onClick={() => handleSelectRole(role.id)}
                    sx={{ borderRadius: 1, mb: 0.5 }}
                  >
                    <ListItemText
                      primary={
                        <Typography variant="body2" sx={{ fontWeight: selectedRoleId === role.id ? 700 : 400 }}>
                          {role.name}
                        </Typography>
                      }
                      secondary={role.description}
                    />
                  </ListItemButton>
                ))}
              </List>
            )}
          </SectionCard>
        </Grid>

        {/* Permissions */}
        <Grid size={{ xs: 12, md: 9 }}>
          <Paper sx={{ p: 2 }}>
            {!selectedRoleId ? (
              <Box sx={{ p: 6, textAlign: 'center' }}>
                <Typography color="text.secondary">Select a role to manage its permissions</Typography>
              </Box>
            ) : (
              <>
                <Stack direction="row" sx={{ alignItems: 'center', justifyContent: "space-between" }}>
                  <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                    <Typography variant="h6">Permissions for</Typography>
                    <Chip label={selectedRole?.name} color="primary" />
                  </Stack>
                  <Button
                    variant="contained"
                    onClick={handleSave}
                    disabled={updatePerms.isPending || isFetching}
                  >
                    {updatePerms.isPending ? 'Saving...' : 'Save Permissions'}
                  </Button>
                </Stack>
                <Divider sx={{ mb: 2 }} />
                {isFetching ? (
                  <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress size={32} /></Box>
                ) : (
                  <Grid container spacing={2}>
                    {Object.entries(grouped).map(([module, perms]) => (
                      <Grid size={{ xs: 12, sm: 6, md: 4 }} key={module}>
                        <Paper variant="outlined" sx={{ p: 1.5 }}>
                          <Typography variant="caption" sx={{ fontWeight: 700, textTransform: 'uppercase', color: 'text.secondary' }}>
                            {module}
                          </Typography>
                          {(perms as any[]).map((p) => (
                            <FormControlLabel
                              key={p.id}
                              control={
                                <Checkbox
                                  size="small"
                                  checked={effectivePerms.includes(p.id)}
                                  onChange={() => togglePerm(p.id)}
                                />
                              }
                              label={<Typography variant="body2">{p.name}</Typography>}
                              sx={{ display: 'flex', ml: 0.5 }}
                            />
                          ))}
                        </Paper>
                      </Grid>
                    ))}
                  </Grid>
                )}
              </>
            )}
          </Paper>
        </Grid>
      </Grid>
    </Box>
  )
}
