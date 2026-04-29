import { httpClient } from "../httpClient"

export async function logout() {
  await httpClient("/api/users/logout", { method: "POST" })
}