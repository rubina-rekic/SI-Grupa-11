import { Navigate, Route, Routes } from "react-router-dom"
import { CreatePostalWorkerPage } from "../../ui/pages/admin/CreatePostalWorkerPage"
import LoginPage from "../../ui/pages/LoginPage"

export function AppRouter() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/admin/users/new" element={<CreatePostalWorkerPage />} />
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  )
}
