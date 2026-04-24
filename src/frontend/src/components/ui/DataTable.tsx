import { Box } from '@mui/material'
import { DataGrid, type DataGridProps, type GridColDef } from '@mui/x-data-grid'
import { EmptyState } from './EmptyState'
import { LoadingSpinner } from './LoadingSpinner'

export type { GridColDef }

interface DataTableProps<R extends object> extends Omit<DataGridProps, 'rows'> {
  rows: R[]
  loading?: boolean
  emptyTitle?: string
  emptyDescription?: string
}

export function DataTable<R extends { id: string | number }>({
  rows,
  loading,
  emptyTitle = 'No data found',
  emptyDescription,
  ...rest
}: DataTableProps<R>) {
  if (loading) return <LoadingSpinner />

  if (!rows.length) {
    return <EmptyState title={emptyTitle} description={emptyDescription} />
  }

  return (
    <Box sx={{ width: '100%' }}>
      <DataGrid
        rows={rows}
        autoHeight
        disableRowSelectionOnClick
        pageSizeOptions={[10, 25, 50]}
        initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
        sx={{
          border: 'none',
          '& .MuiDataGrid-columnHeader': { bgcolor: 'background.default', fontWeight: 600 },
          '& .MuiDataGrid-cell': { alignItems: 'center' },
        }}
        {...rest}
      />
    </Box>
  )
}
