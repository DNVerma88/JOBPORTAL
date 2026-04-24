import { Box, Card, CardContent, Typography, type SxProps, type Theme } from '@mui/material'
import type { ReactNode } from 'react'
import { TrendingUpOutlined, TrendingDownOutlined } from '@mui/icons-material'

interface StatCardProps {
  title: string
  value: string | number
  icon: ReactNode
  trend?: number          // positive = up, negative = down
  trendLabel?: string     // e.g. "vs last month"
  color?: 'primary' | 'success' | 'warning' | 'error' | 'info'
  sx?: SxProps<Theme>
}

const COLOR_MAP = {
  primary: { bg: 'primary.50', icon: 'primary.main' },
  success: { bg: 'success.50', icon: 'success.main' },
  warning: { bg: 'warning.50', icon: 'warning.main' },
  error: { bg: 'error.50', icon: 'error.main' },
  info: { bg: 'info.50', icon: 'info.main' },
} as const

export function StatCard({ title, value, icon, trend, trendLabel, color = 'primary', sx }: StatCardProps) {
  const colors = COLOR_MAP[color]
  const isPositive = (trend ?? 0) >= 0

  return (
    <Card sx={{ height: '100%', ...sx }}>
      <CardContent sx={{ p: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <Box>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              {title}
            </Typography>
            <Typography variant="h4" sx={{ lineHeight: 1.2, fontWeight: 700 }}>
              {value}
            </Typography>
            {trend !== undefined && (
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mt: 1 }}>
                {isPositive ? (
                  <TrendingUpOutlined sx={{ fontSize: 16, color: 'success.main' }} />
                ) : (
                  <TrendingDownOutlined sx={{ fontSize: 16, color: 'error.main' }} />
                )}
                <Typography
                  variant="caption"
                  sx={{ color: isPositive ? 'success.main' : 'error.main', fontWeight: 600 }}
                >
                  {isPositive ? '+' : ''}{trend}%
                </Typography>
                {trendLabel && (
                  <Typography variant="caption" color="text.secondary">
                    {trendLabel}
                  </Typography>
                )}
              </Box>
            )}
          </Box>
          <Box
            sx={{
              width: 48,
              height: 48,
              borderRadius: 2,
              bgcolor: colors.bg,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              color: colors.icon,
              flexShrink: 0,
            }}
          >
            {icon}
          </Box>
        </Box>
      </CardContent>
    </Card>
  )
}
