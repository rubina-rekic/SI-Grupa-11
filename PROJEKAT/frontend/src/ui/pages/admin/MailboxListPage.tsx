import { useState, useEffect, useCallback, useMemo } from "react"
import { MapContainer, Marker, TileLayer } from "react-leaflet"
import "leaflet/dist/leaflet.css"
import { toast } from "sonner"
import {
    createMailbox,
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
import OpenStreetMapPicker from "../../components/common/OpenStreetMapPicker"

const PAGE_SIZE = 25

export default function MailboxListPage() {
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

    const [showForm, setShowForm] = useState(false)
    const [showMap, setShowMap] = useState(false)
    const [selectedLocation, setSelectedLocation] = useState<{ lat: number; lng: number } | null>(null)
    const [formData, setFormData] = useState({
        serialNumber: "",
        address: "",
        latitude: "43.8563",
        longitude: "18.4131",
        type: MailboxType.WallSmall as MailboxType,
        capacity: "100",
        installationYear: new Date().getFullYear().toString(),
        notes: ""
    })
    const [submitting, setSubmitting] = useState(false)

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

    useEffect(() => {
        void loadMailboxes()
    }, [loadMailboxes])

    useEffect(() => {
        setPage(1)
    }, [typeFilter, priorityFilter, addressSearch, sortByPriority])

    const handleLocationSelect = useCallback((lat: number, lng: number) => {
        setSelectedLocation({ lat, lng })
        setFormData(prev => ({ ...prev, latitude: lat.toString(), longitude: lng.toString() }))
    }, [])

    const togglePrioritySort = () => setSortByPriority(prev => !prev)

    const isEmptyDatabase = useMemo(
        () => totalCount === 0 && !typeFilter && !priorityFilter && !addressSearch.trim(),
        [totalCount, typeFilter, priorityFilter, addressSearch]
    )

    if (loading && mailboxes.length === 0) {
        return (
            <Layout>
                <div className="page-container">
                    <h1>Pregled sandučića</h1>
                    <div>Učitavanje...</div>
                </div>
            </Layout>
        )
    }

    return (
        <Layout>
            <div className="page-container">
                <div className="form-card">
                    <div className="form-card__header">
                        <h1 className="form-card__title">Pregled sandučića</h1>
                        <p className="form-card__subtitle">
                            Upravljajte sandučićima i dodajte nove lokacije.
                        </p>
                    </div>

                    <div className="form-card__body">
                        <div style={{ marginBottom: "20px" }}>
                            <h2 style={{ margin: 0, fontSize: "1.1rem", color: "#1e2d3d" }}>
                                Lista sandučića ({totalCount})
                            </h2>
                        </div>

                        <div style={{
                            display: "grid",
                            gridTemplateColumns: "1fr 1fr 2fr",
                            gap: "12px",
                            marginBottom: "16px"
                        }}>
                            <div>
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
                            <div>
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
                            <div>
                                <label className="form-field__label" htmlFor="filter-search">Pretraga po adresi (Naselje/Ulica)</label>
                                <input
                                    id="filter-search"
                                    type="text"
                                    className="form-field__input"
                                    value={addressSearch}
                                    onChange={(e) => setAddressSearch(e.target.value)}
                                    placeholder="Unesite naselje ili ulicu..."
                                />
                            </div>
                        </div>

                        {showForm && (
                            <InlineCreateForm
                                formData={formData}
                                setFormData={setFormData}
                                showMap={showMap}
                                setShowMap={setShowMap}
                                selectedLocation={selectedLocation}
                                setSelectedLocation={setSelectedLocation}
                                submitting={submitting}
                                setSubmitting={setSubmitting}
                                handleLocationSelect={handleLocationSelect}
                                onSuccess={async () => {
                                    setShowForm(false)
                                    await loadMailboxes()
                                }}
                                onCancel={() => setShowForm(false)}
                            />
                        )}

                        {!showForm && (
                            <button
                                className="btn btn--primary"
                                style={{ maxWidth: "200px", marginBottom: "16px" }}
                                onClick={() => setShowForm(true)}
                            >
                                + Dodaj novi sandučić
                            </button>
                        )}

                        {mailboxes.length > 0 && (
                            <div style={{ overflow: "auto", border: "1px solid #e2e8f0", borderRadius: "8px" }}>
                                <table style={{ width: "100%", borderCollapse: "collapse", fontSize: "0.9rem" }}>
                                    <thead>
                                        <tr style={{ backgroundColor: "#f8fafc", borderBottom: "1px solid #e2e8f0" }}>
                                            <th style={thStyle}>Serijski broj</th>
                                            <th style={thStyle}>Adresa</th>
                                            <th style={thStyle}>Tip</th>
                                            <th
                                                style={{ ...thStyle, cursor: "pointer", userSelect: "none" }}
                                                onClick={togglePrioritySort}
                                                title="Kliknite za sortiranje po prioritetu"
                                            >
                                                Prioritet {sortByPriority ? "▲" : ""}
                                            </th>
                                            <th style={{ ...thStyle, textAlign: "center" }}>Status</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {mailboxes.map((mailbox, index) => (
                                            <tr
                                                key={mailbox.id}
                                                style={{
                                                    backgroundColor: index % 2 === 0 ? "#ffffff" : "#f9fafb",
                                                    borderBottom: "1px solid #f3f4f6"
                                                }}
                                            >
                                                <td style={tdStyle}>{mailbox.serialNumber}</td>
                                                <td
                                                    style={{ ...tdStyle, color: "#2563a8", cursor: "pointer", textDecoration: "underline" }}
                                                    onClick={() => setModalMailbox(mailbox)}
                                                    title="Kliknite za prikaz lokacije na mapi"
                                                >
                                                    {mailbox.address}
                                                </td>
                                                <td style={tdStyle}>{mailboxTypeLabels[mailbox.type]}</td>
                                                <td style={tdStyle}>
                                                    <PriorityBadge priority={mailbox.priority} />
                                                </td>
                                                <td style={{ ...tdStyle, textAlign: "center" }}>
                                                    <StatusBadge status={mailbox.status} />
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}

                        {totalPages > 1 && (
                            <div style={{
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                gap: "12px",
                                marginTop: "16px"
                            }}>
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

                        {mailboxes.length === 0 && !loading && isEmptyDatabase && !showForm && (
                            <div style={{
                                textAlign: "center",
                                padding: "40px 20px",
                                backgroundColor: "#f8fafc",
                                border: "1px solid #e2e8f0",
                                borderRadius: "8px"
                            }}>
                                <p style={{ margin: 0, color: "#374151", fontSize: "0.95rem" }}>
                                    Baza sandučića je prazna. Kliknite na 'Dodaj novi' za početak.
                                </p>
                            </div>
                        )}

                        {mailboxes.length === 0 && !loading && !isEmptyDatabase && (
                            <div style={{
                                textAlign: "center",
                                padding: "30px 20px",
                                color: "#6b7280",
                                fontSize: "0.9rem"
                            }}>
                                Nema rezultata za odabrane filtere.
                            </div>
                        )}
                    </div>
                </div>

                {modalMailbox && (
                    <MapModal mailbox={modalMailbox} onClose={() => setModalMailbox(null)} />
                )}
            </div>
        </Layout>
    )
}

const thStyle: React.CSSProperties = {
    padding: "12px",
    textAlign: "left",
    fontWeight: 600,
    color: "#374151",
    borderBottom: "1px solid #e2e8f0"
}

const tdStyle: React.CSSProperties = {
    padding: "12px",
    color: "#4b5563"
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
            display: "inline-block",
            padding: "2px 10px",
            borderRadius: "12px",
            backgroundColor: c.bg,
            color: c.fg,
            fontSize: "0.8rem",
            fontWeight: 500
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
            display: "inline-block",
            padding: "2px 10px",
            borderRadius: "12px",
            backgroundColor: c.bg,
            color: c.fg,
            fontSize: "0.8rem",
            fontWeight: 500
        }}>
            {mailboxStatusLabels[status]}
        </span>
    )
}

function MapModal({ mailbox, onClose }: { mailbox: MailboxResponse; onClose: () => void }) {
    return (
        <div
            onClick={onClose}
            style={{
                position: "fixed",
                inset: 0,
                backgroundColor: "rgba(0,0,0,0.5)",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                zIndex: 1000
            }}
        >
            <div
                onClick={(e) => e.stopPropagation()}
                style={{
                    backgroundColor: "#fff",
                    borderRadius: "8px",
                    padding: "20px",
                    width: "90%",
                    maxWidth: "600px",
                    boxShadow: "0 10px 25px rgba(0,0,0,0.15)"
                }}
            >
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "12px" }}>
                    <div>
                        <h3 style={{ margin: 0, fontSize: "1.1rem", color: "#1e2d3d" }}>
                            {mailbox.serialNumber}
                        </h3>
                        <p style={{ margin: "4px 0 0 0", color: "#4b5563", fontSize: "0.9rem" }}>
                            {mailbox.address}
                        </p>
                    </div>
                    <button
                        onClick={onClose}
                        style={{
                            background: "transparent",
                            border: "none",
                            fontSize: "1.5rem",
                            cursor: "pointer",
                            color: "#64748b",
                            lineHeight: 1
                        }}
                        aria-label="Zatvori"
                    >
                        ×
                    </button>
                </div>
                <div style={{ height: "350px", borderRadius: "8px", overflow: "hidden", border: "1px solid #e2e8f0" }}>
                    <MapContainer
                        center={[Number(mailbox.latitude), Number(mailbox.longitude)]}
                        zoom={16}
                        style={{ height: "100%", width: "100%" }}
                        scrollWheelZoom={true}
                    >
                        <TileLayer
                            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                        />
                        <Marker position={[Number(mailbox.latitude), Number(mailbox.longitude)]} />
                    </MapContainer>
                </div>
            </div>
        </div>
    )
}

interface InlineCreateFormProps {
    formData: {
        serialNumber: string
        address: string
        latitude: string
        longitude: string
        type: MailboxType
        capacity: string
        installationYear: string
        notes: string
    }
    setFormData: React.Dispatch<React.SetStateAction<InlineCreateFormProps["formData"]>>
    showMap: boolean
    setShowMap: (v: boolean) => void
    selectedLocation: { lat: number; lng: number } | null
    setSelectedLocation: (v: { lat: number; lng: number } | null) => void
    submitting: boolean
    setSubmitting: (v: boolean) => void
    handleLocationSelect: (lat: number, lng: number) => void
    onSuccess: () => Promise<void>
    onCancel: () => void
}

function InlineCreateForm(props: InlineCreateFormProps) {
    const {
        formData, setFormData, showMap, setShowMap, selectedLocation, setSelectedLocation,
        submitting, setSubmitting, handleLocationSelect, onSuccess, onCancel
    } = props

    const handleInputChange = (field: keyof typeof formData, value: string | MailboxType) => {
        setFormData(prev => ({ ...prev, [field]: value }))
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()

        if (!formData.serialNumber || !formData.address || !formData.latitude ||
            !formData.longitude || !formData.capacity || !formData.installationYear) {
            toast.error("Sva obavezna polja moraju biti popunjena")
            return
        }

        const capacity = parseInt(formData.capacity)
        const installationYear = parseInt(formData.installationYear)
        const latitude = parseFloat(formData.latitude)
        const longitude = parseFloat(formData.longitude)

        if (isNaN(capacity) || capacity <= 0) {
            toast.error("Kapacitet mora biti pozitivan broj")
            return
        }
        if (isNaN(installationYear) || installationYear < 1900 || installationYear > new Date().getFullYear() + 10) {
            toast.error("Godina instalacije nije validna")
            return
        }
        if (isNaN(latitude) || latitude < -90 || latitude > 90) {
            toast.error("Latitude mora biti između -90 i 90")
            return
        }
        if (isNaN(longitude) || longitude < -180 || longitude > 180) {
            toast.error("Longitude mora biti između -180 i 180")
            return
        }

        try {
            setSubmitting(true)
            await createMailbox({
                serialNumber: formData.serialNumber.trim(),
                address: formData.address.trim(),
                latitude,
                longitude,
                type: formData.type,
                capacity,
                installationYear,
                notes: formData.notes.trim() || undefined
            })

            toast.success(`Sandučić ${formData.serialNumber} uspješno dodan!`)

            setFormData({
                serialNumber: "",
                address: "",
                latitude: "",
                longitude: "",
                type: MailboxType.WallSmall,
                capacity: "",
                installationYear: "",
                notes: ""
            })
            setSelectedLocation(null)
            await onSuccess()
        } catch (error: unknown) {
            const message = error instanceof Error ? error.message : ""
            if (message.includes("već postoji")) {
                toast.error("Sandučić sa ovim serijskim brojem već postoji")
            } else {
                toast.error("Greška pri kreiranju sandučića")
            }
        } finally {
            setSubmitting(false)
        }
    }

    const handleCancel = () => {
        setSelectedLocation(null)
        setFormData({
            serialNumber: "",
            address: "",
            latitude: "43.8563",
            longitude: "18.4131",
            type: MailboxType.WallSmall,
            capacity: "100",
            installationYear: new Date().getFullYear().toString(),
            notes: ""
        })
        onCancel()
    }

    return (
        <div style={{
            marginBottom: "24px",
            padding: "20px",
            backgroundColor: "#f8fafc",
            border: "1px solid #e2e8f0",
            borderRadius: "8px"
        }}>
            <h3 style={{ margin: "0 0 16px 0", fontSize: "1rem", color: "#1e2d3d" }}>
                Dodaj novi sandučić
            </h3>
            <form onSubmit={handleSubmit}>
                <div className="form-row">
                    <div className="form-field">
                        <label className="form-field__label" htmlFor="serialNumber">Serijski broj *</label>
                        <input
                            type="text"
                            id="serialNumber"
                            className="form-field__input"
                            value={formData.serialNumber}
                            onChange={(e) => handleInputChange("serialNumber", e.target.value)}
                            required
                        />
                    </div>
                    <div className="form-field">
                        <label className="form-field__label" htmlFor="type">Tip sandučića *</label>
                        <select
                            id="type"
                            className="form-field__input"
                            value={formData.type}
                            onChange={(e) => handleInputChange("type", parseInt(e.target.value) as MailboxType)}
                            required
                        >
                            <option value={MailboxType.WallSmall}>{mailboxTypeLabels[MailboxType.WallSmall]}</option>
                            <option value={MailboxType.StandaloneLarge}>{mailboxTypeLabels[MailboxType.StandaloneLarge]}</option>
                            <option value={MailboxType.IndoorResidential}>{mailboxTypeLabels[MailboxType.IndoorResidential]}</option>
                            <option value={MailboxType.SpecialPriority}>{mailboxTypeLabels[MailboxType.SpecialPriority]}</option>
                        </select>
                    </div>
                </div>

                <div className="form-field">
                    <label className="form-field__label" htmlFor="address">Adresa *</label>
                    <input
                        type="text"
                        id="address"
                        className="form-field__input"
                        value={formData.address}
                        onChange={(e) => handleInputChange("address", e.target.value)}
                        required
                    />
                </div>

                <div className="form-row">
                    <div className="form-field">
                        <label className="form-field__label" htmlFor="latitude">Latitude *</label>
                        <input
                            type="number"
                            id="latitude"
                            step="0.000001"
                            className="form-field__input"
                            value={formData.latitude}
                            onChange={(e) => handleInputChange("latitude", e.target.value)}
                            required
                        />
                    </div>
                    <div className="form-field">
                        <label className="form-field__label" htmlFor="longitude">Longitude *</label>
                        <input
                            type="number"
                            id="longitude"
                            step="0.000001"
                            className="form-field__input"
                            value={formData.longitude}
                            onChange={(e) => handleInputChange("longitude", e.target.value)}
                            required
                        />
                    </div>
                </div>

                <div className="form-field">
                    <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "8px" }}>
                        <label className="form-field__label" style={{ margin: 0 }}>Lokacija na mapi</label>
                        <button
                            type="button"
                            className="btn"
                            style={{
                                padding: "4px 8px",
                                fontSize: "0.75rem",
                                backgroundColor: showMap ? "#2563a8" : "#1b3a5c",
                                color: "white",
                                border: "none",
                                borderRadius: "4px",
                                cursor: "pointer"
                            }}
                            onClick={() => setShowMap(!showMap)}
                        >
                            {showMap ? "Sakrij" : "Prikaži"}
                        </button>
                    </div>
                    {showMap && (
                        <OpenStreetMapPicker
                            onLocationSelect={handleLocationSelect}
                            initialLat={parseFloat(formData.latitude)}
                            initialLng={parseFloat(formData.longitude)}
                            height="250px"
                        />
                    )}
                    {selectedLocation && (
                        <div style={{
                            marginTop: "8px",
                            padding: "6px 10px",
                            backgroundColor: "#f0fdf4",
                            border: "1px solid #bbf7d0",
                            borderRadius: "4px",
                            fontSize: "0.8rem",
                            color: "#15803d"
                        }}>
                            📍 {selectedLocation.lat.toFixed(6)}, {selectedLocation.lng.toFixed(6)}
                        </div>
                    )}
                </div>

                <div className="form-row">
                    <div className="form-field">
                        <label className="form-field__label" htmlFor="capacity">Kapacitet *</label>
                        <input
                            type="number"
                            id="capacity"
                            className="form-field__input"
                            value={formData.capacity}
                            onChange={(e) => handleInputChange("capacity", e.target.value)}
                            required
                        />
                    </div>
                    <div className="form-field">
                        <label className="form-field__label" htmlFor="installationYear">Godina instalacije *</label>
                        <input
                            type="number"
                            id="installationYear"
                            className="form-field__input"
                            value={formData.installationYear}
                            onChange={(e) => handleInputChange("installationYear", e.target.value)}
                            min="1900"
                            max={new Date().getFullYear() + 10}
                            required
                        />
                    </div>
                </div>

                <div className="form-field">
                    <label className="form-field__label" htmlFor="notes">Napomene</label>
                    <textarea
                        id="notes"
                        className="form-field__input"
                        value={formData.notes}
                        onChange={(e) => handleInputChange("notes", e.target.value)}
                        rows={2}
                    />
                </div>

                <div style={{ display: "flex", justifyContent: "center", gap: "12px", marginTop: "20px" }}>
                    <button
                        type="submit"
                        className="btn btn--primary"
                        disabled={submitting}
                        style={{ padding: "10px 20px", fontSize: "0.9rem", fontWeight: 500 }}
                    >
                        {submitting ? "Čuvanje..." : "Sačuvaj"}
                    </button>
                    <button
                        type="button"
                        className="btn"
                        onClick={handleCancel}
                        style={{
                            backgroundColor: "#64748b",
                            color: "white",
                            padding: "10px 20px",
                            fontSize: "0.9rem",
                            fontWeight: 500
                        }}
                    >
                        Otkaži
                    </button>
                </div>
            </form>
        </div>
    )
}
