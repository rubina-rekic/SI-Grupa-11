import { httpClient } from "../httpClient"
import type { ApiResponse } from "../../../shared/types/api"

export interface CreateMailboxRequest {
    serialNumber: string
    address: string
    latitude: number
    longitude: number
    type: MailboxType
    capacity: number
    installationYear: number
    notes?: string
    priority?: MailboxPriority
    isAlwaysAvailable: boolean
    slot1Start: string | null
    slot1End: string | null
    slot2Start: string | null
    slot2End: string | null
}

export interface UpdateMailboxRequest {
    serialNumber: string
    address: string
    latitude: number
    longitude: number
    type: MailboxType
    capacity: number
    installationYear: number
    notes?: string
    priority?: MailboxPriority
    reason?: string
    isAlwaysAvailable: boolean
    slot1Start: string | null
    slot1End: string | null
    slot2Start: string | null
    slot2End: string | null
}

export interface MailboxResponse {
    id: string
    serialNumber: string
    address: string
    latitude: number
    longitude: number
    type: MailboxType
    priority: MailboxPriority
    status: MailboxStatus
    capacity: number
    installationYear: number
    createdAt: string
    updatedAt: string
    notes?: string
    isAlwaysAvailable: boolean
    slot1Start: string | null
    slot1End: string | null
    slot2Start: string | null
    slot2End: string | null
}

export interface PagedResponse<T> {
    items: T[]
    totalCount: number
    page: number
    pageSize: number
    totalPages: number
}

export const MailboxType = {
    WallSmall: 1,
    StandaloneLarge: 2,
    IndoorResidential: 3,
    SpecialPriority: 4
} as const
export type MailboxType = typeof MailboxType[keyof typeof MailboxType]

export const mailboxTypeLabels: Record<MailboxType, string> = {
    [MailboxType.WallSmall]: "Zidni (mali)",
    [MailboxType.StandaloneLarge]: "Samostojeci (veliki)",
    [MailboxType.IndoorResidential]: "Unutrasnji (stambene zgrade)",
    [MailboxType.SpecialPriority]: "Specijalni (prioritetni)"
}

export const MailboxPriority = {
    Visok: 1,
    Srednji: 2,
    Nizak: 3
} as const
export type MailboxPriority = typeof MailboxPriority[keyof typeof MailboxPriority]

export const mailboxPriorityLabels: Record<MailboxPriority, string> = {
    [MailboxPriority.Visok]: "Visok",
    [MailboxPriority.Srednji]: "Srednji",
    [MailboxPriority.Nizak]: "Nizak"
}

export const MailboxStatus = {
    Prazan: 0,
    Pun: 1
} as const
export type MailboxStatus = typeof MailboxStatus[keyof typeof MailboxStatus]

export const mailboxStatusLabels: Record<MailboxStatus, string> = {
    [MailboxStatus.Prazan]: "Prazan",
    [MailboxStatus.Pun]: "Pun"
}

export interface MailboxListQuery {
    page?: number
    pageSize?: number
    type?: MailboxType
    priority?: MailboxPriority
    search?: string
    sortByPriority?: boolean
}

export async function createMailbox(request: CreateMailboxRequest): Promise<MailboxResponse> {
    const backendRequest = {
        serialNumber: request.serialNumber,
        address: request.address,
        latitude: parseFloat(request.latitude.toFixed(6)),
        longitude: parseFloat(request.longitude.toFixed(6)),
        type: request.type,
        priority: request.priority ?? MailboxPriority.Srednji,
        capacity: request.capacity,
        installationYear: request.installationYear,
        notes: request.notes,
        isAlwaysAvailable: request.isAlwaysAvailable,
        slot1Start: request.slot1Start,
        slot1End: request.slot1End,
        slot2Start: request.slot2Start,
        slot2End: request.slot2End,
    }

    const response = await httpClient("/api/mailboxes", {
        method: "POST",
        body: backendRequest
    })

    if (response.error || !response.data) {
        throw new Error(response.error || "Greska pri kreiranju sanducica")
    }

    return response.data as MailboxResponse
}

export async function checkSerialNumberExists(serialNumber: string): Promise<boolean> {
    const response = await httpClient(`/api/mailboxes/check-serial-number/${encodeURIComponent(serialNumber)}`)
    if (response.error || response.data === null) {
        throw new Error(response.error || "Greska pri provjeri serijskog broja")
    }
    return response.data as boolean
}

export async function getAllMailboxes(query: MailboxListQuery = {}): Promise<PagedResponse<MailboxResponse>> {
    const params = new URLSearchParams()
    params.set("page", String(query.page ?? 1))
    params.set("pageSize", String(query.pageSize ?? 25))
    if (query.type !== undefined) params.set("type", String(query.type))
    if (query.priority !== undefined) params.set("priority", String(query.priority))
    if (query.search) params.set("search", query.search)
    if (query.sortByPriority) params.set("sortByPriority", "true")

    const response = await httpClient(`/api/mailboxes?${params.toString()}`)
    if (response.error || !response.data) {
        throw new Error(response.error || "Greska pri ucitavanju sanducica")
    }
    return response.data as PagedResponse<MailboxResponse>
}

export async function getMailboxById(id: string): Promise<MailboxResponse> {
    const response = await httpClient(`/api/mailboxes/${id}`)
    if (response.error || !response.data) {
        throw new Error(response.error || "Greska pri ucitavanju sanducica")
    }
    return response.data as MailboxResponse
}

export async function updateMailbox(id: string, request: UpdateMailboxRequest): Promise<MailboxResponse> {
    const backendRequest = {
        serialNumber: request.serialNumber,
        address: request.address,
        latitude: parseFloat(request.latitude.toFixed(6)),
        longitude: parseFloat(request.longitude.toFixed(6)),
        type: request.type,
        priority: request.priority ?? MailboxPriority.Srednji,
        capacity: request.capacity,
        installationYear: request.installationYear,
        notes: request.notes,
        reason: request.reason,
        isAlwaysAvailable: request.isAlwaysAvailable,
        slot1Start: request.slot1Start,
        slot1End: request.slot1End,
        slot2Start: request.slot2Start,
        slot2End: request.slot2End,
    }

    const response = await httpClient(`/api/mailboxes/${id}`, {
        method: "PUT",
        body: backendRequest
    })

    if (response.error || !response.data) {
        throw new Error(response.error || "Greska pri azuriranju sanducica")
    }

    return response.data as MailboxResponse
}

export async function deleteMailbox(id: string): Promise<void> {
    const response = await httpClient(`/api/mailboxes/${id}`, { method: "DELETE" })
    if (response.error || (response.status !== 204 && response.status !== 200)) {
        throw new Error(response.error || "Greska pri brisanju sanducica")
    }
}

export interface AuditLogDto {
    id: string
    mailboxId: string
    userId: string
    username: string
    fieldName: string
    oldValue: string | null
    newValue: string | null
    action: string
    reason: string | null
    timestamp: string
}

export function getMailboxHistory(id: string): Promise<ApiResponse<AuditLogDto[]>> {
    return httpClient<AuditLogDto[]>(`/api/mailboxes/${id}/history`, { method: "GET" })
}
