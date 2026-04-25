import { Navigate, Route, Routes } from "react-router-dom"
import { CreatePostalWorkerPage } from "../../ui/pages/admin/CreatePostalWorkerPage"

export function AppRouter() {
  return (
    <Routes>
      <Route path="/admin/users/new" element={<CreatePostalWorkerPage />} />
      <Route path="*" element={<Navigate to="/admin/users/new" replace />} />
    </Routes>
  )
}
