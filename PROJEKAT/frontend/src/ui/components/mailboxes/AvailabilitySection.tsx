import { type UseFormRegister, type UseFormWatch, type UseFormSetValue, type FieldErrors, type FieldValues } from "react-hook-form"

export interface AvailabilityFields {
    isAlwaysAvailable: boolean
    slot1Start: string
    slot1End: string
    slot2Start: string
    slot2End: string
    hasSecondSlot: boolean
}

interface Props {
    register: UseFormRegister<FieldValues>
    watch: UseFormWatch<FieldValues>
    setValue: UseFormSetValue<FieldValues>
    errors: FieldErrors<FieldValues>
}

export function AvailabilitySection({ register, watch, setValue, errors }: Props) {
    const isAlwaysAvailable: boolean = watch("isAlwaysAvailable") ?? false
    const hasSecondSlot: boolean = watch("hasSecondSlot") ?? false

    const inputStyle = (hasError: boolean): React.CSSProperties => ({
        padding: "8px 12px",
        border: `1px solid ${hasError ? "#ef4444" : "#e2e8f0"}`,
        borderRadius: "6px",
        fontSize: "0.9rem",
        width: "100%",
        boxSizing: "border-box",
        backgroundColor: isAlwaysAvailable ? "#f8fafc" : "#fff",
        cursor: isAlwaysAvailable ? "not-allowed" : "text",
        color: "#1e2d3d"
    })

    return (
        <div style={{
            border: "1px solid #e2e8f0",
            borderRadius: "8px",
            padding: "16px",
            backgroundColor: "#f8fafc",
            display: "flex",
            flexDirection: "column",
            gap: "16px"
        }}>
            <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
                <h3 style={{ margin: 0, fontSize: "0.95rem", fontWeight: 600, color: "#1e2d3d" }}>
                    🕐 Dostupnost sandučića
                </h3>

                {/* 24/7 checkbox */}
                <label style={{
                    display: "flex", alignItems: "center", gap: "8px",
                    cursor: "pointer", fontSize: "0.9rem", color: "#1e2d3d", fontWeight: 500
                }}>
                    <input
                        type="checkbox"
                        {...register("isAlwaysAvailable")}
                        onChange={(e) => {
                            setValue("isAlwaysAvailable", e.target.checked)
                            if (e.target.checked) {
                                // Deaktivirati sva polja ako je 24/7
                                setValue("slot1Start", "")
                                setValue("slot1End", "")
                                setValue("slot2Start", "")
                                setValue("slot2End", "")
                                setValue("hasSecondSlot", false)
                            }
                        }}
                        style={{ width: "16px", height: "16px", cursor: "pointer" }}
                    />
                    24/7 dostupnost
                </label>
            </div>

            {isAlwaysAvailable ? (
                <div style={{
                    padding: "10px 14px",
                    backgroundColor: "#f0fdf4",
                    border: "1px solid #bbf7d0",
                    borderRadius: "6px",
                    fontSize: "0.85rem",
                    color: "#15803d"
                }}>
                    ✅ Sandučić je dostupan cijelo vrijeme — bez vremenskih ograničenja.
                </div>
            ) : (
                <>
                    {/* Termin 1 */}
                    <div>
                        <p style={{ margin: "0 0 8px 0", fontSize: "0.85rem", fontWeight: 600, color: "#374151" }}>
                            Termin 1
                        </p>
                        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "12px" }}>
                            <div className="form-field">
                                <label className="form-field__label" htmlFor="slot1Start">
                                    Početak
                                </label>
                                <input
                                    id="slot1Start"
                                    type="time"
                                    style={inputStyle(!!errors.slot1Start)}
                                    disabled={isAlwaysAvailable}
                                    {...register("slot1Start")}
                                />
                                {errors.slot1Start && (
                                    <p className="form-field__error">{String(errors.slot1Start.message)}</p>
                                )}
                            </div>
                            <div className="form-field">
                                <label className="form-field__label" htmlFor="slot1End">
                                    Kraj
                                </label>
                                <input
                                    id="slot1End"
                                    type="time"
                                    style={inputStyle(!!errors.slot1End)}
                                    disabled={isAlwaysAvailable}
                                    {...register("slot1End")}
                                />
                                {errors.slot1End && (
                                    <p className="form-field__error">{String(errors.slot1End.message)}</p>
                                )}
                            </div>
                        </div>
                        {/* Greška vezana za odnos slot1Start/slot1End */}
                        {errors.slot1Range && (
                            <p className="form-field__error" style={{ marginTop: "4px" }}>
                                {String((errors.slot1Range as { message?: string }).message)}
                            </p>
                        )}
                    </div>

                    {/* Dugme za drugi termin */}
                    {!hasSecondSlot ? (
                        <button
                            type="button"
                            onClick={() => setValue("hasSecondSlot", true)}
                            style={{
                                alignSelf: "flex-start",
                                padding: "6px 14px",
                                border: "1px dashed #94a3b8",
                                borderRadius: "6px",
                                backgroundColor: "transparent",
                                color: "#64748b",
                                fontSize: "0.85rem",
                                cursor: "pointer"
                            }}
                        >
                            + Dodaj drugi termin
                        </button>
                    ) : (
                        /* Termin 2 */
                        <div>
                            <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: "8px" }}>
                                <p style={{ margin: 0, fontSize: "0.85rem", fontWeight: 600, color: "#374151" }}>
                                    Termin 2
                                    <span style={{ fontWeight: 400, color: "#64748b", marginLeft: "8px" }}>
                                        (npr. za pauzu u radu)
                                    </span>
                                </p>
                                <button
                                    type="button"
                                    onClick={() => {
                                        setValue("hasSecondSlot", false)
                                        setValue("slot2Start", "")
                                        setValue("slot2End", "")
                                    }}
                                    style={{
                                        background: "transparent", border: "none",
                                        color: "#ef4444", cursor: "pointer", fontSize: "0.85rem"
                                    }}
                                >
                                    Ukloni
                                </button>
                            </div>
                            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "12px" }}>
                                <div className="form-field">
                                    <label className="form-field__label" htmlFor="slot2Start">
                                        Početak
                                    </label>
                                    <input
                                        id="slot2Start"
                                        type="time"
                                        style={inputStyle(!!errors.slot2Start)}
                                        {...register("slot2Start")}
                                    />
                                    {errors.slot2Start && (
                                        <p className="form-field__error">{String(errors.slot2Start.message)}</p>
                                    )}
                                </div>
                                <div className="form-field">
                                    <label className="form-field__label" htmlFor="slot2End">
                                        Kraj
                                    </label>
                                    <input
                                        id="slot2End"
                                        type="time"
                                        style={inputStyle(!!errors.slot2End)}
                                        {...register("slot2End")}
                                    />
                                    {errors.slot2End && (
                                        <p className="form-field__error">{String(errors.slot2End.message)}</p>
                                    )}
                                </div>
                            </div>
                            {errors.slot2Range && (
                                <p className="form-field__error" style={{ marginTop: "4px" }}>
                                    {String((errors.slot2Range as { message?: string }).message)}
                                </p>
                            )}
                            {errors.slotsOverlap && (
                                <p className="form-field__error" style={{ marginTop: "4px" }}>
                                    {String((errors.slotsOverlap as { message?: string }).message)}
                                </p>
                            )}
                        </div>
                    )}
                </>
            )}
        </div>
    )
}