import { useLogout } from "../../application/hooks/useLogout"

export default function DashboardPage() {
  const { handleLogout } = useLogout()

  return (
    <div className="page-container">
      <div className="form-card">
        <header style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <h1>Dashboard</h1>
          <button onClick={handleLogout} className="btn-secondary">
            Odjava
          </button>
        </header>
        <p>Uspješno ste prijavljeni u PostRoute sistem.</p>
      </div>
    </div>
  )
}