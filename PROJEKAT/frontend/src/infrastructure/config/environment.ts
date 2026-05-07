export const environment = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? "",
}

export function getApiBaseUrl(): string {
  return environment.apiBaseUrl
}
