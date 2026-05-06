import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { useNavigate } from "react-router-dom"
import { toast } from "sonner"
import { z } from "zod"
import { createBox } from "../../../infrastructure/api/mailboxes/mailboxesApi"
import { Layout } from "../../../ui/components/Layout/Layout"

const boxTypes = [
	"Zidni (mali)",
	"Samostojeći (veliki)",
	"Unutrašnji (stambene zgrade)",
	"Specijalni (prioritetni)",
] as const;

const schema = z.object({
	address: z
		.string()
		.min(5, "Adresa mora imati najmanje 5 znakova")
		.max(255, "Adresa može imati najviše 255 znakova"),
	latitude: z
		.number()
		.min(-90, "Latitude mora biti između -90 i 90")
		.max(90, "Latitude mora biti između -90 i 90"),
	longitude: z
		.number()
		.min(-180, "Longitude mora biti između -180 i 180")
		.max(180, "Longitude mora biti između -180 i 180"),
	type: z.enum(boxTypes as any).refine(val => val !== "", "Odabir tipa sandučića je obavezan"),
	serialNumber: z
		.string()
		.min(3, "Serijski broj mora imati najmanje 3 znaka")
		.max(50, "Serijski broj može imati najviše 50 znakova")
		.regex(/^[a-zA-Z0-9\-_]+$/, "Dozvoljeni znakovi: slova, brojevi, crtica i donja crtica"),
	capacity: z.number().int().min(1, "Kapacitet mora biti najmanje 1"),
	yearOfInstallation: z
		.number()
		.int()
		.min(1900, "Godina instalacije mora biti 1900 ili novije")
		.max(new Date().getFullYear(), "Godina instalacije ne može biti u budućnosti"),
})

type FormData = z.infer<typeof schema>

export function CreateMailboxPage() {
	const {
		register,
		handleSubmit,
		setError,
		reset,
		formState: { isSubmitting, errors },
	} = useForm<FormData>({ resolver: zodResolver(schema) })

	const navigate = useNavigate()

	const onSubmit = async (data: FormData) => {
		const result = await createBox(data)

		if (result.status === 201) {
			toast.success("Sandučić uspješno kreiran", {
				description: `Serijski broj: ${data.serialNumber}`,
			})
			reset()
			navigate("/admin/mailboxes")
			return
		}

		if (result.status === 409) {
			setError("serialNumber", { message: result.error || "Sandučić sa ovim serijskim brojem već postoji" })
			toast.error("Greška pri kreiranju", {
				description: "Serijski broj je već registrovan.",
			})
			return
		}

		toast.error("Greška pri kreiranju sandučića", {
			description: result.error ?? "Neočekivana greška servera. Pokušajte ponovo.",
		})
	}

	return (
		<Layout>
			<div className="page-container">
				<form onSubmit={handleSubmit(onSubmit)} noValidate className="form-card">
					<div className="form-card__header">
						<h1 className="form-card__title">Dodaj novi sandučić</h1>
					</div>

					<div className="form-card__body">
						<div className="form-field">
							<label htmlFor="address" className="form-field__label">
								Adresa
							</label>
							<input
								id="address"
								type="text"
								placeholder="npr. Ulica 123, Grad"
								className={`form-field__input${errors.address ? " form-field__input--error" : ""}`}
								{...register("address")}
							/>
							{errors.address && <p className="form-field__error">{errors.address.message}</p>}
						</div>

						<div className="form-row">
							<div className="form-field">
								<label htmlFor="latitude" className="form-field__label">
									Latitude
								</label>
								<input
									id="latitude"
									type="number"
									step="0.000001"
									placeholder="npr. 43.856301"
									className={`form-field__input${errors.latitude ? " form-field__input--error" : ""}`}
									{...register("latitude", { valueAsNumber: true })}
								/>
								{errors.latitude && <p className="form-field__error">{errors.latitude.message}</p>}
							</div>

							<div className="form-field">
								<label htmlFor="longitude" className="form-field__label">
									Longitude
								</label>
								<input
									id="longitude"
									type="number"
									step="0.000001"
									placeholder="npr. 18.412776"
									className={`form-field__input${errors.longitude ? " form-field__input--error" : ""}`}
									{...register("longitude", { valueAsNumber: true })}
								/>
								{errors.longitude && <p className="form-field__error">{errors.longitude.message}</p>}
							</div>
						</div>

						<div className="form-field">
							<label htmlFor="type" className="form-field__label">
								Tip sandučića
							</label>
							<select
								id="type"
								className={`form-field__input${errors.type ? " form-field__input--error" : ""}`}
								{...register("type")}
							>
								<option value="">-- Odaberi tip --</option>
								{boxTypes.map((type) => (
									<option key={type} value={type}>
										{type}
									</option>
								))}
							</select>
							{errors.type && <p className="form-field__error">{(errors.type as any)?.message}</p>}
						</div>

						<div className="form-field">
							<label htmlFor="serialNumber" className="form-field__label">
								Serijski broj
							</label>
							<input
								id="serialNumber"
								type="text"
								placeholder="npr. SB-2024-001"
								className={`form-field__input${errors.serialNumber ? " form-field__input--error" : ""}`}
								{...register("serialNumber")}
							/>
							{errors.serialNumber && <p className="form-field__error">{errors.serialNumber.message}</p>}
						</div>

						<div className="form-row">
							<div className="form-field">
								<label htmlFor="capacity" className="form-field__label">
									Kapacitet
								</label>
								<input
									id="capacity"
									type="number"
									min="1"
									placeholder="npr. 50"
									className={`form-field__input${errors.capacity ? " form-field__input--error" : ""}`}
									{...register("capacity", { valueAsNumber: true })}
								/>
								{errors.capacity && <p className="form-field__error">{errors.capacity.message}</p>}
							</div>

							<div className="form-field">
								<label htmlFor="yearOfInstallation" className="form-field__label">
									Godina instalacije
								</label>
								<input
									id="yearOfInstallation"
									type="number"
									min="1900"
									max={new Date().getFullYear()}
									placeholder="npr. 2024"
									className={`form-field__input${errors.yearOfInstallation ? " form-field__input--error" : ""}`}
									{...register("yearOfInstallation", { valueAsNumber: true })}
								/>
								{errors.yearOfInstallation && (
									<p className="form-field__error">{errors.yearOfInstallation.message}</p>
								)}
							</div>
						</div>
					</div>

					<div className="form-card__footer">
						<button
							type="button"
							className="btn btn--secondary"
							onClick={() => navigate(-1)}
							disabled={isSubmitting}
						>
							Odustani
						</button>
						<button type="submit" className="btn btn--primary" disabled={isSubmitting}>
							{isSubmitting ? "Kreiranje…" : "Kreiraj sandučić"}
						</button>
					</div>
				</form>
			</div>
		</Layout>
	)
}
