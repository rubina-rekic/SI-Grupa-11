import React from 'react';
import type { RouteItemResponse } from '../../../infrastructure/api/routesApi';
import './RouteItemList.css';

interface RouteItemListProps {
    items: RouteItemResponse[];
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

const formatTime = (timeString: string): string => {
    // TimeOnly typically comes as HH:mm:ss format
    if (!timeString) return '--:--';
    const parts = timeString.split(':');
    if (parts.length >= 2) {
        return `${parts[0]}:${parts[1]}`;
    }
    return timeString;
};

export const RouteItemList: React.FC<RouteItemListProps> = ({ items }) => {
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
                {items.map((item, index) => (
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
                            </div>

                            <div className="item-details">
                                <span className="estimated-time">
                                    <span className="time-icon">🕐</span>
                                    {formatTime(item.estimatedArrivalTime)}
                                </span>
                            </div>
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
                ))}
            </div>
        </div>
    );
};
