import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { toast } from "sonner"
import { z } from "zod"
import { createMailbox, checkSerialNumberExists, MailboxType, mailboxTypeLabels } from "../../../infrastructure/api/mailboxes/mailboxesApi"
import { Layout } from "../../components/Layout/Layout"

const schema = z.object({
    serialNumber: z
        .string()
        .min(1, "Serijski broj je obavezan")
        .max(50, "Serijski broj može imati najviše 50 karaktera")
        .refine(async (value) => {
            if (!value) return true
            const exists = await checkSerialNumberExists(value.trim())
            return !exists
        }, "Sandučić sa ovim serijskim brojem već postoji"),
    address: z
        .string()
        .min(1, "Adresa je obavezna")
        .max(200, "Adresa može imati najviše 200 karaktera"),
    latitude: z
        .number()
        .min(-90, "Latitude mora biti između -90 i 90")
        .max(90, "Latitude mora biti između -90 i 90"),
    longitude: z
        .number()
        .min(-180, "Longitude mora biti između -180 i 180")
        .max(180, "Longitude mora biti između -180 i 180"),
    type: z.nativeEnum(MailboxType).refine((val) => val !== undefined, {
        message: "Odabir tipa sandučića je obavezan"
    }),
    capacity: z
        .number()
        .min(1, "Kapacitet mora biti veći od 0")
        .max(10000, "Kapacitet mora biti realan"),
    installationYear: z
        .number()
        .min(1900, "Godina instalacije mora biti nakon 1900")
        .max(new Date().getFullYear() + 10, "Godina instalacije mora biti realna"),
    notes: z
        .string()
        .max(500, "Napomene mogu imati najviše 500 karaktera")
        .optional()
})

type FormData = z.infer<typeof schema>

export function CreateMailboxPage() {
    const {
        register,
        handleSubmit,
        formState: { errors, isSubmitting },
        setValue,
        trigger,
        watch
    } = useForm<FormData>({
        resolver: zodResolver(schema),
        mode: "onChange"
    })

    const watchedType = watch("type")

    // Check serial number uniqueness when it changes
    const handleSerialNumberChange = async (value: string) => {
        setValue("serialNumber", value)
        if (value.length >= 3) {
            await trigger("serialNumber")
        }
    }

    const onSubmit = async (data: FormData) => {
        try {
            const mailbox = await createMailbox({
                serialNumber: data.serialNumber.trim(),
                address: data.address.trim(),
                latitude: data.latitude,
                longitude: data.longitude,
                type: data.type,
                capacity: data.capacity,
                installationYear: data.installationYear,
                notes: data.notes?.trim()
            })

            toast.success(`Sandučić ${mailbox.serialNumber} uspješno dodan!`)
            
            // Reset form
            setValue("serialNumber", "")
            setValue("address", "")
            setValue("latitude", 0)
            setValue("longitude", 0)
            setValue("type", MailboxType.WallSmall)
            setValue("capacity", 100)
            setValue("installationYear", new Date().getFullYear())
            setValue("notes", "")
            
        } catch (error: any) {
            console.error("Error creating mailbox:", error)
            const message = error.response?.data?.message || "Greška pri kreiranju sandučića"
            toast.error(message)
        }
    }

    const getPinColor = (type: MailboxType): string => {
        switch (type) {
            case MailboxType.WallSmall:
                return "bg-blue-500"
            case MailboxType.StandaloneLarge:
                return "bg-green-500"
            case MailboxType.IndoorResidential:
                return "bg-yellow-500"
            case MailboxType.SpecialPriority:
                return "bg-red-500"
            default:
                return "bg-gray-500"
        }
    }

    return (
        <Layout>
            <div className="max-w-2xl mx-auto p-6">
                <h1 className="text-3xl font-bold text-gray-900 mb-8">Dodaj novi sandučić</h1>
                
                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                    {/* Tip sandučića */}
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Tip sandučića *
                        </label>
                        <select
                            {...register("type", { valueAsNumber: true })}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                        >
                            <option value="">Odaberite tip sandučića</option>
                            {Object.entries(mailboxTypeLabels).map(([value, label]) => (
                                <option key={value} value={value}>
                                    {label}
                                </option>
                            ))}
                        </select>
                        {errors.type && (
                            <p className="mt-1 text-sm text-red-600">{errors.type.message}</p>
                        )}
                    </div>

                    {/* Serijski broj */}
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Serijski broj *
                        </label>
                        <input
                            type="text"
                            {...register("serialNumber")}
                            onChange={(e) => handleSerialNumberChange(e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                            placeholder="Unesite serijski broj sandučića"
                        />
                        {errors.serialNumber && (
                            <p className="mt-1 text-sm text-red-600">{errors.serialNumber.message}</p>
                        )}
                    </div>

                    {/* Adresa */}
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Adresa *
                        </label>
                        <input
                            type="text"
                            {...register("address")}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                            placeholder="Unesite adresu sandučića"
                        />
                        {errors.address && (
                            <p className="mt-1 text-sm text-red-600">{errors.address.message}</p>
                        )}
                    </div>

                    {/* Koordinate */}
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Latitude *
                            </label>
                            <input
                                type="number"
                                step="any"
                                {...register("latitude", { valueAsNumber: true })}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                                placeholder="npr. 43.8563"
                            />
                            {errors.latitude && (
                                <p className="mt-1 text-sm text-red-600">{errors.latitude.message}</p>
                            )}
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Longitude *
                            </label>
                            <input
                                type="number"
                                step="any"
                                {...register("longitude", { valueAsNumber: true })}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                                placeholder="npr. 18.4131"
                            />
                            {errors.longitude && (
                                <p className="mt-1 text-sm text-red-600">{errors.longitude.message}</p>
                            )}
                        </div>
                    </div>

                    {/* Kapacitet i Godina instalacije */}
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Kapacitet *
                            </label>
                            <input
                                type="number"
                                {...register("capacity", { valueAsNumber: true })}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                                placeholder="Broj pisama"
                            />
                            {errors.capacity && (
                                <p className="mt-1 text-sm text-red-600">{errors.capacity.message}</p>
                            )}
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Godina instalacije *
                            </label>
                            <input
                                type="number"
                                {...register("installationYear", { valueAsNumber: true })}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                                placeholder="npr. 2023"
                            />
                            {errors.installationYear && (
                                <p className="mt-1 text-sm text-red-600">{errors.installationYear.message}</p>
                            )}
                        </div>
                    </div>

                    {/* Napomene */}
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Napomene (opciono)
                        </label>
                        <textarea
                            {...register("notes")}
                            rows={3}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                            placeholder="Dodatne napomene o sandučiću..."
                        />
                        {errors.notes && (
                            <p className="mt-1 text-sm text-red-600">{errors.notes.message}</p>
                        )}
                    </div>

                    {/* Prikaz tipa pine na mapi */}
                    {watchedType && (
                        <div className="bg-gray-50 p-4 rounded-md">
                            <p className="text-sm text-gray-600 mb-2">Prikaz na mapi:</p>
                            <div className="flex items-center space-x-2">
                                <div className={`w-4 h-4 rounded-full ${getPinColor(watchedType)}`}></div>
                                <span className="text-sm font-medium">{mailboxTypeLabels[watchedType]}</span>
                            </div>
                        </div>
                    )}

                    {/* Dugmad */}
                    <div className="flex justify-end space-x-4">
                        <button
                            type="button"
                            onClick={() => window.history.back()}
                            className="px-4 py-2 text-gray-700 bg-gray-200 rounded-md hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-500"
                        >
                            Otkaži
                        </button>
                        <button
                            type="submit"
                            disabled={isSubmitting}
                            className="px-6 py-2 text-white bg-blue-600 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            {isSubmitting ? "Sačuvavanje..." : "Sačuvaj sandučić"}
                        </button>
                    </div>
                </form>
            </div>
        </Layout>
    )
}
