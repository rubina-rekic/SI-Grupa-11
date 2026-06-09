# Test Summary / QA Izvještaj

**Projekat:** PostRoute — Sistem za optimizaciju ruta punjenja i pražnjenja poštanskih sandučića
**Tim:** Grupa 11
**Sprint:** Završni sprint (Sprint 11)
**Produkcija:** [https://postrouteapp.netlify.app](https://postrouteapp.netlify.app)

---

## 1. Sažetak

Ovaj dokument predstavlja završni pregled testiranja sistema PostRoute. Testiranje je vođeno kontinuirano kroz cijeli razvojni ciklus — uz svaki PBI pisan je odgovarajući set testova, a rezultati su evidentirani u `ProofOfTesting.md` dokumentima po sprintovima (Sprint 6 do Sprint 10). Ovaj izvještaj objedinjuje finalno stanje test suite-a, opisuje vrste testova, način pokretanja, konkretne rezultate posljednjeg punog izvršavanja, ono što je provjereno ručno, te iskreno navodi poznate testne propuste i ograničenja.

Sistem je pokriven na četiri nivoa automatizovanog testiranja (BLL, DAL i API na backendu te komponentno/integraciono na frontendu) uz dopunsko ručno i smoke testiranje za tokove koji zahtijevaju realno okruženje (Leaflet mapa, browser sesija, end-to-end protok).

**Finalno stanje (puno izvršavanje od 2026-05-30):**

| Nivo / projekat | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- |
| Backend — BLL servisi (`PostRoute.BLL.Tests`) | xUnit + Moq + FluentAssertions | 149 | PASS (149/149) |
| Backend — DAL repozitoriji (`PostRoute.DAL.Tests`) | xUnit + EF Core InMemory | 39 | PASS (39/39) |
| Backend — API kontroleri (`PostRoute.Api.Tests`) | xUnit + Moq | 52 | PASS (52/52) |
| Frontend — komponente i integracija | Vitest + React Testing Library | 43 | PASS (43/43) |
| **Ukupno** | | **283** | **PASS (283/283)** |

Pored automatizovanih testova, izvršene su runtime smoke provjere (backend i frontend se pokreću bez stderr grešaka) te ručni E2E scenariji za arhivu ruta i kompletan tok obilaska poštara.

---

## 2. Vrste testova

Testna piramida sistema oslanja se na jedinične i komponentne/integracione testove, uz manuelno testiranje za scenarije koji nisu pokriveni automatizacijom.

### 2.1 Unit testovi — Backend (BLL sloj)

Testira se poslovna logika izolovano, uz mockovanje repozitorija. Pokriveni servisi:

- `UserService` — kreiranje korisnika, validacija jedinstvenosti emaila/korisničkog imena, BCrypt hashiranje, login s lockout logikom (5 pokušaja), promjena lozinke, `MustChangePassword` flag.
- `MailboxService` — CRUD sandučića, validacija GPS koordinata i kapaciteta, audit log po izmijenjenom polju, vremenski okviri dostupnosti i radni dani, promjena statusa i povezivanje sa rutom.
- `RouteService` — generisanje rute (nearest-neighbor), filtriranje po radnom danu i vremenskom okviru, dodjela rute, ručna izmjena redoslijeda, izvještaji o učinku poštara i realizaciji po tipu sandučića.
- `IssueService` — upravljanje problematičnim lokacijama: komentari, dodjela akcija, rješavanje, notifikacije.

### 2.2 Unit testovi — Backend (DAL sloj)

Testira se sloj pristupa podacima nad **EF Core InMemory** bazom (svaki test koristi izolovanu instancu baze sa `Guid.NewGuid()` imenom). Pokriva:

- `MailboxRepository` — CRUD, soft-delete (`IsActive = false`), paginacija, filtriranje po tipu/prioritetu/statusu/adresi (case-insensitive), sortiranje.
- `RouteRepository` — dohvat po datumu i poštaru, eager-loading (Postman, RouteItems.Mailbox), arhiva (samo `Zavrsena`/`Otkazana`), filtriranje po periodu, dohvat aktivne rute za poštara/sandučić.

### 2.3 Unit testovi — Backend (API sloj)

Testiraju se kontroleri uz mockovan servisni sloj, sa fokusom na ispravne HTTP statuse i autorizaciju:

- Provjera povratnih kodova (200, 201, 400, 401, 404, 409).
- Validacija ModelState-a.
- Autorizacija kroz `ClaimsPrincipal` (JWT claims) i `ISession` (session-based auth) — uključujući namjenski `TestSession : ISession` helper.
- Ispravno prosljeđivanje komandi servisu (`UpdateMailboxStatusCommand`, `AssignActionRequest`, itd.).

### 2.4 Komponentni i integracioni testovi — Frontend

Alati: **Vitest** + **React Testing Library** (uz `@testing-library/user-event`, `jest-dom`). Pokriva:

- Renderovanje formi i validaciju (React Hook Form + Zod).
- Korisničke interakcije (klik, unos, odabir iz dropdowna).
- Stanja: loading, empty-state, success, error.
- Integraciju s API slojem kroz mockovane pozive.
- Specifične tokove: kreiranje/izmjena sandučića, dodjela rute, dispečerski dashboard i izvještaji, brza pretraga sa debounce-om.

### 2.5 Ručno i smoke testiranje

Za tokove koji zahtijevaju realno okruženje (Leaflet mapa, browser sesija, kompletan E2E protok) izvršeno je ručno testiranje i runtime smoke provjere — detaljno u sekciji 5.

---

## 3. Kako se testovi pokreću

### 3.1 Backend (.NET 9)

```bash
cd PROJEKAT/backend

# Kompletan solution (sva tri test projekta)
dotnet test PostRoute.sln

# Pojedinačni test projekti
dotnet test tests/PostRoute.BLL.Tests/PostRoute.BLL.Tests.csproj
dotnet test tests/PostRoute.DAL.Tests/PostRoute.DAL.Tests.csproj
dotnet test tests/PostRoute.Api.Tests/PostRoute.Api.Tests.csproj

# Filtriranje po PBI-ju (primjer: PBI-052)
dotnet test tests/PostRoute.BLL.Tests/PostRoute.BLL.Tests.csproj --filter "FullyQualifiedName~PBI052"
dotnet test tests/PostRoute.Api.Tests/PostRoute.Api.Tests.csproj --filter "FullyQualifiedName~PBI052"

# S izvještajem o pokrivenosti koda
dotnet test --collect:"XPlat Code Coverage"
```

**Konfiguracija test projekata:**
- `Microsoft.NET.Test.Sdk` 17.12.0, `xunit` 2.9.2, `xunit.runner.visualstudio` 2.8.2, `coverlet.collector` 6.0.2
- BLL: `Moq` 4.20.72, `FluentAssertions` 8.9.0
- API: `Moq` 4.20.70
- DAL: `Microsoft.EntityFrameworkCore.InMemory` 9.0.0, `Moq` 4.20.69
- Target framework: `net9.0`

### 3.2 Frontend (React + TypeScript + Vite)

```bash
cd PROJEKAT/frontend

# Kompletan test paket
npm test -- --run

# Pojedinačni test fajl (primjer)
npm test -- --run src/ui/pages/admin/test/PostmanPerformanceReportPage.PBI050.test.tsx

# Produkcijski build (verifikacija da se aplikacija builduje)
npm run build
```

---

## 4. Koliko testova prolazi

### 4.1 Finalni rezultati (puno izvršavanje, 2026-05-30)

| Komanda | Rezultat |
| --- | --- |
| `dotnet test PostRoute.sln` | PASS — DAL 39/39, BLL 149/149, API 52/52 |
| `npm test -- --run` | PASS — 43/43 |
| `dotnet build PostRoute.sln` | PASS (uz 1 postojeće MSB3277 upozorenje — vidjeti sekciju 6) |
| `npm run build` | PASS (uz Vite upozorenje o veličini chunk-a) |

**Ukupno: 283/283 automatizovanih testova prolazi (0 failed, 0 skipped na nivou suite-a).**

### 4.2 Rast test suite-a kroz sprintove

Brojevi pokazuju da je pokrivenost rasla kumulativno, uz regresijsku provjeru svih prethodnih testova u svakom sprintu:

| Sprint | BLL | DAL | API | Frontend | Napomena |
| --- | --- | --- | --- | --- | --- |
| Sprint 6 | — | — | — | — | ~116 testova ukupno (auth + sandučići + poštari) |
| Sprint 7 | +12 | — | — | +27 | prioriteti, vremenski okviri, generisanje rute |
| Sprint 8 | 92 | 26 | 15 | 31 | rute: dodjela, detalji, ručni redoslijed |
| Sprint 9 | 136 | 34 | 39 | 36 | terenski rad, dashboard, dnevni izvještaj |
| **Sprint 10** | **149** | **39** | **52** | **43** | arhiva, izvještaji, pretraga, problematične lokacije |

> **Napomena o frontend brojevima:** U Sprint 10 dokumentaciji pojavljuju se različiti zbirovi frontend testova (42, 43 i 52) zavisno od trenutka izvršavanja i dodavanja test fajlova tokom sprinta. Posljednje dokumentovano puno izvršavanje (bugfix verifikacija 2026-05-30) prijavljuje **43/43**, što je vrijednost korištena kao referentna u ovom izvještaju.

### 4.3 Pokrivenost po ključnim PBI-jevima (izvod)

| PBI / US | Funkcionalnost | Nivo pokrivenosti | Status |
| --- | --- | --- | --- |
| PBI-011/012/013/014 | Auth, RBAC, login, promjena lozinke | BLL (UserService), API | PASS |
| PBI-017/018/019 | CRUD sandučića, paginacija, filtriranje | BLL, DAL, API, Frontend | PASS |
| PBI-021 | Vremenski okviri i radni dani | BLL (8 scenarija) | PASS |
| PBI-022 | Generisanje rute (nearest-neighbor) | BLL | PASS |
| PBI-023/024/025 | Dodjela, detalji, ručni redoslijed rute | BLL, DAL, API, Frontend | PASS |
| PBI-026/027/028 | Terenski rad poštara | BLL, API (UI dijelovi ručno) | PASS |
| PBI-029/030 | Dispečerski dashboard, dnevni izvještaj | BLL, DAL, API, Frontend | PASS |
| PBI-049/050/051 | Arhiva, izvještaji o učinku, pretraga | BLL, DAL, API, Frontend | PASS |
| PBI-052 (US-40 do 44) | Problematične lokacije, notifikacije | BLL (IssueService), API | PASS |

---

## 5. Šta je ručno testirano

Tokovi koji zavise od realnog browsera, Leaflet mape ili kompletnog E2E protoka verificirani su ručno, jer projekat nema E2E framework (Playwright/Cypress) niti vizuelni regresijski alat u CI-ju.

### 5.1 Runtime smoke provjere

```bash
# Backend
cd PROJEKAT/backend
dotnet run --project src/PostRoute.Api/PostRoute.Api.csproj --no-build --urls http://localhost:5000
```
Backend se pokrenuo u Development okruženju, primijenio postojeće migracije, izvršio seeding i vratio `HTTP 200` na `/health`. Bez stderr grešaka.

```bash
# Frontend
cd PROJEKAT/frontend
npm run dev -- --host 127.0.0.1 --port 5173
```
Frontend Vite server se pokrenuo na `http://127.0.0.1:5173/` i vratio `HTTP 200`. Bez stderr grešaka.

### 5.2 Ručni E2E scenario — Arhiva ruta (preko development seed-a)

1. Pokrenuti PostgreSQL dev bazu, backend (Development) i frontend.
2. Prijava kao administrator (`admin@mail.com` / `Admin123!`).
3. Otvoriti `Arhiva ruta` → vidljiva završena ruta za `Postar User` sa današnjim datumom i `DEV-ARCH-*` sandučićima.
4. Klik na detalje rute → otvara se **read-only** prikaz s mapom, finalnim statusima, timestampima i CSV export dugmetom.

**Rezultat:** PASS.

### 5.3 Ručni E2E scenario — Kompletan tok obilaska rute

1. Prijava kao dispečer → `Generisanje ruta` → generisanje rute za današnji datum i poštara.
2. Dodjela rute poštaru kroz panel za dodjelu.
3. Prijava kao poštar (`postar@mail.com` / `Postar123!`) → `Moja ruta` (`/worker/route`).
4. Za svaku stavku odabir `Napunjen` / `Ispraznjen` / `Nedostupno` (sa razlogom).
5. Nakon zadnje obrađene stavke backend automatski postavlja rutu na `Zavrsena` i popunjava `CompletedAt`.
6. Ponovna prijava kao administrator → ruta vidljiva u `Arhiva ruta`.

**Rezultat:** PASS.

### 5.4 UI acceptance kriteriji verificirani ručno

Za PBI-028 (označavanje nedostupne lokacije) sljedeći AC verificirani su isključivo ručno, jer komponenta koristi Leaflet mapu i browser sesiju:
- Padajući meni s 5 predefinisanih razloga nedostupnosti.
- Blokada potvrde dok razlog nije odabran.
- Vizuelno označavanje pina na mapi (crveni X).
- Toast notifikacija dispečeru (realizovana kroz auto-refresh dashboarda na 30s).

### 5.5 Ostale ručne provjere (bugfix sprint, 2026-05-30)

- **UTF-8 tekst** na `Pregled sandučića` — uklonjen BOM iz `MailboxListPage.tsx`, normalizovana empty-state poruka.
- **Navigacija `Praćenje ruta → Otvori detalje`** — provjereno da dugme navigira na `/admin/routes/:id`, dodat regresioni test.
- **Responzivni prikaz** mobilne rute na širini 360px (NFR-08).

---

## 6. Poznati testni propusti i ograničenja

U skladu sa zahtjevom da se ograničenja iskreno navedu (a ne prikrivaju), slijedi lista poznatih propusta:

### 6.1 Nedostatak automatizovanog E2E i a11y testiranja

- **Nema E2E framework-a (Playwright/Cypress)** u CI pipeline-u, iako je `TestStrategy.md` predviđao sistemsko i prihvatno testiranje. End-to-end tokovi pokriveni su ručno (sekcija 5), ne automatizovano.
- **`jest-axe` (WCAG a11y) i Storybook/Chromatic** vizuelni regresijski testovi navedeni su u Test Strategiji, ali nisu implementirani u finalnoj verziji.

### 6.2 UI dijelovi pokriveni samo ručno

- AC vezani za Leaflet mapu i browser sesiju (vidjeti 5.4) nisu pokriveni automatizovanim testovima.
- Notifikacije dispečeru oslanjaju se na HTTP polling (30s), ne na WebSocket/SignalR — kašnjenje do 30s nije testirano pod opterećenjem (100+ ruta na sporijoj mreži).

### 6.3 Nedostaci u poslovnoj logici koji nisu eksplicitno testirani

- **Reverzne tranzicije statusa** sandučića (npr. `Ispraznjen → Napunjen`) nisu eksplicitno blokirane ni testirane.
- **Promjene `RouteItem.Status`** ne bilježe se u zaseban audit log; `MailboxAuditLog` pokriva samo polja sandučića.
- **Ručno otkazivanje rute** (`Otkazana`) nema namjenski UI/API tok — status postoji kroz arhivu, ali se praktična provjera radi preko kompletiranja rute.

### 6.4 Pokrivenost koda (code coverage)

- `TestStrategy.md` definiše ciljeve (≥ 70% za poslovnu logiku, ≥ 85% za kritične module, ≥ 75% za frontend komponente). **Konkretne izmjerene vrijednosti pokrivenosti nisu dokumentovane** u Proof of Testing dokumentima — `coverlet.collector` je konfigurisan, ali izvještaj o pokrivenosti nije priložen kao artefakt. Ovo je preporučena dopuna za buduće sprintove.

### 6.5 Build upozorenja (ne utiču na prolaznost testova)

- **MSB3277** — upozorenje o transitivnim verzijama `Microsoft.EntityFrameworkCore.Relational` (9.0.1 vs 9.0.4) u API test/build projektu. Prisutno od Sprinta 8; testovi prolaze.
- **Vite chunk size** — upozorenje da je glavni JS chunk veći od 500 kB. Build prolazi.
- **`LeafletRoutingMachine.tsx`** — pre-existing TypeScript tip za `leaflet-routing-machine`; postoji na `main` grani, nije uveden izmjenama u zadnjim sprintovima.

### 6.6 Test koji je prolazio iz pogrešnog razloga (ispravljeno)

Tokom razvoja zabilježen je slučaj gdje je `ShouldExcludeMailboxesOutsideTimeWindow` prolazio jer je working-days filter isključivao sandučić **prije** provjere vremenskog okvira — test nije zapravo validirao ono za što je napisan. Ispravljeno fiksnim datumom i `WorkingDays = SvakiDan`. Navedeno radi transparentnosti procesa.

---

## 7. Dokaz rezultata testiranja

Dokazi su provjerljivi i ponovljivi pokretanjem komandi iz sekcije 3. Primarni artefakti:

### 7.0 Screenshot dokazi

#### Backend testovi

![Backend testovi](images/backend%20testovi.png)

#### Frontend testovi

![Frontend testovi](images/frontend%20testovi.png)

### 7.1 Log izlazi (reprezentativni)

```
> dotnet test PostRoute.sln
...
Passed!  - Failed: 0, Passed: 39, Skipped: 0   (PostRoute.DAL.Tests)
Passed!  - Failed: 0, Passed: 149, Skipped: 0  (PostRoute.BLL.Tests)
Passed!  - Failed: 0, Passed: 52, Skipped: 0   (PostRoute.Api.Tests)
```

```
> npm test -- --run
...
Test Files  ... passed
     Tests  43 passed (43)
```

### 7.2 Dokumentovani artefakti u repozitoriju

- `Sprint6/ProofOfTesting.md` — auth, poštari, sandučići (~116 testova).
- `Sprint7/ProofOfTesting.md` — prioriteti, vremenski okviri, generisanje rute.
- `Sprint8/ProofOfTesting.md` — dodjela, detalji i ručni redoslijed rute.
- `Sprint9/ProofOfTesting.md` — terenski rad, dashboard, dnevni izvještaj.
- `Sprint10/ProofOfTesting.md` — arhiva, izvještaji o učinku, pretraga, problematične lokacije; uključuje runtime smoke provjere i ručne E2E scenarije.

### 7.3 Test fajlovi (lokacije)

- Backend BLL: `PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/*.cs`
- Backend DAL: `PROJEKAT/backend/tests/PostRoute.DAL.Tests/Repositories/*.cs`
- Backend API: `PROJEKAT/backend/tests/PostRoute.Api.Tests/Controllers/*.cs`
- Frontend: `PROJEKAT/frontend/src/ui/pages/admin/test/*.test.tsx`

---

## 8. Zaključak

Sistem PostRoute pokriven je sa **283 automatizovana testa koji svi prolaze** (149 BLL + 39 DAL + 52 API + 43 frontend), uz dopunsko ručno i smoke testiranje za tokove koji zahtijevaju realno okruženje. Svi ključni korisnički tokovi — autentifikacija i RBAC, upravljanje sandučićima, generisanje i dodjela ruta, terenski rad poštara, dispečerski nadzor i izvještavanje — verificirani su kombinacijom automatizovanih i ručnih testova.

Testiranje je provjerljivo: svaka tvrdnja u ovom dokumentu može se reprodukovati pokretanjem navedenih komandi. Poznata ograničenja (odsustvo automatizovanog E2E/a11y testiranja, nedokumentovana izmjerena pokrivenost koda, dijelovi pokriveni samo ručno) jasno su navedena i predstavljaju prioritete za eventualni nastavak projekta.
