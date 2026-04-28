import { useNavigate } from "react-router-dom"
import { toast } from "sonner"
import { logout } from "../../infrastructure/api/auth/authApi"
import { clearAuth } from "../../infrastructure/storage/localStorageService"

export function useLogout() {
  const navigate = useNavigate()

  const handleLogout = async () => {
    await logout()
    clearAuth()
    navigate("/login", { replace: true })
    toast.success("Uspješno ste se odjavili iz sistema.")
  }

  return { handleLogout }
}