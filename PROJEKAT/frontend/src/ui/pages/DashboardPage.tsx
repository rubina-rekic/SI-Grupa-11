import { useAuth } from "../../application/hooks/useAuth"
import { Layout } from "../components/Layout/Layout"
import { useLocation, useNavigate } from "react-router-dom"
import { useEffect } from "react"
import { toast } from "sonner"

export default function DashboardPage() {
  const { currentUser } = useAuth()
  const location = useLocation()
  const navigate = useNavigate()

  useEffect(() => {
    if (location.state?.accessDenied) {
      toast.error("Pristup odbijen", {
        description: "Nemate potrebne privilegije za pregled ove stranice.",
      })
      // Clear the state to prevent showing toast again on refresh
      window.history.replaceState({}, document.title)
    }
  }, [location.state])

  if (!currentUser) {
    return <div>Loading...</div>
  }

  const renderAdminDashboard = () => (
    <div className="dashboard-grid">
      <div className="dashboard-card">
        <h3>👥 Upravljanje korisnicima</h3>
        <p>Kreirajte i upravljajte korisničkim računima poštara</p>
        <button className="btn-primary" onClick={() => navigate("/admin/users")}>Upravljanje korisnicima</button>
      </div>

      <div className="dashboard-card">
        <h3>📮 Pregled sandučića</h3>
        <p>Pregledajte status sandučića i lokacije</p>
        <button className="btn-primary" onClick={() => navigate("/admin/mailboxes")}>Pregled sandučića</button>
      </div>

      <div className="dashboard-card">
        <h3>📊 Statistika sistema</h3>
        <p>Analizirajte performanse i statistike</p>
        <button className="btn-primary" onClick={() => navigate("/admin/statistics")}>Statistike</button>
      </div>

      <div className="dashboard-card">
        <h3>⚙️ Postavke sistema</h3>
        <p>Konfigurišite sistemske postavke</p>
        <button className="btn-primary" onClick={() => navigate("/admin/settings")}>Postavke</button>
      </div>
    </div>
  )

  const renderPostalWorkerDashboard = () => (
    <div className="dashboard-grid">
      <div className="dashboard-card">
        <h3>🗺️ Moja današnja ruta</h3>
        <p>Pregledajte današnju dostavnu rutu</p>
        <button className="btn-primary" onClick={() => navigate("/worker/route")}>Prikaži rutu</button>
      </div>

      <div className="dashboard-card">
        <h3>📍 Mapa sandučića</h3>
        <p>Lokacije sandučića na vašoj ruti</p>
        <button className="btn-primary" onClick={() => navigate("/worker/mailboxes")}>Mapa</button>
      </div>

      <div className="dashboard-card">
        <h3>⚠️ Prijava problema</h3>
        <p>Prijavite probleme na terenu</p>
        <button className="btn-primary" onClick={() => navigate("/worker/issues")}>Prijavi problem</button>
      </div>
    </div>
  )

  return (
    <Layout>
      <div className="dashboard-page">
        <div className="dashboard-header">
          <h1>
            Dobrodošli, {currentUser.username} ({currentUser.role === "Administrator" ? "Administrator" : "Poštar"})
          </h1>
          <p className="dashboard-subtitle">
            {currentUser.role === "Administrator"
              ? "Upravljajte PostRoute sistemom"
              : "Upravljajte vašom dostavnom rutom"
            }
          </p>
        </div>

        {currentUser.role === "Administrator" ? renderAdminDashboard() : renderPostalWorkerDashboard()}
      </div>
    </Layout>
  )
}