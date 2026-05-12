import { useEffect, useMemo, useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { toast } from "sonner"
import { MapContainer as LeafletMap, Marker, Popup, TileLayer } from "react-leaflet"
import L from "leaflet"
import "leaflet/dist/leaflet.css"
import { Layout } from "../../components/Layout/Layout"
import { LeafletRoutingMachine } from "../../components/common/LeafletRoutingMachine"
import { routesApi } from "../../../infrastructure/api/routesApi"
import type { RouteResponse } from "../../../infrastructure/api/routesApi"
import { getUsers } from "../../../infrastructure/api/users/usersApi"
import type { UserListDto } from "../../../infrastructure/api/users/usersApi"

delete (L.Icon.Default.prototype as L.Icon.Default & { _getIconUrl?: unknown })._getIconUrl
L.Icon.Default.mergeOptions({
  iconRetinaUrl: "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png",
  iconUrl: "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png",
  shadowUrl: "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png",
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41],
})

const generateRouteSchema = z.object({
  postmanId: z.string().min(1, "Poštar je obavezan"),
  date: z.string().min(1, "Datum je obavezan"),
  plannedStartTime: z.string().min(1, "Planirano vrijeme početka je obavezno"),
})

type GenerateRouteFormValues = z.infer<typeof generateRouteSchema>

function toDurationLabel(totalMinutes: number) {
  return `${Math.floor(totalMinutes / 60)}h ${totalMinutes % 60}m`
}

function toHoursAndMinutes(timeValue: string | null | undefined) {
  if (!timeValue) {
    return "--:--"
  }

  return timeValue.split(":").slice(0, 2).join(":")
}

function priorityColors(priority: string) {
  if (priority === "Visok") return { bg: "#fee2e2", fg: "#b91c1c" }
  if (priority === "Srednji") return { bg: "#fef3c7", fg: "#92400e" }
  return { bg: "#dcfce7", fg: "#166534" }
}

export default function GenerateRoutePage() {
  const [postmen, setPostmen] = useState<UserListDto[]>([])
  const [routeData, setRouteData] = useState<RouteResponse | null>(null)
  const [loading, setLoading] = useState(false)
  const [fetchingUsers, setFetchingUsers] = useState(true)

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<GenerateRouteFormValues>({
    resolver: zodResolver(generateRouteSchema),
    defaultValues: {
      postmanId: "",
      date: new Date().toISOString().split("T")[0],
      plannedStartTime: "08:00",
    },
  })

  useEffect(() => {
    async function loadUsers() {
      try {
        const res = await getUsers()
        if (res.data) {
          setPostmen(res.data.filter((u) => u.role === "PostalWorker"))
        }
      } catch {
        toast.error("Greška pri učitavanju poštara")
      } finally {
        setFetchingUsers(false)
      }
    }

    void loadUsers()
  }, [])

  const onSubmit = async (data: GenerateRouteFormValues) => {
    try {
      setLoading(true)
      setRouteData(null)
      const result = await routesApi.generateRoute({
        postmanId: data.postmanId,
        date: data.date,
        plannedStartTime: `${data.plannedStartTime}:00`,
      })

      setRouteData(result)

      if (result.exceedsStandardTime) {
        toast.warning("Upozorenje: Ruta premašuje standardno radno vrijeme.")
      } else {
        toast.success("Dnevna ruta uspješno generisana.")
      }
    } catch (err: unknown) {
      if (err instanceof Error) {
        toast.error(err.message)
      } else if (err && typeof err === "object" && "error" in err && typeof (err as Record<string, unknown>).error === "string") {
        toast.error((err as Record<string, unknown>).error as string)
      } else {
        toast.error("Nema dostupnih lokacija za generisanje rute. Provjerite status, radni dan i dostupnost sandučića.")
      }
    } finally {
      setLoading(false)
    }
  }

  const mapCenter = useMemo<[number, number]>(() => {
    if (!routeData || routeData.routeItems.length === 0) return [43.8563, 18.4131]
    return [routeData.routeItems[0].latitude, routeData.routeItems[0].longitude]
  }, [routeData])

  const routeWaypoints = useMemo<Array<[number, number]>>(() => {
    if (!routeData) {
      return []
    }

    return routeData.routeItems.map((item) => [item.latitude, item.longitude])
  }, [routeData])

  return (
    <Layout>
      <div className="page-container">
        <div style={{ width: "100%", maxWidth: "1100px", display: "flex", flexDirection: "column", gap: "20px" }}>
          <section className="form-card" style={{ maxWidth: "unset" }}>
            <div className="form-card__header">
              <h1 className="form-card__title">Generisanje dnevne rute</h1>
              <p className="form-card__subtitle">
                Odaberite poštara, datum i vrijeme početka. Sistem računa rutu po prioritetima, dostupnosti i lokaciji.
              </p>
            </div>

            <form className="form-card__body" onSubmit={handleSubmit(onSubmit)} noValidate>
              <div className="form-row">
                <div className="form-field">
                  <label htmlFor="postmanId" className="form-field__label">Poštar *</label>
                  <select id="postmanId" {...register("postmanId")} className="form-field__input" disabled={fetchingUsers}>
                    <option value="">Odaberite poštara</option>
                    {postmen.map((postman) => (
                      <option key={postman.id} value={postman.id}>
                        {postman.username} ({postman.email})
                      </option>
                    ))}
                  </select>
                  {errors.postmanId && <span className="form-field__error">{errors.postmanId.message}</span>}
                </div>
              </div>

              <div className="form-row">
                <div className="form-field">
                  <label htmlFor="date" className="form-field__label">Datum rute *</label>
                  <input id="date" type="date" {...register("date")} className="form-field__input" />
                  {errors.date && <span className="form-field__error">{errors.date.message}</span>}
                </div>

                <div className="form-field">
                  <label htmlFor="plannedStartTime" className="form-field__label">Planirano vrijeme početka *</label>
                  <input id="plannedStartTime" type="time" {...register("plannedStartTime")} className="form-field__input" />
                  {errors.plannedStartTime && <span className="form-field__error">{errors.plannedStartTime.message}</span>}
                </div>
              </div>

              <div className="form-actions">
                <button type="submit" className="btn btn--primary" disabled={loading || fetchingUsers}>
                  {loading ? "Generisanje rute u toku..." : "Generiši rutu"}
                </button>
              </div>
            </form>
          </section>

          {routeData && (
            <>
              <section className="form-card" style={{ maxWidth: "unset" }}>
                <div className="form-card__body" style={{ gap: "18px" }}>
                  <h2 style={{ margin: 0, fontSize: "1.1rem", color: "#1e2d3d" }}>Sažetak rute</h2>
                  <div
                    style={{
                      display: "grid",
                      gridTemplateColumns: "repeat(auto-fit, minmax(180px, 1fr))",
                      gap: "12px",
                    }}
                  >
                    <div style={{ background: "#f8fafc", border: "1px solid #e2e8f0", borderRadius: "8px", padding: "12px" }}>
                      <div style={{ color: "#64748b", fontSize: "0.8rem" }}>Broj lokacija</div>
                      <div style={{ color: "#1e2d3d", fontSize: "1.15rem", fontWeight: 700 }}>{routeData.routeItems.length}</div>
                    </div>
                    <div style={{ background: "#f8fafc", border: "1px solid #e2e8f0", borderRadius: "8px", padding: "12px" }}>
                      <div style={{ color: "#64748b", fontSize: "0.8rem" }}>Ukupna distanca</div>
                      <div style={{ color: "#1e2d3d", fontSize: "1.15rem", fontWeight: 700 }}>{routeData.totalDistanceKm} km</div>
                    </div>
                    <div style={{ background: "#f8fafc", border: "1px solid #e2e8f0", borderRadius: "8px", padding: "12px" }}>
                      <div style={{ color: "#64748b", fontSize: "0.8rem" }}>Planirano trajanje</div>
                      <div style={{ color: routeData.exceedsStandardTime ? "#b45309" : "#1e2d3d", fontSize: "1.15rem", fontWeight: 700 }}>
                        {toDurationLabel(routeData.totalDurationMinutes)}
                      </div>
                    </div>
                    <div style={{ background: "#f8fafc", border: "1px solid #e2e8f0", borderRadius: "8px", padding: "12px" }}>
                      <div style={{ color: "#64748b", fontSize: "0.8rem" }}>Vremenski raspon</div>
                      <div style={{ color: "#1e2d3d", fontSize: "1.15rem", fontWeight: 700 }}>
                        {toHoursAndMinutes(routeData.plannedStartTime)} - {toHoursAndMinutes(routeData.plannedEndTime)}
                      </div>
                    </div>
                  </div>
                  {routeData.exceedsStandardTime && (
                    <div
                      style={{
                        background: "#fff7ed",
                        color: "#9a3412",
                        border: "1px solid #fed7aa",
                        borderRadius: "8px",
                        padding: "10px 12px",
                        fontSize: "0.88rem",
                        fontWeight: 600,
                      }}
                    >
                      Upozorenje: Ruta premašuje standardno radno vrijeme.
                    </div>
                  )}
                </div>
              </section>

              <section className="form-card" style={{ maxWidth: "unset" }}>
                <div className="form-card__body" style={{ gap: "12px" }}>
                  <h2 style={{ margin: 0, fontSize: "1.1rem", color: "#1e2d3d" }}>Ruta na mapi</h2>
                  <div className="route-map-shell" style={{ height: "420px", borderRadius: "8px", overflow: "hidden", border: "1px solid #e2e8f0" }}>
                    <LeafletMap className="route-map" center={mapCenter} zoom={13} style={{ height: "100%", width: "100%" }}>
                      <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
                      {routeWaypoints.length >= 2 && <LeafletRoutingMachine waypoints={routeWaypoints} />}
                      {routeData.routeItems.map((item) => (
                        <Marker key={item.id} position={[item.latitude, item.longitude]}>
                          <Popup>
                            <strong>{item.order}.</strong> {item.address}
                            <br />
                            Prioritet: {item.priority}
                            <br />
                            Planirani dolazak: {item.estimatedArrivalTime}
                          </Popup>
                        </Marker>
                      ))}
                    </LeafletMap>
                  </div>
                </div>
              </section>

              <section className="form-card" style={{ maxWidth: "unset" }}>
                <div className="form-card__body" style={{ gap: "12px", padding: 0 }}>
                  <div style={{ padding: "18px 24px 0 24px" }}>
                    <h2 style={{ margin: 0, fontSize: "1.1rem", color: "#1e2d3d" }}>Hronološka lista lokacija</h2>
                  </div>
                  <div style={{ overflowX: "auto", borderTop: "1px solid #e2e8f0" }}>
                    <table style={{ width: "100%", borderCollapse: "collapse" }}>
                      <thead>
                        <tr style={{ background: "#f8fafc", color: "#334155", textAlign: "left" }}>
                          <th style={{ padding: "12px 16px", borderBottom: "1px solid #e2e8f0", width: "70px" }}>#</th>
                          <th style={{ padding: "12px 16px", borderBottom: "1px solid #e2e8f0" }}>Adresa</th>
                          <th style={{ padding: "12px 16px", borderBottom: "1px solid #e2e8f0", width: "160px" }}>Prioritet</th>
                          <th style={{ padding: "12px 16px", borderBottom: "1px solid #e2e8f0", width: "180px" }}>Procijenjeno vrijeme</th>
                        </tr>
                      </thead>
                      <tbody>
                        {routeData.routeItems.map((item) => {
                          const colors = priorityColors(item.priority)
                          return (
                            <tr key={item.id} style={{ borderBottom: "1px solid #f1f5f9" }}>
                              <td style={{ padding: "12px 16px", fontWeight: 700, color: "#1e2d3d" }}>{item.order}</td>
                              <td style={{ padding: "12px 16px", color: "#334155" }}>{item.address}</td>
                              <td style={{ padding: "12px 16px" }}>
                                <span
                                  style={{
                                    display: "inline-block",
                                    padding: "3px 10px",
                                    borderRadius: "999px",
                                    fontSize: "0.78rem",
                                    fontWeight: 600,
                                    backgroundColor: colors.bg,
                                    color: colors.fg,
                                  }}
                                >
                                  {item.priority}
                                </span>
                              </td>
                              <td style={{ padding: "12px 16px", color: "#334155", fontWeight: 600 }}>{toHoursAndMinutes(item.estimatedArrivalTime)}</td>
                            </tr>
                          )
                        })}
                      </tbody>
                    </table>
                  </div>
                </div>
              </section>
            </>
          )}
        </div>
      </div>
    </Layout>
  )
}
