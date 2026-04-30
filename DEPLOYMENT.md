# Deployment

PostRoute koristi tri servisa u produkciji:

| Sloj | Servis | Šta hostuje |
|---|---|---|
| Baza | **Neon** | PostgreSQL 16, regija po izboru |
| Backend | **Render** | .NET 9 API u Docker kontejneru |
| Frontend | **Netlify** | React build (statički fajlovi + SPA fallback) |

Push u `main` → Render automatski deploy-uje backend (`render.yaml`), GitHub Actions deploy-uje frontend na Netlify (`frontend-ci.yml`).

## Prvi setup

### 1. Neon

1. Kreiraj projekat na [neon.tech](https://neon.tech) (free plan dovoljan).
2. Iz **Connection string** taba kopiraj string oblika `postgresql://user:pass@host/db?sslmode=require`.
3. Pretvori ga u .NET format: `Host=<host>;Database=<db>;Username=<user>;Password=<pass>;SSL Mode=Require;Trust Server Certificate=true`.

### 2. Render (backend)

1. Na [render.com](https://render.com) klikni **New → Web Service**, povezi GitHub repo.
2. Render će auto-detektovati `render.yaml` u root-u i preložiti konfiguraciju (regija, Dockerfile path, branch=main).
3. Kreiraj servis. Build može trajati 5-10 min na free planu.
4. Otvori servis → **Environment** tab i dodaj:
   - `ConnectionStrings__DefaultConnection` = Neon connection string iz koraka 1
   - `Cors__AllowedOrigins__0` = Netlify URL (vidi korak 3, npr. `https://postroute.netlify.app`)
   - `Seeding__Enabled` = `true` (samo na **prvi deploy** da se kreiraju default korisnici, vidi tabelu ispod)
5. Trigger **Manual Deploy** da varijable preuzmu.
6. Provjeri da backend radi: `https://<service>.onrender.com/health` mora vratiti `{"status":"healthy"}`.
7. **Nakon prvog deploy-a** vrati `Seeding__Enabled` na `false` da se seed ne pokreće svaki put.

#### Default korisnici (seed)

| Email | Lozinka | Uloga |
|---|---|---|
| `admin@posta.ba` | `Admin123!` | Administrator |
| `postar1@posta.ba` | `Postar123!` | PostalWorker |
| `postar2@posta.ba` | `Postar123!` | PostalWorker |

### 3. Netlify (frontend)

1. Na [netlify.com](https://netlify.com) klikni **Add new site → Deploy manually** (ne treba git connection — deploy ide kroz GitHub Actions).
2. Privremeno upload-uj prazan `index.html` da Netlify kreira site i dodijeli URL.
3. Kopiraj **Site ID** iz **Site configuration → Site details**.
4. Generiši **Personal Access Token** u **User settings → Applications → Personal access tokens**.
5. U Render dashboard-u dodaj `Cors__AllowedOrigins__0` = Netlify URL i restartuj servis.

### 4. GitHub Secrets i Variables

**Settings → Secrets and variables → Actions → Secrets:**
- `NETLIFY_AUTH_TOKEN` — token iz Netlify-a (korak 3.4)
- `NETLIFY_SITE_ID` — site ID iz Netlify-a (korak 3.3)

**Settings → Secrets and variables → Actions → Variables:**
- `VITE_API_BASE_URL` — URL Render backend-a, npr. `https://postroute-backend.onrender.com`

**Settings → Environments → production:** — može se dodati i kao environment-scoped da PR-ovi iz fork-ova ne mogu deploy-ovati. Workflow `frontend-ci.yml` koristi `environment: name: production` u deploy job-u.

## Trigger deploy-a

```bash
git push origin main
```

- Render: čita `render.yaml`, gradi backend Docker image, restartuje servis
- GitHub Actions: builda frontend (`npm run build`), deploy-uje `dist/` na Netlify

## Šta nije automatizovano

- **Migracije** — backend ih primjenjuje **na startup-u** (vidi `Program.cs` `await dbContext.Database.MigrateAsync()`). Ako neka migracija pukne, backend pada — provjeri Render logove.
- **Seedanje** — kontrolisano `Seeding__Enabled` env varijablom na Render-u. Uključi na prvi deploy, ugasi nakon.
- **Rollback** — Render dashboard ima **Manual Deploy → Deploy specific commit**.

## Troubleshooting

| Simptom | Najvjerovatniji uzrok |
|---|---|
| `https://<backend>.onrender.com/health` vraća 502 | Backend pao na startup-u (provjeri Render logove — najčešće: connection string format ili migracija) |
| Frontend prikazuje stranicu ali API pozivi padaju s CORS greškom | `Cors__AllowedOrigins__0` na Render-u nije postavljen ili ne odgovara tačnom Netlify URL-u (mora biti `https://`, bez slash-a na kraju) |
| Login uspije (200) ali sljedeći zahtjev je 401 | Browser ne šalje session cookie — provjeri da li frontend pokazuje na `https://` (ne `http://`) i da axios ima `withCredentials: true` |
| Render backend se "spava" pa odgovara nakon ~30s | Free plan suspenduje servis nakon 15 min neaktivnosti — to je očekivano. Upgrade na paid ili koristi vanjski uptime monitor |
| Migracija pukla na deploy-u | `dotnet ef database update` lokalno protiv Neon-a, pa retry deploy |
