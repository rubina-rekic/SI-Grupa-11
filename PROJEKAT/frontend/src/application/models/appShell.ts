export interface AppBootstrapStatus {
  message: string
  initializedAtUtc: string
}

export function createAppBootstrapStatus(): AppBootstrapStatus {
  return {
    message: "Initial PostRoute frontend skeleton is ready.",
    initializedAtUtc: new Date().toISOString(),
  }
}
