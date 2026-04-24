import { Box, Link as MuiLink, Stack, Typography } from '@mui/material'
import { MarkEmailReadOutlined } from '@mui/icons-material'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Link } from 'react-router-dom'
import { AuthLayout } from '@/components/layout/AuthLayout'
import { Button, FormTextField } from '@/components/ui'
import { useForgotPassword } from '@/features/auth/hooks/useAuth'

const schema = z.object({
  email: z.string().email('Enter a valid email'),
})

type FormData = z.infer<typeof schema>

export function ForgotPasswordPage() {
  const { control, handleSubmit, formState } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { email: '' },
  })
  const forgotPassword = useForgotPassword()

  if (forgotPassword.isSuccess) {
    return (
      <AuthLayout>
        <Stack spacing={2} sx={{ alignItems: 'center', textAlign: 'center' }}>
          <MarkEmailReadOutlined color="primary" sx={{ fontSize: 56 }} />
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Check your email
          </Typography>
          <Typography variant="body2" color="text.secondary">
            We&apos;ve sent a password reset link. It expires in 15 minutes.
          </Typography>
          <MuiLink component={Link} to="/login" variant="body2" sx={{ fontWeight: 500 }}>
            Back to sign in
          </MuiLink>
        </Stack>
      </AuthLayout>
    )
  }

  return (
    <AuthLayout>
      <Stack spacing={3}>
        <Box>
          <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>
            Forgot password?
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Enter your email and we&apos;ll send you a reset link.
          </Typography>
        </Box>

        <Box component="form" onSubmit={handleSubmit(({ email }) => forgotPassword.mutate(email))}>
          <Stack spacing={2}>
            <FormTextField name="email" control={control} label="Email address" type="email" autoFocus />
            <Button
              type="submit"
              variant="contained"
              size="large"
              fullWidth
              loading={forgotPassword.isPending}
              disabled={!formState.isDirty}
            >
              Send Reset Link
            </Button>
          </Stack>
        </Box>

        <MuiLink component={Link} to="/login" variant="body2" sx={{ textAlign: 'center' }}>
          Back to sign in
        </MuiLink>
      </Stack>
    </AuthLayout>
  )
}
