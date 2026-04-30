# Deployment

PostRoute koristi tri servisa u produkciji:

| Sloj | Servis | Šta hostuje |
|---|---|---|
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

#### Default korisnici (seed)

| Email | Lozinka | Uloga |
|---|---|---|
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

```bash
git push origin main
```

Render i Netlify oba auto-detektuju push i deploy-uju paralelno.

## Troubleshooting

| Simptom | Najvjerovatniji uzrok |
|---|---|
| `/health` vraća 502 | Backend pao na startup-u — provjeri Render logove (najčešće: connection string format ili migracija) |
| Frontend prikazuje stranicu ali API pozivi padaju s CORS greškom | `Cors__AllowedOrigins__0` ne odgovara tačnom Netlify URL-u (mora biti `https://`, bez slash-a na kraju) |
| Login uspije (200) ali sljedeći zahtjev je 401 | Browser ne šalje session cookie — provjeri da frontend radi preko `https://` (ne `http://`) i da axios koristi `withCredentials: true` |
| Render se "spava" 30s | Free plan suspenduje servis nakon 15 min neaktivnosti — očekivano |
| Migracija pukla | Lokalno: `dotnet ef database update` protiv Neon-a, pa retry deploy |
