# Architecture Overview


## Kratak opis arhitektonskog pristupa

Sistem se implementira kao višeslojna (N-Tier) monolitna web aplikacija s jasno razdvojenim klijentskim i serverskim dijelom - klijent-server arhitektura.

Frontend je React SPA koji komunicira s backendom isključivo putem REST API-ja. Backend je ASP.NET Core Web API koji sadrži cjelokupnu poslovnu logiku: upravljanje korisnicima, rutama, sandučićima i optimizacijski algoritam. Podaci se čuvaju u PostgreSQL bazi podataka, a pristup je posredovan kroz Entity Framework Core ORM.

Monolitna arhitektura odabrana je zbog veličine tima, vremenskog okvira od jednog semestra i potrebe za brzim iteracijama. Mikroservisi su razmatrani, ali odbačeni - overhead u DevOps-u i mrežnom sloju nije opravdan za ovaj opseg projekta.

| **Arhitekturni stil** | **Primarni stack** | **Deployment model** |
|---|---|---|
| Monolitna, klijent-server, N-Tier | React (SPA) + ASP.NET Core Web API + PostgreSQL | Single-instance server (cloud hosted), responsive web klijent |

---

## Glavne komponente sistema

Sistem se sastoji od šest komponenti organizovanih u tri logička sloja.

| **Komponenta** | **Tehnologija** | **Odgovornost** | **Status** |
|---|---|---|---|
| **Presentation Layer (Frontend)** | React + Vite SPA | Administratorski panel, dispečerski dashboard, mobilni prikaz za poštara. Komunicira isključivo putem REST API-ja. | Arhitekturno stabilno |
| **Application Layer (Backend)** | ASP.NET Core Web API | Centralna tačka poslovne logike: upravljanje korisnicima, rutama, sandučićima. Izlaže REST endpointe koje konzumira frontend. | U razvoju |
| **Optimizacijski modul** | C# servis (interni) | Generiše optimalnu dnevnu rutu na osnovu lokacija, prioriteta i radnih pravila sandučića (nearest-neighbor heuristika u MVP-u). | U razvoju |
| **Data Access Layer** | Entity Framework Core | ORM sloj koji enkapsulira sve upite prema bazi. Definira entitetske modele i migracije. | U razvoju |
| **Baza podataka** | PostgreSQL | Relacijska baza: korisnici, sandučići, rute, statusi obilazaka, historija. Jedini izvor istine za sve podatke sistema. | U razvoju |
| **Autentifikacijski podsistem** | JWT + ASP.NET Identity | Prijava korisnika, hashiranje lozinki (bcrypt), generisanje JWT tokena, RBAC (tri uloge: Admin, Dispečer, Poštar). | Kritična — zahtijeva posebnu pažnju |

---

## Odgovornosti komponenti

### Presentation Layer - React SPA

Frontend pruža tri funkcionalno odvojena pogleda, određena ulogom prijavljenog korisnika:

- **Administratorski panel:** upravljanje korisničkim računima (PBI-011 do PBI-014), evidencija sandučića (PBI-017 do PBI-019), pretraga i filtriranje (PBI-051).
- **Dispečerski dashboard:** generisanje i dodjela ruta (PBI-022 do PBI-025), praćenje realizacije (PBI-029), pristup izvještajima (PBI-030, PBI-049, PBI-050).
- **Mobilni prikaz za poštara:** pregled dodijeljene rute (PBI-026), ažuriranje statusa sandučića (PBI-027), evidencija nedostupnih lokacija (PBI-028).

JWT token se čuva u memoriji sesije (ne u localStorage) kako bi se smanjio rizik od XSS napada. Ovaj pristup ima kompromis u vidu gubitka autentifikacijske sesije pri osvježavanju stranice (refresh).

U produkcijskom okruženju razmatra se korištenje HttpOnly cookies za pohranu tokena, uz dodatnu CSRF zaštitu, čime bi se postigla bolja ravnoteža između sigurnosti i korisničkog iskustva.

### Application Layer - ASP.NET Core Web API

Backend je organizovan u četiri interna sloja:

- **Controllers:** HTTP endpointi i validacija zahtjeva. Bez poslovne logike.
- **Services:** Sva poslovna logika - upravljanje rutama, provjere validnosti operacija, pozivanje optimizacijskog modula.
- **Repositories:** Enkapsulacija EF Core upita. Services ne komuniciraju direktno s DbContext-om.
- **Domain Models:** C# klase koje reprezentuju entitete domene - Korisnik, Sandučić, Ruta, ObilazakSandučića.

### Optimizacijski modul

Modul prima listu sandučića s koordinatama, prioritetima i radnim pravilima, a vraća uređenu sekvencu obilaska (PBI-020, PBI-021, PBI-022).

MVP implementacija koristi nearest-neighbor heuristiku: polazi od depoa, iterativno bira najbliži neposjećeni sandučić uz uvažavanje prioriteta. Pristup je zadovoljavajući za realne veličine ruta (do ~50 sandučića).

Modul je izolovan kao zasebni C# servis (`IRouteOptimizationService`) s jasno definisanim interfejsom. Buduća zamjena algoritma ne zahtijeva izmjene u ostatku sistema.

### Data Access Layer - Entity Framework Core

EF Core mapira C# entitete na PostgreSQL tablice putem Code-First pristupa - shema baze se derivira iz C# klasa kroz migracije.

Ključne tablice:

- **Users:** ID, ime, prezime, email, hash lozinke, uloga, status aktivnosti, datum kreiranja.
- **Postboxes:** ID, adresa, geografske koordinate (latitude/longitude), tip, prioritet, radna pravila, status.
- **Routes:** ID, datum, ID dispečera, ID poštara, status (generisana / dodijeljena / realizirana / arhivirana).
- **RouteStops:** ID, FK ruta, FK sandučić, planiran redoslijed, stvarni status (ispražnjen / napunjen / nedostupan / preskočen), timestamp, napomena.
- **AuditLog:** evidencija ključnih promjena podataka.

### Baza podataka - PostgreSQL

PostgreSQL je odabran zbog pouzdane podrške za geografske podatke (koordinate sandučića), ACID garancija i mogućnosti integracije PostGIS ekstenzije u budućim iteracijama.

Sve komponente pristupaju podacima isključivo kroz Data Access Layer - nikad direktno.

### Autentifikacijski podsistem - JWT + ASP.NET Identity

ASP.NET Identity upravlja kreiranjem korisnika i hashiranjem lozinki (bcrypt). Pri uspješnoj prijavi generira se JWT token koji klijent šalje u Authorization headeru svakog zahtjeva.

JWT payload sadrži korisnički ID, ulogu i expiry timestamp. Backend middleware validira token na svakom zaštićenom endpointu, a uloga iz tokena koristi se za RBAC provjere.

Pri prvoj prijavi, ako je postavljen `isForcePasswordChange` flag, API vraća HTTP 403 s opisnim kodom koji frontend tretira kao redirect na ekran za promjenu lozinke.

---

## Tok podataka i interakcija između komponenti

Sva komunikacija prolazi kroz REST API kao jedinu ulaznu tačku u backend. Frontend nikad ne komunicira direktno s bazom.

### Tok autentifikacije

| **Korak** | **Akter** | **Akcija** | **Rezultat** |
|---|---|---|---|
| **1** | Korisnik | POST /api/auth/login { email, password } | HTTP 200 + JWT token |
| **2** | Auth Controller | Validira kredencijale putem ASP.NET Identity | Provjera hash lozinke |
| **3** | JWT Service | Generiše JWT s role claim-om i expiry-jem | Token potpisan tajnim ključem |
| **4** | Frontend | Pohranjuje JWT u memoriju sesije, dekodira ulogu | Prikazuje UI prema ulozi |
| **5** | Svaki naredni zahtjev | Šalje Authorization: Bearer \<token\> | Middleware validira token |

### Tok generisanja rute

| **Korak** | **Akter** | **Akcija** | **Rezultat** |
|---|---|---|---|
| **1** | Dispečer | POST /api/routes/generate { postmanId, date, filters } | Pokretanje generisanja |
| **2** | RouteService | Dohvata aktivne sandučiće s prioritetima iz repozitorija | Lista sandučića s koordinatama |
| **3** | RouteOptimizationService | Primjenjuje nearest-neighbor algoritam | Uređena lista redoslijeda |
| **4** | RouteService | Kreira Route i RouteStop zapise u bazi | HTTP 201 + routeId |
| **5** | Dispečer | PUT /api/routes/{id}/assign { postmanId } | Ruta dodijeljena poštaru |
| **6** | Poštar | GET /api/routes/my-today | Preuzima dnevnu rutu |

### Tok ažuriranja statusa na terenu

Svaki PUT zahtjev za ažuriranje statusa sandučića (PBI-027):

- Šalje se s JWT tokenom koji identificira poštara.
- Backend validira da sandučić pripada dodijelenoj ruti tog poštara.
- RouteStop zapis se ažurira s novim statusom i serverskim timestampom.
- Dispečerski dashboard osvježava prikaz putem periodičnog pollinga (OQ-006).

---

## Ključne tehničke odluke

| **ID** | **Odluka** | **Obrazloženje** | **Kompromis** |
|---|---|---|---|
| **AD-001** | **Monolitna arhitektura umjesto mikroservisa** | Tim od 7 članova, rok od jednog semestra. Mikroservisi donose DevOps overhead koji nije opravdan u ovom kontekstu. | Sav kod u jednom deploymentu - vertikalno skaliranje kao jedina opcija. |
| **AD-002** | **React SPA umjesto server-side renderinga** | Tri korisničke uloge s bitno različitim UI potrebama. SPA omogućava jasnu separaciju frontend/backend odgovornosti i bolji UX bez page-reload-a. | SEO nije podržan - nije relevantno za interni operativni alat. |
| **AD-003** | **ASP.NET Core kao backend framework** | Tim ima iskustvo s C# ekosistemom. ASP.NET Core nativno podržava JWT, Identity, EF Core i Swagger, što smanjuje količinu repetitivnog koda. | Nešto veća potrošnja memorije u odnosu na Node.js. |
| **AD-004** | **PostgreSQL kao RDBMS** | Pouzdana podrška za geografske podatke i mogućnost PostGIS integracije. ACID garancije su kritične za integritet podataka o rutama. | Potreban hosted servis (npr. Supabase) za cloud deployment. |
| **AD-005** | **JWT tokeni za autentifikaciju** | Stateless pristup koji ne zahtijeva server-side session store. Pogodan za mobilni kontekst poštara i potencijalno horizontalno skaliranje. | Revokacija tokena nije trivijalna - rješava se kratkim expiry-jem i refresh token mehanizmom. |
| **AD-006** | **RBAC s tri uloge (Admin / Dispečer / Poštar)** | User stories definišu tri jasno odvojena konteksta s različitim privilegijama. Uloge su međusobno isključive. | Potreba za detaljnijim permisijama unutar uloge ostaje otvoreno pitanje (OQ-004). |
| **AD-007** | **REST API kao integracijski ugovor** | Jasna granica između frontenda i backenda omogućava paralelni razvoj. Swagger/OpenAPI dokumentacija se generira automatski. | GraphQL razmatran, odbačen zbog kompleksnosti i veličine tima. |

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

### Inherentna ograničenja monolitne arhitekture

- **Nedjeljivost deployanja:** Svaka izmjena zahtijeva redeploy cijele aplikacije.
- **Tehnološka homogenost:** Sve komponente moraju koristiti isti jezik (C#) i runtime. Npr. implementacija optimizacijskog modula u Python-u nije moguća bez external service komunikacije.
- **Testna izolacija:** Integration testovi mogu postati kompleksni jer komponente dijele isti process i DbContext instancu.

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
