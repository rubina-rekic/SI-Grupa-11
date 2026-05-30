import { useState, useEffect, useCallback } from "react"
import { useNavigate } from "react-router-dom"
import { toast } from "sonner"
import { routesApi } from "../../../infrastructure/api/routesApi"
import type { RouteResponse } from "../../../infrastructure/api/routesApi"
import { getUsers, type UserListDto } from "../../../infrastructure/api/users/usersApi"
import { Layout } from "../../components/Layout/Layout"

const PAGE_SIZE = 20

export default function ArchiveRouteListPage() {
    const navigate = useNavigate()
    const [routes, setRoutes] = useState<RouteResponse[]>([])
    const [postmen, setPostmen] = useState<UserListDto[]>([])
    const [loading, setLoading] = useState(true)
    const [page, setPage] = useState(1)
    const [totalCount, setTotalCount] = useState(0)
    const [totalPages, setTotalPages] = useState(0)
    const [fromDate, setFromDate] = useState("")
    const [toDate, setToDate] = useState("")
    const [postmanId, setPostmanId] = useState("")

    const loadRoutes = useCallback(async () => {
        try {
            setLoading(true)
            const res = await routesApi.getArchiveRoutes(
                page,
                PAGE_SIZE,
                fromDate || undefined,
                toDate || undefined,
                postmanId || undefined
            )
            setRoutes(res.items)
            setTotalCount(res.totalCount)
            setTotalPages(res.totalPages)
        } catch {
            toast.error("Greska pri ucitavanju arhive")
        } finally {
            setLoading(false)
        }
    }, [page, fromDate, toDate, postmanId])

    useEffect(() => { void loadRoutes() }, [loadRoutes])
    useEffect(() => { setPage(1) }, [fromDate, toDate, postmanId])

    useEffect(() => {
        let isMounted = true

        async function loadPostmen() {
            const response = await getUsers()
            if (!isMounted) return

            if (response.error || !response.data) {
                toast.error("Greska pri ucitavanju postara")
                return
            }

            setPostmen(response.data.filter(user => user.role === "PostalWorker" && !user.isLockedOut))
        }

        void loadPostmen()

        return () => {
            isMounted = false
        }
    }, [])

    const resetFilters = () => {
        setFromDate("")
        setToDate("")
        setPostmanId("")
        setPage(1)
    }

    return (
        <Layout>
            <div className="page-container">
                <div className="form-card" style={{ maxWidth: '1100px', margin: '0 auto' }}>
                    <div className="form-card__header">
                        <h2 className="form-card__title">Arhiva ruta</h2>
                        <p className="form-card__subtitle">
                            Ukupno arhiviranih ruta: {totalCount}
                        </p>
                    </div>
                    <div className="form-card__body">
                        <div style={{ display: 'flex', gap: '10px', marginBottom: '20px', alignItems: 'flex-end', flexWrap: 'wrap' }}>
                            <div className="form-field">
                                <label className="form-field__label" htmlFor="archive-from-date">Od datuma</label>
                                <input id="archive-from-date" type="date" className="form-field__input" value={fromDate} onChange={e => setFromDate(e.target.value)} />
                            </div>
                            <div className="form-field">
                                <label className="form-field__label" htmlFor="archive-to-date">Do datuma</label>
                                <input id="archive-to-date" type="date" className="form-field__input" value={toDate} onChange={e => setToDate(e.target.value)} />
                            </div>
                            <div className="form-field">
                                <label className="form-field__label" htmlFor="archive-postman">Postar</label>
                                <select id="archive-postman" className="form-field__input" value={postmanId} onChange={e => setPostmanId(e.target.value)}>
                                    <option value="">Svi postari</option>
                                    {postmen.map(postman => (
                                        <option key={postman.id} value={postman.id}>{postman.username}</option>
                                    ))}
                                </select>
                            </div>
                            <button type="button" className="btn btn--outline" onClick={resetFilters}>Ponisti</button>
                        </div>

                        {loading && routes.length === 0 ? <p>Ucitavanje...</p> : (
                            <table className="table" style={{ width: '100%', textAlign: 'left', borderCollapse: 'collapse' }}>
                                <thead>
                                    <tr>
                                        <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>Datum</th>
                                        <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>Postar</th>
                                        <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>Status</th>
                                        <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>Planirano tacaka</th>
                                        <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>Udaljenost / Trajanje</th>
                                        <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}>Vrijeme</th>
                                        <th style={{ padding: '8px', borderBottom: '1px solid #ccc' }}></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {routes.map(r => (
                                        <tr key={r.id}>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{r.date}</td>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{r.postmanName || r.postmanId}</td>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{r.status}</td>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{r.routeItems.length}</td>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{r.totalDistanceKm} km / {r.totalDurationMinutes} min</td>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{r.plannedStartTime} - {r.plannedEndTime ?? '-'}</td>
                                            <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>
                                                <button className="btn btn--primary" onClick={() => navigate(`/admin/routes/archive/${r.id}`)}>Detalji</button>
                                            </td>
                                        </tr>
                                    ))}
                                    {routes.length === 0 && (
                                        <tr><td colSpan={7} style={{ padding: '16px', textAlign: 'center' }}>Nema pronadjenih ruta u arhivi.</td></tr>
                                    )}
                                </tbody>
                            </table>
                        )}

                        <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: '20px', alignItems: 'center' }}>
                            <button className="btn btn--outline" disabled={page === 1} onClick={() => setPage(p => p - 1)}>Prethodna</button>
                            <span>Strana {page} od {Math.max(totalPages, 1)}</span>
                            <button className="btn btn--outline" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}>Sljedeca</button>
                        </div>
                    </div>
                </div>
            </div>
        </Layout>
    )
}
