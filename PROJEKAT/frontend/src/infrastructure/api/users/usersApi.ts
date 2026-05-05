import { httpClient } from "../httpClient"
import type { ApiResponse } from "../../../shared/types/api"

export interface CreateUserDto {
  firstName: string
  lastName: string
  username: string
  email: string
  password: string
}

export interface UserDto {
  id: string
  username: string
  email: string
  role: string
}

export function createUser(data: CreateUserDto): Promise<ApiResponse<UserDto>> {
  return httpClient<UserDto>("/api/users", { method: "POST", body: data })
}

export interface UserListDto {
  id: string
  username: string
  email: string
  role: string
  mustChangePassword: boolean
  isLockedOut: boolean
}

export function getUsers(): Promise<ApiResponse<UserListDto[]>> {
  return httpClient<UserListDto[]>("/api/users", { method: "GET" })
}