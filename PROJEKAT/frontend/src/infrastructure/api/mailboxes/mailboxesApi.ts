import { httpClient } from "../httpClient"
import type { ApiResponse } from "../../../shared/types/api"

export interface CreateBoxDto {
	address: string
	latitude: number
	longitude: number
	type: string
	serialNumber: string
	capacity: number
	yearOfInstallation: number
}

export interface BoxDto extends CreateBoxDto {
	id: string
}

export function createBox(data: CreateBoxDto): Promise<ApiResponse<BoxDto>> {
	return httpClient<BoxDto>("/api/boxes", { method: "POST", body: data })
}

export function getBoxes(): Promise<ApiResponse<BoxDto[]>> {
	return httpClient<BoxDto[]>("/api/boxes", { method: "GET" })
}

export function getBoxById(id: string): Promise<ApiResponse<BoxDto>> {
	return httpClient<BoxDto>(`/api/boxes/${id}`, { method: "GET" })
}
