import { useMemo } from "react"
import { createAppBootstrapStatus } from "../models/appShell"

export function useBootstrapStatus() {
  return useMemo(() => createAppBootstrapStatus(), [])
}
