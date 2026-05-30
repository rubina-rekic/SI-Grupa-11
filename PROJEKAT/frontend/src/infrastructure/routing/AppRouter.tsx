import { Navigate, Route, Routes, useLocation } from "react-router-dom"
import { CreatePostalWorkerPage } from "../../ui/pages/admin/CreatePostalWorkerPage"
import CreateMailboxPage from "../../ui/pages/admin/CreateMailboxPage"
import EditMailboxPage from "../../ui/pages/admin/EditMailboxPage"
import LoginPage from "../../ui/pages/LoginPage"
import DashboardPage from "../../ui/pages/DashboardPage"
import ChangePasswordPage from "../../ui/pages/ChangePasswordPage"
import { useAuth } from "../../application/hooks/useAuth"
import { Layout } from "../../ui/components/Layout/Layout"
import PostalWorkersListPage from "../../ui/pages/admin/PostalWorkersListPage"
import MailboxListPage from "../../ui/pages/admin/MailboxListPage"
import MailboxHistoryPage from "../../ui/pages/admin/MailboxHistoryPage"
import GenerateRoutePage from "../../ui/pages/admin/GenerateRoutePage"
import DispatcherRouteDashboardPage from "../../ui/pages/admin/DispatcherRouteDashboardPage"
import PostmanAssignedRoutePage from "../../ui/pages/PostmanAssignedRoutePage"
import IssueDetailPage from "../../ui/pages/admin/IssueDetailPage"
import ArchiveRouteListPage from "../../ui/pages/admin/ArchiveRouteListPage"
import ArchiveRouteDetailsPage from "../../ui/pages/admin/ArchiveRouteDetailsPage"

function PrivateRoute({ children, requiredRole, requiredRoles }: { children: React.ReactNode; requiredRole?: string; requiredRoles?: string[] }) {
  const { currentUser, loading } = useAuth()
  const location = useLocation()

  if (loading) {
    return <div>Loading...</div>
  }

  if (!currentUser) {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  if (currentUser.mustChangePassword) {
    return <Navigate to="/change-password" replace />
  }

  const hasRequiredRole = requiredRole ? currentUser.role === requiredRole : true
  const hasOneOfRequiredRoles = requiredRoles ? requiredRoles.includes(currentUser.role) : true
  if (!hasRequiredRole || !hasOneOfRequiredRoles) {
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
        path="/admin/mailboxes/new"
        element={
          <PrivateRoute requiredRole="Administrator">
            <CreateMailboxPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/mailboxes"
        element={
          <PrivateRoute requiredRole="Administrator">
            <MailboxListPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/mailboxes/:id/edit"
        element={
          <PrivateRoute requiredRole="Administrator">
            <EditMailboxPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/statistics"
        element={
          <PrivateRoute requiredRole="Administrator">
            <Layout>
              <div className="page-container">
                <div className="form-card">
                  <h1>Statistika sistema</h1>
                  <p>Admin funkcionalnost za statistike</p>
                </div>
              </div>
            </Layout>
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/settings"
        element={
          <PrivateRoute requiredRole="Administrator">
            <Layout>
              <div className="page-container">
                <div className="form-card">
                  <h1>Postavke sistema</h1>
                  <p>Admin funkcionalnost za postavke</p>
                </div>
              </div>
            </Layout>
          </PrivateRoute>
        }
      />
      <Route
        path="/worker/route"
        element={
          <PrivateRoute requiredRole="PostalWorker">
            <PostmanAssignedRoutePage />
          </PrivateRoute>
        }
      />
      <Route
        path="/worker/mailboxes"
        element={
          <PrivateRoute requiredRole="PostalWorker">
            <Layout>
              <div className="page-container">
                <div className="form-card">
                  <h1>Mapa sandučića</h1>
                  <p>Funkcionalnost za poštare - mapa sandučića</p>
                </div>
              </div>
            </Layout>
          </PrivateRoute>
        }
      />
      <Route
        path="/worker/issues"
        element={
          <PrivateRoute requiredRole="PostalWorker">
            <Layout>
              <div className="page-container">
                <div className="form-card">
                  <h1>Prijava problema na terenu</h1>
                  <p>Funkcionalnost za poštare - prijava problema</p>
                </div>
              </div>
            </Layout>
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
      <Route
        path="/admin/users"
        element={
          <PrivateRoute requiredRole="Administrator">
            <PostalWorkersListPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/mailboxes/history"
        element={
          <PrivateRoute requiredRole="Administrator">
            <MailboxHistoryPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/routes/generate"
        element={
          <PrivateRoute requiredRoles={["Administrator", "Dispatcher"]}>
            <GenerateRoutePage />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/routes/dashboard"
        element={
          <PrivateRoute requiredRoles={["Administrator", "Dispatcher"]}>
            <DispatcherRouteDashboardPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/routes/:id"
        element={
          <PrivateRoute requiredRoles={["Administrator", "Dispatcher"]}>
            <ArchiveRouteDetailsPage source="tracking" />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/issues"
        element={
          <PrivateRoute requiredRoles={["Administrator", "Dispatcher"]}>
            <IssueDetailPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/issues/:id"
        element={
          <PrivateRoute requiredRoles={["Administrator", "Dispatcher"]}>
            <IssueDetailPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/worker/issues/:id"
        element={
          <PrivateRoute requiredRole="PostalWorker">
            <IssueDetailPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/routes/archive"
        element={
          <PrivateRoute requiredRoles={["Administrator", "Dispatcher"]}>
            <ArchiveRouteListPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/routes/archive/:id"
        element={
          <PrivateRoute requiredRoles={["Administrator", "Dispatcher"]}>
            <ArchiveRouteDetailsPage source="archive" />
          </PrivateRoute>
        }
      />
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  )
}
