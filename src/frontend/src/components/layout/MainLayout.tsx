import { Box, Container } from '@mui/material'
import type { ReactNode } from 'react'
import { TopNav } from './TopNav'

interface MainLayoutProps {
  children: ReactNode
  /** Public layout (no sidebar, shows brand in nav) */
  public?: boolean
}

export function MainLayout({ children, public: isPublic = false }: MainLayoutProps) {
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <TopNav showBrand={isPublic} />
      <Box component="main" sx={{ flex: 1 }}>
        <Container maxWidth="xl" sx={{ py: 3 }}>
          {children}
        </Container>
      </Box>
    </Box>
  )
}
