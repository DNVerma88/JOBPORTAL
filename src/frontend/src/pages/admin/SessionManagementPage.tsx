import {
  Box, Button, Chip, CircularProgress, Paper,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography,
} from '@mui/material'
import { Block as RevokeIcon } from '@mui/icons-material'
import { useSessions, useRevokeSession } from '@/features/admin/adminApi'
import { PageHeader } from '@/components/ui'

export function SessionManagementPage() {
  const { data: sessionsData, isLoading } = useSessions()
  const sessions = Array.isArray(sessionsData) ? sessionsData : (sessionsData as any)?.items ?? []
  const revoke = useRevokeSession()

  return (
    <Box>
      <PageHeader
        title="Active Sessions"
        subtitle="Monitor and revoke user sessions across the platform"
      />

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 6 }}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            {sessions.length} active session{sessions.length !== 1 ? 's' : ''}
          </Typography>
          <TableContainer component={Paper}>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>User</TableCell>
                  <TableCell>IP Address</TableCell>
                  <TableCell>Device</TableCell>
                  <TableCell>Started</TableCell>
                  <TableCell>Last Active</TableCell>
                  <TableCell>Expires</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Action</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {sessions.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={8} align="center" sx={{ py: 4, color: 'text.secondary' }}>
                      No active sessions found
                    </TableCell>
                  </TableRow>
                ) : sessions.map((s: any) => (
                  <TableRow key={s.id} hover>
                    <TableCell>{s.userEmail ?? s.userId}</TableCell>
                    <TableCell>{s.ipAddress ?? '—'}</TableCell>
                    <TableCell sx={{ maxWidth: 200, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                      {s.deviceType ?? s.deviceInfo ?? '—'}
                    </TableCell>
                    <TableCell>{new Date(s.createdOn).toLocaleString()}</TableCell>
                    <TableCell>
                      {s.lastActivityAt
                        ? new Date(s.lastActivityAt).toLocaleString()
                        : s.lastActivityOn
                          ? new Date(s.lastActivityOn).toLocaleString()
                          : '—'}
                    </TableCell>
                    <TableCell>
                      {s.expiresAt ? new Date(s.expiresAt).toLocaleString() : '—'}
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={s.isActive ? 'Active' : 'Expired'}
                        size="small"
                        color={s.isActive ? 'success' : 'default'}
                      />
                    </TableCell>
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
        </>
      )}
    </Box>
  )
}

export default SessionManagementPage
