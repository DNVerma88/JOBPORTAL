import { Alert, Box, Button, Typography } from '@mui/material'
import { ErrorOutlined } from '@mui/icons-material'

interface ErrorAlertProps {
  message: string
  title?: string
  onRetry?: () => void
}

export function ErrorAlert({ message, title = 'Something went wrong', onRetry }: ErrorAlertProps) {
  return (
    <Box sx={{ p: 3 }}>
      <Alert
        severity="error"
        icon={<ErrorOutlined />}
        action={
          onRetry ? (
            <Button color="error" size="small" onClick={onRetry}>
              Retry
            </Button>
          ) : undefined
        }
      >
        <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
          {title}
        </Typography>
        <Typography variant="body2">{message}</Typography>
      </Alert>
    </Box>
  )
}
