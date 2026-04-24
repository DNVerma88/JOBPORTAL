import { Box, Link as MuiLink, Stack, Typography } from '@mui/material'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Link, useSearchParams } from 'react-router-dom'
import { AuthLayout } from '@/components/layout/AuthLayout'
import { Button, FormTextField, ErrorAlert } from '@/components/ui'
import { useResetPassword } from '@/features/auth/hooks/useAuth'

const schema = z
  .object({
    password: z
      .string()
      .min(8, 'Min 8 characters')
      .regex(/[A-Z]/, 'Must contain an uppercase letter')
      .regex(/[0-9]/, 'Must contain a number'),
    confirmPassword: z.string(),
  })
  .refine((d) => d.password === d.confirmPassword, {
    message: "Passwords don't match",
    path: ['confirmPassword'],
  })

type FormData = z.infer<typeof schema>

export function ResetPasswordPage() {
  const [searchParams] = useSearchParams()
  const token = searchParams.get('token') ?? ''
  const { control, handleSubmit } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { password: '', confirmPassword: '' },
  })
  const resetPassword = useResetPassword()

  if (!token) {
    return (
      <AuthLayout>
        <ErrorAlert
          title="Invalid link"
          message="This password reset link is invalid or has expired."
        />
        <MuiLink component={Link} to="/forgot-password" variant="body2" sx={{ display: 'block', textAlign: 'center', mt: 2 }}>
          Request a new link
        </MuiLink>
      </AuthLayout>
    )
  }

  return (
    <AuthLayout>
      <Stack spacing={3}>
        <Box>
          <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>
            Reset password
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Choose a strong new password for your account.
          </Typography>
        </Box>

        <Box
          component="form"
          onSubmit={handleSubmit(({ password, confirmPassword }) =>
            resetPassword.mutate({ token, password, confirmPassword }),
          )}
        >
          <Stack spacing={2}>
            <FormTextField name="password" control={control} label="New password" type="password" autoFocus />
            <FormTextField name="confirmPassword" control={control} label="Confirm new password" type="password" />
            <Button type="submit" variant="contained" size="large" fullWidth loading={resetPassword.isPending}>
              Reset Password
            </Button>
          </Stack>
        </Box>
      </Stack>
    </AuthLayout>
  )
}
