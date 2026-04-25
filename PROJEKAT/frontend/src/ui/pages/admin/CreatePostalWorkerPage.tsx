import { useForm } from "react-hook-form"
import type { CreateUserDto } from "../../../infrastructure/api/users/usersApi"
import { createUser } from "../../../infrastructure/api/users/usersApi"

export function CreatePostalWorkerPage() {
  const {
    register,
    handleSubmit,
    formState: { isSubmitting },
  } = useForm<CreateUserDto>()

  const onSubmit = async (data: CreateUserDto) => {
    await createUser(data)
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
                className="form-field__input"
                placeholder="npr. Amar"
                {...register("firstName")}
              />
            </div>

            <div className="form-field">
              <label className="form-field__label" htmlFor="lastName">
                Prezime
              </label>
              <input
                id="lastName"
                className="form-field__input"
                placeholder="npr. Hodžić"
                {...register("lastName")}
              />
            </div>
          </div>

          <div className="form-field">
            <label className="form-field__label" htmlFor="username">
              Korisničko ime
            </label>
            <input
              id="username"
              className="form-field__input"
              placeholder="npr. amar.hodzic"
              autoComplete="off"
              {...register("username")}
            />
          </div>

          <div className="form-field">
            <label className="form-field__label" htmlFor="email">
              Email adresa
            </label>
            <input
              id="email"
              type="email"
              className="form-field__input"
              placeholder="npr. amar.hodzic@posta.ba"
              autoComplete="off"
              {...register("email")}
            />
          </div>

          <div className="form-field">
            <label className="form-field__label" htmlFor="password">
              Početna lozinka
            </label>
            <input
              id="password"
              type="password"
              className="form-field__input"
              placeholder="Minimalno 8 znakova"
              autoComplete="new-password"
              {...register("password")}
            />
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
