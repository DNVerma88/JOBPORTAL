import { InputAdornment, TextField, IconButton, type TextFieldProps } from '@mui/material'
import { SearchOutlined, CloseOutlined } from '@mui/icons-material'

interface SearchInputProps extends Omit<TextFieldProps, 'onChange'> {
  value: string
  onChange: (value: string) => void
  placeholder?: string
}

export function SearchInput({ value, onChange, placeholder = 'Search…', ...rest }: SearchInputProps) {
  return (
    <TextField
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder={placeholder}
      size="small"
      slotProps={{
        input: {
          startAdornment: (
            <InputAdornment position="start">
              <SearchOutlined fontSize="small" sx={{ color: 'text.disabled' }} />
            </InputAdornment>
          ),
          endAdornment: value ? (
            <InputAdornment position="end">
              <IconButton size="small" onClick={() => onChange('')} edge="end" aria-label="clear search">
                <CloseOutlined fontSize="small" />
              </IconButton>
            </InputAdornment>
          ) : null,
        },
      }}
      {...rest}
    />
  )
}
