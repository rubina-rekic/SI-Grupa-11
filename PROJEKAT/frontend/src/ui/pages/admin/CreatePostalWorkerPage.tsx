import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { z } from "zod"
import { createUser } from "../../../infrastructure/api/users/usersApi"
import { PasswordStrengthIndicator } from "../../components/common/PasswordStrengthIndicator"

const schema = z.object({
  firstName: z
    .string()
    .min(1, "Ime je obavezno")
    .max(50, "Ime može imati najviše 50 znakova"),
  lastName: z
    .string()
    .min(1, "Prezime je obavezno")
    .max(50, "Prezime može imati najviše 50 znakova"),
  username: z
    .string()
    .min(3, "Korisničko ime mora imati najmanje 3 znaka")
    .max(30, "Korisničko ime može imati najviše 30 znakova")
    .regex(
      /^[a-zA-Z0-9._-]+$/,
      "Dozvoljeni znakovi: slova, brojevi, tačka, crtica, donja crtica",
    ),
  email: z
    .string()
    .min(1, "Email je obavezan")
    .email("Unesite ispravnu email adresu")
    .max(100, "Email može imati najviše 100 znakova"),
  password: z
    .string()
    .min(8, "Lozinka mora imati najmanje 8 znakova")
    .max(128, "Lozinka može imati najviše 128 znakova")
    .regex(/[A-Z]/, "Lozinka mora sadržavati najmanje jedno veliko slovo")
    .regex(/\d/, "Lozinka mora sadržavati najmanje jedan broj"),
})

type FormData = z.infer<typeof schema>

export function CreatePostalWorkerPage() {
  const {
    register,
    handleSubmit,
    watch,
    setError,
    formState: { isSubmitting, errors },
  } = useForm<FormData>({ resolver: zodResolver(schema) })

  const passwordValue = watch("password") ?? ""

  const onSubmit = async (data: FormData) => {
    const result = await createUser(data)

    if (result.status === 409) {
      if (result.error?.includes("Email")) {
        setError("email", { message: result.error })
      } else {
        setError("username", { message: result.error })
      }
    }
  }

  return (
    <div className="page-container">
      <div className="form-card">
        <div className="form-card__header">
          <h1 className="form-card__title">Kreiranje računa poštara</h1>
          <p className="form-card__subtitle">
            Novi korisnički račun bit će kreiran s ulogom{" "}
            <span className="role-badge">Poštar</span>. Poštar će morati
            promijeniti lozinku pri prvoj prijavi.
          </p>
        </div>

        <form className="form-card__body" onSubmit={handleSubmit(onSubmit)}>
          <div className="form-row">
            <div className="form-field">
              <label className="form-field__label" htmlFor="firstName">
                Ime
              </label>
              <input
                id="firstName"
                className={`form-field__input${errors.firstName ? " form-field__input--error" : ""}`}
                placeholder="npr. Amar"
                {...register("firstName")}
              />
              {errors.firstName && (
                <p className="form-field__error">{errors.firstName.message}</p>
              )}
            </div>

            <div className="form-field">
              <label className="form-field__label" htmlFor="lastName">
                Prezime
              </label>
              <input
                id="lastName"
                className={`form-field__input${errors.lastName ? " form-field__input--error" : ""}`}
                placeholder="npr. Hodžić"
                {...register("lastName")}
              />
              {errors.lastName && (
                <p className="form-field__error">{errors.lastName.message}</p>
              )}
            </div>
          </div>

          <div className="form-field">
            <label className="form-field__label" htmlFor="username">
              Korisničko ime
            </label>
            <input
              id="username"
              className={`form-field__input${errors.username ? " form-field__input--error" : ""}`}
              placeholder="npr. amar.hodzic"
              autoComplete="off"
              {...register("username")}
            />
            {errors.username && (
              <p className="form-field__error">{errors.username.message}</p>
            )}
          </div>

          <div className="form-field">
            <label className="form-field__label" htmlFor="email">
              Email adresa
            </label>
            <input
              id="email"
              type="email"
              className={`form-field__input${errors.email ? " form-field__input--error" : ""}`}
              placeholder="npr. amar.hodzic@posta.ba"
              autoComplete="off"
              {...register("email")}
            />
            {errors.email && (
              <p className="form-field__error">{errors.email.message}</p>
            )}
          </div>

          <div className="form-field">
            <label className="form-field__label" htmlFor="password">
              Početna lozinka
            </label>
            <input
              id="password"
              type="password"
              className={`form-field__input${errors.password ? " form-field__input--error" : ""}`}
              placeholder="Min. 8 znakova, veliko slovo, broj"
              autoComplete="new-password"
              {...register("password")}
            />
            <PasswordStrengthIndicator password={passwordValue} />
            {errors.password && (
              <p className="form-field__error">{errors.password.message}</p>
            )}
          </div>

          <div className="form-actions">
            <button
              type="submit"
              className="btn btn--primary"
              disabled={isSubmitting}
            >
              {isSubmitting ? "Kreiranje…" : "Kreiraj račun"}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
