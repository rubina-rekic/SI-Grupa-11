import { Navigate, Route, Routes, useLocation } from "react-router-dom"
import { CreatePostalWorkerPage } from "../../ui/pages/admin/CreatePostalWorkerPage"
import LoginPage from "../../ui/pages/LoginPage"
import DashboardPage from "../../ui/pages/DashboardPage"
import ChangePasswordPage from "../../ui/pages/ChangePasswordPage"
import { useAuth } from "../../application/hooks/useAuth"

function PrivateRoute({ children, requiredRole }: { children: React.ReactNode; requiredRole?: string }) {
  const { currentUser, loading } = useAuth()
  const location = useLocation()

  if (loading) {
    return <div>Loading...</div>
  }

  if (!currentUser) {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  if (requiredRole && currentUser.role !== requiredRole) {
    // Show toast notification for access denied
    setTimeout(() => {
      // This will be handled by a global error boundary or toast system
      console.error("Access denied: User role doesn't match required role")
    }, 0)
    return <Navigate to="/dashboard" replace state={{ accessDenied: true }} />
  }

  return <>{children}</>
}

export function AppRouter() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/change-password" element={<ChangePasswordPage />} />
      <Route
        path="/admin/users/new"
        element={
          <PrivateRoute requiredRole="Administrator">
            <CreatePostalWorkerPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/mailboxes"
        element={
          <PrivateRoute requiredRole="Administrator">
            <div className="page-container">
              <div className="form-card">
                <h1>Pregled sandučića</h1>
                <p>Admin funkcionalnost za pregled sandučića</p>
              </div>
            </div>
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/statistics"
        element={
          <PrivateRoute requiredRole="Administrator">
            <div className="page-container">
              <div className="form-card">
                <h1>Statistika sistema</h1>
                <p>Admin funkcionalnost za statistike</p>
              </div>
            </div>
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/settings"
        element={
          <PrivateRoute requiredRole="Administrator">
            <div className="page-container">
              <div className="form-card">
                <h1>Postavke sistema</h1>
                <p>Admin funkcionalnost za postavke</p>
              </div>
            </div>
          </PrivateRoute>
        }
      />
      <Route
        path="/worker/route"
        element={
          <PrivateRoute requiredRole="PostalWorker">
            <div className="page-container">
              <div className="form-card">
                <h1>Moja današnja ruta</h1>
                <p>Funkcionalnost za poštare - prikaz rute</p>
              </div>
            </div>
          </PrivateRoute>
        }
      />
      <Route
        path="/worker/mailboxes"
        element={
          <PrivateRoute requiredRole="PostalWorker">
            <div className="page-container">
              <div className="form-card">
                <h1>Mapa sandučića</h1>
                <p>Funkcionalnost za poštare - mapa sandučića</p>
              </div>
            </div>
          </PrivateRoute>
        }
      />
      <Route
        path="/worker/issues"
        element={
          <PrivateRoute requiredRole="PostalWorker">
            <div className="page-container">
              <div className="form-card">
                <h1>Prijava problema na terenu</h1>
                <p>Funkcionalnost za poštare - prijava problema</p>
              </div>
            </div>
          </PrivateRoute>
        }
      />
      <Route
        path="/dashboard"
        element={
          <PrivateRoute>
            <DashboardPage />
          </PrivateRoute>
        }
      />
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  )
}