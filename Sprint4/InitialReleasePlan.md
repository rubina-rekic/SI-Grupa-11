# Initial Release Plan

## Planirani inkrementi

Razvoj rješenja *Sistem za optimizaciju ruta punjenja i pražnjenja poštanskih sandučića* zasnovan je na **Scrum** metodologiji. Projektni ciklus obuhvata period od **Sprinta 5 do Sprinta 13**, osiguravajući iterativnu isporuku kroz četiri ključna izdanja (release-a).

Sistem je podijeljen tako da svaki release isporučuje potpuno testabilan i upotrebljiv dio softvera koji donosi direktnu vrijednost korisniku.

### Dinamika isporuke po fazama

| Release | Naziv inkrementa | Obuhvat Sprintova | Ključni ishod |
| :--- | :--- | :--- | :--- |
| **R1** | **System Foundation & RBAC Setup** | Sprint 5 – 6 | Funkcionalna platforma sa RBAC sistemom, upravljanjem korisnicima i registrom lokacija. |
| **R2** | **Optimization & Intelligence Service** | Sprint 7 – 8 | Implementiran algoritam za proračun ruta i dispečerski modul za planiranje rada. |
| **R3** | **Field Operations & Mobile Sync** | Sprint 9 – 10 | Aktivirana terenska aplikacija za poštare sa real-time potvrdom zadatka. |
| **R4** | **Analytics & Reporting** | Sprint 11 – 12 | Dashboard za analitiku (BI), automatizovano izvještavanje i finalna stabilizacija. |
---

## RELEASE 1 – SYSTEM FOUNDATION & RBAC SETUP

### Cilj inkrementa
Uspostaviti osnovni radni okvir sistema koji uključuje sigurnosnu infrastrukturu, upravljanje korisnicima i digitalnu evidenciju poštanskih sandučića na terenu.

### Glavne funkcionalnosti
- PBI-011: Kreiranje korisničkog računa poštara 
- PBI-012: Autentifikacija i prijava na sistem 
- PBI-014: Sistem uloga i permisija (Administrator, Dispečer, Poštar)
- PBI-015: Upravljanje sandučićima – unos GPS lokacija i osnovnih informacija 
- PBI-016: Kategorizacija sandučića prema tipu i prioritetu

### Zavisnosti
- PBI-010: Domain model
- PBI-017: Dizajn baze podataka
- PBI-039, PBI-040: Team Charter i Setup okruženja

### Rizici
- R-021: Sigurnosni propusti u autentifikaciji
- R-004: Netačni podaci o lokacijama sandučića (GPS koordinate)
- R-011: Kašnjenje u postavljanju razvojnog okruženja
- R-012: Konflikti pri spajanju koda između članova tima

### Okvirni sprintovi
- Sprint 5: Sigurnosni protokoli i upravljanje korisnicima
- Sprint 6: CRUD operacije za sandučiće i integracija mapa

### Vrijednost
Ovaj release predstavlja osnovni operativni sistem. Omogućava dispečerima da digitalizuju mrežu sandučića i sigurno upravljaju osobljem, čime se postavlja temelj za optimizaciju.

---

## RELEASE 2 – OPTIMIZATION & INTELLIGENCE SERVICE

### Cilj inkrementa
Implementirati ključnu poslovnu logiku – algoritam za optimizaciju ruta koji izračunava najefikasnije putanje za poštare.

### Glavne funkcionalnosti
- PBI-022: Razvoj algoritma za optimizaciju ruta (Nearest-neighbor heuristika)
- PBI-023: Dodjela generisanih ruta specifičnim poštarima
- PBI-025: Vizuelni prikaz optimizovane putanje na mapi za dispečera
- US-20: Ručna modifikacija rute u hitnim slučajevima

### Zavisnosti
- Release 1: Validni podaci o sandučićima i lokacijama

### Rizici
- R-001: Pogrešna procjena napora za algoritam optimizacije
- R-003: Generisanje suboptimalnih ruta uslijed logičke greške
- R-002: Problemi sa performansama algoritma pri velikom broju tačaka
- R-018: Teška integracija sa eksternim API servisima za mape

### Okvirni sprintovi
- Sprint 7: Razvoj i testiranje logike optimizacije
- Sprint 8: Vizualizacija rute i modul za dodjelu zadataka

### Vrijednost
Sistem postaje "inteligentan". Primarna vrijednost je ušteda vremena i resursa (goriva) kroz smanjenje pređenih kilometara poštara.

---

## RELEASE 3 – FIELD OPERATIONS & MOBILE SYNC

### Cilj inkrementa
Povezati dispečerski centar sa radom na terenu omogućavanjem poštarima da prate rute putem mobilnog interfejsa i potvrđuju obavljene zadatke.

### Glavne funkcionalnosti
- PBI-028: Responzivni interfejs za mobilne uređaje 
- PBI-030: Digitalna potvrda pražnjenja/punjenja sandučića u realnom vremenu
- US-32: Slanje notifikacija poštaru o novododijeljenoj ruti
- US-35: Mogućnost prijave problema na sandučiću (npr. oštećenje) direktno sa terena

### Zavisnosti
- Release 2: Funkcionalan modul za dodjelu ruta
- NFR-22: Sigurna HTTPS/TLS komunikacija

### Rizici
- R-033: Slabe performanse na starijim mobilnim uređajima
- R-020: Gubitak mrežne konekcije na terenu pri slanju potvrde
- R-034: UI/UX neusklađenost sa potrebama krajnjih korisnika (poštara)
- R-035: Problemi sa keširanjem podataka na klijentskoj strani

### Okvirni sprintovi
- Sprint 9: Mobilni interfejs i notifikacije
- Sprint 10: Logika potvrde zadataka i rukovanje offline stanjem

### Vrijednost
Ovim release-om se zatvara operativni ciklus. Sistem više nije samo alat za planiranje, već postaje alat za direktno izvršenje i praćenje rada na terenu.

---

## RELEASE 4 – ANALYTICS & REPORTING

### Cilj inkrementa
Omogućiti uvid u performanse sistema kroz izvještaje i analitiku, te finalizirati tehničku dokumentaciju za produkcijsko okruženje.

### Glavne funkcionalnosti
- PBI-037: Dashboard sa statistikom (broj ispražnjenih sandučića, pređeni kilometri)
- PBI-038: Generisanje i izvoz izvještaja u PDF/Excel formatu
- PBI-056: Finalna tehnička dokumentacija (Architecture, Deployment guide)
- PBI-061: Završna stabilizacija sistema i bug fixing

### Zavisnosti
- Stabilna baza podataka sa historijskim podacima iz Release-a 3

### Rizici
- R-007: Netačna interpretacija podataka u statističkim izvještajima
- R-008: Nedostatak testnih podataka za generisanje reprezentativnih izvještaja
- R-036: Greške pri generisanju PDF dokumenata uslijed specifičnih karaktera
- R-010: Kašnjenje u izradi dokumentacije zbog fokusa na tehničke bugove

### Okvirni sprintovi
- Sprint 11: Razvoj modula za izvještavanje i analitiku
- Sprint 12: Finalizacija dokumentacije, poliranje UI-a i završna demonstracija

### Vrijednost
Zadnji release pretvara sirove podatke u korisne informacije za menadžment. Omogućava analizu efikasnosti i pruža uvid u opravdanost investicije u sistem optimizacije.

---

## 3. Kriteriji završetka i osiguranje kvaliteta

Svaki release se smatra zvanično isporučenim isključivo ukoliko su ispunjeni sljedeći uslovi:
1.  **DoD usklađenost:** Sve stavke inkrementa ispunjavaju stavke definisane u *Definition of Done* dokumentu .
2.  **QA Verifikacija:** Inkrement je uspješno prošao planirana QA testiranja bez otvorenih kritičnih bugova.
3.  **PO Approval:** Product Owner je potvrdio ispunjenje Acceptance Criteria na Sprint Review sastanku.

---

## ZAKLJUČAK
Plan je strukturiran tako da tim u ranim fazama rješava tehnički najzahtjevnije stavke (Autentifikacija i Algoritam), dok su kasnije faze fokusirane na korisničko iskustvo na terenu i analitičku nadogradnju, čime se rizik neuspjeha projekta svodi na minimum.