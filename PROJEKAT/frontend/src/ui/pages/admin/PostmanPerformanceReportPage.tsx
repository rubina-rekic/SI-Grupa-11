import { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";
import { routesApi } from "../../../infrastructure/api/routesApi";
import type {
    PostmanPerformanceReportResponse,
    PostmanPerformanceRowResponse,
} from "../../../infrastructure/api/routesApi";
import { Layout } from "../../components/Layout/Layout";
import "./PostmanPerformanceReportPage.css";

type SortDirection = "desc" | "asc";

const toLocalDateString = (date: Date) =>
    `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}-${String(date.getDate()).padStart(2, "0")}`;

const firstDayOfMonth = (date: Date) =>
    toLocalDateString(new Date(date.getFullYear(), date.getMonth(), 1));

const formatDate = (value: string) => {
    const parsed = new Date(`${value}T00:00:00`);
    if (Number.isNaN(parsed.getTime())) return value;
    return parsed.toLocaleDateString("bs-BA", { day: "2-digit", month: "2-digit", year: "numeric" });
};

const formatDateTime = (value?: string | null) => {
    if (!value) return "-";
    const parsed = new Date(value);
    if (Number.isNaN(parsed.getTime())) return "-";
    return parsed.toLocaleString("bs-BA", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
    });
};

const formatPercent = (value: number) => `${value.toFixed(value % 1 === 0 ? 0 : 2)}%`;

const csvEscape = (value: string | number | null | undefined) =>
    `"${String(value ?? "-").replaceAll('"', '""')}"`;

const buildCsv = (report: PostmanPerformanceReportResponse) => {
    const headers = [
        "Postar",
        "Dodijeljeni sanducici",
        "Ispraznjene lokacije",
        "Nerealizovane lokacije",
        "Uspjesnost",
        "Broj zavrsenih ruta",
    ];

    const rows = report.rows.map(row => [
        row.postmanName,
        row.assignedMailboxes,
        row.emptiedLocations,
        row.unrealizedLocations,
        `${row.successPercentage}%`,
        row.completedRoutesCount,
    ]);

    return [headers, ...rows]
        .map(row => row.map(csvEscape).join(","))
        .join("\n");
};

export default function PostmanPerformanceReportPage() {
    const today = useMemo(() => new Date(), []);
    const [fromDate, setFromDate] = useState(firstDayOfMonth(today));
    const [toDate, setToDate] = useState(toLocalDateString(today));
    const [report, setReport] = useState<PostmanPerformanceReportResponse | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [sortDirection, setSortDirection] = useState<SortDirection>("desc");
    const [selectedPostmanId, setSelectedPostmanId] = useState<string | null>(null);

    const loadReport = async () => {
        if (!fromDate || !toDate) {
            setError("Odaberite početni i završni datum.");
            return;
        }

        if (fromDate > toDate) {
            setError("Početni datum ne može biti poslije završnog datuma.");
            return;
        }

        try {
            setLoading(true);
            setError(null);
            const data = await routesApi.getPostmanPerformanceReport(fromDate, toDate);
            setReport(data);
            setSelectedPostmanId(data.rows[0]?.postmanId ?? null);
        } catch {
            setError("Nije moguće učitati izvještaj o učinku.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        void loadReport();
        // Initial load only; the form button applies later period changes.
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const sortedRows = useMemo(() => {
        if (!report) return [];
        return [...report.rows].sort((a, b) => {
            const diff = a.successPercentage - b.successPercentage;
            if (diff === 0) return a.postmanName.localeCompare(b.postmanName, "bs");
            return sortDirection === "desc" ? -diff : diff;
        });
    }, [report, sortDirection]);

    const selectedPostman = sortedRows.find(row => row.postmanId === selectedPostmanId) ?? null;

    const resetPeriod = () => {
        const now = new Date();
        setFromDate(firstDayOfMonth(now));
        setToDate(toLocalDateString(now));
    };

    const exportCsv = () => {
        if (!report || report.rows.length === 0) {
            toast.error("Nema podataka za export.");
            return;
        }

        const blob = new Blob([`\uFEFF${buildCsv(report)}`], { type: "text/csv;charset=utf-8;" });
        const url = URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = `izvjestaj_ucinka_postara_${report.fromDate}_${report.toDate}.csv`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    };

    return (
        <Layout>
            <div className="ppr-page">
                <header className="ppr-header">
                    <div>
                        <h1 className="ppr-title">Izvještaj o učinku poštara</h1>
                        <p className="ppr-subtitle">
                            Period: {report ? `${formatDate(report.fromDate)} - ${formatDate(report.toDate)}` : `${formatDate(fromDate)} - ${formatDate(toDate)}`}
                        </p>
                    </div>
                    <div className="ppr-actions">
                        <button className="btn btn--outline ppr-export" type="button" onClick={exportCsv} disabled={!report || report.rows.length === 0}>
                            Export CSV
                        </button>
                    </div>
                </header>

                <section className="ppr-filter-band" aria-label="Filter perioda">
                    <div className="form-field">
                        <label className="form-field__label" htmlFor="performance-from-date">Od datuma</label>
                        <input
                            id="performance-from-date"
                            className="form-field__input"
                            type="date"
                            value={fromDate}
                            onChange={event => setFromDate(event.target.value)}
                        />
                    </div>
                    <div className="form-field">
                        <label className="form-field__label" htmlFor="performance-to-date">Do datuma</label>
                        <input
                            id="performance-to-date"
                            className="form-field__input"
                            type="date"
                            value={toDate}
                            onChange={event => setToDate(event.target.value)}
                        />
                    </div>
                    <button className="btn btn--primary ppr-filter-submit" type="button" onClick={loadReport} disabled={loading}>
                        {loading ? "Učitavanje..." : "Prikaži izvještaj"}
                    </button>
                    <button className="btn btn--outline ppr-filter-reset" type="button" onClick={resetPeriod} disabled={loading}>
                        Resetuj period
                    </button>
                </section>

                {error && <div className="ppr-error">{error}</div>}

                {report && (
                    <>
                        <section className="ppr-summary" aria-label="Sažetak izvještaja">
                            <div className="ppr-metric">
                                <span>Poštara</span>
                                <strong>{report.totalPostmen}</strong>
                            </div>
                            <div className="ppr-metric">
                                <span>Dodijeljeno</span>
                                <strong>{report.totalAssignedMailboxes}</strong>
                            </div>
                            <div className="ppr-metric">
                                <span>Ispražnjeno</span>
                                <strong>{report.totalEmptiedLocations}</strong>
                            </div>
                            <div className="ppr-metric">
                                <span>Nerealizovano</span>
                                <strong>{report.totalUnrealizedLocations}</strong>
                            </div>
                            <div className="ppr-metric">
                                <span>Prosjek tima</span>
                                <strong>{formatPercent(report.teamAverageSuccessPercentage)}</strong>
                            </div>
                        </section>

                        {sortedRows.length > 0 ? (
                            <>
                                <section className="ppr-panel">
                                    <h2 className="ppr-panel-title">Poređenje učinka</h2>
                                    <div className="ppr-chart" role="img" aria-label="Stubni grafikon uspješnosti poštara">
                                        {sortedRows.map(row => (
                                            <div className="ppr-chart-item" key={row.postmanId}>
                                                <span className="ppr-chart-value">{formatPercent(row.successPercentage)}</span>
                                                <div className="ppr-chart-bar-track">
                                                    <div
                                                        className="ppr-chart-bar"
                                                        style={{ height: `${Math.max(row.successPercentage, 4)}%` }}
                                                    />
                                                </div>
                                                <span className="ppr-chart-label">{row.postmanName}</span>
                                            </div>
                                        ))}
                                    </div>
                                </section>

                                <section className="ppr-panel">
                                    <h2 className="ppr-panel-title">KPI tabela</h2>
                                    <div className="ppr-table-wrap">
                                        <table className="ppr-table">
                                            <thead>
                                                <tr>
                                                    <th>Ime poštara</th>
                                                    <th>Dodijeljeni sandučići</th>
                                                    <th>Uspješno ispražnjeno</th>
                                                    <th>Nerealizovano</th>
                                                    <th>
                                                        <button
                                                            className="ppr-sort-button"
                                                            type="button"
                                                            onClick={() => setSortDirection(prev => prev === "desc" ? "asc" : "desc")}
                                                        >
                                                            Uspješnost {sortDirection === "desc" ? "↓" : "↑"}
                                                        </button>
                                                    </th>
                                                    <th>Završene rute</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {sortedRows.map(row => (
                                                    <tr key={row.postmanId}>
                                                        <td>
                                                            <button
                                                                className="ppr-name-button"
                                                                type="button"
                                                                onClick={() => setSelectedPostmanId(row.postmanId)}
                                                            >
                                                                {row.postmanName}
                                                            </button>
                                                        </td>
                                                        <td>{row.assignedMailboxes}</td>
                                                        <td>{row.emptiedLocations}</td>
                                                        <td>{row.unrealizedLocations}</td>
                                                        <td><span className="ppr-success">{formatPercent(row.successPercentage)}</span></td>
                                                        <td>{row.completedRoutesCount}</td>
                                                    </tr>
                                                ))}
                                            </tbody>
                                        </table>
                                    </div>
                                </section>

                                {selectedPostman && <PostmanRouteDetails row={selectedPostman} onClose={() => setSelectedPostmanId(null)} />}
                            </>
                        ) : (
                            <div className="ppr-state">Nema završenih ruta za odabrani period.</div>
                        )}
                    </>
                )}

                {loading && !report && <div className="ppr-state">Učitavanje izvještaja...</div>}
            </div>
        </Layout>
    );
}

function PostmanRouteDetails({
    row,
    onClose,
}: {
    row: PostmanPerformanceRowResponse;
    onClose: () => void;
}) {
    return (
        <section className="ppr-panel ppr-detail" aria-label={`Detalji ruta za ${row.postmanName}`}>
            <div className="ppr-detail-header">
                <h2 className="ppr-detail-title">Rute u obračunu: {row.postmanName}</h2>
                <button className="btn btn--outline ppr-detail-close" type="button" onClick={onClose}>
                    Zatvori detalje
                </button>
            </div>
            <div className="ppr-table-wrap">
                <table className="ppr-table">
                    <thead>
                        <tr>
                            <th>Datum</th>
                            <th>Početak</th>
                            <th>Završeno</th>
                            <th>Dodijeljeno</th>
                            <th>Ispražnjeno</th>
                            <th>Nerealizovano</th>
                            <th>Uspješnost</th>
                        </tr>
                    </thead>
                    <tbody>
                        {row.routes.map(route => (
                            <tr key={route.routeId}>
                                <td>{formatDate(route.date)}</td>
                                <td>{route.plannedStartTime.slice(0, 5)}</td>
                                <td>{formatDateTime(route.completedAt)}</td>
                                <td>{route.assignedMailboxes}</td>
                                <td>{route.emptiedLocations}</td>
                                <td>{route.unrealizedLocations}</td>
                                <td>{formatPercent(route.successPercentage)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </section>
    );
}
