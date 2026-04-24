import {
  Box, Grid, Paper, Typography, Stack, Button, Chip, LinearProgress,
  Divider, List, ListItem, ListItemText, Alert, CircularProgress
} from '@mui/material'
import { CheckCircle, Star } from '@mui/icons-material'
import {
  useCurrentSubscription, useSubscriptionPlans, useChangePlan, useCancelSubscription
} from '@/features/billing/billingApi'
import { PageHeader, SectionCard, StatusChip } from '@/components/ui'
import { useState } from 'react'

export default function SubscriptionPage() {
  const { data: sub, isLoading: subLoading } = useCurrentSubscription()
  const { data: plans = [], isLoading: plansLoading } = useSubscriptionPlans()
  const changePlan = useChangePlan()
  const cancelSub = useCancelSubscription()
  const [changing, setChanging] = useState<string | null>(null)

  const handleChange = (planId: string) => {
    setChanging(planId)
    changePlan.mutate({ planId, billingCycle: 'Monthly' }, { onSettled: () => setChanging(null) })
  }

  const handleCancel = () => {
    if (window.confirm('Are you sure you want to cancel your subscription? Your access will continue until the end of the billing period.')) {
      cancelSub.mutate()
    }
  }

  const daysRemaining = sub?.endDate
    ? Math.max(0, Math.ceil((new Date(sub.endDate).getTime() - Date.now()) / 86400000))
    : null

  return (
    <Box>
      <PageHeader title="Subscription" subtitle="Manage your plan and billing details" />

      <Grid container spacing={3}>
        {/* Current Plan Card */}
        <Grid size={{ xs: 12, md: 5 }}>
          <SectionCard title="Current Plan">
            {subLoading ? (
              <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}><CircularProgress size={28} /></Box>
            ) : !sub ? (
              <Alert severity="info">No active subscription. Choose a plan below.</Alert>
            ) : (
              <Stack spacing={2}>
                <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
                  <Box>
                    <Typography variant="h5" sx={{ fontWeight: 700 }}>{sub.planName}</Typography>
                    <Typography variant="body2" color="text.secondary">{sub.billingCycle} billing</Typography>
                  </Box>
                  <StatusChip status={sub.status} />
                </Stack>

                {daysRemaining !== null && (
                  <Box>
                    <Stack direction="row" sx={{ justifyContent: 'space-between', mb: 0.5 }}>
                      <Typography variant="body2">Days remaining</Typography>
                      <Typography variant="body2" sx={{ fontWeight: 600 }}>{daysRemaining} days</Typography>
                    </Stack>
                    <LinearProgress
                      variant="determinate"
                      value={Math.min(100, (daysRemaining / (sub.billingCycle === 'Monthly' ? 30 : 365)) * 100)}
                      sx={{ height: 8, borderRadius: 4 }}
                    />
                  </Box>
                )}

                <Divider />
                <Stack spacing={0.5}>
                  {sub.startDate && (
                    <Stack direction="row" sx={{ justifyContent: 'space-between' }}>
                      <Typography variant="body2" color="text.secondary">Started</Typography>
                      <Typography variant="body2">{new Date(sub.startDate).toLocaleDateString()}</Typography>
                    </Stack>
                  )}
                  {sub.endDate && (
                    <Stack direction="row" sx={{ justifyContent: 'space-between' }}>
                      <Typography variant="body2" color="text.secondary">Renews</Typography>
                      <Typography variant="body2">{new Date(sub.endDate).toLocaleDateString()}</Typography>
                    </Stack>
                  )}
                  {sub.amount != null && (
                    <Stack direction="row" sx={{ justifyContent: 'space-between' }}>
                      <Typography variant="body2" color="text.secondary">Amount</Typography>
                      <Typography variant="body2" sx={{ fontWeight: 600 }}>${sub.amount}/{sub.billingCycle === 'Monthly' ? 'mo' : 'yr'}</Typography>
                    </Stack>
                  )}
                </Stack>

                {sub.status === 'Active' && (
                  <Button color="error" variant="outlined" onClick={handleCancel} disabled={cancelSub.isPending}>
                    {cancelSub.isPending ? 'Cancelling…' : 'Cancel Subscription'}
                  </Button>
                )}
              </Stack>
            )}
          </SectionCard>
        </Grid>

        {/* Plan Options */}
        <Grid size={{ xs: 12, md: 7 }}>
          <SectionCard title="Available Plans">
            {plansLoading ? (
              <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress size={28} /></Box>
            ) : (
              <Stack spacing={2}>
                {(plans as any[]).map((plan) => {
                  const isCurrentPlan = sub?.planId === plan.id
                  return (
                    <Paper
                      key={plan.id}
                      variant="outlined"
                      sx={{
                        p: 2,
                        border: isCurrentPlan ? '2px solid' : undefined,
                        borderColor: isCurrentPlan ? 'primary.main' : undefined,
                      }}
                    >
                      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start' }}>
                        <Box sx={{ flex: 1 }}>
                          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                            <Typography variant="h6">{plan.name}</Typography>
                            {isCurrentPlan && <Chip label="Current" size="small" color="primary" />}
                            {plan.isPopular && <Chip label="Popular" size="small" color="secondary" icon={<Star />} />}
                          </Stack>
                          <Typography variant="h5" color="primary" sx={{ fontWeight: 700 }}>
                            ${plan.price}
                            <Typography component="span" variant="caption" color="text.secondary">
                              /{plan.billingCycle === 'Monthly' ? 'mo' : 'yr'}
                            </Typography>
                          </Typography>
                          {plan.features && plan.features.length > 0 && (
                            <List dense disablePadding>
                              {plan.features.slice(0, 3).map((f: any, i: number) => (
                                <ListItem key={i} disablePadding sx={{ py: 0.25 }}>
                                  <CheckCircle sx={{ fontSize: 14, mr: 1, color: 'success.main' }} />
                                  <ListItemText primary={<Typography variant="caption">{f.description ?? f.name}</Typography>} />
                                </ListItem>
                              ))}
                            </List>
                          )}
                        </Box>
                        {!isCurrentPlan && (
                          <Button
                            variant="contained"
                            size="small"
                            onClick={() => handleChange(plan.id)}
                            disabled={changePlan.isPending && changing === plan.id}
                            sx={{ ml: 2, whiteSpace: 'nowrap' }}
                          >
                            {changing === plan.id ? 'Switching…' : 'Switch Plan'}
                          </Button>
                        )}
                      </Stack>
                    </Paper>
                  )
                })}
                {(plans as any[]).length === 0 && (
                  <Alert severity="info">No plans available yet.</Alert>
                )}
              </Stack>
            )}
          </SectionCard>
        </Grid>
      </Grid>
    </Box>
  )
}
