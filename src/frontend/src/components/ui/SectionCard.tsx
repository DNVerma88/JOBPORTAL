import { Card, CardContent, CardHeader, Divider, type SxProps, type Theme } from '@mui/material'
import type { ReactNode } from 'react'

interface SectionCardProps {
  title?: string
  subheader?: string
  action?: ReactNode
  children: ReactNode
  sx?: SxProps<Theme>
  noPadding?: boolean
}

export function SectionCard({ title, subheader, action, children, sx, noPadding }: SectionCardProps) {
  return (
    <Card sx={sx}>
      {title && (
        <>
          <CardHeader
            title={title}
            subheader={subheader}
            action={action}
            titleTypographyProps={{ variant: 'subtitle1', fontWeight: 600 }}
            subheaderTypographyProps={{ variant: 'body2' }}
          />
          <Divider />
        </>
      )}
      <CardContent sx={noPadding ? { p: '0 !important' } : undefined}>
        {children}
      </CardContent>
    </Card>
  )
}
