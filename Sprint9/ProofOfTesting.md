# Proof of Testing - Sprint 9

---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit - Backend (BLL servisi) | US-26, US-27 + regresija svih prethodnih | xUnit + Moq | 115 testova | PASS |
| Unit - Backend (DAL repozitoriji) | Regresija svih prethodnih | xUnit + EF Core InMemory | 26 testova | PASS |
| Unit - Backend (API kontroleri) | US-26, US-27 + regresija svih prethodnih | xUnit + Moq | 30 testova | PASS |

**Ukupno verifikovano:** `dotnet test PostRoute.sln` prolazi — DAL 26/26, BLL 115/115, API 30/30. Postoji poznato MSB3277 upozorenje o transitive EF Core Relational 9.0.1/9.0.4 verzijama u API test buildu (prisutno od Sprint 8, ne utječe na testove).

**Prethodni sprint:** Sprint 8 imao BLL 92, DAL 26, API 15. Sprint 9 dodaje +23 BLL i +15 API testova kroz PBI-026 i PBI-027.

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

## Komande izvrsene lokalno

```bash
cd PROJEKAT/backend
dotnet test PostRoute.sln
```

Rezultat: PASS — DAL 26/26, BLL 115/115, API 30/30.

---

## Veza sa Test Strategijom

| Test strategija nivo | US | PBI | Dokaz | Status |
| --- | --- | --- | --- | --- |
| Unit - BLL servisi | US-26 | PBI-026 | `RouteServiceTests.PBI026` | PASS |
| Unit - API kontroleri | US-26 | PBI-026 | `RoutesControllerTests.PBI026` | PASS |
| Unit - BLL servisi | US-27 | PBI-027 | `MailboxServiceTests.PBI027` | PASS |
| Unit - API kontroleri | US-27 | PBI-027 | `MailboxesControllerTests.PBI027` | PASS |
