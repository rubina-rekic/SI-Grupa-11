# 2. Deployment Procedura 

---

## 1. Naziv aplikacije i opis arhitekture

**Naziv aplikacije:** PostRoute — Sistem za optimizaciju ruta punjenja i pražnjenja poštanskih sandučića

**Arhitektura:** Sistem je organizovan kao troslojna web aplikacija (SPA) bazirana na labavo spregnutim servisima koji su hostovani na odvojenim namjenskim cloud platformama:

```text
Browser
   └── React SPA (Netlify)
         │ HTTPS /api/* (Cookie-based Session)
         ▼
    ASP.NET Core Web API (Render Docker Kontejner)
         │ SSL / TCP (Cloud Connection String)
         ▼
    PostgreSQL (Neon Serverless Cloud DB)
```

**Mehanizam uvezivanja i komunikacije servisa**

- **Frontend -> Backend:** Klijentska React aplikacija izvršava se unutar browsera i komunicira sa REST API-jem slanjem asinhronih HTTP zahtjeva na apsolutnu adresu definisanu kroz `VITE_API_BASE_URL`.
- **Autentifikacija i CORS:** Koristi se nativna .NET autentifikacija zasnovana na kolačićima sesije (Cookie-based session). Kako bi cross-site prenos kolačića bio omogućen između različitih domena (`netlify.app` i `onrender.com`), u produkcijskom okruženju (`ASPNETCORE_ENVIRONMENT=Production`) unutar `Program.cs` se forsira `SameSite=None` i `Secure` politika.
- **Proksiranje i HTTPS:** Unutar `Program.cs` konfigurisan je `ForwardedHeadersOptions` middleware. On omogućava sistemu da bezbjedno prepozna i obradi `X-Forwarded-Proto` zaglavlja proslijeđena sa Renderovog reverse proxy servera, osiguravajući ispravan rad kolačića i HTTPS protokola.
- **Backend -> Baza:** API se povezuje direktno na Neon Serverless PostgreSQL bazu podataka u cloudu koristeći dodijeljeni string konekcije koji se u produkciji povlači iz sigurnosnih varijabli okruženja.

| Komponenta | Platforma | URL / Tip konekcije |
|---|---|---|
| Frontend (React/Vite) | Netlify | https://postrouteapp.netlify.app |
| Backend (.NET 9 API) | Render (Docker) | https://si-grupa-11.onrender.com |
| Baza podataka | Neon Serverless | Eksterni Connection String (PostgreSQL 16) |

---

## 2. Tehnologije i verzije

| Sloj / Komponenta | Tehnologija | Verzija |
|---|---|---|
| Frontend framework | React | 19.2.5 |
| Frontend bundler | Vite | 8.0.10 |
| Frontend routing | React Router | 7.14.2 |
| Backend runtime | .NET (Docker Base) | 9.0.x |
| Backend framework | ASP.NET Core | 9.0.1 |
| ORM | Entity Framework Core | 9.0.4 |
| Baza podataka | PostgreSQL | 16.x |
| Autentifikacija | Session-based / Cookies | Native .NET |
| Mape i rutiranje | Leaflet | 1.9.4 |
| Testiranje (Backend) | xUnit | 2.9.2 |
| Testiranje (Frontend) | Vitest | 4.1.5 |
| Package manager | npm | 10+ |

---

## 3. Potrebni alati

Za lokalno pokretanje sistema potrebno je imati instalirano:

- **.NET 9.0 SDK** — [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
- **Node.js v20 ili noviji** — [nodejs.org](https://nodejs.org)
- **Git CLI** — [git-scm.com](https://git-scm.com)
- **Docker Desktop** (za lokalnu bazu i testiranje kontejnera) — [docker.com/products/docker-desktop](https://docker.com/products/docker-desktop)
- **EF Core CLI alati** (`dotnet-ef`) — Instalira se globalno za upravljanje migracijama.

Provjera instaliranih verzija alata iz terminala:

```bash
dotnet --version
node --version
npm --version
git --version
docker --version
dotnet ef --version
```

---

## 4. Environment varijable

Sve varijable okruženja su definirane direktno u GitHub repozitoriju i automatski se koriste kroz CI/CD pipeline (GitHub Actions). Nisu pohranjene ni u jednom lokalnom fajlu niti se commituju u repozitorij.

Varijable se nalaze na lokaciji: **GitHub repozitorij → Settings → Secrets and variables → Actions**, gdje su podijeljene u tri kategorije:

### Secrets (`Settings → Secrets and variables → Actions → Secrets`)

Osjetljivi podaci koji se enkriptuju i nikad ne prikazuju u logovima:

| Naziv | Opis |
|---|---|
| `ConnectionStrings__DefaultConnection` | String konekcije za Neon Serverless PostgreSQL bazu (sa `SslMode=Require`) |
| `Jwt__Secret` | Sigurnosni ključ za JWT token operacije |

### Variables (`Settings → Secrets and variables → Actions → Variables`)

Javne konfiguracijske vrijednosti koje nisu osjetljive:

| Naziv | Vrijednost | Opis |
|---|---|---|
| `VITE_API_BASE_URL` | `https://si-grupa-11.onrender.com` | Apsolutna adresa backend API-ja, ugrađuje se u frontend build |
| `Cors__AllowedOrigins__0` | `https://postrouteapp.netlify.app` | Dozvoljeni CORS origin za backend |
| `Seeding__Enabled` | `true` / `false` | Kontrola inicijalnog punjenja baze podataka |

### Environments (`Settings → Environments`)

GitHub okruženja omogućavaju grupiranje varijabli i tajni po deployment targetu (npr. `production`). CI/CD workflow referencira odgovarajuće okruženje pri deploymentu na Render i Netlify, čime se osigurava da se ispravne varijable koriste za svaki target.

---

## 5. Lokalno pokretanje backend-a

Pozicionirajte se u direktorij API projekta i pokrenite server:

```bash
cd PROJEKAT/backend/src/PostRoute.Api
dotnet run
```

Lokalni API server se podiže i sluša na adresi: `http://localhost:5032`

---

## 6. Lokalno pokretanje frontend-a

Otvorite novi prozor terminala i izvršite sljedeće komande za podizanje klijentskog Vite servera:

```bash
cd PROJEKAT/frontend
npm install
npm run dev
```

Klijentska aplikacija je uspješno pokrenuta i dostupna na adresi: `http://localhost:5173`

---

## 7. Baza podataka, migracije i seed podaci

### 7.1 Lokalna baza podataka

Lokalna PostgreSQL baza podataka se pokreće u pozadinskom modu u Docker okruženju na portu `5433` pomoću predefinisane konfiguracione datoteke:

```bash
cd PROJEKAT
docker-compose -f docker-compose.dev.yml up -d
```

### 7.2 Primjena Entity Framework Core migracija

Za ručno kreiranje i ažuriranje šeme baze podataka iz terminala, pozicionirajte se u `PROJEKAT/backend` i pokrenite:

```bash
dotnet ef database update --project PostRoute.DAL --startup-project PostRoute.Api
```

> 💡 **Automatizacija na startu (Runtime):** Unutar `Program.cs` implementiran je kod koji pri svakom startovanju aplikacije (lokalno ili na Render kontejneru) automatski izvršava sve preostale migracije nad bazom podataka pomoću komande `dbContext.Database.MigrateAsync()`.

### 7.3 Inicijalni seed podaci i demo korisnici

Ukoliko sistem detektuje da je baza podataka prazna, servis za seeding automatski vrši inicijalno punjenje (uloge, sandučići, rute) i kreira testne korisnike. Gating je u produkciji kontrolisan preko varijable `Seeding__Enabled`. Predefinisani kredencijali su:

- **Administrator:** `admin@mail.com` / `Admin123!`
- **Dispečer:** `dispatcher@mail.com` / `Dispatcher123!`
- **Poštar:** `postar@mail.com` / `Postar123!`

---

## 8. Pokretanje testova

Izvršavanje automatizovanih testova vrši se pokretanjem sljedećih komandi unutar pripadajućih foldera:

**Backend Unit i Integracioni testovi (xUnit):**

```bash
cd PROJEKAT/backend
dotnet test PostRoute.sln --configuration Release --verbosity normal
```

**Frontend Komponentni testovi (Vitest):**

```bash
cd PROJEKAT/frontend
npm test -- --run
```

---

## 9. Produkcijski deployment (Netlify & Render)

### 9.1 Backend (Render Web Service preko `render.yaml`)

Aplikacija koristi deklarativno upravljanje infrastrukturom ("Infrastructure as Code") pomoću `render.yaml` konfiguracione datoteke locirane u korijenu repozitorija:

- **Runtime:** Docker (`runtime: docker`) sa postavljenom putanjom `rootDir: PROJEKAT/backend` i pripadajućim `Dockerfile`-om.
- **Region i Plan:** Server se podiže u Frankfurtu (`region: frankfurt`) na besplatnom planu (`plan: free`).
- **Health Check Endpoint:** `/health` (Vraća HTTP 200 OK i koristi se za verifikaciju zdravlja kontejnera).
- **Automatski Deployment (CD):** Omogućen (`autoDeploy: true`) na osnovu osluškivanja izmjena nad `main` granom repozitorija.

**Zahtijevane tajne varijable (Render Dashboard -> Environment):**

- `ConnectionStrings__DefaultConnection` – String konekcije za spoljnu Neon Serverless PostgreSQL bazu podataka (sa flagom `SslMode=Require`).
- `Jwt__Secret` – Sigurnosni ključ za JWT token operacije.
- `Cors__AllowedOrigins__0` – Produkcijski URL klijenta podignutog na Netlify-ju (`https://postrouteapp.netlify.app`).
- `Seeding__Enabled` – Postaviti na `true` tokom prvog podizanja, a nakon uspješnog kreiranja baze vratiti na `false`.

### 9.2 Frontend (Netlify Static Site)

- **Izvorni direktorij (Base directory):** `PROJEKAT/frontend`
- **Build Command:** `npm run build`
- **Publish Directory:** `dist`
- **Environment Variables (Netlify Console):** Dodati varijablu `VITE_API_BASE_URL` i postaviti vrijednost na apsolutnu adresu vašeg aktivnog Render API-ja (`https://si-grupa-11.onrender.com`).

---

## 10. Poznata ograničenja deployementa

- **Cold Start (Render Free Tier):** Budući da se koristi besplatni plan na Render platformi, kontejner se automatski gasi i uspavljuje nakon 15 minuta potpune neaktivnosti. Prvi naredni dolazni HTTP zahtjev sa klijenta pokrenuće buđenje kontejnera, što može potrajati 30–60 sekundi prije nego što API počne odgovarati na rute.
- **Zavisnost o eksternim map servisima:** Prikaz interaktivnih geografskih mapa, pozicioniranje markera poštanskih sandučića i kalkulacije ruta direktno zavise od spoljne dostupnosti i stabilnosti javnih API servisa OpenStreetMap i OSRM (Open Source Routing Machine).
- **CORS i SSL restrikcije klijenta:** Zbog restriktivnih pravila modernih web browsera i postavki sigurnih kolačića u `Program.cs` (`SameSite=None; Secure`), frontend aplikacija u produkciji mora obavezno pristupati sistemu preko sigurnog HTTPS protokola, u suprotnom će browser blokirati upis sesije i onemogućiti prijavu.

---

## 11. Najčešći problemi i rješenja (Troubleshooting)

**Problem:** API konstantno vraća `404` grešku na osnovnoj ruti (`/`).

> **Uzrok:** Na osnovnom URL-u nije mapiran nijedan podrazumijevani kontroler niti statička stranica.
>
> **Rješenje:** Ovo je normalno i očekivano ponašanje sistema. Za ispravnu verifikaciju rada i zdravlja API-ja pozovite namjenski endpoint `/health` (npr. `https://si-grupa-11.onrender.com/health`) koji mora vratiti prazan odgovor sa statusnim kodom `200 OK`.

---

**Problem:** Prikazuje se prazan sivi ekran na mjestu gdje treba biti geografska mapa.

> **Uzrok:** Browser klijenta ima mrežne restrikcije ili lokalni firewall blokira CDN servere sa kojih Leaflet povlači mapne slojeve.
>
> **Rješenje:** Provjerite internet konekciju i obezbjedite prohodnost ka spoljnim servisima koje koristi Leaflet biblioteka.

---

**Problem:** Konzola browsera prijavljuje grešku `SyntaxError: Unexpected token '<'`.

> **Uzrok:** SPA ruter klijenta pokušava povući podatke sa pogrešne rute ili je adresa API-ja unutar varijabli okruženja netačna, zbog čega server vraća fallback HTML stranicu umjesto JSON objekta.
>
> **Rješenje:** Detaljno provjerite vrijednost varijable `VITE_API_BASE_URL` u Netlify postavkama. URL mora u potpunosti odgovarati onome na Renderu, bez suvišnih kosih crta (`/`) na samom kraju stringa. Nakon izmjene varijable, obavezno pokrenite novi produkcijski build.

---

**Problem:** Korisnik se uspješno prijavi, ali se sesija gubi odmah nakon osvježavanja stranice (F5).

> **Uzrok:** Browser odbija trajno skladištenje kolačića sesije jer aplikacija radi u razvojnom modu ili nedostaju klijentski parametri.
>
> **Rješenje:** Osigurajte da je na Renderu varijabla `ASPNETCORE_ENVIRONMENT` postavljena na `Production`. Takođe, provjerite da li HTTP klijent na frontendu (Axios/Fetch) ima uključen flag `withCredentials: true` prilikom slanja svakog zahtjeva.
