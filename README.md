# PostRoute

Sistem za optimizaciju ruta punjenja i praznjenja postanskih sanducica.

Ovaj repozitorij trenutno sadrzi inicijalni projektni skeleton (foundation), bez implementacije Product Backlog funkcionalnosti.
Cilj trenutnog stanja je da jasno prikaze smjer razvoja i arhitekturu za naredne iteracije.

## Project overview
PostRoute je web aplikacija za podrsku operativnom planiranju i izvrsavanju ruta za postanske sanducice.

Primarni ciljevi:
- smanjenje manuelnog rada dispecera
- bolja vidljivost izvrsenja ruta na terenu
- brza reakcija na operativne incidente
- bolji osnov za izvjestavanje i audit trag

## Current status
Trenutno stanje je inicijalni skeleton:
- backend i frontend osnove su postavljene
- postoji jedan minimalni backend primjer (`User`) kao referentni obrazac slojeva
- nema baze podataka niti stvarne poslovne logike

## Repository structure
- `PROJEKAT/backend`
- `PROJEKAT/frontend`

Detaljnije:
- `PROJEKAT/backend`: ASP.NET Core solution (`PostRoute.sln`) sa slojevima `PostRoute.Api`, `PostRoute.BLL`, `PostRoute.DAL`
- `PROJEKAT/frontend`: React + TypeScript aplikacija strukturirana po slojevima `ui`, `application`, `infrastructure`

## Prerequisites
- .NET SDK 9.x
- Node.js 22+ (ili LTS kompatibilna verzija)
- npm 10+

## Setup and run
### 1. Backend
U prvom terminalu:

```powershell
cd PROJEKAT/backend/src/PostRoute.Api
dotnet restore
dotnet run
```

Napomena:
- Aplikacija starta API host.
- U development okruzenju OpenAPI endpoint je dostupan na `/openapi/v1.json`.

Minimalni primjer endpointa:
- `GET /api/users/{userId}`

### 2. Frontend
U drugom terminalu:

```powershell
cd PROJEKAT/frontend
npm install
npm run dev
```

Frontend ce pokrenuti Vite development server i prikazati skeleton UI.

### 3. Build checks
Backend:

```powershell
cd PROJEKAT/backend
dotnet build PostRoute.sln
```

Frontend:

```powershell
cd PROJEKAT/frontend
npm run build
```

## CI/CD

Pipeline definicije se nalaze u [.github/workflows/](.github/workflows/):

- [backend-ci.yml](.github/workflows/backend-ci.yml) — build, test, publish za .NET backend (+ deploy na main)
- [frontend-ci.yml](.github/workflows/frontend-ci.yml) — lint, build za React frontend (+ deploy na main)

### Branching strategija

- `main` — uvijek stabilna verzija. Ovdje se okida **CI + CD** (deploy).
- `develop` — integraciona grana. Ovdje se okida **samo CI** (PR + merge).
- Feature grane se uvijek otvaraju iz `develop` i mergaju natrag PR-om u `develop`.
- `develop` se rebase-uje na `main` (a ne obrnuto), pošto `main` sadrži najnoviji stabilni kod. Tipičan tok:
  ```bash
  git checkout develop
  git fetch origin
  git rebase origin/main
  git push --force-with-lease origin develop
  ```
  (`--force-with-lease` je sigurnija varijanta od `--force` — neće prepisati tuđe commit-e koji su u međuvremenu stigli.)

### Triggeri

| Događaj | Backend CI | Frontend CI | CD (deploy) |
| --- | --- | --- | --- |
| PR → `develop` | ✅ | ✅ | ❌ |
| Push na `develop` | ✅ | ✅ | ❌ |
| PR → `main` | ✅ | ✅ | ❌ |
| Push na `main` | ✅ | ✅ | ✅ |

CD job ima `if: github.event_name == 'push' && github.ref == 'refs/heads/main'`, tako da se deploy nikada ne dešava sa PR-a.

### GitHub Secrets

Sve osjetljive vrijednosti se čitaju iz **GitHub Secrets** (Settings → Secrets and variables → Actions). Postavite ih za environment **`production`** (Settings → Environments → New environment → `production`).

Potrebni secret-i:

| Secret | Šta je | Gdje se koristi |
| --- | --- | --- |
| `PRODUCTION_DB_CONNECTION_STRING` | npr. `Host=…;Port=5432;Database=postroute;Username=…;Password=…` | Backend CD: `dotnet ef database update` i deploy step (env `ConnectionStrings__DefaultConnection`) |
| `FRONTEND_DEPLOY_TOKEN` | Deploy token od izabranog static-host providera (Azure SWA, Netlify, Vercel…) | Frontend CD step |

Opcionalne **variables** (ne secret-i, mogu biti vidljive):

| Var | Default | Šta je |
| --- | --- | --- |
| `SEEDING_ENABLED` | `true` | Da li seedovati default korisnike pri startu (vidi niže) |
| `VITE_API_BASE_URL` | `http://localhost:5032` | Bazni URL backend-a koji se ugrađuje u frontend bundle |

### Konfiguracija u .NET-u

`appsettings.json` više **ne sadrži** connection string — to je sada čisto runtime konfiguracija koja dolazi iz okoline:

- **Lokalno**: `PROJEKAT/backend/src/PostRoute.Api/appsettings.Development.json` (u `.gitignore`-u, ne komituje se).
- **CI**: env var `ConnectionStrings__DefaultConnection` postavljen u workflow-u (lokalna Postgres service kontejner).
- **Produkcija**: env var iz GitHub Secret-a (`PRODUCTION_DB_CONNECTION_STRING`).

.NET konfig sistem mapira `__` u env var imenu na `:` u konfig ključu, pa `ConnectionStrings__DefaultConnection` automatski override-uje `ConnectionStrings:DefaultConnection`.

### Seeding (UserSeedService)

Seedovanje 3 default korisnika (`admin`, `postar`, `postar1`) se izvršava u [Program.cs](PROJEKAT/backend/src/PostRoute.Api/Program.cs) nakon migracija. Implementacija je **idempotentna** — preskače korisnike koji već postoje po email-u ili username-u.

Ponašanje je pod kontrolom config flag-a `Seeding:Enabled`:

- `null` (default) → seed-uje se **samo u Development** environment-u.
- `true` → uvijek seed-uje (override za prvi deploy u Production).
- `false` → nikad ne seed-uje.

**Preporuka (best practice)**: seed-ovanje treba da se izvršava i lokalno **i** prilikom deploya, ali pod flag-om koji se može isključiti:

- Lokalno: `Seeding:Enabled=null` ili izostavljeno → automatski uključeno jer je `Environment.IsDevelopment()`.
- Prvi deploy u Production: postavite GitHub variable `SEEDING_ENABLED=true` da inicijalni admin nalog bude dostupan za prijavu i konfiguraciju.
- Nakon prvog uspješnog deploy-a: postavite na `false` (ili obrišite) — idempotentnost garantuje da ponovljen seed ništa neće pokvariti, ali isključivanjem se eliminiše bilo kakav rizik da netko slučajno reset-uje default lozinke kroz kod.

> **Sigurnosna napomena**: trenutno su default lozinke (`Admin123!`, `Postar123!`) hardkodovane u [UserSeedService.cs](PROJEKAT/backend/src/PostRoute.BLL/Services/UserSeedService.cs) i `MustChangePassword=false`. Za pravu produkciju to treba prebaciti na lozinke iz Secret-a sa `MustChangePassword=true`, kako bi se prisilila izmjena pri prvom loginu. Trenutno je prihvatljivo za interni/edu deploy.

## Notes
- Ovaj README je pocetna verzija i bice prosiren kako implementacija bude rasla.
- `User` flow je edukativni primjer organizacije kontrolera, servisa i repository sloja.
