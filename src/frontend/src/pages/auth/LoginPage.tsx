import { Alert, Box, Divider, Link as MuiLink, Stack, Typography } from '@mui/material'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Link } from 'react-router-dom'
import { WorkOutlined } from '@mui/icons-material'
import { AuthLayout } from '@/components/layout/AuthLayout'
import { Button, FormCheckbox, FormTextField } from '@/components/ui'
import { useLogin } from '@/features/auth/hooks/useAuth'
import { extractApiError } from '@/api/apiClient'

const schema = z.object({
  email: z.string().email('Enter a valid email'),
  password: z.string().min(1, 'Password is required'),
  rememberMe: z.boolean().optional(),
})

type FormData = z.infer<typeof schema>

export function LoginPage() {
  const { control, handleSubmit } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { email: '', password: '', rememberMe: false },
  })
  const login = useLogin()

  const errorMsg = login.isError ? extractApiError(login.error).message : ''
  const isUnverified = errorMsg.toLowerCase().includes('not been verified')

  return (
    <AuthLayout>
      <Stack spacing={3}>
        {/* Brand */}
        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 1 }}>
          <WorkOutlined color="primary" sx={{ fontSize: 40 }} />
          <Typography variant="h5" sx={{ fontWeight: 700 }}>
            Welcome back
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Sign in to your JobPortal account
          </Typography>
        </Box>

        {/* Form */}
        <Box component="form" onSubmit={handleSubmit((data) => login.mutate(data))}>
          <Stack spacing={2}>
            <FormTextField name="email" control={control} label="Email address" type="email" autoComplete="email" autoFocus />
            <FormTextField name="password" control={control} label="Password" type="password" autoComplete="current-password" />

            {login.isError && (
              <Alert severity="error" sx={{ py: 0.5 }}>
                {errorMsg}
                {isUnverified && (
                  <Box sx={{ mt: 0.5 }}>
                    <MuiLink component={Link} to="/resend-verification" variant="body2">
                      Resend verification email →
                    </MuiLink>
                  </Box>
                )}
              </Alert>
            )}

            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
              <FormCheckbox name="rememberMe" control={control} label="Remember me" />
              <MuiLink component={Link} to="/forgot-password" variant="body2">
                Forgot password?
              </MuiLink>
            </Box>

            <Button type="submit" variant="contained" size="large" fullWidth loading={login.isPending}>
              Sign In
            </Button>
          </Stack>
        </Box>

        <Divider />

        <Typography variant="body2" align="center" color="text.secondary">
          Don&apos;t have an account?{' '}
          <MuiLink component={Link} to="/register" sx={{ fontWeight: 500 }}>
            Create account
          </MuiLink>
        </Typography>
      </Stack>
    </AuthLayout>
  )
}
