import { Box, CircularProgress } from '@mui/material'

interface LoadingSpinnerProps {
  fullPage?: boolean
  size?: number
}

export function LoadingSpinner({ fullPage = false, size = 40 }: LoadingSpinnerProps) {
  if (fullPage) {
    return (
      <Box
        sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '60vh' }}
      >
        <CircularProgress size={size} />
      </Box>
    )
  }
  return <CircularProgress size={size} />
}
