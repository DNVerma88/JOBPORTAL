import {
  Card,
  CardContent,
  Box,
  Typography,
  Avatar,
  Divider,
  IconButton,
  Tooltip,
} from '@mui/material'
import { CancelOutlined, OpenInNewOutlined } from '@mui/icons-material'
import { useNavigate } from 'react-router-dom'
import type { ApplicationSummary } from '@/types'
import { formatDate, formatRelativeTime } from '@/utils/formatters'
import { StatusChip } from '@/components/ui'

interface ApplicationCardProps {
  application: ApplicationSummary
  onWithdraw?: (id: string) => void
}

export function ApplicationCard({ application, onWithdraw }: ApplicationCardProps) {
  const navigate = useNavigate()

  return (
    <Card>
      <CardContent sx={{ p: 2.5 }}>
        <Box sx={{ display: 'flex', gap: 1.5, alignItems: 'flex-start' }}>
          <Avatar variant="rounded" sx={{ width: 44, height: 44, bgcolor: 'primary.50', color: 'primary.main', fontWeight: 700 }}>
            {application.companyName[0]}
          </Avatar>
          <Box sx={{ flex: 1, minWidth: 0 }}>
            <Typography variant="subtitle1" sx={{ fontWeight: 600 }} noWrap>
              {application.jobTitle}
            </Typography>
            <Typography variant="body2" color="text.secondary" noWrap>
              {application.companyName}
            </Typography>
          </Box>
          <Box sx={{ display: 'flex', gap: 0.5, flexShrink: 0, alignItems: 'center' }}>
            <StatusChip status={application.status} type="application" />
            <Tooltip title="View job">
              <IconButton size="small" onClick={() => navigate(`/jobs/${application.id}`)}>
                <OpenInNewOutlined fontSize="small" />
              </IconButton>
            </Tooltip>
            {application.status !== 'Withdrawn' && onWithdraw && (
              <Tooltip title="Withdraw application">
                <IconButton
                  size="small"
                  onClick={() => onWithdraw(application.id)}
                  sx={{ color: 'error.main' }}
                  aria-label="withdraw application"
                >
                  <CancelOutlined fontSize="small" />
                </IconButton>
              </Tooltip>
            )}
          </Box>
        </Box>
        <Divider sx={{ my: 1.5 }} />
        <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
          <Typography variant="caption" color="text.secondary">
            Applied {formatDate(application.appliedAt)}
          </Typography>
          <Typography variant="caption" color="text.secondary">
            Updated {formatRelativeTime(application.lastUpdatedAt)}
          </Typography>
        </Box>
      </CardContent>
    </Card>
  )
}
