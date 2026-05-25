# Proof of Testing - Sprint 9

---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit - Backend (BLL servisi) | US-26, US-27, US-29 + regresija svih prethodnih | xUnit + Moq | 128 testova | PASS |
| Unit - Backend (DAL repozitoriji) | US-29 + regresija svih prethodnih | xUnit + EF Core InMemory | 34 testa | PASS |
| Unit - Backend (API kontroleri) | US-26, US-27, US-29 + regresija svih prethodnih | xUnit + Moq | 35 testova | PASS |
| Unit - Frontend (React UI) | US-30 | Vitest + Testing Library | 5 testova | PASS |

**Ukupno verifikovano:** `dotnet test PostRoute.sln --no-build` prolazi — DAL 34/34, BLL 128/128, API 35/35. Za PBI-030 frontend testovi prolaze komandom `npm test -- --run src/ui/pages/admin/test/DispatcherRouteDashboardPage.PBI030.test.tsx` — 5/5, a cijeli frontend test paket prolazi komandom `npm test -- --run` — 36/36. Postoji poznato MSB3277 upozorenje o transitive EF Core Relational 9.0.1/9.0.4 verzijama u API test buildu (prisutno od Sprint 8, ne utječe na testove).

**Prethodni sprint:** Sprint 8 imao BLL 92, DAL 26, API 15. Sprint 9 dodaje +36 BLL, +8 DAL, +20 API i +5 frontend testova kroz PBI-026, PBI-027, PBI-029, PBI-030 i regresijske popravke završavanja rute.

---

## PBI-026 - Pregled moje današnje rute (US-26)

### Pokriveni AC

| Nivo | AC | Test koji pokriva | Status |
| --- | --- | --- | --- |
| BLL | Vraća rutu kada postoji dodijeljena ruta za danas | `RouteServiceTestsPBI026.GetPostmanAssignedRouteForTodayAsync_ShouldReturnRoute_WhenAssignedRouteExists` | PASS |
| BLL | Vraća rutu kada je status `UProgresu` | `RouteServiceTestsPBI026.GetPostmanAssignedRouteForTodayAsync_ShouldReturnRoute_WhenRouteIsInProgress` | PASS |
| BLL | Vraća null kada nema rute za danas | `RouteServiceTestsPBI026.GetPostmanAssignedRouteForTodayAsync_ShouldReturnNull_WhenNoRouteFound` | PASS |
| BLL | Vraća null za rutu u statusu `Planirana` (nije dodijeljena) | `RouteServiceTestsPBI026.GetPostmanAssignedRouteForTodayAsync_ShouldReturnNull_WhenRouteIsPlanned` | PASS |
| BLL | Vraća null za otkazanu rutu | `RouteServiceTestsPBI026.GetPostmanAssignedRouteForTodayAsync_ShouldReturnNull_WhenRouteCancelled` | PASS |
| BLL | Vraća null za završenu rutu | `RouteServiceTestsPBI026.GetPostmanAssignedRouteForTodayAsync_ShouldReturnNull_WhenRouteCompleted` | PASS |
| BLL | Detalji sandučića (adresa, koordinate, prioritet) prisutni u stavkama rute | `RouteServiceTestsPBI026.GetPostmanAssignedRouteForTodayAsync_ShouldIncludeMailboxDetails_InRouteItems` | PASS |
| BLL | Status sandučića (`MailboxStatus`) prenosi se kroz stavke rute | `RouteServiceTestsPBI026.GetPostmanAssignedRouteForTodayAsync_ShouldIncludeMailboxStatus_InRouteItems` | PASS |
| BLL | Stavke rute sortirane po `Order` bez obzira na redoslijed u bazi | `RouteServiceTestsPBI026.GetPostmanAssignedRouteForTodayAsync_ShouldReturnRouteItemsOrderedByOrder_WhenStoredOutOfOrder` | PASS |
| API | `GET /api/routes/my-assigned-route/today` vraća 200 s dodijeljenom rutom | `RoutesControllerTestsPBI026.GetMyAssignedRouteForToday_ShouldReturnOk_WithAssignedRoute` | PASS |
| API | Vraća 401 kada korisnik nije autentifikovan | `RoutesControllerTestsPBI026.GetMyAssignedRouteForToday_ShouldReturnUnauthorized_WhenUserNotAuthenticated` | PASS |
| API | Vraća 200 s rutom u statusu `UProgresu` | `RoutesControllerTestsPBI026.GetMyAssignedRouteForToday_ShouldReturnOk_WithInProgressRoute` | PASS |
| API | Servis se poziva s ispravnim `postmanId` iz JWT tokena | `RoutesControllerTestsPBI026.GetMyAssignedRouteForToday_ShouldCallServiceWithCorrectPostmanId` | PASS |
| API | Stavke rute sadrže sve obavezne podatke (koordinate, prioritet, status) | `RoutesControllerTestsPBI026.GetMyAssignedRouteForToday_ShouldIncludeRouteItems_WithCompleteData` | PASS |
| API | Vraća 200 s porukom (ne 404) kada nema dodijeljene rute | `RoutesControllerTestsPBI026.GetMyAssignedRouteForToday_ShouldReturnOkWithMessage_WhenNoRouteAssignedToday` | PASS |

### Fajlovi sa testovima

- `PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/RouteServiceTests.PBI026.cs` — 9 BLL unit testova.
- `PROJEKAT/backend/tests/PostRoute.Api.Tests/Controllers/RoutesControllerTests.PBI026.cs` — 6 API controller testova.

---

## PBI-027 - Ažuriranje statusa sandučića (US-27)

### Pokriveni AC

| Nivo | AC | Test koji pokriva | Status |
| --- | --- | --- | --- |
| BLL | Status se ažurira kada sandučić postoji | `MailboxServiceTestsPBI027.UpdateStatusAsync_ShouldUpdateStatus_WhenMailboxExists` | PASS |
| BLL | Prihvata svih 5 validnih statusa (`Obraen`, `Napunjen`, `Ispraznjen`, `Pun`, `Prazan`) | `MailboxServiceTestsPBI027.UpdateStatusAsync_ShouldAcceptAllValidStatuses` (Theory, 5 slučaja) | PASS |
| BLL | `UpdatedAt` timestamp se ažurira pri promjeni statusa | `MailboxServiceTestsPBI027.UpdateStatusAsync_ShouldUpdateUpdatedAt` | PASS |
| BLL | Audit log sadrži ispravna polja (MailboxId, UserId, FieldName, OldValue, NewValue, Action) | `MailboxServiceTestsPBI027.UpdateStatusAsync_ShouldLogAuditEntry_WithCorrectFields` | PASS |
| BLL | Audit log bilježi razlog kada je proslijeđen | `MailboxServiceTestsPBI027.UpdateStatusAsync_ShouldLogAuditEntry_WithReason_WhenProvided` | PASS |
| BLL | Audit log čuva `null` razlog kada nije proslijeđen | `MailboxServiceTestsPBI027.UpdateStatusAsync_ShouldLogAuditEntry_WithNullReason_WhenNotProvided` | PASS |
| BLL | Audit log sadrži timestamp unutar okvirnog vremena poziva | `MailboxServiceTestsPBI027.UpdateStatusAsync_ShouldLogAuditEntry_WithTimestamp` | PASS |
| BLL | Audit log se piše tačno jednom po pozivu | `MailboxServiceTestsPBI027.UpdateStatusAsync_ShouldCallAuditLog_ExactlyOnce` | PASS |
| BLL | Baca `InvalidOperationException` kada sandučić ne postoji | `MailboxServiceTestsPBI027.UpdateStatusAsync_ShouldThrow_WhenMailboxNotFound` | PASS |
| BLL | Ne piše audit log kada sandučić nije pronađen | `MailboxServiceTestsPBI027.UpdateStatusAsync_ShouldNotCallAuditLog_WhenMailboxNotFound` | PASS |
| API | `PATCH /api/mailboxes/{id}/status` vraća 200 s ažuriranim sandučićem | `MailboxesControllerTestsPBI027.UpdateStatusAsync_ShouldReturnOk_WhenRequestIsValid` | PASS |
| API | Kontroler prosljeđuje tačnu komandu servisu (MailboxId, NewStatus, UserId, Reason) | `MailboxesControllerTestsPBI027.UpdateStatusAsync_ShouldPassCorrectCommandToService` | PASS |
| API | Prihvata sva tri postarska statusa (`Obraen`, `Napunjen`, `Ispraznjen`) | `MailboxesControllerTestsPBI027.UpdateStatusAsync_ShouldReturnOk_ForAllPostmanStatuses` (Theory, 3 slučaja) | PASS |
| API | Vraća 401 kada sesija nema `UserId` | `MailboxesControllerTestsPBI027.UpdateStatusAsync_ShouldReturnUnauthorized_WhenSessionHasNoUserId` | PASS |
| API | Vraća 401 kada `UserId` u sesiji nije validan GUID | `MailboxesControllerTestsPBI027.UpdateStatusAsync_ShouldReturnUnauthorized_WhenSessionUserIdIsNotValidGuid` | PASS |
| API | Vraća 404 kada sandučić ne postoji | `MailboxesControllerTestsPBI027.UpdateStatusAsync_ShouldReturnNotFound_WhenMailboxDoesNotExist` | PASS |
| API | Vraća 400 pri neispravnom modelu, servis se ne poziva | `MailboxesControllerTestsPBI027.UpdateStatusAsync_ShouldReturnBadRequest_WhenModelStateInvalid` | PASS |

### Fajlovi sa testovima

- `PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/MailboxServiceTests.PBI027.cs` — 14 BLL unit testova (10 metoda, od kojih 1 Theory s 5 slučaja).
- `PROJEKAT/backend/tests/PostRoute.Api.Tests/Controllers/MailboxesControllerTests.PBI027.cs` — 9 API controller testova (7 metoda, od kojih 1 Theory s 3 slučaja). Sadrži `TestSession : ISession` helper klasu.

---

## PBI-029 - Praćenje statusa rute od strane dispečera (US-29)

### Pokriveni AC

| Nivo | AC | Test koji pokriva | Status |
| --- | --- | --- | --- |
| BLL | Vraća sve rute za zadani datum | `RouteServiceTestsPBI029.GetRoutesForDateAsync_ShouldReturnAllRoutes_ForGivenDate` | PASS |
| BLL | Vraća praznu listu kada nema ruta za datum | `RouteServiceTestsPBI029.GetRoutesForDateAsync_ShouldReturnEmptyList_WhenNoRoutesForDate` | PASS |
| BLL | Prosljeđuje tačan datum repozitoriju | `RouteServiceTestsPBI029.GetRoutesForDateAsync_ShouldPassCorrectDate_ToRepository` | PASS |
| BLL | Mapira puno ime poštara (`FirstName LastName`) | `RouteServiceTestsPBI029.GetRoutesForDateAsync_ShouldMapPostmanName_WhenPostmanExists` | PASS |
| BLL | Mapira status rute u string reprezentaciju | `RouteServiceTestsPBI029.GetRoutesForDateAsync_ShouldMapStatus_ToStringRepresentation` | PASS |
| BLL | Normalizuje rutu u `Zavrsena` kada su svi `RouteItem` zapisi obrađeni | `RouteServiceTestsPBI029.GetRoutesForDateAsync_ShouldNormalizeRouteToCompleted_WhenAllItemsProcessed` | PASS |
| BLL | Ne završava rutu samo na osnovu globalnog `MailboxStatus` sandučića | `RouteServiceTestsPBI029.GetRoutesForDateAsync_ShouldNotCompleteRoute_FromMailboxStatusOnly` | PASS |
| BLL | Uključuje stavke rute s `MailboxStatus` iz sandučića | `RouteServiceTestsPBI029.GetRoutesForDateAsync_ShouldIncludeRouteItems_WithMailboxStatus` | PASS |
| BLL | Vraća rutu s ispravnim `Id` koji se podudara s originalnom rutom | `RouteServiceTestsPBI029.GetRoutesForDateAsync_ShouldReturnRouteWithId_MatchingOriginalRoute` | PASS |
| DAL | Vraća rute za zadani datum | `RouteRepositoryTestsPBI029.GetByDateAsync_ShouldReturnRoutes_ForGivenDate` | PASS |
| DAL | Isključuje rute za druge datume | `RouteRepositoryTestsPBI029.GetByDateAsync_ShouldExcludeRoutesForOtherDates` | PASS |
| DAL | Vraća praznu listu kada nema ruta | `RouteRepositoryTestsPBI029.GetByDateAsync_ShouldReturnEmptyList_WhenNoRoutesForDate` | PASS |
| DAL | Eager-load: uključuje poštara (`Postman`) | `RouteRepositoryTestsPBI029.GetByDateAsync_ShouldIncludePostman_InResult` | PASS |
| DAL | Eager-load: uključuje stavke rute sa sandučićem (`RouteItems.Mailbox`) | `RouteRepositoryTestsPBI029.GetByDateAsync_ShouldIncludeRouteItems_WithMailbox` | PASS |
| DAL | Sortiranje po statusu pa po planiranom početnom vremenu | `RouteRepositoryTestsPBI029.GetByDateAsync_ShouldOrderBy_StatusThenPlannedStartTime` | PASS |
| DAL | Dohvata aktivnu rutu po poštaru i sandučiću kada datum lookup promaši | `RouteRepositoryTestsPBI029.GetActiveByPostmanAndMailboxAsync_ShouldReturnActiveRouteContainingMailbox` | PASS |
| DAL | Ignoriše završene rute pri fallback dohvatu aktivne rute | `RouteRepositoryTestsPBI029.GetActiveByPostmanAndMailboxAsync_ShouldIgnoreCompletedRoutes` | PASS |
| API | `GET /api/routes?date=` vraća 200 s listom ruta | `RoutesControllerTestsPBI029.GetByDate_ShouldReturnOk_WithListOfRoutes` | PASS |
| API | Vraća 200 s praznom listom kada nema ruta za datum | `RoutesControllerTestsPBI029.GetByDate_ShouldReturnOk_WithEmptyList_WhenNoRoutesExist` | PASS |
| API | Kontroler prosljeđuje tačan datum servisu | `RoutesControllerTestsPBI029.GetByDate_ShouldCallService_WithCorrectDate` | PASS |
| API | Vraća rute svih statusa u jednom pozivu | `RoutesControllerTestsPBI029.GetByDate_ShouldReturnAllStatuses_InSingleCall` | PASS |
| API | Stavke rute sadrže `MailboxStatus` iz sandučića | `RoutesControllerTestsPBI029.GetByDate_ShouldReturnRouteItems_WithMailboxStatus` | PASS |

### Fajlovi sa testovima

- `PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/RouteServiceTests.PBI029.cs` — 9 BLL unit testova.
- `PROJEKAT/backend/tests/PostRoute.DAL.Tests/Repositories/RouteRepositoryTests.PBI029.cs` — 8 DAL integration testova (EF Core InMemory). Ne referencira `PostRoute.Domain` — `User` entitet dostupan kroz `PostRoute.DAL.Entities`.
- `PROJEKAT/backend/tests/PostRoute.Api.Tests/Controllers/RoutesControllerTests.PBI029.cs` — 5 API controller testova. Controller setup s `Dispatcher` ulogom u `ClaimsPrincipal`.

---

## PBI-030 - Osnovni dnevni izvještaj (US-30)

### Pokriveni AC

| Nivo | AC | Test koji pokriva | Status |
| --- | --- | --- | --- |
| Frontend | Dispečer/administrator može izabrati poštara i kliknuti "Generiši izvještaj" | `DispatcherRouteDashboardPage.PBI030.generise dnevni izvjestaj sa zaglavljem, sumarnim blokom i detaljnom tabelom` | PASS |
| Frontend | Izvještaj sadrži zaglavlje, sumarni blok i detaljnu tabelu sa adresom, prioritetom, finalnim statusom i timestampom | `DispatcherRouteDashboardPage.PBI030.generise dnevni izvjestaj sa zaglavljem, sumarnim blokom i detaljnom tabelom` | PASS |
| Frontend | Obrađeni, nedostupni i neposjećeni unosi vizuelno se razlikuju u tabeli | `DispatcherRouteDashboardPage.PBI030.generise dnevni izvjestaj sa zaglavljem, sumarnim blokom i detaljnom tabelom` provjerava `rdb-report-row--processed`, `rdb-report-row--unavailable`, `rdb-report-row--unvisited` | PASS |
| Frontend | Kada je realizacija ispod 80%, prikazuje se narandžasto upozorenje | `DispatcherRouteDashboardPage.PBI030.prikazuje upozorenje kada je realizacija ispod 80 posto` | PASS |
| Frontend | Kada nema rute za odabrani datum i poštara, prikazuje se poruka "Nema podataka za odabrane parametre." | `DispatcherRouteDashboardPage.PBI030.prikazuje poruku kada za odabrani datum i postara nema rute` | PASS |
| Frontend | Ruta sa 100% obrađenih sandučića prikazuje se kao završena i realizacija je 100% | `DispatcherRouteDashboardPage.PBI030.u izvjestaju i kartici prikazuje Zavrsena kada su svi sanducici obradeni` | PASS |
| Frontend | Dugme "Preuzmi PDF" otvara print-friendly HTML izvještaj sa podacima za PDF export | `DispatcherRouteDashboardPage.PBI030.otvara print-friendly PDF prikaz sa podacima izvjestaja` | PASS |

### Fajlovi sa testovima

- `PROJEKAT/frontend/src/ui/pages/admin/test/DispatcherRouteDashboardPage.PBI030.test.tsx` — 5 frontend unit/integration testova sa Vitest + Testing Library.

---

## Komande izvrsene lokalno

```bash
cd PROJEKAT/backend
dotnet test PostRoute.sln
```

Rezultat: PASS — DAL 34/34, BLL 128/128, API 35/35.

```bash
cd PROJEKAT/frontend
npm test -- --run src/ui/pages/admin/test/DispatcherRouteDashboardPage.PBI030.test.tsx
```

Rezultat: PASS — 5/5 PBI-030 frontend testova.

```bash
cd PROJEKAT/frontend
npm test -- --run
```

Rezultat: PASS — 36/36 frontend testova u 4 test fajla.

```bash
cd PROJEKAT/frontend
npm run build
```

Rezultat: PASS — TypeScript i Vite produkcijski build uspješni. Vite prikazuje standardno upozorenje da je jedan chunk veći od 500 kB.

---

## Veza sa Test Strategijom

| Test strategija nivo | US | PBI | Dokaz | Status |
| --- | --- | --- | --- | --- |
| Unit - BLL servisi | US-26 | PBI-026 | `RouteServiceTests.PBI026` | PASS |
| Unit - API kontroleri | US-26 | PBI-026 | `RoutesControllerTests.PBI026` | PASS |
| Unit - BLL servisi | US-27 | PBI-027 | `MailboxServiceTests.PBI027` | PASS |
| Unit - API kontroleri | US-27 | PBI-027 | `MailboxesControllerTests.PBI027` | PASS |
| Unit - BLL servisi | US-29 | PBI-029 | `RouteServiceTests.PBI029` | PASS |
| Unit - DAL repozitoriji | US-29 | PBI-029 | `RouteRepositoryTests.PBI029` | PASS |
| Unit - API kontroleri | US-29 | PBI-029 | `RoutesControllerTests.PBI029` | PASS |
| Unit/Integration - Frontend UI | US-30 | PBI-030 | `DispatcherRouteDashboardPage.PBI030.test.tsx` | PASS |
