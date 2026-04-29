import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { toast } from "sonner"
import { httpClient } from "../../infrastructure/api/httpClient"

interface User {
  id: string
  username: string
  email: string
  role: string
  mustChangePassword: boolean
}

export function useAuth() {
  const [currentUser, setCurrentUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)
  const navigate = useNavigate()

  useEffect(() => {
    checkAuthStatus()
  }, [])

  const checkAuthStatus = async () => {
    try {
      const result = await httpClient<User>("/api/users/current-user")

      if (result.data) {
        setCurrentUser(result.data)
      } else {
        setCurrentUser(null)
        navigate("/login")
      }
    } catch (error) {
      console.error("Auth check failed:", error)
      setCurrentUser(null)
      navigate("/login")
    } finally {
      setLoading(false)
    }
  }

  const login = async (email: string, password: string) => {
    const result = await httpClient<User>("/api/users/login", {
      method: "POST",
      body: { email, password },
    })

    if (!result.data) {
      if (result.status === 423) {
        const err = new Error("Account locked") as Error & { status?: number }
        err.status = 423
        throw err
      }
      throw new Error(result.error || "Login failed")
    }

    const user = result.data
    setCurrentUser(user)

    if (user.mustChangePassword) {
      navigate("/change-password")
    } else {
      navigate("/dashboard")
    }

    return user
  }

  const logout = async () => {
    try {
      await httpClient("/api/users/logout", { method: "POST" })
    } catch (error) {
      console.error("Logout error:", error)
    } finally {
      setCurrentUser(null)
      navigate("/login", { replace: true })
      toast.success("Uspješno ste se odjavili iz sistema.")
    }
  }

  return {
    currentUser,
    loading,
    login,
    logout,
    checkAuthStatus,
  }
}
