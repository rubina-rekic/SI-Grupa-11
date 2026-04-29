import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"

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
      const response = await fetch("/api/users/current-user", {
        credentials: "include"
      })
      
      if (response.ok) {
        const user = await response.json()
        setCurrentUser(user)
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
    try {
      const response = await fetch("/api/users/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify({ email, password }),
      })

      if (!response.ok) {
        if (response.status === 423) {
          const err = new Error("Account locked") as Error & { status?: number }
          err.status = 423
          throw err
        }
        if (response.status === 403) {
          const errorData = await response.json()
          throw new Error(errorData.message || "Access denied")
        }
        throw new Error("Login failed")
      }

      const user = await response.json()
      setCurrentUser(user)
      
      if (user.mustChangePassword) {
        navigate("/change-password")
      } else {
        navigate("/dashboard")
      }
      
      return user
    } catch (error: any) {
      console.error("Login error:", error)
      throw error
    }
  }

  const logout = async () => {
    try {
      await fetch("/api/users/logout", {
        method: "POST",
        credentials: "include"
      })
    } catch (error) {
      console.error("Logout error:", error)
    } finally {
      setCurrentUser(null)
      navigate("/login")
    }
  }

  return {
    currentUser,
    loading,
    login,
    logout,
    checkAuthStatus
  }
}
