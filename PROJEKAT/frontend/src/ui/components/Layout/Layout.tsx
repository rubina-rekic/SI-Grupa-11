import { useState } from "react"
import { Sidebar } from "./Sidebar"
import { useAuth } from "../../../application/hooks/useAuth"

interface LayoutProps {
  children: React.ReactNode
}

export function Layout({ children }: LayoutProps) {
  const { currentUser, logout } = useAuth()
  const [sidebarOpen, setSidebarOpen] = useState(true)

  if (!currentUser) {
    return <div>Loading...</div>
  }

  const roleLabel =
    currentUser.role === "Administrator"
      ? "Administrator"
      : currentUser.role === "Dispatcher"
        ? "Dispečer"
        : "Poštar"

  return (
    <div className="app-layout">
      <div className={`sidebar-container ${sidebarOpen ? "open" : "closed"}`}>
        <Sidebar userRole={currentUser.role} username={currentUser.username} />
      </div>

      <div className="main-content">
        <header className="top-header">
          <button className="sidebar-toggle" onClick={() => setSidebarOpen(!sidebarOpen)}>
            ☰
          </button>

          <div className="header-right">
            <span className="welcome-message">
              Dobrodošli, {currentUser.username} ({roleLabel})
            </span>
            <button onClick={logout} className="btn-secondary">
              Odjava
            </button>
          </div>
        </header>

        <main className="content-area">{children}</main>
      </div>
    </div>
  )
}
