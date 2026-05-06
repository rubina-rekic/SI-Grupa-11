# Domain Model

---

## Uvod

Domenski model predstavlja konceptualnu strukturu sistema za optimizaciju ruta poštanskih sandučića.
On identifikuje ključne objekte u realnom svijetu (poput sandučića, ruta, vozila, pošiljki i korisnika),
njihove karakteristike i međusobne odnose. Model pokriva cjelokupan radni tok — od planiranja rute i dodjele
vozila, preko obilaska sandučića smještenih u različitim objektima (tržni centri, poslovne zgrade, javne površine)
sa vlastitim radnim vremenima i neradnim danima, do detaljne evidencije preuzete pošte, incidenata i finalnog
izvještavanja. Model služi kao osnova za logičko projektovanje baze podataka i implementaciju poslovnih pravila
koja osiguravaju efikasno pražnjenje i punjenje sandučića uz maksimalnu sigurnost i sljedivost podataka.

---

## Glavni entiteti

1. **Korisnik (User)** — Predstavlja uposlenike koji koriste sistem (Administrator, Dispečer, Poštar). Sadrži podatke potrebne za identifikaciju i radnu evidenciju.
2. **Zona dostave (DeliveryZone)** — Geografsko područje (kvart, opština, distrikt) koje objedinjuje grupu sandučića i olakšava dodjelu ruta poštarima.
3. **Domaćin lokacije (HostVenue)** — Objekat u kojem se fizički nalazi sandučić (tržni centar, poslovna zgrada, pošta, javna površina). Ima vlastito radno vrijeme i pravila pristupa.
4. **Radni raspored (WorkingSchedule)** — Definicija radnog vremena objekta domaćina ili samog sandučića po danima u sedmici.
5. **Neradni dan (NonWorkingDay)** — Državni praznici, vjerski praznici i ostali dani kada objekat domaćin ne radi, pa sandučić nije dostupan.
6. **Poštanski sandučić (Mailbox)** — Fizički objekat sa lokacijom, kapacitetom, prioritetom i povezanim domaćinom lokacije.
7. **Vozilo (Vehicle)** — Transportno sredstvo (službeno vozilo, bicikl, motocikl, pješačka tura) koje poštar koristi za obilazak rute.
8. **Dodjela vozila (VehicleAssignment)** — Evidencija kojim vozilom je poštar krenuo na koju rutu, sa stanjem kilometraže i goriva na početku i kraju.
9. **Ruta (Route)** — Dnevni operativni plan generisan za jednog poštara.
10. **Stavka rute (RouteItem)** — Ključna poveznica (međutabela) između rute i sandučića; bilježi realizaciju svakog pojedinačnog zadatka na terenu.
11. **Zapis preuzimanja (CollectionRecord)** — Detaljna evidencija šta je poštar zatekao i preuzeo iz sandučića (količina, težina, da li je bio prazan).
12. **Pošiljka (MailItem)** — Pojedinačna jedinica pošte (pismo, paket, preporučena pošiljka, reklamacija) evidentirana unutar zapisa preuzimanja.
13. **Incident** — Poseban zapis o neregularnoj situaciji na terenu (blokiran pristup, oštećenje, krađa, prepunjen sandučić), povezan sa stavkom rute.
14. **Obavještenje (Notification)** — Sistemska poruka upućena korisniku (dodjela rute, incident, hitan zadatak, upozorenje o zaključavanju računa).
15. **Dnevni izvještaj (DailyReport)** — Sumarni prikaz učinka, preuzete pošte i incidenata za određeni datum.
16. **Evidencija aktivnosti (AuditLog)** — Hronološki zapis svih značajnih radnji u sistemu radi sljedivosti i sigurnosne revizije.

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
* **Status računa:** *[Enum]* Stanje računa ('Aktivan', 'Neaktivan', 'Deaktiviran', 'Zaključan').
* **Brojač neuspjelih prijava:** *[Integer]* Broj uzastopnih neuspjelih pokušaja prijave; resetuje se nakon uspješne prijave.
* **Vrijeme zaključavanja:** *[Timestamp, opcionalno]* Trenutak do kojeg je račun automatski zaključan nakon prekoračenja dozvoljenog broja neuspjelih pokušaja (mehanizam ASP.NET Identity lockout-a).
* **MatičniZonaID:** *[Strani ključ, opcionalno]* Primarna zona dostave za koju je poštar zadužen.

### Zona dostave (DeliveryZone)

* **ZonaID:** Jedinstveni identifikator zone.
* **Naziv zone:** Tekstualni naziv područja (npr. "Centar", "Dobrinja", "Ilidža - Istok").
* **Opis:** Tekstualni opis granica i obuhvata zone.
* **Granice (Poligon):** Lista koordinata koja definiše geografski poligon zone.
* **Procjenjeno vrijeme obilaska:** Prosječno vrijeme (u minutama) potrebno za obilazak cijele zone.
* **Preporučeni tip vozila:** *[Enum]* Sugerisano transportno sredstvo za tu zonu (npr. 'Bicikl' za uže gradsko jezgro, 'Automobil' za prigradska područja).
* **Status:** *[Enum]* 'Aktivna', 'Neaktivna'.

### Domaćin lokacije (HostVenue)

Entitet koji opisuje fizički objekat u kojem se nalazi sandučić — direktno adresira scenarij sa tržnim centrom koji ne radi nedjeljom.

* **VenueID:** Jedinstveni identifikator objekta.
* **Naziv objekta:** Npr. "BBI Centar", "Poslovna zgrada Avaz Twist", "Glavna pošta Sarajevo".
* **Tip objekta:** *[Enum]* 'Tržni centar', 'Poslovna zgrada', 'Stambena zgrada', 'Javna površina', 'Pošta', 'Ustanova', 'Industrijski kompleks'.
* **Adresa:** Puna adresa objekta.
* **Geolokacija:** Latitude i Longitude centralne tačke objekta.
* **Kontakt osoba:** Ime osobe zadužene za pristup (npr. domar, recepcionar).
* **Kontakt telefon:** Telefonski broj kontakt osobe.
* **Zahtijeva najavu:** *[Boolean]* Da li je potrebno unaprijed najaviti dolazak.
* **Napomene za pristup:** Slobodan tekst (npr. "Ulaz iz dvorišta", "Potreban ključ od portira").
* **Status objekta:** *[Enum]* 'Aktivan', 'Privremeno zatvoren', 'Trajno zatvoren'.

### Radni raspored (WorkingSchedule)

* **RasporedID:** Jedinstveni identifikator.
* **VenueID:** *[Strani ključ]* Objekat na koji se raspored odnosi.
* **Dan u sedmici:** *[Enum]* 'Ponedjeljak', 'Utorak', 'Srijeda', 'Četvrtak', 'Petak', 'Subota', 'Nedjelja'.
* **Vrijeme otvaranja:** Vrijeme početka radnog vremena (HH:MM).
* **Vrijeme zatvaranja:** Vrijeme kraja radnog vremena (HH:MM).
* **Neprekidan rad:** *[Boolean]* Ako je 'True', objekat radi 24 sata.
* **Zatvoreno:** *[Boolean]* Ako je 'True', objekat ne radi tog dana u sedmici (npr. tržni centar koji ne radi nedjeljom).
* **Pauza od / Pauza do:** *[Timestamp, opcionalno]* Pauza u toku radnog dana (npr. molitva, ručak).

### Neradni dan (NonWorkingDay)

* **NeradniID:** Jedinstveni identifikator.
* **Datum:** Konkretan kalendarski dan.
* **Naziv praznika:** Npr. "Dan nezavisnosti", "Kurban-Bajram", "Božić", "1. maj".
* **Tip:** *[Enum]* 'Državni praznik', 'Vjerski praznik', 'Neradni dan objekta', 'Vanredno zatvaranje'.
* **Primjenjuje se na:** *[Enum]* 'Cijeli sistem', 'Specifičan objekat', 'Specifična zona'.
* **VenueID:** *[Strani ključ, opcionalno]* Ako se odnosi samo na jedan objekat.
* **ZonaID:** *[Strani ključ, opcionalno]* Ako se odnosi samo na jednu zonu.
* **Napomena:** Dodatni opis razloga.

### Poštanski sandučić (Mailbox)

* **SandučićID:** Jedinstveni identifikator koji se koristi za pretragu.
* **Adresa:** Tekstualni opis lokacije sandučića.
* **Geolokacija:** Sastoji se od dva atributa: 'Latitude' i 'Longitude' (Decimalni brojevi).
* **VenueID:** *[Strani ključ, opcionalno]* Objekat domaćin u kojem se sandučić nalazi (npr. tržni centar). Prazno ako je sandučić na otvorenoj javnoj površini.
* **ZonaID:** *[Strani ključ]* Zona dostave kojoj sandučić pripada.
* **Tip sandučića:** *[Enum]* Vrsta sandučića iz predefinisane liste (npr. 'Standardni', 'Uslužni', 'Poslovni') — utiče na način obrade i prikaz u sučelju.
* **Prioritet:** *[Enum]* Određuje hitnost obilaska: 'Visok', 'Srednji', 'Nizak'.
* **Kapacitet:** Numerička vrijednost zapremine sandučića u litrama ili broju standardnih pisama (bitno za optimizaciju punjenja).
* **Maksimalna težina:** Nosivost sandučića u kilogramima.
* **Status objekta:** *[Enum]* Stanje na terenu: 'Aktivan', 'Neaktivan', 'Oštećen', 'U servisu'.
* **Datum instalacije:** Dan postavljanja sandučića na lokaciju.
* **Datum posljednjeg servisa:** Dan posljednjeg tehničkog pregleda ili popravke.
* **Prati radno vrijeme domaćina:** *[Boolean]* Ako je 'True', sandučić se ne smije planirati u terminu kada domaćin ne radi.

### Vozilo (Vehicle)

Entitet koji odgovara na pitanje "čime je poštar išao".

* **VoziloID:** Jedinstveni identifikator.
* **Registarska oznaka:** Npr. "A12-B-345" (prazno za bicikl ili pješačku turu).
* **Tip vozila:** *[Enum]* 'Automobil', 'Kombi', 'Motocikl', 'Skuter', 'Bicikl', 'Električni bicikl', 'Pješice'.
* **Marka i model:** Npr. "Volkswagen Caddy", "Yamaha NMAX".
* **Godina proizvodnje:** Godina.
* **Nosivost (kg):** Maksimalna masa tereta.
* **Zapremina tovarnog prostora (l):** Korisna za planiranje punjenja sandučića.
* **Tip pogona:** *[Enum]* 'Benzin', 'Dizel', 'Električni', 'Hibrid', 'Ljudski pogon'.
* **Trenutno stanje kilometraže:** Aktuelno stanje brojila (u km).
* **Status vozila:** *[Enum]* 'Raspoloživo', 'U upotrebi', 'Na servisu', 'Van upotrebe'.
* **Datum narednog servisa:** Predviđeni datum redovnog održavanja.

### Dodjela vozila (VehicleAssignment)

* **DodjelaID:** Jedinstveni identifikator.
* **VoziloID:** *[Strani ključ]* Vozilo koje se dodjeljuje.
* **PoštarID:** *[Strani ključ]* Poštar koji preuzima vozilo.
* **RutaID:** *[Strani ključ]* Ruta za koju je vozilo dodijeljeno.
* **Vrijeme preuzimanja:** Timestamp kada je poštar preuzeo vozilo.
* **Vrijeme vraćanja:** Timestamp kada je vozilo vraćeno.
* **Kilometraža na početku:** Stanje brojila pri preuzimanju.
* **Kilometraža na kraju:** Stanje brojila pri vraćanju.
* **Gorivo na početku (%):** Nivo goriva pri preuzimanju.
* **Gorivo na kraju (%):** Nivo goriva pri vraćanju.
* **Napomene:** Eventualna oštećenja, primjedbe.

### Ruta (Route)

* **RutaID:** Jedinstveni identifikator rute.
* **PoštarID:** *[Strani ključ]* Povezuje rutu sa konkretnim Poštarom kome je ista dodjeljena.
* **DispečerID:** *[Strani ključ]* Povezuje rutu sa Dispečerom koji ju je kreirao/odobrio.
* **IzvjestajID:** *[Strani ključ]* Povezuje rutu sa Dnevnim izvještajem kojem pripada.
* **ZonaID:** *[Strani ključ]* Primarna zona dostave koju ruta pokriva.
* **Datum:** Kalendarski dan za koji je ruta validna.
* **Planirano vrijeme početka:** Očekivano vrijeme polaska.
* **Planirano vrijeme završetka:** Očekivano vrijeme završetka.
* **Stvarno vrijeme početka:** Timestamp stvarnog starta.
* **Stvarno vrijeme završetka:** Timestamp stvarnog završetka.
* **Status rute:** *[Enum]* Faze: `Planirana`, `Aktivna`, `Završena`, `Prekinuta`, `Arhivirana`.
* **Razlog prekida:** Tekstualni opis incidenta ako je ruta prekinuta (npr. kvar na vozilu).
* **Ukupna planirana distanca:** Procijenjena kilometraža rute (u kilometrima).
* **Ukupna stvarna distanca:** Pređena kilometraža (popunjava se nakon završetka).

### Stavka rute (RouteItem)

* **StavkaRuteID:** Jedinstveni identifikator.
* **RutaID:** Strani ključ koji povezuje stavku sa konkretnim dnevnim planom.
* **SandučićID:** Strani ključ koji identifikuje sandučić koji treba obići.
* **Redoslijed:** Redni broj stajališta u ruti (npr. 1, 2, 3...).
* **Planirano vrijeme dolaska:** Procjenjeno vrijeme dolaska na sandučić.
* **Status obilaska:** *[Enum]* Ishod zadatka: `Planirano`, `Realizovano`, `Preskočeno`, `Nedostupno`.
* **Tip realizacije:** *[Enum, opcionalno]* Detalj akcije kada je status `Realizovano`: `Ispražnjen`, `Napunjen`, `Oboje`. Polje je `null` za ostale statuse.
* **Vrijeme potvrde:** Timestamp (Datum i vrijeme) kada je poštar kliknuo na potvrdu u aplikaciji.
* **Geo-validacija:** Koordinate poštara u momentu potvrde (koristi se za US-13: provjera blizine sandučiću).
* **Napomena:** Tekstualni opis (npr. "Sandučić blokiran parkiranim vozilom").

### Zapis preuzimanja (CollectionRecord)

Entitet koji direktno odgovara na pitanja: "Šta je pokupio iz sandučića. Da li je bio prazan. Kolika je količina pošte."

* **ZapisID:** Jedinstveni identifikator.
* **StavkaRuteID:** *[Strani ključ]* Stavka rute na koju se zapis odnosi (1:1 odnos sa realizovanom stavkom).
* **Bio prazan:** *[Boolean]* `True` ako u sandučiću nije bilo pošiljki.
* **Ukupan broj pošiljki:** *[Integer]* Koliko je komada pošte preuzeto.
* **Ukupna težina (g):** Procijenjena ukupna masa preuzete pošte.
* **Procenat popunjenosti (%):** Procjena koliko je sandučić bio pun prije pražnjenja.
* **Bio prepunjen:** *[Boolean]* `True` ako je sandučić bio prepun (indikator za povećanje frekvencije obilaska).
* **Fotografija sadržaja:** *[URL, opcionalno]* Putanja do fotografije sandučića prije pražnjenja.
* **Vrijeme preuzimanja:** Timestamp činjenja preuzimanja.
* **Primopredajni broj:** Jedinstveni broj primopredaje za interno knjigovodstvo.

### Pošiljka (MailItem)

Pojedinačna stavka pošte evidentirana unutar zapisa preuzimanja.

* **PosiljkaID:** Jedinstveni identifikator pošiljke.
* **ZapisID:** *[Strani ključ]* Zapis preuzimanja kojem pošiljka pripada.
* **Barkod / Tracking broj:** *[Opcionalno]* Ako pošiljka ima barkod, sistem ga skenira.
* **Tip pošiljke:** *[Enum]* 'Obično pismo', 'Preporučena pošiljka', 'Paket', 'Razglednica', 'Novčana uputnica', 'Reklamacija', 'Ostalo'.
* **Težina (g):** Masa pojedinačne pošiljke.
* **Dimenzije:** Opcionalni opis (duljina × širina × visina u cm).
* **Adresat:** *[Opcionalno]* Primalac ako je čitljiv.
* **Pošiljalac:** *[Opcionalno]* Pošiljalac ako je čitljiv.
* **Hitnost:** *[Enum]* 'Standardna', 'Prioritetna', 'Ekspres'.
* **Status obrade:** *[Enum]* 'Preuzeta', 'U tranzitu', 'Predata u centar', 'Isporučena'.

### Incident

* **IncidentID:** Jedinstveni identifikator.
* **StavkaRuteID:** *[Strani ključ, opcionalno]* Stavka rute na kojoj je incident nastao.
* **RutaID:** *[Strani ključ]* Ruta u okviru koje je incident prijavljen.
* **PrijaviteljID:** *[Strani ključ]* Korisnik koji je prijavio incident.
* **Tip incidenta:** *[Enum]* 'Oštećen sandučić', 'Blokiran pristup', 'Prepunjen sandučić', 'Krađa/Vandalizam', 'Kvar vozila', 'Saobraćajna nezgoda', 'Nepovoljni vremenski uslovi', 'Ostalo'.
* **Ozbiljnost:** *[Enum]* 'Niska', 'Srednja', 'Visoka', 'Kritična'.
* **Opis:** Detaljan tekstualni opis događaja.
* **Fotografija:** *[URL, opcionalno]* Dokazna fotografija.
* **Vrijeme prijave:** Timestamp kada je incident zabilježen.
* **Status:** *[Enum]* 'Prijavljen', 'U obradi', 'Riješen', 'Eskaliran'.
* **Riješio (UserID):** *[Strani ključ, opcionalno]* Dispečer/Administrator koji je incident zatvorio.
* **Vrijeme rješavanja:** *[Timestamp, opcionalno]* Kada je incident označen kao riješen.

### Obavještenje (Notification)

* **ObavještenjeID:** Jedinstveni identifikator.
* **PrimalacID:** *[Strani ključ]* Korisnik kojem je obavještenje upućeno.
* **Tip:** *[Enum]* 'Dodjela rute', 'Izmjena rute', 'Incident', 'Sigurnosno upozorenje', 'Sistemska poruka'.
* **Naslov:** Kratki naslov.
* **Sadržaj:** Tekst poruke.
* **Kanal:** *[Enum]* 'U aplikaciji', 'Email', 'SMS', 'Push'.
* **Vrijeme slanja:** Timestamp kreiranja.
* **Pročitano:** *[Boolean]* Da li je korisnik otvorio obavještenje.
* **Vrijeme čitanja:** *[Timestamp, opcionalno]*

### Dnevni izvještaj (DailyReport)

Analitički entitet koji sumira radni dan.

* **IzvjestajID:** Jedinstveni identifikator izvještaja.
* **PregledaoID:** *[Strani ključ]* Povezuje izvještaj sa `UserID` iz tabele Korisnik (Dispečer/Administrator).
* **Datum izvještaja:** Dan na koji se podaci odnose.
* **Broj planiranih obilazaka:** Ukupan broj stavki rute za taj dan.
* **Broj realizovanih obilazaka:** Koliko je stavki uspješno završeno.
* **Broj preskočenih / nedostupnih:** Koliko stavki nije obavljeno.
* **Ukupno preuzetih pošiljki:** Zbir iz svih zapisa preuzimanja.
* **Ukupna težina preuzete pošte (kg):** Sumarni podatak.
* **Broj incidenata:** Zbir prijavljenih incidenata u toku dana.
* **Procenat uspješnosti:** Izračunata efikasnost u procentima (%).
* **Ukupna pređena kilometraža:** Zbir iz svih ruta.
* **Status verifikacije:** *[Enum]* 'Nacrt', 'Verifikovan', 'Arhiviran'.

### Evidencija aktivnosti (AuditLog)

* **LogID:** Jedinstveni identifikator.
* **KorisnikID:** *[Strani ključ]* Korisnik koji je izvršio radnju.
* **Akcija:** *[Enum]* 'Prijava', 'Odjava', 'Kreiranje', 'Izmjena', 'Brisanje', 'Pristup podacima'.
* **Entitet:** Naziv pogođenog entiteta (npr. "Ruta", "Sandučić").
* **EntitetID:** Identifikator pogođenog zapisa.
* **Stara vrijednost:** *[JSON, opcionalno]* Snapshot prije izmjene.
* **Nova vrijednost:** *[JSON, opcionalno]* Snapshot nakon izmjene.
* **IP adresa:** IP sa kojeg je radnja izvršena.
* **Vrijeme:** Timestamp radnje.

---

## Veze između entiteta

U ovom dijelu definisani su odnosi između ključnih objekata sistema, koji osiguravaju integritet podataka i omogućavaju praćenje procesa od planiranja do izvještavanja.

### Korisnik (User) — Ruta (Route) [1:N]
- **Opis:** Jedan **Poštar** može biti zadužen za mnogo različitih ruta tokom vremena, ali jedna konkretna dnevna ruta pripada tačno jednom poštaru. Isto vrijedi i za Dispečera koji kreira/odobrava rutu.
- **Ključ povezivanja:** `PoštarID` i `DispečerID` unutar entiteta **Ruta**.

### Korisnik (Poštar) — Zona dostave (DeliveryZone) [N:1]
- **Opis:** Jedan poštar ima matičnu zonu dostave za koju je primarno zadužen, dok jedna zona može imati više poštara.
- **Ključ povezivanja:** `MatičniZonaID` unutar entiteta **Korisnik**.

### Zona dostave (DeliveryZone) — Poštanski sandučić (Mailbox) [1:N]
- **Opis:** Jedna zona obuhvata više sandučića, a svaki sandučić pripada tačno jednoj zoni.
- **Ključ povezivanja:** `ZonaID` unutar entiteta **Sandučić**.

### Domaćin lokacije (HostVenue) — Poštanski sandučić (Mailbox) [1:N]
- **Opis:** Jedan objekat domaćin (npr. tržni centar) može u sebi imati više sandučića. Sandučić opcionalno pripada domaćinu — sandučići na javnim površinama nemaju domaćina.
- **Ključ povezivanja:** `VenueID` unutar entiteta **Sandučić**.

### Domaćin lokacije (HostVenue) — Radni raspored (WorkingSchedule) [1:N]
- **Opis:** Svaki domaćin ima raspored sa po jednim zapisom za svaki dan u sedmici, čime se modelira scenarij poput tržnog centra koji ne radi nedjeljom.
- **Ključ povezivanja:** `VenueID` unutar entiteta **Radni raspored**.

### Domaćin lokacije (HostVenue) — Neradni dan (NonWorkingDay) [1:N]
- **Opis:** Domaćin može imati vlastita vanredna zatvaranja (inventura, renoviranje) pored opštih državnih praznika koji važe za cijeli sistem.
- **Ključ povezivanja:** `VenueID` unutar entiteta **Neradni dan**.

### Ruta (Route) — Stavka rute (RouteItem) [1:N]
- **Opis:** Svaka ruta se sastoji od više stavki rute (stajališta/zadataka). Jedna stavka rute ne može postojati bez matične rute kojoj pripada.
- **Ključ povezivanja:** `RutaID` unutar entiteta **Stavka rute**.

### Poštanski sandučić (Mailbox) — Stavka rute (RouteItem) [1:N]
- **Opis:** Jedan sandučić se pojavljuje kao cilj u mnogo različitih stavki rute (svaki put kada je planiran za obilazak), ali se jedna konkretna stavka rute odnosi na tačno jedan sandučić.
- **Ključ povezivanja:** `SandučićID` unutar entiteta **Stavka rute**.

### Ruta (Route) — Vozilo (Vehicle) [N:M kroz Dodjela vozila]
- **Opis:** Za svaku rutu evidentira se kojim je vozilom poštar išao. Isto vozilo može biti korišteno za mnogo ruta u različitim danima; jedna ruta ima tačno jednu dodjelu vozila.
- **Ključ povezivanja:** `VoziloID`, `PoštarID` i `RutaID` unutar entiteta **Dodjela vozila**.

### Stavka rute (RouteItem) — Zapis preuzimanja (CollectionRecord) [1:1]
- **Opis:** Svaka realizovana stavka rute ima tačno jedan zapis preuzimanja koji detaljno opisuje šta je poštar zatekao i preuzeo iz sandučića. Stavke koje nisu realizovane (status `Preskočeno` ili `Nedostupno`) nemaju zapis preuzimanja.
- **Ključ povezivanja:** `StavkaRuteID` unutar entiteta **Zapis preuzimanja**.

### Zapis preuzimanja (CollectionRecord) — Pošiljka (MailItem) [1:N]
- **Opis:** Jedan zapis preuzimanja može sadržavati više pojedinačnih pošiljki (pisama, paketa). Ako je sandučić bio prazan, zapis postoji ali nema povezanih pošiljki.
- **Ključ povezivanja:** `ZapisID` unutar entiteta **Pošiljka**.

### Ruta (Route) / Stavka rute (RouteItem) — Incident [1:N]
- **Opis:** Jedna ruta ili konkretna stavka rute može imati više prijavljenih incidenata. Incident je uvijek vezan za rutu, a opcionalno i za konkretnu stavku.
- **Ključ povezivanja:** `RutaID` i `StavkaRuteID` unutar entiteta **Incident**.

### Dnevni izvještaj (DailyReport) — Ruta (Route) [1:N]
- **Opis:** Jedan dnevni izvještaj sumira podatke iz svih ruta koje su se odvijale na taj specifični datum.
- **Ključ povezivanja:** `IzvjestajID` unutar entiteta **Ruta**.

### Dnevni izvještaj (DailyReport) — Korisnik (Dispečer/Admin) [N:1]
- **Opis:** Svaki izvještaj verifikuje jedan uposlenik (Admin ili Dispečer) radi uvida u operativnu efikasnost i rješavanje incidenata prijavljenih sa terena.
- **Ključ povezivanja:** `PregledaoID` unutar entiteta **Dnevni izvještaj**.

### Korisnik (User) — Obavještenje (Notification) [1:N]
- **Opis:** Korisnik prima više sistemskih obavještenja; svako obavještenje je upućeno tačno jednom primaocu.
- **Ključ povezivanja:** `PrimalacID` unutar entiteta **Obavještenje**.

### Korisnik (User) — Evidencija aktivnosti (AuditLog) [1:N]
- **Opis:** Svaka radnja u sistemu se veže za korisnika koji ju je pokrenuo; jedan korisnik generiše mnogo zapisa aktivnosti.
- **Ključ povezivanja:** `KorisnikID` unutar entiteta **Evidencija aktivnosti**.

### Administrator (Admin) — Poštanski sandučić (Mailbox) [1:N]
- **Opis:** Administrator je jedina uloga koja ima pravo vršiti CRUD operacije (kreiranje, čitanje, ažuriranje, brisanje) nad podacima o sandučićima, vozilima, zonama i domaćinima lokacije.
- **Ključ povezivanja:** Logička veza kroz sistemske permisije (implicitno kroz AuditLog).

---

## Poslovna pravila važna za model

### Upravljanje korisnicima
- Korisničke račune za poštare može kreirati isključivo administrator.
- Email / korisničko ime mora biti jedinstveno u cijelom sistemu.
- Inicijalna lozinka mora zadovoljiti minimalne sigurnosne kriterije (min. 8 karaktera, kombinacija slova i brojeva).
- Svaki korisnik koji se prijavljuje prvi put inicijalnom lozinkom mora je promijeniti prije nego što dobije pristup drugim funkcionalnostima.
- Nakon 5 uzastopnih neuspjelih pokušaja prijave sistem automatski zaključava račun (status `Zaključan`) na definisani vremenski period; uspješna prijava resetuje brojač.
- Svaki korisnik ima tačno jednu ulogu koja ne može biti prazna.

### Upravljanje zonama i domaćinima
- Svaki sandučić mora pripadati tačno jednoj aktivnoj zoni dostave.
- Ako se sandučić fizički nalazi unutar objekta domaćina, mora biti povezan sa tim domaćinom preko `VenueID`.
- Radni raspored domaćina mora pokrivati svih 7 dana u sedmici (dan se može označiti kao `Zatvoreno = True`).
- Neradni dan može biti definisan na nivou cijelog sistema (državni praznici), specifičnog objekta (inventura u tržnom centru) ili specifične zone.

### Upravljanje sandučićima
- Sandučić mora imati geografsku lokaciju i adresu definisanu pri unosu.
- Neaktivni sandučić (`Neaktivan`, `Oštećen`, `U servisu`) ne smije biti uključen u automatski generisanu rutu.
- Prioritet sandučića ulazi kao jedan od parametara algoritma optimizacije rute.
- Ako sandučić ima `Prati radno vrijeme domaćina = True`, ne smije se planirati u vremenskom intervalu kada domaćin ne radi (poštovanje radnog rasporeda i neradnih dana).
- Sandučić u tržnom centru koji ne radi nedjeljom **neće** biti uključen u rute za nedjelju.

### Upravljanje vozilima
- Samo vozilo sa statusom `Raspoloživo` može biti dodijeljeno na novu rutu.
- Jedno vozilo u istom vremenskom intervalu može biti dodijeljeno tačno jednoj ruti (jednom poštaru).
- Pri preuzimanju i vraćanju vozila obavezno se evidentira kilometraža i nivo goriva.
- Vozilo sa dospjelim servisom sistem označava upozorenjem i ne preporučuje za dodjelu.
- Tip vozila mora odgovarati preporučenom tipu za zonu (upozorenje, ne tvrdo ograničenje).

### Upravljanje rutama
- Ruta se može dodijeliti samo aktivnom poštaru.
- Svaka ruta mora imati dodijeljeno vozilo prije prelaska u status `Aktivna`.
- Jednom aktiviranoj ruti dispečer ne smije mijenjati redoslijed bez evidentiranja te izmjene (AuditLog).
- Ruta se automatski arhivira kada poštar označi njen završetak.
- Prekinuta ruta mora imati evidentirano koji su sandučići obrađeni, a koji nisu.
- Isti sandučić se ne smije pojaviti dva puta unutar iste rute.
- Pri generisanju rute algoritam mora uvažiti radne rasporede domaćina i neradne dane.

### Realizacija obilaska i preuzimanje pošte
- Poštar može evidentirati samo jedan od sljedećih ishoda po stavci rute: `Realizovano` (uz obavezan tip realizacije), `Nedostupno` ili `Preskočeno`.
- Za svaku stavku sa statusom `Realizovano` i tipom `Ispražnjen` ili `Oboje` mora se kreirati **Zapis preuzimanja**.
- Zapis preuzimanja sa `Bio prazan = False` mora imati najmanje jednu povezanu pošiljku ili evidentiranu ukupnu težinu/broj.
- Ako je sandučić bio prepunjen (`Bio prepunjen = True`), sistem generiše preporuku dispečeru za povećanje frekvencije obilaska.
- Timestamp potvrde generira sistem na strani servera, a ne klijentska aplikacija.
- Nedostupna lokacija mora biti evidentirana s napomenom i, po pravilu, povezanim incidentom kako bi dispečer mogao reagovati.

### Incidenti i obavještenja
- Svaki incident sa ozbiljnošću `Visoka` ili `Kritična` automatski generiše obavještenje dispečeru i administratoru.
- Incident tipa `Kvar vozila` blokira dalje korištenje vozila dok administrator ne promijeni njegov status.
- Oštećen sandučić prijavljen kroz incident automatski dobija status `Oštećen` i izuzima se iz budućih ruta do rješavanja.

### Izvještavanje
- Procenat uspješnosti rute računa se kao: (broj realizovanih stavki / ukupan broj stavki) × 100.
- Dnevni izvještaj agregira sve zapise preuzimanja za taj dan (ukupan broj pošiljki, ukupna težina).
- Arhivirane rute ne mogu se ponovo aktivirati — služe isključivo za uvid i izvještavanje.
- Sve izmjene nad ključnim entitetima (korisnici, rute, sandučići, vozila) evidentiraju se u **Evidenciji aktivnosti** radi sljedivosti.
