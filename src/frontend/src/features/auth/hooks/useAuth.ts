import { useMutation } from '@tanstack/react-query'
import { useSnackbar } from 'notistack'
import { useNavigate } from 'react-router-dom'
import { authApi } from './authApi'
import { useAuthStore } from '../store/authStore'
import { extractApiError } from '@/api/apiClient'

export function useLogin() {
  const { setAccessToken, setUser } = useAuthStore()
  const { enqueueSnackbar } = useSnackbar()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: authApi.login,
    onSuccess: ({ data }) => {
      setAccessToken(data.accessToken)
      setUser({
        id: data.userId,
        firstName: data.fullName.split(' ')[0],
        lastName: data.fullName.split(' ').slice(1).join(' '),
        email: data.email,
        role: data.roles[0] ?? '',
        tenantId: '00000000-0000-0000-0000-000000000001',
      })
      enqueueSnackbar('Welcome back!', { variant: 'success' })
      navigate('/')
    },
    // error is displayed inline in LoginPage — no toast needed here
  })
}

export function useRegister() {
  const { enqueueSnackbar } = useSnackbar()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: authApi.register,
    onSuccess: () => {
      enqueueSnackbar('Account created! Please check your email to verify your account.', { variant: 'success' })
      navigate('/login')
    },
    onError: (error) => {
      const { message } = extractApiError(error)
      enqueueSnackbar(message, { variant: 'error' })
    },
  })
}

export function useForgotPassword() {
  const { enqueueSnackbar } = useSnackbar()

  return useMutation({
    mutationFn: (email: string) => authApi.forgotPassword(email),
    onSuccess: () => {
      enqueueSnackbar('Password reset instructions sent to your email.', { variant: 'success' })
    },
    onError: (error) => {
      const { message } = extractApiError(error)
      enqueueSnackbar(message, { variant: 'error' })
    },
  })
}

export function useResetPassword() {
  const { enqueueSnackbar } = useSnackbar()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: ({ token, password, confirmPassword }: { token: string; password: string; confirmPassword: string }) =>
      authApi.resetPassword(token, password, confirmPassword),
    onSuccess: () => {
      enqueueSnackbar('Password reset successfully. Please sign in.', { variant: 'success' })
      navigate('/login')
    },
    onError: (error) => {
      const { message } = extractApiError(error)
      enqueueSnackbar(message, { variant: 'error' })
    },
  })
}
