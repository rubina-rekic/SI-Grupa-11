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
| Provjera praćenja realizacije i izvještavanja (PBI-029, PBI-030, PBI-049, PBI-050) | Dispečerski pregled statusa obilazaka u realnom vremenu; generisanje dnevnih izvještaja; pretraga i filtriranje podataka | Dispečer u realnom vremenu vidi progres rute; izvještaji sadrže tačne podatke o realizaciji; pretraga i filtriranje vraćaju relevantne rezultate; izvještaj je dostupan isključivo ovlaštenim ulogama |

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
