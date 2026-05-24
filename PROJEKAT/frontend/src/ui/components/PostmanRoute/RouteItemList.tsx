import React, { useState } from 'react';
import type { RouteItemResponse } from '../../../infrastructure/api/routesApi';
import { MailboxStatus } from '../../../infrastructure/api/mailboxes/mailboxesApi';
import {
    getMailboxStatusLabel,
    getVisitStatusClass,
    getVisitStatusIcon,
    getVisitStatusLabel,
    isProcessedMailboxStatus,
    isRouteItemProcessed,
    STATUS_ALREADY_RECORDED_MESSAGE,
} from './statusUtils';
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

const getMailboxStatusBadge = (mailboxStatus: string): { label: string; bg: string; fg: string } => {
    switch (getMailboxStatusLabel(mailboxStatus).toLowerCase()) {
        case 'pun':      return { label: 'Pun',       bg: '#fee2e2', fg: '#b91c1c' };
        case 'napunjen': return { label: 'Napunjen',  bg: '#ffedd5', fg: '#9a3412' };
        case 'ispraznjen': return { label: 'Ispraznjen', bg: '#eff6ff', fg: '#1d4ed8' };
        case 'obrađen':  return { label: 'Obrađen',   bg: '#dcfce7', fg: '#166534' };
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

const formatDateTime = (dateString?: string | null): string => {
    if (!dateString) return '';
    const date = new Date(dateString);
    if (Number.isNaN(date.getTime())) return '';
    return date.toLocaleString('bs-BA', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
    });
};

const STATUS_BUTTONS: { label: string; value: number; activeColor: string }[] = [
    { label: 'Napunjen', value: MailboxStatus.Napunjen, activeColor: '#9a3412' },
    { label: 'Ispraznjen', value: MailboxStatus.Ispraznjen, activeColor: '#1d4ed8' },
];

export const RouteItemList: React.FC<RouteItemListProps> = ({ items, onStatusChange }) => {
    const [loadingIds, setLoadingIds] = useState<Set<string>>(new Set());

    const handleStatusClick = async (item: RouteItemResponse, status: number) => {
        if (!onStatusChange || loadingIds.has(item.mailboxId) || isRouteItemProcessed(item)) return;
        const mailboxId = item.mailboxId;
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
                    const currentMailboxStatus = item.processedStatus ?? item.mailboxStatus ?? '';
                    const badge = getMailboxStatusBadge(currentMailboxStatus);
                    const visitClass = getVisitStatusClass(item);
                    const isProcessed = isRouteItemProcessed(item);
                    const processedAt = formatDateTime(item.processedAt);

                    return (
                        <div
                            key={item.id}
                            className={`route-item ${visitClass}`}
                            data-status={visitClass}
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
                                        {isProcessed ? (
                                            <div className="mailbox-status-locked">
                                                <span>{STATUS_ALREADY_RECORDED_MESSAGE}</span>
                                                {processedAt && <span> Evidentirano: {processedAt}</span>}
                                            </div>
                                        ) : (
                                            <>
                                                <span className="mailbox-status-actions-label">Označi kao obrađen:</span>
                                                {STATUS_BUTTONS.map(btn => {
                                                    const isActive = isProcessedMailboxStatus(currentMailboxStatus)
                                                        && getMailboxStatusLabel(currentMailboxStatus) === btn.label;
                                                    return (
                                                        <button
                                                            key={btn.value}
                                                            className={`mailbox-status-btn${isActive ? ' mailbox-status-btn--active' : ''}`}
                                                            style={isActive ? { backgroundColor: btn.activeColor, color: '#fff', borderColor: btn.activeColor } : undefined}
                                                            disabled={isLoading}
                                                            onClick={() => void handleStatusClick(item, btn.value)}
                                                            title={`Postavi status: ${btn.label}`}
                                                        >
                                                            {isLoading ? '...' : btn.label}
                                                        </button>
                                                    );
                                                })}
                                            </>
                                        )}
                                    </div>
                                )}
                            </div>

                            <div className="item-status">
                                <div
                                    className={`status-indicator status-${visitClass}`}
                                    title={getVisitStatusLabel(item)}
                                >
                                    {getVisitStatusIcon(item)}
                                </div>
                                <span className="status-label">{getVisitStatusLabel(item)}</span>
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
};
