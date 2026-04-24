import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Box, Link as MuiLink, Stack, Typography } from '@mui/material'
import { MarkEmailReadOutlined } from '@mui/icons-material'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { AuthLayout } from '@/components/layout/AuthLayout'
import { Button, FormTextField } from '@/components/ui'
import { authApi } from '@/features/auth/hooks/authApi'

const schema = z.object({ email: z.string().email('Enter a valid email') })
type FormData = z.infer<typeof schema>

export function ResendVerificationPage() {
  const [sent, setSent] = useState(false)
  const [loading, setLoading] = useState(false)

  const { control, handleSubmit, formState } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { email: '' },
  })

  const onSubmit = async ({ email }: FormData) => {
    setLoading(true)
    try {
      await authApi.resendVerification(email)
    } finally {
      setLoading(false)
      setSent(true)
    }
  }

  if (sent) {
    return (
      <AuthLayout>
        <Stack spacing={2} sx={{ alignItems: 'center', textAlign: 'center' }}>
          <MarkEmailReadOutlined color="primary" sx={{ fontSize: 56 }} />
          <Typography variant="h6" sx={{ fontWeight: 600 }}>Check your email</Typography>
          <Typography variant="body2" color="text.secondary">
            If an unverified account exists for that address, we&apos;ve sent a new verification link.
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
          <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>Resend Verification</Typography>
          <Typography variant="body2" color="text.secondary">
            Enter your email and we&apos;ll send a new verification link.
          </Typography>
        </Box>
        <Box component="form" onSubmit={handleSubmit(onSubmit)}>
          <Stack spacing={2}>
            <FormTextField name="email" control={control} label="Email address" type="email" autoFocus />
            <Button type="submit" variant="contained" size="large" fullWidth loading={loading} disabled={!formState.isDirty}>
              Send Verification Link
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
