import { Box, List, ListItem, ListItemText, ListItemIcon, Typography, IconButton, Tooltip, Divider } from '@mui/material'
import {
  NotificationsOutlined,
  DoneAllOutlined,
  CircleOutlined,
} from '@mui/icons-material'
import { useNotifications, useMarkAsRead, useMarkAllAsRead } from '@/features/notifications/notificationsApi'
import { PageHeader, SectionCard, EmptyState, LoadingSpinner, Button } from '@/components/ui'
import { formatRelativeTime } from '@/utils/formatters'

export function NotificationsPage() {
  const { data, isLoading } = useNotifications()
  const markRead = useMarkAsRead()
  const markAll = useMarkAllAsRead()

  const unread = data?.items.filter((n: import('@/types').Notification) => !n.isRead).length ?? 0

  return (
    <Box>
      <PageHeader
        title="Notifications"
        subtitle={unread > 0 ? `${unread} unread notification${unread !== 1 ? 's' : ''}` : 'All caught up!'}
        actions={
          unread > 0 ? (
            <Button
              variant="outlined"
              size="small"
              startIcon={<DoneAllOutlined />}
              onClick={() => markAll.mutate()}
              disabled={markAll.isPending}
            >
              Mark all read
            </Button>
          ) : undefined
        }
      />

      {isLoading ? (
        <LoadingSpinner />
      ) : !data?.items.length ? (
        <EmptyState
          icon={<NotificationsOutlined />}
          title="No notifications"
          description="You're all caught up! Notifications will appear here."
        />
      ) : (
        <SectionCard noPadding>
          <List disablePadding>
            {data.items.map((notif: import('@/types').Notification, i: number) => (
              <Box key={notif.id}>
                <ListItem
                  secondaryAction={
                    !notif.isRead && (
                      <Tooltip title="Mark as read">
                        <IconButton
                          size="small"
                          onClick={() => markRead.mutate(notif.id)}
                          aria-label="mark as read"
                        >
                          <CircleOutlined fontSize="small" sx={{ color: 'primary.main' }} />
                        </IconButton>
                      </Tooltip>
                    )
                  }
                  sx={{ bgcolor: notif.isRead ? 'transparent' : 'primary.50', py: 1.5, px: 2 }}
                >
                  <ListItemIcon>
                    <NotificationsOutlined sx={{ color: notif.isRead ? 'text.disabled' : 'primary.main' }} />
                  </ListItemIcon>
                  <ListItemText
                    primary={
                      <Typography variant="body2" sx={{ fontWeight: notif.isRead ? 400 : 600 }}>
                        {notif.title}
                      </Typography>
                    }
                    secondary={
                      <>
                        <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>
                          {notif.body}
                        </Typography>
                        <Typography variant="caption" color="text.disabled">
                          {formatRelativeTime(notif.createdAt)}
                        </Typography>
                      </>
                    }
                  />
                </ListItem>
                {i < data.items.length - 1 && <Divider />}
              </Box>
            ))}
          </List>
        </SectionCard>
      )}
    </Box>
  )
}
