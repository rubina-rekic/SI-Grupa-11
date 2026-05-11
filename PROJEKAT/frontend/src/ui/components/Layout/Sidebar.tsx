import { NavLink } from "react-router-dom"

interface SidebarProps {
  userRole: string
  username: string
}

export function Sidebar({ userRole, username }: SidebarProps) {
  const adminMenuItems = [
    { path: "/dashboard", label: "Dashboard", icon: "🏠" },
    { path: "/admin/users/new", label: "Upravljanje korisnicima", icon: "👥" },
    { path: "/admin/mailboxes", label: "Pregled sandučića", icon: "📬" },
    { path: "/admin/routes/generate", label: "Generisanje ruta", icon: "🗺️" },
    { path: "/admin/statistics", label: "Statistika sistema", icon: "📊" },
    { path: "/admin/settings", label: "Postavke", icon: "⚙️" },
  ]

  const dispatcherMenuItems = [
    { path: "/dashboard", label: "Dashboard", icon: "🏠" },
    { path: "/admin/routes/generate", label: "Generisanje ruta", icon: "🗺️" },
  ]

  const postalWorkerMenuItems = [
    { path: "/dashboard", label: "Dashboard", icon: "🏠" },
    { path: "/worker/route", label: "Moja današnja ruta", icon: "🗺️" },
    { path: "/worker/mailboxes", label: "Mapa sandučića", icon: "📍" },
    { path: "/worker/issues", label: "Prijava problema na terenu", icon: "⚠️" },
  ]

  const menuItems =
    userRole === "Administrator"
      ? adminMenuItems
      : userRole === "Dispatcher"
        ? dispatcherMenuItems
        : postalWorkerMenuItems

  const roleLabel =
    userRole === "Administrator"
      ? "Administrator"
      : userRole === "Dispatcher"
        ? "Dispečer"
        : "Poštar"

  return (
    <div className="sidebar">
      <div className="sidebar-header">
        <h3>PostRoute</h3>
        <div className="user-info">
          <span className="username">{username}</span>
          <span className="role-badge">{roleLabel}</span>
        </div>
      </div>

      <nav className="sidebar-nav">
        <ul className="nav-list">
          {menuItems.map((item) => (
            <li key={item.path} className="nav-item">
              <NavLink
                to={item.path}
                className={({ isActive }) => `nav-link ${isActive ? "active" : ""}`}
              >
                <span className="nav-icon">{item.icon}</span>
                <span className="nav-label">{item.label}</span>
              </NavLink>
            </li>
          ))}
        </ul>
      </nav>
    </div>
  )
}
