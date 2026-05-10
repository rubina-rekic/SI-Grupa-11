import { z } from "zod"

export const availabilitySchema = z.object({
    isAlwaysAvailable: z.boolean().default(false),
    hasSecondSlot: z.boolean().default(false),
    slot1Start: z.string().optional(),
    slot1End: z.string().optional(),
    slot2Start: z.string().optional(),
    slot2End: z.string().optional(),
}).superRefine((data, ctx) => {
    if (data.isAlwaysAvailable) return

    const toMinutes = (t: string) => {
        const [h, m] = t.split(":").map(Number)
        return h * 60 + m
    }

    const s1HasStart = !!data.slot1Start
    const s1HasEnd = !!data.slot1End

    if (s1HasStart !== s1HasEnd) {
        ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Morate unijeti i početak i kraj prvog termina.",
            path: [s1HasStart ? "slot1End" : "slot1Start"]
        })
        return
    }

    if (s1HasStart && s1HasEnd) {
        if (toMinutes(data.slot1End!) <= toMinutes(data.slot1Start!)) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Krajnje vrijeme mora biti nakon početnog.",
                path: ["slot1End"]
            })
        }
    }

    if (!data.hasSecondSlot) return

    const s2HasStart = !!data.slot2Start
    const s2HasEnd = !!data.slot2End

    if (!s1HasStart) {
        ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Drugi termin zahtijeva da je prvi termin definisan.",
            path: ["slot2Start"]
        })
        return
    }

    if (s2HasStart !== s2HasEnd) {
        ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Morate unijeti i početak i kraj drugog termina.",
            path: [s2HasStart ? "slot2End" : "slot2Start"]
        })
        return
    }

    if (s2HasStart && s2HasEnd) {
        if (toMinutes(data.slot2End!) <= toMinutes(data.slot2Start!)) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Krajnje vrijeme drugog termina mora biti nakon početnog.",
                path: ["slot2End"]
            })
        }

        if (s1HasEnd && toMinutes(data.slot2Start!) < toMinutes(data.slot1End!)) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Drugi termin se preklapa s prvim.",
                path: ["slot2Start"]
            })
        }
    }
})

export function mapAvailabilityToRequest(data: {
    isAlwaysAvailable: boolean
    hasSecondSlot: boolean
    slot1Start?: string
    slot1End?: string
    slot2Start?: string
    slot2End?: string
}) {
    if (data.isAlwaysAvailable) {
        return {
            isAlwaysAvailable: true,
            slot1Start: null,
            slot1End: null,
            slot2Start: null,
            slot2End: null,
        }
    }

    return {
        isAlwaysAvailable: false,
        slot1Start: data.slot1Start || null,
        slot1End: data.slot1End || null,
        slot2Start: data.hasSecondSlot ? (data.slot2Start || null) : null,
        slot2End: data.hasSecondSlot ? (data.slot2End || null) : null,
    }
}