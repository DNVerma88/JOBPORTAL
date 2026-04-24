import {
  FormControl,
  FormHelperText,
  InputLabel,
  MenuItem,
  Select,
  type SelectProps,
} from '@mui/material'
import { useController, type Control, type FieldValues, type Path } from 'react-hook-form'

type Option = { label: string; value: string | number }

type FormSelectProps<T extends FieldValues> = Omit<SelectProps, 'name' | 'value' | 'onChange'> & {
  name: Path<T>
  control: Control<T>
  label: string
  options: Option[]
}

export function FormSelect<T extends FieldValues>({ name, control, label, options, ...rest }: FormSelectProps<T>) {
  const { field, fieldState } = useController({ name, control })
  const labelId = `${name}-label`

  return (
    <FormControl fullWidth error={!!fieldState.error} size="small">
      <InputLabel id={labelId}>{label}</InputLabel>
      <Select labelId={labelId} label={label} {...field} {...rest}>
        {options.map((o) => (
          <MenuItem key={o.value} value={o.value}>
            {o.label}
          </MenuItem>
        ))}
      </Select>
      {fieldState.error && <FormHelperText>{fieldState.error.message}</FormHelperText>}
    </FormControl>
  )
}
