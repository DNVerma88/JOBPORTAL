import { useState } from 'react'
import { Alert, Box, Collapse } from '@mui/material'
import type { AlertColor } from '@mui/material'
import { useAnnouncements } from '@/features/config/configApi'
import type { Announcement } from '@/features/config/configApi'

function isActive(a: Announcement): boolean {
  if (!a.isActive) return false
  const now = new Date()
  if (a.startsOn && new Date(a.startsOn) > now) return false
  if (a.endsOn && new Date(a.endsOn) < now) return false
  return true
}

function severityForType(type: Announcement['type']): AlertColor {
  switch (type) {
    case 'Critical': return 'error'
    case 'Warning': return 'warning'
    default: return 'info'
  }
}

export function AnnouncementBanner() {
  const { data } = useAnnouncements()
  const [dismissed, setDismissed] = useState<string[]>([])

  const items: Announcement[] = Array.isArray(data)
    ? data
    : ((data as unknown as { items?: Announcement[] })?.items ?? [])

  const visible = items.filter((a) => isActive(a) && !dismissed.includes(a.id))

  if (visible.length === 0) return null

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0 }}>
      {visible.map((a) => (
        <Collapse key={a.id} in>
          <Alert
            severity={severityForType(a.type)}
            onClose={() => setDismissed((prev) => [...prev, a.id])}
            sx={{ borderRadius: 0, borderBottom: '1px solid', borderColor: 'divider' }}
          >
            <strong>{a.title}</strong>
            {a.content && ` — ${a.content}`}
          </Alert>
        </Collapse>
      ))}
    </Box>
  )
}
