import { useState, useEffect, useCallback } from "react"
import { useParams, useNavigate } from "react-router-dom"
import { MapContainer, Marker, TileLayer, Polyline, Popup } from "react-leaflet"
import { toast } from "sonner"
import { routesApi } from "../../../infrastructure/api/routesApi"
import type { RouteResponse } from "../../../infrastructure/api/routesApi"
import { Layout } from "../../components/Layout/Layout"
import "leaflet/dist/leaflet.css"

type RouteDetailsSource = "archive" | "tracking"

interface ArchiveRouteDetailsPageProps {
    source?: RouteDetailsSource
}

export default function ArchiveRouteDetailsPage({ source = "archive" }: ArchiveRouteDetailsPageProps) {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()
    const [route, setRoute] = useState<RouteResponse | null>(null)
    const [loading, setLoading] = useState(true)
    const backPath = source === "archive" ? "/admin/routes/archive" : "/admin/routes/dashboard"

    const loadRoute = useCallback(async () => {
        if (!id) return
        try {
            setLoading(true)
            const res = await routesApi.getRouteDetails(id)
            setRoute(res)
        } catch {
            toast.error("Greška pri učitavanju rute")
            navigate(backPath)
        } finally {
            setLoading(false)
        }
    }, [id, navigate, backPath])

    useEffect(() => { void loadRoute() }, [loadRoute])

    const exportToCsv = () => {
        if (!route) return

        const headers = ["Redoslijed", "Adresa", "Prioritet", "Planirani status", "Završni status", "Vrijeme aktivnosti", "Razlog nedostupnosti"]
        const csvContent = [
            headers.join(","),
            ...route.routeItems.map(item => [
                item.order,
                `"${item.address.replaceAll('"', '""')}"`,
                item.priority,
                item.status,
                item.processedStatus || item.mailboxStatus,
                item.processedAt || "-",
                `"${(item.unavailableReason || "-").replaceAll('"', '""')}"`
            ].join(","))
        ].join("\n")

        const blob = new Blob([`\uFEFF${csvContent}`], { type: "text/csv;charset=utf-8;" })
        const link = document.createElement("a")
        const url = URL.createObjectURL(blob)
        link.setAttribute("href", url)
        link.setAttribute("download", `arhivirana_ruta_${route.date}_${route.postmanName ?? route.id}.csv`)
        link.style.visibility = "hidden"
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        URL.revokeObjectURL(url)
    }

    if (loading || !route) {
        return <Layout><div className="page-container"><p>Učitavanje...</p></div></Layout>
    }

    const routeCoordinates = route.routeItems.map(i => [i.latitude, i.longitude] as [number, number])
    const isArchivedRoute = route.status === "Zavrsena" || route.status === "Otkazana"
    const title = isArchivedRoute ? "Detalji arhivirane rute" : "Detalji aktivne rute"
    const helperText = isArchivedRoute
        ? "Read-only arhivski pregled. Statusi i vremena se ne mogu mijenjati iz ovog prikaza."
        : "Pregled rute iz praćenja. Statusi se mijenjaju kroz poštarski prikaz rute."

    return (
        <Layout>
            <div className="page-container">
                <div className="form-card" style={{ maxWidth: '1100px', margin: '0 auto' }}>
                    <div className="form-card__header" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: '16px', flexWrap: 'wrap' }}>
                        <div>
                            <h2 className="form-card__title">{title}</h2>
                            <p className="form-card__subtitle">Datum: {route.date} | Poštar: {route.postmanName} | Status: {route.status}</p>
                            <p className="form-card__subtitle">Započeta: {route.startedAt || "-"} | Završena: {route.completedAt || "-"}</p>
                            <p className="form-card__subtitle">{helperText}</p>
                        </div>
                        <div style={{ display: 'flex', gap: '8px' }}>
                            <button className="btn btn--outline" onClick={() => navigate(backPath)}>Nazad</button>
                            <button className="btn btn--primary" onClick={exportToCsv}>Export CSV za Excel</button>
                        </div>
                    </div>

                    <div className="form-card__body">
                        {routeCoordinates.length > 0 && (
                            <div style={{ height: "400px", marginBottom: "20px", borderRadius: "8px", overflow: "hidden" }}>
                                <MapContainer center={routeCoordinates[0]} zoom={13} style={{ height: "100%", width: "100%" }}>
                                    <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
                                    <Polyline positions={routeCoordinates} color="blue" />
                                    {route.routeItems.map((item, index) => (
                                        <Marker key={item.id} position={[item.latitude, item.longitude]}>
                                            <Popup>
                                                <strong>{index + 1}. {item.address}</strong><br />
                                                Status: {item.processedStatus || item.mailboxStatus}<br />
                                                {item.unavailableReason ? <>Razlog: {item.unavailableReason}<br /></> : null}
                                                Obrađeno: {item.processedAt || "Nepoznato"}
                                            </Popup>
                                        </Marker>
                                    ))}
                                </MapContainer>
                            </div>
                        )}

                        <table className="table" style={{ width: '100%', textAlign: 'left', borderCollapse: 'collapse' }}>
                            <thead>
                                <tr>
                                    <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>R.br.</th>
                                    <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>Adresa</th>
                                    <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>Prioritet</th>
                                    <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>Završni status</th>
                                    <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>Obrađeno u</th>
                                    <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>Razlog problema</th>
                                </tr>
                            </thead>
                            <tbody>
                                {route.routeItems.map(item => {
                                    const finalStatus = item.processedStatus || item.mailboxStatus
                                    return (
                                        <tr key={item.id}>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{item.order}</td>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{item.address}</td>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{item.priority}</td>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>
                                                <span style={{ fontWeight: 'bold', color: finalStatus === "Nedostupan" ? "red" : "green" }}>
                                                    {finalStatus}
                                                </span>
                                            </td>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{item.processedAt || "-"}</td>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{item.unavailableReason || "-"}</td>
                                        </tr>
                                    )
                                })}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </Layout>
    )
}
