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

## Notes
- Ovaj README je pocetna verzija i bice prosiren kako implementacija bude rasla.
- Nema aktivne database integracije u trenutnom skeletonu.
- `User` flow je edukativni primjer organizacije kontrolera, servisa i repository sloja.
