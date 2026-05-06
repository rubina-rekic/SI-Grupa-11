# PROJEKAT - PostRoute

## Struktura

- `backend/` — ASP.NET Core 9 (API / BLL / DAL)
- `frontend/` — React 19 + TypeScript + Vite
- `docker-compose.dev.yml` — PostgreSQL 16 za lokalni razvoj

---

## Lokalni setup (prvi put)

### 1. Baza podataka

```bash
# Iz PROJEKAT/ foldera
docker compose -f docker-compose.dev.yml up -d
```

Pokreće PostgreSQL 16 na portu **5433**. Podaci se čuvaju u Docker volumenu između restartova.

### 2. Backend migracija

```bash
cd backend
dotnet ef database update --project src/PostRoute.DAL --startup-project src/PostRoute.Api
```

> Ako `dotnet ef` nije instaliran: `dotnet tool install --global dotnet-ef`

### 3. Pokretanje backenda

```bash
cd backend
dotnet run --project src/PostRoute.Api
```

Swagger dostupan na: `http://localhost:5000/swagger`

### 4. Pokretanje frontenda

```bash
cd frontend
npm ci
npm run dev
```

Aplikacija dostupna na: `http://localhost:5173`

---

## Svakodnevni rad

```bash
# Pokrenuti bazu (ako nije već gore)
docker compose -f docker-compose.dev.yml up -d

# Zaustaviti bazu
docker compose -f docker-compose.dev.yml down
```

---

## Napomena za konfiguraciju

`appsettings.Development.json` se **ne commituje** (u .gitignore).  
Kreirati ga ručno u `backend/src/PostRoute.Api/` sa sljedećim sadržajem:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=postroute;Username=postgres;Password=postgres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
