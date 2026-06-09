# 2. Deployment Procedura — PostRoute

---

## 1. Naziv aplikacije i opis arhitekture
**Naziv:** PostRoute — Sistem za optimizaciju ruta punjenja i pražnjenja poštanskih sandučića

**Arhitektura:** Sistem je organizovan kao troslojna aplikacija deployovana na odvojenim servisima:

```text
Browser
  └── React SPA (Netlify)
         │ HTTPS /api/*
         ▼
    ASP.NET Core Web API (Render Docker)
         │ SSL / TCP
         ▼
    PostgreSQL (Render Managed DB)
```

| Komponenta                | Platforma          | URL                                  |
| :------------------------ | :----------------- | :----------------------------------- |
| **Frontend (React/Vite)** | Netlify            | <https://postrouteapp.netlify.app>   |
| **Backend (.NET 9 API)**  | Render Web Service | <https://postroute-api.onrender.com> |
| **Baza podataka**         | Render PostgreSQL  | *Internal Connection*                |

---

### 2. Tehnologije i verzije

| Sloj                      | Tehnologija             | Verzija     |
| :------------------------ | :---------------------- | :---------- |
| **Frontend framework**    | React                   | 19.2.5      |
| **Frontend bundler**      | Vite                    | 8.0.10      |
| **Frontend routing**      | React Router            | 7.14.2      |
| **Backend runtime**       | .NET                    | 9.0         |
| **Backend framework**     | ASP.NET Core            | 9.0.1       |
| **ORM**                   | Entity Framework Core   | 9.0.4       |
| **Baza podataka**         | PostgreSQL              | 16          |
| **Autentifikacija**       | Session-based / Cookies | Native .NET |
| **Mape i rutiranje**      | Leaflet                 | 1.9.4       |
| **Testiranje (Backend)**  | xUnit                   | 2.9.2       |
| **Testiranje (Frontend)** | Vitest                  | 4.1.5       |
| **Package manager**       | npm                     | 10+         |


---

### 3. Potrebni alati

Za lokalno pokretanje sistema potrebno je imati instalirano:

  - .NET 9.0 SDK — [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
  - Node.js v20 ili noviji — [nodejs.org](https://nodejs.org)
  - Git — git-scm.com
  - Docker Desktop (za lokalnu bazu) — https://www.docker.com/products/docker-desktop

Provjera instaliranih verzija:
```text
dotnet --version
node --version
npm --version
git --version
docker --version
```
---

### 4. Environment varijable

## 4. Environment varijable

### Backend (**PROJEKAT/backend/src/PostRoute.Api/appsettings.Development.json**)
Lokalno se konfiguracija čuva u JSON formatu. Za produkciju (Render), ovi ključevi se unose direktno kao varijable okruženja.

```bash
# Server
PORT=5000

# Baza podataka
ConnectionStrings__DefaultConnection=Host=localhost;Port=5433;Database=postroute_db;Username=postgres;Password=admin

# Autentifikacija
Jwt__Secret=SuperTajniKljucMin32Karaktera123!

# CORS — URL frontenda (lokalno ili produkcija)
Cors__AllowedOrigins__0=http://localhost:5173
```
Frontend (PROJEKAT/frontend/.env)
```bash
VITE_API_BASE_URL=http://localhost:5000
```
---

### 5. Lokalno pokretanje backenda
```bash
# 1. Klonirati repozitorij

git clone https://github.com/vaš-repo/PostRoute.git
cd PostRoute/PROJEKAT/backend

# 2. Restore paketa i build

dotnet restore
dotnet build

# 3. Pokretanje servera

dotnet run --project src/PostRoute.Api/PostRoute.Api.csproj

Server sluša na: http://localhost:5000
```

---

### 6. Lokalno pokretanje frontenda
```bash
# 1. Pozicionirati se u direktorij

cd PROJEKAT/frontend

# 2. Instalirati zavisnosti

npm install

# 3. Pokrenuti dev server

npm run dev

Aplikacija je dostupna na: http://localhost:5173
```
---
### 7. Baza podataka, migracije i seed podaci

Lokalna baza se pokreće putem Docker Compose-a na portu 5433:
```text
docker-compose -f docker-compose.dev.yml up -d
```
Primjena migracija:
```text
dotnet ef database update --project PostRoute.DAL --startup-project PostRoute.Api
```
Sistem automatski ubacuje početne podatke pri prvom pokretanju ako je baza prazna (Inicijalni administrator, dispečer, poštari i demo sandučići). Inicijalni korisnici su:
 - Admin: admin@mail.com / Admin123!
 - Dispečer: dispatcher@mail.com / Dispatcher123!
 - Poštar: postar@mail.com / Postar123!
---

### 8. Pokretanje testova

Backend (xUnit):
```text
cd PROJEKAT/backend
dotnet test PostRoute.sln
```
Frontend (Vitest):
```text
cd PROJEKAT/frontend
npm test -- --run
```
---

### 9. Produkcijski deployment (Netlify & Render)

Backend (Render Web Service):

  - Runtime: Docker
  - Environment Variables: Dodati ConnectionStrings__DefaultConnection,
    Jwt__Secret, Cors__AllowedOrigins__0.

Frontend (Netlify Static Site):

  - Build Command: npm run build
  - Publish Directory: dist
  - Variable: VITE_API_BASE_URL postavljen na URL Render API-ja.

---
### 10. Poznata ograničenja deploymenta
 - Cold Start (Render Free Tier): Backend servis se gasi nakon 15 minuta neaktivnosti. Prvi zahtjev nakon toga može trajati 30-60 sekundi dok se kontejner ponovo ne podigne.
 - Zavisnost o vanjskim servisima: Prikaz mapa i iscrtavanje ruta direktno zavise od dostupnosti javnih servisa OpenStreetMap i OSRM.
 - CORS restrikcije: Frontend mora biti na HTTPS protokolu u produkciji kako bi browser dozvolio slanje autentifikacijskih cookija prema Render API-ju.
---
### 11. Najčešći problemi i rješenja

  - Problem: API vraća 404 grešku.
      - Rješenje: Provjeriti da li VITE_API_BASE_URL u env varijablama završava
        ispravno.
---
  - Problem: Sivi ekrani umjesto mapa.
      - Rješenje: Provjeriti internet konekciju za Leaflet servere.
---
  - Problem: "SyntaxError: Unexpected token '<'".
      - Rješenje: Provjeriti da li API URL tačno odgovara onome na Renderu.

---
Dokument kreiran: juni 2026 | Tim: SI-Grupa 11
