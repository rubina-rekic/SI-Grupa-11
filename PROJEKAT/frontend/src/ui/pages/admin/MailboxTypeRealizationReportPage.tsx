import { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";
import { routesApi } from "../../../infrastructure/api/routesApi";
import type { MailboxTypeRealizationReportResponse } from "../../../infrastructure/api/routesApi";
import { Layout } from "../../components/Layout/Layout";
import "./MailboxTypeRealizationReportPage.css";

const mailboxTypeLabels: Record<number, string> = {
    1: "Zidni (mali)",
    2: "Samostojeći (veliki)",
    3: "Unutrašnji (stambene zgrade)",
    4: "Specijalni (prioritetni)",
};

const chartColors = [
    "#2563eb",
    "#16a34a",
    "#d97706",
    "#7c3aed",
];

const toLocalDateString = (date: Date) =>
    `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}-${String(date.getDate()).padStart(2, "0")}`;

const formatDate = (value: string) => {
    const parsed = new Date(`${value}T00:00:00`);
    if (Number.isNaN(parsed.getTime())) return value;
    return parsed.toLocaleDateString("bs-BA", { day: "2-digit", month: "2-digit", year: "numeric" });
};

const csvEscape = (value: string | number | null | undefined) =>
    `"${String(value ?? "-").replaceAll('"', '""')}"`;

const buildCsv = (report: MailboxTypeRealizationReportResponse) => {
    const headers = [
        "Tip sandučića",
        "Planirani isprazni",
        "Uspješni isprazni",
        "Problemi",
        "Stopa neuspjeha",
    ];

    const rows = report.rows.flatMap(row => [
        [row.typeName, row.plannedEmpties, row.successfulEmpties, row.problemReports, `${row.failureRate}%`],
        ...row.details.map(detail => [
            `  ${detail.address}`,
            detail.routeDate,
            detail.status,
            detail.notes ?? "",
            ""
        ])
    ]);

    return [headers, ...rows].map(row => row.map(csvEscape).join(",")).join("\n");
};

export default function MailboxTypeRealizationReportPage() {
    const today = useMemo(() => new Date(), []);
    const [fromDate, setFromDate] = useState(firstDayOfMonth(today));
    const [toDate, setToDate] = useState(toLocalDateString(today));
    const [report, setReport] = useState<MailboxTypeRealizationReportResponse | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [selectedTypeId, setSelectedTypeId] = useState<number | null>(null);

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
            const data = await routesApi.getMailboxTypeRealizationReport(fromDate, toDate);
            setReport(data);
            setSelectedTypeId(data.rows[0]?.typeId ?? null);
        } catch {
            setError("Nije moguće učitati izvještaj o realizaciji po tipu sandučića.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        void loadReport();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const selectedRow = useMemo(
        () => report?.rows.find(row => row.typeId === selectedTypeId) ?? null,
        [report, selectedTypeId]
    );

    const totalPlanned = report?.rows.reduce((sum, row) => sum + row.plannedEmpties, 0) ?? 0;

    const exportCsv = () => {
        if (!report || report.rows.length === 0) {
            toast.error("Nema podataka za export.");
            return;
        }

        const blob = new Blob([`\uFEFF${buildCsv(report)}`], { type: "text/csv;charset=utf-8;" });
        const url = URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = `izvjestaj_realizacije_po_tipu_sanducica_${report.fromDate}_${report.toDate}.csv`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    };

    const buildPieStyle = () => {
        if (!report || report.rows.length === 0 || totalPlanned === 0) {
            return undefined;
        }

        let start = 0;
        const slices = report.rows.map((row, index) => {
            const size = (row.plannedEmpties / totalPlanned) * 100;
            const end = start + size;
            const color = chartColors[index % chartColors.length];
            const slice = `${color} ${start}% ${end}%`;
            start = end;
            return slice;
        });

        return { background: `conic-gradient(${slices.join(", ")})` };
    };

    const resetPeriod = () => {
        const now = new Date();
        setFromDate(firstDayOfMonth(now));
        setToDate(toLocalDateString(now));
    };

    return (
        <Layout>
            <div className="mtr-page">
                <header className="mtr-header">
                    <div>
                        <h1 className="mtr-title">Analiza realizacije po tipu sandučića</h1>
                        <p className="mtr-subtitle">
                            Period: {report ? `${formatDate(report.fromDate)} - ${formatDate(report.toDate)}` : `${formatDate(fromDate)} - ${formatDate(toDate)}`}
                        </p>
                    </div>
                    <div className="mtr-actions">
                        <button className="btn btn--outline mtr-export" type="button" onClick={exportCsv} disabled={!report || report.rows.length === 0}>
                            Export CSV
                        </button>
                    </div>
                </header>

                <section className="mtr-filter-band" aria-label="Filter perioda">
                    <div className="form-field">
                        <label className="form-field__label" htmlFor="mailbox-type-from-date">Od datuma</label>
                        <input
                            id="mailbox-type-from-date"
                            className="form-field__input"
                            type="date"
                            value={fromDate}
                            onChange={event => setFromDate(event.target.value)}
                        />
                    </div>
                    <div className="form-field">
                        <label className="form-field__label" htmlFor="mailbox-type-to-date">Do datuma</label>
                        <input
                            id="mailbox-type-to-date"
                            className="form-field__input"
                            type="date"
                            value={toDate}
                            onChange={event => setToDate(event.target.value)}
                        />
                    </div>
                    <button className="btn btn--primary mtr-filter-submit" type="button" onClick={loadReport} disabled={loading}>
                        {loading ? "Učitavanje..." : "Prikaži izvještaj"}
                    </button>
                    <button className="btn btn--outline mtr-filter-reset" type="button" onClick={resetPeriod} disabled={loading}>
                        Resetuj period
                    </button>
                </section>

                {error && <div className="mtr-error">{error}</div>}

                {report && (
                    <>
                        <section className="mtr-summary" aria-label="Sažetak izvještaja">
                            <div className="mtr-metric">
                                <span>Tipova</span>
                                <strong>{report.totalTypes}</strong>
                            </div>
                            <div className="mtr-metric">
                                <span>Planirani isprazni</span>
                                <strong>{report.totalPlannedEmpties}</strong>
                            </div>
                            <div className="mtr-metric">
                                <span>Uspješni isprazni</span>
                                <strong>{report.totalSuccessfulEmpties}</strong>
                            </div>
                            <div className="mtr-metric">
                                <span>Problemi</span>
                                <strong>{report.totalProblemReports}</strong>
                            </div>
                            <div className="mtr-metric">
                                <span>Prosjek neuspjeha</span>
                                <strong>{report.averageFailureRate.toFixed(2)}%</strong>
                            </div>
                        </section>

                        <section className="mtr-panel">
                            <h2 className="mtr-panel-title">Distribucija planiranih isprazni</h2>
                            <div className="mtr-pie-chart" style={buildPieStyle()} aria-label="Torta planiranih ispraznih po tipu sandučića" />
                            <div className="mtr-legend">
                                {report.rows.map((row, index) => (
                                    <div className="mtr-legend-item" key={row.typeId}>
                                        <span className="mtr-legend-swatch" style={{ background: chartColors[index % chartColors.length] }} />
                                        <span>{mailboxTypeLabels[row.typeId] ?? row.typeName}</span>
                                        <strong>{row.plannedEmpties}</strong>
                                    </div>
                                ))}
                            </div>
                        </section>

                        {report.rows.length > 0 ? (
                            <>
                                <section className="mtr-panel">
                                    <h2 className="mtr-panel-title">Detalji po tipu mailboxes</h2>
                                    <div className="mtr-table-wrap">
                                        <table className="mtr-table">
                                            <thead>
                                                <tr>
                                                    <th>Tip sandučića</th>
                                                    <th>Planirano</th>
                                                    <th>Uspješno</th>
                                                    <th>Problemi</th>
                                                    <th>Stopa neuspjeha</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {report.rows.map(row => (
                                                    <tr key={row.typeId} className={selectedTypeId === row.typeId ? "mtr-selected-row" : ""}>
                                                        <td>
                                                            <button
                                                                className="mtr-name-button"
                                                                type="button"
                                                                onClick={() => setSelectedTypeId(row.typeId)}
                                                            >
                                                                {mailboxTypeLabels[row.typeId] ?? row.typeName}
                                                            </button>
                                                        </td>
                                                        <td>{row.plannedEmpties}</td>
                                                        <td>{row.successfulEmpties}</td>
                                                        <td>{row.problemReports}</td>
                                                        <td>{row.failureRate.toFixed(2)}%</td>
                                                    </tr>
                                                ))}
                                            </tbody>
                                        </table>
                                    </div>
                                </section>

                                <section className="mtr-panel mtr-detail" aria-label="Detalji po tipu sandučića">
                                    <div className="mtr-detail-header">
                                        <div>
                                            <h2 className="mtr-detail-title">Detalji za {selectedRow ? mailboxTypeLabels[selectedRow.typeId] ?? selectedRow.typeName : "odabrani tip"}</h2>
                                            <p className="mtr-detail-subtitle">Kliknite tip iz tabele da vidite sve bilješke i probleme</p>
                                        </div>
                                        <button className="btn btn--outline mtr-detail-close" type="button" onClick={() => setSelectedTypeId(null)}>
                                            Zatvori
                                        </button>
                                    </div>
                                    {selectedRow ? (
                                        selectedRow.details.length > 0 ? (
                                            <div className="mtr-table-wrap">
                                                <table className="mtr-table">
                                                    <thead>
                                                        <tr>
                                                            <th>Adresa</th>
                                                            <th>Datum</th>
                                                            <th>Status</th>
                                                            <th>Napomena</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        {selectedRow.details.map(detail => (
                                                            <tr key={`${detail.mailboxId}-${detail.routeDate}-${detail.status}`}>
                                                                <td>{detail.address}</td>
                                                                <td>{formatDate(detail.routeDate)}</td>
                                                                <td>{detail.status}</td>
                                                                <td>{detail.notes ?? "-"}</td>
                                                            </tr>
                                                        ))}
                                                    </tbody>
                                                </table>
                                            </div>
                                        ) : (
                                            <div className="mtr-state">Nema zabilježenih problema za ovaj tip.</div>
                                        )
                                    ) : (
                                        <div className="mtr-state">Odaberite tip sandučića iz tabele za detalje.</div>
                                    )}
                                </section>
                            </>
                        ) : (
                            <div className="mtr-state">Nema podataka za odabrani period.</div>
                        )}
                    </>
                )}

                {loading && !report && <div className="mtr-state">Učitavanje izvještaja...</div>}
            </div>
        </Layout>
    );
}

function firstDayOfMonth(date: Date) {
    return toLocalDateString(new Date(date.getFullYear(), date.getMonth(), 1));
}
