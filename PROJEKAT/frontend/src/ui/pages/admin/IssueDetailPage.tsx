import { useCallback, useEffect, useState } from "react"
import { useLocation, useNavigate, useParams, useSearchParams } from "react-router-dom"
import { toast } from "sonner"
import { Layout } from "../../components/Layout/Layout"
import {
    getAllIssues, getIssueById, addComment, assignAction, resolveIssue,
    IssueStatus, IssueAction, issueActionLabels, issueStatusLabels,
    getIssueByIdForWorker,
} from "../../../infrastructure/api/issues/issuesApi"
import type { IssueDto } from "../../../infrastructure/api/issues/issuesApi"
import { getUsers } from "../../../infrastructure/api/users/usersApi"
import type { UserListDto } from "../../../infrastructure/api/users/usersApi"
import "./IssueDetailPage.css"

function StatusBadge({ status }: { status: IssueStatus }) {
    const cls = status === IssueStatus.Otvoren
        ? "issue-badge issue-badge--open"
        : status === IssueStatus.UObradi
            ? "issue-badge issue-badge--inprogress"
            : "issue-badge issue-badge--resolved"
    return <span className={cls}>{issueStatusLabels[status]}</span>
}

function TimelineIcon({ type }: { type: string }) {
    if (type === "created") return <span className="tl-icon tl-icon--created">📍</span>
    if (type === "comment") return <span className="tl-icon tl-icon--comment">💬</span>
    if (type === "action") return <span className="tl-icon tl-icon--action">⚡</span>
    return <span className="tl-icon tl-icon--status">🔄</span>
}

function formatDateTime(value: string) {
    return new Date(value).toLocaleString("bs-BA", {
        day: "2-digit", month: "2-digit", year: "numeric",
        hour: "2-digit", minute: "2-digit"
    })
}

export default function IssueDetailPage() {
    const { id } = useParams<{ id: string }>()
    const [searchParams] = useSearchParams()
    const routeItemId = searchParams.get("routeItemId")
    const location = useLocation()
    const navigate = useNavigate()
    const isPostmanView = location.pathname.startsWith("/worker/")

    const [issue, setIssue] = useState<IssueDto | null>(null)
    const [loading, setLoading] = useState(true)
    const [comment, setComment] = useState("")
    const [submittingComment, setSubmittingComment] = useState(false)
    const [selectedAction, setSelectedAction] = useState<IssueAction | "">("")
    const [targetPostmanId, setTargetPostmanId] = useState("")
    const [postmen, setPostmen] = useState<UserListDto[]>([])
    const [submittingAction, setSubmittingAction] = useState(false)
    const [resolving, setResolving] = useState(false)

    const loadIssue = useCallback(async () => {
        if (id) {
            try {
                const data = isPostmanView
                 ? await getIssueByIdForWorker(id)
                : await getIssueById(id)
                setIssue(data)
            } catch {
                toast.error("Nije moguće učitati detalje problema.")
            } finally {
                setLoading(false)
            }
            return
        }

        if (routeItemId) {
            try {
                const issues = await getAllIssues()
                const issueSummary = issues.find(issue => issue.routeItemId === routeItemId)
                if (!issueSummary) {
                    toast.error("Problem nije pronađen za odabranu lokaciju.")
                    setLoading(false)
                    return
                }
                navigate(`/admin/issues/${issueSummary.id}`, { replace: true })
            } catch {
                toast.error("Nije moguće učitati detalje problema.")
                setLoading(false)
            }
            return
        }

        setLoading(false)
    }, [id, routeItemId, navigate])

    useEffect(() => { loadIssue() }, [loadIssue])

    useEffect(() => {
        getUsers().then(res => {
            if (res.data) setPostmen(res.data.filter(u => u.role === "PostalWorker"))
        })
    }, [])

    const handleAddComment = async () => {
        if (!id || !comment.trim()) return
        setSubmittingComment(true)
        try {
            const updated = await addComment(id, comment.trim())
            setIssue(updated)
            setComment("")
            toast.success("Komentar dodan.")
        } catch (err) {
            console.error("Add comment failed:", err)
            toast.error(err instanceof Error ? err.message : "Greška pri dodavanju komentara.")
        } finally {
            setSubmittingComment(false)
        }
    }

    const handleAssignAction = async () => {
        if (!id || selectedAction === "") return
        setSubmittingAction(true)
        try {
            const updated = await assignAction(
                id,
                selectedAction as IssueAction,
                selectedAction === IssueAction.DrugiPostar ? targetPostmanId : undefined
            )
            setIssue(updated)
            setSelectedAction("")
            setTargetPostmanId("")
            toast.success("Akcija dodijeljena.")
        } catch (err) {
            console.error("Assign action failed:", err)
            toast.error(err instanceof Error ? err.message : "Greška pri dodjeli akcije.")
        } finally {
            setSubmittingAction(false)
        }
    }

    const handleResolve = async () => {
        if (!id) return
        setResolving(true)
        try {
            const updated = await resolveIssue(id)
            setIssue(updated)
            toast.success("Problem označen kao riješen.")
        } catch (err) {
            console.error("Resolve issue failed:", err)
            toast.error(err instanceof Error ? err.message : "Greška pri rješavanju problema.")
        } finally {
            setResolving(false)
        }
    }

    if (loading) return (
        <Layout>
            <div className="issue-detail-loading">
                <div className="issue-spinner" />
                <p>Učitavanje...</p>
            </div>
        </Layout>
    )

    if (!issue) return (
        <Layout>
            <div className="issue-detail-loading">
                <p>Problem nije pronađen.</p>
            </div>
        </Layout>
    )

    const isResolved = issue.status === IssueStatus.Rijesen

    return (
        <Layout>
            <div className="issue-detail-page">

                {/* Header */}
                <div className="issue-detail-header">
                    <button className="issue-back-btn" onClick={() => navigate(-1)}>← Nazad</button>
                    <div className="issue-detail-title-row">
                        <div>
                            <h1 className="issue-detail-title">Problematična lokacija</h1>
                            <p className="issue-detail-subtitle">{issue.mailboxAddress}</p>
                        </div>
                        <StatusBadge status={issue.status} />
                    </div>
                </div>

                <div className="issue-detail-body">

                    {/* Info panel */}
                    <div className="issue-info-panel">
                        <h2 className="issue-section-title">Informacije o problemu</h2>
                        <div className="issue-info-grid">
                            <div className="issue-info-item">
                                <span className="issue-info-label">Adresa</span>
                                <span className="issue-info-value">{issue.mailboxAddress}</span>
                            </div>
                            <div className="issue-info-item">
                                <span className="issue-info-label">Serijski broj</span>
                                <span className="issue-info-value">{issue.mailboxSerialNumber}</span>
                            </div>
                            <div className="issue-info-item">
                                <span className="issue-info-label">Prijavio</span>
                                <span className="issue-info-value">{issue.reportedByUsername}</span>
                            </div>
                            <div className="issue-info-item">
                                <span className="issue-info-label">Datum prijave</span>
                                <span className="issue-info-value">{formatDateTime(issue.createdAt)}</span>
                            </div>
                            <div className="issue-info-item issue-info-item--full">
                                <span className="issue-info-label">Razlog nedostupnosti</span>
                                <span className="issue-info-value issue-info-value--reason">
                                    {issue.unavailableReason ?? "Nije naveden"}
                                </span>
                            </div>
                            {issue.assignedAction !== null && (
                                <div className="issue-info-item issue-info-item--full">
                                    <span className="issue-info-label">Aktuelna akcija</span>
                                    <span className="issue-info-value issue-info-value--action">
                                        {issue.assignedActionLabel}
                                        {issue.actionAssignedToUsername && ` → ${issue.actionAssignedToUsername}`}
                                    </span>
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Action panel */}
                    {!isResolved && !isPostmanView && (
                        <div className="issue-action-panel">
                            <h2 className="issue-section-title">Dodjela akcije</h2>
                            <div className="issue-action-form">
                                <select
                                    className="issue-select"
                                    value={selectedAction}
                                    onChange={e => {
                                        setSelectedAction(e.target.value === "" ? "" : Number(e.target.value) as IssueAction)
                                        setTargetPostmanId("")
                                    }}
                                >
                                    <option value="">Odaberi akciju...</option>
                                    {Object.entries(issueActionLabels).map(([key, label]) => (
                                        <option key={key} value={key}>{label}</option>
                                    ))}
                                </select>

                                {selectedAction === IssueAction.DrugiPostar && (
                                    <select
                                        className="issue-select"
                                        value={targetPostmanId}
                                        onChange={e => setTargetPostmanId(e.target.value)}
                                    >
                                        <option value="">Odaberi poštara...</option>
                                        {postmen.map(p => (
                                            <option key={p.id} value={p.id}>{p.username}</option>
                                        ))}
                                    </select>
                                )}

                                <button
                                    className="issue-btn issue-btn--primary"
                                    onClick={handleAssignAction}
                                    disabled={
                                        submittingAction ||
                                        selectedAction === "" ||
                                        (selectedAction === IssueAction.DrugiPostar && !targetPostmanId)
                                    }
                                >
                                    {submittingAction ? "Dodjela u toku..." : "Dodijeli akciju"}
                                </button>

                                <button
                                    className="issue-btn issue-btn--resolve"
                                    onClick={handleResolve}
                                    disabled={resolving}
                                >
                                    {resolving ? "Rješavanje..." : "Označi kao riješen"}
                                </button>
                            </div>
                        </div>
                    )}

                    {/* Comment thread */}
                    <div className="issue-comments-panel">
                        <h2 className="issue-section-title">Komunikacija</h2>

                        {issue.comments.length === 0 ? (
                            <p className="issue-no-comments">Nema komentara. Budite prvi koji će ostaviti komentar.</p>
                        ) : (
                            <div className="issue-comment-thread">
                                {issue.comments.map(c => (
                                    <div key={c.id} className={`issue-comment issue-comment--${c.authorRole === "PostalWorker" ? "postman" : "dispatcher"}`}>
                                        <div className="issue-comment-header">
                                            <span className="issue-comment-author">{c.authorUsername}</span>
                                            <span className="issue-comment-role">
                                                {c.authorRole === "PostalWorker" ? "Poštar" : "Dispečer"}
                                            </span>
                                            <span className="issue-comment-time">{formatDateTime(c.createdAt)}</span>
                                        </div>
                                        <p className="issue-comment-content">{c.content}</p>
                                    </div>
                                ))}
                            </div>
                        )}

                        {!isResolved && (
                            <div className="issue-comment-form">
                                <textarea
                                    className="issue-textarea"
                                    placeholder="Napišite komentar ili instrukciju..."
                                    rows={3}
                                    value={comment}
                                    onChange={e => setComment(e.target.value)}
                                />
                                <button
                                    className="issue-btn issue-btn--primary"
                                    onClick={handleAddComment}
                                    disabled={submittingComment || !comment.trim()}
                                >
                                    {submittingComment ? "Slanje..." : "Pošalji komentar"}
                                </button>
                            </div>
                        )}
                    </div>

                    {/* Timeline */}
                    <div className="issue-timeline-panel">
                        <h2 className="issue-section-title">Historija aktivnosti</h2>
                        <div className="issue-timeline">
                            {issue.timeline.map((entry, idx) => (
                                <div key={idx} className="issue-timeline-entry">
                                    <TimelineIcon type={entry.type} />
                                    <div className="issue-timeline-content">
                                        <p className="issue-timeline-desc">{entry.description}</p>
                                        <div className="issue-timeline-meta">
                                            {entry.actorUsername && <span>{entry.actorUsername}</span>}
                                            <span>{formatDateTime(entry.timestamp)}</span>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>

                </div>
            </div>
        </Layout>
    )
}