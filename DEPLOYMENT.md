# Deployment

PostRoute koristi tri servisa u produkciji:

| Sloj | Servis | Šta hostuje |
|---|---|---|
<<<<<<< HEAD
| Baza | **Neon** | PostgreSQL 16 |
| Backend | **Render** | .NET 9 API u Docker kontejneru |
| Frontend | **Netlify** | React build (statički fajlovi + SPA fallback) |

Push u `main` → Render auto-deploy backend (preko `render.yaml`), GitHub Actions deploy frontend na Netlify (`frontend-ci.yml`).

## Prvi setup

### 1. Neon (production baza)

1. Kreiraj projekat na [neon.tech](https://neon.tech). Free plan dovoljan.
2. Iz **Connection string** taba kopiraj string oblika `postgresql://user:pass@host/db?sslmode=require`.
3. Pretvori u .NET format:
   `Host=<host>;Database=<db>;Username=<user>;Password=<pass>;SSL Mode=Require;Trust Server Certificate=true`

### 2. Render (backend)

1. Login na [render.com](https://render.com) sa GitHub OAuth.
2. **New → Web Service** → izaberi repo `SI-Grupa-11`.
3. Render će auto-detektovati `render.yaml` (regija frankfurt, Dockerfile path, branch=main).
4. Klikni **Apply**. Build prvi put traje 5-10 min na free planu.
5. Otvori servis → **Environment** tab i dodaj:
   - `ConnectionStrings__DefaultConnection` = Neon connection string iz koraka 1
   - `Cors__AllowedOrigins__0` = Netlify URL (npr. `https://postroute.netlify.app`) — tek nakon koraka 3
   - `Seeding__Enabled` = `true` (samo na **prvi deploy**)
6. **Manual Deploy** da primijeni varijable.
7. Provjeri `https://<service>.onrender.com/health` → mora vratiti `{"status":"healthy"}`.
8. **Nakon prvog uspješnog deploy-a** vrati `Seeding__Enabled` na `false`.
=======
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
>>>>>>> origin/develop

#### Default korisnici (seed)

| Email | Lozinka | Uloga |
|---|---|---|
<<<<<<< HEAD
| `admin@mail.com` | `Admin123!` | Administrator |
| `postar@mail.com` | `Postar123!` | PostalWorker |
| `postar1@mail.com` | `Postar123!` | PostalWorker |

### 3. Netlify (frontend)

1. Login na [netlify.com](https://netlify.com) sa GitHub OAuth.
2. **Add new site → Deploy manually** → upload prazan `index.html` (samo da kreira site i URL).
3. Kopiraj **Site ID** iz **Site configuration → Site details**.
4. **User settings → Applications → Personal access tokens** → kreiraj token.
5. **Site configuration → Build & deploy → Continuous deployment** → **isključi** automatski git build (deploy ide kroz GitHub Actions).
6. Vrati se u Render dashboard i postavi `Cors__AllowedOrigins__0` na Netlify URL.

### 4. GitHub Secrets i Variables

Repo Settings → Secrets and variables → Actions:

**Secrets:**
- `NETLIFY_AUTH_TOKEN` — token iz koraka 3.4
- `NETLIFY_SITE_ID` — site ID iz koraka 3.3

**Variables:**
- `VITE_API_BASE_URL` — Render backend URL, npr. `https://postroute-backend.onrender.com`

## Trigger deploy
=======
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
>>>>>>> origin/develop

```bash
git push origin main
```

<<<<<<< HEAD
Render i Netlify oba auto-detektuju push i deploy-uju paralelno.
=======
- Render: čita `render.yaml`, gradi backend Docker image, restartuje servis
- GitHub Actions: builda frontend (`npm run build`), deploy-uje `dist/` na Netlify

## Šta nije automatizovano

- **Migracije** — backend ih primjenjuje **na startup-u** (vidi `Program.cs` `await dbContext.Database.MigrateAsync()`). Ako neka migracija pukne, backend pada — provjeri Render logove.
- **Seedanje** — kontrolisano `Seeding__Enabled` env varijablom na Render-u. Uključi na prvi deploy, ugasi nakon.
- **Rollback** — Render dashboard ima **Manual Deploy → Deploy specific commit**.
>>>>>>> origin/develop

## Troubleshooting

| Simptom | Najvjerovatniji uzrok |
|---|---|
<<<<<<< HEAD
| `/health` vraća 502 | Backend pao na startup-u — provjeri Render logove (najčešće: connection string format ili migracija) |
| Frontend prikazuje stranicu ali API pozivi padaju s CORS greškom | `Cors__AllowedOrigins__0` ne odgovara tačnom Netlify URL-u (mora biti `https://`, bez slash-a na kraju) |
| Login uspije (200) ali sljedeći zahtjev je 401 | Browser ne šalje session cookie — provjeri da frontend radi preko `https://` (ne `http://`) i da axios koristi `withCredentials: true` |
| Render se "spava" 30s | Free plan suspenduje servis nakon 15 min neaktivnosti — očekivano |
| Migracija pukla | Lokalno: `dotnet ef database update` protiv Neon-a, pa retry deploy |
=======
| `https://<backend>.onrender.com/health` vraća 502 | Backend pao na startup-u (provjeri Render logove — najčešće: connection string format ili migracija) |
| Frontend prikazuje stranicu ali API pozivi padaju s CORS greškom | `Cors__AllowedOrigins__0` na Render-u nije postavljen ili ne odgovara tačnom Netlify URL-u (mora biti `https://`, bez slash-a na kraju) |
| Login uspije (200) ali sljedeći zahtjev je 401 | Browser ne šalje session cookie — provjeri da li frontend pokazuje na `https://` (ne `http://`) i da axios ima `withCredentials: true` |
| Render backend se "spava" pa odgovara nakon ~30s | Free plan suspenduje servis nakon 15 min neaktivnosti — to je očekivano. Upgrade na paid ili koristi vanjski uptime monitor |
| Migracija pukla na deploy-u | `dotnet ef database update` lokalno protiv Neon-a, pa retry deploy |
>>>>>>> origin/develop
