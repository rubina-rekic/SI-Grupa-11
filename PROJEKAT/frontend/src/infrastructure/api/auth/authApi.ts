import { httpClient } from "../httpClient"

export async function logout() {
  await httpClient("/api/auth/logout", { method: "POST" })
}