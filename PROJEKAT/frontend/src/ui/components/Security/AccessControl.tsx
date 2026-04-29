import { useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { useAuth } from "../../../application/hooks/useAuth"
import { toast } from "sonner"

interface AccessControlProps {
  children: React.ReactNode
  requiredRole?: string
}

export function AccessControl({ children, requiredRole }: AccessControlProps) {
  const { currentUser, loading } = useAuth()
  const navigate = useNavigate()

  useEffect(() => {
    if (!loading) {
      if (!currentUser) {
        toast.error("Sesija istekla", {
          description: "Molimo prijavite se ponovo.",
        })
        navigate("/login")
        return
      }

      if (requiredRole && currentUser.role !== requiredRole) {
        toast.error("Pristup odbijen", {
          description: "Nemate potrebne privilegije za ovu akciju.",
        })
        navigate("/dashboard")
        return
      }
    }
  }, [currentUser, loading, requiredRole, navigate])

  if (loading) {
    return <div>Loading...</div>
  }

  if (!currentUser) {
    return null
  }

  if (requiredRole && currentUser.role !== requiredRole) {
    return null
  }

  return <>{children}</>
}
