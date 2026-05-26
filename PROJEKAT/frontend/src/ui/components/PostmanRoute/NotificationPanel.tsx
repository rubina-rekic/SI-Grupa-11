import { useEffect, useRef, useState } from "react"
import {
    getMyNotifications,
    markNotificationRead
} from "../../../infrastructure/api/issues/issuesApi"
import type { IssueNotificationDto } from "../../../infrastructure/api/issues/issuesApi"
import { useNavigate } from "react-router-dom"
import "./NotificationPanel.css"

export function NotificationPanel() {
    const [notifications, setNotifications] = useState<IssueNotificationDto[]>([])
    const [open, setOpen] = useState(false)
    const navigate = useNavigate()
    const panelRef = useRef<HTMLDivElement>(null)

    useEffect(() => {
        const load = () => {
            getMyNotifications().then(setNotifications).catch(() => {})
        }
        load()
        const interval = setInterval(load, 30_000)
        return () => clearInterval(interval)
    }, [])

    useEffect(() => {
        const handleOutsideClick = (e: MouseEvent) => {
            if (panelRef.current && !panelRef.current.contains(e.target as Node)) {
                setOpen(false)
            }
        }
        if (open) document.addEventListener("mousedown", handleOutsideClick)
        return () => document.removeEventListener("mousedown", handleOutsideClick)
    }, [open])

    const unreadCount = notifications.filter(n => !n.isRead).length

    const handleClick = async (n: IssueNotificationDto) => {
        if (!n.isRead) {
            await markNotificationRead(n.id)
            setNotifications(prev => prev.map(x => x.id === n.id ? { ...x, isRead: true } : x))
        }
        setOpen(false)
        navigate(`/worker/issues/${n.issueId}`)
    }

    return (
        <div className="notif-panel" ref={panelRef}>
            <button className="notif-bell" onClick={() => setOpen(o => !o)}>
                🔔
                {unreadCount > 0 && (
                    <span className="notif-badge">{unreadCount}</span>
                )}
            </button>

            {open && (
                <div className="notif-dropdown">
                    <div className="notif-header">
                        <span className="notif-header-title">Obavijesti</span>
                        {unreadCount > 0 && (
                            <span className="notif-header-count">{unreadCount} novo</span>
                        )}
                        <button className="notif-close" onClick={() => setOpen(false)}>✕</button>
                    </div>

                    {notifications.length === 0 ? (
                        <div className="notif-empty">
                            <span className="notif-empty-icon">🔕</span>
                            <p>Nema novih obavijesti.</p>
                        </div>
                    ) : (
                        <div className="notif-list">
                            {notifications.map(n => (
                                <button
                                    key={n.id}
                                    className={`notif-item${n.isRead ? "" : " notif-item--unread"}`}
                                    onClick={() => handleClick(n)}
                                >
                                    {!n.isRead && <span className="notif-dot" />}
                                    <div className="notif-item-body">
                                        <span className="notif-item-title">{n.title}</span>
                                        <span className="notif-item-msg">{n.message}</span>
                                        <span className="notif-item-time">
                                            {new Date(n.createdAt).toLocaleTimeString("bs-BA", {
                                                hour: "2-digit", minute: "2-digit"
                                            })}
                                        </span>
                                    </div>
                                </button>
                            ))}
                        </div>
                    )}
                </div>
            )}
        </div>
    )
}