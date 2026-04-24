import { useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { Box, CircularProgress, Link as MuiLink, Stack, Typography } from '@mui/material'
import { CheckCircleOutlined, ErrorOutlined } from '@mui/icons-material'
import { AuthLayout } from '@/components/layout/AuthLayout'
import { authApi } from '@/features/auth/hooks/authApi'

export function VerifyEmailPage() {
  const [searchParams] = useSearchParams()
  const token = searchParams.get('token') ?? ''
  const [state, setState] = useState<'loading' | 'success' | 'error'>('loading')
  const [errorMsg, setErrorMsg] = useState('')

  useEffect(() => {
    if (!token) {
      setState('error')
      setErrorMsg('No verification token found. Please check your email link.')
      return
    }

    authApi
      .verifyEmail(token)
      .then(() => setState('success'))
      .catch((err: any) => {
        setState('error')
        setErrorMsg(
          err?.response?.data?.message ??
          'This verification link is invalid or has expired. Please request a new one.',
        )
      })
  }, [token])

  return (
    <AuthLayout>
      <Stack spacing={3} sx={{ alignItems: 'center', textAlign: 'center', py: 2 }}>
        {state === 'loading' && (
          <>
            <CircularProgress size={48} />
            <Typography variant="h6">Verifying your email…</Typography>
          </>
        )}

        {state === 'success' && (
          <>
            <CheckCircleOutlined color="success" sx={{ fontSize: 64 }} />
            <Typography variant="h5" sx={{ fontWeight: 700 }}>Email Verified!</Typography>
            <Typography variant="body2" color="text.secondary">
              Your email address has been verified. You can now sign in to your account.
            </Typography>
            <MuiLink component={Link} to="/login" variant="body1" sx={{ fontWeight: 600 }}>
              Go to Sign In →
            </MuiLink>
          </>
        )}

        {state === 'error' && (
          <>
            <ErrorOutlined color="error" sx={{ fontSize: 64 }} />
            <Typography variant="h5" sx={{ fontWeight: 700 }}>Verification Failed</Typography>
            <Typography variant="body2" color="text.secondary">{errorMsg}</Typography>
            <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', justifyContent: 'center' }}>
              <MuiLink component={Link} to="/login" variant="body2">Back to Sign In</MuiLink>
              <Typography variant="body2" color="text.secondary">·</Typography>
              <MuiLink component={Link} to="/resend-verification" variant="body2">
                Resend Verification
              </MuiLink>
            </Box>
          </>
        )}
      </Stack>
    </AuthLayout>
  )
}
