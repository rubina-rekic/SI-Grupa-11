import { useEffect, useState } from "react"
import { toast } from "sonner"
import { Layout } from "../../components/Layout/Layout"
import {
    getMailboxHistory,
    getAllMailboxes,
    type AuditLogDto,
    type MailboxResponse
} from "../../../infrastructure/api/mailboxes/mailboxesApi"

const priorityLabels: Record<string, string> = {
    "1": "🔴 Visok",
    "2": "🟡 Srednji",
    "3": "🟢 Nizak"
}

const priorityStyles: Record<string, { background: string; color: string }> = {
    "1": {
        background: "#fee2e2",
        color: "#b91c1c"
    },
    "2": {
        background: "#fef3c7",
        color: "#92400e"
    },
    "3": {
        background: "#dcfce7",
        color: "#166534"
    }
}

interface HistoryEntry extends AuditLogDto {
    mailboxSerial: string
    mailboxAddress: string
}

export default function MailboxHistoryPage() {
    const [entries, setEntries] = useState<HistoryEntry[]>([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        const load = async () => {
            try {
                const mailboxes = await getAllMailboxes({ pageSize: 100 })

                const allLogs: HistoryEntry[] = []

                await Promise.all(
                    mailboxes.items.map(async (mailbox: MailboxResponse) => {
                        const result = await getMailboxHistory(mailbox.id)

                        if (result.data) {
                            console.log(result.data)

                            const priorityLogs = result.data
                                .filter(log => log.fieldName === "Priority")
                                .map(log => ({
                                    ...log,
                                    mailboxSerial: mailbox.serialNumber,
                                    mailboxAddress: mailbox.address
                                }))

                            allLogs.push(...priorityLogs)
                        }
                    })
                )

                allLogs.sort(
                    (a, b) =>
                        new Date(b.timestamp).getTime() -
                        new Date(a.timestamp).getTime()
                )

                setEntries(allLogs)
            } catch (error) {
                console.error("Response error:", error)

                toast.error(
                    error instanceof Error
                        ? error.message
                        : "Greška pri učitavanju historije"
                )
            } finally {
                setLoading(false)
            }
        }

        void load()
    }, [])

    if (loading) {
        return (
            <Layout>
                <div className="page-container">
                    <div
                        className="form-card"
                        style={{
                            padding: "40px",
                            textAlign: "center"
                        }}
                    >
                        <p
                            style={{
                                color: "#64748b",
                                fontSize: "1rem"
                            }}
                        >
                            Učitavanje historije...
                        </p>
                    </div>
                </div>
            </Layout>
        )
    }

    return (
        <Layout>
            <div className="page-container">
                <div className="form-card">
                    {/* Header */}
                    <div
                        className="form-card__header"
                        style={{
                            marginBottom: "24px"
                        }}
                    >
                        <h1
                            className="form-card__title"
                            style={{
                                fontSize: "1.8rem",
                                marginBottom: "8px"
                            }}
                        >
                            📜 Historija promjena prioriteta
                        </h1>

                        <p
                            className="form-card__subtitle"
                            style={{
                                color: "#64748b"
                            }}
                        >
                            Pregled svih promjena prioriteta sandučića i razloga
                            izmjena.
                        </p>
                    </div>

                    <div className="form-card__body">
                        {entries.length === 0 ? (
                            <div
                                style={{
                                    textAlign: "center",
                                    padding: "50px",
                                    backgroundColor: "#f8fafc",
                                    borderRadius: "14px",
                                    border: "1px solid #e2e8f0",
                                    color: "#64748b"
                                }}
                            >
                                Nema zabilježenih promjena prioriteta.
                            </div>
                        ) : (
                            <div
                                style={{
                                    display: "flex",
                                    flexDirection: "column",
                                    gap: "18px"
                                }}
                            >
                                {entries.map((entry) => {
                                    const oldStyle =
                                        priorityStyles[
                                        entry.oldValue ?? ""
                                        ] || {
                                            background: "#e2e8f0",
                                            color: "#334155"
                                        }

                                    const newStyle =
                                        priorityStyles[
                                        entry.newValue ?? ""
                                        ] || {
                                            background: "#e2e8f0",
                                            color: "#334155"
                                        }

                                    return (
                                        <div
                                            key={entry.id}
                                            style={{
                                                background: "#ffffff",
                                                border:
                                                    "1px solid #e2e8f0",
                                                borderRadius: "16px",
                                                padding: "20px",
                                                boxShadow:
                                                    "0 2px 10px rgba(0,0,0,0.04)",
                                                transition: "all 0.2s ease"
                                            }}
                                        >
                                            {/* Top section */}
                                            <div
                                                style={{
                                                    display: "flex",
                                                    justifyContent:
                                                        "space-between",
                                                    alignItems: "flex-start",
                                                    gap: "16px",
                                                    marginBottom: "16px",
                                                    flexWrap: "wrap"
                                                }}
                                            >
                                                <div>
                                                    <div
                                                        style={{
                                                            fontWeight: 700,
                                                            fontSize: "1rem",
                                                            color: "#1e293b",
                                                            marginBottom:
                                                                "6px"
                                                        }}
                                                    >
                                                        📮{" "}
                                                        {entry.mailboxSerial}
                                                    </div>

                                                    <div
                                                        style={{
                                                            color: "#64748b",
                                                            fontSize: "0.9rem",
                                                            lineHeight: 1.4
                                                        }}
                                                    >
                                                        {
                                                            entry.mailboxAddress
                                                        }
                                                    </div>
                                                </div>

                                                <div
                                                    style={{
                                                        fontSize: "0.8rem",
                                                        color: "#94a3b8",
                                                        whiteSpace: "nowrap"
                                                    }}
                                                >
                                                    {new Date(
                                                        entry.timestamp
                                                    ).toLocaleString("bs-BA")}
                                                </div>
                                            </div>

                                            {/* Priority section */}
                                            <div
                                                style={{
                                                    display: "flex",
                                                    alignItems: "center",
                                                    gap: "12px",
                                                    flexWrap: "wrap",
                                                    marginBottom: "16px"
                                                }}
                                            >
                                                <span
                                                    style={{
                                                        ...oldStyle,
                                                        padding:
                                                            "6px 14px",
                                                        borderRadius:
                                                            "999px",
                                                        fontSize: "0.82rem",
                                                        fontWeight: 600
                                                    }}
                                                >
                                                    {priorityLabels[
                                                        entry.oldValue ??
                                                        ""
                                                    ] ??
                                                        entry.oldValue}
                                                </span>

                                                <span
                                                    style={{
                                                        color: "#94a3b8",
                                                        fontWeight: 700,
                                                        fontSize: "1rem"
                                                    }}
                                                >
                                                    →
                                                </span>

                                                <span
                                                    style={{
                                                        ...newStyle,
                                                        padding:
                                                            "6px 14px",
                                                        borderRadius:
                                                            "999px",
                                                        fontSize: "0.82rem",
                                                        fontWeight: 600
                                                    }}
                                                >
                                                    {priorityLabels[
                                                        entry.newValue ??
                                                        ""
                                                    ] ??
                                                        entry.newValue}
                                                </span>

                                                <span
                                                    style={{
                                                        marginLeft: "auto",
                                                        fontSize: "0.85rem",
                                                        color: "#475569"
                                                    }}
                                                >
                                                    👤 {entry.username}
                                                </span>
                                            </div>

                                            {/* Reason */}
                                            <div
                                                style={{
                                                    background:
                                                        "#f8fafc",
                                                    border:
                                                        "1px solid #e2e8f0",
                                                    borderRadius: "12px",
                                                    padding: "14px"
                                                }}
                                            >
                                                <div
                                                    style={{
                                                        fontWeight: 600,
                                                        fontSize: "0.85rem",
                                                        color: "#334155",
                                                        marginBottom:
                                                            "6px"
                                                    }}
                                                >
                                                    💬 Razlog izmjene
                                                </div>

                                                <div
                                                    style={{
                                                        color: "#475569",
                                                        fontSize: "0.92rem",
                                                        lineHeight: 1.5
                                                    }}
                                                >
                                                    {entry.reason?.trim() ||
                                                        "Nije naveden razlog izmjene."}
                                                </div>
                                            </div>
                                        </div>
                                    )
                                })}
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </Layout>
    )
}