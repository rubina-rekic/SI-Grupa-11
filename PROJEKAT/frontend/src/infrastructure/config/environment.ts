export const environment = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5032",
}

export function getApiBaseUrl(): string {
  return environment.apiBaseUrl
}
