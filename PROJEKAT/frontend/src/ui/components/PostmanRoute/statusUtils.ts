import type { RouteItemResponse } from '../../../infrastructure/api/routesApi';

export const STATUS_ALREADY_RECORDED_MESSAGE =
    'Status je već evidentiran. Kontaktirajte dispečera za ispravku.';

export const normalizeStatus = (status?: string | null) =>
    (status ?? '').trim().toLowerCase();

export const isProcessedMailboxStatus = (status?: string | null) => {
    const normalized = normalizeStatus(status);
    return normalized === 'obrađen' ||
        normalized === 'obrađeno' ||
        normalized === 'obradjen' ||
        normalized === 'obradeno' ||
        normalized === 'obraen' ||
        normalized === 'napunjen' ||
        normalized === 'ispraznjen' ||
        normalized === 'obraä‘en' ||
        normalized === 'obraä‘eno';
};

export const isUnavailableStatus = (status?: string | null) =>
    normalizeStatus(status) === 'nedostupan';

export const isRouteItemProcessed = (item: RouteItemResponse) =>
    Boolean(item.processedAt) ||
    isProcessedMailboxStatus(item.processedStatus) ||
    isProcessedMailboxStatus(item.mailboxStatus) ||
    isProcessedMailboxStatus(item.status);

export const isRouteItemUnavailable = (item: RouteItemResponse) =>
    isUnavailableStatus(item.status) || isUnavailableStatus(item.mailboxStatus);

export const getVisitStatusClass = (item: RouteItemResponse) => {
    if (isRouteItemProcessed(item)) return 'processed';
    if (isRouteItemUnavailable(item)) return 'unavailable';
    return 'pending';
};

export const getVisitStatusLabel = (item: RouteItemResponse) => {
    if (isRouteItemProcessed(item)) return 'Obrađen';
    if (isRouteItemUnavailable(item)) return 'Nedostupan';
    const normalized = normalizeStatus(item.status);
    if (normalized === 'planirano' || normalized === 'čeka' || normalized === 'ceka') {
        return 'Čeka';
    }
    return item.status || 'Čeka';
};

export const getVisitStatusIcon = (item: RouteItemResponse) => {
    if (isRouteItemProcessed(item)) return '✓';
    if (isRouteItemUnavailable(item)) return '✕';
    return '○';
};

export const getMailboxStatusLabel = (status?: string | null) => {
    const normalized = normalizeStatus(status);
    if (normalized === 'pun') return 'Pun';
    if (normalized === 'prazan' || normalized === '') return 'Prazan';
    if (normalized === 'napunjen') return 'Napunjen';
    if (normalized === 'ispraznjen') return 'Ispraznjen';
    if (isProcessedMailboxStatus(status)) return 'Obrađen';
    return status ?? 'Prazan';
};

export const getRouteStatusLabel = (status: string) => {
    if (status === 'Dodijeljena') return 'Dodijeljena';
    if (status === 'UProgresu') return 'U toku';
    if (status === 'Zavrsena') return 'Završena';
    if (status === 'Planirana') return 'Planirana';
    if (status === 'Otkazana') return 'Otkazana';
    return status;
};
