# Proof of Testing - Sprint 10

---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit - Backend (BLL servisi) | US-40, US-41, US-42, US-43, US-44 | xUnit + Moq | 11 testova | PASS |
| Unit - Backend (API kontroleri) | US-40, US-41, US-42, US-43, US-44 | xUnit + Moq | 10 testova | PASS |

**Ukupno verifikovano:** `dotnet test tests\PostRoute.BLL.Tests\PostRoute.BLL.Tests.csproj --filter "FullyQualifiedName~PBI052"` i `dotnet test tests\PostRoute.Api.Tests\PostRoute.Api.Tests.csproj --filter "FullyQualifiedName~PBI052"` prolaze — BLL 6/6, API 5/5. PBI-044 notifikacije su također pokrivene dodatnim testovima.

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
