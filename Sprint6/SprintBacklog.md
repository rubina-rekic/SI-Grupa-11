## Sprint 6 (PBI-015, PBI-016, PBI-017, PBI-018, PBI-019)
**Cilj:** Napuniti bazu podacima s kojima će sistem raditi i omogućiti osnovne operacije nad tim podacima.

## Tabela sprint backloga

| ID | Naziv stavke / zadatka | Odgovorna osoba | Procjena | Status | Napomena |
|---|---|---|---|---|---|
| PBI-015 / US-11 | Unos novog poštara u sistem | Kerim | 2h | Done | Forma za unos, validacija, toast obavijest, čišćenje forme |
| PBI-015 / US-12 | Validacija duplog ID broja | Emrah | 1h | Done | Provjera jedinstvenosti ID-a, poruka s imenom postojećeg poštara, blokada dugmeta |
| PBI-016 / US-13 | Tabelarni pregled poštara | Rubina | 2h 30min | Done | Prikaz svih poštara u sistemu i status njihove aktivnosti |
| PBI-017 / US-14 | Unos lokacije sandučića putem koordinata | Nejla | 3h | To Do | Forma s koordinatama, validacija raspona, mini-mapa s pinom, dugme Odaberi na mapi |
| PBI-017 / US-15 | Unos tipa i osnovnih podataka sandučića | Aldin | 2h | Done | Dropdown tip, serijski broj, kapacitet, godina instalacije, jedinstvenost serijskog broja |
| PBI-018 / US-16 | Ažuriranje podataka o sandučiću | Ibrahim | 2h | To Do | Forma prepopulirana postojećim podacima, iste validacije kao pri kreiranju, Audit Log |
| PBI-019 / US-17 | Pregled liste sandučića | Faruk | 2h 30min | To Do | Tabela s kolonama, straničenje 25/str, filteri, sortiranje po prioritetu, prazna lista poruka |

---

### PBI-015 Dodavanje poštara

#### User Stories
- **US-11:** Kao administrator, želim unijeti podatke o novom poštaru (ime, prezime, kontakt telefon, ID broj), kako bih ga uključio u bazu aktivnih uposlenika na terenu.
- **US-12:** Kao administrator, želim da sistem provjeri da li poštar sa istim ID brojem već postoji, kako bi se izbjegli dupli unosi i konfuzija pri dodjeli ruta.

#### Poslovna vrijednost
Ovaj modul je ključan za operativno planiranje. Bez tačne baze poštara, dispečer ne može vršiti dodjelu ruta, a sistem ne može pratiti ko je odgovoran za koji sandučić.

#### Prioritet: High

---

#### Detaljna razrada Story-ja

##### ID storyja: US-11
**Naziv storyja:** Unos novog poštara u sistem<br>
**Opis:** Kao **administrator**, želim **popuniti formu sa podacima o novom uposleniku**, kako bi **on postao vidljiv u listi za dodjelu dnevnih ruta**.<br>
**Poslovna vrijednost:** Digitalizacija evidencije terenskih radnika.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Administrator ima pristup panelu za upravljanje osobljem.
- *Pretpostavka:* Kreiranje login naloga i dodjela početnih kredencijala pokriveni su kroz PBI-011.
**Veze sa drugim storyjima:**
- **Osnova za:** US-13 (Pregled liste poštara) i US-23 (Dodjela rute poštaru).

#### Acceptance criteria

- Kada administrator otvori formu za unos, tada sistem mora prikazati sljedeća polja: Ime (text), Prezime (text), Broj telefona (numeric/format), Interni ID poštara (numeric) i Status (dropdown: Aktivan/Neaktivan).
- Kada administrator pokuša spasiti podatke, ako je bilo koje od polja (Ime, Prezime, Telefon, ID) prazno, tada sistem mora spriječiti spašavanje i označiti prazna polja crvenom bojom uz poruku "Polje je obavezno".
- Sistem mora validirati polje "Broj telefona" tako da prihvata isključivo cifre i znak "+", te odbiti unos slova.
- Sistem ne smije dozvoliti unos već postojećeg "Internog ID-a poštara" (dupli ID u bazi) – u tom slučaju mora iskočiti toast obavijest: "Greška: Poštar sa ovim ID brojem već postoji!".
- Kada administrator klikne na dugme "Spasi", ako su svi podaci validni, tada se podaci upisuju u bazu, polja na formi se čiste, a sistem prikazuje zelenu toast obavijest: "Poštar uspješno dodan u evidenciju".
- Kada je poštar spašen, tada on mora postati momentalno pretraživ u listi (US-13) i dostupan za dodjelu ruta (US-23).

---

##### ID storyja: US-12
**Naziv storyja:** Validacija duplog ID broja<br>
**Opis:** Kao **administrator**, želim **da sistem spriječi unos novog poštara ukoliko se njegov ID broj već nalazi u bazi**, kako bi se **izbjegli dupli unosi i konfuzija pri dodjeli ruta**.<br>
**Poslovna vrijednost:** Održavanje integriteta baze podataka i sprečavanje operativnih grešaka prilikom vođenja evidencije.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* ID broj radnika je striktno jedinstven identifikator na nivou cijelog sistema.
- *Otvoreno pitanje:* Da li sistem, u slučaju unosa duplog ID-a, treba odmah ponuditi link za pregled profila postojećeg poštara sa tim ID brojem?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-11 (Unos novog poštara u sistem).

#### Acceptance criteria

- Kada administrator unese vrijednost u polje "Interni ID poštara", ako taj ID već postoji u bazi podataka kod drugog korisnika, tada sistem mora momentalno prikazati crvenu poruku ispod polja: "Ovaj ID je već dodijeljen poštaru [Ime i Prezime]".
- Sistem mora onemogućiti dugme "Spasi" sve dok se ne unese jedinstveni ID broj koji ne postoji u bazi.
- Kada sistem detektuje dupli ID, tada korisnik (administrator) treba dobiti opciju unutar poruke o grešci (link ili dugme) koji vodi na profil poštara koji već koristi taj ID.
- Sistem ne smije dozvoliti slanje forme (submit) čak i ako se zaobiđe frontend validacija; serverska strana mora vratiti grešku (Conflict) ako se pokuša spasiti dupli ID.
- Kada administrator promijeni ID u jedinstvenu vrijednost, tada poruka o grešci mora nestati, a polje se mora vratiti u neutralno/validno stanje.

---

### PBI-016 Pregled liste poštara

#### User Stories
- **US-13:** Kao administrator ili dispečer, želim vidjeti listu svih registrovanih poštara sa njihovim trenutnim statusom, kako bih znao koga mogu zadužiti za nove zadatke.

#### Poslovna vrijednost
Omogućava brz pregled dostupnih ljudskih resursa, što direktno utiče na brzinu reagovanja dispečera pri planiranju vanrednih ili redovnih pražnjenja sandučića.

#### Prioritet: Medium

---

##### ID storyja: US-13
**Naziv storyja:** Tabelarni pregled poštara<br>
**Opis:** Kao **administrator ili dispečer**, želim **imati pregled svih poštara u obliku tabele sa osnovnim podacima i statusom aktivnosti**, kako bih **brzo pronašao određenog uposlenika i procijenio njegovu raspoloživost**.<br>
**Poslovna vrijednost:** Transparentnost i lakša navigacija kroz bazu uposlenika.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Lista prikazuje samo relevantne podatke (npr. bez lozinki).
- *Otvoreno pitanje:* Da li u listi treba biti vidljiva i zadnja poznata lokacija poštara radi bolje koordinacije?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-11 (Unos poštara).

#### Acceptance criteria

- Kada administrator ili dispečer otvori stranicu "Lista poštara", ako u bazi postoje podaci, tada sistem mora prikazati tabelu sa kolonama: ID, Ime i prezime, Kontakt telefon, Status (Dostupan/Zauzet) i Zadnja aktivnost (Vrijeme).
- Sistem mora implementirati straničenje tako da se inicijalno prikazuje 20 poštara po stranici, uz "Lazy loading" ili klasičnu navigaciju (1, 2, 3...) na dnu tabele.
- Kada korisnik pređe kursorom preko statusa "Zauzet", tada sistem mora u malom oblačiću prikazati ID rute na kojoj se poštar trenutno nalazi.
- Sistem mora omogućiti vizuelnu indikaciju statusa:
  - Dostupan: Zeleni krug (poštar je spreman za novu rutu).
  - Zauzet: Narandžasti krug (poštar trenutno obilazi sandučiće).
  - Neaktivan: Crveni krug (račun je deaktiviran).
- Kada administrator klikne na kolonu "Status", tada sistem mora grupisati sve dostupne poštare na vrh liste.
- Korisnik treba imati dugme "Osvježi" koje ažurira statuse poštara u realnom vremenu bez ponovnog učitavanja cijele stranice.
---

### PBI-017 Dodavanje poštanskog sandučića

#### User Stories
- **US-14:** Kao administrator ili dispečer, želim dodati novi poštanski sandučić u sistem unoseći njegovu adresu i precizne GPS koordinate, kako bi on bio vidljiv u evidenciji i mogao biti korišten za planiranje ruta.
- **US-15:** Kao administrator ili dispečer, želim pri unosu definisati tip sandučića i osnovne podatke o njemu, kako bi zapis bio potpun i spreman za dalju obradu u sistemu.

#### Poslovna vrijednost
Precizna evidencija sandučića je temelj optimizacije. Unos GPS koordinata eliminiše nagađanje na terenu, dok evidentiranje tipa i osnovnih podataka omogućava kvalitetnije planiranje obilazaka.

#### Prioritet: High

---

#### Detaljna razrada Story-ja 

##### ID storyja: US-14
**Naziv storyja:** Unos lokacije sandučića putem koordinata<br>
**Opis:** Kao **administrator ili dispečer**, želim **unijeti adresu, geografsku širinu (Latitude) i dužinu (Longitude) za svaki sandučić**, kako bi **sistem mogao evidentirati lokaciju i koristiti je pri generisanju ruta**.<br>
**Poslovna vrijednost:** Omogućava matematičku preciznost pri generisanju dnevnih ruta i smanjuje potrošnju vremena na terenu.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Korisnik ima pristup Google Maps ili sličnom servisu za očitavanje koordinata.
- *Otvoreno pitanje:* Da li sistem treba omogućiti „pinovanje“ lokacije direktno na mapi unutar aplikacije umjesto ručnog kucanja brojeva?
**Veze sa drugim storyjima:**
- **Osnova za:** US-22 (Generisanje dnevne rute).

#### Acceptance criteria

- Kada administrator otvori formu za dodavanje sandučića, tada sistem mora prikazati polja: Adresa (text), Latitude (decimal) i Longitude (decimal).
- Sistem mora validirati unos geografskih koordinata prema standardima:
    - Latitude mora biti broj u rasponu od $-90$ do $90$.
    - Longitude mora biti broj u rasponu od $-180$ do $180$.
- Sistem ne smije dozvoliti spašavanje ako su polja prazna ili sadrže tekst (osim decimalne tačke), uz prikaz poruke: "Unesite ispravan format koordinata (npr. 43.8563)".
- Kada administrator unese koordinate, tada sistem mora (ispod ili pored polja) prikazati mini-mapu sa pinom na toj lokaciji radi vizuelne potvrde adrese.
- Sistem mora omogućiti dugme "Odaberi na mapi" koje otvara interaktivnu mapu; kada korisnik klikne na bilo koju tačku na mapi, tada sistem mora automatski popuniti polja Latitude i Longitude tim koordinatama.
- Sistem mora podržavati preciznost od najmanje 6 decimalnih mjesta kako bi se osigurala tačnost lokacije u krugu od nekoliko metara.

---

##### ID storyja: US-15
**Naziv storyja:** Unos tipa i osnovnih podataka sandučića<br>
**Opis:** Kao **administrator ili dispečer**, želim **prilikom kreiranja sandučića definisati njegov tip i osnovne podatke**, kako bi **zapis bio kompletan i spreman za dalje upravljanje**.<br>
**Poslovna vrijednost:** Omogućava konzistentnu evidenciju sandučića i lakše upravljanje njihovim karakteristikama.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Postoji unaprijed definisan skup tipova sandučića.
- *Otvoreno pitanje:* Koji skup osnovnih podataka je obavezan pored adrese i koordinata?
**Veze sa drugim storyjima:**
- **Nadovezuje se na:** US-14.

#### Acceptance criteria

- Kada administrator otvori formu za novi sandučić, tada sistem mora ponuditi padajući meni (dropdown) "Tip sandučića" sa sljedećim opcijama: Zidni (mali), Samostojeći (veliki), Unutrašnji (stambene zgrade) i Specijalni (prioritetni).
- Sistem mora pored adrese i koordinata (iz US-18) zahtijevati unos sljedećih obaveznih polja:
  - Serijski broj (jedinstveni identifikator na poleđini sandučića).
  - Kapacitet (približan broj pisama koji može primiti).
  - Godina instalacije (za potrebe održavanja).
- Kada administrator pokuša spasiti sandučić bez odabira tipa, tada sistem mora odbiti unos i prikazati poruku: "Odabir tipa sandučića je obavezan".
- Sistem ne smije dozvoliti ručni unos teksta u polje "Tip sandučića" (mora se izabrati isključivo ponuđena opcija) kako bi se izbjegle greške u kucanju.
- Kada je tip sandučića odabran, tada sistem na mapi (US-18) mora automatski promijeniti ikonu pina u boju specifičnu za taj tip (npr. Plava za standardni, Crvena za prioritetni).
- Sistem mora provjeriti jedinstvenost "Serijskog broja"; ako broj već postoji u bazi, prikazati toast obavijest: "Sandučić sa ovim serijskim brojem je već registrovan".
---

### PBI-018 Izmjena podataka o sandučiću

#### User Stories
- **US-16:** Kao administrator, želim izmijeniti lokaciju, tip, prioritet i druge podatke postojećeg sandučića, kako bih osigurao da baza podataka odgovara stvarnom stanju na terenu.

#### Poslovna vrijednost
Održavanje tačnosti baze podataka. Pogrešne ili zastarjele informacije dovode do gubitka vremena poštara na terenu i neispravnog planiranja rute.

#### Prioritet: Medium

---

#### Detaljna razrada Story-ja 

##### ID storyja: US-16
**Naziv storyja:** Ažuriranje podataka o sandučiću<br>
**Opis:** Kao **administrator**, želim **otvoriti formu za uređivanje postojećeg sandučića i spasiti izmjene nad lokacijom, tipom, prioritetom i drugim podacima**, kako bi **promjene bile odmah vidljive svim korisnicima sistema**.<br>
**Poslovna vrijednost:** Fleksibilnost sistema u slučaju urbanističkih promjena ili tehničkih grešaka pri prvom unosu.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sandučić već postoji u bazi podataka.
- *Otvoreno pitanje:* Da li sistem treba čuvati historiju promjena (ko je i kada izmijenio podatke) radi interne kontrole?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-14 i US-15.
- **Povezano sa:** US-18 (Definisanje prioriteta sandučića).

#### Acceptance criteria

- Kada administrator klikne na opciju "Uredi" pored sandučića u listi, tada sistem mora otvoriti formu unaprijed popunjenu trenutnim podacima iz baze (Adresa, Koordinate, Tip, Prioritet).
- Sistem mora primijeniti iste validacijske protokole kao pri kreiranju (US-15, US-18):
  - Koordinate moraju biti u ispravnom rasponu.
  - Obavezna polja ne smiju biti ispražnjena.
- Kada administrator klikne na "Spasi izmjene", ako podaci nisu promijenjeni, tada sistem ne treba slati zahtjev na server, već samo zatvoriti formu.
- Sistem mora u posebnoj tabeli (Audit Log) sačuvati historiju promjena koja sadrži: ID administratora, tačno vrijeme izmjene, stare vrijednosti i nove vrijednosti polja.
- Kada se spašavanje završi, tada sistem mora prikazati zelenu toast obavijest: "Podaci o sandučiću [Serijski broj] su uspješno ažurirani".
- Sistem mora osigurati da se promjena lokacije sandučića (koordinata) momentalno reflektuje na svim mapama koje koriste poštari u realnom vremenu.

---

### PBI-019 Pregled sandučića na listi

#### User Stories
- **US-17:** Kao administrator ili dispečer, želim vidjeti listu svih evidentiranih sandučića kroz jednostavnu tabelu ili listu, kako bih imao pregled nad stanjem i raspoloživim tačkama za planiranje.

#### Poslovna vrijednost
Vizuelni pregled svih tačaka u sistemu omogućava brz uvid u evidenciju sandučića i olakšava operativni rad pri planiranju i kontroli.

#### Prioritet: Medium

---

##### ID storyja: US-17
**Naziv storyja:** Pregled liste sandučića<br>
**Opis:** Kao **administrator ili dispečer**, želim **pregledati sve evidentirane sandučiće kroz jednostavnu tabelu ili listu sa osnovnim podacima**, kako bih **brzo pronašao traženi sandučić i provjerio njegove informacije**.<br>
**Poslovna vrijednost:** Brža navigacija i bolja organizacija resursa.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Lista prikazuje osnovne podatke potrebne za rad (npr. lokaciju, tip i status/prioritet gdje je primjenjivo).
- *Otvoreno pitanje:* Da li u ovoj fazi treba omogućiti i pretragu ili filtriranje po tipu/naselju?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-14 i US-15.

#### Acceptance criteria

- Kada administrator ili dispečer otvori stranicu "Lista sandučića", tada sistem mora prikazati tabelu sa kolonama: Serijski broj, Adresa, Tip, Prioritet (npr. Visok/Srednji/Nizak) i Status (Prazan/Pun).
- Sistem mora implementirati straničenje od 25 sandučića po stranici kako bi se osigurala brzina učitavanja stranice.
- Sistem mora omogućiti filtere iznad tabele koji omogućavaju filtriranje liste prema:
  - Tipu sandučića (Zidni, Samostojeći, itd.).
  - Prioritetu (Visok, Srednji, Nizak).
  - Naselju/Ulici (pretraga po tekstu u polju "Adresa").
- Sistem mora omogućiti sortiranje liste klikom na zaglavlje kolone "Prioritet", tako da se najhitniji sandučići prikažu prvi.
- Kada korisnik klikne na adresu sandučića u tabeli, tada sistem mora otvoriti mini-prozor ili modal sa prikazom te lokacije na mapi (poveznica sa US-18).
- Kada u bazi nema evidentiranih sandučića, tada sistem umjesto prazne tabele mora prikazati poruku: "Baza sandučića je prazna. Kliknite na 'Dodaj novi' za početak."
