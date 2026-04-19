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
- React 18
- TypeScript (strict)
- Vite
- React Router v6
- Tailwind CSS
- React Hook Form + Zod
- Axios (centralizovan API klijent)

### 2.2 Backend
- ASP.NET Core Web API
- C#
- ASP.NET Identity
- JWT autentifikacija i RBAC (Admin / Dispečer / Poštar)
- Swagger / OpenAPI

### 2.3 Data layer
- Entity Framework Core (Code-First, migracije)
- PostgreSQL

### 2.4 Mapa i geolokacija
- OpenStreetMap kao map podloga
- Konkretna map biblioteka (npr. Leaflet) formalizuje se kroz Decision Log u implementacijskoj fazi

---

## 3. CI/CD i kvalitet

- GitHub Actions kao CI osnova
- Minimalni pipeline po MR/PR-u:
  - Build
  - Test suite
  - Provjera osnovnih quality gate-ova
- MR/PR bez prolaska CI ne ulazi u `develop`/`main`
- Merge u `main` predstavlja release kandidat za demo/isporuku

---

## 4. Lokalni razvojni setup (osnovni)

- Git i GitHub repozitorij
- Node.js LTS za frontend dio
- .NET SDK (verzija usklađena sa ASP.NET Core projektom)
- PostgreSQL lokalno ili kroz definisano razvojno okruženje
- Environment konfiguracija kroz `.env` / appsettings, bez hardkodiranih tajni

---

## 5. Usklađenost sa projektnim artefaktima

Ovaj Technical Setup je usklađen sa:
- Architecture Overview (monolit, API/BLL/DAL, React SPA)
- Test Strategy (review + CI + regresiono testiranje)
- Non-Functional Requirements (sigurnost, performanse, održivost)
- Definition of Done (code review, testiranje, merge disciplina)
- Initial Release Plan (tehnički skeleton kao preduslov implementacije)

---

*Grupa 11 - Sprint 4 (April 2026)*
