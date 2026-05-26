import React, { useCallback, useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';
import { Layout } from '../../components/Layout/Layout';
import { routesApi } from '../../../infrastructure/api/routesApi';
import type { RouteResponse, RouteItemResponse } from '../../../infrastructure/api/routesApi';
import { getUsers } from '../../../infrastructure/api/users/usersApi';
import type { UserListDto } from '../../../infrastructure/api/users/usersApi';
import {
    getMailboxStatusLabel,
    getVisitStatusClass,
    getVisitStatusLabel,
    isRouteItemProcessed,
    isRouteItemUnavailable,
} from '../../components/PostmanRoute/statusUtils';
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

function formatDate(date: string) {
    const parsed = new Date(`${date}T00:00:00`);
    if (Number.isNaN(parsed.getTime())) return date;
    return parsed.toLocaleDateString('bs-BA', { day: '2-digit', month: '2-digit', year: 'numeric' });
}

function formatDateTime(value?: string | null) {
    if (!value) return '—';
    const parsed = new Date(value);
    if (Number.isNaN(parsed.getTime())) return '—';
    return parsed.toLocaleString('bs-BA', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
    });
}

function routeStatusLabel(status: string) {
    return STATUS_LABELS[status] ?? status;
}

function isDone(item: RouteItemResponse) {
    return isRouteItemProcessed(item);
}

function isProblematic(item: RouteItemResponse) {
    return isRouteItemUnavailable(item);
}

function routeProgress(route: RouteResponse) {
    const total = route.routeItems.length;
    const done  = route.routeItems.filter(isDone).length;
    const prob  = route.routeItems.filter(isProblematic).length;
    return { total, done, prob };
}

function getEffectiveRouteStatus(route: RouteResponse) {
    const { total, done } = routeProgress(route);

    if (total > 0 && done === total && route.status !== 'Otkazana') {
        return 'Zavrsena';
    }

    if (done > 0 && (route.status === 'Planirana' || route.status === 'Dodijeljena')) {
        return 'UProgresu';
    }

    return route.status;
}

function hasProblematicItems(route: RouteResponse) {
    return route.routeItems.some(isProblematic);
}

function getReportStatus(item: RouteItemResponse) {
    if (isRouteItemUnavailable(item)) {
        return { label: 'Nedostupan', css: 'unavailable' };
    }

    if (isRouteItemProcessed(item)) {
        const type = getMailboxStatusLabel(item.processedStatus ?? item.mailboxStatus);
        return {
            label: type === 'Obrađen' ? 'Obrađen' : `Obrađen (${type})`,
            css: 'processed',
        };
    }

    return { label: 'Nije posjećen', css: 'unvisited' };
}

function routeName(route: RouteResponse) {
    return `Dnevna ruta ${route.id.slice(0, 8).toUpperCase()}`;
}

function escapeHtml(value: string) {
    return value
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#39;');
}

function buildReportMetrics(route: RouteResponse) {
    const total = route.routeItems.length;
    const unavailable = route.routeItems.filter(isRouteItemUnavailable).length;
    const processed = route.routeItems.filter(item => !isRouteItemUnavailable(item) && isRouteItemProcessed(item)).length;
    const unvisited = total - processed - unavailable;
    const realization = total === 0 ? 0 : Math.round((processed / total) * 100);

    return { total, processed, unavailable, unvisited, realization };
}

function buildReportHtml(route: RouteResponse) {
    const metrics = buildReportMetrics(route);
    const rows = route.routeItems.map((item) => {
        const status = getReportStatus(item);
        return `
            <tr class="${status.css}">
                <td>${item.order}</td>
                <td>${escapeHtml(item.address)}</td>
                <td>${escapeHtml(item.priority)}</td>
                <td>${escapeHtml(status.label)}</td>
                <td>-</td>
                <td>${escapeHtml(formatDateTime(item.processedAt))}</td>
            </tr>`;
    }).join('');

    return `<!doctype html>
<html lang="bs">
<head>
    <meta charset="utf-8" />
    <title>${escapeHtml(routeName(route))}</title>
    <style>
        body { font-family: Arial, sans-serif; color: #0f172a; margin: 32px; }
        h1 { margin: 0 0 4px; font-size: 22px; }
        .meta { color: #475569; font-size: 13px; margin-bottom: 18px; }
        .summary { display: grid; grid-template-columns: repeat(5, 1fr); gap: 8px; margin: 16px 0; }
        .summary div { border: 1px solid #cbd5e1; border-radius: 6px; padding: 10px; }
        .label { color: #64748b; font-size: 11px; text-transform: uppercase; }
        .value { font-size: 18px; font-weight: 700; margin-top: 4px; }
        .warning { background: #fff7ed; border: 1px solid #fed7aa; color: #9a3412; padding: 10px; border-radius: 6px; margin: 12px 0; }
        table { border-collapse: collapse; width: 100%; margin-top: 16px; font-size: 12px; }
        th, td { border: 1px solid #cbd5e1; padding: 7px 8px; text-align: left; vertical-align: top; }
        th { background: #f8fafc; color: #334155; }
        tr.processed td { background: #f0fdf4; }
        tr.unavailable td { background: #fef2f2; }
        tr.unvisited td { background: #f8fafc; color: #64748b; }
    </style>
</head>
<body>
    <h1>${escapeHtml(routeName(route))}</h1>
    <div class="meta">
        Datum: ${escapeHtml(formatDate(route.date))} &nbsp;|&nbsp;
        Poštar: ${escapeHtml(route.postmanName ?? 'Nije dodijeljeno')} &nbsp;|&nbsp;
        Status: ${escapeHtml(routeStatusLabel(getEffectiveRouteStatus(route)))} &nbsp;|&nbsp;
        Završeno: ${escapeHtml(formatDateTime(route.completedAt))}
    </div>
    <div class="summary">
        <div><span class="label">Ukupno</span><div class="value">${metrics.total}</div></div>
        <div><span class="label">Obrađeno</span><div class="value">${metrics.processed}</div></div>
        <div><span class="label">Nedostupno</span><div class="value">${metrics.unavailable}</div></div>
        <div><span class="label">Nije posjećeno</span><div class="value">${metrics.unvisited}</div></div>
        <div><span class="label">Realizacija</span><div class="value">${metrics.realization}%</div></div>
    </div>
    ${metrics.realization < 80 ? '<div class="warning">Upozorenje: Realizacija rute ispod standardnog praga (80%).</div>' : ''}
    <table>
        <thead>
            <tr>
                <th>#</th>
                <th>Adresa</th>
                <th>Prioritet</th>
                <th>Finalni status</th>
                <th>Razlog nedostupnosti</th>
                <th>Timestamp akcije</th>
            </tr>
        </thead>
        <tbody>${rows}</tbody>
    </table>
</body>
</html>`;
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
    const navigate = useNavigate();
    const { total, done, prob } = routeProgress(route);
    const problematic = hasProblematicItems(route);
    const effectiveStatus = getEffectiveRouteStatus(route);

    return (
        <div
            className={`rdb-card${problematic ? ' rdb-card--warning' : ''}${effectiveStatus === 'UProgresu' ? ' rdb-card--active' : ''}`}
        >
            <div className="rdb-card-header">
                <div className="rdb-card-postman">
                    <span className="rdb-card-postman-name">{route.postmanName ?? '—'}</span>
                    <span className="rdb-card-time">
                        {fmtTime(route.plannedStartTime)}
                        {route.plannedEndTime ? ` – ${fmtTime(route.plannedEndTime)}` : ''}
                    </span>
                </div>
                <StatusBadge status={effectiveStatus} />
            </div>

            {problematic && (
                <div className="rdb-card-alert">
                    ⚠ {prob} {prob === 1 ? 'sandučić zahtijeva' : 'sandučića zahtijeva'} pažnju
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
                      className={`rdb-item${isProblematic(item) ? ' rdb-item--problem rdb-item--clickable' : ''}`}
                      onClick={isProblematic(item) ? () => navigate(`/admin/issues?routeItemId=${item.id}`) : undefined}
                      title={isProblematic(item) ? "Klikni za detalje problema" : undefined}
                    >
                        <span className="rdb-item-order">{item.order}</span>
                        <span className="rdb-item-address">{item.address}</span>
                        <span className="rdb-item-time">
                            {item.processedAt
                                ? new Date(item.processedAt).toLocaleTimeString('bs', { hour: '2-digit', minute: '2-digit' })
                                : fmtTime(item.estimatedArrivalTime)}
                        </span>
                        <span className={`rdb-item-status rdb-item-status--${getVisitStatusClass(item)}`}>
                            {isDone(item)
                                ? getMailboxStatusLabel(item.processedStatus ?? item.mailboxStatus)
                                : getVisitStatusLabel(item)}
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

function DailyReportPreview({ route, onDownloadPdf }: { route: RouteResponse; onDownloadPdf: () => void }) {
    const metrics = buildReportMetrics(route);
    const effectiveStatus = getEffectiveRouteStatus(route);

    return (
        <div className="rdb-report-preview">
            <div className="rdb-report-preview-header">
                <div>
                    <h3>{routeName(route)}</h3>
                    <p>
                        {formatDate(route.date)} · {route.postmanName ?? 'Nije dodijeljeno'} · {routeStatusLabel(effectiveStatus)}
                    </p>
                </div>
                <button className="rdb-report-download" onClick={onDownloadPdf}>
                    Preuzmi PDF
                </button>
            </div>

            <div className="rdb-report-summary">
                <div className="rdb-report-stat">
                    <span>Ukupno sandučića</span>
                    <strong>{metrics.total}</strong>
                </div>
                <div className="rdb-report-stat rdb-report-stat--processed">
                    <span>Obrađenih</span>
                    <strong>{metrics.processed}</strong>
                </div>
                <div className="rdb-report-stat rdb-report-stat--unavailable">
                    <span>Nedostupnih</span>
                    <strong>{metrics.unavailable}</strong>
                </div>
                <div className="rdb-report-stat">
                    <span>Nije posjećeno</span>
                    <strong>{metrics.unvisited}</strong>
                </div>
                <div className="rdb-report-stat">
                    <span>Realizacija</span>
                    <strong>{metrics.realization}%</strong>
                </div>
            </div>

            {metrics.realization < 80 && (
                <div className="rdb-report-warning">
                    Upozorenje: Realizacija rute ispod standardnog praga (80%).
                </div>
            )}

            <div className="rdb-report-table-wrap">
                <table className="rdb-report-table">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Adresa</th>
                            <th>Prioritet</th>
                            <th>Finalni status</th>
                            <th>Razlog nedostupnosti</th>
                            <th>Timestamp akcije</th>
                        </tr>
                    </thead>
                    <tbody>
                        {route.routeItems.map(item => {
                            const status = getReportStatus(item);
                            return (
                                <tr key={item.id} className={`rdb-report-row rdb-report-row--${status.css}`}>
                                    <td>{item.order}</td>
                                    <td>{item.address}</td>
                                    <td>{item.priority}</td>
                                    <td>{status.label}</td>
                                    <td>—</td>
                                    <td>{formatDateTime(item.processedAt)}</td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            </div>
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
    const [postmen, setPostmen]       = useState<UserListDto[]>([]);
    const [selectedPostmanId, setSelectedPostmanId] = useState('');
    const [reportRoute, setReportRoute] = useState<RouteResponse | null>(null);
    const [reportMessage, setReportMessage] = useState<string | null>(null);
    const [postmenLoading, setPostmenLoading] = useState(false);

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

    useEffect(() => {
        let active = true;
        setPostmenLoading(true);

        getUsers()
            .then(result => {
                if (!active) return;

                if (!result.data) {
                    setReportMessage('Nije moguće učitati listu poštara.');
                    return;
                }

                const workers = result.data
                    .filter(user => user.role === 'PostalWorker')
                    .sort((a, b) => a.username.localeCompare(b.username, 'bs'));

                setPostmen(workers);
                setSelectedPostmanId(prev => prev || workers[0]?.id || '');
            })
            .catch(() => {
                if (active) setReportMessage('Nije moguće učitati listu poštara.');
            })
            .finally(() => {
                if (active) setPostmenLoading(false);
            });

        return () => {
            active = false;
        };
    }, []);

    // Initial + date-change load
    useEffect(() => {
        load(date);
    }, [date, load]);

    useEffect(() => {
        setReportRoute(null);
        setReportMessage(null);
    }, [date, selectedPostmanId]);

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

    const filtered = filter === 'Sve' ? routes : routes.filter(r => getEffectiveRouteStatus(r) === filter);

    const summaryByStatus = STATUS_FILTER_OPTIONS.filter(s => s !== 'Sve').map(s => ({
        status: s,
        count: routes.filter(r => getEffectiveRouteStatus(r) === s).length,
    }));

    const generateReport = () => {
        if (!selectedPostmanId) {
            setReportRoute(null);
            setReportMessage('Odaberite poštara za generisanje izvještaja.');
            return;
        }

        const route = routes.find(r => r.postmanId === selectedPostmanId);

        if (!route) {
            setReportRoute(null);
            setReportMessage('Nema podataka za odabrane parametre.');
            return;
        }

        setReportRoute(route);
        setReportMessage(null);
    };

    const downloadReportPdf = () => {
        if (!reportRoute) return;

        const reportWindow = window.open('', '_blank', 'width=960,height=720');
        if (!reportWindow) {
            toast.error('Browser je blokirao otvaranje PDF izvještaja.');
            return;
        }

        reportWindow.document.write(buildReportHtml(reportRoute));
        reportWindow.document.close();
        reportWindow.focus();
        reportWindow.setTimeout(() => reportWindow.print(), 250);
    };

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
                            {s === 'Sve' ? `Sve (${routes.length})` : `${STATUS_LABELS[s]} (${routes.filter(r => getEffectiveRouteStatus(r) === s).length})`}
                        </button>
                    ))}
                </div>

                <div className="rdb-report-panel">
                    <div className="rdb-report-controls">
                        <div className="rdb-report-copy">
                            <h2>Dnevni izvještaj</h2>
                            <p>{formatDate(date)}</p>
                        </div>
                        <div className="rdb-report-form">
                            <select
                                className="rdb-report-select"
                                value={selectedPostmanId}
                                onChange={event => setSelectedPostmanId(event.target.value)}
                                disabled={postmenLoading || postmen.length === 0}
                            >
                                <option value="">Odaberite poštara</option>
                                {postmen.map(postman => (
                                    <option key={postman.id} value={postman.id}>
                                        {postman.username}
                                    </option>
                                ))}
                            </select>
                            <button
                                className="rdb-report-generate"
                                onClick={generateReport}
                                disabled={loading || postmenLoading}
                            >
                                Generiši izvještaj
                            </button>
                        </div>
                    </div>

                    {reportMessage && (
                        <div className="rdb-report-message">{reportMessage}</div>
                    )}

                    {reportRoute && (
                        <DailyReportPreview route={reportRoute} onDownloadPdf={downloadReportPdf} />
                    )}
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
