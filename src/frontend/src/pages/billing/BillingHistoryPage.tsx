import { useState } from 'react'
import {
  Box, Tab, Tabs, Chip, Button, CircularProgress, Alert,
  Paper, TableContainer, Table, TableHead, TableRow, TableCell, TableBody
} from '@mui/material'
import { Download as DownloadIcon } from '@mui/icons-material'
import { useInvoices, usePaymentTransactions } from '@/features/billing/billingApi'
import { PageHeader, StatusChip } from '@/components/ui'

function InvoicesTab() {
  const { data: invoicesResult, isLoading } = useInvoices()
  const invoices = invoicesResult?.items ?? []

  return (
    <Box>
      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 6 }}><CircularProgress /></Box>
      ) : invoices.length === 0 ? (
        <Alert severity="info">No invoices yet.</Alert>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Invoice #</TableCell>
                <TableCell>Plan</TableCell>
                <TableCell>Period</TableCell>
                <TableCell>Amount</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Due Date</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {invoices.map((inv) => (
                <TableRow key={inv.id} hover>
                  <TableCell sx={{ fontFamily: 'monospace' }}>{inv.invoiceNumber ?? inv.id.slice(0, 8).toUpperCase()}</TableCell>
                  <TableCell>{'—'}</TableCell>
                  <TableCell>
                    {inv.billingPeriodStart && inv.billingPeriodEnd
                      ? `${new Date(inv.billingPeriodStart).toLocaleDateString()} – ${new Date(inv.billingPeriodEnd).toLocaleDateString()}`
                      : '—'}
                  </TableCell>
                  <TableCell sx={{ fontWeight: 600 }}>${(inv.totalAmount ?? inv.amount)?.toFixed(2) ?? '0.00'}</TableCell>
                  <TableCell><StatusChip status={inv.status} /></TableCell>
                  <TableCell>{inv.dueDate ? new Date(inv.dueDate).toLocaleDateString() : '—'}</TableCell>
                  <TableCell>
                    {inv.invoiceFileUrl && (
                      <Button
                        size="small"
                        startIcon={<DownloadIcon />}
                        href={inv.invoiceFileUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                      >
                        PDF
                      </Button>
                    )}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  )
}

function TransactionsTab() {
  const { data: transactions = [], isLoading } = usePaymentTransactions()

  return (
    <Box>
      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 6 }}><CircularProgress /></Box>
      ) : (transactions as any[]).length === 0 ? (
        <Alert severity="info">No payment transactions yet.</Alert>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Date</TableCell>
                <TableCell>Reference</TableCell>
                <TableCell>Gateway</TableCell>
                <TableCell>Amount</TableCell>
                <TableCell>Currency</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {(transactions as any[]).map((tx) => (
                <TableRow key={tx.id} hover>
                  <TableCell>{new Date(tx.createdOn).toLocaleString()}</TableCell>
                  <TableCell sx={{ fontFamily: 'monospace', fontSize: 12 }}>{tx.gatewayReference ?? tx.id.slice(0, 12)}</TableCell>
                  <TableCell>
                    <Chip label={tx.gateway ?? 'N/A'} size="small" variant="outlined" />
                  </TableCell>
                  <TableCell sx={{ fontWeight: 600 }}>${tx.amount?.toFixed(2) ?? '0.00'}</TableCell>
                  <TableCell>{tx.currency ?? 'USD'}</TableCell>
                  <TableCell>
                    <Chip
                      label={tx.status}
                      size="small"
                      color={tx.status === 'Success' ? 'success' : tx.status === 'Failed' ? 'error' : 'default'}
                    />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  )
}

export default function BillingHistoryPage() {
  const [tab, setTab] = useState(0)

  return (
    <Box>
      <PageHeader title="Billing History" subtitle="Your invoices and payment transactions" />
      <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 3 }}>
        <Tab label="Invoices" />
        <Tab label="Transactions" />
      </Tabs>
      {tab === 0 && <InvoicesTab />}
      {tab === 1 && <TransactionsTab />}
    </Box>
  )
}
