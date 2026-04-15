# Test Strategija
## Sistem za optimizaciju ruta punjenja i pražnjenja poštanskih sandučića

---

# Cilj testiranja

Cilj testiranja sistema za optimizaciju ruta poštanskih sandučića je osigurati da sve funkcionalnosti sistema rade ispravno, pouzdano i sigurno, u skladu sa definisanim zahtjevima i acceptance kriterijima. Poseban fokus stavlja se na ispravnost optimizacijskog algoritma, integritet toka podataka od dispečera do poštara na terenu, te sigurnost rolno-baziranog pristupa za sve tri korisničke uloge (Administrator, Dispečer, Poštar).

| Cilj | Obim | Kriterij uspjeha |
|------|------|-----------------|
| Verifikacija ispravnosti toka autentifikacije i sigurnosti korisničke sesije (PBI-012, PBI-013) | Prijava i odjava za sve tri korisničke uloge (Administrator, Dispečer, Poštar); zaključavanje računa pri višestrukim neuspjelim pokušajima; obavezna promjena inicijalne lozinke pri prvoj prijavi | Svi AC iz PBI-012 i PBI-013 zadovoljeni; neautoriziran pristup zaštićenim rutama je onemogućen; povratak browserom nakon odjave ne otvara zaštićeni sadržaj; račun se zaključava nakon 5 uzastopnih neuspjelih pokušaja |
| Provjera ispravnosti rolno-baziranog pristupa (RBAC) za sve korisničke uloge (PBI-014) | Pristup rutama, UI elementima i API endpointima za uloge: Administrator, Dispečer, Poštar; direktan URL pristup kao neovlašteni korisnik | Svaka uloga vidi isključivo funkcionalnosti i podatke predviđene za tu ulogu; HTTP 403 ili redirect za neovlaštene zahtjeve; Poštar ne može pristupiti dispečerskom dashboardu ni administratorskom panelu |
| Validacija upravljanja korisničkim računima i podacima o poštarima (PBI-011, PBI-015, PBI-016) | Administratorsko kreiranje računa; validacija jedinstvenosti emaila i snage lozinke; dodavanje i pregled poštara; provjera duplikata ID broja | Račun se kreira isključivo s validnim podacima; duplikat emaila i ID broja je blokiran; lozinka se ne sprema u čitljivom obliku; kreiran poštar je odmah vidljiv u listi |
| Validacija upravljanja poštanskim sandučićima (PBI-017, PBI-018, PBI-019, PBI-020, PBI-021) | Dodavanje, izmjena i pregled sandučića; definisanje prioriteta; evidencija vremenskih okvira dostupnosti i radnih dana | Sandučić se kreira, prikazuje, ažurira i briše ispravno; prioritet i radna pravila se korektno pohranjuju i odražavaju na algoritam planiranja; neispravni GPS format je blokiran |
| Provjera ispravnosti generisanja i upravljanja rutama (PBI-022, PBI-023, PBI-024, PBI-025) | Automatsko generisanje dnevne rute primjenom nearest-neighbor heuristike; dodjela rute poštaru; pregled detalja rute; ručna izmjena redoslijeda obilaska | Generisana ruta uvažava prioritete sandučića i radna pravila; ruta se ispravno dodjeljuje poštaru; ručne izmjene redoslijeda se čuvaju; dispečer ne može dodijeliti rutu nepostojećem poštaru |
| Validacija mobilnog prikaza i terenskih operacija poštara (PBI-026, PBI-027, PBI-028) | Pregled dodijeljene rute na mobilnom uređaju; ažuriranje statusa sandučića (ispražnjen / napunjen / nedostupan / preskočen); evidentiranje nedostupnih lokacija | Poštar vidi isključivo svoju dodijeljenu rutu; promjena statusa se odmah reflektuje kod dispečera; nedostupna lokacija se bilježi s odgovarajućom napomenom; UI je funkcionalan na mobilnom prikazu |
| Provjera praćenja realizacije i izvještavanja (PBI-029, PBI-030, PBI-049, PBI-050) | Dispečerski pregled statusa obilazaka uz periodično ažuriranje (HTTP polling, interval 15–30s); generisanje dnevnih izvještaja; pretraga i filtriranje podataka | Dispečer vidi progres rute uz kašnjenje unutar definisanog polling intervala (≤ 30s); izvještaji sadrže tačne podatke o realizaciji; pretraga i filtriranje vraćaju relevantne rezultate; izvještaj je dostupan isključivo ovlaštenim ulogama |

---

# Nivoi testiranja

## 1. Unit testiranje

Unit testiranje se provodi na nivou pojedinačnih komponenti, funkcija i klasa sistema, izolovano od ostalih dijelova. Svaka jedinica koda testira se nezavisno korištenjem mock objekata tamo gdje postoje zavisnosti prema vanjskim servisima ili bazi podataka.

Obuhvata testiranje:

* Validacione logike za kreiranje korisničkih računa (format emaila, jedinstvenost emaila/korisničkog imena, jačina lozinke, obavezna polja)
* Logike hashiranja i provjere lozinke (bcrypt)
* Business logike za generisanje JWT tokena i provjeru `isForcePasswordChange` flaga
* Logike provjere duplikata (email, korisničko ime, ID broja poštara)
* Validacije podataka o sandučiću (format GPS koordinata, obavezna polja, ispravnost vremenskih okvira dostupnosti)
* Business logike optimizacijskog modula (`IRouteOptimizationService`) — ispravnost nearest-neighbor heuristike, uvažavanje prioriteta sandučića, poštivanje radnih pravila i vremenskih okvira
* Logike za izračun i ažuriranje statusa obilaska sandučića (ispražnjen / napunjen / nedostupan / preskočen)
* Validacije ručnih izmjena redoslijeda rute (zabrana nedozvoljenih operacija, provjera integriteta sekvence)
* Funkcija za generisanje dnevnih izvještaja i agregaciju podataka o realizaciji

**Odgovorne osobe:** Razvojni tim

**Izlazni kriterij:**

* 100% unit testova prolazi (0 failova)
* Pokrivenost koda (code coverage) ≥ 90% za business logiku i optimizacijski modul
* Svi granični slučajevi (edge cases) za validaciju i algoritam rutiranja pokriveni testovima

---

## 2. Integraciono testiranje

Integraciono testiranje provjerava ispravnu komunikaciju između modula sistema te korektnost toka podataka između komponenti — konkretno između React frontenda, ASP.NET Core Web API backenda i PostgreSQL baze podataka.

Ključne integracije koje se testiraju:

* Integracija autentifikacije i autorizacije — provjera da JWT middleware ispravno ograničava pristup svim zaštićenim endpointima za svaku od tri uloge
* Integracija korisničkog modula i baze podataka — provjera da se korisnički podaci i uloge ispravno pohranjuju, čitaju i validiraju kroz Entity Framework Core
* Integracija modula sandučića i optimizacijskog servisa — provjera da prioriteti, koordinate i radna pravila sandučića ispravno ulaze u algoritam i da rezultat odgovara očekivanom redoslijedu obilaska
* Integracija dispečerskog modula i modula ruta — provjera da se generisana ruta ispravno dodjeljuje poštaru i da je poštar može dohvatiti putem svog API endpointa
* Integracija terenskog modula i baze podataka — provjera da promjena statusa sandučića od strane poštara u realnom vremenu ažurira zapise u tabeli `RouteStops` i da je taj status vidljiv dispečeru
* Integracija modula izvještaja sa bazom podataka — provjera da agregacijski upiti vraćaju tačne podatke o realizaciji ruta i statusima sandučića
* Integracija autorizacijskog podsistema (JWT + ASP.NET Identity) s rolama — provjera da RBAC provjere na API nivou odgovaraju ulogama definisanim u bazi

**Odgovorne osobe:** Razvojni tim i QA

**Izlazni kriterij:**

* Svi kritični integracioni tokovi prolaze bez grešaka
* Nema otvorenih defekata vezanih za komunikaciju između modula
* API response time unutar definisanog praga (< 3 sekunde za CRUD operacije i < 5 sekundi za generisanje rute)
* Svi API endpointi vraćaju ispravne HTTP statuse (200, 201, 400, 401, 403, 404, 500)

---

## 3. Sistemsko testiranje

Sistemsko testiranje provjerava ponašanje cjelokupnog sistema kao integrisane cjeline, uključujući end-to-end tokove od korisničkog interfejsa do baze podataka, te validaciju ne-funkcionalnih zahtjeva.

Obuhvata:

* End-to-end testiranje kompletnog dispečerskog toka: prijava → pregled sandučića → generisanje rute → pregled i ručna korekcija → dodjela poštaru → praćenje realizacije → generisanje izvještaja
* End-to-end testiranje terenskog toka poštara: prijava → pregled dodijeljene rute na mobilnom uređaju → ažuriranje statusa svakog sandučića → evidentiranje nedostupnih lokacija
* Testiranje sigurnosti: provjera da neautorizovani korisnici ne mogu pristupiti zaštićenim resursima direktnim URL-om ili manipulacijom HTTP zahtjeva; provjera da se JWT token ne pohranjuje u localStorage radi zaštite od XSS napada
* Testiranje integriteta podataka: provjera konzistentnosti podataka između tabela `Routes`, `RouteStops` i `Postboxes` nakon kreiranja, izmjene i brisanja entiteta
* UI testiranje responzivnosti: provjera da mobilni prikaz za poštara (PBI-026) ispravno funkcioniše na različitim veličinama ekrana
* Testiranje rubnih slučajeva algoritma: generisanje rute za maksimalan broj sandučića (~50), ruta sa sandučićima koji imaju restriktivna radna pravila, ruta pri kojoj ni jedan sandučić nije dostupan
* Testiranje performansi: pregled liste sandučića i listi ruta sa većim brojem zapisa

**Odgovorne osobe:** QA

**Izlazni kriterij:**

* Svi end-to-end scenariji prolaze bez grešaka
* Svi sigurnosni testovi prolaze (neovlašteni pristup blokiran u 100% slučajeva)
* Mobilni prikaz ispravno prikazuje rutu i omogućava ažuriranje statusa na uređajima s ekranom manjim od 480px
* Konzistentnost podataka potvrđena nakon svih CRUD operacija

---

## 4. Prihvatno testiranje

Prihvatno testiranje se provodi na osnovu acceptance kriterija definisanih u user stories za svaki PBI. Cilj je potvrditi da sistem u cijelosti ispunjava poslovne zahtjeve i da je spreman za puštanje u produkciju.

Ovo testiranje obuhvata verifikaciju svakog acceptance kriterija za sve user storije, uz aktivno sudjelovanje vlasnika proizvoda te predstavnika krajnjih korisničkih uloga (Administrator, Dispečer, Poštar) u finalnom pregledu funkcionalnosti.

**Odgovorne osobe:** Product Owner, krajnji korisnici (Administrator, Dispečer, Poštar), QA

**Izlazni kriterij:**

* Više od 90% acceptance kriterija verifikovano kao zadovoljeno
* Nema otvorenih Critical ili High defekata
* Product Owner dao formalno odobrenje za puštanje u produkciju
* Sva otvorena pitanja iz user storija riješena ili svjesno odložena uz dokumentovano obrazloženje

---
 
## 5. Regresiono testiranje
 
Regresiono testiranje se provodi nakon svake izmjene koda (bugfix, nova funkcionalnost, refactoring) kako bi se osiguralo da već verificirane funkcionalnosti i dalje rade ispravno. Posebno je važno u kontekstu ovog sistema jer promjene u optimizacijskom modulu, RBAC logici ili real-time komunikaciji mogu imati neočekivane nuspojave na ostatak sistema.
 
Regresiono testiranje obuhvata:
 
* Ponovno izvršavanje svih automatizovanih unit testova nakon svake izmjene poslovne logike
* Ponovno izvršavanje integracijskih testova za sve module koji su direktno ili indirektno zahvaćeni izmjenom
* Smoke testiranje ključnih end-to-end tokova (prijava, generisanje rute, ažuriranje statusa, generisanje izvještaja) nakon svakog deploya na testno okruženje
* Punu regresijsku suite jednom sedmično ili prije svakog release kandidata
* Provjeru da bugfix za jedan defekt nije uzrokovao novi defekt u istom ili susjednom modulu
Prioritet regresionog testiranja po modulu:
 
| Prioritet | Modul | Obrazloženje |
|-----------|-------|--------------|
| Kritičan | Autentifikacija i RBAC (PBI-012, PBI-013, PBI-014) | Sigurnosni propusti imaju visok rizik; svaka izmjena mora biti regresiono testirana |
| Kritičan | Optimizacijski algoritam i generisanje rute (PBI-022) | Promjene u nearest-neighbor heuristici ili logici prioriteta mogu poremetiti čitav tok operacija |
| Visok | Ažuriranje statusa sandučića i real-time sinhronizacija (PBI-027, PBI-028, PBI-029) | Promjene u ovom modulu direktno utječu na terenski rad poštara i dispečerski pregled |
| Visok | Dodjela rute poštaru (PBI-023) | Greške u dodjeli mogu ostaviti poštara bez rute ili dodijeliti rutu pogrešnoj osobi |
| Srednji | Upravljanje sandučićima i radnim pravilima (PBI-017–PBI-021) | Izmjene validacije ili modela podataka mogu utjecati na ispravnost ulaznih podataka za algoritam |
| Srednji | Generisanje izvještaja (PBI-030) | Agregatni upiti su podložni greškama pri izmjenama sheme baze podataka |
| Nizak | Pregled lista i UI prikazi (PBI-016, PBI-019, PBI-024) | Manji vizuelni propusti ne blokiraju poslovni tok |
 
**Okidači za pokretanje regresionog testiranja:**
 
* Svaki merge u `main` / `develop` granu putem pull requesta
* Svaki bugfix koji je zatvorio defekt s prioritetom Critical ili High
* Refactoring koji se tiče zajedničkih servisa (npr. `IRouteOptimizationService`, JWT middleware, Entity Framework Core repozitorija)
* Izmjene baze podataka (migracije, promjene sheme)
* Priprema release kandidata za produkciju
  
**Odgovorne osobe:** QA (automatizovano), razvojni tim (smoke testiranje nakon deploya)
 
**Izlazni kriterij:**
 
* 100% automatizovanih testova prolazi nakon izmjene
* Svi kritični i visokoprioriteti smoke testovi prolaze
* Nema novih regresijskih defekata uvedenih izmjenom
* Svaki regresijski defekt koji se pojavi mora biti zabilježen, reproduciran i prioritiziran prije sljedećeg deploya
---

# Šta se testira u kojem nivou

| Funkcionalnost | Unit testiranje | Integraciono | Sistemsko | Prihvatno | Regresiono |
|---|---|---|---|---|---|
| **PBI-011 Kreiranje korisničkog računa poštara** | DA – validacija obaveznih polja, format emaila, jedinstvenost emaila, pravila jačine lozinke, hashiranje lozinke | DA – POST /api/users, upis u bazu, provjera duplikata na DB nivou, slanje email obavijesti | DA – forma → zelena potvrda → korisnik vidljiv u listi; blokada duplikata; dugme onemogućeno tokom obrade | DA – administrator kreira račun za poštara s ispravnim i neispravnim podacima | SREDNJI – ponovo testirati nakon izmjena validacijske logike ili modela korisnika |
| **PBI-012 Prijava korisnika** | DA – autentifikacija kredencijala, bcrypt hash provjera, generisanje JWT tokena, provjera `isForcePasswordChange` flaga | DA – POST /api/auth/login, JWT middleware, redirect na ekran za promjenu lozinke pri prvoj prijavi | DA – prijava → redirect na Dashboard; pogrešni kredencijali → generička poruka; zaključavanje nakon 5 pokušaja | DA – sve tri uloge se uspješno prijavljuju; obavezna promjena inicijalne lozinke potvrđena | KRITIČAN – obavezno regresiono testirati nakon svake izmjene JWT logike, middleware-a ili autentifikacijskog toka |
| **PBI-013 Odjava korisnika** | NE | DA – invalidacija sesije na serveru, provjera da zaštićeni endpointi odbijaju token nakon odjave | DA – odjava → redirect na Login; povratak browserom → blokada; direktan URL → redirect | DA – korisnik se odjavljuje; zaštićeni sadržaj nije dostupan nakon odjave | KRITIČAN – obavezno regresiono testirati uz PBI-012 nakon izmjena sesijskog upravljanja |
| **PBI-014 Uloge i pristup (RBAC)** | DA – logika provjere uloge za svaki zaštićeni endpoint, RBAC middleware | DA – autorizacijski middleware na svakim zaštićenim API endpointima; provjera upisivanja Role ID-a u bazu | DA – Poštar pokušava pristupiti admin URL-ovima → odbijen + toast; role-specific dashboard per ulozi | DA – svaka uloga vidi isključivo sebi predviđene funkcionalnosti | KRITIČAN – svaka izmjena rola, permisija ili autorizacijske logike zahtijeva punu regresijsku provjeru RBAC-a |
| **PBI-015 Dodavanje poštara** | DA – validacija obaveznih polja, format telefonskog broja, jedinstvenost Internog ID-a | DA – POST /api/postari, referentni integritet u bazi, zabrana duplikata na serverskoj strani | DA – unos ispravnih podataka → poštar u listi; dupli ID → poruka s linkom na postojećeg | DA – administrator dodaje poštara; poštar postaje dostupan za dodjelu rute | SREDNJI – ponovo testirati nakon izmjena modela poštara ili validacijskih pravila |
| **PBI-016 Pregled liste poštara** | NE | DA – GET /api/postari, straničenje, provjera autorizacije (Admin/Dispečer) | DA – lista se učitava sa statusima; sortiranje po statusu; status "Zauzet" prikazuje ID rute | NE – niži prioritet za UAT | NIZAK – regresiono testirati samo nakon izmjena API endpointa ili modela poštara |
| **PBI-017 Dodavanje poštanskog sandučića** | DA – validacija GPS koordinata (format, opseg), obavezna polja, jedinstvenost serijskog broja | DA – POST /api/sandučići, upis koordinata i tipa u bazu, provjera duplikata serijskog broja | DA – unos koordinata → mini-mapa prikazuje pin; odabir na mapi → auto-popunjavanje polja; dugme "Odaberi na mapi" | DA – sandučić se kreira i postaje vidljiv na listi i na mapi | SREDNJI – ponovo testirati nakon izmjena modela sandučića ili validacije koordinata |
| **PBI-018 Izmjena podataka o sandučiću** | DA – iste validacije kao pri kreiranju; logika upisa u Audit Log | DA – PUT/PATCH /api/sandučići/:id; Audit Log zapis; provjera da promjena koordinata ažurira prikaz na svim mapama | DA – izmjena → zelena potvrda; podaci ažurirani na detaljima; nepromijenjeni podaci → bez serverskog zahtjeva | DA – izmjena lokacije se reflektuje u realnom vremenu na mobilnom prikazu | SREDNJI – ponovo testirati nakon refactoringa Audit Log logike ili izmjene PUT/PATCH endpointa |
| **PBI-019 Pregled sandučića na listi** | NE | DA – GET /api/sandučići, filteri po tipu i prioritetu, sortiranje po prioritetu | DA – straničenje funkcioniše; filteri sužavaju listu; klik na adresu → modal s mapom; prazna baza → poruka | NE – niži prioritet za UAT | NIZAK – regresiono testirati samo nakon izmjena filtera ili modela sandučića |
| **PBI-020 Definisanje prioriteta sandučića** | DA – logika automatskog prioriteta za tip "Specijalni"; zabrana pohranjivanja sandučića bez prioriteta | DA – PATCH /api/sandučići/:id/prioritet; provjera da promjena prioriteta ažurira algoritam kod sljedećeg generisanja | DA – promjena prioriteta → odmah vidljiva u listi; sandučići "Visok" se pojavljuju na vrhu liste generisanja | DA – visokoprioritetan sandučić ima prednost u generisanoj ruti | VISOK – svaka izmjena logike prioriteta zahtijeva regresiono testiranje algoritma (PBI-022) |
| **PBI-021 Evidencija radnih pravila sandučića** | DA – validacija vremenskih okvira (krajnje > početno, nema preklapanja dva termina, nepostojuće vrijeme); zabrana pohranjivanja bez radnog dana | DA – PATCH /api/sandučići/:id/pravila; provjera da algoritam isključuje sandučić koji nije u radnom danu ili vremenskom okviru | DA – sandučić bez subote izostavljen iz subotnje rute; sandučić s "24/7" uvijek uključen | DA – radna pravila se korektno primjenjuju pri generisanju rute | VISOK – svaka izmjena modela radnih pravila zahtijeva regresiono testiranje generisanja rute (PBI-022) |
| **PBI-022 Generisanje dnevne rute** | DA – nearest-neighbor heuristika: ispravan redoslijed, uvažavanje prioriteta, isključivanje sandučića van radnog okvira; proračun trajanja | DA – POST /api/rute/generiši; komunikacija s optimizacijskim servisom; provjera da su isključeni neaktivni i nedostupni sandučići | DA – ruta se vizualizuje na mapi; proračun za 50 tačaka < 5s; upozorenje ako >8h; "Nema lokacija" poruka pri praznom skupu | DA – dispečer generiše rutu i vizuelno potvrđuje logičnost redoslijeda obilaska | KRITIČAN – svaka izmjena `IRouteOptimizationService` ili ulaznih podataka algoritma zahtijeva punu regresijsku provjeru |
| **PBI-023 Dodjela rute poštaru** | DA – zabrana dodjele poštaru koji je već zauzet; zabrana višestruke dodjele iste rute | DA – PATCH /api/rute/:id/dodjela; ažuriranje statusa poštara u bazi; slanje internih obavijesti poštaru | DA – dodjela → poštar dobija obavijest; status poštara mijenja u "Zauzet"; dodjela bez odabranog poštara → blokada | DA – poštar vidi svoju rutu odmah nakon dodjele | VISOK – regresiono testirati nakon izmjena statusa poštara ili logike dodjele |
| **PBI-024 Pregled detalja rute** | NE | DA – GET /api/rute/:id; ispravan redoslijed sandučića; sumarne statistike (km, trajanje, broj tačaka) | DA – dispečer vidi numerisane pinove na mapi; klik na sandučić → centriranje + detalji; dugme "Nazad" bez gubitka podataka | DA – detalji rute odgovaraju generisanoj sekvenci | NIZAK – regresiono testirati samo ako je promijenjen GET /api/rute/:id endpoint |
| **PBI-025 Ručna izmjena redoslijeda obilaska** | DA – validacija da se drag-and-drop ne može koristiti za brisanje tačaka; ponovna kalkulacija kilometraže i ETA | DA – PATCH /api/rute/:id/redoslijed; provjera da se novi redoslijed pohranjuje i prenosi poštaru | DA – drag-and-drop ažurira numeraciju i statistiku; napuštanje bez čuvanja → upozoravajući modal | DA – ručno uređena ruta se korektno prikazuje poštaru na mobilnom uređaju | VISOK – ponovo testirati nakon izmjena PATCH endpointa za redoslijed ili kalkulacije ETA/km |
| **PBI-026 Mobilni prikaz dodijeljene rute** | NE | DA – GET /api/postari/:id/aktivna-ruta; provjera da se prikazuje isključivo ruta dodijeljena prijavljenom poštaru | DA – mobilni prikaz (< 480px) funkcioniše; prebacivanje lista ↔ mapa; "Nema ruta za danas" poruka; otvaranje ext. mape klikm na adresu | DA – poštar vidi svoju rutu na mobilnom uređaju sa svim potrebnim informacijama | SREDNJI – ponovo testirati pri izmjenama responsive layouta ili mobilne komponente |
| **PBI-027 Ažuriranje statusa sandučića** | DA – validacija statusa (Na čekanju / Završeno / Problem); zabrana ponovne promjene iz "Završeno"; logika offline pohrane | DA – PATCH /api/route-stops/:id/status; provjera da se timestamp i GPS koordinate zapisuju; real-time ažuriranje dispečerske konzole | DA – promjena statusa → boja pina na mapi se mijenja; status "Problem" zahtijeva obaveznu napomenu; dispečer vidi promjenu bez osvježavanja | DA – dispečer u realnom vremenu prati ažuriranja statusa od strane poštara | KRITIČAN – svaka izmjena real-time logike ili modela statusa zahtijeva regresijsku provjeru zajedno s PBI-029 |
| **PBI-028 Označavanje nedostupne lokacije** | DA – validacija napomene (min. 10 karaktera); zabrana pohrane statusa "Nedostupno" bez napomene | DA – PATCH /api/route-stops/:id/status (status: Nedostupno); zapis GPS koordinata i napomene u bazu | DA – izbor "Nedostupno" → obavezno polje napomene; pohrana → crveni indikator na listi i mapi | DA – dispečer vidi napomenu o nedostupnosti i GPS lokaciju poštara u trenutku prijave | VISOK – ponovo testirati uz PBI-027 nakon izmjena PATCH endpointa za status |
| **PBI-029 Praćenje statusa rute od strane dispečera** | NE | DA – GET /api/rute/:id/status; real-time ažuriranje (polling svakih 60s); provjera autorizacije (samo Dispečer/Admin) | DA – procentualni indikator napretka se ažurira; klik na sandučić "Nedostupno" → prikazuje napomenu; ruta završena → status "Arhivirana" | DA – dispečer prati realizaciju rute bez ručnog osvježavanja | KRITIČAN – regresiono testirati uz PBI-027 i PBI-028 nakon svake izmjene real-time ili polling logike |
| **PBI-030 Osnovni dnevni izvještaj** | DA – logika agregacije podataka: planiranih vs. ispražnjenih vs. neuspješnih | DA – GET /api/izvještaji?datum=...; autorizacija (Admin/Dispečer); provjera tačnosti agregatnih podataka | DA – odabir datuma → izvještaj s brojevima; preuzimanje PDF-a funkcioniše; "Nema podataka" poruka za datum bez aktivnosti | DA – administrator ili dispečer preuzima tačan dnevni izvještaj | SREDNJI – regresiono testirati nakon izmjena sheme baze podataka ili agregatnih upita |

---

# Veza sa acceptance kriterijima

| User Story | Acceptance Criteria | ID testnog slučaja | Opis testnog slučaja |
|---|---|---|---|
| **PBI-011 / US-01, US-02, US-03** | Kreiranje bez popunjenih polja je blokirano; sistem označava obavezna polja crvenom bojom | TC-01 | **Preduslovi:** Administrator otvorio formu za kreiranje. **Koraci:** Kliknuti "Kreiraj" bez unesenih podataka. **Očekivani rezultat:** Sva obavezna polja (Ime, Prezime, Email, Lozinka) označena crveno uz poruku "Ovo polje je obavezno". |
| **PBI-011 / US-02** | Sistem blokira kreiranje računa s emailom koji već postoji u bazi | TC-02 | **Preduslovi:** U bazi postoji korisnik s emailom `test@post.ba`. **Koraci:** Unijeti iste podatke s tim emailom i kliknuti "Kreiraj". **Očekivani rezultat:** Poruka "Korisnik sa ovim emailom već postoji"; račun nije kreiran. |
| **PBI-011 / US-02** | Lozinka mora zadovoljiti pravila jačine; dugme "Kreiraj" onemogućeno dok lozinka nije validna | TC-03 | **Preduslovi:** Forma za kreiranje otvorena. **Koraci:** Unijeti lozinku bez specijalnog znaka (npr. `Lozinka1`). **Očekivani rezultat:** Indikator lozinke nije zelen; dugme "Kreiraj" ostaje onemogućeno. |
| **PBI-011 / US-01** | Uspješno kreiranje → zelena potvrda, polja se čiste, poštar dobija email | TC-04 | **Preduslovi:** Svi podaci su validni i jednistveni. **Koraci:** Unijeti ispravne podatke i kliknuti "Kreiraj". **Očekivani rezultat:** Zelena toast obavijest "Račun za [Ime Prezime] je uspješno kreiran. Podaci su poslani na email korisnika."; polja forme prazna. |
| **PBI-012 / US-04** | Uspješna prijava → redirect na Dashboard u roku od 2 sekunde | TC-05 | **Preduslovi:** Korisnik s ulogom Dispečer postoji u sistemu. **Koraci:** Unijeti ispravne kredencijale i kliknuti "Prijava". **Očekivani rezultat:** Korisnik preusmjeren na Dashboard u roku od 2 sekunde; aktivna sesija uspostavljena. |
| **PBI-012 / US-05** | Pogrešni kredencijali → generička poruka bez otkrivanja koji podatak je pogrešan | TC-06 | **Preduslovi:** Korisnik postoji u sistemu. **Koraci:** Unijeti ispravan email ali pogrešnu lozinku. **Očekivani rezultat:** Prikazuje se "Neispravni kredencijali. Pokušajte ponovo." Poruka ne otkriva šta je pogrešno. |
| **PBI-012 / US-04** | Račun se zaključava na 15 minuta nakon 5 uzastopnih neuspjelih pokušaja prijave | TC-07 | **Preduslovi:** Korisnik postoji u sistemu. **Koraci:** Unijeti pogrešnu lozinku 5 puta zaredom. **Očekivani rezultat:** Toast obavijest "Račun je privremeno zaključan zbog previše neuspješnih pokušaja."; prijava blokirana 15 min. |
| **PBI-012 / US-06** | Poštar se pri prvoj prijavi ne može nastaviti bez promjene inicijalne lozinke | TC-08 | **Preduslovi:** Poštar se prvi put prijavljuje s inicijalnom lozinkom. **Koraci:** Unijeti inicijalne kredencijale. **Očekivani rezultat:** Sistem prikazuje ekran "Promjena lozinke"; navigacija na druge stranice blokirana. |
| **PBI-012 / US-06** | Nova lozinka ne smije biti identična inicijalnoj | TC-09 | **Preduslovi:** Poštar je na ekranu obavezne promjene lozinke. **Koraci:** Unijeti istu vrijednost kao inicijalna lozinka. **Očekivani rezultat:** Poruka "Nova lozinka ne smije biti ista kao privremena."; promjena odbijena. |
| **PBI-013 / US-07** | Nakon odjave korisnik ne može pristupiti zaštićenim stranicama direktnim URL-om | TC-10 | **Preduslovi:** Korisnik se odjavljivao. **Koraci:** Ručno unijeti `/dashboard` u adresnu traku. **Očekivani rezultat:** Sistem odbija pristup i preusmjerava na Login uz poruku "Pristup odbijen. Molimo prijavite se." |
| **PBI-013 / US-07** | Klik na "Back" u browseru nakon odjave ne otvara zaštićeni sadržaj | TC-11 | **Preduslovi:** Korisnik izvršio odjavu. **Koraci:** Pritisnuti "Nazad" u browseru. **Očekivani rezultat:** Browser ne prikazuje zaštićeni sadržaj; korisnik ostaje na Login ekranu ili biva preusmjeren na njega. |
| **PBI-014 / US-09** | Poštar vidi isključivo sebi namijenjen dashboard; administrativne opcije su nevidljive | TC-12 | **Preduslovi:** Korisnik s ulogom Poštar postoji. **Koraci:** Prijaviti se kao Poštar. **Očekivani rezultat:** Sidebar prikazuje samo "Moja današnja ruta, Mapa sandučića, Prijava problema". Opcije upravljanja korisnicima i sandučićima nisu vidljive. |
| **PBI-014 / US-10** | Poštar ne može pristupiti administrativnim URL-ovima; pokušaj pristupa se bilježi u Security Log | TC-13 | **Preduslovi:** Korisnik prijavljen kao Poštar. **Koraci:** Ručno unijeti URL `/admin/postari` u adresnu traku. **Očekivani rezultat:** Redirect na Poštar-Dashboard uz toast "Nemate ovlaštenje za ovu akciju."; zapis u Security Log-u. |
| **PBI-014** | API endpointi odbijaju zahtjeve bez odgovarajuće uloge, čak i kad se šalju direktno | TC-14 | **Preduslovi:** Validan JWT token s ulogom Poštar. **Koraci:** Poslati DELETE zahtjev na `/api/sandučići/1` koristeći Poštar token. **Očekivani rezultat:** API vraća HTTP 403; akcija nije izvršena. |
| **PBI-015 / US-11** | Forma za dodavanje poštara blokira pohrane bez obaveznih polja | TC-15 | **Preduslovi:** Administrator otvorio formu za dodavanje poštara. **Koraci:** Ostaviti polje "Ime" praznim i kliknuti "Spasi". **Očekivani rezultat:** Polje označeno crvenom bojom uz poruku "Polje je obavezno"; pohrana spriječena. |
| **PBI-015 / US-12** | Sistem blokira unos duplog Internog ID-a; prikazuje link na postojećeg poštara | TC-16 | **Preduslovi:** Poštar s ID brojem `101` postoji u bazi. **Koraci:** Unijeti novog poštara s istim ID-om `101` i kliknuti "Spasi". **Očekivani rezultat:** Poruka "Ovaj ID je već dodijeljen poštaru [Ime i Prezime]" s linkom na tog poštara; dugme "Spasi" onemogućeno. |
| **PBI-017 / US-14** | Neispravni format GPS koordinata blokira pohrane sandučića | TC-17 | **Preduslovi:** Administrator otvorio formu za novi sandučić. **Koraci:** Unijeti slova u polje Latitude (npr. `abc`). **Očekivani rezultat:** Poruka "Unesite ispravan format koordinata (npr. 43.8563)"; pohrana blokirana. |
| **PBI-017 / US-14** | Unos koordinata prikazuje mini-mapu s pinom radi vizuelne potvrde | TC-18 | **Preduslovi:** Forma za sandučić otvorena. **Koraci:** Unijeti validne koordinate (npr. `43.8563, 18.4131`). **Očekivani rezultat:** Mini-mapa ispod polja pokazuje pin na unesenoj lokaciji. |
| **PBI-017 / US-15** | Sistem blokira duplikat serijskog broja sandučića | TC-19 | **Preduslovi:** Sandučić s serijskim brojem `SB-001` postoji. **Koraci:** Pokušati kreirati novi sandučić s istim serijskim brojem. **Očekivani rezultat:** Toast "Sandučić sa ovim serijskim brojem je već registrovan"; pohrana odbijena. |
| **PBI-020 / US-18** | Sandučić mora imati inicijalno postavljen prioritet "Srednji" pri kreiranju | TC-20 | **Preduslovi:** Forma za kreiranje sandučića otvorena. **Koraci:** Otvoriti formu i provjeriti vrijednost polja "Prioritet". **Očekivani rezultat:** Dropdown "Prioritet" inicijalno postavljen na "Srednji". |
| **PBI-021 / US-32** | Sistem odbija vremenski okvir gdje je krajnje vrijeme ranije od početnog | TC-21 | **Preduslovi:** Forma za sandučić, sekcija "Dostupnost". **Koraci:** Unijeti od `14:00` do `10:00`. **Očekivani rezultat:** Poruka "Krajnje vrijeme mora biti nakon početnog"; pohrana odbijena. |
| **PBI-021 / US-33** | Sandučić bez označene subote se ne pojavljuje u subotnjoj ruti | TC-22 | **Preduslovi:** Sandučić ima označene samo radne dane Pon–Pet. **Koraci:** Generisati rutu za Subotu. **Očekivani rezultat:** Taj sandučić nije uključen u generisanu rutu bez obzira na prioritet. |
| **PBI-022 / US-22** | Algoritam uvažava prioritete — Visok prioritet ima prednost u redoslijedu obilaska | TC-23 | **Preduslovi:** Postoje sandučići s različitim prioritetima. **Koraci:** Generisati rutu i pregledati redoslijed. **Očekivani rezultat:** Sandučići s prioritetom "Visok" nalaze se u ranijim pozicijama rute u odnosu na sandučiće s nižim prioritetom pri istoj udaljenosti. |
| **PBI-022 / US-22** | Generisanje rute za do 50 tačaka završava se u roku od 5 sekundi | TC-24 | **Preduslovi:** Postoji 50 aktivnih sandučića s važećim radnim pravilima. **Koraci:** Pokrenuti generisanje rute. **Očekivani rezultat:** Ruta se generisala i prikazuje na mapi u roku od maksimalno 5 sekundi; prikazuje se loader za to vrijeme. |
| **PBI-022 / US-22** | Dugme "Generiši" je onemogućeno kada nema dostupnih sandučića | TC-25 | **Preduslovi:** Svi sandučići su neaktivni ili nemaju označen današnji dan. **Koraci:** Otvoriti ekran za generisanje rute. **Očekivani rezultat:** Dugme "Generiši" je onemogućeno; poruka "Nema dostupnih lokacija za generisanje rute." |
| **PBI-023 / US-25** | Sistem blokira dodjelu rute poštaru koji je već zauzet | TC-26 | **Preduslovi:** Poštar ima aktivan zadatak. **Koraci:** Pokušati dodijeliti novu rutu tom poštaru. **Očekivani rezultat:** Sistem onemogućava dodjelu; taj poštar nije ponuđen u padajućem meniju. |
| **PBI-023 / US-25** | Nakon dodjele poštar dobija internu obavijest; status poštara mijenja u "Zauzet" | TC-27 | **Preduslovi:** Ruta generisana; poštar ima status "Dostupan". **Koraci:** Odabrati poštara i kliknuti "Potvrdi dodjelu". **Očekivani rezultat:** Zelena toast "Ruta je uspješno dodijeljena poštaru [Ime i prezime]"; status poštara u listi = "Zauzet"; poštar prima obavijest. |
| **PBI-025 / US-24** | Napuštanje stranice s nespremljenim izmjenama prikazuje upozoravajući modal | TC-28 | **Preduslovi:** Dispečer promijenio redoslijed sandučića u ruti bez klika na "Sačuvaj promjene". **Koraci:** Pokušati napustiti stranicu. **Očekivani rezultat:** Modal "Imate nesačuvane promjene. Želite li ih sačuvati prije odlaska?". |
| **PBI-025 / US-24** | Drag-and-drop ažurira numeraciju i sumarne statistike (km, ETA) u realnom vremenu | TC-29 | **Preduslovi:** Dispečer otvorio detalje rute s omogućenom izmjenom. **Koraci:** Premjestiti sandučić s pozicije 3 na poziciju 1 drag-and-drop metodom. **Očekivani rezultat:** Numeracija tačaka na listi i mapi se odmah ažurira; prikazana kilometraža i ETA su ponovo izračunati. |
| **PBI-026 / US-26** | Poštar koji nema dodijeljenu rutu vidi odgovarajuću poruku umjesto praznog ekrana | TC-30 | **Preduslovi:** Poštar prijavljen bez dodijeljene rute za danas. **Koraci:** Otvoriti mobilni prikaz rute. **Očekivani rezultat:** Poruka "Trenutno nemate dodijeljenih ruta za danas." |
| **PBI-026 / US-26** | Mobilni prikaz je funkcionalan i upotrebljiv na ekranima manjim od 480px | TC-31 | **Preduslovi:** Poštar prijavljen s dodijeljenom rutom. **Koraci:** Otvoriti aplikaciju na uređaju/emulatoru s ekranom 375px. **Očekivani rezultat:** Lista sandučića, dugmad i mapa su pregledni i upotrebljivi; nema horizontalnog scrollanja ni preklapanja elemenata. |
| **PBI-027 / US-28** | Promjena statusa na "Završeno" bilježi timestamp i GPS koordinate; boja pina se mijenja | TC-32 | **Preduslovi:** Poštar otvorio sandučić u mobilnoj listi rute. **Koraci:** Kliknuti "Potvrdi pražnjenje" (status → Završeno). **Očekivani rezultat:** Pin sandučića na mapi mijenja boju u zelenu; timestamp i GPS koordinate zapisane u bazi; status više ne može biti promijenjen. |
| **PBI-027 / US-28** | Promjene statusa se u realnom vremenu prikazuju na dispečerskoj konzoli | TC-33 | **Preduslovi:** Poštar i dispečer istovremeno otvorili sistem. **Koraci:** Poštar mijenja status sandučića na "Završeno". **Očekivani rezultat:** Dispečer vidi promjenu bez ručnog osvježavanja (unutar 60 sekundi). |
| **PBI-028 / US-29** | Status "Nedostupno" zahtijeva obaveznu napomenu od min. 10 karaktera | TC-34 | **Preduslovi:** Poštar otvorio sandučić na mobilnoj listi. **Koraci:** Odabrati status "Nedostupno" i pokušati spasiti bez napomene. **Očekivani rezultat:** Poruka "Molimo unesite razlog nedostupnosti lokacije."; pohrana spriječena. |
| **PBI-029 / US-30** | Dispečer vidi procentualni indikator napretka za svaku aktivnu rutu | TC-35 | **Preduslovi:** Poštar je obradio dio sandučića. **Koraci:** Dispečer otvara Operativni pregled. **Očekivani rezultat:** Svaka aktivna ruta prikazuje procentualni indikator (npr. "45% završeno") i sumarnu statistiku: završeni / nedostupni / na čekanju. |
| **PBI-029 / US-30** | Klik na sandučić "Nedostupno" prikazuje napomenu poštara | TC-36 | **Preduslovi:** Poštar evidentirao nedostupnost s napomenom. **Koraci:** Dispečer klikne na taj sandučić u operativnom pregledu. **Očekivani rezultat:** Prikazuje se napomena poštara i GPS lokacija u trenutku prijave. |
| **PBI-029 / US-30** | Kada su svi sandučići obrađeni, ruta dobija status "Arhivirana" | TC-37 | **Preduslovi:** Svi sandučići na ruti imaju status "Završeno" ili "Nedostupno". **Koraci:** Provjeriti status rute u sistemu. **Očekivani rezultat:** Ruta je označena kao "Arhivirana" uz zabilježeno ukupno trajanje obilaska. |
| **PBI-030 / US-31** | Dnevni izvještaj sadrži tačne agregatne podatke za odabrani datum | TC-38 | **Preduslovi:** Za odabrani datum postoje završene rute. **Koraci:** Odabrati datum i generisati izvještaj. **Očekivani rezultat:** Izvještaj prikazuje ukupan broj planiranih, ispražnjenih i neuspješnih obilazaka; izlistane su napomene o nedostupnim lokacijama grupirane po poštarima. |
| **PBI-030 / US-31** | Preuzimanje izvještaja u PDF formatu funkcioniše jednim klikom | TC-39 | **Preduslovi:** Izvještaj za odabrani datum generisan i prikazan. **Koraci:** Kliknuti dugme za preuzimanje. **Očekivani rezultat:** PDF fajl s izvještajem se preuzima na uređaj. |
| **PBI-030 / US-31** | Izvještaj za datum bez aktivnosti prikazuje informativnu poruku | TC-40 | **Preduslovi:** Za odabrani datum nisu evidentirani nikakvi obilasci. **Koraci:** Odabrati datum bez aktivnosti i pokrenuti generisanje. **Očekivani rezultat:** Poruka "Za traženi datum nisu pronađene zabilježene aktivnosti."; dugme za preuzimanje onemogućeno. |

# Način evidentiranja rezultata testiranja

## Dokumentacija test slučajeva

Za svaki test slučaj (TC) evidentiiraju se sljedeće informacije:

- Identifikator test slučaja (TC-[redni broj])
- Veza sa PBI-jem i acceptance kriterijem koji pokriva
- Preduslovi, koraci izvršavanja i očekivani rezultat
- Stvarni rezultat nakon izvršavanja
- Status: **Passed / Failed / Blocked / Skipped**
- Prioritet: **Critical / High / Medium / Low**

## Evidencija defekta

Svaki pronađeni defekt evidentira se s minimalnim skupom informacija potrebnih za reprodukciju i rješavanje:

- Jedinstveni ID defekta (DEF-[redni broj])
- Kratak naziv i opis problema
- Koraci za reprodukciju i razlika između očekivanog i stvarnog ponašanja
- Severity: **Critical / Major / Minor**
- Status: **New / In Progress / Fixed / Verified / Closed**
- Veza s test slučajem i PBI-jem; screenshot ili log prilog gdje je primjenljivo

## Test izvještaj

Po završetku svake faze testiranja generiše se kratak Test Report koji sadrži:

- Sažetak: ukupan broj TC-ova i raspodjela po statusu (Passed / Failed / Blocked / Skipped)
- Listu otvorenih defekta s Severity klasifikacijom
- Postotak pokrivenih acceptance kriterija
- Preporuku: **Go / No-Go** za nastavak prema sljedećoj fazi ili puštanje u produkciju

---

# Glavni rizici kvaliteta

| ID | Opis rizika | Vjerovatnoća | Uticaj | Strategija mitigacije |
|---|---|---|---|---|
| R01 | Neispravna implementacija RBAC-a — korisnik s ulogom Poštar može pristupiti administrativnim endpointima ili URL-ovima | Srednja | Visok | Testirati svaki zaštićeni API endpoint direktnim HTTP zahtjevima za svaku ulogu (TC-13, TC-14); uključiti provjeru Security Loga |
| R02 | Optimizacijski algoritam ne uvažava prioritete sandučića ili radna pravila — ruta sadrži sandučiće koji nisu dostupni za taj dan | Srednja | Visok | Pokriti unit testovima sve kombinacije prioriteta i radnih dana; sistemski test s rubnim slučajevima (TC-22, TC-23) |
| R03 | Real-time sinhronizacija između poštara i dispečera ne funkcioniše — dispečer vidi zastarjele statuse | Srednja | Visok | Integracioni test s istovremenim sesijama poštara i dispečera; potvrditi automatsko osvježavanje unutar 60 sekundi (TC-33) |
| R04 | Sesija korisnika ostaje aktivna nakon odjave — moguć neovlašten pristup na dijeljenim uređajima | Niska | Visok | Testirati invalidaciju sesije na serveru i blokadu zaštićenih ruta nakon odjave (TC-10, TC-11) |
| R05 | Mobilni prikaz za poštara nije upotrebljiv na manjim ekranima — terenska operativnost sistema kompromitovana | Visoka | Srednji | UI testiranje na ekranima < 480px za sve ključne akcije poštara: pregled rute, promjena statusa, unos napomene (TC-31) |
| R06 | Generisanje rute za veći broj sandučića premašuje vremenski prag od 5 sekundi — dispečer ne može efikasno raditi | Niska | Srednji | Sistemski performansni test s 50 sandučića; pratiti API response time u integracionim testovima (TC-24) |
