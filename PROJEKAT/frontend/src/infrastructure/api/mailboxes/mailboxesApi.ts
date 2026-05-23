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
    notes?: string | null
    priority?: MailboxPriority
    isAlwaysAvailable: boolean
    slot1Start: string | null
    slot1End: string | null
    slot2Start: string | null
    slot2End: string | null
    workingDays: MailboxWorkingDays
}

export interface UpdateMailboxRequest {
    serialNumber: string
    address: string
    latitude: number
    longitude: number
    type: MailboxType
    capacity: number
    installationYear: number
    notes?: string | null
    priority?: MailboxPriority
    reason?: string
    isAlwaysAvailable: boolean
    slot1Start: string | null
    slot1End: string | null
    slot2Start: string | null
    slot2End: string | null
    workingDays: MailboxWorkingDays
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
    workingDays: MailboxWorkingDays
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
    Pun: 1,
    Obraen: 2,
    Napunjen: 3,
    Ispraznjen: 4
} as const
export type MailboxStatus = typeof MailboxStatus[keyof typeof MailboxStatus]

export const mailboxStatusLabels: Record<MailboxStatus, string> = {
    [MailboxStatus.Prazan]: "Prazan",
    [MailboxStatus.Pun]: "Pun",
    [MailboxStatus.Obraen]: "Obrađen",
    [MailboxStatus.Napunjen]: "Napunjen",
    [MailboxStatus.Ispraznjen]: "Ispraznjen"
}

// US-33: Radni dani sandučića (flags enum)
export const MailboxWorkingDays = {
    None: 0,
    Ponedjeljak: 1,
    Utorak: 2,
    Srijeda: 4,
    Cetvrtak: 8,
    Petak: 16,
    Subota: 32,
    Nedjelja: 64,
    RadniDani: 31, // Mon-Fri (1|2|4|8|16)
    Vikend: 96, // Sat-Sun (32|64)
    SvakiDan: 127 // All days (1|2|4|8|16|32|64)
} as const
export type MailboxWorkingDays = typeof MailboxWorkingDays[keyof typeof MailboxWorkingDays]

export const workingDayLabels: Record<string, string> = {
    "Ponedjeljak": "Ponedjeljak",
    "Utorak": "Utorak",
    "Srijeda": "Srijeda",
    "Cetvrtak": "Četvrtak",
    "Petak": "Petak",
    "Subota": "Subota",
    "Nedjelja": "Nedjelja"
}

export const workingDayBits = [
    { name: "Ponedjeljak", bit: 1 },
    { name: "Utorak", bit: 2 },
    { name: "Srijeda", bit: 4 },
    { name: "Cetvrtak", bit: 8 },
    { name: "Petak", bit: 16 },
    { name: "Subota", bit: 32 },
    { name: "Nedjelja", bit: 64 }
]

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
        notes: request.notes ?? null,
        isAlwaysAvailable: request.isAlwaysAvailable,
        slot1Start: request.slot1Start,
        slot1End: request.slot1End,
        slot2Start: request.slot2Start,
        slot2End: request.slot2End,
        workingDays: request.workingDays,
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
        notes: request.notes ?? null,
        reason: request.reason,
        isAlwaysAvailable: request.isAlwaysAvailable,
        slot1Start: request.slot1Start,
        slot1End: request.slot1End,
        slot2Start: request.slot2Start,
        slot2End: request.slot2End,
        workingDays: request.workingDays,
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

export interface UpdateMailboxStatusRequest {
    status: MailboxStatus
    reason?: string
}

export async function updateMailboxStatus(
    id: string,
    request: UpdateMailboxStatusRequest
): Promise<MailboxResponse> {
    const response = await httpClient<MailboxResponse>(`/api/mailboxes/${id}/status`, {
        method: "PATCH",
        body: request
    })
    if (response.error || !response.data) {
        throw new Error(response.error || "Greška pri ažuriranju statusa sandučića")
    }
    return response.data
}
