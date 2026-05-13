import { zodResolver } from "@hookform/resolvers/zod"
import { useForm, type Resolver, type FieldValues, type UseFormRegister, type UseFormWatch, type UseFormSetValue, type FieldErrors } from "react-hook-form"
import { toast } from "sonner"
import { z } from "zod"
import { Layout } from "../../components/Layout/Layout"
import OpenStreetMapPicker from "../../components/common/OpenStreetMapPicker"
import { useState, useCallback } from "react"
import { useNavigate } from "react-router-dom"
import { createMailbox, checkSerialNumberExists, MailboxType, MailboxPriority, mailboxTypeLabels, MailboxWorkingDays } from "../../../infrastructure/api/mailboxes/mailboxesApi"
import { availabilitySchema, mapAvailabilityToRequest } from "../../../infrastructure/validation/availabilitySchema"
import { workingDaysSchema } from "../../../infrastructure/validation/workingDaysSchema"
import { AvailabilitySection } from "../../components/mailboxes/AvailabilitySection"
import { WorkingDaysSection } from "../../components/mailboxes/WorkingDaysSection"

const schema = z.object({
    serialNumber: z
        .string()
        .min(1, "Serijski broj je obavezan")
        .max(50, "Serijski broj može imati najviše 50 karaktera")
        .refine(async (value) => {
            if (!value) return true
            const exists = await checkSerialNumberExists(value.trim())
            return !exists
        }, "Sandučić sa ovim serijskim brojem već postoji"),
    address: z
        .string()
        .min(1, "Adresa je obavezna")
        .max(200, "Adresa može imati najviše 200 karaktera"),
    latitude: z
        .number({ error: "Odaberite lokaciju na mapi" })
        .min(-90).max(90),
    longitude: z
        .number({ error: "Odaberite lokaciju na mapi" })
        .min(-180).max(180),
    type: z.nativeEnum(MailboxType),
    priority: z.nativeEnum(MailboxPriority),
    capacity: z
        .number()
        .min(1, "Kapacitet mora biti veći od 0")
        .max(10000, "Kapacitet ne može biti veći od 10000"),
    installationYear: z
        .number()
        .min(1900, "Godina instalacije mora biti nakon 1900")
        .max(new Date().getFullYear(), `Godina instalacije ne može biti veća od ${new Date().getFullYear()}`),
    notes: z
        .string()
        .max(500, "Napomene mogu imati najviše 500 karaktera")
        .optional()
}).and(availabilitySchema).and(workingDaysSchema)

type FormData = z.infer<typeof schema>

export default function CreateMailboxPage() {
    const navigate = useNavigate()
    const [selectedLocation, setSelectedLocation] = useState<{ lat: number; lng: number } | null>(null)

    const { register, handleSubmit, setValue, watch, formState: { errors, isSubmitting } } = useForm<FormData>({
        resolver: zodResolver(schema) as unknown as Resolver<FormData>,
        mode: "onChange",
        defaultValues: {
            type: MailboxType.WallSmall,
            priority: MailboxPriority.Srednji,
            capacity: 100,
            installationYear: new Date().getFullYear(),
            isAlwaysAvailable: false,
            hasSecondSlot: false,
            slot1Start: "",
            slot1End: "",
            slot2Start: "",
            slot2End: "",
            workingDays: MailboxWorkingDays.RadniDani,
        }
    })

    const watchedType = watch("type")
    const watchedLat = watch("latitude")
    const watchedLng = watch("longitude")

    const handleLocationSelect = useCallback((lat: number, lng: number) => {
        setSelectedLocation({ lat, lng })
        setValue("latitude", lat, { shouldValidate: true })
        setValue("longitude", lng, { shouldValidate: true })
    }, [setValue])

    const onSubmit = async (data: FormData) => {
        if (!selectedLocation) {
            toast.error("Odaberite lokaciju na mapi")
            return
        }
        try {
            await createMailbox({
                serialNumber: data.serialNumber.trim(),
                address: data.address.trim(),
                latitude: data.latitude,
                longitude: data.longitude,
                type: data.type,
                priority: data.priority,
                capacity: data.capacity,
                installationYear: data.installationYear,
                workingDays: data.workingDays as MailboxWorkingDays,
                notes: data.notes?.trim() || null,
                ...mapAvailabilityToRequest(data),
            })
            toast.success(`Sandučić ${data.serialNumber} uspješno dodan!`)
            navigate("/admin/mailboxes")
        } catch (error: unknown) {
            const err = error as { message?: string; status?: number }
            if (err.message?.includes("već postoji")) {
                toast.error("Sandučić sa ovim serijskim brojem već postoji")
            } else if (err.status === 403) {
                toast.error("Nemate dozvolu za kreiranje sandučića.")
            } else {
                toast.error("Greška pri kreiranju sandučića. Pokušajte ponovo.")
            }
        }
    }

    return (
        <Layout>
            <div className="page-container">
                <div className="form-card">
                    <div className="form-card__header">
                        <h1 className="form-card__title">Dodavanje novog sandučića</h1>
                        <p className="form-card__subtitle">
                            Unesite podatke o sandučiću i označite lokaciju na mapi.
                        </p>
                    </div>

                    <form className="form-card__body" onSubmit={handleSubmit(onSubmit)} noValidate>

                        {/* Red 1: Serijski broj i tip */}
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
                                    {...register("serialNumber")}
                                />
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

                        {/* Prioritet — puna širina */}
                        <div className="form-field">
                            <label className="form-field__label" htmlFor="priority">
                                Prioritet
                            </label>
                            <select
                                id="priority"
                                className="form-field__input"
                                {...register("priority", { valueAsNumber: true })}
                            >
                                <option value={MailboxPriority.Visok}>🔴 Visok — pražnjenje svakodnevno</option>
                                <option value={MailboxPriority.Srednji}>🟡 Srednji — pražnjenje svaka 2-3 dana</option>
                                <option value={MailboxPriority.Nizak}>🟢 Nizak — pražnjenje po potrebi</option>
                            </select>
                        </div>

                        {/* US-32: Dostupnost */}
                        <AvailabilitySection
                            register={register as unknown as UseFormRegister<FieldValues>}
                            watch={watch as unknown as UseFormWatch<FieldValues>}
                            setValue={setValue as unknown as UseFormSetValue<FieldValues>}
                            errors={errors as FieldErrors<FieldValues>}
                        />

                        {/* US-33: Radni dani */}
                        <WorkingDaysSection
                            watch={watch as unknown as UseFormWatch<FieldValues>}
                            setValue={setValue as unknown as UseFormSetValue<FieldValues>}
                            errors={errors as FieldErrors<FieldValues>}
                        />

                        {/* Mapa */}
                        <div className="form-field">
                            <label className="form-field__label">
                                Lokacija na mapi *
                            </label>
                            <p style={{ fontSize: "0.85rem", color: "#64748b", marginBottom: "8px", marginTop: 0 }}>
                                Kliknite na mapu da označite lokaciju sandučića.
                            </p>
                            <div style={{ border: "1px solid #e2e8f0", borderRadius: "8px", overflow: "hidden" }}>
                                <OpenStreetMapPicker
                                    onLocationSelect={handleLocationSelect}
                                    onAddressFound={(address) => setValue("address", address, { shouldValidate: true })}
                                    initialLat={watchedLat ?? 43.8563}
                                    initialLng={watchedLng ?? 18.4131}
                                    height="350px"
                                />
                            </div>
                            {selectedLocation ? (
                                <div style={{
                                    marginTop: "8px", padding: "8px 12px",
                                    backgroundColor: "#f0fdf4", border: "1px solid #bbf7d0",
                                    borderRadius: "6px", fontSize: "0.85rem", color: "#15803d"
                                }}>
                                    📍 Odabrana lokacija: {selectedLocation.lat.toFixed(6)}, {selectedLocation.lng.toFixed(6)}
                                </div>
                            ) : (
                                <div style={{
                                    marginTop: "8px", padding: "8px 12px",
                                    backgroundColor: "#fef9c3", border: "1px solid #fde047",
                                    borderRadius: "6px", fontSize: "0.85rem", color: "#854d0e"
                                }}>
                                    ⚠️ Lokacija nije odabrana — kliknite na mapu
                                </div>
                            )}
                        </div>

                        {/* Kapacitet i godina */}
                        <div className="form-row">
                            <div className="form-field">
                                <label className="form-field__label" htmlFor="capacity">
                                    Kapacitet (broj pisama) *
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
                                    placeholder="npr. 2020"
                                    min="1900"
                                    max={new Date().getFullYear()}
                                    {...register("installationYear", { valueAsNumber: true })}
                                />
                                {errors.installationYear && (
                                    <p className="form-field__error">{errors.installationYear.message}</p>
                                )}
                            </div>
                        </div>

                        {/* Info o tipu */}
                        <div style={{
                            padding: "12px", backgroundColor: "#f8fafc",
                            border: "1px solid #e2e8f0", borderRadius: "8px",
                            fontSize: "0.85rem", color: "#64748b"
                        }}>
                            <strong>Odabrani tip:</strong> {mailboxTypeLabels[watchedType]}
                            {watchedType === MailboxType.WallSmall && <div style={{ marginTop: "4px" }}>🏠 Zidni sandučić, manji kapacitet</div>}
                            {watchedType === MailboxType.StandaloneLarge && <div style={{ marginTop: "4px" }}>📮 Samostojeći sandučić, veliki kapacitet</div>}
                            {watchedType === MailboxType.IndoorResidential && <div style={{ marginTop: "4px" }}>🏢 Unutrašnji, stambene zgrade</div>}
                            {watchedType === MailboxType.SpecialPriority && <div style={{ marginTop: "4px" }}>⭐ Specijalni, prioritetni tretman</div>}
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

                        {/* Dugmad */}
                        <div className="form-actions" style={{ display: "flex", justifyContent: "flex-end", gap: "12px", marginTop: "24px" }}>
                            <button
                                type="button"
                                className="btn"
                                style={{
                                    padding: "12px 24px", backgroundColor: "#64748b",
                                    color: "white", border: "none", borderRadius: "6px",
                                    cursor: "pointer", fontSize: "0.9rem", fontWeight: "500"
                                }}
                                onClick={() => navigate("/admin/mailboxes")}
                            >
                                Otkaži
                            </button>
                            <button
                                type="submit"
                                className="btn btn--primary"
                                disabled={isSubmitting || !selectedLocation}
                                style={{ padding: "12px 24px", fontSize: "0.9rem", fontWeight: "500" }}
                            >
                                {isSubmitting ? "Čuvanje..." : "Sačuvaj sandučić"}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </Layout>
    )
}