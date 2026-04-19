import type { ApiRequestOptions, ApiResponse } from "../../shared/types/api"

export async function httpClient<T>(path: string, options: ApiRequestOptions = {}): Promise<ApiResponse<T>> {
  const method = options.method ?? "GET"

  return {
    data: null,
    status: 501,
    error: `HTTP client placeholder: ${method} ${path}`,
  }
}
