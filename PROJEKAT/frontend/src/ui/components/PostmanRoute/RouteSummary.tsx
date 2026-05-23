import React from 'react';
import type { RouteResponse } from '../../../infrastructure/api/routesApi';
import './RouteSummary.css';

interface RouteSummaryProps {
    route: RouteResponse;
}

/* ── SVG Icons ───────────────────────────────────────────── */

const IconBarChart = () => (
    <svg width="15" height="15" viewBox="0 0 16 16" fill="none">
        <rect x="1" y="9" width="3" height="6" rx="1" stroke="currentColor" strokeWidth="1.4"/>
        <rect x="6.5" y="5" width="3" height="10" rx="1" stroke="currentColor" strokeWidth="1.4"/>
        <rect x="12" y="1" width="3" height="14" rx="1" stroke="currentColor" strokeWidth="1.4"/>
    </svg>
);

const IconMailbox = () => (
    <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
        <rect x="1.5" y="4" width="13" height="9" rx="2" stroke="currentColor" strokeWidth="1.4"/>
        <path d="M1.5 7h4l2 2 2-2h4" stroke="currentColor" strokeWidth="1.4" strokeLinejoin="round"/>
        <path d="M6 2h4" stroke="currentColor" strokeWidth="1.4" strokeLinecap="round"/>
    </svg>
);

const IconCheck = () => (
    <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
        <circle cx="8" cy="8" r="6.5" stroke="currentColor" strokeWidth="1.4"/>
        <path d="M5 8l2 2 4-4" stroke="currentColor" strokeWidth="1.4" strokeLinecap="round" strokeLinejoin="round"/>
    </svg>
);

const IconSkip = () => (
    <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
        <path d="M3 3.5l6 4.5-6 4.5V3.5z" stroke="currentColor" strokeWidth="1.4" strokeLinejoin="round"/>
        <path d="M13 3.5v9" stroke="currentColor" strokeWidth="1.4" strokeLinecap="round"/>
    </svg>
);

const IconClock = () => (
    <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
        <circle cx="8" cy="8" r="6.5" stroke="currentColor" strokeWidth="1.4"/>
        <path d="M8 4.5V8l2.5 2" stroke="currentColor" strokeWidth="1.4" strokeLinecap="round" strokeLinejoin="round"/>
    </svg>
);

/* ────────────────────────────────────────────────────────── */

export const RouteSummary: React.FC<RouteSummaryProps> = ({ route }) => {
    const totalMailboxes = route.routeItems.length;
    const processedMailboxes = route.routeItems.filter(
        (item) => item.status === 'Obrađen' || item.status === 'Obrađeno'
    ).length;
    const skippedMailboxes = route.routeItems.filter(
        (item) => item.status === 'Nedostupan'
    ).length;
    const completionPercentage =
        totalMailboxes > 0 ? Math.round((processedMailboxes / totalMailboxes) * 100) : 0;

    const remainingMailboxes = totalMailboxes - processedMailboxes;
    const estimatedMinutesPerMailbox =
        route.totalDurationMinutes > 0
            ? Math.ceil(route.totalDurationMinutes / totalMailboxes)
            : 5;
    const estimatedRemainingMinutes = remainingMailboxes * estimatedMinutesPerMailbox;

    const formatTime = (minutes: number) => {
        if (minutes < 60) return `${minutes}m`;
        const hours = Math.floor(minutes / 60);
        const mins = minutes % 60;
        return mins > 0 ? `${hours}h ${mins}m` : `${hours}h`;
    };

    return (
        <div className="route-summary">
            <div className="summary-header">
                <h2>
                    <span className="summary-header-icon">
                        <IconBarChart />
                    </span>
                    Pregled rute
                </h2>
                <span className={`route-status status-${route.status.toLowerCase()}`}>
                    {route.status === 'Dodijeljena' ? 'Dodijeljena' : 'U toku'}
                </span>
            </div>

            <div className="summary-grid">
                <div className="summary-card">
                    <div className="summary-card-icon summary-card-icon--total">
                        <IconMailbox />
                    </div>
                    <div className="summary-label">Ukupno sandučića</div>
                    <div className="summary-value">{totalMailboxes}</div>
                </div>
                <div className="summary-card">
                    <div className="summary-card-icon summary-card-icon--processed">
                        <IconCheck />
                    </div>
                    <div className="summary-label">Obrađenih</div>
                    <div className="summary-value success">{processedMailboxes}</div>
                </div>
                <div className="summary-card">
                    <div className="summary-card-icon summary-card-icon--skipped">
                        <IconSkip />
                    </div>
                    <div className="summary-label">Preskočenih</div>
                    <div className="summary-value warning">{skippedMailboxes}</div>
                </div>
                <div className="summary-card">
                    <div className="summary-card-icon summary-card-icon--time">
                        <IconClock />
                    </div>
                    <div className="summary-label">Preostalo vrijeme</div>
                    <div className="summary-value">{formatTime(estimatedRemainingMinutes)}</div>
                </div>
            </div>

            <div className="progress-section">
                <div className="progress-label">
                    <span>Napredak: {processedMailboxes}/{totalMailboxes}</span>
                    <span className="progress-percentage">{completionPercentage}%</span>
                </div>
                <div className="progress-bar">
                    <div
                        className="progress-fill"
                        style={{ width: `${completionPercentage}%` }}
                    />
                </div>
            </div>
        </div>
    );
};