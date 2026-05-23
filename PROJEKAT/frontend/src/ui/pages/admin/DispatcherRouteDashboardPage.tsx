import React, { useCallback, useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Layout } from '../../components/Layout/Layout';
import { routesApi } from '../../../infrastructure/api/routesApi';
import type { RouteResponse, RouteItemResponse } from '../../../infrastructure/api/routesApi';
import './DispatcherRouteDashboardPage.css';

// ── helpers ────────────────────────────────────────────────────

const toLocalDateString = (d: Date) =>
    `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;

const STATUS_ORDER: Record<string, number> = {
    UProgresu: 0,
    Dodijeljena: 1,
    Planirana: 2,
    Zavrsena: 3,
    Otkazana: 4,
};

const STATUS_LABELS: Record<string, string> = {
    Planirana:  'Planirana',
    Dodijeljena: 'Dodijeljena',
    UProgresu:  'U toku',
    Zavrsena:   'Završena',
    Otkazana:   'Otkazana',
};

const STATUS_FILTER_OPTIONS = ['Sve', 'UProgresu', 'Dodijeljena', 'Planirana', 'Zavrsena', 'Otkazana'] as const;

const REFRESH_INTERVAL = 30;

function fmtTime(t: string) {
    return t?.slice(0, 5) ?? '—';
}

function isDone(item: RouteItemResponse) {
    return item.mailboxStatus === 'Obraen' || item.mailboxStatus === 'Ispraznjen';
}

function isProblematic(item: RouteItemResponse) {
    return item.mailboxStatus === 'Napunjen';
}

function routeProgress(route: RouteResponse) {
    const total = route.routeItems.length;
    const done  = route.routeItems.filter(isDone).length;
    const prob  = route.routeItems.filter(isProblematic).length;
    return { total, done, prob };
}

function hasProblematicItems(route: RouteResponse) {
    return route.routeItems.some(isProblematic);
}

// ── sub-components ─────────────────────────────────────────────

function StatusBadge({ status }: { status: string }) {
    return (
        <span className={`rdb-status rdb-status--${status.toLowerCase()}`}>
            {STATUS_LABELS[status] ?? status}
        </span>
    );
}

function ProgressBar({ done, total }: { done: number; total: number }) {
    const pct = total === 0 ? 0 : Math.round((done / total) * 100);
    return (
        <div className="rdb-progress">
            <div className="rdb-progress-bar" style={{ width: `${pct}%` }} />
        </div>
    );
}

function RouteCard({ route, onOpen }: { route: RouteResponse; onOpen: (id: string) => void }) {
    const { total, done, prob } = routeProgress(route);
    const problematic = hasProblematicItems(route);

    return (
        <div
            className={`rdb-card${problematic ? ' rdb-card--warning' : ''}${route.status === 'UProgresu' ? ' rdb-card--active' : ''}`}
        >
            <div className="rdb-card-header">
                <div className="rdb-card-postman">
                    <span className="rdb-card-postman-name">{route.postmanName ?? '—'}</span>
                    <span className="rdb-card-time">
                        {fmtTime(route.plannedStartTime)}
                        {route.plannedEndTime ? ` – ${fmtTime(route.plannedEndTime)}` : ''}
                    </span>
                </div>
                <StatusBadge status={route.status} />
            </div>

            {problematic && (
                <div className="rdb-card-alert">
                    ⚠ {prob} {prob === 1 ? 'sandučić zahtijeva' : 'sandučića zahtijeva'} pažnju (Napunjen)
                </div>
            )}

            <div className="rdb-card-stats">
                <span className="rdb-card-stat">
                    <span className="rdb-card-stat-val">{done}/{total}</span>
                    <span className="rdb-card-stat-lbl">obrađeno</span>
                </span>
                <span className="rdb-card-stat">
                    <span className="rdb-card-stat-val">{route.totalDistanceKm} km</span>
                    <span className="rdb-card-stat-lbl">ukupno</span>
                </span>
                <span className="rdb-card-stat">
                    <span className="rdb-card-stat-val">{route.totalDurationMinutes} min</span>
                    <span className="rdb-card-stat-lbl">trajanje</span>
                </span>
            </div>

            <ProgressBar done={done} total={total} />

            <div className="rdb-card-items">
                {route.routeItems.map(item => (
                    <div
                        key={item.id}
                        className={`rdb-item${isProblematic(item) ? ' rdb-item--problem' : isDone(item) ? ' rdb-item--done' : ''}`}
                    >
                        <span className="rdb-item-order">{item.order}</span>
                        <span className="rdb-item-address">{item.address}</span>
                        <span className="rdb-item-time">{fmtTime(item.estimatedArrivalTime)}</span>
                        <span className={`rdb-item-status rdb-item-status--${(item.mailboxStatus ?? '').toLowerCase()}`}>
                            {item.mailboxStatus || 'Prazan'}
                        </span>
                    </div>
                ))}
            </div>

            <button className="rdb-card-link" onClick={() => onOpen(route.id)}>
                Otvori detalje →
            </button>
        </div>
    );
}

// ── main page ──────────────────────────────────────────────────

const DispatcherRouteDashboardPage: React.FC = () => {
    const navigate = useNavigate();

    const [date, setDate]             = useState(toLocalDateString(new Date()));
    const [routes, setRoutes]         = useState<RouteResponse[]>([]);
    const [loading, setLoading]       = useState(false);
    const [error, setError]           = useState<string | null>(null);
    const [filter, setFilter]         = useState<string>('Sve');
    const [countdown, setCountdown]   = useState(REFRESH_INTERVAL);
    const [lastRefresh, setLastRefresh] = useState<Date | null>(null);

    const countdownRef = useRef<ReturnType<typeof setInterval> | null>(null);

    const load = useCallback(async (selectedDate: string) => {
        setLoading(true);
        setError(null);
        try {
            const data = await routesApi.getRoutesForDate(selectedDate);
            const sorted = [...data].sort((a, b) =>
                (STATUS_ORDER[a.status] ?? 99) - (STATUS_ORDER[b.status] ?? 99)
            );
            setRoutes(sorted);
            setLastRefresh(new Date());
            setCountdown(REFRESH_INTERVAL);
        } catch {
            setError('Nije moguće učitati rute. Provjerite konekciju.');
        } finally {
            setLoading(false);
        }
    }, []);

    // Initial + date-change load
    useEffect(() => {
        load(date);
    }, [date, load]);

    // Auto-refresh countdown
    useEffect(() => {
        if (countdownRef.current) clearInterval(countdownRef.current);

        countdownRef.current = setInterval(() => {
            setCountdown(prev => {
                if (prev <= 1) {
                    load(date);
                    return REFRESH_INTERVAL;
                }
                return prev - 1;
            });
        }, 1000);

        return () => {
            if (countdownRef.current) clearInterval(countdownRef.current);
        };
    }, [date, load]);

    const filtered = filter === 'Sve' ? routes : routes.filter(r => r.status === filter);

    const summaryByStatus = STATUS_FILTER_OPTIONS.filter(s => s !== 'Sve').map(s => ({
        status: s,
        count: routes.filter(r => r.status === s).length,
    }));

    return (
        <Layout>
            <div className="rdb-page">

                {/* ── Header ─────────────────────────────── */}
                <div className="rdb-header">
                    <div className="rdb-header-left">
                        <h1 className="rdb-title">Praćenje ruta</h1>
                        <p className="rdb-subtitle">Pregled statusa svih ruta i sandučića</p>
                    </div>
                    <div className="rdb-header-right">
                        <div className="rdb-refresh-info">
                            {lastRefresh && (
                                <span className="rdb-last-refresh">
                                    Ažurirano u {lastRefresh.toLocaleTimeString('bs', { hour: '2-digit', minute: '2-digit' })}
                                </span>
                            )}
                            <span className="rdb-countdown">
                                Osvježava za {countdown}s
                            </span>
                            <button
                                className="rdb-refresh-btn"
                                onClick={() => load(date)}
                                disabled={loading}
                                title="Ručno osvježi"
                            >
                                {loading ? '…' : '↻'}
                            </button>
                        </div>
                        <input
                            type="date"
                            className="rdb-date-input"
                            value={date}
                            onChange={e => setDate(e.target.value)}
                        />
                    </div>
                </div>

                {/* ── Summary chips ───────────────────────── */}
                {routes.length > 0 && (
                    <div className="rdb-summary">
                        {summaryByStatus.filter(s => s.count > 0).map(s => (
                            <div key={s.status} className={`rdb-summary-chip rdb-summary-chip--${s.status.toLowerCase()}`}>
                                <span className="rdb-summary-count">{s.count}</span>
                                <span className="rdb-summary-label">{STATUS_LABELS[s.status]}</span>
                            </div>
                        ))}
                    </div>
                )}

                {/* ── Filter chips ────────────────────────── */}
                <div className="rdb-filters">
                    {STATUS_FILTER_OPTIONS.map(s => (
                        <button
                            key={s}
                            className={`rdb-filter-btn${filter === s ? ' rdb-filter-btn--active' : ''}`}
                            onClick={() => setFilter(s)}
                        >
                            {s === 'Sve' ? `Sve (${routes.length})` : `${STATUS_LABELS[s]} (${routes.filter(r => r.status === s).length})`}
                        </button>
                    ))}
                </div>

                {/* ── Content ─────────────────────────────── */}
                {loading && routes.length === 0 && (
                    <div className="rdb-state">
                        <div className="rdb-spinner" />
                        <p>Učitavanje ruta...</p>
                    </div>
                )}

                {error && (
                    <div className="rdb-error">{error}</div>
                )}

                {!loading && !error && filtered.length === 0 && (
                    <div className="rdb-state">
                        <p className="rdb-empty-icon">📭</p>
                        <p>{routes.length === 0 ? 'Nema ruta za odabrani datum.' : 'Nema ruta za odabrani filter.'}</p>
                    </div>
                )}

                {filtered.length > 0 && (
                    <div className="rdb-grid">
                        {filtered.map(route => (
                            <RouteCard
                                key={route.id}
                                route={route}
                                onOpen={id => navigate(`/admin/routes/generate?routeId=${id}`)}
                            />
                        ))}
                    </div>
                )}
            </div>
        </Layout>
    );
};

export default DispatcherRouteDashboardPage;
