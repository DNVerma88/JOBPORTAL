import {
  AppBar,
  Avatar,
  Badge,
  Box,
  Chip,
  Divider,
  IconButton,
  Menu,
  MenuItem,
  Toolbar,
  Tooltip,
  Typography,
  useMediaQuery,
  useTheme,
} from '@mui/material'
import {
  WorkOutlined,
  NotificationsOutlined,
  DarkModeOutlined,
  LightModeOutlined,
  MenuOutlined,
  SwapHorizOutlined,
  ApartmentOutlined,
} from '@mui/icons-material'
import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuthStore } from '@/features/auth/store/authStore'
import { useThemeStore } from '@/store/themeStore'
import { getInitials } from '@/utils/formatters'
import { SwitchTenantDialog } from './SwitchTenantDialog'
import { ROLES } from '@/utils/constants'
import { authApi } from '@/features/auth/hooks/authApi'

interface TopNavProps {
  onMenuClick?: () => void
  showBrand?: boolean
}

export function TopNav({ onMenuClick, showBrand = false }: TopNavProps) {
  const { user, isAuthenticated, logout, activeTenantId, activeTenantName } = useAuthStore()
  const { isDarkMode, toggle } = useThemeStore()
  const navigate = useNavigate()
  const theme = useTheme()
  const isMobile = useMediaQuery(theme.breakpoints.down('md'))
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)
  const [switchTenantOpen, setSwitchTenantOpen] = useState(false)

  const canSwitchTenant = user?.role === ROLES.SUPER_ADMIN || user?.role === ROLES.TENANT_ADMIN
  // Show tenant badge when actively viewing a non-system tenant
  const isSystemTenant = activeTenantId === '00000000-0000-0000-0000-000000000001'

  const handleLogout = async () => {
    setAnchorEl(null)
    try {
      await authApi.logout()
    } catch {
      // If server-side revocation fails (e.g. token already expired), still clear local state
    }
    logout()
    navigate('/login')
  }

  const displayName = user ? `${user.firstName} ${user.lastName}`.trim() : ''

  return (
    <AppBar
      position="sticky"
      color="inherit"
      elevation={0}
      sx={{ borderBottom: 1, borderColor: 'divider', zIndex: (t) => t.zIndex.drawer - 1 }}
    >
      <Toolbar sx={{ gap: 1, px: { xs: 1, sm: 2 } }}>
        {/* Hamburger — mobile only */}
        {onMenuClick && isMobile && (
          <IconButton edge="start" onClick={onMenuClick} size="small" aria-label="open navigation">
            <MenuOutlined />
          </IconButton>
        )}

        {/* Brand — shown in public/auth layouts */}
        {(showBrand || !onMenuClick) && (
          <Box
            component={Link}
            to="/"
            sx={{ display: 'flex', alignItems: 'center', gap: 1, textDecoration: 'none', color: 'inherit' }}
          >
            <WorkOutlined color="primary" sx={{ fontSize: 28 }} />
            <Typography variant="h6" sx={{ fontWeight: 700 }} color="primary">
              JobPortal
            </Typography>
          </Box>
        )}

        <Box sx={{ flex: 1 }} />

        {/* Active tenant indicator — shown when not on system tenant */}
        {isAuthenticated && !isSystemTenant && canSwitchTenant && (
          <Chip
            icon={<ApartmentOutlined sx={{ fontSize: '16px !important' }} />}
            label={activeTenantName}
            color="warning"
            size="small"
            onClick={() => setSwitchTenantOpen(true)}
            sx={{ fontWeight: 600, cursor: 'pointer' }}
          />
        )}

        {/* Theme toggle */}
        <Tooltip title={isDarkMode ? 'Light mode' : 'Dark mode'}>
          <IconButton onClick={toggle} size="small" aria-label="toggle theme">
            {isDarkMode ? <LightModeOutlined /> : <DarkModeOutlined />}
          </IconButton>
        </Tooltip>

        {isAuthenticated ? (
          <>
            <Tooltip title="Notifications">
              <IconButton size="small" onClick={() => navigate('/notifications')} aria-label="notifications">
                <Badge badgeContent={0} color="error" max={99}>
                  <NotificationsOutlined />
                </Badge>
              </IconButton>
            </Tooltip>

            <Tooltip title={displayName || 'Account'}>
              <IconButton onClick={(e) => setAnchorEl(e.currentTarget)} size="small" aria-label="account menu">
                <Avatar src={user?.avatarUrl} sx={{ width: 32, height: 32, fontSize: 13, bgcolor: 'primary.main' }}>
                  {getInitials(displayName)}
                </Avatar>
              </IconButton>
            </Tooltip>

            <Menu anchorEl={anchorEl} open={!!anchorEl} onClose={() => setAnchorEl(null)}>
              <MenuItem disabled sx={{ flexDirection: 'column', alignItems: 'flex-start' }}>
                <Typography variant="subtitle2">{displayName}</Typography>
                <Typography variant="caption" color="text.secondary">{user?.email}</Typography>
                {!isSystemTenant && (
                  <Chip
                    label={`Tenant: ${activeTenantName}`}
                    size="small"
                    color="warning"
                    sx={{ mt: 0.5, fontSize: 10 }}
                  />
                )}
              </MenuItem>
              <MenuItem onClick={() => { setAnchorEl(null); navigate('/profile') }}>My Profile</MenuItem>
              <MenuItem onClick={() => { setAnchorEl(null); navigate('/settings') }}>Settings</MenuItem>
              {canSwitchTenant && [
                <Divider key="div" />,
                <MenuItem key="switch" onClick={() => { setAnchorEl(null); setSwitchTenantOpen(true) }}>
                  <SwapHorizOutlined sx={{ mr: 1, fontSize: 18 }} />
                  Switch Tenant
                  {!isSystemTenant && (
                    <Chip label="Active" size="small" color="warning" sx={{ ml: 'auto', fontSize: 10 }} />
                  )}
                </MenuItem>,
              ]}
              <Divider />
              <MenuItem onClick={handleLogout} sx={{ color: 'error.main' }}>Sign Out</MenuItem>
            </Menu>

            <SwitchTenantDialog open={switchTenantOpen} onClose={() => setSwitchTenantOpen(false)} />
          </>
        ) : (
          <>
            <Box
              component={Link}
              to="/login"
              sx={{ textDecoration: 'none', color: 'text.primary', fontWeight: 500, fontSize: 14 }}
            >
              Sign In
            </Box>
            <Box
              component={Link}
              to="/register"
              sx={{
                textDecoration: 'none',
                bgcolor: 'primary.main',
                color: 'white',
                px: 2,
                py: 0.8,
                borderRadius: 2,
                fontWeight: 500,
                fontSize: 14,
                '&:hover': { bgcolor: 'primary.dark' },
              }}
            >
              Get Started
            </Box>
          </>
        )}
      </Toolbar>
    </AppBar>
  )
}
