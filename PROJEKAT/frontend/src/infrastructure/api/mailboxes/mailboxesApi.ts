import { httpClient } from "../httpClient"

export interface CreateMailboxRequest {
    serialNumber: string
    address: string
    latitude: number
    longitude: number
    type: MailboxType
    capacity: number
    installationYear: number
    notes?: string
}

export interface MailboxResponse {
    id: string
    serialNumber: string
    address: string
    latitude: number
    longitude: number
    type: MailboxType
    capacity: number
    installationYear: number
    createdAt: string
    updatedAt: string
    notes?: string
}

export const MailboxType = {
    WallSmall: 1,
    StandaloneLarge: 2,
    IndoorResidential: 3,
    SpecialPriority: 4
} as const

export type MailboxType = typeof MailboxType[keyof typeof MailboxType]

export const mailboxTypeLabels = {
    [MailboxType.WallSmall]: "Zidni (mali)",
    [MailboxType.StandaloneLarge]: "Samostojeći (veliki)",
    [MailboxType.IndoorResidential]: "Unutrašnji (stambene zgrade)",
    [MailboxType.SpecialPriority]: "Specijalni (prioritetni)"
}

export async function createMailbox(request: CreateMailboxRequest): Promise<MailboxResponse> {
    const response = await httpClient("/mailboxes", {
        method: "POST",
        body: request
    })
    if (response.error || !response.data) {
        throw new Error(response.error || "Greška pri kreiranju sandučića")
    }
    return response.data as MailboxResponse
}

export async function checkSerialNumberExists(serialNumber: string): Promise<boolean> {
    const response = await httpClient(`/mailboxes/check-serial-number/${encodeURIComponent(serialNumber)}`)
    if (response.error || response.data === null) {
        throw new Error(response.error || "Greška pri provjeri serijskog broja")
    }
    return response.data as boolean
}

export async function getAllMailboxes(): Promise<MailboxResponse[]> {
    const response = await httpClient("/mailboxes")
    if (response.error || !response.data) {
        throw new Error(response.error || "Greška pri učitavanju sandučića")
    }
    return response.data as MailboxResponse[]
}
