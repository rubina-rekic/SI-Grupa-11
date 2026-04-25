import axios, { AxiosError } from "axios"
import { getApiBaseUrl } from "../config/environment"
import type { ApiRequestOptions, ApiResponse } from "../../shared/types/api"

const axiosInstance = axios.create({
  baseURL: getApiBaseUrl(),
  headers: { "Content-Type": "application/json" },
})

axiosInstance.interceptors.request.use((config) => {
  const token = localStorage.getItem("token")
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export async function httpClient<T>(
  path: string,
  options: ApiRequestOptions = {},
): Promise<ApiResponse<T>> {
  try {
    const response = await axiosInstance.request<T>({
      url: path,
      method: options.method ?? "GET",
      data: options.body,
      headers: options.headers,
    })

    return { data: response.data, status: response.status }
  } catch (err) {
    const error = err as AxiosError<{ message?: string }>
    const status = error.response?.status ?? 0
    const message =
      error.response?.data?.message ?? error.message ?? "Nepoznata greška"

    return { data: null, status, error: message }
  }
}
