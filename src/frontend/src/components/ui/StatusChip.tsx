import { Chip, type ChipProps } from '@mui/material'
import {
  APP_STATUS_LABELS,
  APP_STATUS_COLORS,
  JOB_STATUS_LABELS,
  JOB_STATUS_COLORS,
} from '@/utils/constants'
import type { ApplicationStatus, JobStatus } from '@/types'

type StatusType = 'application' | 'job'

// Generic statuses used across admin/billing/company pages
const GENERIC_STATUS_COLORS: Record<string, ChipProps['color']> = {
  Active: 'success',
  Inactive: 'default',
  Pending: 'warning',
  Suspended: 'error',
  Expired: 'error',
  Cancelled: 'error',
  Completed: 'success',
  Scheduled: 'info',
  Failed: 'error',
  Success: 'success',
  Paid: 'success',
  Unpaid: 'warning',
  Overdue: 'error',
}

interface StatusChipProps extends Omit<ChipProps, 'label' | 'color'> {
  status: ApplicationStatus | JobStatus | string
  type?: StatusType
}

export function StatusChip({ status, type, size = 'small', ...rest }: StatusChipProps) {
  let label: string
  let color: ChipProps['color']

  if (type === 'application') {
    label = APP_STATUS_LABELS[status as ApplicationStatus] ?? status
    color = APP_STATUS_COLORS[status as ApplicationStatus] as ChipProps['color'] ?? 'default'
  } else if (type === 'job') {
    label = JOB_STATUS_LABELS[status as JobStatus] ?? status
    color = JOB_STATUS_COLORS[status as JobStatus] as ChipProps['color'] ?? 'default'
  } else {
    // Auto-detect from both maps, then fall back to generic
    label =
      APP_STATUS_LABELS[status as ApplicationStatus] ??
      JOB_STATUS_LABELS[status as JobStatus] ??
      status
    color =
      (APP_STATUS_COLORS[status as ApplicationStatus] as ChipProps['color']) ??
      (JOB_STATUS_COLORS[status as JobStatus] as ChipProps['color']) ??
      GENERIC_STATUS_COLORS[status] ??
      'default'
  }

  return <Chip label={label} color={color} size={size} {...rest} />
}
