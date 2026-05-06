# Use Case Model

---

### UC-01: Kreiranje korisničkog računa 
* **Akter:** Administrator
* **Naziv use casea:** Kreiranje korisničkog računa
* **Kratak opis:** Administrator kreira profil za novog uposlenika unosom osnovnih podataka i dodjelom uloge.
* **Preduslovi:**<br> 1. Administrator je uspješno autentifikovan na sistemu.<br>
    2. Administrator posjeduje privilegije za upravljanje korisnicima.<br>
    3. Sistem ima aktivnu vezu sa bazom podataka.
* **Glavni tok:**
  1. Administrator odabire opciju za kreiranje novog korisničkog računa.
  2. Sistem prikazuje formu za unos podataka (ime, prezime, email, inicijalna lozinka).
  3. Administrator popunjava sva obavezna polja.
  4. Administrator bira ulogu iz padajućeg menija (Dispečer ili Poštar).
  5. Administrator potvrđuje unos podataka klikom na dugme "Spremi".
  6. Sistem validira jedinstvenost emaila i snagu unesene lozinke.
  7. Sistem trajno pohranjuje podatke o novom korisniku u bazu podataka.
  8. Sistem prikazuje vizuelnu potvrdu o uspješno kreiranom računu.
* **Alternativni tokovi:**
    * **A1 – Email već postoji:** Sistem utvrđuje da je email zauzet, odbija unos i traži od administratora novi email.
    * **A2 – Lozinka preslaba:** Sistem detektuje da lozinka ne ispunjava sigurnosne kriterije i traži ponovni unos.
* **Ishod:** Novi korisnički račun je pohranjen u bazi podataka sa statusom "aktivan".
  
### UC-02: Prijava na sistem 
* **Akter:** Korisnik (Poštar, Dispečer, Administrator)
* **Naziv use casea:** Prijava na sistem
* **Kratak opis:** Korisnik pristupa sistemu koristeći svoje kredencijale uz provjeru identiteta.
* **Preduslovi:** <br> 1. Korisnik posjeduje kreiran račun u sistemu.<br>
    2. Korisnički račun je u statusu "aktivan".<br>
    3. Uređaj sa kojeg se pristupa ima stabilnu internet konekciju. <br>
* **Glavni tok:**
     1. Korisnik pristupa stranici za prijavu.
     2. Korisnik unosi svoj identifikator (email ili korisničko ime — vidjeti OQ-001) i lozinku.
     3. Korisnik potvrđuje unos klikom na dugme "Prijava".
     4. Sistem šalje upit bazi podataka radi provjere kredencijala.
     5. Sistem potvrđuje identitet korisnika i provjerava nivo pristupa (Role-based access).
     6. Sistem dozvoljava pristup i preusmjerava korisnika na početnu stranicu (dashboard) specifičnu za njegovu ulogu.
* **Alternativni tokovi:**
    * **A1 – Pogrešni kredencijali:** Sistem prikazuje poruku o neispravnim podacima.
    * **A2 – Zaključavanje računa:** Nakon 5 neuspješnih pokušaja, sistem privremeno blokira prijavu.
* **Ishod:** Korisnik je uspješno autentifikovan i usmjeren na odgovarajući interfejs.

### UC-03: Obavezna promjena inicijalne lozinke
* **Akter:** Korisnik
* **Naziv use casea:** Obavezna promjena lozinke pri prvoj prijavi
* **Kratak opis:** Korisnik koji se prvi put prijavljuje mora zamijeniti privremenu lozinku personalizovanom.
* **Preduslovi:** <br>1. Za korisnika je prethodno kreiran račun od strane administratora i dodjeljena mu je uloga u sistemu.<br>
  2. Korisnik se prijavljuje sa inicijalnom lozinkom koju je generisao administrator.
* **Glavni tok:**
     1. Sistem detektuje da korisnik pristupa prvi put sa inicijalnom lozinkom.
     2. Sistem automatski blokira pristup dashboardu i prikazuje formu za promjenu lozinke.
     3. Korisnik unosi novu lozinku u predviđeno polje.
     4. Korisnik ponavlja novu lozinku radi potvrde.
     5. Korisnik potvrđuje promjenu klikom na "Ažuriraj lozinku".
     6. Sistem validira novu lozinku i ažurira zapis u bazi podataka.
     7. Sistem prikazuje poruku o uspješnoj promjeni i preusmjerava korisnika na dashboard.
* **Alternativni tokovi:**
    * **A1 – Nova lozinka ista kao inicijalna:** Sistem javlja da lozinka mora biti nova i drugačija.
    * **A2 – Lozinke se ne podudaraju:** Korisnik unese različite lozinke u polja; sistem traži ponovni unos oba polja.
    * **A3 – Napuštanje procesa:** Korisnik pokuša izaći bez promjene; sistem ga automatski odjavljuje.
* **Ishod:** Korisnički račun je osiguran personalizovanom lozinkom.

### UC-04: Odjava sa sistema
* **Akter:** Korisnik
* **Naziv use casea:** Odjava sa sistema
* **Kratak opis:** Siguran prekid aktivne sesije korisnika.
* **Preduslovi:** <br> 1.Korisnik je trenutno prijavljen na sistem.
* **Glavni tok:**
     1. Korisnik odabire opciju "Odjava" u navigacionom meniju.
     2. Sistem prikazuje prozor za potvrdu odjave.
     3. Korisnik potvrđuje da želi prekinuti sesiju.
     4. Sistem uništava aktivnu sesiju i briše lokalne tokene.
     5. Sistem preusmjerava korisnika na ekran za prijavu.
* **Alternativni tokovi:**
    * **A1 – Istek sesije (Timeout):** Korisnik je neaktivan duže od 30 minuta; sistem automatski izvršava odjavu.
    * **A2 – Brzo zatvaranje tab-a:** Korisnik zatvori browser; sistem čisti sesiju pri sljedećoj provjeri servera.
* **Ishod:** Pristup sistemu je prekinut, a podaci korisnika su sigurno odjavljeni.

### UC-05: Autorizacija i kontrola pristupa
* **Akter:** Korisnik
* **Naziv use casea:** Prikaz interfejsa na osnovu uloge
* **Kratak opis:** Sistem kontroliše vidljivost modula na osnovu uloge korisnika (Administrator, Dispečer ili Poštar).
* **Preduslovi:** <br> 1. Korisnik se uspješno prijavio.
* **Glavni tok:**
     1. Sistem učitava ulogu korisnika iz baze podataka tokom procesa prijave.
     2. Sistem provjerava definisane permisije za tu ulogu.
     3. Sistem sakriva module kojima korisnik ne smije pristupiti.
     4. Sistem prikazuje korisniku samo one elemente navigacije koji su mu dozvoljeni.
     5. Korisnik počinje rad u svom personalizovanom okruženju.
* **Alternativni tokovi:**
    * **A1 – Pokušaj neovlaštenog pristupa putanji:** Korisnik pokuša manualno unijeti URL drugog modula; sistem prikazuje stranicu "Pristup odbijen".
    * **A2 – Promjena uloge u toku rada:** Administrator promijeni ulogu korisniku; sistem pri sljedećoj akciji osvježava permisije.
* **Ishod:** Osigurana je kontrola pristupa i spriječena zloupotreba funkcija sistema.

### UC-06: Ažuriranje profila poštara
* **Akter:** Administrator
* **Naziv use casea:** Ažuriranje podataka o poštaru
* **Kratak opis:** Izmjena postojećih informacija o poštaru u sistemu.
* **Preduslovi:** <br> 1.Poštar je već evidentiran u bazi podataka.
* **Glavni tok:**
     1. Administrator otvara listu svih poštara.
     2. Administrator bira opciju "Uredi" pored imena poštara.
     3. Sistem otvara formu sa trenutnim podacima.
     4. Administrator vrši izmjene (npr. kontakt telefon ili prezime).
     5. Administrator potvrđuje izmjene klikom na "Sačuvaj".
     6. Sistem ažurira bazu i prikazuje potvrdu o uspješnoj izmjeni.
* **Alternativni tokovi:**
    * **A1 – Email zauzet:** Administrator pokuša dodijeliti email koji već koristi drugi korisnik; sistem javlja grešku.
    * **A2 – Brisanje obaveznih polja:** Administrator obriše ime i pokuša spasiti; sistem onemogućava akciju.
    * **A3 – Odustajanje:** Administrator bira "Otkaži"; sistem se vraća na listu bez spašavanja izmjena.
* **Ishod:** Podaci o poštaru su ažurni i tačni u svim dijelovima sistema.

### UC-07: Evidencija novog poštanskog sandučića
* **Akter:** Administrator (Dispečer ima isključivo read-only pristup kroz UC-09/UC-10 i modul planiranja ruta)
* **Naziv use casea:** Evidencija novog poštanskog sandučića
* **Kratak opis:** Unos nove lokacije sandučića u sistem uz GPS koordinate i validaciju lokacije.
* **Preduslovi:** <br> 1. Administrator ima pristup modulu za upravljanje resursima.<br>
    2. Sistem je povezan sa OpenStreetMap radi verifikacije.<br>
    3. Baza podataka je dostupna za upis novih zapisa.
* **Glavni tok:**
     1. Administrator odabire opciju "Dodaj novi sandučić".
     2. Sistem prikazuje formu za unos (Adresa, GPS koordinate, Tip sandučića, Prioritet).
     3. Administrator unosi GPS koordinate ili bira lokaciju direktno na mapi.
     4. Sistem vrši automatsko popunjavanje adrese na osnovu koordinata (Reverse Geocoding).
     5. Administrator bira tip iz predefinisane liste (npr. Standardni, Uslužni, Poslovni) i postavlja početni prioritet (Visok/Srednji/Nizak).
     6. Administrator potvrđuje unos klikom na dugme "Dodaj".
     7. Sistem validira podatke i trajno pohranjuje sandučić.
* **Alternativni tokovi:**
    * **A1 – Nevalidne koordinate:** Korisnik unese koordinate van definisane radne zone; sistem javlja grešku.
    * **A2 – Duplikat na lokaciji:** Sistem detektuje da na toj tački već postoji sandučić; sistem nudi opciju spajanja ili odbijanja unosa.
* **Ishod:** Novi resurs je registrovan i spreman za proces planiranja ruta.

### UC-08: Podešavanje tehničkih parametara sandučića
* **Akter:** Administrator
* **Naziv use casea:** Podešavanje kapaciteta i prioriteta
* **Kratak opis:** Definisanje tehničkih specifikacija sandučića koje direktno utiču na algoritam za optimizaciju rute.
* **Preduslovi:** <br> 1. Korisnik je prijavljen sa ulogom "Administrator". <br> 2. Specifični sandučić je prethodno evidentiran u bazi podataka (UC-07). <br> 3. Sistem ima pristup modulu za upravljanje resursima.
* **Glavni tok:**
     1. Administrator otvara listu sandučića kroz kontrolni panel. 
     2. Administrator pronalazi i odabire specifični sandučić za uređivanje parametara.
     3. Sistem prikazuje trenutne vrijednosti kapaciteta, tipa i prioriteta.
     4. Administrator unosi vrijednost maksimalnog kapaciteta (npr. broj pisama ili zapremina).
     5. Administrator postavlja nivo prioriteta (Nizak, Srednji, Visok) na osnovu operativnih potreba i prioriteta obilaska (US-32).
     6. Administrator definiše tip sandučića iz predefinisane liste (Standardni, Uslužni, Poslovni) što utiče na težinski koeficijent u algoritmu.
     7. Administrator potvrđuje izmjene klikom na dugme "Sačuvaj izmjene".
     8. Sistem validira unesene numeričke vrijednosti i ažurira bazu podataka.
* **Alternativni tokovi:**
    * **A1 – Unos nevalidnog kapaciteta:** Administrator unese vrijednost 0 ili negativan broj; sistem onemogućava spašavanje i označava polje crvenom bojom uz poruku "Kapacitet mora biti pozitivan broj".
    * **A2 – Konflikt prioriteta:** Administrator pokuša postaviti "Visok" prioritet na sandučić koji je označen kao "Neaktivan"; sistem šalje upozorenje da status mora biti usklađen sa prioritetom.
    * **A3 – Odustajanje od izmjena:** Administrator zatvara formu bez spašavanja; sistem zadržava stare parametre i vraća korisnika na listu.
* **Ishod:** Tehnički parametri su ažurirani i biće korišteni pri sljedećem pokretanju algoritma za optimizaciju rute (UC-11).

### UC-09: Napredno filtriranje sandučića
* **Akter:** Administrator / Dispečer
* **Naziv use casea:** Napredno filtriranje resursa
* **Kratak opis:** Filtriranje liste sandučića prema tipu, statusu ili prioritetu radi bržeg pronalaženja specifičnih resursa.
* **Preduslovi:** <br> 1. Korisnik je uspješno autentifikovan na sistemu. <br> 2. Otvoren je modul za upravljanje resursima sa učitanom listom sandučića. <br> 3. Sistem ima aktivnu vezu sa bazom podataka.
* **Glavni tok:**
     1. Korisnik otvara panel za filtriranje iznad liste resursa.
     2. Korisnik bira jedan ili više filtera iz padajućih menija (npr. Tip: Ekspresni, Status: Neaktivan, Prioritet: Visok).
     3. Sistem detektuje promjenu odabranih parametara u realnom vremenu.
     4. Sistem vrši asinhroni upit prema bazi podataka (bez osvježavanja cijele stranice).
     5. Sistem ažurira tabelarni prikaz prikazujući samo sandučiće koji zadovoljavaju SVE odabrane kriterije.
     6. Korisnik pregleda filtrirani podskup podataka.
* **Alternativni tokovi:**
    * **A1 – Nema rezultata:** Odabrani filteri ne odgovaraju nijednom sandučiću; sistem prikazuje poruku "Nema rezultata za odabrane kriterije" unutar tabele.
    * **A2 – Resetovanje filtera:** Korisnik klikne na dugme "Resetuj"; sistem trenutno briše sve parametre i vraća puni prikaz liste.
    * **A3 – Uklanjanje pojedinačnog filtera:** Korisnik klikne na ikonu "x" pored određenog filtera; sistem trenutno osvježava listu prema preostalim kriterijima.
* **Ishod:** Korisniku je olakšan rad sa velikim brojem podataka kroz fokusiran i brz prikaz relevantnih informacija.

### UC-10: Brza pretraga resursa
* **Akter:** Administrator / Dispečer
* **Naziv use casea:** Brza pretraga po ključnoj riječi
* **Kratak opis:** Pronalaženje sandučića ili poštara brzim unosom teksta.
* **Preduslovi:**  <br> 1. Korisnik se nalazi na modulu sa listom podataka.<br>
2. Korisnik ima rolu administrator ili dispečer.<br>
* **Glavni tok:**
     1. Korisnik pozicionira kursor u polje "Pretraga".
     2. Korisnik unosi pojam (npr. dio adrese).
     3. Sistem dok korisnik kuca filtrira prikazane rezultate.
     4. Sistem ističe traženi pojam u listi.
     5. Korisnik pronalazi traženi resurs.
* **Alternativni tokovi:**
    * **A1 – Specijalni karakteri:** Korisnik unosi simbole; sistem ih tretira kao običan tekst.
    * **A2 – Nema podudaranja:** Korisnik unese pojam koji ne postoji; sistem prikazuje informaciju da nema rezultata.
* **Ishod:** Ubrzan je proces pronalaženja specifičnih informacija unutar sistema.

### UC-11: Generisanje optimalne rute
* **Akter:** Dispečer 
* **Naziv use casea:** Automatska optimizacija ruta
* **Kratak opis:** Sistem izračunava najbrži put za pražnjenje/punjenje sandučića koristeći matematički algoritam i kartografske podatke.
* **Preduslovi:** <br> 1. Korisnik je autentifikovan sa ulogom "Dispečer". <br>
    2. U sistemu su definisani aktivni sandučići sa validnim GPS koordinatama.<br>
    3. Definisana je polazna tačka (npr. centralna poslovnica/depo) od koje ruta počinje.<br>
    4. Sistem ima stabilnu vezu sa vanjskim servisom za navigaciju.
* **Glavni tok:**
     1. Dispečer odabire opciju "Generiši rutu".
     2. Sistem filtrira sandučiće koji su označeni za pražnjenje/punjenje i uzima u obzir njihove prioritete (US-32, US-33).
     3. Sistem pokreće algoritam za optimizaciju (nearest-neighbor heuristika nad GPS koordinatama, vidjeti Architecture AR-002).
     4. Sistem koristi **OpenStreetMap** podlogu za iscrtavanje aproksimativno optimalnog redoslijeda obilaska.
     5. Sistem prikazuje detaljnu procjenu trajanja rute i ukupne kilometraže.
     6. Dispečer vrši finalnu provjeru generisanog plana.
* **Alternativni tokovi:**
    * **A1 – Nema sandučića za obradu:** Sistem obavještava korisnika da trenutno nema sandučića koji zahtijevaju intervenciju.
    * **A2 – Servis za mape nedostupan:** Sistem javlja grešku pri povezivanju sa OSM servisom i onemogućava iscrtavanje rute.
    * **A3 – Nemoguće pronaći put:** Sistem detektuje da su neke lokacije nedostupne (npr. na ostrvu bez puta); sistem označava sporne lokacije i nudi parcijalnu rutu.
* **Ishod:** Kreiran je matematički optimalan plan kretanja koji je spreman za dodjelu poštaru.
  
### UC-12: Ručna modifikacija i potvrda rute
* **Akter:** Dispečer
* **Naziv use casea:** Ručna modifikacija rute
* **Kratak opis:** Dispečer vrši korekcije na ruti koju je sistem predložio prije finalizacije.
* **Preduslovi:** <br> 1. Sistem je generisao prijedlog rute.
* **Glavni tok:**
     1. Dispečer pregleda generisanu rutu na mapi.
     2. Dispečer klikom na tačku uklanja sandučić iz rute.
     3. Dispečer prevlačenjem tačaka mijenja redoslijed posjeta.
     4. Sistem trenutno ažurira ukupne parametre rute (vrijeme/put).
     5. Dispečer potvrđuje finalni plan rute.
* **Alternativni tokovi:**
    * **A1 – Poništavanje izmjena:** Dispečer bira opciju "Vrati na predloženo"; sistem briše sve ručne korekcije.
    * **A2 – Ruta bez tačaka:** Dispečer ukloni sve sandučiće; sistem onemogućava potvrdu prazne rute.
* **Ishod:** Ruta je spremna za dodjelu izvršiocu.

### UC-13: Dodjela radnog naloga
* **Akter:** Dispečer
* **Naziv use casea:** Dodjela rute poštaru
* **Kratak opis:** Slanje instrukcija za vožnju na mobilni terminal konkretnog poštara.
* **Preduslovi:** <br> 1.Ruta je potvrđena i postoji barem jedan slobodan poštar.
* **Glavni tok:**
     1. Dispečer bira finaliziranu rutu.
     2. Dispečer odabire slobodnog poštara sa ponuđene liste.
     3. Dispečer potvrđuje dodjelu.
     4. Sistem uspostavlja vezu ruta–poštar; poštar će pri sljedećoj prijavi dohvatiti rutu (GET /api/routes/my-today).
     5. Status rute ostaje `Planirana` do momenta kada poštar započne obilazak (tada prelazi u `Aktivna`).
* **Alternativni tokovi:**
    * **A1 – Poštar nema aktivnu sesiju:** Sistem upozorava dispečera da poštar trenutno nije prijavljen, ali dozvoljava dodjelu — ruta će biti dostavljena pri sljedećoj prijavi.
    * **A2 – Otkazivanje dodjele:** Dispečer zatvara prozor; ruta ostaje nedodjeljena (status `Planirana`, bez PoštarID-a).
* **Ishod:** Poštar je obaviješten o svom radnom zadatku.

### UC-14: Monitoring realizacije
* **Akter:** Dispečer
* **Naziv use casea:** Praćenje realizacije ruta na terenu
* **Kratak opis:** Vizuelni uvid u napredak obilaska kroz statuse stavki rute (RouteItem) tokom radnog vremena.
* **Preduslovi:** <br> 1. Postoje aktivne rute koje se trenutno izvršavaju.<br>
    2. Sistem ima stabilnu vezu sa **OpenStreetMap** API-jem.
* **Glavni tok:**
     1. Dispečer otvara ekran za monitoring.
     2. Sistem učitava aktivne rute i statuse svih stavki rute (RouteItem) sa pripadajućim sandučićima.
     3. Sistem prikazuje sandučiće na mapi obojene prema statusu obilaska (`Planirano`, `Realizovano`, `Preskočeno`, `Nedostupno`) i pozicije koje su poštari potvrdili (polje Geo-validacija iz posljednje realizovane stavke).
     4. Sistem vizuelno razlikuje tipove realizacije (`Ispražnjen` / `Napunjen`) na ikonama završenih tačaka.
     5. Podaci se periodično osvježavaju putem HTTP pollinga u intervalu 15–30 sekundi (vidjeti Architecture, Polling vs. WebSocket).
* **Alternativni tokovi:**
    * **A1 – Prekid konekcije na backend:** Sistem prikazuje posljednje poznato stanje sa timestampom i informativnom porukom o prekidu.
    * **A2 – Dugotrajno bez napretka:** Ako u periodu dužem od 15 minuta nema nove potvrde stavke na aktivnoj ruti, sistem vizuelno ističe rutu kao potencijalno zastojnu.
* **Ishod:** Dispečer ima ažuran pregled napretka realizacije svih aktivnih ruta.

### UC-15: Upravljanje evidencijom i profilima poštara
* **Akter:** Administrator
* **Kratak opis:** Unos i ažuriranje specifičnih operativnih podataka poštara koji nisu dio osnovnog login naloga (US-11, US-12).
* **Preduslovi:** 1. Administrator je prijavljen.<br>
    2. Postoji kreiran osnovni korisnički nalog za poštara (UC-01).
* **Glavni tok:**
     1. Administrator otvara modul "Evidencija zaposlenih".
     2. Sistem prikazuje listu svih poštara.
     3. Administrator odabire opciju "Uredi profil" za određenog poštara.
     4. Administrator unosi dodatne informacije: Interni ID broj i kontakt telefon.
     5. Administrator potvrđuje promjene.
     6. Sistem validira da li je Interni ID jedinstven.
     7. Sistem ažurira bazu podataka.
* **Alternativni tokovi:**
    * **A1 – Neispravan format ID-a:** Sistem javlja da ID mora biti numerička vrijednost.
* **Ishod:** Podaci o poštaru su kompletirani i spremni za proces dodjele ruta.

### UC-17: Upravljanje statusom stavke rute na terenu
* **Akter:** Poštar
* **Naziv use casea:** Potvrda realizacije stavke rute (Pražnjenje/Punjenje) / Preskakanje stavke
* **Kratak opis:** Poštar putem mobilne aplikacije evidentira ishod rada na pojedinačnoj stavci rute (RouteItem) uz GPS validaciju. Alternativne ishode (`Nedostupno` zbog prepreke) pokriva UC-18.
* **Preduslovi:** <br> 1. Poštar je prijavljen na mobilnu aplikaciju. <br> 2. Poštaru je dodijeljena aktivna ruta (UC-13). <br> 3. Uređaj ima aktivne lokacijske usluge (GPS).
* **Glavni tok:**
     1. Poštar na mapi ili listi unutar aplikacije bira stavku rute (sandučić) na kojoj se trenutno nalazi.
     2. Poštar odabire opciju "Potvrdi realizaciju" i bira tip realizacije (`Ispražnjen` ili `Napunjen`).
     3. Sistem provjerava GPS lokaciju poštara kako bi potvrdio prisutnost na lokaciji sandučića (geo-validacija).
     4. Poštar potvrđuje akciju u aplikaciji.
     5. Sistem šalje podatak serveru, postavlja status stavke rute (RouteItem) na `Realizovano` uz odabrani tip realizacije (`Ispražnjen` / `Napunjen`) i bilježi serverski timestamp potvrde.
     6. Sistem vizuelno označava stavku na korisničkom interfejsu aplikacije kao realizovanu.
* **Alternativni tokovi:**
    * **A1 – Predaleko od lokacije:** Sistem detektuje da je poštar udaljen više od 50m od koordinata sandučića; sistem šalje upozorenje i traži dodatnu potvrdu ili razlog odstupanja.
    * **A2 – Gubitak konekcije:** Sistem pohranjuje promjenu statusa lokalno na uređaju i vrši sinhronizaciju čim internet veza postane dostupna.
    * **A3 – Preskakanje stavke:** Poštar bira opciju "Preskoči"; sistem postavlja status stavke rute na `Preskočeno` uz obaveznu napomenu s razlogom.
    * **A4 – Nedostupna lokacija (prepreka):** Poštar umjesto potvrde prijavljuje problem — tok prelazi u UC-18, gdje sistem postavlja status stavke rute na `Nedostupno` uz obaveznu napomenu.
* **Ishod:** Ishod stavke rute je evidentiran u skladu s formalnim enumom (`Realizovano` / `Preskočeno` / `Nedostupno`) i vidljiv dispečeru u monitoringu (veza sa US-33).

### UC-18: Evidentiranje nepredviđenih prepreka na ruti
* **Akter:** Poštar
* **Naziv use casea:** Prijava problema na lokaciji
* **Kratak opis:** Poštar prijavljuje nemogućnost pristupa sandučiću ili tehnički kvar uočen na terenu.
* **Preduslovi:** <br> 1. Poštar ima aktivnu dodijeljenu rutu. <br> 2. Postoji fizička ili tehnička prepreka koja onemogućava rad na specifičnoj tački.
* **Glavni tok:**
     1. Poštar unutar radnog naloga bira opciju "Prijavi problem" za određeni sandučić.
     2. Poštar bira tip prepreke iz ponuđenog menija (npr. Kvar brave, Nepristupačan prilaz, Oštećenje sandučića).
     3. Poštar unosi obavezni kratak tekstualni opis prepreke i po potrebi prilaže fotografiju.
     4. Poštar potvrđuje slanje prijave.
     5. Sistem postavlja status stavke rute (RouteItem) na `Nedostupno`, bilježi napomenu s razlogom i serverski timestamp, te obavještava dispečera (vidljivo u UC-14 monitoringu uz kašnjenje polling intervala 15–30s).
     6. Sistem nudi poštaru automatsku navigaciju do sljedeće tačke na ruti.
* **Alternativni tokovi:**
    * **A1 – Odustajanje od prijave:** Poštar zatvara formu prije slanja; sistem ne mijenja status sandučića i vraća na pregled rute.
* **Ishod:** Dispečer je obaviješten o problemu, a podaci su sačuvani za planiranje budućih intervencija održavanja.

### UC-19: Upravljanje postavkama algoritma optimizacije
* **Akter:** Administrator
* **Naziv use casea:** Konfiguracija parametara optimizacije
* **Kratak opis:** Administrator podešava globalne faktore i koeficijente koje algoritam koristi za generisanje najbržih ruta.
* **Preduslovi:** <br> 1. Korisnik je prijavljen s administratorskim pravima. <br> 2. Pristup modulu "Sistemske postavke".
* **Glavni tok:**
     1. Administrator otvara modul za konfiguraciju algoritma.
     2. Administrator podešava prioritet.
     3. Administrator postavlja ograničenja kao što su maksimalno vrijeme trajanja rute ili maksimalan broj sandučića po poštaru.
     4. Administrator sprema nove postavke.
     5. Sistem validira logičku ispravnost parametara i primjenjuje ih na svako sljedeće pokretanje algoritma (UC-11).
* **Alternativni tokovi:**
    * **A1 – Nelogični parametri:** Administrator unese vrijednosti koje bi mogle dovesti do greške u proračunu; sistem nudi opciju "Vrati na podrazumijevane postavke".
* **Ishod:** Algoritam je prilagođen trenutnim operativnim potrebama (veza sa US-32).

### UC-20: Operativno izvještavanje i historija ruta
* **Akter:** Administrator, Dispečer
* **Naziv use casea:** Generisanje izvještaja i pregled arhive
* **Kratak opis:** Korisnik generiše izvještaje o realizaciji obilazaka i pristupa arhivi završenih ruta radi analize učinka i kontrole kvaliteta.
* **Preduslovi:** <br> 1. Korisnik je prijavljen s ulogom Administrator ili Dispečer. <br> 2. U bazi podataka postoje evidentirani podaci o završenim ili prekinutim rutama. <br> 3. Sistem ima pristup modulu za analitiku.
* **Glavni tok:**
     1. Korisnik pristupa modulu za izvještavanje ili arhivu ruta.
     2. **Opcija A (Dnevni izvještaj):** Korisnik bira datum; sistem prikazuje sažetak realizovanih i nerealizovanih tačaka.
     3. **Opcija B (Operativni izvještaj):** Korisnik definiše period i parametre (npr. učinak poštara); sistem računa procenat uspješnosti (Realizovano/Planirano × 100).
     4. **Opcija C (Arhiva):** Korisnik bira listu završenih ruta; sistem prikazuje detalje (datum, poštar, status).
     5. Korisnik po potrebi filtrira rezultate po imenu poštara ili vremenskom periodu.
     6. Sistem prikazuje finalni tabelarni pregled podataka.
* **Alternativni tokovi:**
    * **A1 – Nema podataka:** Za odabrani datum ili parametre ne postoje zapisi; sistem prikazuje poruku "Nema evidentiranih podataka za traženi period".
    * **A2 – Nevalidan period:** Korisnik odabere krajnji datum koji je raniji od početnog; sistem javlja grešku u unosu.
* **Ishod:** Korisnik dobija analitički pregled operativnog učinka koji podržava donošenje odluka zasnovanih na podacima.

---

*Napomena: Ovaj dokument je živi artefakt. Use Case Model će biti dopunjavan i dorađivan u narednim sprintovima u skladu s razvojem zahtjeva i povratnom informacijom Product Ownera.*
