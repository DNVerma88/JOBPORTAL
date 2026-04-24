import { Checkbox, FormControlLabel, FormHelperText, type CheckboxProps } from '@mui/material'
import { useController, type Control, type FieldValues, type Path } from 'react-hook-form'

type FormCheckboxProps<T extends FieldValues> = Omit<CheckboxProps, 'name' | 'checked' | 'onChange'> & {
  name: Path<T>
  control: Control<T>
  label: string
}

export function FormCheckbox<T extends FieldValues>({ name, control, label, ...rest }: FormCheckboxProps<T>) {
  const { field, fieldState } = useController({ name, control })
  return (
    <>
      <FormControlLabel
        label={label}
        control={<Checkbox {...rest} {...field} checked={!!field.value} />}
      />
      {fieldState.error && <FormHelperText error>{fieldState.error.message}</FormHelperText>}
    </>
  )
}
