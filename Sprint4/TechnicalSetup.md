# Technical Setup
### Grupa 11 - Sistem za optimizaciju ruta punjenja i pražnjenja poštanskih sandučića

---

## 1. Branching strategija

### 1.1 Odabrani model
Odabran je **GitLab Flow** (varijanta sa `main` i `develop` granom).

Razlog odabira:
- Jednostavniji je od punog Gitflow modela, a daje bolju kontrolu integracije od čistog trunk pristupa.
- Usklađen je sa postojećim artefaktima projekta koji već navode rad kroz `develop/main`.
- Dobro odgovara timu koji radi sprintovski i uvodi funkcionalnosti inkrementalno.

Napomena: Iako je repozitorij na GitHub-u, workflow pravila i tok rada slijede GitLab Flow princip.

### 1.2 Uloge grana

| Grana | Namjena | Pravila |
|---|---|---|
| `main` | Stabilna grana spremna za release i demonstraciju | Zaštićena, bez direktnog push-a, merge isključivo kroz PR/MR |
| `develop` | Integraciona grana aktivnog sprinta | Feature/fix/docs grane se prvo spajaju ovdje |
| `feature/PBI-xxx-kratak-opis` | Nova funkcionalnost vezana za backlog stavku | Kreira se iz `develop`, briše nakon merge-a |
| `fix/PBI-xxx-kratak-opis` | Ispravka greške u aktivnom razvoju | Kreira se iz `develop`, briše nakon merge-a |
| `hotfix/kratak-opis` | Hitna ispravka na stabilnoj verziji | Kreira se iz `main`, pa merge u `main` i `develop` |
| `docs/PBI-xxx-kratak-opis` | Dokumentacijske izmjene | Kreira se iz `develop`, merge nazad u `develop` |

### 1.3 Pravila rada

1. Direktan commit na `main` i `develop` nije dozvoljen.
2. Svaka promjena ide kroz Merge Request / Pull Request.
3. Minimalno jedan član tima mora odobriti MR/PR prije merge-a.
4. MR/PR mora biti povezan sa konkretnim PBI-jem.
5. CI pipeline mora proći prije merge-a.
6. Jedna grana pokriva jednu primarnu stavku (bez miješanja nepovezanih izmjena).
7. Nakon merge-a kratkoživu granu obavezno obrisati.

### 1.4 Standardni tok rada (feature)

1. Ažurirati lokalni `develop`.
2. Kreirati granu, npr. `feature/PBI-022-generisanje-rute`.
3. Implementirati promjene kroz manje, logički grupisane commitove.
4. Otvoriti MR/PR prema `develop`.
5. Proći review i CI.
6. Merge u `develop`.
7. Na kraju planiranog inkrementa/sprinta otvoriti MR/PR `develop -> main`.

### 1.5 Hotfix tok

1. Kreirati `hotfix/...` iz `main`.
2. Ispravku merge-ati u `main` nakon review-a i CI provjere.
3. Istu ispravku odmah merge-ati i u `develop` da grane ostanu usklađene.

### 1.6 Konvencija commit poruka
Koristi se Conventional Commits:

- `feat:` nova funkcionalnost
- `fix:` ispravka greške
- `docs:` izmjena dokumentacije
- `refactor:` interna promjena bez promjene ponašanja
- `test:` dodavanje/izmjena testova
- `chore:` konfiguracija, dependency, tooling

Primjeri:
- `feat(routes): implementirati osnovu za PBI-022`
- `fix(auth): zaključavanje naloga nakon 5 pokušaja`
- `docs(sprint4): ažuriran technical setup`

### 1.7 Branch protection (preporuka)

- Obavezan MR/PR za `main` i `develop`
- Obavezan najmanje 1 review approve
- Obavezan prolaz CI checks prije merge-a
- Onemogućen force-push na zaštićene grane

---

## 2. Tehnički stack

### 2.1 Frontend

| Tehnologija | Verzija | Zašto | Alternativa i zašto odbijena |
|---|---|---|---|
| React | 19.x | De facto standard za SPA razvoj; komponente se prirodno mapiraju na UI dijelove sistema (mapa, lista ruta, forma); najšira ekosistema biblioteka | **Angular** — previše overhead (moduli, decoratori, CLI) za MVP scope; **Vue.js** — manji ekosistem, manje iskustva u timu |
| TypeScript | 6.x (strict) | Kompajlerska provjera tipova eliminiše cijelu klasu grešaka pri radu s API modelima; shared tipovi između frontend i backend projekata | **JavaScript** — nema tipizacije; greške u API kontraktu se otkrivaju tek u runtime-u, ne pri kompilaciji |
| Vite | 8.x | HMR ispod 50ms; native TypeScript podrška bez konfiguracije; produkcijski build optimizovan za SPA | **Create React App** — zastarjelo, sporiji build, više konfiguracije; **Webpack** — ručna konfiguracija nije opravdana za ovaj scope |
| React Router | v6 | Standardno rješenje za klijentski routing; deklarativna definicija zaštićenih ruta po RBAC ulozi | **TanStack Router** — zreliji TypeScript, ali ekosistema manji i tim nema iskustva |
| Tailwind CSS | 3.x | Utility-first eliminira konflikte CSS klasa; konzistentan dizajn bez custom CSS fajlova; dobro se skalira u timu | **MUI / Ant Design** — gotove komponente, ali teže prilagoditi dizajn; veći bundle; **CSS Modules** — više pisanja za isti rezultat |
| React Hook Form + Zod | latest | Hook Form smanjuje re-rendere pri validaciji; Zod validacijske sheme se dijele s backendom (isti kontrakt) | **Formik** — sporiji re-renderi u kompleksnim formama; **Yup** — manje TypeScript podrške od Zoda |
| Axios | latest | Interceptori za automatsko dodavanje JWT tokena i globalno rukovanje greškama; centralizovan API klijent | **Fetch API** — nema interceptore nativno; zahtijeva više boilerplate za JWT i error handling |

### 2.2 Backend

| Tehnologija | Verzija | Zašto | Alternativa i zašto odbijena |
|---|---|---|---|
| ASP.NET Core Web API | .NET 9 | Trenutno najnovija verzija; ugrađen DI kontejner, middleware sistem i Kestrel web server; odlična performansa za REST API-je; integracija s ASP.NET Identity bez dodatnih paketa | **Java Spring Boot** — tim nema Java iskustva; **Node.js + Express** — TypeScript full-stack opcija, ali tim ima više iskustva s C# za domenski model |
| C# | 13 | Statički tipiziran; odlična podrška za domenski model s record tipovima i pattern matchingom; tim ima prethodno iskustvo | **F#** — funkcionalniji pristup, ali manji ekosistem i manje iskustva u timu |
| ASP.NET Identity | — | Gotovo, dobro testirano rješenje za upravljanje korisnicima i BCrypt hashiranje — ne treba izmišljati autentifikaciju od nule | **Ručna implementacija** — rizik sigurnosnih propusta; **Keycloak** — overkill za MVP, uvodi dependency na eksterni servis |
| JWT autentifikacija + RBAC | — | Stateless autentifikacija; token nosi ulogu korisnika — backend ne mora hitati u bazu pri svakom zahtjevu; prikladna za SPA koji ne koristi cookies | **Session-based auth** — zahtijeva server-side session storage što komplikuje horizontalno skaliranje; **OAuth2/OIDC** — overkill za interni sistem bez eksternih identity provajdera |
| Swagger / OpenAPI | — | API kontrakt dostupan u realnom vremenu na `/swagger`; frontend može razvijati s mock podacima dok backend nije gotov — kritično za paralelni razvoj | **Postman kolekcija** — nije verzionisana uz kod; zahtijeva ručno ažuriranje |

### 2.3 Data layer

| Tehnologija | Verzija | Zašto | Alternativa i zašto odbijena |
|---|---|---|---|
| Entity Framework Core | 8.x (Code-First) | ORM eliminiše ručni SQL za CRUD operacije; Code-First migracije drže shemu sinhroniziranom s domenskim modelom; LINQ upiti su type-safe | **Dapper** — brži, ali zahtijeva ručni SQL za sve upite što usporava razvoj; **ADO.NET direktno** — previše boilerplate koda |
| PostgreSQL | 16.x | Besplatan, open-source; odlična podrška za geolokacijske podatke (PostGIS ekstenzija za GPS koordinate sandučića dostupna post-MVP); ACID transakcije; bolje performanse za kompleksne upite od MySQL-a | **MySQL** — slabija podrška za JSON i geolokacijske tipove; **SQL Server** — licencni troškovi, Windows-orijentisan što je neusklađeno s Ubuntu deployment targetom; **SQLite** — ne podržava konkurentni višekorisnički pristup |

### 2.4 Mapa i geolokacija
- OpenStreetMap kao map podloga (besplatan, bez API ključa za osnovnu upotrebu)
- Odluka o konkretnoj map biblioteci biće evidentirana u Decision Logu (PBI-040) na početku Sprinta 5

**Kandidati za evaluaciju:**

| Biblioteka | Prednosti | Mane |
|---|---|---|
| Leaflet.js | Lagana, zrela, odlična dokumentacija | Manje TypeScript podrške |
| React Leaflet | React-native wrapper oko Leaflet-a | Dodatna apstrakcija, ponekad zaostaje za Leaflet verzijom |
| MapLibre GL JS | WebGL rendering, odlične performanse za veće skupove podataka | Teža konfiguracija |

**Kriteriji odluke:** lakoća integracije sa React/TypeScript stackom, podrška za crtanje ruta, performanse pri renderovanju više markera (sandučići na mapi), veličina bundle-a.

---

## 3. CI/CD i kvalitet

### 3.1 CI platforma
- GitHub Actions (workflow fajlovi u `.github/workflows/`)

### 3.2 Pipeline po MR/PR-u

**Frontend (`frontend-ci.yml`):**
1. `npm ci` — instalacija zavisnosti
2. `npm run type-check` — TypeScript kompilacija bez emitovanja
3. `npm run lint` — ESLint provjera
4. `npm run test` — Vitest unit testovi
5. `npm run build` — produkcijski build (Vite)

**Backend (`backend-ci.yml`):**
1. `dotnet restore` — vraćanje NuGet paketa
2. `dotnet build --no-restore` — kompilacija
3. `dotnet test --no-build` — xUnit testovi
4. `dotnet publish` — provjera da je aplikacija objavljivana

### 3.3 Quality gate thresholds
- Nula TypeScript grešaka (strict mode)
- Nula ESLint grešaka (warnings su dozvoljeni, ali dokumentovani)
- Svi testovi moraju proći (0 failed)
- Build mora uspjeti bez grešaka

### 3.4 Pravila merge-a
- MR/PR bez prolaska CI ne ulazi u `develop` ni `main`
- Merge u `main` predstavlja release kandidat za demo/isporuku

---

## 4. Lokalni razvojni setup

### 4.1 Preduvjeti

| Alat | Verzija | Napomena |
|---|---|---|
| Git | 2.x+ | — |
| Node.js | 22 LTS | Preporučeno upravljati kroz `nvm` |
| npm | 10.x+ | Dolazi uz Node.js 22 |
| .NET SDK | 8.0 | `dotnet --version` mora vraćati `8.0.x` |
| PostgreSQL | 16.x | Lokalno ili Docker kontejner |

### 4.2 Inicijalni setup

```bash
# Kloniranje i setup
git clone <repo-url>
cd SI-Grupa-11/PROJEKAT

# Frontend
cd frontend
npm ci

# Backend
cd ../backend
dotnet restore
dotnet run --project src/PostRoute.Api
```

### 4.3 Provjera da setup radi (smoke test)

```bash
# Frontend — treba se pokrenuti na http://localhost:5173
cd frontend && npm run dev

# Backend — Swagger dostupan na http://localhost:5000/swagger
cd backend && dotnet run --project src/PostRoute.Api

# Pokretanje testova (kada budu dodani u Sprint 5+)
cd frontend && npm run test
cd backend && dotnet test
```

### 4.4 Konfiguracija
- Environment varijable kroz `.env.local` (frontend) i `appsettings.Development.json` (backend) — dodaje se u Sprint 5 pri uvođenju baze i autentifikacije
- Fajlovi sa tajnama nikad se ne commitaju; template fajlovi (`.env.example`, `appsettings.Example.json`) se commitaju bez stvarnih vrijednosti
- Docker i GitHub Actions workflow fajlovi planiraju se dodati u Sprint 5 (PBI-038)

---

## 5. Deployment strategija

### 5.1 Ciljna platforma

Aplikacija se deployuje na **virtualnu mašinu (VM) u oblaku** ili ekvivalentni Linux server.

| Parametar | Odabir | Obrazloženje | Alternativa i zašto odbijena |
|---|---|---|---|
| Tip okruženja | Cloud VM | Potpuna kontrola nad okruženjem; nema vendor lock-in; jednostavniji od orchestratora za tim od 6 osoba | **Azure App Service / AWS Elastic Beanstalk** — managed servisi uvode troškove i zahtijevaju kreditne kartice; nepraktično za akademski projekat; **Bare metal** — skuplje i teže za reprodukciju okruženja |
| Operativni sistem | **Ubuntu 22.04 LTS** | Najraširenija Linux distribucija za .NET deployment; Microsoft zvanično podržava .NET 8 na Ubuntu 22.04; LTS garancija podrške do 2027; besplatan | **Windows Server** — licencni troškovi; **Debian** — manje Microsoft dokumentacije za .NET; **Ubuntu 24.04** — kraća LTS historija, manje provjenih recepta za .NET 8 deployment |
| Kontejnerizacija | **Docker + Docker Compose** | Eliminiše "radi na mom računaru" probleme; svi servisi definirani u jednom fajlu; brzo podizanje okruženja na novom serveru jednom komandom | **Kubernetes** — značajna kompleksnost (cluster management, YAML overhead) nije opravdana za MVP; **Vagrant** — VM-based, sporiji i teži za dijeljenje; **bez kontejnera** — razlike u okruženjima između developera uzrokuju skrivene bugove |

### 5.2 Arhitektura deployovanja

**Komponente:**
- **Nginx** — web server i reverse proxy; servira statički React build (`npm run build` izlaz); proksira sve `/api/*` zahtjeve na Kestrel; terminira SSL
- **Kestrel** — ASP.NET Core-ov ugrađeni web server; radi unutar Docker kontejnera; nije direktno izložen internetu (uvijek iza Nginxa)
- **PostgreSQL** — baza podataka u Docker kontejneru; podaci persistentni na host volumu (`./data/postgres`)
- **React SPA** — produkcijski build kao statički fajlovi; servira ih Nginx, ne postoji posebni Node.js process u produkciji

### 5.3 Docker Compose struktura

Produkcijsko okruženje podizano sa:
```bash
docker compose -f docker-compose.prod.yml up -d
```

Servisi definirani u `docker-compose.prod.yml`:
- `nginx` — reverse proxy, volumeni za React build i SSL certifikate
- `backend` — .NET 8 aplikacija, zavisna od `postgres`
- `postgres` — PostgreSQL 16, persistentni volumen za podatke

### 5.4 Obrazloženje deployment odabira

**Zašto VM, a ne serverless/PaaS?**
Tim ima potpunu kontrolu nad okruženjem i konfiguracijama; nema vendor lock-in; lakše debugovanje i logging; dovoljna skalabilnost za MVP sa < 100 korisnika.

**Zašto Docker Compose, a ne Kubernetes?**
Kubernetes donosi značajnu kompleksnost (orchestration, YAML konfiguracija, cluster management) koja nije opravdana za tim od 6 osoba i MVP scope projekta. Docker Compose daje iste prednosti konzistentnosti bez operativnog overhead-a.

**Zašto Nginx kao reverse proxy, a ne IIS?**
IIS je Windows-specifičan; Ubuntu server + Nginx kombinacija je standardna za .NET Core deployment na Linuxu; Nginx je lakši i performansniji za serving statičkih fajlova od Kestrel-a direktno. **Apache** je alternativa ali veći memory footprint i sporija konfiguracija od Nginxa.

**Zašto ne čisti cloud provider servisi (Azure App Service, AWS Elastic Beanstalk)?**
Managed cloud servisi uvode troškove i zahtijevaju kreditne kartice. Za akademski projekat, Docker Compose na VM-u ili besplatnom tier-u cloud provajdera je praktičniji izbor bez finansijskog rizika.

### 5.5 Environment konfiguracija

| Okruženje | Frontend | Backend | Baza |
|---|---|---|---|
| Development | `http://localhost:5173` (Vite dev server) | `https://localhost:7000` (Kestrel HTTPS) | PostgreSQL lokalno ili Docker |
| Production | Statički fajlovi servira Nginx | Kestrel iza Nginxa, port 5000 | PostgreSQL Docker kontejner |

Tajne (DB lozinke, JWT secret, SMTP credentials) upravljaju se kroz:
- Docker secrets ili environment varijable u `docker-compose.prod.yml` (ne commitati u repozitorij)
- Na VM-u kroz `/etc/environment` ili `.env` fajl koji nije u git-u

---

## 6. Pregled stack-a

| Sloj | Tehnologija | Verzija |
|---|---|---|
| Frontend | React + TypeScript + Vite | React 19.x, TS 6.x, Vite 8.x |
| UI stilovi | Tailwind CSS | 3.x |
| Forme i validacija | React Hook Form + Zod | latest |
| HTTP klijent | Axios | latest |
| Routing (frontend) | React Router | v6 |
| Backend | ASP.NET Core Web API (C#) | .NET 9 |
| Autentifikacija | ASP.NET Identity + JWT | — |
| ORM | Entity Framework Core (Code-First) | 8.x |
| Baza podataka | PostgreSQL | 16.x |
| Mapa i geolokacija | OpenStreetMap + biblioteka (odluka Sprint 5) | — |
| Web server | Nginx (reverse proxy) + Kestrel | — |
| Kontejnerizacija | Docker + Docker Compose | — |
| Operativni sistem | Ubuntu 22.04 LTS | — |
| Hosting | Cloud VM | — |
| CI/CD | GitHub Actions | — |
| Branching | GitLab Flow (`main` + `develop`) | — |

---

## 7. Usklađenost sa projektnim artefaktima

Ovaj Technical Setup je usklađen sa:
- Architecture Overview (monolit, API/BLL/DAL, React SPA)
- Test Strategy (review + CI + regresiono testiranje)
- Non-Functional Requirements (sigurnost, performanse, održivost)
- Definition of Done (code review, testiranje, merge disciplina)
- Initial Release Plan (tehnički skeleton kao preduslov implementacije)

---

*Grupa 11 - Sprint 4 (April 2026)*
