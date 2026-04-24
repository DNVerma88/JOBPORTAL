import { Box, Typography, type SxProps, type Theme } from '@mui/material'
import { InboxOutlined } from '@mui/icons-material'
import type { ReactNode } from 'react'

interface EmptyStateProps {
  icon?: ReactNode
  title: string
  description?: string
  action?: ReactNode
  sx?: SxProps<Theme>
}

export function EmptyState({ icon, title, description, action, sx }: EmptyStateProps) {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        py: 8,
        px: 2,
        textAlign: 'center',
        gap: 2,
        ...sx,
      }}
    >
      <Box sx={{ color: 'text.disabled', '& svg': { fontSize: 64 } }}>
        {icon ?? <InboxOutlined />}
      </Box>
      <Box>
        <Typography variant="h6" sx={{ fontWeight: 600 }} gutterBottom>
          {title}
        </Typography>
        {description && (
          <Typography variant="body2" color="text.secondary" sx={{ maxWidth: 400 }}>
            {description}
          </Typography>
        )}
      </Box>
      {action}
    </Box>
  )
}
