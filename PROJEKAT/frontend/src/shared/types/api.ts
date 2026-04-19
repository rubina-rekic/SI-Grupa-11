export interface ApiRequestOptions {
  method?: "GET" | "POST" | "PUT" | "PATCH" | "DELETE"
  body?: unknown
  headers?: Record<string, string>
}

export interface ApiResponse<T> {
  data: T | null
  status: number
  error?: string
}
