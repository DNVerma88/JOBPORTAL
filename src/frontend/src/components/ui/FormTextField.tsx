import { TextField, type TextFieldProps } from '@mui/material'
import { useController, type Control, type FieldValues, type Path } from 'react-hook-form'

type FormTextFieldProps<T extends FieldValues> = Omit<TextFieldProps, 'name' | 'value' | 'onChange' | 'onBlur' | 'error' | 'helperText'> & {
  name: Path<T>
  control: Control<T>
}

export function FormTextField<T extends FieldValues>({ name, control, ...rest }: FormTextFieldProps<T>) {
  const { field, fieldState } = useController({ name, control })
  return (
    <TextField
      {...rest}
      {...field}
      error={!!fieldState.error}
      helperText={fieldState.error?.message}
      fullWidth
    />
  )
}
