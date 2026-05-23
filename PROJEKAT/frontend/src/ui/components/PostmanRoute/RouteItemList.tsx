import React, { useState } from 'react';
import type { RouteItemResponse } from '../../../infrastructure/api/routesApi';
import './RouteItemList.css';

interface RouteItemListProps {
    items: RouteItemResponse[];
    onStatusChange?: (mailboxId: string, status: number) => Promise<void>;
}

const getPriorityColor = (priority: string): string => {
    const priorityLower = priority.toLowerCase();
    if (priorityLower === 'visok') return '#a32d2d';
    if (priorityLower === 'srednji') return '#854f0b';
    if (priorityLower === 'nizak') return '#3b6d11';
    return '#64748b';
};

const getStatusIcon = (status: string): string => {
    const statusLower = status.toLowerCase();
    if (statusLower === 'obrađen' || statusLower === 'obrađeno') return '✓';
    if (statusLower === 'nedostupan') return '✗';
    return '◯';
};

const getStatusLabel = (status: string): string => {
    const statusLower = status.toLowerCase();
    if (statusLower === 'obrađen' || statusLower === 'obrađeno') return 'Obrađen';
    if (statusLower === 'nedostupan') return 'Nedostupan';
    if (statusLower === 'planirano') return 'Čeka';
    return status;
};

const getMailboxStatusBadge = (mailboxStatus: string): { label: string; bg: string; fg: string } => {
    switch (mailboxStatus.toLowerCase()) {
        case 'pun':      return { label: 'Pun',       bg: '#fee2e2', fg: '#b91c1c' };
        case 'obraen':   return { label: 'Obrađen',   bg: '#dcfce7', fg: '#166534' };
        case 'napunjen': return { label: 'Napunjen',  bg: '#ffedd5', fg: '#9a3412' };
        case 'ispraznjen': return { label: 'Ispraznjen', bg: '#eff6ff', fg: '#1d4ed8' };
        default:         return { label: 'Prazan',    bg: '#f0fdf4', fg: '#15803d' };
    }
};

const formatTime = (timeString: string): string => {
    if (!timeString) return '--:--';
    const parts = timeString.split(':');
    if (parts.length >= 2) {
        return `${parts[0]}:${parts[1]}`;
    }
    return timeString;
};

const STATUS_BUTTONS: { label: string; value: number; activeColor: string }[] = [
    { label: 'Obrađen',    value: 2, activeColor: '#166534' },
    { label: 'Napunjen',   value: 3, activeColor: '#9a3412' },
    { label: 'Ispraznjen', value: 4, activeColor: '#1d4ed8' },
];

export const RouteItemList: React.FC<RouteItemListProps> = ({ items, onStatusChange }) => {
    const [loadingIds, setLoadingIds] = useState<Set<string>>(new Set());

    const handleStatusClick = async (mailboxId: string, status: number) => {
        if (!onStatusChange || loadingIds.has(mailboxId)) return;
        setLoadingIds(prev => new Set(prev).add(mailboxId));
        try {
            await onStatusChange(mailboxId, status);
        } finally {
            setLoadingIds(prev => {
                const next = new Set(prev);
                next.delete(mailboxId);
                return next;
            });
        }
    };

    if (items.length === 0) {
        return (
            <div className="route-item-list">
                <div className="empty-state">
                    <p>Nema sandučića u ruti</p>
                </div>
            </div>
        );
    }

    return (
        <div className="route-item-list">
            <div className="list-header">
                <h3>
                    <span className="list-header-icon">📋</span>
                    Sandučići
                </h3>
                <span className="item-count">{items.length} stavki</span>
            </div>

            <div className="items-container">
                {items.map((item, index) => {
                    const isLoading = loadingIds.has(item.mailboxId);
                    const badge = getMailboxStatusBadge(item.mailboxStatus ?? '');

                    return (
                        <div
                            key={item.id}
                            className={`route-item ${item.status.toLowerCase()}`}
                            data-status={item.status}
                        >
                            <div className="item-number">{index + 1}</div>

                            <div className="item-content">
                                <div className="item-header">
                                    <span className="item-address">{item.address}</span>
                                    <span
                                        className="priority-badge"
                                        style={{ backgroundColor: getPriorityColor(item.priority) }}
                                        title={`Prioritet: ${item.priority}`}
                                    >
                                        {item.priority.charAt(0).toUpperCase()}
                                    </span>
                                    <span
                                        className="mailbox-status-badge"
                                        style={{ backgroundColor: badge.bg, color: badge.fg }}
                                    >
                                        {badge.label}
                                    </span>
                                </div>

                                <div className="item-details">
                                    <span className="estimated-time">
                                        <span className="time-icon">🕐</span>
                                        {formatTime(item.estimatedArrivalTime)}
                                    </span>
                                </div>

                                {onStatusChange && (
                                    <div className="mailbox-status-actions">
                                        {STATUS_BUTTONS.map(btn => {
                                            const isActive = item.mailboxStatus?.toLowerCase() === btn.label.toLowerCase()
                                                || (btn.label === 'Obrađen' && item.mailboxStatus?.toLowerCase() === 'obraen');
                                            return (
                                                <button
                                                    key={btn.value}
                                                    className={`mailbox-status-btn${isActive ? ' mailbox-status-btn--active' : ''}`}
                                                    style={isActive ? { backgroundColor: btn.activeColor, color: '#fff', borderColor: btn.activeColor } : undefined}
                                                    disabled={isLoading}
                                                    onClick={() => void handleStatusClick(item.mailboxId, btn.value)}
                                                    title={`Postavi status: ${btn.label}`}
                                                >
                                                    {isLoading ? '...' : btn.label}
                                                </button>
                                            );
                                        })}
                                    </div>
                                )}
                            </div>

                            <div className="item-status">
                                <div
                                    className={`status-indicator status-${item.status.toLowerCase()}`}
                                    title={getStatusLabel(item.status)}
                                >
                                    {getStatusIcon(item.status)}
                                </div>
                                <span className="status-label">{getStatusLabel(item.status)}</span>
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
};
