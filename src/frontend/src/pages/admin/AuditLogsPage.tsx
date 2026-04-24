import { useState } from 'react'
import {
  Box, Tab, Tabs, Stack, Button, Chip, MenuItem, Select, FormControl, InputLabel,
  Paper, TableContainer, Table, TableHead, TableRow, TableCell, TableBody,
  CircularProgress
} from '@mui/material'
import { Block as RevokeIcon } from '@mui/icons-material'
import { useAuditLogs, useSessions, useRevokeSession } from '@/features/admin/adminApi'
import { PageHeader, StatusChip } from '@/components/ui'

function AuditLogsTab() {
  const [actionFilter, setActionFilter] = useState('')
  const { data, isLoading } = useAuditLogs({ action: actionFilter })
  const rows = Array.isArray(data) ? data : (data as any)?.items ?? []

  return (
    <Box>
      <Stack direction="row" spacing={2} sx={{ mb: 2 }}>
        <FormControl size="small" sx={{ minWidth: 180 }}>
          <InputLabel>Action</InputLabel>
          <Select value={actionFilter} label="Action" onChange={e => setActionFilter(e.target.value)}>
            <MenuItem value="">All Actions</MenuItem>
            <MenuItem value="Login">Login</MenuItem>
            <MenuItem value="Logout">Logout</MenuItem>
            <MenuItem value="Create">Create</MenuItem>
            <MenuItem value="Update">Update</MenuItem>
            <MenuItem value="Delete">Delete</MenuItem>
          </Select>
        </FormControl>
      </Stack>

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 6 }}><CircularProgress /></Box>
      ) : (
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Timestamp</TableCell>
                <TableCell>User</TableCell>
                <TableCell>Action</TableCell>
                <TableCell>Entity</TableCell>
                <TableCell>Entity ID</TableCell>
                <TableCell>IP</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={6} align="center" sx={{ py: 4, color: 'text.secondary' }}>
                    No audit logs found
                  </TableCell>
                </TableRow>
              ) : rows.map((log: any) => (
                <TableRow key={log.id} hover>
                  <TableCell>{new Date(log.createdOn).toLocaleString()}</TableCell>
                  <TableCell>{log.userEmail ?? '—'}</TableCell>
                  <TableCell>
                    <Chip label={log.action} size="small"
                      color={log.action === 'Delete' ? 'error' : log.action === 'Create' ? 'success' : 'default'}
                    />
                  </TableCell>
                  <TableCell>{log.entityType}</TableCell>
                  <TableCell sx={{ fontFamily: 'monospace', fontSize: 11 }}>{log.entityId?.slice(0, 8)}…</TableCell>
                  <TableCell>{log.ipAddress ?? '—'}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  )
}

function SessionsTab() {
  const { data: sessionsData, isLoading } = useSessions()
  const data = Array.isArray(sessionsData) ? sessionsData : (sessionsData as any)?.items ?? []
  const revoke = useRevokeSession()

  return (
    <Box>
      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 6 }}><CircularProgress /></Box>
      ) : (
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>User</TableCell>
                <TableCell>IP Address</TableCell>
                <TableCell>Device</TableCell>
                <TableCell>Started</TableCell>
                <TableCell>Last Active</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Action</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {(data as any[]).length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} align="center" sx={{ py: 4, color: 'text.secondary' }}>
                    No active sessions
                  </TableCell>
                </TableRow>
              ) : (data as any[]).map((s) => (
                <TableRow key={s.id} hover>
                  <TableCell>{s.userEmail}</TableCell>
                  <TableCell>{s.ipAddress}</TableCell>
                  <TableCell sx={{ maxWidth: 180, overflow: 'hidden', textOverflow: 'ellipsis' }}>
                    {s.deviceInfo ?? '—'}
                  </TableCell>
                  <TableCell>{new Date(s.createdOn).toLocaleString()}</TableCell>
                  <TableCell>{s.lastActivityOn ? new Date(s.lastActivityOn).toLocaleString() : '—'}</TableCell>
                  <TableCell><StatusChip status={s.isActive ? 'Active' : 'Expired'} /></TableCell>
                  <TableCell>
                    {s.isActive && (
                      <Button
                        size="small"
                        color="error"
                        startIcon={<RevokeIcon />}
                        onClick={() => revoke.mutate(s.id)}
                        disabled={revoke.isPending}
                      >
                        Revoke
                      </Button>
                    )}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  )
}

export default function AuditLogsPage() {
  const [tab, setTab] = useState(0)

  return (
    <Box>
      <PageHeader title="Audit Logs" subtitle="Track all system activity and active sessions" />
      <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 2 }}>
        <Tab label="Audit Logs" />
        <Tab label="Active Sessions" />
      </Tabs>
      {tab === 0 && <AuditLogsTab />}
      {tab === 1 && <SessionsTab />}
    </Box>
  )
}
