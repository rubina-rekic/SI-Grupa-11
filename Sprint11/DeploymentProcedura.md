# 2. Deployment Procedura — PostRoute

### Sadržaj
1. [Naziv aplikacije i opis arhitekture](#1-naziv-aplikacije-i-opis-arhitekture)
2. [Tehnologije i verzije](#2-tehnologije-i-verzije)
3. [Potrebni alati](#3-potrebni-alati)
4. [Environment varijable](#4-environment-varijable)
5. [Lokalno pokretanje backenda](#5-lokalno-pokretanje-backenda)
6. [Lokalno pokretanje frontenda](#6-lokalno-pokretanje-frontenda)
7. [Baza podataka](#7-baza-podataka)
8. [Migracije i seed podaci](#8-migracije-i-seed-podaci)
9. [Pokretanje testova](#9-pokretanje-testova)
10. [Produkcijski deployment (Netlify & Render)](#10-produkcijski-deployment-netlify--render)
11. [Link na deployment](#11-link-na-deployment)
12. [Poznata ograničenja deploymenta](#12-poznata-ograničenja-deploymenta)
13. [Najčešći problemi i rješenja](#13-najčešći-problemi-i-rješenja)

---

### 1. Naziv aplikacije i opis arhitekture
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

| Komponenta                | Platforma          | URL                                  |
| :------------------------ | :----------------- | :----------------------------------- |
| **Frontend (React/Vite)** | Netlify            | <https://postrouteapp.netlify.app>   |
| **Backend (.NET 9 API)**  | Render Web Service | <https://postroute-api.onrender.com> |
| **Baza podataka**         | Render PostgreSQL  | *Internal Connection*                |

Frontend šalje API zahtjeve na backend (VITE_API_BASE_URL). Backend koristi
Layered arhitekturu (API, BLL, DAL) i spaja se na PostgreSQL bazu. Mape i rute
se iscrtavaju pomoću Leaflet i OSRM servisa.

2. Tehnologije i verzije

| Sloj                   | Tehnologija             | Verzija             |
| :--------------------- | :---------------------- | :------------------ |
| **Frontend framework** | React                   | 19.0.0              |
| **Frontend bundler**   | Vite                    | 6.x                 |
| **Frontend routing**   | React Router            | 7.x                 |
| **Backend runtime**    | .NET                    | 9.0                 |
| **Backend framework**  | ASP.NET Core Web API    | 9.0                 |
| **ORM**                | Entity Framework Core   | 9.0                 |
| **Baza podataka**      | PostgreSQL              | 16                  |
| **Autentifikacija**    | Session-based / Cookies | Native .NET Session |
| **Mape i rutiranje**   | Leaflet / OSRM          | Latest              |
| **Testiranje**         | xUnit / Vitest          | Latest              |

3. Potrebni alati

Za lokalno pokretanje sistema potrebno je imati instalirano:

  - .NET 9.0 SDK — https://dotnet.microsoft.com/download
  - Node.js v20 ili noviji — https://nodejs.org
  - Git — https://git-scm.com
  - PostgreSQL 16 ili Docker Desktop (za lokalnu bazu)

4. Environment varijable

Backend (PROJEKAT/backend/src/PostRoute.Api/appsettings.Development.json)

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=postroute_db;Username=postgres;Password=admin"
  },
  "Jwt": {
    "Secret": "SuperTajniKljucMin32Karaktera123!"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  }
}

Frontend (PROJEKAT/frontend/.env)

VITE_API_BASE_URL=http://localhost:5000

U produkciji: VITE_API_BASE_URL mora pokazivati na URL Render backenda.

5. Lokalno pokretanje backenda

# 1. Klonirati repozitorij
git clone https://github.com/vaš-repo/PostRoute.git
cd PostRoute/PROJEKAT/backend

# 2. Restore paketa i build
dotnet restore
dotnet build

# 3. Pokretanje servera
dotnet run --project src/PostRoute.Api/PostRoute.Api.csproj

# Server sluša na: http://localhost:5000

6. Lokalno pokretanje frontenda

# 1. Pozicionirati se u direktorij
cd PROJEKAT/frontend

# 2. Instalirati zavisnosti
npm install

# 3. Pokrenuti dev server
npm run dev

# Aplikacija je dostupna na: http://localhost:5173

7. Baza podataka

Lokalna baza se pokreće putem Docker Compose-a na portu 5433:

docker-compose -f docker-compose.dev.yml up -d

Produkcijska baza na Renderu je Managed PostgreSQL. Konekcija se vrši putem
DATABASE_URL varijable koju Render automatski mapira.

8. Migracije i seed podaci

Migracije: Šema baze se ažurira EF Core migracijama. Komanda za ručno
pokretanje:

dotnet ef database update --project PostRoute.DAL --startup-project PostRoute.Api

Seed podaci: Backend automatski vrši migraciju i ubacuje demo podatke
(administrator, dispečer, poštari i početni sandučići) pri svakom startu
aplikacije zahvaljujući await dbContext.Database.MigrateAsync() pozivu u
Program.cs.

9. Pokretanje testova

Backend (xUnit):

cd PROJEKAT/backend
dotnet test PostRoute.sln

Frontend (Vitest):

cd PROJEKAT/frontend
npm test -- --run

Rezultati (juni 2026): 283 automatizovana testa (149 BLL, 39 DAL, 52 API, 43
Frontend) — svi PASS.

10. Produkcijski deployment (Netlify & Render)

Backend (Render Web Service):

  - Root Directory: PROJEKAT/backend
  - Runtime: Docker (Render koristi Dockerfile iz roota)
  - Environment Variables: Dodati ConnectionStrings__DefaultConnection,
    Jwt__Secret, Cors__AllowedOrigins__0.

Frontend (Netlify Static Site):

  - Base Directory: PROJEKAT/frontend
  - Build Command: npm run build
  - Publish Directory: dist
  - Environment Variable: VITE_API_BASE_URL postavljen na URL produkcijskog
    API-ja.

11. Link na deployment

  - Frontend (Live): https://postrouteapp.netlify.app
  - Backend (Health): https://postroute-api.onrender.com/health

12. Poznata ograničenja deploymenta

  - Cold Start: Zbog Renderovog besplatnog tier-a, backend se gasi nakon
    neaktivnosti. Prvi zahtjev može trajati do 60 sekundi.
  - OSRM Limitacije: Prikaz putanja na mapi zavisi od dostupnosti javnog OSRM
    endpointa.
  - CORS: Ako se URL frontenda promijeni, mora se ažurirati lista dozvoljenih
    origin-a na backendu.

13. Najčešći problemi i rješenja

  - Problem: API vraća 404 grešku na svim rutama.
      - Rješenje: Provjeriti da li VITE_API_BASE_URL u env varijablama završava
        sa /api (ako je tako definisano u routeru).
  - Problem: Sivi ekrani umjesto mapa.
      - Rješenje: Provjeriti internet konekciju. Leaflet mape zahtijevaju
        pristup OpenStreetMap serverima.
  - Problem: "SyntaxError: Unexpected token '<'".
      - Rješenje: Frontend pokušava pročitati HTML (obično 404 stranicu) kao
        JSON. Provjeriti da li API URL tačno odgovara onome na Renderu.

Dokument kreiran: juni 2026 |Tim: SI-Grupa 11

