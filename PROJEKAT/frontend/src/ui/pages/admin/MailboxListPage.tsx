import { useState, useEffect } from "react"
import { toast } from "sonner"
import { createMailbox, getAllMailboxes, MailboxType, mailboxTypeLabels } from "../../../infrastructure/api/mailboxes/mailboxesApi"
import { Layout } from "../../components/Layout/Layout"
import OpenStreetMapPicker from "../../components/common/OpenStreetMapPicker"
import { useCallback } from "react"

interface Mailbox {
    id: string
    serialNumber: string
    address: string
    latitude: number
    longitude: number
    type: MailboxType
    capacity: number
    installationYear: number
    createdAt: string
    updatedAt: string
    notes?: string
}

export default function MailboxListPage() {
    const [mailboxes, setMailboxes] = useState<Mailbox[]>([])
    const [loading, setLoading] = useState(true)
    const [showForm, setShowForm] = useState(false)
    const [showMap, setShowMap] = useState(false)
    const [selectedLocation, setSelectedLocation] = useState<{ lat: number; lng: number } | null>(null)
    const [formData, setFormData] = useState({
        serialNumber: "",
        address: "",
        latitude: "43.8563",
        longitude: "18.4131",
        type: MailboxType.WallSmall,
        capacity: "100",
        installationYear: new Date().getFullYear().toString(),
        notes: ""
    })
    const [submitting, setSubmitting] = useState(false)
    
    const handleLocationSelect = useCallback((lat: number, lng: number) => {
        setSelectedLocation({ lat, lng })
        setFormData(prev => ({ ...prev, latitude: lat.toString(), longitude: lng.toString() }))
    }, [])

    const toggleMap = () => {
        setShowMap(!showMap)
    }

    useEffect(() => {
        loadMailboxes()
    }, [])

    const loadMailboxes = async () => {
        try {
            const data = await getAllMailboxes()
            setMailboxes(data)
        } catch (error) {
            toast.error("Greška pri učitavanju sandučića")
        } finally {
            setLoading(false)
        }
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
            
            // Reset form
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
            setShowForm(false)
            
            // Reload mailboxes
            await loadMailboxes()
        } catch (error: any) {
            if (error.message?.includes("već postoji")) {
                toast.error("Sandučić sa ovim serijskim brojem već postoji")
            } else {
                toast.error("Greška pri kreiranju sandučića")
            }
        } finally {
            setSubmitting(false)
        }
    }

    const handleInputChange = (field: string, value: string | MailboxType) => {
        setFormData(prev => ({ ...prev, [field]: value }))
    }

    if (loading) {
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
                                Lista sandučića ({mailboxes.length})
                            </h2>
                        </div>

                        {/* Forma za dodavanje */}
                        {showForm && (
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
                                            <label className="form-field__label" htmlFor="serialNumber">
                                                Serijski broj *
                                            </label>
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
                                            <label className="form-field__label" htmlFor="type">
                                                Tip sandučića *
                                            </label>
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
                                        <label className="form-field__label" htmlFor="address">
                                            Adresa *
                                        </label>
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
                                            <label className="form-field__label" htmlFor="latitude">
                                                Latitude *
                                            </label>
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
                                            <label className="form-field__label" htmlFor="longitude">
                                                Longitude *
                                            </label>
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
                                            <label className="form-field__label" style={{ margin: 0 }}>
                                                Lokacija na mapi
                                            </label>
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
                                                onClick={toggleMap}
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
                                            <label className="form-field__label" htmlFor="capacity">
                                                Kapacitet *
                                            </label>
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
                                            <label className="form-field__label" htmlFor="installationYear">
                                                Godina instalacije *
                                            </label>
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
                                        <label className="form-field__label" htmlFor="notes">
                                            Napomene
                                        </label>
                                        <textarea
                                            id="notes"
                                            className="form-field__input"
                                            value={formData.notes}
                                            onChange={(e) => handleInputChange("notes", e.target.value)}
                                            rows={2}
                                        />
                                    </div>

                                    <div className="form-actions" style={{
                                        display: "flex",
                                        justifyContent: "center",
                                        gap: "12px",
                                        marginTop: "20px"
                                    }}>
                                        <button 
                                            type="submit" 
                                            className="btn btn--primary"
                                            disabled={submitting}
                                            style={{
                                                marginRight: "8px",
                                                padding: "10px 20px",
                                                fontSize: "0.9rem",
                                                fontWeight: "500"
                                            }}
                                        >
                                            {submitting ? "Čuvanje..." : "Sačuvaj"}
                                        </button>
                                        <button 
                                            type="button" 
                                            className="btn"
                                            style={{
                                                backgroundColor: "#64748b",
                                                color: "white",
                                                padding: "10px 20px",
                                                fontSize: "0.9rem",
                                                fontWeight: "500"
                                            }}
                                            onClick={() => {
                                                setShowForm(false)
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
                                            }}
                                        >
                                            Otkaži
                                        </button>
                                    </div>
                                </form>
                            </div>
                        )}

                        {!showForm && (
                            <button 
                                className="btn btn--primary"
                                style={{ maxWidth: "200px" }}
                                onClick={() => setShowForm(true)}
                            >
                                + Dodaj novi sandučić
                            </button>
                        )}

                        {/* Tabela sa sandučićima */}
                        {mailboxes.length > 0 && (
                            <div style={{
                                overflow: "auto",
                                border: "1px solid #e2e8f0",
                                borderRadius: "8px"
                            }}>
                                <table style={{
                                    width: "100%",
                                    borderCollapse: "collapse",
                                    fontSize: "0.9rem"
                                }}>
                                    <thead>
                                        <tr style={{
                                            backgroundColor: "#f8fafc",
                                            borderBottom: "1px solid #e2e8f0"
                                        }}>
                                            <th style={{
                                                padding: "12px",
                                                textAlign: "left",
                                                fontWeight: "600",
                                                color: "#374151",
                                                borderBottom: "1px solid #e2e8f0"
                                            }}>Serijski broj</th>
                                            <th style={{
                                                padding: "12px",
                                                textAlign: "left",
                                                fontWeight: "600",
                                                color: "#374151",
                                                borderBottom: "1px solid #e2e8f0"
                                            }}>Adresa</th>
                                            <th style={{
                                                padding: "12px",
                                                textAlign: "left",
                                                fontWeight: "600",
                                                color: "#374151",
                                                borderBottom: "1px solid #e2e8f0"
                                            }}>Tip</th>
                                            <th style={{
                                                padding: "12px",
                                                textAlign: "center",
                                                fontWeight: "600",
                                                color: "#374151",
                                                borderBottom: "1px solid #e2e8f0"
                                            }}>Kapacitet</th>
                                            <th style={{
                                                padding: "12px",
                                                textAlign: "center",
                                                fontWeight: "600",
                                                color: "#374151",
                                                borderBottom: "1px solid #e2e8f0"
                                            }}>Godina</th>
                                            <th style={{
                                                padding: "12px",
                                                textAlign: "left",
                                                fontWeight: "600",
                                                color: "#374151",
                                                borderBottom: "1px solid #e2e8f0"
                                            }}>Lokacija</th>
                                            <th style={{
                                                padding: "12px",
                                                textAlign: "center",
                                                fontWeight: "600",
                                                color: "#374151",
                                                borderBottom: "1px solid #e2e8f0"
                                            }}>Kreirano</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {mailboxes.map((mailbox, index) => (
                                            <tr key={mailbox.id} style={{
                                                backgroundColor: index % 2 === 0 ? "#ffffff" : "#f9fafb",
                                                borderBottom: "1px solid #f3f4f6"
                                            }}>
                                                <td style={{
                                                    padding: "12px",
                                                    fontWeight: "500",
                                                    color: "#1f2937"
                                                }}>{mailbox.serialNumber}</td>
                                                <td style={{
                                                    padding: "12px",
                                                    color: "#4b5563"
                                                }}>{mailbox.address}</td>
                                                <td style={{
                                                    padding: "12px",
                                                    color: "#4b5563"
                                                }}>📮 {mailboxTypeLabels[mailbox.type]}</td>
                                                <td style={{
                                                    padding: "12px",
                                                    textAlign: "center",
                                                    color: "#4b5563"
                                                }}>{mailbox.capacity}</td>
                                                <td style={{
                                                    padding: "12px",
                                                    textAlign: "center",
                                                    color: "#4b5563"
                                                }}>{mailbox.installationYear}</td>
                                                <td style={{
                                                    padding: "12px",
                                                    color: "#4b5563",
                                                    fontSize: "0.8rem"
                                                }}>
                                                    {mailbox.latitude.toFixed(4)}, {mailbox.longitude.toFixed(4)}
                                                </td>
                                                <td style={{
                                                    padding: "12px",
                                                    textAlign: "center",
                                                    color: "#6b7280",
                                                    fontSize: "0.8rem"
                                                }}>
                                                    {new Date(mailbox.createdAt).toLocaleDateString('bs-BA')}
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}

                        {mailboxes.length === 0 && !showForm && (
                            <div style={{
                                textAlign: "center",
                                padding: "40px 20px",
                                backgroundColor: "#f8fafc",
                                border: "1px solid #e2e8f0",
                                borderRadius: "8px"
                            }}>
                                <div style={{ fontSize: "2rem", marginBottom: "16px" }}>📮</div>
                                <h3 style={{
                                    margin: "0 0 8px 0",
                                    fontSize: "1.1rem",
                                    color: "#374151"
                                }}>Nema sandučića</h3>
                                <p style={{
                                    margin: 0,
                                    color: "#6b7280",
                                    fontSize: "0.9rem"
                                }}>
                                    Nema nijednog sandučića u sistemu. Kliknite na dugme iznad da dodate prvi.
                                </p>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </Layout>
    )
}
