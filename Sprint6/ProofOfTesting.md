# Proof of Testing — Sprint 6

---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit — Backend (BLL Servisi) | US-11, US-12, US-13, US-14, US-15 | xUnit + Moq | 46 testova | PASS |
| Unit — Backend (API Kontroleri) | US-14, US-15, US-16, US-17 | xUnit + Moq | 18 testova | PASS |
| Unit — Backend (DAL Repozitorij) | US-14, US-15, US-16, US-17 | xUnit + EF InMemory | 11 testova | PASS |
| Unit — Frontend (komponente i interakcija) | US-14, US-15 | Jest + React Testing Library | 17 testova | PASS |
| Unit — Frontend (poslovna logika) | US-14, US-15 | TypeScript (vanilla) | 8 testnih grupa | PASS |
| **Ukupno Sprint 6** | **US-11 do US-15** | | **~100 testova** | **PASS** |

---

## PBI-015 — Dodavanje poštara

### Pokriveni AC (US-11, US-12)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit | US-11 | Uspješno kreiranje novog poštara s validnim podacima | `UserServiceTests.CreateAsync_WhenCommandIsValid_ShouldAddUser` | PASS |
| Unit | US-11 | Mapiranje kreiranog korisnika na model | `UserServiceTests.CreateAsync_WhenCommandIsValid_ShouldReturnMappedUserModel` | PASS |
| Unit | US-11 | Nevažeća uloga odbija kreiranje korisnika | `UserServiceTests.CreateAsync_WhenRoleIsInvalid_ShouldThrowInvalidOperationException` | PASS |
| Unit | US-11 | Promjena lozinke ažurira hash lozinke | `UserServiceTests.ChangePasswordAsync_WhenRequestIsValid_ShouldUpdatePasswordHash` | PASS |
| Unit | US-11 | `MustChangePassword` flag se resetuje nakon uspješne promjene lozinke (prva prijava) | `UserServiceTests.ChangePasswordAsync_WhenRequestIsValid_ShouldSetMustChangePasswordToFalse` | PASS |
| Unit | US-11 | Promjena lozinke odbija neispravnu trenutnu lozinku | `UserServiceTests.ChangePasswordAsync_WhenCurrentPasswordIsIncorrect_ShouldThrowInvalidOperationException` | PASS |
| Unit | US-11 | Nova lozinka mora se razlikovati od trenutne | `UserServiceTests.ChangePasswordAsync_WhenNewPasswordIsSameAsCurrentPassword_ShouldThrowInvalidOperationException` | PASS |
| Unit | US-11 | Promjena lozinke odbija nepostojećeg korisnika | `UserServiceTests.ChangePasswordAsync_WhenUserDoesNotExist_ShouldThrowInvalidOperationException` | PASS |
| Unit | US-12 | Dupli email odbijen s greškom | `UserServiceTests.CreateAsync_WhenEmailAlreadyExists_ShouldThrowInvalidOperationException` | PASS |
| Unit | US-12 | Duplo korisničko ime odbijeno s greškom | `UserServiceTests.CreateAsync_WhenUsernameAlreadyExists_ShouldThrowInvalidOperationException` | PASS |
| Unit | US-12 | Provjera emaila vrši se prije korisničkog imena — rana terminacija pri duplikatu | `UserServiceTests.CreateAsync_WhenEmailExists_ShouldNotCheckUsername` | PASS |

### Fajlovi sa testovima

- [backend/tests/PostRoute.BLL.Tests/Services/UserServiceTests.cs](../PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/UserServiceTests.cs) — 22 testa (pokriva PBI-015 i PBI-016)

---

## PBI-016 — Pregled liste poštara

### Pokriveni AC (US-13)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit | US-13 | Lista korisnika vraća se s ispravnim `IsLockedOut` flagom za svakog poštara | `UserServiceTests.GetAllAsync_WhenUsersExist_ShouldReturnMappedUserModelsWithIsLockedOut` | PASS |
| Unit | US-13 | Prazna lista vraća se kada nema korisnika u sistemu | `UserServiceTests.GetAllAsync_WhenNoUsersExist_ShouldReturnEmptyList` | PASS |
| Unit | US-13 | Greška iz repozitorija se propagira prema pozivaocu | `UserServiceTests.GetAllAsync_WhenRepositoryThrowsException_ShouldPropagateException` | PASS |

### Fajlovi sa testovima

- [backend/tests/PostRoute.BLL.Tests/Services/UserServiceTests.cs](../PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/UserServiceTests.cs) — 22 testa (pokriva PBI-015 i PBI-016)

---

## PBI-017 — Dodavanje poštanskog sandučića

### Pokriveni AC (US-14, US-15)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — BLL | US-14, US-15 | Uspješno kreiranje sandučića s validnim podacima | `MailboxServiceTests.CreateAsync_ShouldCreateMailboxWithValidData` | PASS |
| Unit — BLL | US-15 | Dupli serijski broj odbija kreiranje s `InvalidOperationException` | `MailboxServiceTests.CreateAsync_ShouldThrowExceptionWhenSerialNumberExists` | PASS |
| Unit — BLL | US-14, US-15 | Nevažeće koordinate, kapacitet ≤ 0 i nevažeća godina instalacije odbijeni — 7 parametriziranih scenarija | `MailboxServiceTests.CreateAsync_ShouldThrowExceptionWithInvalidCoordinatesOrData` | PASS |
| Unit — BLL | US-15 | Sva 4 tipa sandučića podržana: WallSmall, StandaloneLarge, IndoorResidential, SpecialPriority | `MailboxServiceTests.CreateAsync_ShouldHandleAllMailboxTypes` | PASS |
| Unit — BLL | US-15 | Napomena (notes) može biti null | `MailboxServiceTests.CreateAsync_ShouldHandleNullNotes` | PASS |
| Unit — BLL | US-15 | Napomena može biti prazan string | `MailboxServiceTests.CreateAsync_ShouldHandleEmptyNotes` | PASS |
| Unit — BLL | US-14, US-15 | Whitespace se trimuje iz serijskog broja, adrese i napomene | `MailboxServiceTests.CreateAsync_ShouldTrimWhitespaceFromStrings` | PASS |
| Unit — BLL | US-14, US-15 | Repozitorij metode pozvane tačno jednom pri kreiranju | `MailboxServiceTests.CreateAsync_ShouldCallRepositoryMethods` | PASS |
| Unit — BLL | US-15 | Dupli serijski broj odbijen — `InvalidOperationException` (opći servis) | `MailboxServiceTests.CreateAsync_WhenSerialNumberExists_ShouldThrowInvalidOperationException` | PASS |
| Unit — BLL | US-14 | Latitude van opsega (−90 do 90) odbijen | `MailboxServiceTests.CreateAsync_WhenLatitudeIsOutOfRange_ShouldThrowInvalidOperationException` | PASS |
| Unit — BLL | US-14 | Longitude van opsega (−180 do 180) odbijen | `MailboxServiceTests.CreateAsync_WhenLongitudeIsOutOfRange_ShouldThrowInvalidOperationException` | PASS |
| Unit — BLL | US-15 | Kapacitet ≤ 0 odbijen | `MailboxServiceTests.CreateAsync_WhenCapacityIsInvalid_ShouldThrowInvalidOperationException` | PASS |
| Unit — BLL | US-15 | Godina instalacije van opsega (1900 do tekuće godine) odbijena | `MailboxServiceTests.CreateAsync_WhenInstallationYearIsInvalid_ShouldThrowInvalidOperationException` | PASS |
| Unit — BLL | US-14, US-15 | Validno kreiranje sandučića kroz opći servis | `MailboxServiceTests.CreateAsync_WhenValidData_ShouldCreateMailbox` | PASS |
| Unit — BLL | US-14, US-15 | Sandučić se dohvata po ID-u | `MailboxServiceTests.GetByIdAsync_WhenMailboxExists_ShouldReturnMailbox` | PASS |
| Unit — BLL | US-14, US-15 | Null vraćen za nepostojući sandučić po ID-u | `MailboxServiceTests.GetByIdAsync_WhenMailboxDoesNotExist_ShouldReturnNull` | PASS |
| Unit — BLL | US-15 | Sandučić se dohvata po serijskom broju | `MailboxServiceTests.GetBySerialNumberAsync_WhenMailboxExists_ShouldReturnMailbox` | PASS |
| Unit — BLL | US-15 | Delegiranje provjere serijskog broja repozitoriju | `MailboxServiceTests.SerialNumberExistsAsync_ShouldReturnRepositoryResult` | PASS |
| Unit — API | US-14, US-15 | API vraća 201 Created s ispravnim Location headerom i kompletnim `MailboxResponse` | `MailboxesControllerTests.CreateAsync_ShouldReturnCreatedResult_WhenValidRequest` | PASS |
| Unit — API | US-14, US-15 | Prazan serijski broj vraća 400 Bad Request | `MailboxesControllerTests.CreateAsync_ShouldReturnBadRequest_WhenInvalidRequest` | PASS |
| Unit — API | US-15 | Dupli serijski broj vraća 409 Conflict | `MailboxesControllerTests.CreateAsync_ShouldReturnConflict_WhenSerialNumberExists` | PASS |
| Unit — API | US-14, US-15 | Neočekivana greška servisa vraća 500 Internal Server Error | `MailboxesControllerTests.CreateAsync_ShouldReturnInternalServerError_WhenServiceThrowsException` | PASS |
| Unit — API | US-14, US-15 | Nevažeće koordinate, kapacitet i godina vraćaju 400 — 7 parametriziranih scenarija | `MailboxesControllerTests.CreateAsync_ShouldReturnBadRequest_WhenInvalidData` | PASS |
| Unit — API | US-15 | Sva 4 tipa sandučića prihvaćena i ispravno vraćena u odgovoru | `MailboxesControllerTests.CreateAsync_ShouldHandleAllMailboxTypes` | PASS |
| Unit — API | US-15 | Null napomena sačuvana i ispravno vraćena | `MailboxesControllerTests.CreateAsync_ShouldHandleNullNotes` | PASS |
| Unit — API | US-15 | `checkSerialNumberExists` vraća true za postojeći serijski broj | `MailboxesControllerTests.CheckSerialNumberExists_ShouldReturnTrue_WhenSerialNumberExists` | PASS |
| Unit — API | US-15 | `checkSerialNumberExists` vraća false za nepostojući serijski broj | `MailboxesControllerTests.CheckSerialNumberExists_ShouldReturnFalse_WhenSerialNumberDoesNotExist` | PASS |
| Unit — DAL | US-14, US-15 | Sandučić persistira u bazi s `CreatedAt` i `UpdatedAt` timestampovima | `MailboxRepositoryTests.AddAsync_ShouldAddMailboxToDatabase` | PASS |
| Unit — DAL | US-15 | Sandučić dohvaćen po serijskom broju | `MailboxRepositoryTests.GetBySerialNumberAsync_WhenMailboxExists_ShouldReturnMailbox` | PASS |
| Unit — DAL | US-15 | Null vraćen za nepostojući serijski broj | `MailboxRepositoryTests.GetBySerialNumberAsync_WhenMailboxDoesNotExist_ShouldReturnNull` | PASS |
| Unit — DAL | US-15 | `SerialNumberExistsAsync` vraća true za postojeći serijski broj | `MailboxRepositoryTests.SerialNumberExistsAsync_WhenSerialNumberExists_ShouldReturnTrue` | PASS |
| Unit — DAL | US-15 | `SerialNumberExistsAsync` vraća false za nepostojući serijski broj | `MailboxRepositoryTests.SerialNumberExistsAsync_WhenSerialNumberDoesNotExist_ShouldReturnFalse` | PASS |
| Unit — Frontend | US-14, US-15 | Forma prikazuje sva polja: serijski broj, adresa, tip, kapacitet, godina, napomena | `CreateMailboxPage.test.tsx > Form Rendering > should render all form fields` | PASS |
| Unit — Frontend | US-14 | Dugme za prikaz interaktivne mape se renderuje | `CreateMailboxPage.test.tsx > Form Rendering > should render map toggle button` | PASS |
| Unit — Frontend | US-14, US-15 | Dugmad "Spremi" i "Odustani" se renderuju | `CreateMailboxPage.test.tsx > Form Rendering > should render save and cancel buttons` | PASS |
| Unit — Frontend | US-14, US-15 | Prikazuju se greške validacije za obavezna polja pri pokušaju slanja | `CreateMailboxPage.test.tsx > Form Validation > should show validation errors for required fields` | PASS |
| Unit — Frontend | US-15 | Validacija formata serijskog broja (minimalna dužina, format) | `CreateMailboxPage.test.tsx > Form Validation > should validate serial number format` | PASS |
| Unit — Frontend | US-14 | Validacija opsega koordinata — latitude prikazuje grešku van opsega | `CreateMailboxPage.test.tsx > Form Validation > should validate coordinate ranges` | PASS |
| Unit — Frontend | US-14 | Klik na toggle prikazuje OpenStreetMap komponentu | `CreateMailboxPage.test.tsx > Map Integration > should show map when toggle button is clicked` | PASS |
| Unit — Frontend | US-14 | Odabir lokacije na mapi automatski popunjava polja koordinata | `CreateMailboxPage.test.tsx > Map Integration > should update coordinates when location is selected` | PASS |
| Unit — Frontend | US-14, US-15 | Forma se uspješno šalje s validnim podacima — API pozvan s ispravnim payload-om | `CreateMailboxPage.test.tsx > Form Submission > should submit form successfully with valid data` | PASS |
| Unit — Frontend | US-14, US-15 | Toast poruka uspjeha prikazana nakon kreiranja sandučića | `CreateMailboxPage.test.tsx > Form Submission > should show success message when mailbox is created` | PASS |
| Unit — Frontend | US-14, US-15 | Toast poruka greške "greška pri kreiranju sandučića" prikazana pri neuspjehu | `CreateMailboxPage.test.tsx > Form Submission > should show error message when creation fails` | PASS |
| Unit — Frontend | US-14, US-15 | Forma se resetuje na prazne vrijednosti nakon uspješnog slanja | `CreateMailboxPage.test.tsx > Form Submission > should reset form after successful submission` | PASS |
| Unit — Frontend | US-15 | API poziv za provjeru serijskog broja pozvan pri `onBlur` | `CreateMailboxPage.test.tsx > Serial Number Validation > should check serial number uniqueness` | PASS |
| Unit — Frontend | US-15 | Inline greška "sandučić sa ovim serijskim brojem već postoji" prikazana | `CreateMailboxPage.test.tsx > Serial Number Validation > should show error when serial number already exists` | PASS |
| Unit — Frontend | US-15 | Dropdown prikazuje sva 4 tipa sandučića | `CreateMailboxPage.test.tsx > Mailbox Types > should render all mailbox types` | PASS |
| Unit — Frontend | US-15 | Odabir različitih tipova iz dropdowna funkcionira ispravno | `CreateMailboxPage.test.tsx > Mailbox Types > should allow selection of different mailbox types` | PASS |
| Unit — Frontend | US-14, US-15 | Klik na "Odustani" navigira korisnika nazad | `CreateMailboxPage.test.tsx > Cancel Button > should navigate back when cancel is clicked` | PASS |
| Unit — Frontend (logika) | US-14, US-15 | Kreiranje sandučića s validnim podacima vraća ispravna polja: ID, serijski broj, adresa, koordinate, tip, kapacitet, godina, napomena, timestampovi | `CreateMailboxPage.simple.test.ts > testCreateMailboxWithValidData` | PASS |
| Unit — Frontend (logika) | US-14, US-15 | Nevažeći podaci odbijeni: prazan serijski broj, lat=91, kapacitet=0 | `CreateMailboxPage.simple.test.ts > testCreateMailboxWithInvalidData` | PASS |
| Unit — Frontend (logika) | US-15 | Provjera serijskog broja vraća true/false ispravno | `CreateMailboxPage.simple.test.ts > testCheckSerialNumberExists` | PASS |
| Unit — Frontend (logika) | US-15 | Sva 4 valjana tipa (1–4) prihvaćena; tip 99 odbijen | `CreateMailboxPage.simple.test.ts > testMailboxTypeValidation` | PASS |
| Unit — Frontend (logika) | US-14 | Validni opseg koordinata (uključujući rubne vrijednosti: polovi, ekvator, nulte linije) prihvaćen; nevažeće vrijednosti (−91, 91, −181, 181) odbijene | `CreateMailboxPage.simple.test.ts > testCoordinateValidation` | PASS |
| Unit — Frontend (logika) | US-15 | Kapaciteti 1–10 000 prihvaćeni; kapacitet 0 i negativni odbijeni | `CreateMailboxPage.simple.test.ts > testCapacityValidation` | PASS |
| Unit — Frontend (logika) | US-15 | Godine u opsegu 1900 do tekuća+10 prihvaćene; van opsega odbijene | `CreateMailboxPage.simple.test.ts > testInstallationYearValidation` | PASS |
| Unit — Frontend (logika) | US-15 | Prazne i dugačke napomene (500+ znakova) obrađene ispravno | `CreateMailboxPage.simple.test.ts > testNotesHandling` | PASS |

### Fajlovi sa testovima

- [backend/tests/PostRoute.BLL.Tests/Services/MailboxServiceTests.PBI017.cs](../PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/MailboxServiceTests.PBI017.cs) — 14 testova (8 metoda; uključuje [Theory] s 7 parametriziranih scenarija)
- [backend/tests/PostRoute.BLL.Tests/Services/MailboxServiceTests.cs](../PROJEKAT/backend/tests/PostRoute.BLL.Tests/Services/MailboxServiceTests.cs) — 10 testova
- [backend/PostRoute.Api.Tests/Controllers/MailboxesControllerTests.PBI017.cs](../PROJEKAT/backend/PostRoute.Api.Tests/Controllers/MailboxesControllerTests.PBI017.cs) — 18 testova (12 metoda; uključuje [Theory] s 7 parametriziranih scenarija)
- [backend/tests/PostRoute.DAL.Tests/Repositories/MailboxRepositoryTests.cs](../PROJEKAT/backend/tests/PostRoute.DAL.Tests/Repositories/MailboxRepositoryTests.cs) — 11 testova (pokriva PBI-017, PBI-018, PBI-019)
- [frontend/src/ui/pages/admin/__tests__/CreateMailboxPage.test.tsx](../PROJEKAT/frontend/src/ui/pages/admin/__tests__/CreateMailboxPage.test.tsx) — 17 testova
- [frontend/src/ui/pages/admin/__tests__/CreateMailboxPage.simple.test.ts](../PROJEKAT/frontend/src/ui/pages/admin/__tests__/CreateMailboxPage.simple.test.ts) — 8 testnih grupa

---

## PBI-018 i PBI-019 — Izmjena i pregled sandučića (djelimično pokriveno)

PBI-018 (US-16: Ažuriranje podataka o sandučiću) i PBI-019 (US-17: Pregled liste sandučića) su u statusu *To Do* za ovaj sprint. Repozitorij i API sloj su međutim testirani i pripremljeni za buduću implementaciju servisa.

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — DAL | US-16 | Ažuriranje sandučića persistira izmjene s novim `UpdatedAt` timestampom | `MailboxRepositoryTests.UpdateAsync_ShouldUpdateMailboxInDatabase` | PASS |
| Unit — DAL | US-17 | Lista svih sandučića vraća se sortirana po serijskom broju | `MailboxRepositoryTests.GetAllAsync_ShouldReturnAllMailboxesOrderedBySerialNumber` | PASS |
| Unit — DAL | US-17 | Sandučić dohvaćen po ID-u — svi podaci ispravno učitani | `MailboxRepositoryTests.GetByIdAsync_WhenMailboxExists_ShouldReturnMailbox` | PASS |
| Unit — DAL | US-17 | Null vraćen za nepostojući ID | `MailboxRepositoryTests.GetByIdAsync_WhenMailboxDoesNotExist_ShouldReturnNull` | PASS |
| Unit — DAL | US-17 | Brisanje sandučića uklanja entitet iz baze | `MailboxRepositoryTests.DeleteAsync_ShouldRemoveMailboxFromDatabase` | PASS |
| Unit — DAL | US-17 | Brisanje nepostojećeg sandučića ne baca grešku | `MailboxRepositoryTests.DeleteAsync_WhenMailboxDoesNotExist_ShouldNotThrowException` | PASS |
| Unit — API | US-17 | API vraća kompletnu listu svih sandučića | `MailboxesControllerTests.GetAllAsync_ShouldReturnAllMailboxes` | PASS |
| Unit — API | US-17 | API vraća sandučić po ID-u s ispravnim podacima | `MailboxesControllerTests.GetByIdAsync_ShouldReturnMailbox_WhenMailboxExists` | PASS |
| Unit — API | US-17 | API vraća 404 Not Found za nepostojući sandučić | `MailboxesControllerTests.GetByIdAsync_ShouldReturnNotFound_WhenMailboxDoesNotExist` | PASS |

---

## Veza sa Test Strategijom

| Test strategija nivo | US | PBI | Dokaz | Status |
| --- | --- | --- | --- | --- |
| Unit — BLL Servisi | US-11, US-12, US-13, US-14, US-15 | PBI-015, PBI-016, PBI-017 | `UserServiceTests`, `MailboxServiceTests`, `MailboxServiceTests.PBI017` | PASS |
| Unit — API Kontroleri | US-14, US-15, US-17 | PBI-017, PBI-019 | `MailboxesControllerTests.PBI017` | PASS |
| Unit — DAL Repozitorij (EF InMemory) | US-14, US-15, US-16, US-17 | PBI-017, PBI-018, PBI-019 | `MailboxRepositoryTests` | PASS |
| Unit — Frontend komponente | US-14, US-15 | PBI-017 | `CreateMailboxPage.test.tsx` | PASS |
| Unit — Frontend poslovna logika | US-14, US-15 | PBI-017 | `CreateMailboxPage.simple.test.ts` | PASS |

---

## Lokalno pokretanje testova

### Backend

```bash
cd PROJEKAT/backend

# Svi testovi
dotnet test

# Samo PBI-017 testovi
dotnet test --filter "FullyQualifiedName~PBI017"

# S izvještajem o pokrivenosti koda
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend

```bash
cd PROJEKAT/frontend

# Jest + React Testing Library testovi
npm test -- CreateMailboxPage.test.tsx

# Vanilla TypeScript testovi
npx ts-node --project tsconfig.json src/ui/pages/admin/__tests__/CreateMailboxPage.simple.test.ts
```
