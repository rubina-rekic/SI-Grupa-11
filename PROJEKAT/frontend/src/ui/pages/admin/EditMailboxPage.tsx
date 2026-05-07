import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { toast } from "sonner"
import { z } from "zod"
import { useEffect, useState, useCallback } from "react"
import { useNavigate, useParams } from "react-router-dom"
import { 
    getMailboxById,
    updateMailbox,
    MailboxType, 
    mailboxTypeLabels,
    type MailboxResponse,
    type UpdateMailboxRequest
} from "../../../infrastructure/api/mailboxes/mailboxesApi"
import { Layout } from "../../components/Layout/Layout"
import OpenStreetMapPicker from "../../components/common/OpenStreetMapPicker"

const schema = z.object({
    serialNumber: z
        .string()
        .min(1, "Serijski broj je obavezan")
        .max(50, "Serijski broj može imati najviše 50 karaktera"),
    address: z
        .string()
        .min(1, "Adresa je obavezna")
        .max(200, "Adresa može imati najviše 200 karaktera"),
    latitude: z
        .number()
        .min(-90, "Latitude mora biti između -90 i 90")
        .max(90, "Latitude mora biti između -90 i 90"),
    longitude: z
        .number()
        .min(-180, "Longitude mora biti između -180 i 180")
        .max(180, "Longitude mora biti između -180 i 180"),
    type: z.nativeEnum(MailboxType).refine((val) => val !== undefined, {
        message: "Tip sandučića je obavezan"
    }),
    capacity: z
        .number()
        .min(1, "Kapacitet mora biti veći od 0")
        .max(10000, "Kapacitet ne može biti veći od 10000"),
    installationYear: z
        .number()
        .min(1900, "Godina instalacije mora biti nakon 1900")
        .max(new Date().getFullYear() + 10, "Godina instalacije ne može biti u budućnosti"),
    notes: z
        .string()
        .max(500, "Napomene mogu imati najviše 500 karaktera")
        .optional()
})

type FormData = z.infer<typeof schema>

export default function EditMailboxPage() {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()
    const [loading, setLoading] = useState(true)
    const [selectedLocation, setSelectedLocation] = useState<{ lat: number; lng: number } | null>(null)
    const [showMap, setShowMap] = useState(false)
    const [originalData, setOriginalData] = useState<MailboxResponse | null>(null)
    
    const { register, handleSubmit, setValue, watch, formState: { errors, isSubmitting }, trigger, reset } = useForm<FormData>({
        resolver: zodResolver(schema),
        mode: "onChange"
    })

    const watchedLat = watch("latitude")
    const watchedLng = watch("longitude")
    const watchedType = watch("type")

    useEffect(() => {
        const loadMailbox = async () => {
            if (!id) return
            
            try {
                setLoading(true)
                const mailbox = await getMailboxById(id)
                setOriginalData(mailbox)
                
                reset({
                    serialNumber: mailbox.serialNumber,
                    address: mailbox.address,
                    latitude: mailbox.latitude,
                    longitude: mailbox.longitude,
                    type: mailbox.type,
                    capacity: mailbox.capacity,
                    installationYear: mailbox.installationYear,
                    notes: mailbox.notes || ""
                })
                
                setSelectedLocation({ lat: mailbox.latitude, lng: mailbox.longitude })
            } catch {
                toast.error("Greška pri učitavanju sandučića")
                navigate("/admin/mailboxes")
            } finally {
                setLoading(false)
            }
        }

        loadMailbox()
    }, [id, navigate, reset])

    const handleLocationSelect = useCallback((lat: number, lng: number) => {
        setSelectedLocation({ lat, lng })
        setValue("latitude", lat)
        setValue("longitude", lng)
        trigger(["latitude", "longitude"])
    }, [setValue, trigger])

    const toggleMap = () => {
        setShowMap(!showMap)
    }

    const onSubmit = async (data: FormData) => {
        if (!id || !originalData) return

        try {
            const updateData: UpdateMailboxRequest = {
                serialNumber: data.serialNumber.trim(),
                address: data.address.trim(),
                latitude: data.latitude,
                longitude: data.longitude,
                type: data.type,
                capacity: data.capacity,
                installationYear: data.installationYear,
                notes: data.notes?.trim() || undefined,
                priority: originalData.priority
            }

            await updateMailbox(id, updateData)
            
            toast.success(`Podaci o sandučiću ${data.serialNumber} su uspješno ažurirani!`, {
                description: "Sve promjene su zabilježene u audit log."
            })
            
            navigate("/admin/mailboxes")
            
        } catch (error: unknown) {
            const errorDetails = error as {
                message?: string
                response?: unknown
                status?: number
            }
            
            if (errorDetails.message?.includes("već postoji")) {
                toast.error("Sandučić sa ovim serijskim brojem već postoji")
            } else if (errorDetails.status === 404) {
                toast.error("Sandučić nije pronađen")
                navigate("/admin/mailboxes")
            } else if (errorDetails.status === 403) {
                toast.error("Nemate dozvolu za uređivanje sandučića")
            } else if (errorDetails.status === 401) {
                toast.error("Niste ulogovani. Molimo prijavite se.")
            } else {
                const errorMessage = error instanceof Error ? error.message : "Greška pri ažuriranju sandučića"
                toast.error(errorMessage)
            }
        }
    }

    if (loading) {
        return (
            <Layout>
                <div className="page-container">
                    <h1>Uređivanje sandučića</h1>
                    <div>Učitavanje...</div>
                </div>
            </Layout>
        )
    }

    if (!originalData) {
        return (
            <Layout>
                <div className="page-container">
                    <h1>Sandučić nije pronađen</h1>
                </div>
            </Layout>
        )
    }

    return (
        <Layout>
            <div className="page-container">
                <div className="form-card">
                    <div className="form-card__header">
                        <h1 className="form-card__title">Uređivanje sandučića</h1>
                        <p className="form-card__subtitle">
                            Izmijenite podatke o sandučiću i spremite promjene.
                        </p>
                    </div>

                    <form className="form-card__body" onSubmit={handleSubmit(onSubmit)} noValidate>
                        {/* Osnovni podaci */}
                        <div className="form-row">
                            <div className="form-field">
                                <label className="form-field__label" htmlFor="serialNumber">
                                    Serijski broj *
                                </label>
                                <input
                                    id="serialNumber"
                                    type="text"
                                    className={`form-field__input${errors.serialNumber ? " form-field__input--error" : ""}`}
                                    placeholder="npr. SN001"
                                    autoComplete="off"
                                    readOnly
                                    style={{ backgroundColor: "#f8fafc", cursor: "not-allowed" }}
                                    {...register("serialNumber")}
                                />
                                <p style={{ fontSize: "0.85rem", color: "#64748b", marginTop: "4px" }}>
                                    📝 Uređujete postojeći sandučić - serijski broj se ne može promijeniti
                                </p>
                                {errors.serialNumber && (
                                    <p className="form-field__error">{errors.serialNumber.message}</p>
                                )}
                            </div>

                            <div className="form-field">
                                <label className="form-field__label" htmlFor="type">
                                    Tip sandučića *
                                </label>
                                <select
                                    id="type"
                                    className={`form-field__input${errors.type ? " form-field__input--error" : ""}`}
                                    {...register("type", { valueAsNumber: true })}
                                >
                                    <option value={MailboxType.WallSmall}>{mailboxTypeLabels[MailboxType.WallSmall]}</option>
                                    <option value={MailboxType.StandaloneLarge}>{mailboxTypeLabels[MailboxType.StandaloneLarge]}</option>
                                    <option value={MailboxType.IndoorResidential}>{mailboxTypeLabels[MailboxType.IndoorResidential]}</option>
                                    <option value={MailboxType.SpecialPriority}>{mailboxTypeLabels[MailboxType.SpecialPriority]}</option>
                                </select>
                                {errors.type && (
                                    <p className="form-field__error">{errors.type.message}</p>
                                )}
                            </div>
                        </div>

                        <div className="form-field">
                            <label className="form-field__label" htmlFor="address">
                                Adresa *
                            </label>
                            <input
                                id="address"
                                type="text"
                                className={`form-field__input${errors.address ? " form-field__input--error" : ""}`}
                                placeholder="npr. Zmaja od Bosne 1, Sarajevo"
                                autoComplete="street-address"
                                {...register("address")}
                            />
                            {errors.address && (
                                <p className="form-field__error">{errors.address.message}</p>
                            )}
                        </div>

                        {/* Koordinate */}
                        <div className="form-row">
                            <div className="form-field">
                                <label className="form-field__label" htmlFor="latitude">
                                    Latitude *
                                </label>
                                <input
                                    id="latitude"
                                    type="number"
                                    step="0.000001"
                                    className={`form-field__input${errors.latitude ? " form-field__input--error" : ""}`}
                                    placeholder="-90 do 90"
                                    {...register("latitude", { valueAsNumber: true })}
                                />
                                {errors.latitude && (
                                    <p className="form-field__error">{errors.latitude.message}</p>
                                )}
                            </div>

                            <div className="form-field">
                                <label className="form-field__label" htmlFor="longitude">
                                    Longitude *
                                </label>
                                <input
                                    id="longitude"
                                    type="number"
                                    step="0.000001"
                                    className={`form-field__input${errors.longitude ? " form-field__input--error" : ""}`}
                                    placeholder="-180 do 180"
                                    {...register("longitude", { valueAsNumber: true })}
                                />
                                {errors.longitude && (
                                    <p className="form-field__error">{errors.longitude.message}</p>
                                )}
                            </div>
                        </div>

                        {/* Mapa */}
                        <div className="form-field">
                            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "8px" }}>
                                <label className="form-field__label" style={{ margin: 0 }}>
                                    Lokacija na mapi
                                </label>
                                <button
                                    type="button"
                                    className="btn"
                                    style={{
                                        padding: "6px 12px",
                                        fontSize: "0.8rem",
                                        backgroundColor: showMap ? "#2563a8" : "#1b3a5c",
                                        color: "white",
                                        border: "none",
                                        borderRadius: "6px",
                                        cursor: "pointer"
                                    }}
                                    onClick={toggleMap}
                                >
                                    {showMap ? "Sakrij mapu" : "Prikaži mapu"}
                                </button>
                            </div>
                            
                            {showMap && (
                                <OpenStreetMapPicker
                                    onLocationSelect={handleLocationSelect}
                                    initialLat={watchedLat}
                                    initialLng={watchedLng}
                                    height="350px"
                                />
                            )}
                            
                            {selectedLocation && (
                                <div style={{
                                    marginTop: "8px",
                                    padding: "8px 12px",
                                    backgroundColor: "#f0fdf4",
                                    border: "1px solid #bbf7d0",
                                    borderRadius: "6px",
                                    fontSize: "0.85rem",
                                    color: "#15803d"
                                }}>
                                    📍 Izabrana lokacija: {selectedLocation.lat.toFixed(6)}, {selectedLocation.lng.toFixed(6)}
                                </div>
                            )}
                        </div>

                        {/* Kapacitet i godina */}
                        <div className="form-row">
                            <div className="form-field">
                                <label className="form-field__label" htmlFor="capacity">
                                    Kapacitet *
                                </label>
                                <input
                                    id="capacity"
                                    type="number"
                                    className={`form-field__input${errors.capacity ? " form-field__input--error" : ""}`}
                                    placeholder="npr. 100"
                                    min="1"
                                    max="10000"
                                    {...register("capacity", { valueAsNumber: true })}
                                />
                                {errors.capacity && (
                                    <p className="form-field__error">{errors.capacity.message}</p>
                                )}
                            </div>

                            <div className="form-field">
                                <label className="form-field__label" htmlFor="installationYear">
                                    Godina instalacije *
                                </label>
                                <input
                                    id="installationYear"
                                    type="number"
                                    className={`form-field__input${errors.installationYear ? " form-field__input--error" : ""}`}
                                    placeholder="npr. 2023"
                                    min="1900"
                                    max={new Date().getFullYear() + 10}
                                    {...register("installationYear", { valueAsNumber: true })}
                                />
                                {errors.installationYear && (
                                    <p className="form-field__error">{errors.installationYear.message}</p>
                                )}
                            </div>
                        </div>

                        {/* Napomene */}
                        <div className="form-field">
                            <label className="form-field__label" htmlFor="notes">
                                Napomene
                            </label>
                            <textarea
                                id="notes"
                                className={`form-field__input${errors.notes ? " form-field__input--error" : ""}`}
                                placeholder="Opcionalne napomene o sandučiću..."
                                rows={3}
                                {...register("notes")}
                            />
                            {errors.notes && (
                                <p className="form-field__error">{errors.notes.message}</p>
                            )}
                        </div>

                        {/* Informacije o tipu */}
                        <div style={{
                            padding: "12px",
                            backgroundColor: "#f8fafc",
                            border: "1px solid #e2e8f0",
                            borderRadius: "8px",
                            fontSize: "0.85rem",
                            color: "#64748b"
                        }}>
                            <strong>Tip sandučića:</strong> {mailboxTypeLabels[watchedType]}
                            {watchedType === MailboxType.WallSmall && (
                                <div style={{ marginTop: "4px" }}>🏠 Zidni sandučić, manji kapacitet</div>
                            )}
                            {watchedType === MailboxType.StandaloneLarge && (
                                <div style={{ marginTop: "4px" }}>📮 Samostojeći sandučić, veliki kapacitet</div>
                            )}
                            {watchedType === MailboxType.IndoorResidential && (
                                <div style={{ marginTop: "4px" }}>🏢 Unutrašnji, stambene zgrade</div>
                            )}
                            {watchedType === MailboxType.SpecialPriority && (
                                <div style={{ marginTop: "4px" }}>⭐ Specijalni, prioritetni tretman</div>
                            )}
                        </div>

                        {/* Dugmad */}
                        <div className="form-actions" style={{
                            display: "flex",
                            justifyContent: "center",
                            gap: "12px",
                            marginTop: "24px"
                        }}>
                            <button 
                                type="button"
                                className="btn"
                                style={{
                                    padding: "12px 24px",
                                    backgroundColor: "#64748b",
                                    color: "white",
                                    border: "none",
                                    borderRadius: "6px",
                                    cursor: "pointer",
                                    fontSize: "0.9rem",
                                    fontWeight: "500"
                                }}
                                onClick={() => navigate("/admin/mailboxes")}
                            >
                                Otkaži
                            </button>
                            <button
                                type="submit"
                                className="btn btn--primary"
                                disabled={isSubmitting}
                                style={{
                                    padding: "12px 24px",
                                    fontSize: "0.9rem",
                                    fontWeight: "500"
                                }}
                            >
                                {isSubmitting ? "Čuvanje..." : "Sačuvaj izmjene"}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </Layout>
    )
}
