# Proof of Testing — Sprint 7

---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit — Backend (BLL Servisi) | US-22, US-32 | xUnit + Moq | 12 testova | PASS |
| Unit — Frontend (komponente i interakcija) | US-18, US-32 | Vitest + React Testing Library | 27 testova | PASS |
| **Ukupno Sprint 7** | **US-18, US-22, US-32** | | **~39 testova** | **PASS** |

---

## PBI-020 — Postavljanje prioriteta sandučića (US-18)

### Pokriveni AC

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Frontend | US-18 | Dropdown za prioritet je prisutan u formi za dodavanje sandučića | `CreateMailboxPage.test.tsx > prikazuje dropdown za prioritet` | PASS |
| Unit — Frontend | US-18 | Forma za kreiranje sandučića šalje validan payload sa prioritetom | `CreateMailboxPage.test.tsx > uspješno šalje formu s validnim podacima` | PASS |
| Unit — Frontend | US-18 | Edit sandučića poziva update API i zadržava dostupne podatke | `EditMailboxPage.test.tsx > poziva updateMailbox pri slanju` | PASS |

### Napomena o statusu

- US-18 je izveden i djelomično pokriven testovima kroz frontend validaciju i slanje forme.
- US-19 i US-20 su u Sprint 7 backlogu označeni kao *To Do* i nisu završeni, stoga u ovom sprintu nemaju kompletne testove.

### Fajlovi sa testovima

- [frontend/src/ui/pages/admin/test/CreateMailboxPage.test.tsx](../PROJEKAT/frontend/src/ui/pages/admin/test/CreateMailboxPage.test.tsx) — 27 frontend testova za formu kreiranja i osnovnu validaciju.
- [frontend/src/ui/pages/admin/test/EditMailboxPage.test.tsx](../PROJEKAT/frontend/src/ui/pages/admin/test/EditMailboxPage.test.tsx) — 4 frontend testa za uređivanje sandučića.

---

## PBI-021 — Definisanje vremenskih okvira dostupnosti sandučića (US-32)

### Pokriveni AC

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — BLL | US-32 | Kreiranje sandučića sa 24/7 dostupnošću | `MailboxServiceTests.PBI021.CreateAsync_WithAlwaysAvailable_ShouldCreateSuccessfully` | PASS |
| Unit — BLL | US-32 | Kreiranje sandučića sa jednim vremenskim okvirom | `MailboxServiceTests.PBI021.CreateAsync_WithSingleTimeSlot_ShouldCreateSuccessfully` | PASS |
| Unit — BLL | US-32 | Kreiranje sandučića sa dva vremenska okvira | `MailboxServiceTests.PBI021.CreateAsync_WithTwoTimeSlots_ShouldCreateSuccessfully` | PASS |
| Unit — BLL | US-32 | Validacija da kraj vremena ne može biti prije početka | `MailboxServiceTests.PBI021.CreateAsync_WithEndTimeBeforeStartTime_ShouldThrow` | PASS |
| Unit — BLL | US-32 | Validacija preklapanja vremenskih okvira | `MailboxServiceTests.PBI021.CreateAsync_WithOverlappingTimeSlots_ShouldThrow` | PASS |
| Unit — BLL | US-32 | Validacija da je potreban barem jedan vremenski okvir ako nije 24/7 | `MailboxServiceTests.PBI021.CreateAsync_WithoutTimeSlotAndNotAlwaysAvailable_ShouldThrow` | PASS |
| Unit — BLL | US-32 | Ažuriranje sandučića s novim vremenskim terminima | `MailboxServiceTests.PBI021.UpdateAsync_WithNewTimeSlots_ShouldUpdateSuccessfully` | PASS |
| Unit — BLL | US-32 | Edge case: noćni termin (00:00–06:00) | `MailboxServiceTests.PBI021.CreateAsync_WithMidnightHours_ShouldCreateSuccessfully` | PASS |

### Fajlovi sa testovima

- [backend/tests/PostRoute.BLL.Tests/Services/MailboxServiceTests.PBI021.cs](../PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/MailboxServiceTests.PBI021.cs) — 8 unit testova za logiku vremenskih okvira i radnih pravila.

---

## PBI-022 — Generisanje dnevne rute (US-22)

### Pokriveni AC

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — BLL | US-22 | Vraća postojeću rutu ako za izabranog poštara i datum već postoji ruta | `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldReturnExistingRoute_WhenSamePostmanAndDateAlreadyExist` | PASS |
| Unit — BLL | US-22 | Izbacuje neaktivne sandučiće iz generisane rute | `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldFilterOutInactiveMailboxes` | PASS |
| Unit — BLL | US-22 | Izbacuje sandučiće čiji vremenski okvir ne poklapa planirani start | `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldExcludeMailboxesOutsideTimeWindow` | PASS |
| Unit — BLL | US-22 | Prioritetno ponderisanje / pravila hladnog starta utiču na uključene tačke rute | `RouteServiceTestsPBI022.GenerateRouteAsync_ShouldApplyPriorityCooldownRules` | PASS |

### Fajlovi sa testovima

- [backend/tests/PostRoute.BLL.Tests/RouteServiceTestsPBI022.cs](../PROJEKAT/backend/tests/PostRoute.BLL.Tests/RouteServiceTestsPBI022.cs) — 4 unit testa za logiku generisanja rute.

---

## Veza sa Test Strategijom

| Test strategija nivo | US | PBI | Dokaz | Status |
| --- | --- | --- | --- | --- |
| Unit — BLL Servisi | US-32, US-22 | PBI-021, PBI-022 | `MailboxServiceTests.PBI021`, `RouteServiceTestsPBI022` | PASS |
| Unit — Frontend komponente | US-18, US-32 | PBI-020, PBI-021 | `CreateMailboxPage.test.tsx`, `EditMailboxPage.test.tsx` | PASS |

---

## Lokalno pokretanje testova

### Backend

```bash
cd PROJEKAT/backend

dotnet test --filter "FullyQualifiedName~PBI021||FullyQualifiedName~PBI022"
```

### Frontend

```bash
cd PROJEKAT/frontend

npm test -- CreateMailboxPage.test.tsx
npm test -- EditMailboxPage.test.tsx
```
