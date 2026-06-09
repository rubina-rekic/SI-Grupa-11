# Continuous Deployment 

## Pregled CD strategije

PostRoute koristi **hibridni CD model**:

- **Frontend** → GitHub Actions builda i deploya na **Netlify** pri svakom pushu na `main`
- **Backend** → GitHub Actions builda i testira, a **Render auto-deploy** okida deployment pri svakom pushu na `main`
- **Baza i migracije** → PostgreSQL na Renderu; EF Core migracije se primjenjuju **automatski pri svakom startu** aplikacije putem `MigrateAsync()` u `Program.cs`

---

## Gdje se nalaze pipeline fajlovi

```
.github/
└── workflows/
    ├── frontend-ci.yml   # Frontend build, lint, test i Netlify deploy
    └── backend-ci.yml    # Backend build i testovi
```

---

## Pipeline 1: Frontend (`frontend-ci.yml`)

### Kada se okida
- Push na `main` ili `develop` grana kada se promijene fajlovi u `PROJEKAT/frontend/**`
- Pull request prema `main` ili `develop`

### Koraci

| Korak | Opis |
|---|---|
| `actions/checkout@v4` | Klonira repozitorij |
| `actions/setup-node@v4` | Instalira Node.js 24 |
| `npm ci` | Instalira zavisnosti iz `package-lock.json` |
| `npm run lint` | ESLint provjera koda |
| `npm run build` | Vite production build — generira `dist/` folder |
| `upload-artifact` | Čuva `dist/` kao GitHub Actions artifact (7 dana) |
| `netlify-cli deploy --prod` | Deploya `dist/` na Netlify (samo na `main`) |

### Secrets i varijable (GitHub)

| Naziv | Tip | Opis |
|---|---|---|
| `NETLIFY_AUTH_TOKEN` | Secret | Netlify Personal Access Token |
| `NETLIFY_SITE_ID` | Secret | ID Netlify sajta |
| `VITE_API_BASE_URL` | Variable | https://si-grupa-11.onrender.com |

### Rezultat deploymenta
Aplikacija je dostupna na: **https://postrouteapp.netlify.com**

---

## Pipeline 2: Backend (`backend-ci.yml`)

### Kada se okida
- Push na `main` ili `develop` grana kada se promijene fajlovi u `PROJEKAT/backend/**`
- Pull request prema `main` ili `develop`

### Koraci

| Korak | Opis |
|---|---|
| `actions/checkout@v4` | Klonira repozitorij |
| `actions/setup-dotnet@v4` | Instalira .NET 9 SDK |
| `dotnet restore` | Restore NuGet paketa za `PostRoute.sln` |
| `dotnet build --configuration Release` | Release build |
| `dotnet test --configuration Release` | Pokretanje svih testova (BLL + DAL sloj) |

### Render auto-deploy
Nakon što GitHub Actions pipeline uspješno prođe na `main` grani, **Render automatski detektuje novi commit** i pokreće deployment backend servisa. Render prati `main` granu i koristi sljedeće komande:

- **Build Command:** `dotnet publish PROJEKAT/backend/src/PostRoute.Api/PostRoute.Api.csproj -c Release -o out`
- **Start Command:** `dotnet out/PostRoute.Api.dll`
- **Runtime:** .NET 9, Region: Frankfurt (EU)

### Environment varijable na Renderu

| Naziv | Opis |
|---|---|
| `DATABASE_URL` | Neon connection string |
| `ASPNETCORE_ENVIRONMENT` | Postaviti na `Production` |
| `Cors__AllowedOrigins__0` | URL frontenda, npr. `https://postrouteapp.netlify.com` |
| `Seeding__Enabled` | `true` za inicijalno seedovanje korisnika |

---

## Migracije baze podataka

Migracije se **ne pokreću ručno** — automatski se primjenjuju pri svakom startu backend aplikacije:

```csharp
// Program.cs
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync(CancellationToken.None);
}
```

Ovo znači da svaki novi deployment automatski primjenjuje sve pending migracije prije nego što aplikacija počne primati zahtjeve.

---

## Kako provjeriti da je deployment uspješan

### Frontend
1. Otvoriti https://postrouteapp.netlify.com
2. Provjeriti da se login stranica učitava

### Backend
1. Poslati `GET` zahtjev na `https://si-grupa-11.onrender.com`
2. Očekivani odgovor: `HTTP 200 OK`

### GitHub Actions
1. Otvoriti repozitorij na GitHubu
2. Kliknuti na tab **Actions**
3. Provjeriti da su posljednji workflows zeleni (✅)

---

## Preduvjeti za pokretanje CD-a

### Za frontend deployment
- GitHub repository secrets: `NETLIFY_AUTH_TOKEN`, `NETLIFY_SITE_ID`
- GitHub repository variable: `VITE_API_BASE_URL`
- Netlify sajt kreiran i povezan

### Za backend deployment
- Render servis kreiran i povezan sa GitHub repozitorijem
- Render servis konfiguriran da prati `main` granu
- Environment varijable postavljene u Render dashboardu
- PostgreSQL baza kreirana na Renderu

---

## Tok kompletnog deploymenta

```
Developer push na main
        │
        ▼
GitHub Actions okida oba workflowa paralelno
        │
        ├──► frontend-ci.yml
        │         │
        │    lint + build
        │         │
        │    deploy na Netlify
        │         │
        │    ✅ https://postrouteapp.netlify.com
        │
        └──► backend-ci.yml
                  │
             restore + build + test
                  │
             ✅ GitHub Actions prolazi
                  │
             Render detektuje push na main
                  │
             Render builda i deploya
                  │
             MigrateAsync() pri startu
                  │
             ✅ hhttps://si-grupa-11.onrender.com/health
```

---

## Poznata ograničenja

- **Render free tier** — backend servis se gasi nakon 15 minuta neaktivnosti; prvi zahtjev nakon toga može trajati 30–60 sekundi (cold start)
- **Seeding u produkciji** — `Seeding__Enabled` mora biti postavljen na `true` samo pri prvom deploymentu, nakon toga ga treba isključiti ili ostaviti default (`false` u produkciji)

---

## Najčešći problemi

### Frontend se builda ali API pozivi ne rade
- Provjeriti da je `VITE_API_BASE_URL` GitHub variable ispravno postavljena na Render URL
- Provjeriti da Render servis nije u cold start stanju

### Backend deployment ne okida na Renderu
- Provjeriti u Render dashboardu da je auto-deploy uključen za `main` granu
- Provjeriti da GitHub Actions `backend-ci.yml` prolazi bez grešaka

### Migracije ne prolaze pri startu
- Provjeriti da je `DATABASE_URL` environment varijabla ispravno postavljena na Renderu
- Provjeriti Render deployment logove za detalje greške
