# Architecture Overview


## Kratak opis arhitektonskog pristupa

Sistem se implementira kao monolitna web aplikacija, pri čemu je backend organizovan prema troslojnoj arhitekturi: **API**, **BLL** i **DAL**, a frontend kao React SPA sa vlastitom slojevitom organizacijom: **Presentation (UI)**, **Application (state/hooks/servisi)** i **Infrastructure (API klijent, router, storage)**.

Frontend i backend komuniciraju isključivo putem REST API-ja. Backend je ASP.NET Core aplikacija u kojoj **API sloj** izlaže HTTP endpointe, **BLL (Business Logic Layer)** sadrži poslovna pravila i optimizacijsku logiku, dok **DAL (Data Access Layer)** upravlja pristupom podacima preko Entity Framework Core ORM-a. Podaci se čuvaju u PostgreSQL bazi podataka.

Monolitna arhitektura odabrana je zbog veličine tima, vremenskog okvira od jednog semestra i potrebe za brzim iteracijama. Troslojna organizacija backenda i zrcalna slojevita organizacija frontenda uvode jasnu separaciju odgovornosti i olakšavaju održavanje, testiranje i daljnje proširenje sistema.

| **Arhitekturni stil** | **Primarni stack** | **Deployment model** |
|---|---|---|
| Monolitna; backend organizovan po troslojnoj arhitekturi (API/BLL/DAL), frontend po slojevitoj SPA arhitekturi (UI/Application/Infrastructure) | React + Vite + TypeScript SPA + ASP.NET Core Web API + PostgreSQL | Single-instance server (cloud hosted), responsive web klijent; frontend servisiran kao statički build iza CDN-a ili reverse proxy-ja |

---

## Glavne komponente sistema

Sistem se sastoji od klijentskog dijela i backenda organizovanog kroz tri jasno odvojena sloja: API, BLL i DAL.

| **Komponenta** | **Tehnologija** | **Odgovornost** | **Status** |
|---|---|---|---|
| **Presentation Layer (Frontend)** | React 18 + TypeScript SPA, Vite build tool | Administratorski panel, dispečerski dashboard, mobilni prikaz za poštara. Komunicira isključivo putem REST API-ja. Organizovana kroz tri vlastita sloja (UI / Application / Infrastructure). | Arhitekturno stabilno |
| **Frontend Routing & Route Guards** | React Router v6 | Client-side rutiranje, lazy loading po ulozi, zaštita ruta kroz `<RequireAuth roles={...}/>` wrapper komponentu. | U razvoju |
| **Frontend State Management** | React Context API + custom hooks (MVP); TanStack Query razmatra se u Sprintu 5 (OQ-009) | Globalni auth state, server-state cache i invalidacija, UI state (modali, notifikacije). | U razvoju |
| **Frontend API Client** | axios instanca sa request/response interceptorima | Centralizovano ubacivanje JWT-a u `Authorization` header, handling 401 odgovora, mapiranje grešaka na user-friendly poruke. | U razvoju |
| **Frontend Forms & Validation** | React Hook Form + Zod schema | Performantne kontrolisane forme, runtime validacija ulaza, dijeljenje tipova između forme i API DTO-a. | U razvoju |
| **Frontend UI / Styling** | Tailwind CSS + shared UI komponente (design system) | Utility-first styling, konzistentan izgled između admin / dispečerskog / mobilnog prikaza, tamna/svijetla tema. | U razvoju |
| **API Layer** | ASP.NET Core Web API | Ulazna tačka sistema. Prima HTTP zahtjeve, vrši autentifikaciju/autorizaciju, validaciju zahtjeva i izlaže REST endpointe koje konzumira frontend. | U razvoju |
| **Business Logic Layer (BLL)** | C# servisni sloj / class library | Sadrži poslovnu logiku sistema: upravljanje korisnicima, rutama, sandučićima, pravilima rada i orkestraciju use-caseova. | U razvoju |
| **Optimizacijski modul** | C# servis (interni, dio BLL-a) | Generiše optimalnu dnevnu rutu na osnovu lokacija, prioriteta i radnih pravila sandučića (nearest-neighbor heuristika u MVP-u). | U razvoju |
| **Data Access Layer (DAL)** | Entity Framework Core | Enkapsulira sve upite prema bazi, definiše entitetske modele, repozitorije i migracije. | U razvoju |
| **Baza podataka** | PostgreSQL | Relacijska baza: korisnici, sandučići, rute, statusi obilazaka, historija. Jedini izvor istine za sve podatke sistema. | U razvoju |
| **Autentifikacijski podsistem** | JWT + ASP.NET Identity | Prijava korisnika, hashiranje lozinki (bcrypt), generisanje JWT tokena, RBAC (tri uloge: Admin, Dispečer, Poštar). | Kritična — zahtijeva posebnu pažnju |
| **Map library (frontend)** | OpenStreetMap tile server + map biblioteka (konkretan izbor — npr. Leaflet — donosi se u Sprintu 4 i evidentira u Decision Logu, PBI-040) | Prikaz sandučića i ruta na mapi, interaktivno odabiranje koordinata pri unosu sandučića (US-14). Integracija izvedena tako da je tile provajder zamjenjiv bez promjene aplikacijske logike. | U razvoju — zavisi od OSM-a kao eksternog servisa (vidi R-008) |

---

## Odgovornosti komponenti

### Presentation Layer - React SPA

Frontend je implementiran kao React 18 + TypeScript SPA koja prati vlastitu slojevitu arhitekturu. Ova organizacija odslikava troslojnu podjelu backenda i uvodi istu vrstu separacije odgovornosti na klijentskoj strani.

#### Slojevita organizacija frontenda

| **Sloj** | **Sadržaj** | **Odgovornost** |
|---|---|---|
| **UI / Presentation Layer** | React komponente: Pages, Layouts, Shared UI, Feature komponente | Prikaz podataka, korisnička interakcija, kompozicija sučelja. Ne sadrži poslovna pravila niti direktne pozive API-ju. |
| **Application Layer** | Custom hooks, state management (Context + reducers), view-modeli, klijentske validacije, mapiranje DTO ↔ view model | Orkestracija UI stanja, forme, validacije, poziv ka Infrastructure sloju. Klijentska analogija backend BLL-u. |
| **Infrastructure Layer** | API klijent (axios instanca), interceptori, React Router, auth storage (sessionStorage/in-memory), future WebSocket/poll klijent | Komunikacija sa backendom, rutiranje, perzistencija sesije, mapiranje HTTP grešaka. Klijentska analogija backend DAL-u. |

Komponenta nikada ne pristupa axios-u direktno — svi pozivi idu kroz Application → Infrastructure sloj, čime se održava ista disciplina odvajanja koja važi za backend.

#### Struktura projekta (feature-based)

```
src/
├── app/              # Bootstrap, global providers, router, auth context
├── features/         # Vertikalni moduli po use-caseovima
│   ├── auth/         # Login, promjena lozinke (US-01, US-06)
│   ├── users/        # Admin CRUD nad korisnicima (PBI-011..014)
│   ├── mailboxes/    # Evidencija sandučića + mapa (PBI-017..019)
│   ├── routes/       # Generisanje i dodjela ruta (PBI-022..025)
│   ├── field/        # Mobilni prikaz za poštara (PBI-026..028)
│   └── reports/      # Dnevni izvještaji (PBI-030, PBI-049, PBI-050)
├── shared/           # Dijeljene UI komponente, hooks, utils, tipovi
├── api/              # axios instanca, endpoint wrapperi, DTO tipovi
└── assets/           # Statičke datoteke (ikone, slike, fontovi)
```

Feature-based podjela (umjesto tipskog grupiranja — `components/`, `pages/`, `services/`) izabrana je jer omogućava da tim radi na zasebnim use-caseovima bez konflikata i olakšava buduće izdvajanje modula ako sistem preraste monolit.

#### Glavne tehnologije i odgovornosti

- **Build tool:** Vite (brzi dev server, HMR, ESM build, automatski tree-shaking).
- **UI runtime:** React 18 sa funkcionalnim komponentama i hooks API-jem.
- **Jezik:** TypeScript u strict mode-u — tipovi DTO-a se dijele između API klijenta i formi.
- **Routing:** React Router v6 sa guarded (protected) rutama po ulozi. Nepristupačne rute preusmjeravaju na login ili 403 stranicu.
- **State management:** lokalni state kroz `useState`/`useReducer`; aplikativni state kroz Context API + custom hooks za MVP. Uvođenje TanStack Query-a za server state (cache, refetch, invalidation) razmatra se u Sprintu 5 (OQ-009).
- **Forme i validacija:** React Hook Form za performantnu obradu kontrolisanih formi + Zod schema za runtime validaciju i automatsko izvođenje TypeScript tipova.
- **Styling:** Tailwind CSS kao utility-first pristup + shared UI komponente za konzistentan design system između tri pogleda.
- **HTTP klijent:** axios instanca sa:
    - **request interceptorom** koji automatski injektuje JWT iz auth contexta u `Authorization: Bearer ...` header,
    - **response interceptorom** koji na 401 pokušava refresh token flow i, ako ne uspije, izvodi forced logout i preusmjerava na login.
- **Map komponenta:** `<MapView />` wrapper oko map biblioteke (konkretan izbor — Leaflet — donosi se u Sprintu 4, PBI-040). Wrapper izoluje tile provider (OSM) kako bi bio zamjenjiv bez dodira na feature kod (R-008).
- **Error handling:** globalni `<ErrorBoundary />` na nivou aplikacije + feature-level fallback UI. API greške mapiraju se na user-friendly poruke kroz toast notifikacije.
- **Code splitting:** route-based lazy loading (`React.lazy` + `Suspense`) za admin, dispečerski i mobilni prikaz. Mobilni korisnici (poštari) ne učitavaju admin bundle, što smanjuje inicijalni transfer na terenskom mobilnom internetu.
- **i18n (planski):** aplikacija je primarno na bosanskom jeziku; svi stringovi prolaze kroz dictionary pattern (`t('user.create.title')`) radi lakše buduće lokalizacije.

#### Tri funkcionalna pogleda zavisna od uloge

Frontend pruža tri funkcionalno odvojena pogleda, određena ulogom prijavljenog korisnika:

- **Administratorski panel (desktop-first):** upravljanje korisničkim računima (PBI-011 do PBI-014), evidencija sandučića (PBI-017 do PBI-019), pretraga i filtriranje (PBI-051).
- **Dispečerski dashboard (desktop-first):** generisanje i dodjela ruta (PBI-022 do PBI-025), praćenje realizacije (PBI-029), pristup izvještajima (PBI-030, PBI-049, PBI-050).
- **Mobilni prikaz za poštara (mobile-first, PWA kandidat):** pregled dodijeljene rute (PBI-026), ažuriranje statusa sandučića (PBI-027), evidencija nedostupnih lokacija (PBI-028).

Svi pogledi dijele isti codebase, design system i API klijent, ali koriste različite layout komponente, rute i bundle chunk-ove.

#### Autentifikacija i sigurnost na klijentu

JWT token se čuva u memoriji aplikacije (React context + sessionStorage kao fallback za istu tab sesiju), a ne u localStorage-u, kako bi se smanjio rizik od XSS napada. Ovaj pristup ima kompromis u vidu gubitka autentifikacijske sesije pri potpunom osvježavanju stranice (hard refresh), što se ublažava refresh-token mehanizmom.

- **Protected routes:** wrapper komponenta `<RequireAuth roles={['Admin','Dispecer']}/>` provjerava JWT i ulogu prije renderovanja djece; neautorizovani korisnici se preusmjeravaju na login ili 403 stranicu.
- **HTTP interceptor:** axios request interceptor dodaje `Authorization` header; response interceptor na 401 pokušava refresh i, ako ne uspije, izvodi forced logout.
- **Force password change:** pri prijavi, ako API vrati `requiresPasswordChange: true`, router automatski preusmjerava na ekran za promjenu lozinke i blokira pristup ostatku aplikacije (US-06).
- **CSRF:** nije relevantan dok se koristi Bearer token u headeru; postaje bitan ako se u produkciji pređe na HttpOnly cookie varijantu.
- **Content Security Policy:** strict CSP header sa whitelistom origin-a za API, mapu (OSM tile server) i fontove.

U produkcijskom okruženju razmatra se korištenje HttpOnly cookies za pohranu tokena, uz dodatnu CSRF zaštitu, čime bi se postigla bolja ravnoteža između sigurnosti i korisničkog iskustva.

#### Komunikacija sa backendom

Svi pozivi prolaze kroz centralizovan API sloj (`src/api/`) koji wrapa axios instancu. Svaki feature ima svoj "service" fajl (npr. `usersApi.ts`, `routesApi.ts`) koji izlaže tipizirane funkcije (`createUser(dto): Promise<UserDto>`, `getRouteById(id): Promise<RouteDto>`). React komponente pozivaju samo te funkcije — nikada direktno axios.

Ovaj pristup omogućava:
- Centralizovan handling grešaka i tokena.
- Lako mock-ovanje u testovima (zamjena service fajla stub-om).
- Jedinstvenu tačku za dodavanje logging-a, retry logike, cache-a ili migraciju na TanStack Query.

#### Testiranje frontenda

- **Unit testovi:** Vitest za pure funkcije, reducer logiku i mappere.
- **Hook testovi:** React Testing Library `renderHook` za custom hooks (npr. `useAuth`, `useRouteDetails`).
- **Komponentni testovi:** React Testing Library + jsdom za ključne UI tokove (login forma, lista ruta, status update dugme).
- **E2E testovi (Sprint 8+):** Playwright za kritične end-to-end tokove (prijava → pregled rute → ažuriranje statusa sandučića).

#### Responzivnost, pristupačnost i performanse

- **Mobile-first** za poštarski prikaz (primaran radni uređaj je telefon, često na slabijoj mreži).
- **Desktop-first** za admin i dispečerski prikaz (raditi će se sa većim tabelama, mapama i formama).
- **WCAG 2.1 AA** kao non-functional requirement: dovoljni kontrast, navigacija tastaturom, ARIA atributi na svim interaktivnim komponentama.
- **Performance budget:** inicijalni bundle za mobilni prikaz target ≤ 200 KB gzip; route-based code splitting osigurava da admin/dispečerski kod ne završi u poštarskom bundle-u.

### API Layer - ASP.NET Core Web API

API sloj predstavlja ulaznu tačku u sistem i zadužen je za komunikaciju s frontendom.

- **Controllers / Endpoints:** primaju HTTP zahtjeve i vraćaju odgovore prema klijentu.
- **Autentifikacija i autorizacija:** JWT validacija, kontrola pristupa po ulogama i zaštita endpointa.
- **Validacija zahtjeva i DTO modeli:** provjera ulaznih podataka prije prosljeđivanja poslovnoj logici.
- **Delegiranje ka BLL-u:** API sloj ne sadrži poslovna pravila, nego poziva odgovarajuće servise iz BLL-a.

### Business Logic Layer (BLL)

BLL predstavlja centralni sloj sistema i sadrži svu poslovnu logiku.

- **Services / Use-case logika:** upravljanje korisnicima, sandučićima, rutama i izvještajima.
- **Poslovna pravila:** provjere validnosti operacija, statusne tranzicije, pravila dodjele ruta i obrade izuzetaka.
- **Orkestracija procesa:** koordinacija između API sloja, optimizacijskog modula i DAL-a.
- **Domain modeli i interfejsi:** C# klase i apstrakcije koje reprezentuju entitete domene - Korisnik, Sandučić, Ruta, StavkaRute, DnevniIzvjestaj.

### Optimizacijski modul

Modul prima listu sandučića s koordinatama, prioritetima i radnim pravilima, a vraća uređenu sekvencu obilaska (PBI-020, PBI-021, PBI-022).

MVP implementacija koristi nearest-neighbor heuristiku: polazi od depoa, iterativno bira najbliži neposjećeni sandučić uz uvažavanje prioriteta. Pristup je zadovoljavajući za realne veličine ruta (do ~50 sandučića).

Modul je izolovan kao zasebni C# servis (`IRouteOptimizationService`) s jasno definisanim interfejsom. Buduća zamjena algoritma ne zahtijeva izmjene u ostatku sistema.

### Data Access Layer (DAL) - Entity Framework Core

DAL je zadužen za pristup i perzistenciju podataka. EF Core mapira C# entitete na PostgreSQL tablice putem Code-First pristupa - shema baze se derivira iz C# klasa kroz migracije.

Unutar DAL-a nalaze se `DbContext`, repozitoriji i konfiguracija mapiranja, dok BLL koristi DAL preko jasno definisanih interfejsa i ne komunicira direktno s bazom.

Ključne tablice:

- **Users:** ID, ime, prezime, email, username, telefon, hash lozinke, uloga, status aktivnosti, flag prve prijave (`isForcePasswordChange`), datum kreiranja, vrijeme zadnje prijave.
- **Postboxes:** ID, adresa, geografske koordinate (latitude/longitude), tip, prioritet, kapacitet, radna pravila (radni dani, vremenski okvir dostupnosti), status objekta.
- **Routes:** ID, datum, ID dispečera, ID poštara, FK dnevnog izvještaja, status (Planirana / Aktivna / Završena / Prekinuta), razlog prekida, ukupna procijenjena distanca.
- **RouteItems:** ID, FK ruta, FK sandučić, planiran redoslijed, status obilaska (`Planirano` / `Realizovano` / `Preskočeno` / `Nedostupno`), tip realizacije (`Ispražnjen` / `Napunjen`, popunjeno samo kada je status `Realizovano`), timestamp potvrde, koordinate poštara u momentu potvrde (geo-validacija), napomena.
- **DailyReports:** ID, FK korisnika koji je pregledao izvještaj, datum, agregirani statistički podaci, procenat uspješnosti, lista incidenata.
- **AuditLog:** evidencija ključnih promjena podataka.

### Baza podataka - PostgreSQL

PostgreSQL je odabran zbog pouzdane podrške za geografske podatke (koordinate sandučića), ACID garancija i mogućnosti integracije PostGIS ekstenzije u budućim iteracijama.

Sve komponente pristupaju podacima isključivo kroz Data Access Layer - nikad direktno.

### Autentifikacijski podsistem - JWT + ASP.NET Identity

ASP.NET Identity upravlja kreiranjem korisnika i hashiranjem lozinki (bcrypt). Pri uspješnoj prijavi generira se JWT token koji klijent šalje u Authorization headeru svakog zahtjeva.

JWT payload sadrži korisnički ID, ulogu i expiry timestamp. Backend middleware validira token na svakom zaštićenom endpointu, a uloga iz tokena koristi se za RBAC provjere.

Pri prvoj prijavi, ako je postavljen `isForcePasswordChange` flag, API vraća HTTP 200 sa izdatim tokenom i posebnim poljem u response-u (npr. `requiresPasswordChange: true`) koje frontend tretira kao obaveznu navigaciju na ekran za promjenu lozinke prije pristupa Dashboard-u (US-06).

---

## Tok podataka i interakcija između komponenti

Sva komunikacija prolazi kroz REST API kao jedinu ulaznu tačku u backend. Frontend nikad ne komunicira direktno s bazom. Tipičan tok zahtjeva je:

**Frontend -> API Layer -> BLL -> DAL -> PostgreSQL**

Odgovor se vraća obrnutim smjerom, pri čemu svaki sloj zadržava svoju odgovornost i ne preskače naredni sloj.

### Tok autentifikacije

| **Korak** | **Akter** | **Akcija** | **Rezultat** |
|---|---|---|---|
| **1** | Korisnik | POST /api/auth/login { identifier, password } (konkretan identifikator — email ili username — utvrđuje se kroz OQ-001) | HTTP 200 + JWT token (+ `requiresPasswordChange` flag pri prvoj prijavi) |
| **2** | API Layer (Auth Controller) | Prima zahtjev, validira ulazne podatke i prosljeđuje ga BLL-u | Zahtjev pripremljen za obradu |
| **3** | BLL (Auth Service) | Validira kredencijale putem ASP.NET Identity i generiše JWT s role claim-om i expiry-jem | Provjera hash lozinke + potpisan token |
| **4** | Frontend | Pohranjuje JWT u memoriju sesije, dekodira ulogu | Prikazuje UI prema ulozi |
| **5** | Svaki naredni zahtjev | Šalje Authorization: Bearer \<token\> | API middleware validira token |

### Tok generisanja rute

| **Korak** | **Akter** | **Akcija** | **Rezultat** |
|---|---|---|---|
| **1** | Dispečer | POST /api/routes/generate { date, filters } | Pokretanje generisanja (bez dodjele poštara) |
| **2** | API Layer | Prima zahtjev i delegira obradu BLL-u | Pokrenut use-case generisanja rute |
| **3** | BLL (RouteService) | Dohvata aktivne sandučiće s prioritetima iz DAL-a | Lista sandučića s koordinatama |
| **4** | BLL (RouteOptimizationService) | Primjenjuje nearest-neighbor algoritam nad GPS koordinatama (Haversine udaljenost) uz poštivanje prioriteta i radnih pravila | Uređena lista redoslijeda |
| **5** | BLL + DAL | Kreira Route i RouteItem zapise u bazi sa statusom `Planirana` | HTTP 201 + routeId |
| **6** | Dispečer | Pregleda prijedlog, po potrebi ručno mijenja redoslijed (PUT /api/routes/{id}/reorder) | Ažuriran redoslijed stavki |
| **7** | Dispečer | PUT /api/routes/{id}/assign { postmanId } | Ruta dodijeljena poštaru (status ostaje `Planirana` do početka obilaska) |
| **8** | Poštar | GET /api/routes/my-today | Preuzima dnevnu rutu |

### Tok ažuriranja statusa na terenu

Svaki PUT zahtjev za ažuriranje statusa sandučića (PBI-027):

- Šalje se s JWT tokenom koji identificira poštara.
- API sloj validira autentifikaciju i prosljeđuje zahtjev BLL-u.
- BLL provjerava da sandučić pripada dodijeljenoj ruti tog poštara.
- DAL ažurira `RouteItem` zapis s novim statusom i serverskim timestampom.
- Dispečerski dashboard osvježava prikaz putem periodičnog pollinga (OQ-006).

---

## Ključne tehničke odluke

| **ID** | **Odluka** | **Obrazloženje** | **Kompromis** |
|---|---|---|---|
| **AD-001** | **Monolitna arhitektura umjesto mikroservisa** | Tim od 7 članova, rok od jednog semestra. Mikroservisi donose DevOps overhead koji nije opravdan u ovom kontekstu. | Sav kod u jednom deploymentu - vertikalno skaliranje kao jedina opcija. |
| **AD-002** | **React SPA umjesto server-side renderinga** | Tri korisničke uloge s bitno različitim UI potrebama. SPA omogućava jasnu separaciju frontend/backend odgovornosti i bolji UX bez page-reload-a. | SEO nije podržan - nije relevantno za interni operativni alat. |
| **AD-003** | **ASP.NET Core kao backend framework uz troslojnu organizaciju API/BLL/DAL** | Tim ima iskustvo s C# ekosistemom. ASP.NET Core nativno podržava JWT, Identity, EF Core i Swagger, dok podjela na API, BLL i DAL uvodi jasnu separaciju odgovornosti. | Nešto veća potrošnja memorije u odnosu na Node.js i dodatna potreba za disciplinom u odvajanju odgovornosti između slojeva. |
| **AD-004** | **PostgreSQL kao RDBMS** | Pouzdana podrška za geografske podatke i mogućnost PostGIS integracije. ACID garancije su kritične za integritet podataka o rutama. | Potreban hosted servis (npr. Supabase) za cloud deployment. |
| **AD-005** | **JWT tokeni za autentifikaciju** | Stateless pristup koji ne zahtijeva server-side session store. Pogodan za mobilni kontekst poštara i potencijalno horizontalno skaliranje. | Revokacija tokena nije trivijalna - rješava se kratkim expiry-jem i refresh token mehanizmom. |
| **AD-006** | **RBAC s tri uloge (Admin / Dispečer / Poštar)** | User stories definišu tri jasno odvojena konteksta s različitim privilegijama. Uloge su međusobno isključive. | Potreba za detaljnijim permisijama unutar uloge ostaje otvoreno pitanje (OQ-004). |
| **AD-007** | **REST API kao integracijski ugovor** | Jasna granica između frontenda i backenda omogućava paralelni razvoj. Swagger/OpenAPI dokumentacija se generira automatski. | GraphQL razmatran, odbačen zbog kompleksnosti i veličine tima. |
| **AD-008** | **React 18 + Vite + TypeScript umjesto CRA** | Vite nudi znatno brži dev server i HMR nego Create React App (koji je i zvanično u održavanju bez novih funkcionalnosti). TypeScript strict mode otkriva greške u kompajliranju i dijeli DTO tipove sa axios wrapperima. | Tim se mora držati TS discipline; `any` tipovi se zabranjuju u code review-u. |
| **AD-009** | **Feature-based umjesto type-based struktura foldera** | Tim od 7 članova paralelno radi na različitim use-caseovima (auth, routes, mailboxes). Feature folderi smanjuju merge konflikte i lakše izdvajanje modula u budućnosti. | Potrebna je disciplina da se dijeljeni kod izdvaja u `shared/`, a ne duplira kroz feature. |
| **AD-010** | **Tailwind CSS umjesto CSS-in-JS (styled-components)** | Utility-first pristup ubrzava prototipiranje, manji runtime overhead, bolji performanse na mobilnim uređajima (nema runtime CSS injection-a). | Markup postaje verbose; ublažava se kroz `@apply` direktive i shared komponente. |
| **AD-011** | **React Hook Form + Zod za forme i validaciju** | Hook Form minimizira re-render-e (nekontrolisani inputi), Zod omogućava dijeljenje validacije između frontend formi i (potencijalno) backend DTO-a. | Dodatna biblioteka u dependency stablu; opravdano kompleksnošću formi (korisnici, sandučići). |
| **AD-012** | **Context API + custom hooks za MVP (umjesto Redux)** | Za veličinu stanja u MVP-u (auth, notifikacije, globalne preference) Context API je dovoljan. Redux uvodi boilerplate koji se u ovoj fazi ne isplati. | Za server-state cache i složene sinhronizacije razmatra se TanStack Query u Sprintu 5 (OQ-009), a ne Redux. |
| **AD-013** | **Route-based code splitting po ulozi** | Mobilni korisnik (poštar) nikada ne treba admin kod. Razdvajanjem bundle-a inicijalni transfer na terenskom mobilnom internetu se smanjuje višestruko. | Kompleksnija build konfiguracija i mogućnost duplog koda u chunk-ovima; mitigacija kroz Vite-ov automatski deduping. |

---

## Ograničenja i rizici arhitekture

| **ID** | **Nivo** | **Rizik** | **Opis** | **Mitigacija** |
|---|---|---|---|---|
| **AR-001** | **Visok** | **Skalabilnost monolita** | Pri velikom broju simultanih korisnika i ruta, monolitna arhitektura može postati usko grlo. | MVP je interne namjene s ograničenim brojem korisnika. Refaktoring na modularne servise planiran kao post-semester aktivnost. |
| **AR-002** | **Visok** | **Kompleksnost optimizacijskog algoritma** | Nearest-neighbor heuristika daje suboptimalne rute za veće skupove sandučića. | Za MVP (<50 sandučića po ruti) heuristika je dovoljna. Algoritam je izoliran u zasebnom servisu - zamjena ne zahtijeva promjene u ostatku sistema. |
| **AR-003** | **Srednji** | **JWT token revokacija** | Stateless JWT tokeni ne mogu biti poništeni bez dodatne infrastrukture (blacklist ili token store). | Kratko trajanje tokena (15–30 min) + refresh token mehanizam. |
| **AR-004** | **Srednji** | **Integritet historijskih podataka** | Bez soft-delete mehanizma, brisanje rute ili sandučića narušava historijske zapise u izvještajima. | Sve tablice planiraju soft-delete flag. Arhiviranje umjesto permanentnog brisanja (PBI-049). |
| **AR-005** | **Nizak** | **Vendor lock-in (EF Core + PostgreSQL)** | Tjesna integracija ORM-a s PostgreSQL specifičnostima otežava migraciju na drugi RDBMS. | Apstrakcija kroz repozitorijum pattern. Promjena RDBMS-a nije realistična u ovom projektu. |
| **AR-006** | **Nizak** | **CORS konfiguracija** | Neispravna CORS politika može izložiti API neautoriziranim klijentima ili blokirati legitimne zahtjeve. | Eksplicitna CORS politika s whitelistom origin-a. HTTPS obavezno u produkciji. |
| **AR-007** | **Srednji** | **Gubitak JWT-a pri hard refresh-u** | Čuvanje tokena u memoriji/sessionStorage znači da hard refresh izbacuje korisnika na login ekran. | Uvođenje refresh-token flow-a (HttpOnly cookie) u Sprintu 6; za MVP — jasna UX poruka i brzi re-login. |
| **AR-008** | **Srednji** | **Bundle size za mobilni prikaz** | Mobilni poštari rade na slabijoj mreži — preveliki bundle usporava inicijalno učitavanje. | Route-based code splitting (AD-013), tree-shaking kroz Vite, performance budget ≤ 200 KB gzip za inicijalni chunk. |
| **AR-009** | **Srednji** | **Inkonzistentnost server state-a u Context-u** | Bez dedikovanog server-state rješenja (TanStack Query), cache invalidacija se piše ručno — sklono greškama. | Strikta konvencija `invalidate(...)` nakon mutacija; migracija na TanStack Query u Sprintu 5 (OQ-009). |
| **AR-010** | **Nizak** | **Inkompatibilnost starijih mobilnih browser-a** | Stariji Android telefoni u terenskim uslovima mogu imati zastarjele WebView-ove koji ne podržavaju sve ES modul feature-e. | Vite `build.target` postavljen na ES2020; polyfill-i kroz `@vitejs/plugin-legacy` ako testovi na terenu pokažu problem. |

### Inherentna ograničenja monolitne arhitekture

- **Nedjeljivost deployanja:** Svaka izmjena zahtijeva redeploy cijele aplikacije.
- **Tehnološka homogenost:** Sve backend komponente unutar API/BLL/DAL slojeva moraju koristiti isti jezik (C#) i runtime. Npr. implementacija optimizacijskog modula u Python-u nije moguća bez external service komunikacije.
- **Testna izolacija:** Integration testovi mogu postati kompleksni jer komponente dijele isti process, iako su logički odvojene na API, BLL i DAL.

---

## Otvorena pitanja

| **ID** | **Pitanje** | **Detalji** | **Ciljani sprint** | **Arhitektonski uticaj** |
|---|---|---|---|---|
| **OQ-001** | **Primarni identifikator za prijavu** | Email ili korisničko ime kao login kredencijal? | **Sprint 5** | Utiče na DB shemu, login formu i JWT claims. |
| **OQ-002** | **Generisanje inicijalne lozinke** | Ručni unos od strane admina ili automatsko generisanje s dostavom poštom? | **Sprint 5** | Utiče na UX kreiranja korisnika i potrebu za email servisom. |
| **OQ-003** | **Dostava kredencijala poštaru** | Kojim kanalom admin dostavlja inicijalne kredencijale? | **Sprint 5–6** | Može zahtijevati email servis (SendGrid/SMTP) kao novu infrastrukturnu komponentu. |
| **OQ-004** | **Granularnost RBAC-a** | Da li su potrebne prilagodljive permisije unutar uloge (npr. dispečer koji samo pregledava, ali ne i dodjeljuje rute)? | **Sprint 7** | Može komplicirati authorization middleware. |
| **OQ-005** | **Algoritam optimizacije za veće skupove** | Da li je OR-Tools ili sličan solver target za post-MVP fazu? | **Sprint 8** | Izolacija modula iza interfejsa (AD-001) omogućava zamjenu bez većeg refaktoringa. |
| **OQ-006** | **Offline podrška za poštara** | Šta se dešava pri gubitku konekcije na terenu - reconnect ili lokalni cache? | **Sprint 9** | Potencijalno uvođenje Service Worker-a ili PWA strategije. |
| **OQ-007** | **Format dnevnih izvještaja** | PDF export, slanje emailom ili prikaz unutar aplikacije? | **Sprint 9** | Utiče na izbor reporting biblioteke i potrebu za email servisom. |
| **OQ-008** | **Deployment target** | Azure, Supabase, Railway ili self-hosted? | **Sprint 4** | Potrebna odluka prije postavljanja tehničkog skeletona (PBI-038). |
| **OQ-009** | **Server state management na frontendu** | Context API + ručni refetch ili TanStack Query za cache, dedup i invalidaciju? | **Sprint 5** | Utiče na strukturu API sloja i način pisanja feature-a; migracija nije trivijalna kasnije. |
| **OQ-010** | **PWA strategija za poštarski prikaz** | Da li uvodimo service worker, install prompt i offline cache za mobilni prikaz u MVP-u ili post-MVP fazi? | **Sprint 6** | Zahtijeva odluku o offline podršci (OQ-006) i manifest konfiguraciju. |
| **OQ-011** | **Hosting statičkog frontend buildta** | Isti server kao backend (reverse proxy) ili izdvojeno (Vercel/Netlify/S3+CloudFront)? | **Sprint 4** | Utiče na CORS, deployment pipeline i CDN strategiju. |

### Polling vs. WebSocket za praćenje rute u realnom vremenu

Dispečer prati realizaciju rute u realnom vremenu (PBI-029). Razmatrana su dva pristupa:

| **Pristup** | **Prednosti** | **Mane** |
|---|---|---|
| **HTTP Polling** | Jednostavna implementacija, nema infrastrukturnih zahtjeva. | Kašnjenje jednako polling intervalu (10–30s), nepotreban promet pri neaktivnosti. |
| **WebSocket / SignalR** | Promptno ažuriranje, efikasnije pri većem broju korisnika. | Kompleksnija implementacija, potencijalni problemi s load balancerima. |

**Odluka za MVP:** HTTP polling s intervalom od 15–30 sekundi odabran je zbog jednostavnosti implementacije i ograničenog broja istovremenih korisnika u inicijalnoj fazi sistema.

SignalR (WebSocket-based pristup) je razmatran kao preferirano rješenje za real-time komunikaciju, ali je odgođen za post-MVP fazu kako bi se smanjila inicijalna kompleksnost i ubrzao razvoj.

Očekivani uticaj ove odluke je blago kašnjenje u prikazu statusa (do 30 sekundi), što je prihvatljivo u kontekstu internog operativnog alata. Migracija na SignalR ostaje planirana kao buduća optimizacija u slučaju povećanja broja korisnika ili zahtjeva za strožim real-time prikazom.

---

*Ovaj dokument se ažurira uz svaki sprint koji donosi arhitekturalnu promjenu ili razrješava otvoreno pitanje*
