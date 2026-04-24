import {
  Box, Grid, Paper, Typography, Stack, LinearProgress, CircularProgress, Alert
} from '@mui/material'
import { Work as JobIcon, Visibility as ViewIcon } from '@mui/icons-material'
import { useJobCredits } from '@/features/billing/billingApi'
import { PageHeader, SectionCard } from '@/components/ui'

interface CreditGaugeProps {
  title: string
  icon: React.ReactNode
  used: number
  total: number
  unit?: string
}

function CreditGauge({ title, icon, used, total, unit = 'credits' }: CreditGaugeProps) {
  const percent = total > 0 ? (used / total) * 100 : 0
  const remaining = total - used
  const color = percent > 80 ? 'error' : percent > 60 ? 'warning' : 'primary'

  return (
    <Paper sx={{ p: 3 }}>
      <Stack direction="row" spacing={2} sx={{ alignItems: 'center', mb: 2 }}>
        <Box sx={{ color: `${color}.main` }}>{icon}</Box>
        <Typography variant="h6">{title}</Typography>
      </Stack>

      <Stack direction="row" sx={{ justifyContent: 'space-between', mb: 0.5 }}>
        <Typography variant="body2" color="text.secondary">Used</Typography>
        <Typography variant="body2" sx={{ fontWeight: 600 }}>
          {used} / {total} {unit}
        </Typography>
      </Stack>

      <LinearProgress
        variant="determinate"
        value={Math.min(100, percent)}
        color={color as any}
        sx={{ height: 10, borderRadius: 5, mb: 1 }}
      />

      <Stack direction="row" sx={{ justifyContent: 'space-between' }}>
        <Typography variant="caption" color="text.secondary">
          {percent.toFixed(0)}% used
        </Typography>
        <Typography
          variant="caption"
          sx={{ fontWeight: 700, color: remaining <= 5 ? 'error.main' : 'success.main' }}
        >
          {remaining} remaining
        </Typography>
      </Stack>
    </Paper>
  )
}

export default function JobCreditsPage() {
  const { data: creditsArr, isLoading } = useJobCredits()
  const credits = Array.isArray(creditsArr) ? creditsArr[0] : creditsArr

  return (
    <Box>
      <PageHeader title="Job Credits" subtitle="Track your posting and resume view credit usage" />

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 6 }}><CircularProgress /></Box>
      ) : !credits ? (
        <Alert severity="info">No credit information available yet.</Alert>
      ) : (
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, md: 6 }}>
            <CreditGauge
              title="Job Posting Credits"
              icon={<JobIcon fontSize="large" />}
              used={credits.usedCredits ?? 0}
              total={credits.totalCredits ?? 0}
              unit="jobs"
            />
          </Grid>
          <Grid size={{ xs: 12, md: 6 }}>
            <CreditGauge
              title="Resume View Credits"
              icon={<ViewIcon fontSize="large" />}
              used={credits.usedCredits ?? 0}
              total={credits.totalCredits ?? 0}
            />
          </Grid>

          <Grid size={{ xs: 12 }}>
            <SectionCard title="Credit Summary">
              <Alert severity="info">Available credits: {credits.availableCredits}. Used: {credits.usedCredits}. Total allocated: {credits.totalCredits}.</Alert>
            </SectionCard>
          </Grid>
        </Grid>
      )}
    </Box>
  )
}
