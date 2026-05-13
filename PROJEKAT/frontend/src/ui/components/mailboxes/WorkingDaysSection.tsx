import { type UseFormWatch, type UseFormSetValue, type FieldErrors, type FieldValues } from "react-hook-form"
import { MailboxWorkingDays, workingDayBits } from "../../../infrastructure/api/mailboxes/mailboxesApi"

interface Props {
    watch: UseFormWatch<FieldValues>
    setValue: UseFormSetValue<FieldValues>
    errors: FieldErrors<FieldValues>
}

export function WorkingDaysSection({ watch, setValue, errors }: Props) {
    const workingDays: number = watch("workingDays") ?? MailboxWorkingDays.RadniDani

    // Helper: Check if a specific day is selected
    const isDaySelected = (bit: number): boolean => {
        return (workingDays & bit) === bit
    }

    // Helper: Toggle a specific day
    const toggleDay = (bit: number) => {
        const newValue = workingDays ^ bit // XOR to toggle
        setValue("workingDays", newValue, { shouldValidate: true })
    }

    // Helper: Select all days
    const selectAll = () => {
        setValue("workingDays", MailboxWorkingDays.SvakiDan, { shouldValidate: true })
    }

    // Helper: Deselect all days
    const deselectAll = () => {
        setValue("workingDays", MailboxWorkingDays.None, { shouldValidate: true })
    }

    const hasError = !!errors.workingDays

    return (
        <div
            style={{
                border: "1px solid #e2e8f0",
                borderRadius: "8px",
                padding: "16px",
                backgroundColor: "#f8fafc",
                display: "flex",
                flexDirection: "column",
                gap: "16px"
            }}
        >
            <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
                <h3 style={{ margin: 0, fontSize: "0.95rem", fontWeight: 600, color: "#1e2d3d" }}>
                    📅 Radni dani sandučića
                </h3>
                {hasError && (
                    <span style={{ fontSize: "0.85rem", color: "#dc2626" }}>
                        Odaberite barem jedan dan
                    </span>
                )}
            </div>

            {/* Quick actions */}
            <div style={{ display: "flex", gap: "8px" }}>
                <button
                    type="button"
                    onClick={selectAll}
                    style={{
                        padding: "6px 12px",
                        backgroundColor: "#f0f9ff",
                        border: "1px solid #0284c7",
                        borderRadius: "4px",
                        fontSize: "0.85rem",
                        fontWeight: 500,
                        color: "#0284c7",
                        cursor: "pointer",
                        transition: "all 0.2s"
                    }}
                    onMouseEnter={(e) => {
                        (e.currentTarget as HTMLButtonElement).style.backgroundColor = "#0284c7";
                        (e.currentTarget as HTMLButtonElement).style.color = "#fff"
                    }}
                    onMouseLeave={(e) => {
                        (e.currentTarget as HTMLButtonElement).style.backgroundColor = "#f0f9ff";
                        (e.currentTarget as HTMLButtonElement).style.color = "#0284c7"
                    }}
                >
                    ✓ Označi sve
                </button>
                <button
                    type="button"
                    onClick={deselectAll}
                    style={{
                        padding: "6px 12px",
                        backgroundColor: "#fef2f2",
                        border: "1px solid #dc2626",
                        borderRadius: "4px",
                        fontSize: "0.85rem",
                        fontWeight: 500,
                        color: "#dc2626",
                        cursor: "pointer",
                        transition: "all 0.2s"
                    }}
                    onMouseEnter={(e) => {
                        (e.currentTarget as HTMLButtonElement).style.backgroundColor = "#dc2626";
                        (e.currentTarget as HTMLButtonElement).style.color = "#fff"
                    }}
                    onMouseLeave={(e) => {
                        (e.currentTarget as HTMLButtonElement).style.backgroundColor = "#fef2f2";
                        (e.currentTarget as HTMLButtonElement).style.color = "#dc2626"
                    }}
                >
                    ✕ Odznači sve
                </button>
            </div>

            {/* Checkboxes grid */}
            <div
                style={{
                    display: "grid",
                    gridTemplateColumns: "repeat(auto-fit, minmax(140px, 1fr))",
                    gap: "12px"
                }}
            >
                {workingDayBits.map((day) => (
                    <label
                        key={day.name}
                        style={{
                            display: "flex",
                            alignItems: "center",
                            gap: "8px",
                            padding: "10px 12px",
                            backgroundColor: isDaySelected(day.bit) ? "#f0fdf4" : "#fff",
                            border: `2px solid ${isDaySelected(day.bit) ? "#22c55e" : "#e2e8f0"}`,
                            borderRadius: "6px",
                            cursor: "pointer",
                            transition: "all 0.2s",
                            fontWeight: 500,
                            color: "#1e2d3d",
                            fontSize: "0.9rem"
                        }}
                        onMouseEnter={(e) => {
                            (e.currentTarget as HTMLLabelElement).style.backgroundColor = isDaySelected(day.bit)
                                ? "#f0fdf4"
                                : "#f8fafc";
                            (e.currentTarget as HTMLLabelElement).style.borderColor = isDaySelected(day.bit)
                                ? "#16a34a"
                                : "#cbd5e1"
                        }}
                        onMouseLeave={(e) => {
                            (e.currentTarget as HTMLLabelElement).style.backgroundColor = isDaySelected(day.bit)
                                ? "#f0fdf4"
                                : "#fff";
                            (e.currentTarget as HTMLLabelElement).style.borderColor = isDaySelected(day.bit)
                                ? "#22c55e"
                                : "#e2e8f0"
                        }}
                    >
                        <input
                            type="checkbox"
                            checked={isDaySelected(day.bit)}
                            onChange={() => toggleDay(day.bit)}
                            style={{
                                width: "16px",
                                height: "16px",
                                cursor: "pointer",
                                accentColor: "#22c55e"
                            }}
                        />
                        {day.name}
                    </label>
                ))}
            </div>

            {/* Selected days summary */}
            <div
                style={{
                    padding: "10px 12px",
                    backgroundColor: "#f9fafb",
                    border: "1px solid #e5e7eb",
                    borderRadius: "4px",
                    fontSize: "0.85rem",
                    color: "#6b7280"
                }}
            >
                {workingDays === MailboxWorkingDays.None ? (
                    <span style={{ color: "#dc2626", fontWeight: 500 }}>
                        ⚠️ Nema odabranih dana
                    </span>
                ) : workingDays === MailboxWorkingDays.SvakiDan ? (
                    <span>✅ Sandučić je dostupan svakog dana</span>
                ) : workingDays === MailboxWorkingDays.RadniDani ? (
                    <span>✅ Radni dani (Pon-Pet)</span>
                ) : workingDays === MailboxWorkingDays.Vikend ? (
                    <span>✅ Vikend (Sub-Ned)</span>
                ) : (
                    <span>
                        ✅ Odabrani dani:{" "}
                        {workingDayBits
                            .filter((d) => isDaySelected(d.bit))
                            .map((d) => d.name)
                            .join(", ")}
                    </span>
                )}
            </div>
        </div>
    )
}
