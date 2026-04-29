export function setLocalItem(key: string, value: string) {
  localStorage.setItem(key, value)
}

export function getLocalItem(key: string): string | null {
  return localStorage.getItem(key)
}

export function removeLocalItem(key: string) {
  localStorage.removeItem(key)
}

export function clearAuth() {
  localStorage.removeItem("token")
  localStorage.removeItem("role")
  localStorage.removeItem("mustChangePassword")
}