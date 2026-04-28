import { Navigate, Route, Routes } from "react-router-dom"
import { CreatePostalWorkerPage } from "../../ui/pages/admin/CreatePostalWorkerPage"
import LoginPage from "../../ui/pages/LoginPage"
import DashboardPage from "../../ui/pages/DashboardPage"
import ChangePasswordPage from "../../ui/pages/ChangePasswordPage";

export function AppRouter() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/admin/users/new" element={<CreatePostalWorkerPage />} />
      <Route path="/dashboard" element={<DashboardPage />} />
      <Route path="/change-password" element={<ChangePasswordPage />} />
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  )
}
