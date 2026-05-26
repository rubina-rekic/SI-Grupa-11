import { httpClient } from "../httpClient"

export const IssueStatus = {
    Otvoren: 0,
    UObradi: 1,
    Rijesen: 2,
} as const
export type IssueStatus = typeof IssueStatus[keyof typeof IssueStatus]

export const issueStatusLabels: Record<IssueStatus, string> = {
    [IssueStatus.Otvoren]: "Otvoren",
    [IssueStatus.UObradi]: "U obradi",
    [IssueStatus.Rijesen]: "Riješen",
}

export const IssueAction = {
    PonovniPokusaj: 0,
    DrugiPostar: 1,
    OdgodaZasutra: 2,
} as const
export type IssueAction = typeof IssueAction[keyof typeof IssueAction]

export const issueActionLabels: Record<IssueAction, string> = {
    [IssueAction.PonovniPokusaj]: "Ponovni pokušaj",
    [IssueAction.DrugiPostar]: "Dodijeli drugom poštaru",
    [IssueAction.OdgodaZasutra]: "Ostavi za naredni dan",
}

export interface IssueCommentDto {
    id: string
    authorId: string
    authorUsername: string
    authorRole: string
    content: string
    createdAt: string
}

export interface IssueTimelineEntry {
    type: "created" | "comment" | "action" | "status"
    description: string
    actorUsername: string | null
    timestamp: string
}

export interface IssueDto {
    id: string
    routeItemId: string
    mailboxId: string
    mailboxAddress: string
    mailboxSerialNumber: string
    reportedByUserId: string
    reportedByUsername: string
    unavailableReason: string | null
    status: IssueStatus
    statusLabel: string
    assignedAction: IssueAction | null
    assignedActionLabel: string | null
    actionAssignedToUserId: string | null
    actionAssignedToUsername: string | null
    createdAt: string
    updatedAt: string
    comments: IssueCommentDto[]
    timeline: IssueTimelineEntry[]
}

export interface IssueSummaryDto {
    id: string
    routeItemId: string
    mailboxId: string
    mailboxAddress: string
    reportedByUsername: string
    unavailableReason: string | null
    status: IssueStatus
    statusLabel: string
    assignedAction: IssueAction | null
    createdAt: string
}

export interface IssueNotificationDto {
    id: string
    issueId: string
    mailboxAddress: string
    title: string
    message: string
    isRead: boolean
    createdAt: string
}

export async function getAllIssues(status?: IssueStatus): Promise<IssueSummaryDto[]> {
    const params = status !== undefined ? `?status=${status}` : ""
    const res = await httpClient<IssueSummaryDto[]>(`/api/issues${params}`)
    if (res.error || !res.data) throw new Error(res.error || "Greška pri učitavanju problema")
    return res.data
}

export async function getIssueById(id: string): Promise<IssueDto> {
    const res = await httpClient<IssueDto>(`/api/issues/${id}`)
    if (res.error || !res.data) throw new Error(res.error || "Greška pri učitavanju problema")
    return res.data
}

export async function addComment(issueId: string, content: string): Promise<IssueDto> {
    const res = await httpClient<IssueDto>(`/api/issues/${issueId}/comments`, {
        method: "POST",
        body: { content }
    })
    if (res.error || !res.data) throw new Error(res.error || "Greška pri dodavanju komentara")
    return res.data
}

export async function assignAction(
    issueId: string,
    action: IssueAction,
    targetPostmanId?: string
): Promise<IssueDto> {
    const body: { action: IssueAction; targetPostmanId?: string | null } = { action }
    if (action === IssueAction.DrugiPostar) {
        body.targetPostmanId = targetPostmanId ?? null
    }

    const res = await httpClient<IssueDto>(`/api/issues/${issueId}/action`, {
        method: "PUT",
        body
    })
    if (res.error || !res.data) throw new Error(res.error || "Greška pri dodjeli akcije")
    return res.data
}

export async function resolveIssue(issueId: string): Promise<IssueDto> {
    const res = await httpClient<IssueDto>(`/api/issues/${issueId}/resolve`, { method: "PUT" })
    if (res.error || !res.data) throw new Error(res.error || "Greška pri rješavanju problema")
    return res.data
}

export async function getMyNotifications(): Promise<IssueNotificationDto[]> {
    const res = await httpClient<IssueNotificationDto[]>("/api/issues/my-notifications")
    if (res.error || !res.data) throw new Error(res.error || "Greška pri učitavanju notifikacija")
    return res.data
}

export async function markNotificationRead(notificationId: string): Promise<void> {
    await httpClient(`/api/issues/notifications/${notificationId}/read`, { method: "PUT" })
}

export async function getIssueByIdForWorker(id: string): Promise<IssueDto> {
    const res = await httpClient<IssueDto>(`/api/issues/worker/${id}`)
    if (res.error || !res.data) throw new Error(res.error || "Greška pri učitavanju problema")
    return res.data
}