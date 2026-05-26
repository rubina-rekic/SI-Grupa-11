import { useState } from "react"
import { Sidebar } from "./Sidebar"
import { useAuth } from "../../../application/hooks/useAuth"
import { NotificationPanel } from "../PostmanRoute/NotificationPanel"

interface LayoutProps {
  children: React.ReactNode
}

export function Layout({ children }: LayoutProps) {
  const { currentUser, logout } = useAuth()
  const [sidebarOpen, setSidebarOpen] = useState(() => window.innerWidth > 768)

  if (!currentUser) {
    return <div>Loading...</div>
  }

  const roleLabel =
    currentUser.role === "Administrator" ? "Administrator"
    : currentUser.role === "Dispatcher" ? "Dispečer"
    : "Poštar"

  const closeSidebarOnMobile = () => {
    if (window.innerWidth <= 768) setSidebarOpen(false)
  }

  return (
    <div className="app-layout">
      {sidebarOpen && (
        <div className="sidebar-overlay" onClick={() => setSidebarOpen(false)} />
      )}

      <div className={`sidebar-container ${sidebarOpen ? "open" : "closed"}`}>
        <Sidebar
          userRole={currentUser.role}
          username={currentUser.username}
          onNavClick={closeSidebarOnMobile}
        />
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

            {/* ← Zvonce samo za poštara, direktno u headeru */}
            {currentUser.role === "PostalWorker" && (
              <NotificationPanel />
            )}

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