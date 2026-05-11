export interface ApiRequestOptions {
  method?: "GET" | "POST" | "PUT" | "PATCH" | "DELETE"
  body?: unknown
  headers?: Record<string, string>
}
// Ova funkcija je namijenjena za slanje API zahtjeva i obradu odgovora
export interface ApiResponse<T> {
  data: T | null
  status: number
  error?: string
}
