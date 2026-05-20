# Proof of Testing — Sprint 8

---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit — Backend (BLL Servisi) | US-22, US-24, US-25 | xUnit + Moq | 21 testova | PASS |
| Unit — Backend (DAL Repozitoriji) | US-24 | xUnit + EF Core InMemory | 5 testova | PASS |
| **Ukupno Sprint 8** | **US-22, US-24, US-25** | | **26 testova** | **PASS** |

### Napomena

- **PBI-023** (US-23 — Vizualizacija rute na mapi) nema automatskih testova u ovom sprintu. Komponenta koristi Leaflet.js koji zahtijeva pravi DOM i tile server; rendering nije podržan u jsdom okruženju. Funkcionalno testiranje vršeno je ručno.
- **PBI-024** nema frontend testova jer `RouteDetailsPage` oslanja na Leaflet mapu koja nije renderabilna u jsdom.

---

## PBI-022 — Generisanje dnevne rute (US-22) — prošireni testovi

### Pokriveni AC

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — BLL | US-22 | Vraća postojeću rutu ako za izabranog poštara i datum već postoji ruta | `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldReturnExistingRoute_WhenSamePostmanAndDateAlreadyExist` | PASS |
| Unit — BLL | US-22 | Izbacuje neaktivne sandučiće iz generisane rute | `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldFilterOutInactiveMailboxes` | PASS |
| Unit — BLL | US-22 | Izbacuje sandučiće čiji vremenski okvir ne poklapa planirani start | `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldExcludeMailboxesOutsideTimeWindow` | PASS |
| Unit — BLL | US-22 | Prioritetno ponderisanje / pravila hladnog starta utiču na uključene tačke rute | `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldApplyPriorityCooldownRules` | PASS |
| Unit — BLL | US-22 | Izbacuje sandučić koji ne radi na dan rute (radni dani filter) | `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldExcludeMailboxes_WhenNotWorkingOnRouteDay` | PASS |
| Unit — BLL | US-22 | Uključuje sandučić koji radi na dan rute (radni dani filter) | `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldIncludeMailbox_WhenWorkingOnRouteDay` | PASS |

### Fajlovi sa testovima

- [backend/tests/PostRoute.BLL.Tests/RouteServiceTestsPBI022.cs](../PROJEKAT/backend/tests/PostRoute.BLL.Tests/RouteServiceTestsPBI022.cs) — 6 unit testova za logiku generisanja rute (uključuje 2 nova testa za filter radnih dana dodata u Sprintu 8).

---

## PBI-024 — Pregled detalja rute (US-24)

### Pokriveni AC

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — BLL | US-24 | Vraća detalje rute po ID-u | `RouteServiceTestsPBI024.GetRouteDetailsAsync_ShouldReturnRouteDetails_WhenRouteExists` | PASS |
| Unit — BLL | US-24 | Vraća null/baca exception kad ruta ne postoji | `RouteServiceTestsPBI024.GetRouteDetailsAsync_ShouldThrow_WhenRouteNotFound` | PASS |
| Unit — BLL | US-24 | Mapira sve stavke rute u odgovor | `RouteServiceTestsPBI024.GetRouteDetailsAsync_ShouldMapRouteItemsCorrectly` | PASS |
| Unit — BLL | US-24 | Sortira stavke rute po redoslijedu | `RouteServiceTestsPBI024.GetRouteDetailsAsync_ShouldReturnItemsOrderedByOrder` | PASS |
| Unit — DAL | US-24 | `GetByIdAsync` vraća rutu po ID-u | `RouteRepositoryTests.GetByIdAsync_ReturnsRoute_WhenExists` | PASS |
| Unit — DAL | US-24 | `GetByIdAsync` uključuje stavke rute | `RouteRepositoryTests.GetByIdAsync_IncludesRouteItems` | PASS |
| Unit — DAL | US-24 | `GetByIdAsync` uključuje podatke o sandučiću | `RouteRepositoryTests.GetByIdAsync_IncludesMailboxData` | PASS |
| Unit — DAL | US-24 | `GetByIdAsync` vraća null ako ruta ne postoji | `RouteRepositoryTests.GetByIdAsync_ReturnsNull_WhenNotFound` | PASS |
| Unit — DAL | US-24 | `GetByIdAsync` uključuje podatke o poštaru | `RouteRepositoryTests.GetByIdAsync_IncludesPostmanData` | PASS |

### Fajlovi sa testovima

- [backend/tests/PostRoute.BLL.Tests/Services/RouteServiceTests.PBI024.cs](../PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/RouteServiceTests.PBI024.cs) — 4 BLL unit testa za logiku dohvaćanja detalja rute.
- [backend/tests/PostRoute.DAL.Tests/Repositories/RouteRepositoryTests.cs](../PROJEKAT/backend/tests/PostRoute.DAL.Tests/Repositories/RouteRepositoryTests.cs) — 5 DAL testova za EF Core upit sa eager loadingom.

---

## PBI-025 — Ručna izmjena redoslijeda obilaska (US-25)

### Pokriveni AC

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — BLL | US-25 | Baca exception ako ruta nije pronađena | `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldThrow_WhenRouteNotFound` | PASS |
| Unit — BLL | US-25 | Baca exception ako je ruta u statusu UProgresu | `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldThrow_WhenRouteIsInProgress` | PASS |
| Unit — BLL | US-25 | Baca exception ako je ruta u statusu Zavrsena | `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldThrow_WhenRouteIsFinished` | PASS |
| Unit — BLL | US-25 | Primjenjuje novi redoslijed za rutu u statusu Planirana | `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldApplyNewOrder_WhenRouteIsPlanirana` | PASS |
| Unit — BLL | US-25 | Obilježava premještene stavke zastavicom `IsManuallyReordered` | `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldMarkMovedItems_AsManuallyReordered` | PASS |
| Unit — BLL | US-25 | Ne obilježava stavke ako se redoslijed nije promijenio | `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldNotMarkItems_AsManuallyReordered_WhenOrderUnchanged` | PASS |
| Unit — BLL | US-25 | Preračunava procijenjena vremena dolaska nakon promjene redoslijeda | `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldRecalculateArrivalTimes_AfterReorder` | PASS |
| Unit — BLL | US-25 | Ažurira ukupno trajanje i planirani kraj rute | `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldUpdateTotalDuration_AfterReorder` | PASS |
| Unit — BLL | US-25 | Postavlja audit polja `LastReorderedBy` i `LastReorderedAt` | `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldSetAuditFields_WithDispatcherNameAndTimestamp` | PASS |
| Unit — BLL | US-25 | Poziva `UpdateAsync` repozitorija tačno jednom | `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldCallUpdateAsync_ExactlyOnce` | PASS |
| Unit — BLL | US-25 | Dozvoljava izmjenu redoslijeda za rutu u statusu Otkazana | `RouteServiceTestsPBI025.ReorderRouteAsync_ShouldAllowReorder_WhenRouteIsOtkazana` | PASS |

### Fajlovi sa testovima

- [backend/tests/PostRoute.BLL.Tests/Services/RouteServiceTests.PBI025.cs](../PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/RouteServiceTests.PBI025.cs) — 11 unit testova za svu logiku ručnog preuređivanja rute.

---

## Veza sa Test Strategijom

| Test strategija nivo | US | PBI | Dokaz | Status |
| --- | --- | --- | --- | --- |
| Unit — BLL Servisi | US-22 | PBI-022 | `RouteServiceTestsPBI022` (6 testova) | PASS |
| Unit — BLL Servisi | US-24 | PBI-024 | `RouteServiceTests.PBI024` (4 testa) | PASS |
| Unit — DAL Repozitoriji | US-24 | PBI-024 | `RouteRepositoryTests` (5 testova) | PASS |
| Unit — BLL Servisi | US-25 | PBI-025 | `RouteServiceTests.PBI025` (11 testova) | PASS |

---

## Lokalno pokretanje testova

### Svi testovi

```bash
cd PROJEKAT/backend
dotnet test
```

### Samo Sprint 8 testovi (BLL)

```bash
cd PROJEKAT/backend
dotnet test --filter "FullyQualifiedName~PBI022||FullyQualifiedName~PBI024||FullyQualifiedName~PBI025"
```

### Po PBI-u

```bash
# PBI-022 (generisanje rute — prošireni)
dotnet test --filter "FullyQualifiedName~PBI022"

# PBI-024 (detalji rute — BLL + DAL)
dotnet test --filter "FullyQualifiedName~PBI024"

# PBI-025 (ručna izmjena redoslijeda)
dotnet test --filter "FullyQualifiedName~PBI025"
```
