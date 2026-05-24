import React, { useEffect, useState } from 'react';
import { toast } from 'sonner';
import { Layout } from '../components/Layout/Layout';
import { RouteMap } from '../components/PostmanRoute/RouteMap';
import { RouteItemList } from '../components/PostmanRoute/RouteItemList';
import { RouteSummary } from '../components/PostmanRoute/RouteSummary';
import { routesApi } from '../../infrastructure/api/routesApi';
import type { RouteResponse } from '../../infrastructure/api/routesApi';
import { updateMailboxStatus, mailboxStatusLabels } from '../../infrastructure/api/mailboxes/mailboxesApi';
import { getRouteStatusLabel, isRouteItemProcessed } from '../components/PostmanRoute/statusUtils';
import './PostmanAssignedRoutePage.css';

/* ── SVG Icons ───────────────────────────────────────────── */

const IconCalendar = () => (
    <svg width="14" height="14" viewBox="0 0 16 16" fill="none">
        <rect x="1.5" y="2.5" width="13" height="12" rx="2" stroke="currentColor" strokeWidth="1.4"/>
        <path d="M1.5 6.5h13" stroke="currentColor" strokeWidth="1.4"/>
        <path d="M5 1.5v2M11 1.5v2" stroke="currentColor" strokeWidth="1.4" strokeLinecap="round"/>
    </svg>
);

const IconMap = () => (
    <svg width="15" height="15" viewBox="0 0 16 16" fill="none">
        <path d="M1 3.5l4.5-1.5 5 2L15 2.5v10L10.5 14l-5-2L1 13.5V3.5z"
              stroke="currentColor" strokeWidth="1.4" strokeLinejoin="round"/>
        <path d="M5.5 2v10M10.5 4v10" stroke="currentColor" strokeWidth="1.4" strokeLinecap="round"/>
    </svg>
);

const IconWarning = () => (
    <svg width="18" height="18" viewBox="0 0 20 20" fill="none">
        <path d="M10 2L18.5 17H1.5L10 2z" stroke="currentColor" strokeWidth="1.5" strokeLinejoin="round"/>
        <path d="M10 8v4" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round"/>
        <circle cx="10" cy="14.5" r=".9" fill="currentColor"/>
    </svg>
);

const IconInbox = () => (
    <svg width="30" height="30" viewBox="0 0 30 30" fill="none">
        <rect x="3" y="6" width="24" height="18" rx="3" stroke="currentColor" strokeWidth="1.5"/>
        <path d="M3 18h6l2.5 3.5h7L21 18h6" stroke="currentColor" strokeWidth="1.5" strokeLinejoin="round"/>
        <path d="M9.5 12h11M9.5 15.5h7" stroke="currentColor" strokeWidth="1.4" strokeLinecap="round"/>
    </svg>
);

const IconInfo = () => (
    <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
        <circle cx="8" cy="8" r="6.5" stroke="currentColor" strokeWidth="1.4"/>
        <path d="M8 7v5" stroke="currentColor" strokeWidth="1.4" strokeLinecap="round"/>
        <circle cx="8" cy="4.5" r=".85" fill="currentColor"/>
    </svg>
);

const IconStatus = () => (
    <svg width="10" height="10" viewBox="0 0 10 10" fill="none">
        <circle cx="5" cy="5" r="4" stroke="currentColor" strokeWidth="1.2"/>
        <path d="M3 5l1.5 1.5L7 3.5" stroke="currentColor" strokeWidth="1.2" strokeLinecap="round" strokeLinejoin="round"/>
    </svg>
);

/* ────────────────────────────────────────────────────────── */

const PostmanAssignedRoutePage: React.FC = () => {
    const [route, setRoute] = useState<RouteResponse | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const loadRoute = async () => {
            try {
                setLoading(true);
                setError(null);
                const assignedRoute = await routesApi.getMyAssignedRouteForToday();
                setRoute(assignedRoute);
            } catch (err) {
                console.error('Greška pri učitavanju rute:', err);
                setError('Nije moguće učitati rutu. Pokušajte osvježiti stranicu.');
            } finally {
                setLoading(false);
            }
        };
        loadRoute();
    }, []);

    const handleStatusChange = async (mailboxId: string, status: number) => {
        try {
            await updateMailboxStatus(mailboxId, { status: status as 0 | 1 | 2 | 3 | 4 });
            const label = mailboxStatusLabels[status as 0 | 1 | 2 | 3 | 4] ?? String(status);
            setRoute(prev => {
                if (!prev) return prev;
                const now = new Date().toISOString();
                const routeItems = prev.routeItems.map(item =>
                    item.mailboxId === mailboxId
                        ? {
                            ...item,
                            status: 'Obrađen',
                            mailboxStatus: label,
                            processedStatus: label,
                            processedAt: now,
                            processedBy: prev.postmanId,
                        }
                        : item
                );
                const routeFinished = routeItems.length > 0 && routeItems.every(isRouteItemProcessed);

                return {
                    ...prev,
                    routeItems,
                    status: routeFinished ? 'Zavrsena' : 'UProgresu',
                    startedAt: prev.startedAt ?? now,
                    completedAt: routeFinished ? now : prev.completedAt,
                };
            });
            toast.success(`Status sandučića ažuriran: ${label}`);
        } catch (err) {
            toast.error(err instanceof Error ? err.message : 'Greška pri ažuriranju statusa');
        }
    };

    const mjeseci = [
    'januar', 'februar', 'mart', 'april', 'maj', 'juni',
    'juli', 'august', 'septembar', 'oktobar', 'novembar', 'decembar'
    ];
    const dani = ['nedjelja', 'ponedjeljak', 'utorak', 'srijeda', 'četvrtak', 'petak', 'subota'];

    const today = new Date();
    const todayFormatted = `${dani[today.getDay()]}, ${today.getDate()}. ${mjeseci[today.getMonth()]} ${today.getFullYear()}.`;

    return (
        <Layout>
            <div className="postman-route-page">

                {/* ══════════════ HERO HEADER ══════════════ */}
                <div className="route-header">
                    <div className="route-header-inner">
                        <div className="route-header-left">
                            <div className="route-header-eyebrow">
                                <span className="route-header-eyebrow-dot" />
                                Aktivna sesija
                            </div>
                            <h1 className="page-title">
                                Moja ruta&nbsp;<span>za danas</span>
                            </h1>
                            <div className="route-header-date">
                                <IconCalendar />
                                {todayFormatted}
                            </div>
                        </div>
                        <div className="route-header-right">
                            <div className="route-header-badge">
                                <IconStatus />
                                {route ? getRouteStatusLabel(route.status) : 'Dodijeljena'}
                            </div>
                        </div>
                    </div>
                    <div className="route-header-line" />
                </div>

                {/* ══════════════ PAGE BODY ══════════════ */}
                <div className="route-body">

                    {/* Loading */}
                    {loading && (
                        <div className="loading-container">
                            <div className="spinner" />
                            <p>Učitavanje vaše rute...</p>
                        </div>
                    )}

                    {/* Error */}
                    {error && !loading && (
                        <div className="error-container">
                            <div className="error-message">
                                <span className="error-icon"><IconWarning /></span>
                                <p>{error}</p>
                            </div>
                        </div>
                    )}

                    {/* No route */}
                    {!route && !loading && !error && (
                        <div className="no-route-container">
                            <div className="no-route-message">
                                <span className="no-route-icon"><IconInbox /></span>
                                <h2>Nema dodijeljene rute za danas</h2>
                                <p>Dispečer će vam dodijeliti rutu kada bude dostupna.</p>
                                <p>Osvježite stranicu za ažuriranje.</p>
                            </div>
                        </div>
                    )}

                    {/* Route content */}
                    {route && (
                        <div className="route-content">

                            <RouteSummary route={route} />

                            <div className="map-section">
                                <h2 className="section-title">
                                    <span className="section-title-icon section-title-icon--map">
                                        <IconMap />
                                    </span>
                                    Interaktivna mapa
                                </h2>
                                <RouteMap items={route.routeItems} />
                                <p className="map-help-text">
                                    Kliknite na marker da vidite detalje o sandučiću
                                </p>
                            </div>

                            <div className="list-section">
                                <RouteItemList items={route.routeItems} onStatusChange={handleStatusChange} />
                            </div>

                            <div className="route-actions">
                                <span className="route-actions-icon"><IconInfo /></span>
                                <p className="info-text">
                                    Koristite Detalje sandučića da promijenite status tokom obilaska
                                </p>
                            </div>

                        </div>
                    )}
                </div>
            </div>
        </Layout>
    );
};

export default PostmanAssignedRoutePage;
