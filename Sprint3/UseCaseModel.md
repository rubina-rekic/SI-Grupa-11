## Use Case Model

---

### UC-01: Upravljanje korisnicima i pristupom

**Akter:** Administrator  
**Kratak opis:** Administrator kreira korisničke račune za poštare i dispečere, dodjeljuje im odgovarajuće uloge te time kontroliše koje funkcionalnosti su dostupne svakom korisniku unutar sistema.

**Preduslovi:**
- Administrator je prijavljen na sistem.
- Administrator ima pristup modulu za upravljanje korisnicima.

**Glavni tok:**
1. Administrator odabire opciju za kreiranje novog korisničkog računa.
2. Sistem prikazuje formu za unos podataka (ime, prezime, email/korisničko ime, inicijalna lozinka).
3. Administrator popunjava sva obavezna polja i odabire ulogu novog korisnika (Dispečer ili Poštar).
4. Administrator potvrđuje kreiranje.
5. Sistem validira jedinstvenost emaila/korisničkog imena i snagu lozinke.
6. Sistem kreira korisnički račun s dodijeljenom ulogom i sprema podatke u bazu.
7. Sistem prikazuje potvrdu o uspješnom kreiranju računa.
8. Nakon prijave u sistem, svaki korisnik dobija dashboard i skup funkcionalnosti koji odgovaraju isključivo njegovoj ulozi.

**Alternativni tokovi:**
- *A1 – Email/korisničko ime već postoji:* U koraku 5 sistem utvrđuje da je unijeti identifikator već zauzet. Sistem odbija unos i prikazuje opisnu poruku o grešci. Administrator ispravlja podatak i ponavlja korak 4.
- *A2 – Lozinka ne zadovoljava sigurnosne kriterije:* U koraku 5 sistem utvrđuje da lozinka ne ispunjava definirane zahtjeve. Sistem odbija unos i prikazuje poruku s opisom zahtjeva. Administrator unosi ispravnu lozinku.
- *A3 – Nedostaju obavezna polja:* U koraku 4 administrator nije popunio sva obavezna polja. Sistem ne dozvoljava slanje forme i označava polja koja nedostaju.
- *A4 – Korisnik s ograničenim pravima pokušava pristupiti zabranjenoj stranici:* Sistem odbija pristup i prikazuje poruku o zabrani. Akcija nije izvršena.
- *A5 – Prva prijava korisnika s inicijalnom lozinkom:* Sistem prepoznaje inicijalnu lozinku i preusmjerava korisnika na obaveznu promjenu lozinke. Korisnik pristupa dashboardu tek nakon što postavi novu lozinku.

**Ishod:** Korisnički račun je kreiran s dodijeljenom ulogom. Sistem primjenjuje odgovarajuća prava pristupa, a svaki korisnik vidi isključivo funkcionalnosti relevantne za svoju ulogu.

---

### UC-02: Upravljanje poštarima i poštanskim sandučićima

**Akter:** Administrator (primarni). Dispečer ima read-only pristup listi sandučića kroz modul planiranja ruta (UC-03).  
**Kratak opis:** Administrator evidentira, pregleda i ažurira podatke o poštarima i poštanskim sandučićima, uključujući njihove lokacije, tipove, prioritete i radna pravila, čime se uspostavlja temeljna baza podataka potrebna za planiranje ruta.

**Preduslovi:**
- Administrator je prijavljen na sistem s odgovarajućim pravima.

**Glavni tok:**
1. Administrator pristupa modulu za upravljanje poštarima ili sandučićima.
2. Administrator bira akciju: dodavanje novog zapisa, izmjena postojećeg ili pregled liste.

   **Tok 2a – Dodavanje novog poštara:**
   - Administrator popunjava formu s podacima poštara (ime, prezime, kontakt telefon, ID broj).
   - Sistem provjerava jedinstvenost ID broja i sprema zapis.

   **Tok 2b – Dodavanje novog sandučića:**
   - Administrator unosi adresu, GPS koordinate (Latitude i Longitude) te odabire tip sandučića iz predefinisane liste.
   - Administrator postavlja nivo prioriteta sandučića (Visok, Srednji, Nizak).
   - Administrator definiše radna pravila: vremenski okvir dostupnosti (od–do) i radne dane u sedmici.
   - Sistem validira unesene podatke i sprema zapis.

   **Tok 2c – Izmjena postojećeg sandučića:**
   - Administrator pronalazi sandučić u listi, otvara formu za uređivanje, mijenja željene podatke i sprema izmjene.
   - Sistem odmah prikazuje ažurirane informacije svim korisnicima s pravom pristupa.

   **Tok 2d – Pregled liste:**
   - Sistem prikazuje tabelarni prikaz svih poštara ili sandučića s osnovnim podacima (ime/adresa, tip, prioritet, status).
   - Administrator može pretražiti listu po ID-u ili adresi (minimalno 3 karaktera) ili primijeniti filtere po tipu, statusu i prioritetu. Sistem ažurira prikaz u realnom vremenu.

3. Sistem prikazuje potvrdu o uspješno izvršenoj akciji.

**Alternativni tokovi:**
- *A1 – ID broj poštara već postoji:* U toku 2a sistem odbija unos i prikazuje poruku o grešci. Administrator provjerava i ispravlja podatak.
- *A2 – Nevalidne GPS koordinate:* U toku 2b sistem odbija unos i prikazuje uputu o ispravnom formatu.
- *A3 – Nelogičan vremenski raspon radnih pravila:* Sistem utvrđuje da je „Vrijeme do" uneseno prije „Vremena od" te odbija unos i prikazuje poruku o grešci.
- *A4 – Nedostaju obavezna polja:* Sistem blokira potvrdu unosa dok sva obavezna polja nisu ispunjena.
- *A5 – Pretraga ili filtriranje ne daje rezultate:* Sistem prikazuje praznu listu s informativnom porukom.

**Ishod:** Baza podataka poštara i sandučića je kompletna i ažurna. Svaki sandučić ima evidentiranu lokaciju, tip, prioritet i radna pravila, što predstavlja neophodan ulaz za algoritam optimizacije ruta.

---

### UC-03: Generisanje i upravljanje rutama

**Akter:** Dispečer  
**Kratak opis:** Dispečer pokreće algoritam za automatsko generisanje optimizirane dnevne rute, pregledava i po potrebi ručno prilagođava predloženi redoslijed obilaska, te dodjeljuje finalnu rutu konkretnom poštaru.

**Preduslovi:**
- Dispečer je prijavljen na sistem.
- U sistemu postoje evidentirani sandučići s unesenim GPS koordinatama, prioritetima i radnim pravilima.
- U sistemu postoji barem jedan aktivni poštar.

**Glavni tok:**
1. Dispečer pristupa modulu za planiranje ruta i odabire datum generisanja.
2. Dispečer pokreće algoritam optimizacije klikom na dugme 'Generiši'.
3. Sistem izvršava algoritam uzimajući u obzir GPS koordinate, prioritete sandučića te radna pravila (dostupnost po danima i vremenskim okvirima) za odabrani datum.
4. Sistem kreira prijedlog optimizirane rute i prikazuje je dispečeru: redoslijed obilaska, lista uključenih sandučića, ukupan broj tačaka i procijenjena dužina rute.
5. Dispečer pregledava prijedlog i po potrebi ručno mijenja redoslijed tačaka unutar rute.
6. Dispečer odabire poštara s liste dostupnih radnika i dodjeljuje mu rutu.
7. Sistem uspostavlja vezu između poštara i rute te prikazuje potvrdu o uspješnoj dodjeli.

**Alternativni tokovi:**
- *A1 – Nema dostupnih sandučića za odabrani datum:* U koraku 3 sistem utvrđuje da nema sandučića koji zadovoljavaju kriterije za zadani datum. Sistem ne generiše rutu i prikazuje informativnu poruku.
- *A2 – Dispečer odustaje od ručne izmjene:* Dispečer poništava promjene. Sistem vraća prethodni automatski prijedlog redoslijeda.
- *A3 – Nema dostupnih poštara:* U koraku 6 lista poštara je prazna. Dispečer ne može izvršiti dodjelu dok se poštar ne doda u sistem.

**Ishod:** Optimizirana ruta je dodijeljena poštaru i odmah dostupna u njegovom mobilnom sučelju. Dispečer ima dokumentovan plan obilaska za taj dan.

---

### UC-04: Evidencija obilaska na terenu

**Akter:** Poštar (primarni), Dispečer (pratilac)  
**Kratak opis:** Poštar putem mobilnog sučelja pregledava dodijeljenu rutu i u realnom vremenu evidentira status svakog sandučića tokom obilaska, a dispečer prati napredak i reaguje na eventualne probleme.

**Preduslovi:**
- Poštar je prijavljen na sistem s mobilnog uređaja.
- Dispečer je poštaru dodijelio aktivnu rutu.

**Glavni tok:**
1. Poštar se prijavljuje na sistem. Sistem prepoznaje dodijeljenu aktivnu rutu i prikazuje je u responzivnom sučelju prilagođenom mobilnim uređajima.
2. Poštar pregledava redoslijed obilaska i osnovne informacije o svakoj tački (adresa, tip sandučića).
3. Po dolasku na lokaciju, poštar odabire sandučić s liste i jednim klikom evidentira ishod (status `Realizovano` uz tip realizacije `Ispražnjen` ili `Napunjen`).
4. Sistem bilježi promjenu statusa s vremenskom oznakom i odmah ažurira prikaz za poštara.
5. Ako je lokacija nedostupna, poštar odabire opciju 'Nedostupna lokacija', po potrebi unosi kratku napomenu s razlogom i potvrđuje unos.
6. Sistem evidentira status 'Nedostupno' i ažurira prikaz.
7. Dispečer u realnom vremenu prati napredak rute kroz tabelarni ili vizuelni prikaz: koje su tačke obrađene, preskočene ili označene kao nedostupne.

**Alternativni tokovi:**
- *A1 – Poštar nema dodijeljenu rutu:* Sistem prikazuje informativnu poruku da za taj dan nema dodjeljene rute.
- *A2 – Poštar ne unosi napomenu uz nedostupnu lokaciju:* Napomena nije obavezna. Sistem evidentira status 'Nedostupno' i bez tekstualnog opisa.
- *A3 – Poštar još nije počeo s obilaskom:* Sve tačke imaju status 'Planirano'. Dispečer vidi rutu bez bilješki o napretku.

**Ishod:** Sve akcije poštara na terenu su evidentirane u sistemu s vremenskim oznakama. Dispečer ima ažurnu sliku izvršenja rute, a sistem raspolaže svim podacima potrebnim za generisanje izvještaja.

---

### UC-05: Operativno izvještavanje i historija ruta

**Akter:** Administrator, Dispečer  
**Kratak opis:** Ovlašteni korisnik generiše dnevne i periodične izvještaje o realizaciji obilazaka te pristupa arhivi svih završenih ruta radi retrospektivne analize i kontrole kvaliteta.

**Preduslovi:**
- Korisnik (Administrator ili Dispečer) je prijavljen na sistem s odgovarajućim pravima.
- U sistemu postoje evidentirani podaci o obilascima.

**Glavni tok:**
1. Korisnik pristupa modulu za izvještavanje ili arhivu ruta.

   **Tok 1a – Osnovni dnevni izvještaj:**
   - Korisnik odabire datum i pokreće generisanje.
   - Sistem dohvaća podatke i prikazuje sažetak: broj realizovanih, nerealizovanih i nedostupnih tačaka za odabrani dan.

   **Tok 1b – Prošireno operativno izvještavanje:**
   - Korisnik odabire tip izvještaja (učinak poštara za period ili analiza po tipu sandučića).
   - Korisnik definiše parametre (period, ime poštara i sl.).
   - Sistem izračunava agregirane pokazatelje (npr. procenat uspješnosti: Realizovano / Planirano × 100) i prikazuje rezultate u tabelarnoj formi s opcijom filtriranja po datumu.

   **Tok 1c – Pregled arhive realizovanih ruta:**
   - Korisnik odabire modul 'Arhiva'.
   - Sistem prikazuje listu svih završenih ruta s podacima: datum, ime poštara, ukupan broj sandučića i status rute (Završena/Prekinuta).
   - Korisnik može filtrirati listu po periodu ili imenu poštara.
   - Korisnik odabire konkretnu rutu za detaljan pregled. Sistem prikazuje listu svih sandučića unutar te rute s njihovim statusima i vremenskim oznakama akcija.

2. Sistem prikazuje tražene podatke.

**Alternativni tokovi:**
- *A1 – Nema podataka za odabrani datum ili parametre:* Sistem prikazuje informativnu poruku da za odabrane kriterije ne postoje evidentirani podaci.
- *A2 – Primjena filtera u arhivi ne daje rezultate:* Sistem prikazuje praznu listu s informativnom porukom.

**Ishod:** Korisnik dobija sažetak ili detaljan analitički pregled operativnog učinka koji podržava donošenje odluka zasnovanih na podacima te omogućava retrospektivnu kontrolu i rješavanje eventualnih sporova.

---

*Napomena: Ovaj dokument je živi artefakt. Use Case Model će biti dopunjavan i dorađivan u narednim sprintovima u skladu s razvojem zahtjeva i povratnom informacijom Product Ownera.*