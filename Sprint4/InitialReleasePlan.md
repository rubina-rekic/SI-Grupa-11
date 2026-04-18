# Initial Release Plan

---

## 1. Svrha dokumenta

Ovaj dokument definiše planiranu strategiju isporuke vrijednosti kroz inkremente tokom AI-enabled faze projekta (Sprintovi 5–13). Plan je zasnovan na Product Backlogu, definisanom MVP opsegu, identifikovanim zavisnostima između funkcionalnosti i procijenjenim kapacitetima tima.

Initial Release Plan nije statičan dokument — evoluirat će u skladu s feedbackom Product Ownera, stvarnim napretkom implementacije i promjenama prioriteta koje mogu nastati tokom semestra. Sve značajne izmjene plana bit će evidentirane u Decision Logu.

---

## 2. Pretpostavke plana

- Tehnički skeleton (PBI-038) je postavljen i funkcionalan do kraja Sprinta 4.
- Tim ima kapacitet od otprilike 5–7 efektivnih radnih dana po sprintu, uzimajući u obzir akademske obaveze (R-004).
- Nearest-neighbor heuristika usvojena je kao algoritam optimizacije ruta (AD-001/AR-002 iz Architecture Overview).
- Frontend i backend se razvijaju paralelno uz API-first pristup (OpenAPI/Swagger specifikacija definisana u Sprintu 4).
- Product Owner je dostupan za pojašnjenja u roku od 48h radnih dana (R-002).
- Svaka stavka mora zadovoljiti Definition of Done (PBI-036) da bi bila smatrana završenom.

---

## 3. Inkrementi i plan isporuke

---

### Inkrement 1 — Autentifikacija i upravljanje pristupom
**Okvirni sprintovi:** Sprint 5
**Status:** Planiran

#### Cilj inkrementa
Uspostaviti sigurnu osnovu sistema kojom se osigurava da samo ovlašteni korisnici mogu pristupiti sistemu, te da svaka korisnička uloga (Administrator, Dispečer, Poštar) vidi isključivo funkcionalnosti i podatke koji su predviđeni za tu ulogu. Bez ovog inkrementa nijedan naredni inkrement ne može biti sigurno isporučen.

#### Obuhvaćene PBI stavke

| PBI ID | Naziv | Kratak opis isporuke |
|--------|-------|----------------------|
| PBI-011 | Kreiranje korisničkog računa poštara | Administrator unosi podatke poštara i kreira račun s inicijalnom lozinkom; sistem validira jedinstvenost emaila i snagu lozinke; poštar prima sistemski email za prvu prijavu |
| PBI-012 | Prijava korisnika | Korisnik se prijavljuje emailom/korisničkim imenom i lozinkom; pri prvoj prijavi obavezna promjena lozinke; neuspjeli pokušaji rezultiraju zaključavanjem računa nakon 5 uzastopnih grešaka |
| PBI-013 | Odjava korisnika | Sigurna odjava uz invalidaciju JWT tokena na serveru; povratak browserom nakon odjave ne otvara zaštićeni sadržaj |
| PBI-014 | Uloge i pristup po ulozi (RBAC) | Svaka uloga (Administrator, Dispečer, Poštar) ima strogo definisan skup dostupnih ruta i UI elemenata; neovlašteni API pozivi vraćaju HTTP 403 |
| PBI-040 | Uspostava Decision Loga | Kreiranje dokumenta za evidenciju tehničkih i arhitektonskih odluka; retroaktivno popunjavanje odluka iz Sprintova 1–4 |
| PBI-041 | Uspostava AI Usage Loga | Kreiranje obrasca za evidenciju korištenja AI alata uz prijavu svrhe, modificiranog i odbačenog sadržaja |

#### Kriterij uspjeha inkrementa
- Sve tri korisničke uloge mogu se prijaviti i odjaviti bez grešaka
- Svaka uloga vidi isključivo predviđene funkcionalnosti (RBAC verifikovan direktnim HTTP zahtjevima za svaku ulogu)
- Lozinke se ne spašavaju u čitljivom obliku (BCrypt hashiranje potvrđeno code reviewom)
- JWT token je invalidiran na serveru nakon odjave
- Security Log (NFR-07) aktivan od prvog dana autentifikacije

#### Zavisnosti
- PBI-038 (tehnički skeleton) mora biti postavljen i funkcionalan — ovo je tvrd preduslov
- API kontrakt (OpenAPI/Swagger) mora biti definisan prije paralelnog razvoja frontenda i backenda

#### Rizici ovog inkrementa

| ID rizika | Opis | Vjerovatnoća | Uticaj | Mjera mitigacije za ovaj inkrement |
|-----------|------|-------------|--------|-------------------------------------|
| R-010 | Sigurnosni propusti u implementaciji autentifikacije i autorizacije | Srednja | Visok | Autorizacija se provjerava i na UI nivou i na API nivou; NFR-01 do NFR-07 tretiraju se kao acceptance kriteriji koji se provjeravaju pri svakom PR-u; BCrypt hashiranje potvrđuje se code reviewom |
| R-026 | Nemogućnost revokacije JWT tokena kompromitira sigurnost sesije | Niska | Visok | Implementirati invalidaciju sesije na serveru od prvog dana; testirati TC-10 i TC-11 iz Test Strategije |
| R-011 | Kašnjenje u postavljanju tehničkog skeletona blokira početak implementacije | Srednja | Visok | PBI-038 ima najviši interni prioritet u Sprintu 4 i radi se prvi; odgovorni Kerim Šikalo i Emrah Žunić s backup planom |
| R-014 | Nedostatak iskustva s .NET backend implementacijom autentifikacije | Srednja | Srednji | Pair programming s iskusnijim članom; za nepoznate tehnologije predvidjeti 20–30% više vremena u procjeni napora |

#### Pretpostavke tima za ovaj inkrement
- Kao primarni identifikator za prijavu koristi se email adresa (otvoreno pitanje iz US-01 — potrebno potvrditi s PO-om)
- Inicijalna lozinka se ručno unosi od strane administratora (otvoreno pitanje iz US-02 — potrebno potvrditi s PO-om)
- Token expiry i refresh strategija bit će definisani kao formalna tehnička odluka u Decision Logu u Sprintu 5

---

### Inkrement 2 — Upravljanje poštarima i poštanskim sandučićima
**Okvirni sprintovi:** Sprint 6
**Status:** Planiran

#### Cilj inkrementa
Izgraditi operativnu osnovu sistema kroz evidenciju i upravljanje ključnim entitetima — poštarima i poštanskim sandučićima. Ovaj inkrement osigurava da dispečeri i administratori imaju kompletne i validne podatke koji su preduslov za generisanje ruta u Inkrementu 4.

#### Obuhvaćene PBI stavke

| PBI ID | Naziv | Kratak opis isporuke |
|--------|-------|----------------------|
| PBI-015 | Dodavanje poštara | Administrator unosi osnovne podatke o poštaru (ime, prezime, ID broj, kontakt); sistem validira jedinstvenost ID broja i sprječava duplikate |
| PBI-016 | Pregled liste poštara | Tabela svih poštara s osnovnim podacima i statusom aktivnosti (aktivan/neaktivan); dostupna samo administratoru i dispečeru |
| PBI-017 | Dodavanje poštanskog sandučića | Unos novog sandučića s GPS koordinatama, tipom i osnovnim podacima; frontend i backend validacija koordinata (Lat ∈ [-90, 90], Lng ∈ [-180, 180]); vizualni prikaz pina na mini-mapi pri unosu |
| PBI-018 | Izmjena podataka o sandučiću | Administrator mijenja lokaciju, tip, prioritet i druge podatke; promjene se reflektuju na budućim generisanjima ruta |
| PBI-019 | Pregled sandučića na listi | Tabelarni pregled svih evidentiranih sandučića s ključnim podacima; dostupan administratoru i dispečeru |

#### Kriterij uspjeha inkrementa
- Sandučić se može kreirati, prikazati, ažurirati i brisati bez grešaka
- GPS koordinate se validiraju na frontendu i backendu (dupla zaštita po R-015)
- Duplikat ID broja poštara je blokiran
- Lista poštara i lista sandučića prikazuju tačne i ažurne podatke

#### Zavisnosti
- Inkrement 1 mora biti završen — RBAC mora biti funkcionalan jer pristup ovim funkcionalnostima ovisi o ulozi korisnika
- Definisana shema baze podataka s odgovarajućim indeksima (R-013) za entitete Poštar i Sandučić

#### Rizici ovog inkrementa

| ID rizika | Opis | Vjerovatnoća | Uticaj | Mjera mitigacije za ovaj inkrement |
|-----------|------|-------------|--------|-------------------------------------|
| R-015 | Nekonzistentni ili nevalidni GPS podaci uzrokuju greške algoritma | Srednja | Srednji | Frontend validacija koordinata obavezna; mini-mapa s vizuelnim prikazom pina pri unosu; backend dodatno validira koordinate pri primanju API zahtjeva; dugme "Odaberi na mapi" kao primarni način unosa |
| R-008 | Zavisnost o OpenStreetMap servisu — nepotpuni podaci za BiH | Srednja | Visok | GPS koordinate sandučića čuvaju se u vlastitoj bazi i nisu zavisne od OSM; sistem prikazuje jasnu poruku korisniku ako mapa nije dostupna; izbor tile provajdera evidentiran u Decision Logu |
| R-009 | Scope creep — proširivanje evidencije sandučića izvan MVP-a | Srednja | Visok | Sve nove stavke idu na dno backlog liste; Product Vision dokument definiše što ne ulazi u MVP |

#### Pretpostavke tima za ovaj inkrement
- Tip sandučića ima predefinisanu listu vrijednosti (potrebno potvrditi s PO-om koji tipovi su relevantni za MVP)
- Brisanje sandučića koji je dio aktivne rute bit će zabranjeno — formalna odluka ide u Decision Log

---

### Inkrement 3 — Prioriteti i radna pravila sandučića
**Okvirni sprintovi:** Sprint 7
**Status:** Planiran

#### Cilj inkrementa
Proširiti evidenciju sandučića s podacima o prioritetima i radnim pravilima koji direktno ulaze kao ulazni parametri u algoritam optimizacije ruta. Kvalitet ovog inkrementa direktno određuje kvalitet generisanih ruta u Inkrementu 4.

#### Obuhvaćene PBI stavke

| PBI ID | Naziv | Kratak opis isporuke |
|--------|-------|----------------------|
| PBI-020 | Definisanje prioriteta sandučića | Administrator postavlja ili mijenja prioritet sandučića (npr. visoki, srednji, niski); prioritet se korektno pohranjuje i uvažava u algoritmu |
| PBI-021 | Evidencija radnih pravila sandučića | Čuvanje radnih dana i vremenskih okvira dostupnosti za svaki sandučić; neispravni vremenski okviri su blokirani validacijom |

#### Kriterij uspjeha inkrementa
- Prioritet i radna pravila se korektno pohranjuju u bazu
- Algoritam u Inkrementu 4 uvažava prioritete i radna pravila pri generisanju rute (provjera unit testovima po R-031)
- Sandučić s postavljenim radnim pravilima nije uključen u rutu izvan definisanog vremenskog okvira

#### Zavisnosti
- Inkrement 2 mora biti završen — entitet Sandučić mora postojati u bazi
- Definisana struktura prioritetnog ponderisanja za nearest-neighbor algoritam (arhitektonska odluka iz R-001)

#### Rizici ovog inkrementa

| ID rizika | Opis | Vjerovatnoća | Uticaj | Mjera mitigacije za ovaj inkrement |
|-----------|------|-------------|--------|-------------------------------------|
| R-001 | Pogrešna procjena napora — radna pravila povećavaju kompleksnost algoritma | Srednja | Visok | Radna pravila implementirati kao izolovani modul iza `IRouteOptimizationService` interfejsa; PoC algoritma planiran u Sprintu 5 daje realnu osnovu za procjenu napora |
| R-031 | Algoritam ne uvažava prioritete i radna pravila | Srednja | Visok | Pokriti unit testovima sve kombinacije prioriteta i radnih dana; rubni slučajevi (sandučić nedostupan tog dana, isti prioritet više sandučića) moraju biti eksplicitno testirani |
| R-002 | Nejasna pravila prioritizacije zahtijevaju pojašnjenje PO-a | Srednja | Visok | Tim definira privremenu pretpostavku (visoki prioritet = uvijek prvi u ruti) i evidentira je u Decision Logu; pojašnjenje zatražiti od PO-a na reviewu Sprinta 6 |

---

### Inkrement 4 — Generisanje i upravljanje rutama
**Okvirni sprintovi:** Sprint 8
**Status:** Planiran

#### Cilj inkrementa
Implementirati centralnu vrijednost sistema — automatsko generisanje optimizovanih dnevnih ruta za poštare, uz mogućnost dodjele, pregleda i ručnog prilagođavanja. Ovo je tehnički najkompleksniji inkrement i direktno isporučuje primarnu poslovnu vrijednost sistema.

#### Obuhvaćene PBI stavke

| PBI ID | Naziv | Kratak opis isporuke |
|--------|-------|----------------------|
| PBI-022 | Generisanje dnevne rute | Automatsko generisanje rute za odabranog poštara primjenom nearest-neighbor heuristike; uvažavaju se prioriteti sandučića, radna pravila i vremenski okviri dostupnosti; loader indikator tokom proračuna (max 10s — NFR-12) |
| PBI-023 | Dodjela rute poštaru | Dispečer dodjeljuje generisanu rutu konkretnom poštaru; sistem blokira dodjelu poštaru koji već ima aktivnu rutu |
| PBI-024 | Pregled detalja rute | Prikaz redoslijeda obilaska s listom sandučića, procijenjenom kilometražom i ETA; ruta vidljiva na mapi |
| PBI-025 | Ručna izmjena redoslijeda obilaska | Dispečer može drag-and-drop metodom promijeniti redoslijed sandučića; numeracija i sumarni podaci (km, ETA) se odmah ažuriraju; napuštanje stranice s nespremljenim izmjenama prikazuje upozoravajući modal |

#### Kriterij uspjeha inkrementa
- Generisana ruta uvažava prioritete sandučića i radna pravila iz Inkrementa 3
- Ruta se ispravno dodjeljuje poštaru; poštaru koji ima aktivnu rutu nije moguće dodijeliti novu
- Ručne izmjene redoslijeda se spašavaju i odražavaju na prikaz rute
- Performansni cilj generisanja rute: ≤ 10 sekundi za standardni skup sandučića (NFR-12)

#### Zavisnosti
- Inkrement 3 mora biti završen — prioriteti i radna pravila moraju biti dostupni kao ulaz za algoritam
- `IRouteOptimizationService` interfejs mora biti definisan i testiran unit testovima prije integracije
- PoC algoritma iz Sprinta 5 mora potvrditi izvodivost nearest-neighbor pristupa

#### Rizici ovog inkrementa

| ID rizika | Opis | Vjerovatnoća | Uticaj | Mjera mitigacije za ovaj inkrement |
|-----------|------|-------------|--------|-------------------------------------|
| R-001 | Algoritam optimizacije podcijenjen po naporu; nearest-neighbor daje suboptimalne rute za > 50 sandučića | Srednja | Visok | Algoritam izolovan iza `IRouteOptimizationService` interfejsa — zamjena ne zahtijeva refaktoring ostatka sistema; napredniji algoritmi (Dijkstra, OR-Tools) ostaju kao post-MVP opcija (OQ-005) |
| R-012 | Integracija frontend i backend komponenti za prikaz rute uzrokuje nekompatibilnosti | Srednja | Srednji | API kontrakt definisan u Sprintu 4; API-first pristup — backend piše Swagger dokumentaciju, frontend koristi mock podatke dok pravi endpointi nisu gotovi |
| R-013 | Performanse degradiraju s povećanjem broja sandučića | Niska | Srednji | Baza podataka indeksirana na relevantnim stupcima; performansno testiranje u Sprintu 11 (PBI-052) |

#### Pretpostavke tima za ovaj inkrement
- Udaljenost između tačaka računa se Haversine formulom nad GPS koordinatama (ne Euklidskom)
- Maksimalan broj sandučića po ruti koji je podržan u MVP-u — potrebno definisati s PO-om i evidentirati u Decision Logu

---

### Inkrement 5 — Mobilni prikaz i operativno praćenje
**Okvirni sprintovi:** Sprint 9
**Status:** Planiran

#### Cilj inkrementa
Omogućiti poštarima operativni rad na terenu kroz mobilni prikaz dodijeljene rute i ažuriranje statusa sandučića, te dispečerima praćenje napretka realizacije u realnom vremenu. Ovaj inkrement zatvara operativni krug sistema — od generisanja rute do evidencije realizacije na terenu.

#### Obuhvaćene PBI stavke

| PBI ID | Naziv | Kratak opis isporuke |
|--------|-------|----------------------|
| PBI-026 | Mobilni prikaz dodijeljene rute | Poštar pregledava svoju dodijeljenu rutu putem responzivnog web interfejsa; UI funkcionalan na minimalnoj širini ekrana od 360px (NFR-08); poštar bez dodijeljene rute vidi informativnu poruku |
| PBI-027 | Ažuriranje statusa sandučića | Poštar postavlja status stavke rute (`Realizovano` uz tip `Ispražnjen`/`Napunjen`, `Preskočeno`); serverski timestamp i GPS koordinate se zapisuju; realizovana stavka ne može se ponovo mijenjati |
| PBI-028 | Označavanje nedostupne lokacije | Poštar evidentira nedostupnost lokacije uz obaveznu napomenu minimalne dužine 10 karaktera; pohrana je spriječena bez napomene |
| PBI-029 | Praćenje statusa rute od strane dispečera | Dispečer vidi progres svake aktivne rute s procentualnim indikatorom i sumarnom statistikom po statusima; promjene statusa vidljive unutar polling intervala (≤ 30s) bez ručnog osvježavanja |
| PBI-030 | Osnovni dnevni izvještaj | Generisanje dnevnog izvještaja o realizovanim i nerealizovanim obilascima; dostupan isključivo ovlaštenim ulogama; preuzimanje u PDF formatu jednim klikom |

#### Kriterij uspjeha inkrementa
- Poštar vidi isključivo svoju dodijeljenu rutu (ne tuđe)
- Promjena statusa sandučića od strane poštara vidljiva je dispečeru unutar polling intervala ≤ 30 sekundi (TC-33)
- Mobilni interfejs funkcionalan na uređaju od 360px širine bez horizontalnog scrollanja (TC-31, NFR-08)
- Nedostupna lokacija se ne može evidentirati bez napomene (TC-34)
- Kada su sve stavke rute obrađene, ruta automatski prelazi u status `Završena`

#### Zavisnosti
- Inkrement 4 mora biti završen — rute moraju biti generirane i dodijeljene
- Polling interval (15–30s) mora biti definisan kao formalna arhitektonska odluka u Decision Logu

#### Rizici ovog inkrementa

| ID rizika | Opis | Vjerovatnoća | Uticaj | Mjera mitigacije za ovaj inkrement |
|-----------|------|-------------|--------|-------------------------------------|
| R-007 | Tehnička kompleksnost responzivnog mobilnog interfejsa | Srednja | Visok | Mobile-first pristup dizajnu; testiranje na stvarnom uređaju obavezno za svaki PBI vezan za mobilni prikaz; kritična dugmad za promjenu statusa provjeravaju Faruk Avdagić i Nejla Karalić kao dio DoD (NFR-09: max 3 interakcije) |
| R-032 | Sinhronizacija poštara i dispečera ne funkcioniše — dispečer vidi zastarjele statuse | Srednja | Visok | Integracioni test s istovremenim sesijama poštara i dispečera; automatsko osvježavanje unutar polling intervala (≤ 30s) verificirano TC-33 |

---

### Inkrement 6 — Historija, izvještavanje i pretraga
**Okvirni sprintovi:** Sprint 10
**Status:** Planiran

#### Cilj inkrementa
Proširiti sistem s alatima koji povećavaju operativnu preglednost i upotrebljivost za administratore i dispečere — historija obilazaka, napredniji izvještaji i pretraga/filtriranje sandučića.

#### Obuhvaćene PBI stavke

| PBI ID | Naziv | Kratak opis isporuke |
|--------|-------|----------------------|
| PBI-049 | Historija obilazaka i arhiva ruta | Pregled svih realizovanih ruta i obilazaka po datumu i poštaru; pristup dostupan administratoru i dispečeru |
| PBI-050 | Prošireno operativno izvještavanje | Izvještaji po poštaru, periodu i tipu sandučića; nadogradnja PBI-030 s filtriranjem po datumskom opsegu |
| PBI-051 | Pretraga i filtriranje sandučića | Pretraga i filtriranje liste sandučića po lokaciji, tipu, prioritetu i statusu |

#### Kriterij uspjeha inkrementa
- Historija ruta prikazuje tačne podatke za svaki realizovani obilazak
- Izvještaji su filtrirani po ovlastima uloge — Poštar ne može vidjeti tuđe podatke
- Pretraga i filtriranje sandučića vraćaju relevantne rezultate bez performansnih problema

#### Zavisnosti
- Inkrementi 4 i 5 moraju biti završeni — podaci o rutama i obilascima moraju biti evidentirani u bazi
- Baza podataka mora imati indekse na stupcima koji se koriste u historijskim upitima (SandučićID, PoštarID, Datum, Status)

#### Rizici ovog inkrementa

| ID rizika | Opis | Vjerovatnoća | Uticaj | Mjera mitigacije za ovaj inkrement |
|-----------|------|-------------|--------|-------------------------------------|
| R-013 | Količina historijskih podataka utiče na performanse upita | Niska | Srednji | Odgovarajući indeksi na stupcima koji se koriste u filterima; performansni test u Sprintu 11 (PBI-052) |
| R-009 | Zahtjevi za izvještavanjem proširuju se izvan MVP-a | Srednja | Visok | Sve nove stavke idu na dno backlog liste; Product Vision definiše granicu MVP izvještavanja |

---

### Inkrement 7 — Stabilizacija i regresijsko testiranje
**Okvirni sprintovi:** Sprint 11
**Status:** Planiran

#### Cilj inkrementa
Stabilizovati sistem kroz sistemsko regresijsko testiranje svih ključnih funkcionalnosti, otklanjanje pronađenih grešaka i pripremu tehničke osnove za finalnu isporuku. Na kraju ovog inkrementa sistem mora biti stabilan i demonstrabilan.

#### Obuhvaćene PBI stavke

| PBI ID | Naziv | Kratak opis isporuke |
|--------|-------|----------------------|
| PBI-052 | Stabilizacija sistema i regresijsko testiranje | Regresijsko testiranje svih ključnih funkcionalnosti; otklanjanje pronađenih grešaka; lista poznatih ograničenja i tehničkog duga; plan završne demonstracije |

#### Kriterij uspjeha inkrementa
- Svi kritični i visoko-prioritetni defekti su zatvoreni
- Regresijsko testiranje pokriva sve ključne korisničke tokove (autentifikacija, generisanje rute, terenski rad, izvještavanje)
- Performansni test s 50 sandučića pokazuje generisanje rute unutar 10 sekundi (NFR-12)
- Lista poznatih ograničenja i tehničkog duga je dokumentovana i transparentna

#### Zavisnosti
- Svi prethodni inkrementi moraju biti završeni
- Test evidencija iz Sprintova 5–10 mora biti ažurna i dostupna za pregled

#### Rizici ovog inkrementa

| ID rizika | Opis | Vjerovatnoća | Uticaj | Mjera mitigacije za ovaj inkrement |
|-----------|------|-------------|--------|-------------------------------------|
| R-004 | Ispitni rok u periodu Sprinta 11 smanjuje kapacitet tima | Visoka | Visok | Kritične stavke stabilizacije ne smiju biti planirane istovremeno s ispitnim rokom; svaki član na početku sprinta prijavljuje periode nedostupnosti |
| R-006 | Bug fixing neproporcionalno pada na jednu osobu | Visoka | Srednji | Defekti raspoređuju se putem JIRA-e s primarnom i sekundarnom odgovornom osobom; verifikacija napretka dan prije isteka sprinta |

---

### Inkrement 8 — Finalna verzija i kompletna dokumentacija
**Okvirni sprintovi:** Sprint 12
**Status:** Planiran

#### Cilj inkrementa
Isporučiti finalnu verziju sistema zajedno s kompletnom korisničkom i tehničkom dokumentacijom, Release Notes-ima i ažuriranim artefaktima. Na kraju ovog inkrementa sistem je spreman za završnu demonstraciju i formalnu predaju.

#### Obuhvaćene PBI stavke

| PBI ID | Naziv | Kratak opis isporuke |
|--------|-------|----------------------|
| PBI-053 | Finalni inkrement i bug fixing | Završne korekcije koda, UI detalja i rubnih scenarija na osnovu feedbacka iz Sprinta 11; finalni kandidat za demonstraciju |
| PBI-054 | Release Notes | Opis isporučenih funkcionalnosti, poznatih limitacija i načina korištenja finalne verzije |
| PBI-055 | Korisnička dokumentacija | Opis sistema, korisničkih uloga, osnovnog načina korištenja i ograničenja |
| PBI-056 | Tehnička dokumentacija | Pregled arhitekture, glavnih komponenti, logike sistema, napomene za pokretanje i poznati tehnički dug |
| PBI-057 | Ažuriranje svih logova i artefakata | Product Backlog, Decision Log, AI Usage Log i svi ostali obavezni artefakti potpuni i ažurni za finalnu predaju |

#### Kriterij uspjeha inkrementa
- Sistem je demonstrabilan bez kritičnih grešaka
- Korisnička dokumentacija pokriva sve tri korisničke uloge i njihove ključne tokove korištenja
- Tehnička dokumentacija opisuje arhitekturu u skladu s Architecture Overview dokumentom
- Svi logovi i artefakti su kompletni i međusobno konzistentni

#### Zavisnosti
- Inkrement 7 mora biti završen — stabilan sistem je preduslov za finalnu dokumentaciju
- Svi obavezni artefakti iz faze 1 moraju biti ažurni i konzistentni s isporučenim sistemom

#### Rizici ovog inkrementa

| ID rizika | Opis | Vjerovatnoća | Uticaj | Mjera mitigacije za ovaj inkrement |
|-----------|------|-------------|--------|-------------------------------------|
| R-004 | Nedovoljno vremena za kvalitetnu dokumentaciju ako se bug fixing prolongira | Visoka | Visok | Dokumentacija se piše paralelno s razvojem kroz semestar; Rubina Rekić i Aldin Bulbul vode dokumentaciju kontinuirano |
| R-016 | Nekompletni logovi zbog odluka koje nisu pravovremeno evidentirane | Srednja | Srednji | Decision Log se ažurira unutar 24h od svake odluke; koordinator čita recap prethodnih odluka na početku svakog Meet-a |

---

### Inkrement 9 — Završna demonstracija i individualna refleksija
**Okvirni sprintovi:** Sprint 13
**Status:** Planiran

#### Cilj inkrementa
Prezentovati završni sistem, predati individualnu refleksiju i peer evaluation, te završiti semestar s formalnim dokazima o individualnom i timskom angažmanu.

#### Obuhvaćene PBI stavke

| PBI ID | Naziv | Kratak opis isporuke |
|--------|-------|----------------------|
| PBI-058 | Individualna refleksija | Svaki član tima priprema refleksiju o doprinosu, lekcijama, izazovima i razlici između human-first i AI-enabled faze |
| PBI-059 | Peer evaluation | Svaki član tima ispunjava peer evaluation obrazac za ostale članove |
| PBI-060 | Završni refleksivni izvještaj tima | Tim zajedno priprema izvještaj koji sumira razvoj, ključne odluke, izazove i naučene lekcije |
| PBI-061 | Završna demonstracija | Prezentacija gotovog sistema: problem, ključne funkcionalnosti, evolucija zahtjeva, arhitektura, pristup testiranju, korištenje AI alata, preostala ograničenja |

#### Kriterij uspjeha inkrementa
- Demonstracija pokriva sve zahtijevane oblasti iz Vodiča za projektni rad (sekcija 11)
- Svaki član tima može individualno objasniti arhitekturu, backlog, testove i vlastiti doprinos
- Peer evaluation je iskren, konkretan i profesionalan za svakog člana tima

#### Zavisnosti
- Inkrement 8 mora biti završen — finalna verzija sistema i dokumentacija moraju biti predane

#### Rizici ovog inkrementa

| ID rizika | Opis | Vjerovatnoća | Uticaj | Mjera mitigacije za ovaj inkrement |
|-----------|------|-------------|--------|-------------------------------------|
| R-006 | Član koji je manje doprinosio ne može smisleno opisati vlastiti doprinos | Visoka | Srednji | Evidencija doprinosa vođena tokom cijelog semestra (JIRA, Decision Log, commit historija) daje osnovu za autentičnu refleksiju |
| R-004 | Preklapanje završnih akademskih obaveza s pripremom demonstracije | Visoka | Visok | Demonstracija se uvježbava u Sprintu 12 kao dio pripreme — ne prvi put u Sprintu 13 |

---

## 4. Vizuelni pregled plana po sprintovima

| Sprint | Inkrement | Ključne isporuke |
|--------|-----------|-----------------|
| Sprint 5 | Inkrement 1 | Autentifikacija, RBAC, Decision Log, AI Usage Log |
| Sprint 6 | Inkrement 2 | Upravljanje poštarima i sandučićima |
| Sprint 7 | Inkrement 3 | Prioriteti i radna pravila sandučića |
| Sprint 8 | Inkrement 4 | Generisanje i upravljanje rutama |
| Sprint 9 | Inkrement 5 | Mobilni prikaz, ažuriranje statusa, praćenje i izvještaj |
| Sprint 10 | Inkrement 6 | Historija, prošireno izvještavanje, pretraga |
| Sprint 11 | Inkrement 7 | Stabilizacija i regresijsko testiranje |
| Sprint 12 | Inkrement 8 | Finalna verzija, dokumentacija, ažuriranje artefakata |
| Sprint 13 | Inkrement 9 | Završna demonstracija, refleksija, peer evaluation |

---

Ovaj plan je okvirni i evoluirat će tokom semestra. Sve promjene prioriteta, scopea ili redosljeda inkremenata bit će evidentirane u Decision Logu s razlogom izmjene, odražene u ažuriranom Product Backlogu i usklađene s feedbackom Product Ownera s aktuelnog sprint reviewa.

Tim ne tretira ovaj dokument kao fiksirani plan, nego kao živi alat za upravljanje isporukom vrijednosti kroz semestar.
