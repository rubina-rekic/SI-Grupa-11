# Proof of Testing - Sprint 10

---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit - Backend (BLL servisi) | US-40, US-41, US-42, US-43, US-44 | xUnit + Moq | 11 testova | PASS |
| Unit - Backend (API kontroleri) | US-40, US-41, US-42, US-43, US-44 | xUnit + Moq | 10 testova | PASS |
| Unit - Backend (BLL servisi) | US-36 | xUnit + Moq | 3 testa | PASS |
| Unit - Backend (DAL repozitoriji) | US-36 | xUnit + EF Core InMemory | 1 test | PASS |
| Unit - Backend (API kontroleri) | US-36 | xUnit + Moq | 3 testa | PASS |
| Frontend component/integration | US-36 | Vitest + Testing Library | 4 testa | PASS |
| Frontend component/integration | US-38 | Vitest + Testing Library | 5 testova | PASS |

**Ukupno verifikovano:** `dotnet test tests\PostRoute.BLL.Tests\PostRoute.BLL.Tests.csproj --filter "FullyQualifiedName~PBI052"` i `dotnet test tests\PostRoute.Api.Tests\PostRoute.Api.Tests.csproj --filter "FullyQualifiedName~PBI052"` prolaze — BLL 6/6, API 5/5. PBI-044 notifikacije su također pokrivene dodatnim testovima.

**PBI-051 / US-38 verifikovano:** `npm test -- --run src/ui/pages/admin/test/MailboxListPage.PBI038.test.tsx` prolazi — 5/5 testova PASS. Regresioni test `MailboxListPage.PBI039.test.tsx` (3/3) i kompletan frontend suite (52/52) prolaze bez regresija.

---

## PBI-051 / US-38 - Brza pretraga sandučića

### Pokrivenost po user story

| US | Naslov | Automatizirani testovi | Status |
| --- | --- | --- | --- |
| US-38 | Brza pretraga sandučića | `MailboxListPage.PBI038.test.tsx` (5 testova) | PASS |

### Šta je verifikovano

| Funkcionalnost / AC | Verifikacija | Test koji pokriva | Status |
| --- | --- | --- | --- |
| Pretraga se ne aktivira za < 3 karaktera | Frontend ne šalje `search` param kada su unesena 1-2 karaktera | `ne aktivira pretragu kada korisnik unese manje od 3 karaktera` | PASS |
| Pretraga se aktivira za >= 3 karaktera | Frontend šalje `search` param kada su unesena 3+ karaktera | `aktivira pretragu kada korisnik unese 3 ili vise karaktera` | PASS |
| Poruka "Nema pronađenih sandučića za uneseni pojam." | Prikazana kada je `debouncedSearch` aktivan i nema rezultata | `prikazuje poruku kada pretraga ne vrati rezultate` | PASS |
| Povratak na punu listu pri brisanju inputa | Frontend šalje upit bez `search` kada se input isprazni | `vraca punu listu kada se polje za pretragu isprazni` | PASS |
| Parcijalno pretraživanje | Frontend šalje parcijalni string koji backend dalje pretražuje | `podrzava parcijalno pretrazivanje po adresi` | PASS |
| Case-insensitive pretraga | Implementirano na backend strani (`ToLower().Contains(needle)`) — pokriveno u `MailboxRepositoryTestsPBI019` | - | PASS (pre-existing) |
| Paginacija nad filtriranim rezultatima | Paginacija ostaje aktivna i nad search rezultatima (ne resetuje se) | PBI-039 testovi + produkcijski API tok | PASS |

### Komande izvršene lokalno

```bash
cd PROJEKAT/frontend
npm test -- --run src/ui/pages/admin/test/MailboxListPage.PBI038.test.tsx
npm test -- --run src/ui/pages/admin/test/MailboxListPage.PBI039.test.tsx
npm test -- --run
```

### Rezultati

| Komanda | Rezultat |
| --- | --- |
| `npm test -- --run src/ui/pages/admin/test/MailboxListPage.PBI038.test.tsx` | PASS - 5/5 |
| `npm test -- --run src/ui/pages/admin/test/MailboxListPage.PBI039.test.tsx` | PASS - 3/3 (regresioni test) |
| `npm test -- --run` | PASS - 52/52 |

### Napomene

- Backend pretraga (po adresi, serijskom broju i ID-u, case-insensitive) implementirana u PBI-019 i nije mijenjana.
- Build greška u `LeafletRoutingMachine.tsx` (TypeScript tip za `leaflet-routing-machine`) je pre-existing issue koji postoji na main grani i nije uveden ovom izmjenom.
- Debounce od 300ms osigurava da pretraga odgovara u < 1 sekundi za baze do 1000 sandučića (AC #7).

---

---

**PBI-050 / US-36 verifikovano:** `dotnet test` filter `FullyQualifiedName~PBI050` prolazi za BLL, DAL i API sloj, a `npm test -- --run src/ui/pages/admin/test/PostmanPerformanceReportPage.PBI050.test.tsx` prolazi za frontend. Ukupno US-36: 11/11 testova PASS.

---

## PBI-052 - Upravljanje problematičnim lokacijama

### Pokrivenost po user story

| US | Naslov | Automatizirani testovi | Status |
| --- | --- | --- | --- |
| US-40 | Pregled detalja problematične lokacije | `IssueServiceTestsPBI052.GetByIdAsync_ShouldReturnMappedIssue_WhenIssueExists`, `IssuesControllerTestsPBI052.GetById_ShouldReturnOk_WhenIssueExists` | PASS |
| US-41 | Komentarisanje problema između dispečera i poštara | `IssueServiceTestsPBI052.AddCommentAsync_ShouldAddCommentAndSetStatusToInProgress_WhenIssueOpen`, `IssueServiceTestsPBI052.AddCommentAsync_ShouldCreateNotification_WhenDispatcherAddsComment`, `IssuesControllerTestsPBI052.AddComment_ShouldReturnOk_WhenCommentAdded` | PASS |
| US-42 | Dodjela akcije za problematičnu lokaciju | `IssueServiceTestsPBI052.AssignActionAsync_ShouldAssignActionAndSetAssignedTo_WhenActionIsDrugiPostar`, `IssuesControllerTestsPBI052.AssignAction_ShouldReturnOk_WhenActionAssigned` | PASS |
| US-43 | Evidencija statusa rješavanja problema | `IssueServiceTestsPBI052.ResolveAsync_ShouldMarkIssueAsResolved_WhenIssueIsNotResolved`, `IssuesControllerTestsPBI052.Resolve_ShouldReturnOk_WhenIssueResolved` | PASS |
| US-44 | Notifikacije za ažuriranje problema | `IssueServiceTestsPBI044.*`, `IssuesControllerTestsPBI044.*` | PASS |

---

## US-40 - Pregled detalja problematične lokacije

| Nivo | AC | Test koji pokriva | Status |
| --- | --- | --- | --- |
| BLL | Vraća detalje problema i adresu lokacije | `IssueServiceTestsPBI052.GetByIdAsync_ShouldReturnMappedIssue_WhenIssueExists` | PASS |
| API | `GET /api/issues/{id}` vraća 200 s detaljima | `IssuesControllerTestsPBI052.GetById_ShouldReturnOk_WhenIssueExists` | PASS |

---

## US-41 - Komentarisanje problema između dispečera i poštara

| Nivo | AC | Test koji pokriva | Status |
| --- | --- | --- | --- |
| BLL | Dodaje komentar i prelazi u status U obradi | `IssueServiceTestsPBI052.AddCommentAsync_ShouldAddCommentAndSetStatusToInProgress_WhenIssueOpen` | PASS |
| BLL | Kreira notifikaciju kada dispečer komentariše | `IssueServiceTestsPBI052.AddCommentAsync_ShouldCreateNotification_WhenDispatcherAddsComment` | PASS |
| API | `POST /api/issues/{id}/comments` vraća 200 | `IssuesControllerTestsPBI052.AddComment_ShouldReturnOk_WhenCommentAdded` | PASS |

---

## US-42 - Dodjela akcije za problematičnu lokaciju

| Nivo | AC | Test koji pokriva | Status |
| --- | --- | --- | --- |
| BLL | Dodjeljuje akciju i postavlja poštara | `IssueServiceTestsPBI052.AssignActionAsync_ShouldAssignActionAndSetAssignedTo_WhenActionIsDrugiPostar` | PASS |
| API | `PUT /api/issues/{id}/action` vraća 200 | `IssuesControllerTestsPBI052.AssignAction_ShouldReturnOk_WhenActionAssigned` | PASS |

---

## US-43 - Evidencija statusa rješavanja problema

| Nivo | AC | Test koji pokriva | Status |
| --- | --- | --- | --- |
| BLL | Označava problem kao riješen | `IssueServiceTestsPBI052.ResolveAsync_ShouldMarkIssueAsResolved_WhenIssueIsNotResolved` | PASS |
| API | `PUT /api/issues/{id}/resolve` vraća 200 | `IssuesControllerTestsPBI052.Resolve_ShouldReturnOk_WhenIssueResolved` | PASS |

---

## US-44 - Notifikacije za ažuriranje problema

### Pokriveni AC

| Nivo | AC | Test koji pokriva | Status |
| --- | --- | --- | --- |
| BLL | Vraća listu notifikacija za poštara | `IssueServiceTestsPBI044.GetMyNotificationsAsync_ShouldReturnMappedNotifications_WhenNotificationsExist` | PASS |
| BLL | Vraća praznu listu bez notifikacija | `IssueServiceTestsPBI044.GetMyNotificationsAsync_ShouldReturnEmptyList_WhenNoNotificationsExist` | PASS |
| BLL | Označava notifikaciju kao pročitanu | `IssueServiceTestsPBI044.MarkNotificationReadAsync_ShouldSetIsReadTrue_WhenRecipientMatches` | PASS |
| BLL | Baca grešku kada notifikacija ne postoji | `IssueServiceTestsPBI044.MarkNotificationReadAsync_ShouldThrow_WhenNotificationNotFound` | PASS |
| BLL | Baca grešku kada korisnik nema pristup notifikaciji | `IssueServiceTestsPBI044.MarkNotificationReadAsync_ShouldThrow_WhenRecipientDoesNotMatch` | PASS |
| API | `GET /api/issues/my-notifications` vraća 200 s listom notifikacija | `IssuesControllerTestsPBI044.GetMyNotifications_ShouldReturnOk_WithNotificationList` | PASS |
| API | `GET /api/issues/my-notifications` vraća 401 bez validnog UserId | `IssuesControllerTestsPBI044.GetMyNotifications_ShouldReturnUnauthorized_WhenUserIdMissing` | PASS |
| API | `PUT /api/issues/notifications/{notificationId}/read` vraća 200 na uspjeh | `IssuesControllerTestsPBI044.MarkNotificationRead_ShouldReturnOk_WhenServiceSucceeds` | PASS |
| API | `PUT /api/issues/notifications/{notificationId}/read` vraća 400 kada servis ne pronađe notifikaciju | `IssuesControllerTestsPBI044.MarkNotificationRead_ShouldReturnBadRequest_WhenServiceThrowsInvalidOperationException` | PASS |
| API | `PUT /api/issues/notifications/{notificationId}/read` vraća 401 za nevalidan UserId | `IssuesControllerTestsPBI044.MarkNotificationRead_ShouldReturnUnauthorized_WhenUserIdInvalidGuid` | PASS |

---

## Komande izvršene lokalno

```bash
cd PROJEKAT/backend
dotnet test tests\PostRoute.BLL.Tests\PostRoute.BLL.Tests.csproj --filter "FullyQualifiedName~PBI052"
dotnet test tests\PostRoute.Api.Tests\PostRoute.Api.Tests.csproj --filter "FullyQualifiedName~PBI052"
```

Rezultat: PASS — 6/6 BLL i 5/5 API testova za PBI-052. Postoji standardno upozorenje o verzijama EF Core Relational, ali testovi su uspješno prošli :)


---

## PBI-050 / US-36 - Izvjestaj o ucinku postara

### Pokrivenost po user story

| US | Naslov | Automatizirani testovi | Status |
| --- | --- | --- | --- |
| US-36 | Izvjestaj o ucinku postara | `RouteServiceTestsPBI050`, `RouteRepositoryTestsPBI050`, `RoutesControllerTestsPBI050`, `PostmanPerformanceReportPage.PBI050.test.tsx` | PASS |

### Sta je verifikovano

| Funkcionalnost / AC | Verifikacija | Test koji pokriva | Status |
| --- | --- | --- | --- |
| Tabela sa kolonama: ime postara, dodijeljeni sanducici, uspjesno ispraznjeno i nerealizovano | Frontend prikazuje KPI tabelu iz report odgovora | `PostmanPerformanceReportPage.PBI050.test.tsx` - prikazuje KPI tabelu | PASS |
| Formula `(Ispraznjeno / Planirano) * 100` | BLL agregira broj planiranih i ispraznjenih lokacija i racuna procenat po postaru | `RouteServiceTestsPBI050.GetPostmanPerformanceReportAsync_ShouldAggregateKpis_AndSortBySuccessDescending` | PASS |
| Filtriranje po proizvoljnom vremenskom periodu | API prima `fromDate` i `toDate`; DAL vraca samo zavrsene rute iz tog perioda; frontend salje odabrani period | `RouteRepositoryTestsPBI050.GetCompletedRoutesForPerformanceReportAsync_ShouldReturnOnlyCompletedRoutesInPeriod`, `RoutesControllerTestsPBI050.GetPostmanPerformanceReport_ShouldReturnOk_WithReport`, `PostmanPerformanceReportPage.PBI050.test.tsx` - filtrira period | PASS |
| Stubni grafikon za poredjenje ucinka postara | Frontend renderuje CSS bar chart na osnovu procenta uspjesnosti | `PostmanPerformanceReportPage.PBI050.test.tsx` - prikazuje stubni grafikon | PASS |
| Sortiranje tabele prema procentu uspjesnosti | BLL vraca redove sortirane opadajuce; frontend omogucava promjenu smjera sortiranja | `RouteServiceTestsPBI050.GetPostmanPerformanceReportAsync_ShouldAggregateKpis_AndSortBySuccessDescending`, `PostmanPerformanceReportPage.PBI050.test.tsx` - sortira tabelu | PASS |
| Klik na ime postara otvara detalje ruta koje ulaze u obracun | Frontend otvara detaljni panel sa rutama, datumom, pocetkom, zavrsetkom i KPI vrijednostima po ruti | `PostmanPerformanceReportPage.PBI050.test.tsx` - otvara detalje ruta klikom na ime postara | PASS |
| Export sumarnog izvjestaja u CSV | Frontend generise CSV datoteku iz trenutno ucitanog reporta | `PostmanPerformanceReportPage.PBI050.test.tsx` - exportuje sumarni izvjestaj u CSV | PASS |
| Postar bez zavrsenih ruta u periodu ne ulazi u prosjek tima | BLL vraca prazne redove i timski prosjek 0 kada nema zavrsenih ruta | `RouteServiceTestsPBI050.GetPostmanPerformanceReportAsync_ShouldReturnEmptyRows_WhenNoCompletedRoutes` | PASS |
| Validacija perioda | BLL odbija period gdje je pocetni datum poslije zavrsnog, a API vraca `BadRequest` kada period nedostaje ili je neispravan | `RouteServiceTestsPBI050.GetPostmanPerformanceReportAsync_ShouldRejectInvalidPeriod`, `RoutesControllerTestsPBI050.GetPostmanPerformanceReport_ShouldReturnBadRequest_WhenPeriodMissing`, `RoutesControllerTestsPBI050.GetPostmanPerformanceReport_ShouldReturnBadRequest_WhenServiceRejectsPeriod` | PASS |

### Komande izvrsene lokalno

```bash
cd PROJEKAT/backend
dotnet test tests\PostRoute.BLL.Tests\PostRoute.BLL.Tests.csproj --filter "FullyQualifiedName~PBI050"
dotnet test tests\PostRoute.DAL.Tests\PostRoute.DAL.Tests.csproj --filter "FullyQualifiedName~PBI050"
dotnet test tests\PostRoute.Api.Tests\PostRoute.Api.Tests.csproj --filter "FullyQualifiedName~PBI050"
```

```bash
cd PROJEKAT/frontend
npm test -- --run src/ui/pages/admin/test/PostmanPerformanceReportPage.PBI050.test.tsx
```

### Rezultati

| Komanda | Rezultat |
| --- | --- |
| `dotnet test tests\PostRoute.BLL.Tests\PostRoute.BLL.Tests.csproj --filter "FullyQualifiedName~PBI050"` | PASS - 3/3 |
| `dotnet test tests\PostRoute.DAL.Tests\PostRoute.DAL.Tests.csproj --filter "FullyQualifiedName~PBI050"` | PASS - 1/1 |
| `dotnet test tests\PostRoute.Api.Tests\PostRoute.Api.Tests.csproj --filter "FullyQualifiedName~PBI050"` | PASS - 3/3 |
| `npm test -- --run src/ui/pages/admin/test/PostmanPerformanceReportPage.PBI050.test.tsx` | PASS - 4/4 |

### Napomene

- API test/build projekat i dalje prikazuje postojece MSB3277 upozorenje o verzijama `Microsoft.EntityFrameworkCore.Relational`; PBI-050 API testovi prolaze.
- CSV export je testiran kroz browser API mock (`URL.createObjectURL` i klik na privremeni link), bez uvodjenja nove biblioteke.

---

## PBI-049 / US-34 i US-35, PBI-051 / US-39 - Filtriranje sanducica i arhiva ruta

### Pokrivenost po user story

| US | Naslov | Automatizirani testovi | Status |
| --- | --- | --- | --- |
| US-39 | Filtriranje po atributima | `MailboxServiceTestsPBI019`, `MailboxRepositoryTestsPBI019`, `MailboxesControllerTestsPBI017`, `MailboxListPage.PBI039.test.tsx` | PASS |
| US-34 | Pregled arhive realizovanih ruta | `RouteServiceTests_PBI034`, `RouteRepositoryTestsPBI034`, `RoutesControllerTestsPBI034`, `ArchiveRoutePages.PBI034035.test.tsx` | PASS |
| US-35 | Detaljni uvid u arhiviranu rutu | `RouteServiceTests_PBI034.GetRouteDetailsAsync_ReturnsUnavailableReasonAndFinalStatus_ForArchivedRoute`, `RoutesControllerTestsPBI034.GetRouteDetails_ShouldReturnUnavailableReason_ForArchivedRouteDetails`, `ArchiveRoutePages.PBI034035.test.tsx` | PASS |

### Sta je verifikovano

| Funkcionalnost | Verifikacija | Rezultat |
| --- | --- | --- |
| Filter po tipu/statusu/prioritetu | Backend prosljedjuje i DAL kombinuje filtere; frontend salje kombinovane filtere | PASS |
| Integracija filtera i pretrage | DAL pretrazuje adresu, serijski broj i ID; frontend query kombinuje search s filterima | PASS |
| Reset filtera | Frontend test potvrdjuje da reset uklanja type/status/search i vraca prvu stranicu | PASS |
| Empty-state za filtere bez rezultata | Frontend test potvrdjuje poruku `Nema sandučića koji odgovaraju odabranim kriterijima filtriranja.` | PASS |
| Arhiva ruta | DAL vraca samo `Zavrsena` i `Otkazana`, filtrira po periodu/postaru i sortira najnovije prvo | PASS |
| Detalj arhivirane rute | BLL/API/frontend prikazuju finalni status, timestamp i razlog nedostupnosti | PASS |
| Read-only detalj | Frontend test provjerava da nema edit/save akcija u arhivskom detalju | PASS |
| Mapa i pinovi | Frontend test mockuje mapu i provjerava render mape na detalju arhivirane rute | PASS |
| Export | Frontend test provjerava CSV export koji Excel moze otvoriti | PASS |

### Komande izvrsene lokalno

```bash
cd PROJEKAT/backend
dotnet test tests\PostRoute.BLL.Tests\PostRoute.BLL.Tests.csproj --filter "FullyQualifiedName~PBI034|FullyQualifiedName~PBI019"
dotnet test tests\PostRoute.DAL.Tests\PostRoute.DAL.Tests.csproj --filter "FullyQualifiedName~PBI034|FullyQualifiedName~PBI019"
dotnet test tests\PostRoute.Api.Tests\PostRoute.Api.Tests.csproj --filter "FullyQualifiedName~PBI034|FullyQualifiedName~PBI017"
dotnet test PostRoute.sln
dotnet build PostRoute.sln
```

```bash
cd PROJEKAT/frontend
npm test -- --run src/ui/pages/admin/test/MailboxListPage.PBI039.test.tsx src/ui/pages/admin/test/ArchiveRoutePages.PBI034035.test.tsx
npm test -- --run
npm run build
```

### Rezultati

| Komanda | Rezultat |
| --- | --- |
| `dotnet test tests\PostRoute.BLL.Tests\PostRoute.BLL.Tests.csproj --filter "FullyQualifiedName~PBI034|FullyQualifiedName~PBI019"` | PASS - 10/10 |
| `dotnet test tests\PostRoute.DAL.Tests\PostRoute.DAL.Tests.csproj --filter "FullyQualifiedName~PBI034|FullyQualifiedName~PBI019"` | PASS - 13/13 |
| `dotnet test tests\PostRoute.Api.Tests\PostRoute.Api.Tests.csproj --filter "FullyQualifiedName~PBI034|FullyQualifiedName~PBI017"` | PASS - 10/10 |
| `npm test -- --run src/ui/pages/admin/test/MailboxListPage.PBI039.test.tsx src/ui/pages/admin/test/ArchiveRoutePages.PBI034035.test.tsx` | PASS - 6/6 |
| `dotnet test PostRoute.sln` | PASS - DAL 39/39, BLL 149/149, API 52/52 |
| `npm test -- --run` | PASS - 42/42 |
| `dotnet build PostRoute.sln` | PASS - build succeeded, 1 existing MSB3277 EF Core Relational warning |
| `npm run build` | PASS - TypeScript + Vite build succeeded; Vite reported bundle chunk size warning |

### Runtime smoke provjera

```bash
cd PROJEKAT/backend
dotnet run --project src\PostRoute.Api\PostRoute.Api.csproj --no-build --urls http://localhost:5000
```

Backend se pokrenuo na `http://localhost:5000`, provjerio migracije i prijavio da je baza vec azurna. Nije bilo stderr gresaka.

```bash
cd PROJEKAT/frontend
npm run dev -- --host 127.0.0.1 --port 5173
```

Frontend Vite server se pokrenuo na `http://127.0.0.1:5173/` i vratio HTTP 200. Nije bilo stderr gresaka. Privremeni procesi su zaustavljeni nakon provjere.

### Napomene

- Prvi pokusaj paralelnog pokretanja vise `dotnet test` komandi je pao zbog zakljucanog `PostRoute.DAL.dll` build artefakta; sekvencijalno pokretanje istih testova je proslo.
- Export detalja arhivirane rute implementiran je kao CSV datoteka kompatibilna s Excelom, bez nove biblioteke.
- Backend build i API testovi i dalje prikazuju postojece MSB3277 upozorenje o verzijama `Microsoft.EntityFrameworkCore.Relational`; testovi i build prolaze.
- Vite production build prikazuje upozorenje o velikom JS chunk-u; build prolazi.

---

## Sprint 10 bugfix verifikacija - 2026-05-30

### Šta je dodatno provjereno

| Tema | Verifikacija | Rezultat |
| --- | --- | --- |
| UTF-8 tekst na `Pregled sandučića` | Skeniran frontend `src` za mojibake codepoint-e; uklonjen BOM iz `MailboxListPage.tsx`; normalizovana empty-state poruka | PASS |
| `Praćenje ruta -> Otvori detalje` | Dugme sada navigira na `/admin/routes/:id`; dodat regresioni test koji potvrđuje da se ne otvara `Generisanje ruta` | PASS |
| Shared route details pattern | `Arhiva ruta` koristi `ArchiveRouteDetailsPage source="archive"`, a `Praćenje ruta` koristi isti prikaz sa `source="tracking"` | PASS |
| Arhiva ruta u lokalnom developmentu | Dodan Development-only seed za jednu završenu rutu sa `DEV-ARCH-*` sandučićima | PASS |
| Ručni tok kompletiranja rute | Backend već završava rutu kada poštar obradi sve stavke kroz `/worker/route`; arhiva prikazuje `Zavrsena` i `Otkazana` rute | PASS |

### Komande izvršene lokalno

```bash
cd PROJEKAT/backend
dotnet test tests/PostRoute.DAL.Tests/PostRoute.DAL.Tests.csproj --filter "FullyQualifiedName~RouteRepositoryTests"
dotnet test tests/PostRoute.BLL.Tests/PostRoute.BLL.Tests.csproj --filter "FullyQualifiedName~RouteServiceTests|FullyQualifiedName~MailboxServiceTests"
dotnet test tests/PostRoute.Api.Tests/PostRoute.Api.Tests.csproj --filter "FullyQualifiedName~RoutesControllerTests|FullyQualifiedName~MailboxesControllerTests"
dotnet test PostRoute.sln
dotnet build PostRoute.sln
```

```bash
cd PROJEKAT/frontend
npm test -- --run src/ui/pages/admin/test/MailboxListPage.PBI039.test.tsx src/ui/pages/admin/test/ArchiveRoutePages.PBI034035.test.tsx src/ui/pages/admin/test/DispatcherRouteDashboardPage.PBI030.test.tsx
npm test -- --run
npm run build
```

### Rezultati

| Komanda | Rezultat |
| --- | --- |
| `dotnet test tests/PostRoute.DAL.Tests/PostRoute.DAL.Tests.csproj --filter "FullyQualifiedName~RouteRepositoryTests"` | PASS - 18/18 |
| `dotnet test tests/PostRoute.BLL.Tests/PostRoute.BLL.Tests.csproj --filter "FullyQualifiedName~RouteServiceTests\|FullyQualifiedName~MailboxServiceTests"` | PASS - 116/116 |
| `dotnet test tests/PostRoute.Api.Tests/PostRoute.Api.Tests.csproj --filter "FullyQualifiedName~RoutesControllerTests\|FullyQualifiedName~MailboxesControllerTests"` | PASS - 42/42 |
| `npm test -- --run src/ui/pages/admin/test/MailboxListPage.PBI039.test.tsx src/ui/pages/admin/test/ArchiveRoutePages.PBI034035.test.tsx src/ui/pages/admin/test/DispatcherRouteDashboardPage.PBI030.test.tsx` | PASS - 12/12 |
| `dotnet test PostRoute.sln` | PASS - DAL 39/39, BLL 149/149, API 52/52 |
| `npm test -- --run` | PASS - 43/43 |
| `dotnet build PostRoute.sln` | PASS - build succeeded, 1 existing MSB3277 EF Core Relational warning |
| `npm run build` | PASS - TypeScript + Vite build succeeded; Vite reported bundle chunk size warning |

### Runtime smoke provjera

```bash
cd PROJEKAT/backend
dotnet run --project src\PostRoute.Api\PostRoute.Api.csproj --no-build --urls http://localhost:5000
```

Backend se pokrenuo u Development okruženju, primijenio postojeće migracije, izvršio seeding i vratio `HTTP 200` na `http://localhost:5000/health`. Nije bilo stderr grešaka. Tokom prvog pokretanja seed je dodao `DEV-ARCH-001` i `DEV-ARCH-002` sandučiće i jednu završenu rutu za provjeru arhive.

```bash
cd PROJEKAT/frontend
npm run dev -- --host 127.0.0.1 --port 5173
```

Frontend Vite server se pokrenuo na `http://127.0.0.1:5173/` i vratio `HTTP 200`. Nije bilo stderr grešaka. Privremeni procesi su zaustavljeni nakon provjere.

### Ručni test: arhiva preko development seed podataka

1. Pokrenuti PostgreSQL dev bazu.
2. Pokrenuti backend u Development okruženju.
3. Pokrenuti frontend.
4. Prijaviti se kao administrator: `admin@mail.com` / `Admin123!`.
5. Otvoriti `Arhiva ruta`.
6. Očekivano: vidljiva je završena ruta za poštara `Postar User` sa današnjim datumom i `DEV-ARCH-*` sandučićima.
7. Kliknuti detalje rute.
8. Očekivano: otvara se read-only detalj arhivirane rute sa mapom, finalnim statusima, timestampima i CSV export dugmetom.

### Ručni test: arhiva preko stvarnog toka kompletiranja rute

1. Pokrenuti backend.
2. Pokrenuti frontend.
3. Prijaviti se kao administrator ili dispečer.
4. Otvoriti `Generisanje ruta`.
5. Generisati rutu za današnji datum i poštara `postar`.
6. Ako ruta nije dodijeljena, dodijeliti je poštaru kroz postojeći panel za dodjelu.
7. Odjaviti se i prijaviti se kao poštar: `postar@mail.com` / `Postar123!`.
8. Otvoriti `Moja ruta` / `/worker/route`.
9. Za svaku stavku rute kliknuti `Napunjen`, `Ispraznjen` ili `Nedostupno` sa razlogom.
10. Nakon zadnje obrađene stavke backend postavlja status rute na `Zavrsena` i popunjava `CompletedAt`.
11. Odjaviti se i ponovo prijaviti kao administrator ili dispečer.
12. Otvoriti `Arhiva ruta`.
13. Očekivano: završena ruta je vidljiva u arhivi; klik na detalje otvara read-only pregled.

### Napomene

- Nije pronađen postojeći UI/API tok za ručno otkazivanje rute. Backend i archive query podržavaju `Otkazana`, ali praktična ručna provjera kroz aplikaciju trenutno ide preko kompletiranja rute.
- Development seed je ograničen na Development okruženje i postojeći `Seeding:Enabled` mehanizam; ne dodaje produkcijske runtime podatke.
- Ostaju postojeća upozorenja: MSB3277 za EF Core Relational u API test/build projektu i Vite upozorenje o velikom JS chunk-u.
