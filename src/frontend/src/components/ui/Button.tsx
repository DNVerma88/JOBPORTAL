import {
  Button as MuiButton,
  CircularProgress,
  type ButtonProps as MuiButtonProps,
} from '@mui/material'
import type { ReactNode } from 'react'

interface ButtonProps extends MuiButtonProps {
  loading?: boolean
  children: ReactNode
}

export function Button({ loading = false, disabled, children, startIcon, ...rest }: ButtonProps) {
  return (
    <MuiButton
      disabled={disabled || loading}
      startIcon={loading ? <CircularProgress size={16} color="inherit" /> : startIcon}
      {...rest}
    >
      {children}
    </MuiButton>
  )
}
