import { useState, useEffect, useCallback, useMemo } from "react"
import { MapContainer, Marker, TileLayer } from "react-leaflet"
import { useNavigate } from "react-router-dom"
import "leaflet/dist/leaflet.css"
import { toast } from "sonner"
import {
    getAllMailboxes,
    MailboxType,
    MailboxPriority,
    MailboxStatus,
    mailboxTypeLabels,
    mailboxPriorityLabels,
    mailboxStatusLabels,
    type MailboxResponse
} from "../../../infrastructure/api/mailboxes/mailboxesApi"
import { Layout } from "../../components/Layout/Layout"

const PAGE_SIZE = 25

export default function MailboxListPage() {
    const navigate = useNavigate()
    const [mailboxes, setMailboxes] = useState<MailboxResponse[]>([])
    const [loading, setLoading] = useState(true)
    const [typeFilter, setTypeFilter] = useState<MailboxType | "">("")
    const [priorityFilter, setPriorityFilter] = useState<MailboxPriority | "">("")
    const [addressSearch, setAddressSearch] = useState("")
    const [sortByPriority, setSortByPriority] = useState(false)
    const [page, setPage] = useState(1)
    const [totalCount, setTotalCount] = useState(0)
    const [totalPages, setTotalPages] = useState(0)
    const [modalMailbox, setModalMailbox] = useState<MailboxResponse | null>(null)

    const loadMailboxes = useCallback(async () => {
        try {
            setLoading(true)
            const result = await getAllMailboxes({
                page,
                pageSize: PAGE_SIZE,
                type: typeFilter === "" ? undefined : typeFilter,
                priority: priorityFilter === "" ? undefined : priorityFilter,
                search: addressSearch.trim() || undefined,
                sortByPriority
            })
            setMailboxes(result.items)
            setTotalCount(result.totalCount)
            setTotalPages(result.totalPages)
        } catch {
            toast.error("Greška pri učitavanju sandučića")
        } finally {
            setLoading(false)
        }
    }, [page, typeFilter, priorityFilter, addressSearch, sortByPriority])

    useEffect(() => { void loadMailboxes() }, [loadMailboxes])
    useEffect(() => { setPage(1) }, [typeFilter, priorityFilter, addressSearch, sortByPriority])

    const isEmptyDatabase = useMemo(
        () => totalCount === 0 && !typeFilter && !priorityFilter && !addressSearch.trim(),
        [totalCount, typeFilter, priorityFilter, addressSearch]
    )

    if (loading && mailboxes.length === 0) {
        return (
            <Layout>
                <div className="page-container">
                    <div className="form-card">
                        <p>Učitavanje...</p>
                    </div>
                </div>
            </Layout>
        )
    }

    return (
        <Layout>
            <div className="page-container mailbox-list-page">
                <div className="mailbox-list-page__shell">
                <div className="form-card mailbox-list-page__card">
                    <div className="form-card__header">
                        <div className="mailbox-list-page__header-actions">
                            <button
                                className="btn mailbox-list-page__history-button"
                                onClick={() => navigate("/admin/mailboxes/history")}
                            >
                                📋 Historija promjena
                            </button>
                            <button
                                className="btn btn--primary mailbox-list-page__add-button"
                                onClick={() => navigate("/admin/mailboxes/new")}
                            >
                                + Dodaj novi sandučić
                            </button>
                        </div>
                    </div>

                    <div className="form-card__body">
                        <div className="mailbox-list-page__title-row">
                            <h2 className="mailbox-list-page__title">
                                Lista sandučića — ukupno {totalCount}
                            </h2>
                        </div>

                        {/* Filteri */}
                        <div className="mailbox-list-page__filter-grid">
                            <div className="form-field">
                                <label className="form-field__label" htmlFor="filter-type">Tip</label>
                                <select
                                    id="filter-type"
                                    className="form-field__input"
                                    value={typeFilter}
                                    onChange={(e) => setTypeFilter(e.target.value === "" ? "" : Number(e.target.value) as MailboxType)}
                                >
                                    <option value="">Svi tipovi</option>
                                    <option value={MailboxType.WallSmall}>{mailboxTypeLabels[MailboxType.WallSmall]}</option>
                                    <option value={MailboxType.StandaloneLarge}>{mailboxTypeLabels[MailboxType.StandaloneLarge]}</option>
                                    <option value={MailboxType.IndoorResidential}>{mailboxTypeLabels[MailboxType.IndoorResidential]}</option>
                                    <option value={MailboxType.SpecialPriority}>{mailboxTypeLabels[MailboxType.SpecialPriority]}</option>
                                </select>
                            </div>
                            <div className="form-field">
                                <label className="form-field__label" htmlFor="filter-priority">Prioritet</label>
                                <select
                                    id="filter-priority"
                                    className="form-field__input"
                                    value={priorityFilter}
                                    onChange={(e) => setPriorityFilter(e.target.value === "" ? "" : Number(e.target.value) as MailboxPriority)}
                                >
                                    <option value="">Svi prioriteti</option>
                                    <option value={MailboxPriority.Visok}>{mailboxPriorityLabels[MailboxPriority.Visok]}</option>
                                    <option value={MailboxPriority.Srednji}>{mailboxPriorityLabels[MailboxPriority.Srednji]}</option>
                                    <option value={MailboxPriority.Nizak}>{mailboxPriorityLabels[MailboxPriority.Nizak]}</option>
                                </select>
                            </div>
                        </div>

                        {/* Pretraga po adresi */}
                        <div className="mailbox-list-page__search-row">
                            <div className="form-field mailbox-list-page__search-field">
                                <label className="form-field__label" htmlFor="filter-search">🔍 Pretraga po lokaciji</label>
                                <input
                                    id="filter-search"
                                    type="text"
                                    className="form-field__input"
                                    value={addressSearch}
                                    onChange={(e) => setAddressSearch(e.target.value)}
                                    placeholder="Unesite naselje, ulicu ili broj..."
                                    autoComplete="off"
                                />
                            </div>
                            <div className="mailbox-list-page__search-actions">
                                <button
                                    type="button"
                                    className="btn mailbox-list-page__sort-button"
                                    onClick={() => setSortByPriority(p => !p)}
                                    title="Sortiraj po prioritetu"
                                >
                                    {sortByPriority ? "▼ Po prioritetu" : "↕ Sortiranje"}
                                </button>
                            </div>
                        </div>

                        {/* Kartice */}
                        {mailboxes.length > 0 ? (
                            <div className="mailbox-list-page__cards-grid">
                                {mailboxes.map((mailbox) => (
                                    <div
                                        key={mailbox.id}
                                        className="mailbox-list-page__card-item"
                                        onMouseEnter={(e) => {
                                            const elem = e.currentTarget
                                            elem.style.boxShadow = "0 10px 22px rgba(15,37,64,0.12)"
                                            elem.style.transform = "translateY(-2px)"
                                        }}
                                        onMouseLeave={(e) => {
                                            const elem = e.currentTarget
                                            elem.style.boxShadow = "0 1px 3px rgba(15,37,64,0.1)"
                                            elem.style.transform = "translateY(0)"
                                        }}
                                    >
                                        {/* Zaglavlj kartice */}
                                        <div style={{ marginBottom: "12px" }}>
                                            <div style={{
                                                fontSize: "0.85rem",
                                                color: "#64748b",
                                                marginBottom: "4px"
                                            }}>
                                                SN: <strong>{mailbox.serialNumber}</strong>
                                            </div>
                                            <h3 style={{
                                                margin: 0,
                                                fontSize: "0.95rem",
                                                color: "#1e2d3d",
                                                fontWeight: 600,
                                                lineHeight: 1.3,
                                                display: "-webkit-box",
                                                WebkitLineClamp: 2,
                                                WebkitBoxOrient: "vertical",
                                                overflow: "hidden",
                                                textOverflow: "ellipsis"
                                            }}>
                                                {mailbox.address}
                                            </h3>
                                        </div>

                                        {/* Separatna linija */}
                                        <div style={{ borderBottom: "1px solid #f3f4f6", marginBottom: "12px" }} />

                                        {/* Informacije */}
                                        <div style={{ marginBottom: "12px", display: "flex", gap: "8px", flexWrap: "wrap" }}>
                                            <div style={{
                                                fontSize: "0.8rem",
                                                backgroundColor: "#f0f4f8",
                                                padding: "4px 10px",
                                                borderRadius: "6px",
                                                color: "#2563a8",
                                                fontWeight: 500
                                            }}>
                                                {mailboxTypeLabels[mailbox.type]}
                                            </div>
                                            <PriorityBadge priority={mailbox.priority} />
                                        </div>

                                        {/* Status i akcije */}
                                        <div style={{
                                            display: "flex",
                                            justifyContent: "space-between",
                                            alignItems: "center",
                                            gap: "8px"
                                        }}>
                                            <StatusBadge status={mailbox.status} />
                                            <div style={{ display: "flex", gap: "8px" }}>
                                                <button
                                                    className="btn btn--primary"
                                                    style={{
                                                        padding: "6px 12px",
                                                        fontSize: "0.8rem",
                                                        flex: 1,
                                                        minWidth: 0
                                                    }}
                                                    onClick={() => navigate(`/admin/mailboxes/${mailbox.id}/edit`)}
                                                    title="Uredi sandučić"
                                                >
                                                    Uredi
                                                </button>
                                                <button
                                                    className="btn"
                                                    style={{
                                                        padding: "6px 12px",
                                                        fontSize: "0.8rem"
                                                    }}
                                                    onClick={() => setModalMailbox(mailbox)}
                                                    title="Prikaži lokaciju na mapi"
                                                >
                                                    🗺️
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        ) : !loading && isEmptyDatabase ? (
                            <div style={{
                                textAlign: "center",
                                padding: "40px 20px",
                                backgroundColor: "#f8fafc",
                                border: "1px solid #e2e8f0",
                                borderRadius: "8px"
                            }}>
                                <p style={{ margin: 0, color: "#374151" }}>
                                    Baza sandučića je prazna. Kliknite na '+ Dodaj novi sandučić' za početak.
                                </p>
                            </div>
                        ) : !loading ? (
                            <div style={{ textAlign: "center", padding: "30px", color: "#6b7280" }}>
                                Nema rezultata za odabrane filtere.
                            </div>
                        ) : null}

                        {/* Straničenje */}
                        {totalPages > 1 && (
                            <div className="mailbox-list-page__pagination">
                                <button
                                    className="btn"
                                    onClick={() => setPage(p => Math.max(1, p - 1))}
                                    disabled={page <= 1}
                                    style={{ padding: "6px 14px" }}
                                >
                                    ← Prethodna
                                </button>
                                <span style={{ color: "#4b5563", fontSize: "0.9rem" }}>
                                    Strana {page} od {totalPages}
                                </span>
                                <button
                                    className="btn"
                                    onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                                    disabled={page >= totalPages}
                                    style={{ padding: "6px 14px" }}
                                >
                                    Sljedeća →
                                </button>
                            </div>
                        )}
                    </div>
                </div>
                </div>
            </div>

            {/* Modal mapa */}
            {modalMailbox && (
                <div
                    onClick={() => setModalMailbox(null)}
                    style={{
                        position: "fixed", inset: 0,
                        backgroundColor: "rgba(0,0,0,0.5)",
                        display: "flex", alignItems: "center", justifyContent: "center",
                        zIndex: 1000
                    }}
                >
                    <div
                        onClick={(e) => e.stopPropagation()}
                        style={{
                            backgroundColor: "#fff", borderRadius: "8px", padding: "20px",
                            width: "90%", maxWidth: "600px",
                            boxShadow: "0 10px 25px rgba(0,0,0,0.15)"
                        }}
                    >
                        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "12px" }}>
                            <div>
                                <h3 style={{ margin: 0, fontSize: "1.1rem", color: "#1e2d3d" }}>
                                    {modalMailbox.serialNumber}
                                </h3>
                                <p style={{ margin: "4px 0 0 0", color: "#4b5563", fontSize: "0.9rem" }}>
                                    {modalMailbox.address}
                                </p>
                            </div>
                            <button
                                onClick={() => setModalMailbox(null)}
                                style={{
                                    background: "transparent", border: "none",
                                    fontSize: "1.5rem", cursor: "pointer", color: "#64748b"
                                }}
                            >×</button>
                        </div>
                        <div style={{ height: "350px", borderRadius: "8px", overflow: "hidden", border: "1px solid #e2e8f0" }}>
                            <MapContainer
                                center={[Number(modalMailbox.latitude), Number(modalMailbox.longitude)]}
                                zoom={16}
                                style={{ height: "100%", width: "100%" }}
                            >
                                <TileLayer
                                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
                                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                                />
                                <Marker position={[Number(modalMailbox.latitude), Number(modalMailbox.longitude)]} />
                            </MapContainer>
                        </div>
                    </div>
                </div>
            )}
        </Layout>
    )
}

function PriorityBadge({ priority }: { priority: MailboxPriority }) {
    const colors: Record<MailboxPriority, { bg: string; fg: string }> = {
        [MailboxPriority.Visok]: { bg: "#fee2e2", fg: "#b91c1c" },
        [MailboxPriority.Srednji]: { bg: "#fef3c7", fg: "#92400e" },
        [MailboxPriority.Nizak]: { bg: "#dcfce7", fg: "#166534" }
    }
    const c = colors[priority]
    return (
        <span style={{
            display: "inline-block", padding: "2px 10px",
            borderRadius: "12px", backgroundColor: c.bg,
            color: c.fg, fontSize: "0.8rem", fontWeight: 500
        }}>
            {mailboxPriorityLabels[priority]}
        </span>
    )
}

function StatusBadge({ status }: { status: MailboxStatus }) {
    const c = status === MailboxStatus.Pun
        ? { bg: "#fee2e2", fg: "#b91c1c" }
        : { bg: "#dcfce7", fg: "#166534" }
    return (
        <span style={{
            display: "inline-block", padding: "2px 10px",
            borderRadius: "12px", backgroundColor: c.bg,
            color: c.fg, fontSize: "0.8rem", fontWeight: 500
        }}>
            {mailboxStatusLabels[status]}
        </span>
    )
}