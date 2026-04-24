import { useState } from 'react'
import {
  Box,
  Grid,
  Typography,
  TextField,
  Divider,
  FormControlLabel,
  Switch,
  Alert,
  CircularProgress,
} from '@mui/material'
import { useAuthStore } from '@/features/auth/store/authStore'
import { useThemeStore } from '@/store/themeStore'
import { PageHeader, SectionCard, Button } from '@/components/ui'
import { useSnackbar } from 'notistack'
import { useChangePassword } from '@/features/auth/hooks/authApi'

export function SettingsPage() {
  const user = useAuthStore((s) => s.user)
  const { isDarkMode, toggle } = useThemeStore()
  const { enqueueSnackbar } = useSnackbar()

  const changePassword = useChangePassword()

  const [passwords, setPasswords] = useState({ current: '', next: '', confirm: '' })
  const [emailPrefs, setEmailPrefs] = useState({
    jobAlerts: true,
    applicationUpdates: true,
    marketingEmails: false,
    weeklyDigest: true,
  })

  const handleChangePassword = () => {
    if (passwords.next !== passwords.confirm) {
      enqueueSnackbar('Passwords do not match', { variant: 'error' })
      return
    }
    if (passwords.next.length < 8) {
      enqueueSnackbar('Password must be at least 8 characters', { variant: 'error' })
      return
    }
    changePassword.mutate(
      { currentPassword: passwords.current, newPassword: passwords.next },
      { onSuccess: () => setPasswords({ current: '', next: '', confirm: '' }) }
    )
  }

  return (
    <Box>
      <PageHeader title="Settings" subtitle="Manage your account, security, and preferences" />

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 7 }}>
          {/* Account info */}
          <SectionCard title="Account Information" sx={{ mb: 3 }}>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
              <Grid container spacing={2}>
                <Grid size={6}>
                  <TextField label="First Name" value={user?.firstName ?? ''} disabled size="small" fullWidth />
                </Grid>
                <Grid size={6}>
                  <TextField label="Last Name" value={user?.lastName ?? ''} disabled size="small" fullWidth />
                </Grid>
              </Grid>
              <TextField label="Email" value={user?.email ?? ''} disabled size="small" fullWidth />
              <TextField label="Role" value={user?.role ?? ''} disabled size="small" fullWidth />
              <Alert severity="info" sx={{ mt: 1 }}>
                To update your name or email, please contact support.
              </Alert>
            </Box>
          </SectionCard>

          {/* Change password */}
          <SectionCard title="Change Password" sx={{ mb: 3 }}>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
              <TextField
                label="Current Password"
                type="password"
                value={passwords.current}
                onChange={(e) => setPasswords((p) => ({ ...p, current: e.target.value }))}
                size="small"
                autoComplete="current-password"
              />
              <TextField
                label="New Password"
                type="password"
                value={passwords.next}
                onChange={(e) => setPasswords((p) => ({ ...p, next: e.target.value }))}
                size="small"
                autoComplete="new-password"
                helperText="Minimum 8 characters"
              />
              <TextField
                label="Confirm New Password"
                type="password"
                value={passwords.confirm}
                onChange={(e) => setPasswords((p) => ({ ...p, confirm: e.target.value }))}
                size="small"
                autoComplete="new-password"
                error={!!passwords.confirm && passwords.next !== passwords.confirm}
                helperText={passwords.confirm && passwords.next !== passwords.confirm ? 'Passwords do not match' : ''}
              />
              <Button
                variant="outlined"
                onClick={handleChangePassword}
                disabled={!passwords.current || !passwords.next || !passwords.confirm || changePassword.isPending}
                startIcon={changePassword.isPending ? <CircularProgress size={16} /> : undefined}
                sx={{ alignSelf: 'flex-start' }}
              >
                {changePassword.isPending ? 'Updating…' : 'Update Password'}
              </Button>
            </Box>
          </SectionCard>
        </Grid>

        <Grid size={{ xs: 12, md: 5 }}>
          {/* Appearance */}
          <SectionCard title="Appearance" sx={{ mb: 3 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <Box>
                <Typography variant="body2" sx={{ fontWeight: 500 }}>Dark Mode</Typography>
                <Typography variant="caption" color="text.secondary">Use dark theme across the application</Typography>
              </Box>
              <Switch checked={isDarkMode} onChange={toggle} />
            </Box>
          </SectionCard>

          {/* Email preferences */}
          <SectionCard title="Email Preferences">
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.5 }}>
              {(Object.keys(emailPrefs) as Array<keyof typeof emailPrefs>).map((key) => (
                <Box key={key}>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={emailPrefs[key]}
                        onChange={(e) => setEmailPrefs((p) => ({ ...p, [key]: e.target.checked }))}
                        size="small"
                      />
                    }
                    label={
                      <Typography variant="body2">
                        {key === 'jobAlerts' && 'Job Alerts'}
                        {key === 'applicationUpdates' && 'Application Updates'}
                        {key === 'marketingEmails' && 'Marketing Emails'}
                        {key === 'weeklyDigest' && 'Weekly Digest'}
                      </Typography>
                    }
                  />
                  <Divider />
                </Box>
              ))}
              <Button variant="outlined" size="small" sx={{ mt: 1, alignSelf: 'flex-start' }} onClick={() => enqueueSnackbar('Email preferences will be saved in a future update.', { variant: 'info' })}>
                Save Preferences
              </Button>
            </Box>
          </SectionCard>
        </Grid>
      </Grid>
    </Box>
  )
}
