import { useEffect, useMemo, useRef, useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { toast } from "sonner"
import { MapContainer as LeafletMap, Marker, Popup, TileLayer, Circle } from "react-leaflet"
import L from "leaflet"
import "leaflet/dist/leaflet.css"
import { Layout } from "../../components/Layout/Layout"
import { LeafletRoutingMachine } from "../../components/common/LeafletRoutingMachine"
import { routesApi } from "../../../infrastructure/api/routesApi"
import type { AvailablePostmanResponse, RouteItemResponse, RouteResponse } from "../../../infrastructure/api/routesApi"
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

function formatDateTime(value: string | null | undefined) {
  if (!value) {
    return "Nije evidentirano"
  }

  return new Intl.DateTimeFormat("bs-BA", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value))
}

function routeStatusLabel(status: string) {
  if (status === "Planirana") return "Prijedlog"
  if (status === "Dodijeljena") return "Dodijeljena"
  if (status === "UProgresu") return "U toku"
  if (status === "Zavrsena") return "Završena"
  if (status === "Otkazana") return "Otkazana"
  return status
}

function routeStatusPillClass(status: string) {
  if (status === "Dodijeljena") return "pill pill--blue"
  if (status === "UProgresu") return "pill pill--amber"
  if (status === "Zavrsena") return "pill pill--green"
  if (status === "Otkazana") return "pill pill--red"
  return "pill pill--purple"
}

function priorityColors(priority: string) {
  if (priority === "Visok") return { bg: "#fee2e2", fg: "#b91c1c" }
  if (priority === "Srednji") return { bg: "#fef3c7", fg: "#92400e" }
  return { bg: "#dcfce7", fg: "#166534" }
}

const DEPOT_LAT = 43.8563
const DEPOT_LNG = 18.4131
const SPEED_KMH = 30
const STOP_MINUTES = 5

function euclideanDistance(lat1: number, lng1: number, lat2: number, lng2: number) {
  return Math.sqrt((lat2 - lat1) ** 2 + (lng2 - lng1) ** 2)
}

function recalculateArrivals(items: RouteItemResponse[], startTime: string): RouteItemResponse[] {
  const [startH, startM] = startTime.split(":").map(Number)
  let totalMinutes = startH * 60 + startM
  let currentLat = DEPOT_LAT
  let currentLng = DEPOT_LNG

  return items.map((item) => {
    const dist = euclideanDistance(currentLat, currentLng, item.latitude, item.longitude)
    const travelMin = Math.round((dist * 111) / SPEED_KMH * 60)
    totalMinutes += travelMin
    const h = Math.floor(totalMinutes / 60) % 24
    const m = totalMinutes % 60
    const estimatedArrivalTime = `${String(h).padStart(2, "0")}:${String(m).padStart(2, "0")}:00`
    currentLat = item.latitude
    currentLng = item.longitude
    totalMinutes += STOP_MINUTES
    return { ...item, estimatedArrivalTime }
  })
}

export default function GenerateRoutePage() {
  const [postmen, setPostmen] = useState<UserListDto[]>([])
  const [routeData, setRouteData] = useState<RouteResponse | null>(null)
  const [loading, setLoading] = useState(false)
  const [fetchingUsers, setFetchingUsers] = useState(true)
  const [selectedItemId, setSelectedItemId] = useState<string | null>(null)
  const [localItems, setLocalItems] = useState<RouteItemResponse[]>([])
  const [saving, setSaving] = useState(false)
  const [hasLocalChanges, setHasLocalChanges] = useState(false)
  const [assignmentOpen, setAssignmentOpen] = useState(false)
  const [availablePostmen, setAvailablePostmen] = useState<AvailablePostmanResponse[]>([])
  const [selectedAssigneeId, setSelectedAssigneeId] = useState("")
  const [loadingPostmenAvailability, setLoadingPostmenAvailability] = useState(false)
  const [assigningRoute, setAssigningRoute] = useState(false)
  const originalItemsRef = useRef<RouteItemResponse[]>([])

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
          setPostmen(res.data.filter((u) => u.role === "PostalWorker" && !u.isLockedOut))
        }
      } catch {
        toast.error("Greška pri učitavanju poštara")
      } finally {
        setFetchingUsers(false)
      }
    }

    void loadUsers()
  }, [])

  useEffect(() => {
    if (!routeData) {
      setAvailablePostmen([])
      setSelectedAssigneeId("")
      return
    }

    async function loadAvailability() {
      try {
        setLoadingPostmenAvailability(true)
        const postmenAvailability = await routesApi.getAvailablePostmen(routeData!.id)
        setAvailablePostmen(postmenAvailability)

        const currentAssignee = postmenAvailability.find((postman) => postman.isCurrentAssignee)
        const firstAvailable = postmenAvailability.find((postman) => postman.isAvailable)
        setSelectedAssigneeId(currentAssignee?.id ?? firstAvailable?.id ?? "")
      } catch {
        toast.error("Greška pri učitavanju dostupnih poštara.")
      } finally {
        setLoadingPostmenAvailability(false)
      }
    }

    void loadAvailability()
  }, [routeData])

  const onSubmit = async (data: GenerateRouteFormValues) => {
    try {
      setLoading(true)
      setRouteData(null)
      const result = await routesApi.generateRoute({
        postmanId: data.postmanId,
        date: data.date,
        plannedStartTime: `${data.plannedStartTime}:00`,
      })

      const sortedItems = [...result.routeItems].sort((a, b) => a.order - b.order)
      setRouteData(result)
      setLocalItems(sortedItems)
      setHasLocalChanges(false)
      setAssignmentOpen(false)
      originalItemsRef.current = sortedItems

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
        toast.error("Nema dostupnih lokacija za generisanje rute. Provjerite status, dostupnost i pravila prioriteta sanducica.")
      }
    } finally {
      setLoading(false)
    }
  }

  const mapCenter = useMemo<[number, number]>(() => {
    if (localItems.length === 0) return [43.8563, 18.4131]
    return [localItems[0].latitude, localItems[0].longitude]
  }, [localItems])

  const routeWaypoints = useMemo<Array<[number, number]>>(() => {
    if (localItems.length === 0) return []
    return localItems.map((item) => [item.latitude, item.longitude])
  }, [localItems])

  const isRouteEditable = routeData
    ? routeData.status !== "UProgresu" && routeData.status !== "Zavrsena"
    : false

  const canAssignRoute = routeData
    ? routeData.status === "Planirana" || routeData.status === "Dodijeljena"
    : false

  const availableAssignees = useMemo(
    () => availablePostmen.filter((postman) => postman.isAvailable),
    [availablePostmen]
  )

  async function assignRoute() {
    if (!routeData || !selectedAssigneeId) return

    setAssigningRoute(true)
    try {
      const result = await routesApi.assignRoute(routeData.id, selectedAssigneeId)
      const sortedItems = [...result.routeItems].sort((a, b) => a.order - b.order)
      const selectedPostman = availablePostmen.find((postman) => postman.id === selectedAssigneeId)

      setRouteData(result)
      setLocalItems(sortedItems)
      originalItemsRef.current = sortedItems
      setHasLocalChanges(false)
      setAssignmentOpen(false)
      toast.success(`Ruta je uspješno dodijeljena poštaru ${selectedPostman?.fullName ?? result.postmanName ?? ""}.`)
    } catch (err: unknown) {
      if (err && typeof err === "object" && "error" in err && typeof (err as Record<string, unknown>).error === "string") {
        toast.error((err as Record<string, unknown>).error as string)
      } else {
        toast.error("Greška pri dodjeli rute poštaru.")
      }
    } finally {
      setAssigningRoute(false)
    }
  }

  function moveItem(index: number, direction: -1 | 1) {
    const newItems = [...localItems]
    const swapWith = index + direction
    ;[newItems[index], newItems[swapWith]] = [newItems[swapWith], newItems[index]]

    const reindexed = newItems.map((item, i) => ({ ...item, order: i + 1 }))
    const withArrivals = recalculateArrivals(reindexed, routeData!.plannedStartTime)

    const origIds = originalItemsRef.current.map((i) => i.id)
    const updated = withArrivals.map((item) => ({
      ...item,
      isManuallyReordered: origIds[item.order - 1] !== item.id
    }))

    setLocalItems(updated)
    setHasLocalChanges(true)
  }

  function resetToOriginal() {
    const reset = originalItemsRef.current.map((item, i) => ({
      ...item,
      order: i + 1,
      isManuallyReordered: false
    }))
    setLocalItems(recalculateArrivals(reset, routeData!.plannedStartTime))
    setHasLocalChanges(false)
  }

  async function saveReorder() {
    if (!routeData) return
    setSaving(true)
    try {
      const result = await routesApi.reorderRoute(
        routeData.id,
        localItems.map((item) => ({ routeItemId: item.id, newOrder: item.order }))
      )
      const sortedItems = [...result.routeItems].sort((a, b) => a.order - b.order)
      setRouteData(result)
      setLocalItems(sortedItems)
      setHasLocalChanges(false)
      originalItemsRef.current = sortedItems
      toast.success("Izmjene redoslijeda su uspješno sačuvane.")
    } catch {
      toast.error("Greška pri čuvanju izmjena redoslijeda.")
    } finally {
      setSaving(false)
    }
  }

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

            <form className="form-card__body" onSubmit={(e) => { void handleSubmit(onSubmit)(e) }} noValidate>
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
                      <div style={{ color: "#64748b", fontSize: "0.8rem" }}>Status rute</div>
                      <div style={{ marginTop: "4px" }}>
                        <span className={routeStatusPillClass(routeData.status)}>{routeStatusLabel(routeData.status)}</span>
                      </div>
                    </div>
                    <div style={{ background: "#f8fafc", border: "1px solid #e2e8f0", borderRadius: "8px", padding: "12px" }}>
                      <div style={{ color: "#64748b", fontSize: "0.8rem" }}>Vremenski raspon</div>
                      <div style={{ color: "#1e2d3d", fontSize: "1.15rem", fontWeight: 700 }}>
                        {toHoursAndMinutes(routeData.plannedStartTime)} - {toHoursAndMinutes(routeData.plannedEndTime)}
                      </div>
                    </div>
                    <div style={{ background: "#f8fafc", border: "1px solid #e2e8f0", borderRadius: "8px", padding: "12px" }}>
                      <div style={{ color: "#64748b", fontSize: "0.8rem" }}>Dodijeljeni poštar</div>
                      <div style={{ color: "#1e2d3d", fontSize: "1rem", fontWeight: 700 }}>
                        {routeData.status === "Dodijeljena" ? routeData.postmanName ?? "Evidentiran poštar" : "Nije dodijeljeno"}
                      </div>
                      {routeData.assignedAt && (
                        <div style={{ color: "#64748b", fontSize: "0.78rem", marginTop: "2px" }}>
                          {formatDateTime(routeData.assignedAt)} · {routeData.assignedBy ?? "Nepoznat dispečer"}
                        </div>
                      )}
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
                <div className="form-card__body" style={{ gap: "14px" }}>
                  <div
                    style={{
                      display: "flex",
                      alignItems: "flex-start",
                      justifyContent: "space-between",
                      gap: "12px",
                      flexWrap: "wrap",
                    }}
                  >
                    <div>
                      <h2 style={{ margin: 0, fontSize: "1.1rem", color: "#1e2d3d" }}>Dodjela rute</h2>
                      <p style={{ margin: "4px 0 0", color: "#64748b", fontSize: "0.88rem" }}>
                        {routeData.status === "Dodijeljena"
                          ? "Ruta je dodijeljena i može se preraspodijeliti dok obilazak nije počeo."
                          : "Prijedlog rute pretvorite u operativni nalog izborom dostupnog poštara."}
                      </p>
                    </div>
                    {canAssignRoute ? (
                      <button
                        type="button"
                        className="btn btn--primary route-assignment__toggle"
                        onClick={() => setAssignmentOpen((open) => !open)}
                      >
                        {routeData.status === "Dodijeljena" ? "Promijeni poštara" : "Dodijeli poštaru"}
                      </button>
                    ) : (
                      <span className="pill pill--amber">Preraspodjela nije dostupna</span>
                    )}
                  </div>

                  {!canAssignRoute && (
                    <div className="route-assignment__notice">
                      Dodjela rute je dostupna samo za prijedloge ili već dodijeljene rute.
                    </div>
                  )}

                  {canAssignRoute && assignmentOpen && (
                    <div className="route-assignment">
                      {loadingPostmenAvailability ? (
                        <div className="route-assignment__notice">Učitavanje dostupnih poštara...</div>
                      ) : availableAssignees.length === 0 ? (
                        <div className="route-assignment__notice">Nema dostupnih poštara za odabrani datum.</div>
                      ) : (
                        <>
                          <div className="form-field">
                            <label htmlFor="assigneeId" className="form-field__label">Poštar *</label>
                            <select
                              id="assigneeId"
                              className="form-field__input"
                              value={selectedAssigneeId}
                              onChange={(event) => setSelectedAssigneeId(event.target.value)}
                            >
                              {availablePostmen.map((postman) => (
                                <option
                                  key={postman.id}
                                  value={postman.id}
                                  disabled={!postman.isAvailable}
                                  title={postman.unavailableReason ?? undefined}
                                >
                                  {postman.fullName} ({postman.email})
                                  {!postman.isAvailable ? " - zauzet" : ""}
                                  {postman.isCurrentAssignee ? " - trenutno dodijeljen" : ""}
                                </option>
                              ))}
                            </select>
                          </div>

                          {availablePostmen.some((postman) => !postman.isAvailable) && (
                            <div className="route-assignment__hint">
                              Poštari koji već imaju dodijeljenu rutu za ovaj datum ostaju vidljivi, ali su onemogućeni u izboru.
                            </div>
                          )}

                          <div className="route-assignment__actions">
                            <button
                              type="button"
                              className="btn-secondary"
                              onClick={() => setAssignmentOpen(false)}
                              disabled={assigningRoute}
                            >
                              Otkaži
                            </button>
                            <button
                              type="button"
                              className="btn btn--primary route-assignment__confirm"
                              onClick={() => { void assignRoute() }}
                              disabled={!selectedAssigneeId || assigningRoute}
                            >
                              {assigningRoute ? "Dodjela u toku..." : "Potvrdi dodjelu"}
                            </button>
                          </div>
                        </>
                      )}
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
                      {localItems.map((item) => (
                        <Marker
                          key={item.id}
                          position={[item.latitude, item.longitude]}
                          eventHandlers={{
                            click: () => setSelectedItemId(item.id),
                          }}
                        >
                          <Popup>
                            <strong>{item.order}.</strong> {item.address}
                            <br />
                            Prioritet: {item.priority}
                            <br />
                            Planirani dolazak: {item.estimatedArrivalTime}
                          </Popup>
                        </Marker>
                      ))}
                      {selectedItemId && localItems.find((item) => item.id === selectedItemId) && (
                        <Circle
                          center={[
                            localItems.find((item) => item.id === selectedItemId)!.latitude,
                            localItems.find((item) => item.id === selectedItemId)!.longitude,
                          ]}
                          radius={100}
                          pathOptions={{ color: "#2563a8", opacity: 0.6, fillOpacity: 0.1 }}
                        />
                      )}
                    </LeafletMap>
                  </div>
                </div>
              </section>

              <section className="form-card" style={{ maxWidth: "unset" }}>
                <div className="form-card__body" style={{ gap: "12px", padding: 0 }}>
                  <div style={{ padding: "18px 24px 0 24px", display: "flex", alignItems: "center", justifyContent: "space-between" }}>
                    <h2 style={{ margin: 0, fontSize: "1.1rem", color: "#1e2d3d" }}>Hronološka lista lokacija</h2>
                    {!isRouteEditable && (
                      <span style={{ fontSize: "0.82rem", color: "#b45309", fontWeight: 600 }}>
                        Izmjena redoslijeda nije dostupna za rute u toku ili završene rute.
                      </span>
                    )}
                  </div>
                  <div style={{ overflowX: "auto", borderTop: "1px solid #e2e8f0" }}>
                    <table style={{ width: "100%", borderCollapse: "collapse" }}>
                      <thead>
                        <tr style={{ background: "#f8fafc", color: "#334155", textAlign: "left" }}>
                          <th style={{ padding: "12px 16px", borderBottom: "1px solid #e2e8f0", width: "80px" }}>#</th>
                          <th style={{ padding: "12px 16px", borderBottom: "1px solid #e2e8f0" }}>Adresa</th>
                          <th style={{ padding: "12px 16px", borderBottom: "1px solid #e2e8f0", width: "160px" }}>Prioritet</th>
                          <th style={{ padding: "12px 16px", borderBottom: "1px solid #e2e8f0", width: "180px" }}>Procijenjeno vrijeme</th>
                          {isRouteEditable && (
                            <th style={{ padding: "12px 16px", borderBottom: "1px solid #e2e8f0", width: "100px", textAlign: "center" }}>Redoslijed</th>
                          )}
                        </tr>
                      </thead>
                      <tbody>
                        {localItems.map((item, index) => {
                          const colors = priorityColors(item.priority)
                          const isSelected = selectedItemId === item.id
                          return (
                            <tr
                              key={item.id}
                              onClick={() => setSelectedItemId(item.id)}
                              style={{
                                borderBottom: "1px solid #f1f5f9",
                                backgroundColor: isSelected ? "#eff6ff" : "transparent",
                                cursor: "pointer",
                                transition: "background-color 0.2s",
                              }}
                            >
                              <td style={{ padding: "12px 16px", fontWeight: 700, color: "#1e2d3d" }}>
                                {item.order}
                                {item.isManuallyReordered && (
                                  <span title="Ručno premješteno" style={{ marginLeft: "6px", color: "#d97706", fontSize: "0.85rem" }}>✎</span>
                                )}
                              </td>
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
                              {isRouteEditable && (
                                <td style={{ padding: "8px 16px", textAlign: "center" }} onClick={(e) => e.stopPropagation()}>
                                  <div style={{ display: "flex", gap: "4px", justifyContent: "center" }}>
                                    <button
                                      disabled={index === 0}
                                      onClick={() => moveItem(index, -1)}
                                      title="Pomjeri gore"
                                      style={{
                                        width: "28px", height: "28px", border: "1px solid #cbd5e1",
                                        borderRadius: "4px", background: index === 0 ? "#f1f5f9" : "#fff",
                                        cursor: index === 0 ? "not-allowed" : "pointer",
                                        color: index === 0 ? "#94a3b8" : "#334155",
                                        fontWeight: 700, fontSize: "0.9rem", lineHeight: 1,
                                      }}
                                    >↑</button>
                                    <button
                                      disabled={index === localItems.length - 1}
                                      onClick={() => moveItem(index, 1)}
                                      title="Pomjeri dolje"
                                      style={{
                                        width: "28px", height: "28px", border: "1px solid #cbd5e1",
                                        borderRadius: "4px", background: index === localItems.length - 1 ? "#f1f5f9" : "#fff",
                                        cursor: index === localItems.length - 1 ? "not-allowed" : "pointer",
                                        color: index === localItems.length - 1 ? "#94a3b8" : "#334155",
                                        fontWeight: 700, fontSize: "0.9rem", lineHeight: 1,
                                      }}
                                    >↓</button>
                                  </div>
                                </td>
                              )}
                            </tr>
                          )
                        })}
                      </tbody>
                    </table>
                  </div>
                  {isRouteEditable && (
                    <div style={{ padding: "16px 24px", display: "flex", gap: "12px", borderTop: "1px solid #e2e8f0" }}>
                      <button
                        onClick={resetToOriginal}
                        disabled={!hasLocalChanges}
                        style={{
                          padding: "8px 16px", borderRadius: "6px", border: "1px solid #cbd5e1",
                          background: hasLocalChanges ? "#fff" : "#f8fafc",
                          color: hasLocalChanges ? "#334155" : "#94a3b8",
                          cursor: hasLocalChanges ? "pointer" : "not-allowed",
                          fontWeight: 600, fontSize: "0.88rem",
                        }}
                      >
                        Resetuj na originalni redoslijed
                      </button>
                      <button
                        onClick={saveReorder}
                        disabled={!hasLocalChanges || saving}
                        style={{
                          padding: "8px 16px", borderRadius: "6px", border: "none",
                          background: hasLocalChanges && !saving ? "#2563eb" : "#93c5fd",
                          color: "#fff",
                          cursor: hasLocalChanges && !saving ? "pointer" : "not-allowed",
                          fontWeight: 600, fontSize: "0.88rem",
                        }}
                      >
                        {saving ? "Čuvanje..." : "Sačuvaj izmjene"}
                      </button>
                    </div>
                  )}
                </div>
              </section>
            </>
          )}
        </div>
      </div>
    </Layout>
  )
}
