import {
  Box,
  Drawer,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Tooltip,
  Typography,
  Divider,
  useTheme,
  useMediaQuery,
  IconButton,
} from '@mui/material'
import {
  DashboardOutlined,
  WorkOutlined,
  PeopleOutlined,
  AssignmentOutlined,
  BusinessOutlined,
  PersonOutlined,
  BookmarkOutlined,
  SettingsOutlined,
  PostAddOutlined,
  NotificationsOutlined,
  ChevronLeftOutlined,
  ChevronRightOutlined,
  ManageAccountsOutlined,
  ViewKanbanOutlined,
  EventOutlined,
  DescriptionOutlined,
  NotificationsActiveOutlined,
  CreditCardOutlined,
  ReceiptOutlined,
  MonetizationOnOutlined,
  ApartmentOutlined,
  SecurityOutlined,
  HistoryOutlined,
  TuneOutlined,
  CategoryOutlined,
} from '@mui/icons-material'
import { useState } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { useAuthStore } from '@/features/auth/store/authStore'
import { ROLES } from '@/utils/constants'

const DRAWER_WIDTH = 240
const MINI_WIDTH = 64

const ALL_ROLES = [ROLES.SUPER_ADMIN, ROLES.TENANT_ADMIN, ROLES.RECRUITER, ROLES.HIRING_MANAGER, ROLES.JOB_SEEKER]
const ADMIN_ROLES = [ROLES.SUPER_ADMIN, ROLES.TENANT_ADMIN]
const RECRUITER_ROLES = [ROLES.RECRUITER, ROLES.HIRING_MANAGER, ROLES.TENANT_ADMIN, ROLES.SUPER_ADMIN]
const JOBSEEKER_ROLES = [ROLES.JOB_SEEKER]
const SUPERADMIN_ONLY = [ROLES.SUPER_ADMIN]

interface NavItem {
  label: string
  path: string
  icon: React.ReactNode
  roles: string[]
}

interface NavSection {
  heading: string
  roles: string[]
  items: NavItem[]
}

const NAV_SECTIONS: NavSection[] = [
  {
    heading: 'Main',
    roles: ALL_ROLES,
    items: [
      { label: 'Dashboard', path: '/', icon: <DashboardOutlined />, roles: ALL_ROLES },
    ],
  },
  {
    heading: 'Jobs',
    roles: JOBSEEKER_ROLES,
    items: [
      { label: 'Find Jobs', path: '/jobs', icon: <WorkOutlined />, roles: JOBSEEKER_ROLES },
      { label: 'My Applications', path: '/my-applications', icon: <AssignmentOutlined />, roles: JOBSEEKER_ROLES },
      { label: 'Saved Jobs', path: '/saved-jobs', icon: <BookmarkOutlined />, roles: JOBSEEKER_ROLES },
      { label: 'Job Alerts', path: '/job-alerts', icon: <NotificationsActiveOutlined />, roles: JOBSEEKER_ROLES },
      { label: 'My Profile', path: '/profile', icon: <PersonOutlined />, roles: JOBSEEKER_ROLES },
    ],
  },
  {
    heading: 'Recruitment',
    roles: RECRUITER_ROLES,
    items: [
      { label: 'Post a Job', path: '/post-job', icon: <PostAddOutlined />, roles: RECRUITER_ROLES },
      { label: 'Manage Jobs', path: '/manage-jobs', icon: <WorkOutlined />, roles: RECRUITER_ROLES },
      { label: 'Applications', path: '/applications', icon: <AssignmentOutlined />, roles: RECRUITER_ROLES },
      { label: 'Hiring Pipeline', path: '/pipeline', icon: <ViewKanbanOutlined />, roles: RECRUITER_ROLES },
      { label: 'Interviews', path: '/interviews', icon: <EventOutlined />, roles: RECRUITER_ROLES },
      { label: 'Offer Letters', path: '/offers', icon: <DescriptionOutlined />, roles: RECRUITER_ROLES },
    ],
  },
  {
    heading: 'Company',
    roles: RECRUITER_ROLES,
    items: [
      { label: 'Company Profile', path: '/company', icon: <BusinessOutlined />, roles: RECRUITER_ROLES },
      { label: 'Browse Companies', path: '/companies', icon: <ApartmentOutlined />, roles: ALL_ROLES },
      { label: 'Candidates', path: '/candidates', icon: <PeopleOutlined />, roles: RECRUITER_ROLES },
    ],
  },
  {
    heading: 'Subscription',
    roles: ADMIN_ROLES,
    items: [
      { label: 'Current Plan', path: '/billing/subscription', icon: <CreditCardOutlined />, roles: ADMIN_ROLES },
      { label: 'Billing History', path: '/billing/history', icon: <ReceiptOutlined />, roles: ADMIN_ROLES },
      { label: 'Job Credits', path: '/billing/credits', icon: <MonetizationOnOutlined />, roles: ADMIN_ROLES },
    ],
  },
  {
    heading: 'Administration',
    roles: ADMIN_ROLES,
    items: [
      { label: 'Users', path: '/admin/users', icon: <ManageAccountsOutlined />, roles: ADMIN_ROLES },
      { label: 'Tenants', path: '/admin/tenants', icon: <ApartmentOutlined />, roles: SUPERADMIN_ONLY },
      { label: 'Roles & Permissions', path: '/admin/roles', icon: <SecurityOutlined />, roles: ADMIN_ROLES },
      { label: 'Audit Logs', path: '/admin/audit-logs', icon: <HistoryOutlined />, roles: ADMIN_ROLES },
      { label: 'System Settings', path: '/admin/settings', icon: <TuneOutlined />, roles: ADMIN_ROLES },
      { label: 'Master Data', path: '/admin/master-data', icon: <CategoryOutlined />, roles: ADMIN_ROLES },
    ],
  },
  {
    heading: 'General',
    roles: ALL_ROLES,
    items: [
      { label: 'Notifications', path: '/notifications', icon: <NotificationsOutlined />, roles: ALL_ROLES },
      { label: 'Settings', path: '/settings', icon: <SettingsOutlined />, roles: ALL_ROLES },
    ],
  },
]

interface SidebarProps {
  mobileOpen: boolean
  onClose: () => void
}

export function Sidebar({ mobileOpen, onClose }: SidebarProps) {
  const [collapsed, setCollapsed] = useState(false)
  const user = useAuthStore(s => s.user)
  const role = user?.role ?? ''
  const location = useLocation()
  const navigate = useNavigate()
  const theme = useTheme()
  const isMobile = useMediaQuery(theme.breakpoints.down('md'))

  const visibleSections = NAV_SECTIONS.map(section => ({
    ...section,
    items: section.items.filter(item => item.roles.includes(role)),
  })).filter(section => section.items.length > 0 && section.roles.includes(role))

  const drawerWidth = collapsed && !isMobile ? MINI_WIDTH : DRAWER_WIDTH

  const handleNav = (path: string) => {
    navigate(path)
    if (isMobile) onClose()
  }

  const isActive = (path: string) =>
    path === '/' ? location.pathname === '/' : location.pathname.startsWith(path)

  const contents = (
    <Box sx={{ display: 'flex', flexDirection: 'column', height: '100%', overflow: 'hidden' }}>
      {/* Header */}
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          px: 2,
          py: 1.5,
          minHeight: 64,
          gap: 1,
          justifyContent: collapsed && !isMobile ? 'center' : 'space-between',
        }}
      >
        {(!collapsed || isMobile) && (
          <Typography variant="h6" sx={{ fontWeight: 700 }} color="primary" noWrap>
            JobPortal
          </Typography>
        )}
        {!isMobile && (
          <IconButton size="small" onClick={() => setCollapsed((c) => !c)}>
            {collapsed ? <ChevronRightOutlined /> : <ChevronLeftOutlined />}
          </IconButton>
        )}
      </Box>

      <Divider />

      {/* Nav list */}
      <List sx={{ flex: 1, overflowY: 'auto', py: 1, px: collapsed && !isMobile ? 0.5 : 1 }}>
        {visibleSections.map((section, sIdx) => (
          <Box key={section.heading}>
            {sIdx > 0 && <Divider sx={{ my: 0.75 }} />}
            {(!collapsed || isMobile) && (
              <Typography
                variant="caption"
                sx={{ px: 1.5, py: 0.5, display: 'block', fontWeight: 700, color: 'text.disabled', textTransform: 'uppercase', letterSpacing: '0.08em' }}
              >
                {section.heading}
              </Typography>
            )}
            {section.items.map((item) => {
              const active = isActive(item.path)
              const btn = (
                <ListItemButton
                  key={item.path + item.label}
                  selected={active}
                  onClick={() => handleNav(item.path)}
                  sx={{
                    borderRadius: 1.5,
                    mb: 0.25,
                    justifyContent: collapsed && !isMobile ? 'center' : 'flex-start',
                    minHeight: 40,
                    px: collapsed && !isMobile ? 1 : 1.5,
                    '&.Mui-selected': {
                      bgcolor: 'primary.main',
                      color: 'primary.contrastText',
                      '& .MuiListItemIcon-root': { color: 'primary.contrastText' },
                      '&:hover': { bgcolor: 'primary.dark' },
                    },
                  }}
                >
                  <ListItemIcon
                    sx={{
                      minWidth: collapsed && !isMobile ? 0 : 36,
                      color: active ? 'inherit' : 'text.secondary',
                    }}
                  >
                    {item.icon}
                  </ListItemIcon>
                  {(!collapsed || isMobile) && (
                    <ListItemText
                      primary={
                        <Typography variant="body2" sx={{ fontWeight: active ? 600 : 400 }}>
                          {item.label}
                        </Typography>
                      }
                    />
                  )}
                </ListItemButton>
              )

              return collapsed && !isMobile ? (
                <Tooltip title={item.label} placement="right" key={item.path + item.label}>
                  <span>{btn}</span>
                </Tooltip>
              ) : btn
            })}
          </Box>
        ))}
      </List>
    </Box>
  )

  return (
    <Box component="nav" sx={{ width: { md: drawerWidth }, flexShrink: { md: 0 } }}>
      {/* Mobile drawer */}
      <Drawer
        variant="temporary"
        open={mobileOpen}
        onClose={onClose}
        ModalProps={{ keepMounted: true }}
        sx={{
          display: { xs: 'block', md: 'none' },
          '& .MuiDrawer-paper': { width: DRAWER_WIDTH, boxSizing: 'border-box' },
        }}
      >
        {contents}
      </Drawer>

      {/* Desktop drawer */}
      <Drawer
        variant="permanent"
        sx={{
          display: { xs: 'none', md: 'flex' },
          '& .MuiDrawer-paper': {
            width: drawerWidth,
            boxSizing: 'border-box',
            overflowX: 'hidden',
            transition: theme.transitions.create('width', {
              easing: theme.transitions.easing.sharp,
              duration: theme.transitions.duration.enteringScreen,
            }),
          },
        }}
        open
      >
        {contents}
      </Drawer>
    </Box>
  )
}
