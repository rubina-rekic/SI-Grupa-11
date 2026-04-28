import { Navigate, Route, Routes } from "react-router-dom"
import { CreatePostalWorkerPage } from "../../ui/pages/admin/CreatePostalWorkerPage"
import LoginPage from "../../ui/pages/LoginPage"
import DashboardPage from "../../ui/pages/DashboardPage"
import ChangePasswordPage from "../../ui/pages/ChangePasswordPage"

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const token = localStorage.getItem("token")
  return token ? <>{children}</> : <Navigate to="/login" replace />
}

export function AppRouter() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/change-password" element={<ChangePasswordPage />} />
      <Route
        path="/admin/users/new"
        element={
          <PrivateRoute>
            <CreatePostalWorkerPage />
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