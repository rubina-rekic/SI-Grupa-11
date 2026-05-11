import { httpClient } from "../httpClient"

export async function logout() {
  //random comment to trigger frontend check and netlify deployment
  await httpClient("/api/users/logout", { method: "POST" })
}