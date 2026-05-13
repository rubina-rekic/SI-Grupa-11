import { z } from "zod"
import { MailboxWorkingDays } from "../api/mailboxes/mailboxesApi"

// US-33: Radni dani validacija
export const workingDaysSchema = z.object({
    workingDays: z.number().int().min(0).max(MailboxWorkingDays.SvakiDan),
}).superRefine((data, ctx) => {
    // Validate that at least one day is selected
    if (data.workingDays === MailboxWorkingDays.None) {
        ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Sandučić mora imati barem jedan definisan radni dan",
            path: ["workingDays"]
        })
    }
})
