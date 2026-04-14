# Domain Model

---

## Uvod

Domenski model predstavlja konceptualnu strukturu sistema za optimizaciju ruta poštanskih sandučića.
On identifikuje ključne objekte u realnom svijetu (poput sandučića, ruta i korisnika), 
njihove karakteristike i međusobne odnose. Ovaj model služi kao osnova za logičko projektovanje baze podataka i implementaciju poslovnih pravila koja osiguravaju efikasno pražnjenje i punjenje sandučića uz maksimalnu sigurnost podataka.

---

## Glavni entiteti
1. **Korisnik (User)** - Predstavlja uposlenike koji koriste sistem. Sadrži podatke potrebne za identifikaciju i radnu evidenciju.
2. **Poštanski sandučić (Mailbox)** - Fizički objekt sa lokacijom i prioritetom.
3. **Ruta (Route)** - Dnevni operativni plan generisan za jednog poštara.
4. **Stavka rute (RouteItem)** - Ovaj entitet je ključna poveznica (međutabela) između rute i  sandučića. 
On bilježi realizaciju svakog pojedinačnog zadatka na terenu.
5. **Dnevni izvještaj (Daily Report)** - Sumarni prikaz učinka i incidenata za određeni datum.

---

## Ključni atributi 

### Korisnik (User)

* **UserID:** Službeni identifikator unutar sistema.
* **Ime i Prezime:** Osnovni podaci o identitetu uposlenika.
* **Email:** Jedinstvena adresa za komunikaciju i sistemska obavještenja.
* **Korisničko ime (Username):** Jedinstveni niz karaktera za prijavu (validiran format).
* **Lozinka:** Sigurnosni ključ (čuva se kao hashirana vrijednost).
* **Telefon:** Kontakt broj.
* **Uloga (Role):** *[Enum]* Određuje prava pristupa. Vrijednosti: 
    - ADMINISTRATOR (Upravlja sistemom i korisnicima)
    - DISPEČER (Planira i nadgleda rute)
    - POŠTAR (Izvršava rad na terenu)
* **Zastavica prve prijave:** *[Boolean]* Ako je 'True', sistem zahtijeva promjenu lozinke pri prvom ulasku.
* **ZadnjaPrijava:** *[Timestamp]* Vrijeme posljednjeg uspješnog pristupa sistemu.
* **Status računa:** *[Enum]* Stanje računa ('Aktivan', 'Neaktivan', 'Deaktiviran').

### Poštanski sandučić (Mailbox)

* **SandučićID:** Jedinstveni identifikator koji se koristi za pretragu.
* **Adresa:** Tekstualni opis lokacije sandučića.
* **Geolokacija:** Sastoji se od dva atributa: 'Latitude' i 'Longitude' (Decimalni brojevi).
* **Tip sandučića:** *[Enum]* Vrsta sandučića iz predefinisane liste (npr. 'Standardni', 'Uslužni', 'Poslovni') — utiče na način obrade i prikaz u sučelju.
* **Prioritet:** *[Enum]* Određuje hitnost obilaska: 'Visok', 'Srednji', 'Nizak'.
* **Kapacitet:** Numerička vrijednost zapremine sandučića (bitno za optimizaciju punjenja).
* **Status objekta:** *[Enum]* Stanje na terenu: 'Aktivan', 'Neaktivan', 'Oštećen'.
* **Radni režim:** Definiše 'Vrijeme od', 'Vrijeme do' i 'Radne dane'.

### Ruta (Route)

* **RutaID:** Jedinstveni identifikator rute.
* **PoštarID:** *[Strani ključ]* Povezuje rutu sa konkretnim Poštarom kome je ista dodjeljena.
* **DispečerID:** *[Strani ključ]* Povezuje rutu sa Dispečerom koji ju je kreirao/odobrio.
* **IzvjestajID:** *[Strani ključ]* Povezuje rutu sa Dnevnim izvještajem kojem pripada.
* **Datum:** Kalendarski dan za koji je ruta validna.
* **Status rute:** *[Enum]* Faze: `Planirana`, `Aktivna`, `Završena`, `Prekinuta`.
* **Razlog prekida:** Tekstualni opis incidenta ako je ruta prekinuta (npr. kvar na vozilu).
* **Ukupna distanca:** Procijenjena kilometraža rute (u kilometrima).

### Stavka rute (RouteItem)

* **StavkaRuteID:** Jedinstveni identifikator.
* **RutaID:** Strani ključ koji povezuje stavku sa konkretnim dnevnim planom.
* **SandučićID:** Strani ključ koji identifikuje sandučić koji treba obići.
* **Redoslijed:** Redni broj stajališta u ruti (npr. 1, 2, 3...).
* **Status obilaska:** *[Enum]* Ishod zadatka: `Planirano`, `Realizovano`, `Preskočeno`, `Nedostupno`.
* **Tip realizacije:** *[Enum, opcionalno]* Detalj akcije kada je status `Realizovano`: `Ispražnjen` ili `Napunjen`. Polje je `null` za ostale statuse.
* **Vrijeme potvrde:** Timestamp (Datum i vrijeme) kada je poštar kliknuo na potvrdu u aplikaciji.
* **Geo-validacija:** Koordinate poštara u momentu potvrde (koristi se za US-13: provjera blizine sandučiću).
* **Napomena:** Tekstualni opis (npr. "Sandučić blokiran parkiranim vozilom").

### Dnevni izvještaj (Daily Report)
Analitički entitet koji sumira radni dan.

* **IzvjestajID:** Jedinstveni identifikator izvještaja.
* **PregledaoID:** Strani ključ -> Povezuje izvještaj sa `UserID` iz tabele Korisnik.
* **Datum izvještaja:** Dan na koji se podaci odnose.
* **Statistika:** Sumarni podaci (broj planiranih vs. broj realizovanih obilazaka).
* **Procenat uspješnosti:** Izračunata efikasnost u procentima (%).
* **Lista incidenata:** Pregled svih stavki sa statusom `Nedostupno` ili `Oštećen`.

---

## Veze između entiteta

U ovom dijelu definisani su odnosi između ključnih objekata sistema, koji osiguravaju integritet podataka i omogućavaju praćenje procesa od planiranja do izvještavanja.

### Korisnik (User) — Ruta (Route) [1:N]
- **Opis:** Jedan **Poštar** može biti zadužen za mnogo različitih ruta tokom vremena, ali jedna konkretna dnevna ruta pripada tačno jednom poštaru. 
- **Opis:** Jedan **Dispečer** može kreirati i upravljati sa više ruta, dok svaka ruta mora imati zabilježenog dispečera koji ju je odobrio.
- **Ključ povezivanja:** `PoštarID` i `DispečerID` unutar entiteta **Ruta**.

### Ruta (Route) — Stavka rute (RouteItem) [1:N]
- **Opis:** Svaka **Ruta** se sastoji od više **Stavki rute** (stajališta/zadataka). Jedna stavka rute ne može postojati bez matične rute kojoj pripada.
- **Ključ povezivanja:** `RutaID` unutar entiteta **Stavka rute**.

### Poštanski sandučić (Mailbox) — Stavka rute (RouteItem) [1:N]
- **Opis:** Jedan **Sandučić** se pojavljuje kao cilj u mnogo različitih stavki rute (svaki put kada je planiran za obilazak), ali se jedna konkretna stavka rute (zadatak u tom trenutku) odnosi na tačno jedan sandučić.
- **Ključ povezivanja:** `SandučićID` unutar entiteta **Stavka rute**.

### Dnevni izvještaj (Daily Report) — Ruta (Route) [1:N]
- **Opis:** Jedan **Dnevni izvještaj** sumira podatke iz svih ruta koje su se odvijale na taj specifični datum. Iako izvještaj agregira podatke, svaka ruta je povezana sa izvještajem preko datuma izvršenja.
- **Ključ povezivanja:** 'IzvjestajID' unutar entiteta **Ruta**.

### Administrator (Admin) — Poštanski sandučić (Mailbox) [1:N]
- **Opis:** Administrator je jedina uloga koja ima pravo vršiti CRUD operacije (kreiranje, čitanje, ažuriranje, brisanje) nad podacima o sandučićima.
- **Ključ povezivanja:** Logička veza kroz sistemske permisije.

### Dnevni izvještaj (Daily Report) — Korisnik (Dispečer/Admin) [N:1]
- **Opis:** Svaki izvještaj verifikuje jedan uposlenik (Admin ili Dispečer) radi uvida u operativnu efikasnost i rješavanje incidenata prijavljenih sa terena.
- **Ključ povezivanja:**  Veza se ostvaruje preko `PregledaoID` unutar entiteta **Dnevni Izvještaj**.

## Poslovna pravila važna za model

### Upravljanje korisnicima
- Korisničke račune za poštare može kreirati isključivo administrator.
- Email / korisničko ime mora biti jedinstveno u cijelom sistemu.
- Inicijalna lozinka mora zadovoljiti minimalne sigurnosne kriterije (min. 8 karaktera, kombinacija slova i brojeva).
- Svaki korisnik (Administrator, Dispečer, Poštar) koji se prijavljuje prvi put inicijalnom lozinkom mora je promijeniti prije nego što dobije pristup bilo kojoj drugoj funkcionalnosti.
- Svaki korisnik ima tačno jednu ulogu koja ne može biti prazna.

### Upravljanje sandučićima
- Sandučić mora imati geografsku lokaciju i adresu definisanu pri unosu.
- Neaktivni sandučić ne smije biti uključen u automatski generisanu rutu.
- Prioritet sandučića ulazi kao jedan od parametara algoritma optimizacije rute.
- Radni dani i vremenska ograničenja sandučića moraju biti poštovana pri generisanju rute.

### Upravljanje rutama
- Ruta se može dodijeliti samo aktivnom poštaru.
- Jednom aktiviranoj ruti (poštar je počeo obilazak) dispečer ne smije mijenjati redoslijed bez evidentiranja te izmjene.
- Ruta se automatski arhivira kada poštar označi njen završetak.
- Prekinuta ruta mora imati evidentirano koji su sandučići obrađeni, a koji nisu.
- Isti sandučić se ne smije pojaviti dva puta unutar iste rute.

### Realizacija obilaska
- Poštar može evidentirati samo jedan od sljedećih ishoda po stavci rute: `Realizovano` (uz obavezan tip realizacije — `Ispražnjen` ili `Napunjen`), `Nedostupno` ili `Preskočeno`.
- Timestamp potvrde generira sistem na strani servera, a ne klijentska aplikacija.
- Nedostupna lokacija mora biti evidentirana s napomenom kako bi dispečer mogao reagovati.

### Izvještavanje
- Procenat uspješnosti rute računa se kao: (broj realizovanih stavki / ukupan broj stavki) × 100.
- Arhivirane rute ne mogu se ponovo aktivirati — služe isključivo za uvid i izvještavanje.

