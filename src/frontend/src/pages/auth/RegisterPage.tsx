import {
  Box,
  Divider,
  Link as MuiLink,
  Stack,
  ToggleButton,
  ToggleButtonGroup,
  Typography,
} from '@mui/material'
import { Person, Business } from '@mui/icons-material'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Link } from 'react-router-dom'
import { useState } from 'react'
import { AuthLayout } from '@/components/layout/AuthLayout'
import { Button, FormTextField } from '@/components/ui'
import { useRegister } from '@/features/auth/hooks/useAuth'

const baseSchema = z.object({
  firstName: z.string().min(2, 'Min 2 characters'),
  lastName: z.string().min(2, 'Min 2 characters'),
  email: z.string().email('Enter a valid email'),
  password: z
    .string()
    .min(8, 'Min 8 characters')
    .regex(/[A-Z]/, 'Must contain an uppercase letter')
    .regex(/[0-9]/, 'Must contain a number'),
  confirmPassword: z.string(),
  tenantSlug: z.string().optional(),
  companyName: z.string().optional(),
})

const schema = baseSchema.refine((d) => d.password === d.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword'],
})

type FormData = z.infer<typeof schema>

export function RegisterPage() {
  const [role, setRole] = useState<'JobSeeker' | 'Recruiter'>('JobSeeker')
  const { control, handleSubmit } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { firstName: '', lastName: '', email: '', password: '', confirmPassword: '' },
  })
  const register = useRegister()

  return (
    <AuthLayout>
      <Stack spacing={3}>
        <Box sx={{ textAlign: 'center' }}>
          <Typography variant="h5" sx={{ fontWeight: 700 }}>
            Create your account
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Join thousands of professionals on JobPortal
          </Typography>
        </Box>

        {/* Role toggle */}
        <Box sx={{ display: 'flex', justifyContent: 'center' }}>
          <ToggleButtonGroup
            value={role}
            exclusive
            onChange={(_, v) => v && setRole(v)}
            size="small"
            color="primary"
          >
            <ToggleButton value="JobSeeker" sx={{ gap: 0.5, px: 2 }}>
              <Person fontSize="small" /> Job Seeker
            </ToggleButton>
            <ToggleButton value="Recruiter" sx={{ gap: 0.5, px: 2 }}>
              <Business fontSize="small" /> Recruiter
            </ToggleButton>
          </ToggleButtonGroup>
        </Box>

        <Box
          component="form"
          onSubmit={handleSubmit((data) => register.mutate({ ...data, role }))}
        >
          <Stack spacing={2}>
            <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 2 }}>
              <FormTextField name="firstName" control={control} label="First name" autoFocus />
              <FormTextField name="lastName" control={control} label="Last name" />
            </Box>
            <FormTextField name="email" control={control} label="Email address" type="email" autoComplete="email" />
            <FormTextField name="password" control={control} label="Password" type="password" />
            <FormTextField name="confirmPassword" control={control} label="Confirm password" type="password" />

            {role === 'Recruiter' && (
              <>
                <FormTextField name="companyName" control={control} label="Company name" />
                <FormTextField name="tenantSlug" control={control} label="Company URL slug" />
              </>
            )}

            <Button type="submit" variant="contained" size="large" fullWidth loading={register.isPending}>
              Create Account
            </Button>

            <Typography variant="caption" color="text.secondary" align="center">
              By creating an account you agree to our{' '}
              <MuiLink href="/terms" target="_blank">Terms</MuiLink> and{' '}
              <MuiLink href="/privacy" target="_blank">Privacy Policy</MuiLink>.
            </Typography>
          </Stack>
        </Box>

        <Divider />

        <Typography variant="body2" align="center" color="text.secondary">
          Already have an account?{' '}
          <MuiLink component={Link} to="/login" sx={{ fontWeight: 500 }}>
            Sign in
          </MuiLink>
        </Typography>
      </Stack>
    </AuthLayout>
  )
}
