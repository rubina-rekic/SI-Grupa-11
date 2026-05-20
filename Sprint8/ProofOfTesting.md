# Proof of Testing - Sprint 8

---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit - Backend (BLL servisi) | US-22, US-23, US-24, US-25 | xUnit + Moq | 92 testa | PASS |
| Unit - Backend (DAL repozitoriji) | US-23, US-24 | xUnit + EF Core InMemory | 26 testova | PASS |
| Unit - Backend (API kontroleri) | US-23, US-24 + regresija mailbox kontrolera | xUnit + Moq | 15 testova | PASS |
| Unit/Component - Frontend | US-23 + regresija mailbox formi | Vitest + Testing Library | 31 test | PASS |
| Build - Frontend | Sprint 8 UI | TypeScript + Vite | 1 build | PASS |

**Ukupno verifikovano:** backend `dotnet test .\PostRoute.sln` prolazi (92 BLL + 26 DAL + 15 API), frontend `npm test -- --run` prolazi (31 test), frontend `npm run build` prolazi.

**Napomena:** API test projekat je dodan u `PostRoute.sln`, tako da komanda `dotnet test .\PostRoute.sln` od sada pokriva i controller testove. Tokom API test builda ostaje postojece MSB3277 upozorenje o transitive EF Core Relational 9.0.1/9.0.4 verzijama, ali testovi prolaze bez gresaka.

---

## PBI-023 - Dodjela rute postaru (US-23)

### Pokriveni AC

| Nivo | AC | Test koji pokriva | Status |
| --- | --- | --- | --- |
| BLL | Dodjela mijenja status rute iz prijedloga u `Dodijeljena` i evidentira postara, vrijeme i dispecera | `RouteServiceTestsPBI023.AssignRouteAsync_ShouldAssignPlaniranaRoute_ToAvailablePostman` | PASS |
| BLL | Sistem ne dozvoljava dodjelu postaru koji vec ima aktivnu rutu za isti datum | `RouteServiceTestsPBI023.AssignRouteAsync_ShouldThrow_WhenPostmanAlreadyHasActiveRouteForDate` | PASS |
| BLL | Preraspodjela nije dozvoljena kada je ruta u toku | `RouteServiceTestsPBI023.AssignRouteAsync_ShouldThrow_WhenRouteIsInProgress` | PASS |
| BLL | Samo aktivni postari mogu biti odabrani | `RouteServiceTestsPBI023.AssignRouteAsync_ShouldThrow_WhenUserIsNotActivePostman` | PASS |
| BLL | Lista dostupnih postara filtrira neaktivne i oznacava zauzete | `RouteServiceTestsPBI023.GetAvailablePostmenAsync_ShouldReturnActivePostmenWithAvailability` | PASS |
| DAL | Repozitorij vraca samo postare sa dodijeljenim ili aktivnim rutama za datum | `RouteRepositoryTestsPBI023.GetPostmanIdsWithActiveRouteOnDateAsync_ReturnsAssignedAndInProgressRoutesOnly` | PASS |
| DAL | Repozitorij izuzima trenutnu rutu kod preraspodjele | `RouteRepositoryTestsPBI023.GetPostmanIdsWithActiveRouteOnDateAsync_ExcludesCurrentRoute` | PASS |
| API | `PUT /api/routes/{id}/assign` vraca dodijeljenu rutu | `RoutesControllerTestsPBI023.Assign_ShouldReturnOk_WithAssignedRoute` | PASS |
| API | `PUT /api/routes/{id}/assign` vraca BadRequest kada servis odbije dodjelu | `RoutesControllerTestsPBI023.Assign_ShouldReturnBadRequest_WhenServiceRejectsAssignment` | PASS |
| API | `GET /api/routes/{id}/available-postmen` vraca listu dostupnosti | `RoutesControllerTestsPBI023.GetAvailablePostmen_ShouldReturnOk_WithAvailabilityList` | PASS |
| Frontend | Nakon generisanja prijedloga prikazuje se dugme `Dodijeli postaru` | `GenerateRoutePage - PBI-023.prikazuje dugme Dodijeli postaru nakon generisanja prijedloga rute` | PASS |
| Frontend | Dropdown prikazuje zauzetog postara kao onemogucen izbor | `GenerateRoutePage - PBI-023.prikazuje dropdown sa zauzetim postarom kao onemogucenim izborom` | PASS |
| Frontend | Potvrda dodjele poziva API i prikazuje toast uspjeha | `GenerateRoutePage - PBI-023.dodjeljuje rutu odabranom postaru i prikazuje toast uspjeha` | PASS |
| Frontend | Kada nema slobodnih postara prikazuje se poruka | `GenerateRoutePage - PBI-023.prikazuje poruku kada nema slobodnih postara za datum rute` | PASS |

### Fajlovi sa testovima

- `PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/RouteServiceTests.PBI023.cs` - 5 BLL unit testova.
- `PROJEKAT/backend/tests/PostRoute.DAL.Tests/Repositories/RouteRepositoryTests.PBI023.cs` - 2 DAL unit testa.
- `PROJEKAT/backend/tests/PostRoute.Api.Tests/Controllers/RoutesControllerTests.PBI023.cs` - 3 API controller testa.
- `PROJEKAT/frontend/src/ui/pages/admin/test/GenerateRoutePage.PBI023.test.tsx` - 4 frontend component testa.

---

## PBI-022 - Generisanje dnevne rute (US-22)

Postojeci i prosireni testovi za generisanje rute ostaju aktivni i prolaze u punom BLL suite-u.

### Ključni testovi

- `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldReturnExistingRoute_WhenSamePostmanAndDateAlreadyExist`
- `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldFilterOutInactiveMailboxes`
- `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldExcludeMailboxesOutsideTimeWindow`
- `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldApplyPriorityCooldownRules`
- `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldExcludeMailboxes_WhenNotWorkingOnRouteDay`
- `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldIncludeMailbox_WhenWorkingOnRouteDay`

---

## PBI-024 - Pregled detalja rute (US-24)

BLL, DAL i API testovi za pregled detalja rute prolaze nakon azuriranja API testova na aktuelni `RouteItemResponse` ugovor (`Address`, `Latitude`, `Longitude`, `Priority` kao string).

### Ključni testovi

- `RouteServiceTestsPBI024.GetRouteDetailsAsync_ShouldReturnRouteDetails_WhenRouteExists`
- `RouteServiceTestsPBI024.GetRouteDetailsAsync_ShouldIncludeRouteItems_InResponse`
- `RouteRepositoryTestsPBI024.GetByIdAsync_ReturnsRoute_WithIncludedItems`
- `RouteRepositoryTestsPBI024.GetByIdAsync_IncludesPostmanData`
- `RoutesControllerTestsPBI024.GetRouteDetails_ShouldReturnOkResult_WithRouteData`

---

## PBI-025 - Rucna izmjena redoslijeda obilaska (US-25)

Postojecih 11 BLL testova za `ReorderRouteAsync` prolaze u punom BLL suite-u. Funkcionalnost nije regresirana dodavanjem statusa `Dodijeljena`; rute u statusima `UProgresu` i `Zavrsena` i dalje su blokirane.

### Ključni testovi

- `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldApplyNewOrder_WhenRouteIsPlanirana`
- `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldMarkMovedItems_AsManuallyReordered`
- `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldRecalculateArrivalTimes_AfterReorder`
- `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldSetAuditFields_WithDispatcherNameAndTimestamp`

---

## Komande izvrsene lokalno

```bash
cd PROJEKAT/backend
dotnet test .\PostRoute.sln
```

Rezultat: PASS - DAL 26/26, BLL 92/92, API 15/15.

```bash
cd PROJEKAT/frontend
npm test -- --run
```

Rezultat: PASS - 31/31 frontend testova.

```bash
cd PROJEKAT/frontend
npm run build
```

Rezultat: PASS - TypeScript build i Vite production build uspjesni. Vite prijavljuje standardno upozorenje da je glavni chunk veci od 500 kB.

---

## Veza sa Test Strategijom

| Test strategija nivo | US | PBI | Dokaz | Status |
| --- | --- | --- | --- | --- |
| Unit - BLL servisi | US-22 | PBI-022 | `RouteServiceTestsPBI022` | PASS |
| Unit - BLL servisi | US-23 | PBI-023 | `RouteServiceTests.PBI023` | PASS |
| Unit - DAL repozitoriji | US-23 | PBI-023 | `RouteRepositoryTests.PBI023` | PASS |
| Unit - API kontroleri | US-23 | PBI-023 | `RoutesControllerTests.PBI023` | PASS |
| Component - Frontend | US-23 | PBI-023 | `GenerateRoutePage.PBI023.test.tsx` | PASS |
| Unit - BLL/DAL/API | US-24 | PBI-024 | `RouteServiceTests.PBI024`, `RouteRepositoryTests.PBI024`, `RoutesControllerTests.PBI024` | PASS |
| Unit - BLL servisi | US-25 | PBI-025 | `RouteServiceTests.PBI025` | PASS |
