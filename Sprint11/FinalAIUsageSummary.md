# Završni Sažetak Korištenja AI Alata

## Uvod

Tokom razvoja projekta PostRoute koristili smo Claude Code, GitHub Copilot, ChatGPT i Codex – svaki u kontekstu u kojem je bio najkorisniji. Nijedno rješenje nije kopirano bez pregleda. Tamo gdje je AI predlagao nešto što nije odgovaralo arhitekturi, odbacivali smo prijedlog ili ga prilagođavali. Tamo gdje je griješio, ispravljali smo i bilježili.

Ovaj dokument se oslanja na unose iz AI Usage Loga koji smo vodili sprint po sprint.

---

## 1. Za šta je AI korišten

### Sprint 5 – Postavljanje temelja

Na početku Sprinta 5 projekt je bio u skeleton fazi: frontend paketi nisu bili instalirani, `httpClient` stub je vraćao 501, BCrypt nije bio integrisan, a CORS politika nije bila konfigurirana. Claude Code je identifikovao sva četiri blokatora i pomogao u njihovom uklanjanju: instalacija paketa (Axios, React Router, React Hook Form, Zod, Sonner), postavljanje `httpClient`-a sa JWT interceptorom, integracija BCrypt.Net-Next i CORS konfiguracija u `Program.cs`. Copilot je riješio port konflikt s lokalnim PostgreSQL-om kreiranjem `docker-compose.dev.yml` sa PostgreSQL 16 na portu 5433.

Paralelno je Copilot implementovao lockout logiku (`LoginAsync` sa 5 pokušaja i HTTP 423), a ChatGPT je predložio strukturu login forme sa cooldown mehanizmom i osnovni tok za promjenu lozinke. Claude Code je zatim napravio sistematičnu provjeru svih deset user storija i pronašao pet propusta koje niko nije primjetio: `GetCurrentUser` hardkodirao `mustChangePassword: false`, `PrivateRoute` nije blokirao navigaciju za korisnike kojima je lozinka morala biti promijenjena, logout nije koristio `replace: true` i tako dalje. Sve je ispravljeno na zasebnoj grani `fix/PBI-012-013-auth-ispravke`.

Kroz Claude Code smo uradili i sigurnosne ispravke koje su bile kritične: `ChangePasswordAsync` bez provjere stare lozinke (svako ko zna email mogao je promijeniti lozinku), login pokušaji koji se nisu logirali u `SecurityLog`, i razdvajanje generičkog `InvalidOperationException` u `AccountLockedException` i `InvalidCredentialsException` kako bi se uklonio antipattern `ex.Message.Contains("locked")`.

### Sprintovi 6–7 – Sandučići

CRUD za sandučiće bio je najveći blok funkcionalnosti: GPS koordinate sa Leaflet mapom, tipovi, prioriteti, statusi, vremenska dostupnost, radni dani kao bitwise flags enum, serverska paginacija i filtriranje, `MailboxAuditLog` za svaku izmjenu. Sve je implementovao Claude Code, uz UI dotjerivanje i road-aware prikaz ruta koji je dodao GitHub Copilot.

### Sprintovi 8–9 – Rute i praćenje

Routing modul je bio najkompleksniji dio projekta. Claude Code je implementovao generisanje ruta (nearest-neighbor), working days filter, ručnu promjenu redoslijeda sa re-kalkulacijom vremenskih oznaka, dodjelu rute poštarima sa blokadom zauzetih poštara za isti datum, worker pregled dodijeljene rute, ažuriranje statusa sandučića i dashboard dispečera sa 30-sekundnim auto-refreshom. GitHub Copilot je dodao OSRM integraciju za road-aware prikaz na Leaflet mapi i sredil UI prikaze vremenskih podataka.

### Sprintovi 9–10 – Izvještaji i završnica

ChatGPT/Codex je implementovao dnevni izvještaj poštara i izvještaj o učinku (KPI tabela, procenat uspješnosti, CSS stubni grafikon). Claude Code je dodao izvještaj po tipu sandučića i implementovao brzу pretragu sandučića sa debounce-om. Svi CSV exporti su rađeni bez dodatnih biblioteka.

Testovi su pisani paralelno uz svaki PBI. Na kraju projekta: BLL 136/136, DAL 34/34, API 39/39 – svi prolaze.

---

## 2. Šta je prihvaćeno

Neke AI prijedloge prihvatili smo direktno, bez izmjena, jer su bili usklađeni sa arhitekturom i nisu uvodili nepotrebnu složenost:

* `docker-compose.dev.yml` sa PostgreSQL 16 na portu 5433
* Zod + React Hook Form za frontend validaciju
* Leaflet + OSRM za prikaz ruta na mapi
* Bitwise flags enum za `WorkingDays`
* `MailboxAuditLog` i `SecurityLog` za audit trail
* `PagedResponse<T>` wrapper za paginirane endpointe
* `ReorderRouteRequest` model za ručnu promjenu redoslijeda
* 30-sekundni auto-refresh dashboard
* CSV export bez dodatnih biblioteka

---

## 3. Šta je izmijenjeno

Većina AI koda zahtijevala je barem sitne intervencije. Neke su bile trivijalne (dupli `using`, pogrešan naziv metode), a neke su bile nužne korekcije stvarnih grešaka.

Najznačajnija izmjena bila je switch s Bearer tokena na session-based autentifikaciju tokom PBI-014. AI je inicijalno predložio JWT pristup, ali tim je odlučio koristiti sesiju jer ona nativno podržava `withCredentials` i cookie transport bez dodatnih CORS komplikacija. Iz iste odluke slijedio je refaktor `useAuth.ts` i `ChangePasswordPage`. Oboje su koristili native `fetch` s relativnim URL-om koji je Vite interceptovao umjesto backenda, što je prouzrokovalo `SyntaxError: Unexpected token '<'` jer je Vite vraćao HTML.

Druga važnija izmjena bila je EF migracija za `SecurityLog.UserId`: AI je generisao generički `AlterColumn`, ali PostgreSQL odbija konverziju `text → uuid` bez `USING` klauzule. Zamijenili smo s raw SQL-om: `ALTER TABLE ... ALTER COLUMN ... TYPE uuid USING NULLIF(...)::uuid`.

Working days filter u `RouteRepository` je AI ostavio s komentarom "bez aktivnih dana", bez stvarne implementacije. Dodan je `ToDayFlag()` helper i bitwise `Where` filter koji je bio neophodan da rute funkcionišu ispravno.

Tokom projekta modeli su se postupno proširivali: `User` je dobio `FailedAttempts`, `IsLockedOut`, `MustChangePassword`; `Route` je dobio `IsManuallyReordered`, `LastReorderedAt`, `LastReorderedBy`; `RouteItem` je dobio `Order`, `IsManuallyReordered`, `MailboxStatus`. Svaka od ovih promjena je bila odluka tima, a AI je implementovao na osnovu definisanih zahtjeva.

---

## 4. Šta je odbačeno

Nekoliko AI prijedloga odbacili smo jer nisu odgovarali MVP scopeu ili su uvodili nepotrebnu kompleksnost.

`react-leaflet` biblioteku smo odbacili u korist direktnog Leaflet API-ja. Za jednu mapu po stranici biblioteka je bila overhead bez benefita. SignalR za real-time notifikacije bio je logičan prijedlog, ali za MVP je 30-sekundni polling bio dovoljan i znatno jednostavniji za održavanje. Server-side PDF generisanje odbacili smo iz istog razloga. Browser print + CSV pokrili su izvještajne potrebe bez uvođenja nove biblioteke. Chart.js za grafikon učinka odbačen jer se isti vizuelni efekt mogao postići CSS-om.

TypeScript enume smo zamijenili s `const ... as const` jer su se enumi loše konvertirali s backend vrijednostima. Cancel route UI nije implementiran jer `Otkazana` status postoji kroz arhivu, a nijedan user story nije zahtijevao dedicirani cancel tok.

---

## 5. Greške koje je AI napravio

### Implementacijske greške

Prva greška koja nas je zaustavila u Sprintu 5 bila je `[property: Required]` prefiks u data annotations. ASP.NET Core 9 ga ignoriše, pa je API vraćao 500 bez jasne poruke. Odmah nakon toga, `CreatedAtAction` reflection nije mogao pronaći `Guid` parametar u ruti, što je izazvalo drugu 500 grešku. Obje su bile karakteristične greške AI alata koji nije vidio razlike između verzija frameworka.

U Sprintu 6 AI je generisao duplikat `MailboxLocationForm` komponente u dva različita foldera. Kad je `MailboxListPage` importala pogrešnu kopiju, `react-leaflet` koji nije ni bio instaliran uzrokovao je runtime crash. Obje kopije su obrisane.

U Sprintu 8 smo naišli na suptilniji problem: AI je kreirao novu `RouteDetailsPage` komponentu umjesto da poboljša već postojeću `GenerateRoutePage` koja je imala sve potrebne dijelove. Ovo se desilo jer AI nije imao cjelovit uvid u projekt. Nova stranica je obrisana, a funkcionalnost integrirana u postojeću.

Testovi su imali svojih specifičnih problema. `ShouldExcludeMailboxesOutsideTimeWindow` je prolazio iz pogrešnog razloga: working days filter je isključivao sandučić prije time window provjere, pa test nije zapravo validirao ono za što je bio napisan. Fiksiran je fiksnim datumom i `WorkingDays=SvakiDan`. U Sprintu 9, polje `mailboxStatus` dodano je u model kroz PBI-027, ali AI nije ažurirao mockove u testovima iz PBI-023. Ručno je dodano. Audit repo mock je koristio `AddAsync` umjesto `LogAsync`. `DefaultHttpContext` bez konfigurisane sesije pucao je pri pristupu `.Session`, pa je zahtijevalo kreiranje `TestSession : ISession` implementacije.

### Sigurnosni propusti

Dva propusta su bila sigurnosne prirode. `ChangePasswordAsync` je inicijalno bio implementovan bez provjere stare lozinke, što bi praktično značilo da ko god zna email može promijeniti lozinku. Dodana je BCrypt verifikacija. Login pokušaji se nisu logirali u `SecurityLog`, pa nije bilo audit traga za neuspjele pokušaje. Dodana je `LogLoginAttemptAsync` metoda koja pokriva sva tri scenarija.

Treći incident bio je drugačije prirode: Faruk je tokom push faze slučajno ostavio GitHub PAT token u chat razgovoru. Token je odmah rotiran. `appsettings.Development.json` je već bio u `.gitignore`, ali je fajl bio commitovan ranije. Proces `git add` je od tada oprezniji.

---

## 6. Šta treba posebno razumjeti

Svi članovi tima trebaju moći objasniti dijelove sistema koji su razvijeni uz značajnu AI pomoć, bez gledanja u kod.

**Autentifikacija i sigurnost** – Cijeli auth stack je AI-generisan uz ručni review. Session timeout koristi IIS default od 20 minuta i nije eksplicitno konfiguriran. `SecurityLog` nema automatski cleanup i tabela će s vremenom rasti. Cookie-based auth funkcioniše ispravno samo uz HTTPS.

**Routing engine** – `GenerateRouteAsync` koristi nearest-neighbor algoritam koji nije optimalan za veće skupove podataka. Za 100+ sandučića performanse mogu biti spore. OSRM javni endpoint nema fallback. Ako je nedostupan, mapa ne prikazuje rutu. Ručna promjena redoslijeda ne reoptimizira putanju, samo je rekordinira.

**Sandučići – validacija i audit** – `WorkingDays` bitwise flags enum s XOR logikom za 7 checkboxa je jedinstven dio sistema koji zahtijeva pažnju. Validacija "barem jedan dan" nije enforced na nivou baze, samo u kodu. `MailboxAuditLog` nema retention policy. GPS preciznost od 6 decimalnih mjesta je dovoljna za testno okruženje.

**Dashboard dispečera** – 30-sekundna latency znači da promjena statusa nije vidljiva odmah. Nije testirano sa 100+ ruta na sporijoj mreži.

**Statusi sandučića** – Reverse transitions (npr. Ispraznjen → Napunjen) nisu eksplicitno blokirane u kodu. Promjene `RouteItem.Status` nisu u posebnom audit logu. `MailboxAuditLog` pokriva samo polja sandučića, ne stavke rute.

---

## 7. Kritički osvrt

AI je bio najkorisniji tamo gdje su zahtjevi bili precizno definirani. Kad je user story imao jasne acceptance criteria, generisani kod je bio dobar polazišni punkt: čitljiv, usklađen s patternima koje smo već uspostavili, bez nepotrebnih apstrakcija. To se posebno osjetilo kod pisanja testova: AI je brzo generisao skeleton sa happy path i edge case scenarijima, a tim je samo provjeravao da li mock podaci odgovaraju stvarnoj logici.

S druge strane, AI konzistentno nije vidio cijeli projekt. Rezultat toga bili su duplikati komponenti i polja koja su dodana u model ali ne i u testove koji su već postojali. Svaki put kad je AI mijenjao nešto što je imalo cross-layer zavisnosti, trebalo je ručno proći kroz sve zahvaćene fajlove. Arhitekturalne odluke, kao što su session vs. JWT, polling vs. WebSocket i library vs. native API, AI nije mogao donijeti. Te odluke je tim donosio samostalno, a AI bi implementovao što je odlučeno.

Jedan obrazac koji smo prepoznali: greške su bile učestalije u Sprintu 5 (data annotations, `CreatedAtAction`, cross-PBI auth propusti) i postepeno su se smanjivale kako je projekt napredovao. Do Sprinta 9 i 10 AI je uglavnom radio na testovima i dokumentaciji, a implementacijske greške bile su rijetke.

Kad bismo ponovo radili projekat ovakve veličine, pisali bismo testove prije implementacije umjesto paralelno. To bi AI-ju dalo jasniji kontekst o intenciji koda. Pored toga, eksplicitna analiza cijelog projekta prije svake veće implementacije smanjila bi broj redundantnih komponenti.

---

## 8. Zaključak

Koristili smo AI alate kao alate, ne kao autoritet. Svaka greška je dokumentovana i ispravljena. Svaka odbačena odluka je obrazložena. Testovi pokrivaju realne scenarije, ne samo prolaze kroz code review.

Kod koji je AI generisao je stabilan i testiran. Ipak, ko god bude održavao sistem trebao bi biti svjestan ograničenja navedenih u Sekciji 6, posebno u pogledu rasta audit tabela, OSRM dostupnosti i odsustva eksplicitne state machine validacije za statusne prijelaze sandučića.
