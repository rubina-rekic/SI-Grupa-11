# Initial Release Plan

## Planirani inkrementi

Razvoj rješenja *Sistem za optimizaciju ruta punjenja i pražnjenja poštanskih sandučića* organizovan je prema **Scrum metodologiji**, uz jasno definisane iteracije i kontinuiranu isporuku vrijednosti.

Projektni ciklus obuhvata period od **Sprinta 5 do Sprinta 13**, tokom kojeg se planira realizacija kroz **četiri ključna izdanja (release-a)**. Svaki release predstavlja zaokruženu funkcionalnu cjelinu koja se može samostalno testirati i koristiti u realnom okruženju.

### Dinamika isporuke po fazama

| Release | Naziv inkrementa | Obuhvat Sprintova | Ključni ishod |
| :--- | :--- | :--- | :--- |
| **R1** | **System Foundation & RBAC Setup** | Sprint 5 – 6 | Postavljena osnovna platforma sa autentifikacijom, upravljanjem korisnicima i registrom sandučića. |
| **R2** | **Optimization & Intelligence Service** | Sprint 7 – 8 | Implementiran algoritam za optimizaciju ruta, vizuelni prikaz i mobilni interfejs. |
| **R3** | **Field Operations & Monitoring** | Sprint 9 – 10 | Real-time potvrda zadataka sa terena, praćenje ruta i prva faza stabilizacije. |
| **R4** | **Analytics & Finalization** | Sprint 11 – 13 | Uvedena BI analitika, kompletirana dokumentacija i izvršena završna demonstracija. |

---

## RELEASE 1 – SYSTEM FOUNDATION & RBAC SETUP

### Cilj inkrementa
Uspostaviti osnovni radni okvir sistema koji uključuje sigurnosnu infrastrukturu, upravljanje korisnicima i digitalnu evidenciju poštanskih sandučića.

### Glavne funkcionalnosti
* **PBI-011:** Kreiranje korisničkog računa poštara
* **PBI-012:** Autentifikacija i prijava na sistem
* **PBI-013:** Oporavak lozinke (Forgot password)
* **PBI-014:** Sistem uloga i permisija (Admin, Dispečer, Poštar)
* **PBI-015:** Upravljanje profilima poštara
* **PBI-017:** Dodavanje poštanskog sandučića (GPS lokacija i osnovne informacije)
* **PBI-020:** Definisanje prioriteta sandučića
* **PBI-021:** Evidencija radnih pravila (vremena pražnjenja)

### Zavisnosti
* **PBI-033:** Domain model (osnova za bazu podataka)
* **PBI-038:** Tehnički skeleton projekta (.NET/Git setup)
* **PBI-039:** Team Charter (radna pravila tima)

### Rizici
* **R-010:** Sigurnosni propusti u autentifikaciji i autorizaciji
* **R-011:** Kašnjenje u postavljanju razvojnog okruženja
* **R-015:** Inkonzistentni podaci pri unosu GPS koordinata sandučića
* **R-023:** Konflikti pri spajanju koda (Merge conflicts)

### Okvirni sprintovi
* **Sprint 5:** Sigurnosni protokoli, upravljanje korisnicima i PoC algoritma.
* **Sprint 6:** CRUD operacije za sandučiće, prioriteti i radna pravila.

### Vrijednost
Ovaj release digitalizuje mrežu sandučića i omogućava sigurno upravljanje osobljem, čime se postavlja temelj za primjenu optimizacijskih algoritama.

---

## RELEASE 2 – OPTIMIZATION & INTELLIGENCE SERVICE

### Cilj inkrementa
Implementirati ključnu poslovnu logiku kroz algoritam za optimizaciju ruta i omogućiti prvi uvid u mobilni interfejs.

### Glavne funkcionalnosti
* **PBI-022:** Razvoj algoritma za optimizaciju ruta (Heuristika)
* **PBI-023:** Automatska dodjela ruta poštarima
* **PBI-025:** Vizuelni prikaz optimizovane putanje na mapi za dispečera
* **PBI-026:** Responzivni (mobilni) prikaz dodijeljene rute

### Zavisnosti
* **Release 1:** Validni podaci o sandučićima u bazi podataka.
* **PBI-040:** Decision Log (dokumentovana odluka o odabranom algoritmu).

### Rizici
* **R-001:** Pogrešna procjena napora za algoritam (kompleksnost implementacije)
* **R-008:** Zavisnost o eksternim servisima (OpenStreetMap API preciznost)
* **R-034:** Preveliki JavaScript bundle usporava mobilni prikaz rute

### Okvirni sprintovi
* **Sprint 7:** Implementacija logike optimizacije i integracija sa mapama.
* **Sprint 8:** Vizualizacija rute za dispečera i inicijalni mobilni UI.

### Vrijednost
Primarna vrijednost je ušteda vremena i resursa (goriva) kroz smanjenje pređenih kilometara, uz uvođenje podrške za rad na mobilnim uređajima.

---

## RELEASE 3 – FIELD OPERATIONS & MONITORING

### Cilj inkrementa
Zatvoriti krug komunikacije između dispečera i terena uz praćenje statusa u realnom vremenu i stabilizaciju osnovnih funkcija.

### Glavne funkcionalnosti
* **PBI-027:** Ažuriranje statusa sandučića (pražnjenje/punjenje) u realnom vremenu
* **PBI-029:** Praćenje statusa rute od strane dispečera (Monitoring)
* **PBI-049:** Historija obilazaka i arhiva ruta
* **PBI-051:** Pretraga i filtriranje sandučića po statusu
* **PBI-052:** Stabilizacija sistema i regresijsko testiranje

### Zavisnosti
* **Release 2:** Funkcionalan modul za proračun i dodjelu ruta.
* **PBI-035:** Test Strategy (definisan proces verifikacije podataka).

### Rizici
* **R-032:** Problemi sa sinhronizacijom (dispečer vidi zastarjele podatke)
* **R-020:** Gubitak mrežne konekcije na terenu (Offline mod)
* **R-021:** Akumulacija bugova uslijed kasnog testiranja

### Okvirni sprintovi
* **Sprint 9:** Logika potvrde zadataka, notifikacije i pretraga.
* **Sprint 10:** Historija, stabilizacija koda i bug fixing.

### Vrijednost
Sistem postaje alat za direktno izvršenje i nadzor, omogućavajući potpunu transparentnost u radu poštara na terenu.

---

## RELEASE 4 – ANALYTICS & FINALIZATION

### Cilj inkrementa
Finalizirati projekt kroz naprednu analitiku performansi, kompletnu dokumentaciju i završnu prezentaciju rješenja.

### Glavne funkcionalnosti
* **PBI-050:** Dashboard sa operativnim izvještajima (statistika efikasnosti)
* **PBI-055:** Korisnička dokumentacija (Uputstvo za rad)
* **PBI-056:** Tehnička dokumentacija (Arhitektura i Deployment)
* **PBI-061:** Završna demonstracija sistema

### Zavisnosti
* **Release 3:** Stabilna baza sa historijskim podacima za analizu.

### Rizici
* **R-007:** Netačna interpretacija podataka u BI izvještajima
* **R-017:** Kašnjenje dokumentacije zbog fokusiranja na finalne bugove

### Okvirni sprintovi
* **Sprint 11:** Razvoj BI dashboarda i statističkih modula.
* **Sprint 12:** Finalizacija tehničkih artefakata i poliranje UI-a.
* **Sprint 13:** Refleksija tima, primopredaja i završna demonstracija.

### Vrijednost
Pretvara operativne podatke u strateške informacije za menadžment i osigurava dugoročnu održivost sistema kroz kompletnu dokumentaciju.

---

## 3. Kriteriji završetka i osiguranje kvaliteta

Svaki release se smatra zvanično isporučenim isključivo ukoliko su ispunjeni sljedeći uslovi:
1. **DoD usklađenost:** Sve stavke inkrementa ispunjavaju kriterije definisane u *Definition of Done*.
2. **QA Verifikacija:** Inkrement je prošao planirana testiranja bez otvorenih bugova visokog prioriteta.
3. **PO Approval:** Product Owner je potvrdio ispunjenje Acceptance Criteria na Sprint Review sastanku.