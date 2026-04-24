import { useEffect, useRef, useState } from 'react'

export function useDebounce<T>(value: T, delay = 400): T {
  const [debounced, setDebounced] = useState(value)
  useEffect(() => {
    const timer = setTimeout(() => setDebounced(value), delay)
    return () => clearTimeout(timer)
  }, [value, delay])
  return debounced
}

export function useDebouncedCallback<T extends (...args: unknown[]) => unknown>(fn: T, delay = 400): T {
  const fnRef = useRef(fn)
  fnRef.current = fn
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null)
  return ((...args: unknown[]) => {
    if (timerRef.current) clearTimeout(timerRef.current)
    timerRef.current = setTimeout(() => fnRef.current(...args), delay)
  }) as T
}
