# Dokumentacija Korisničkih Priča (User Stories)

---

## Sprint 5 (PBI-011, PBI-012, PBI-013, PBI-014) 
**Cilj:** Omogućiti kreiranje profila i osigurati zaštitu podataka.

### PBI-011 Kreiranje korisničkog računa poštara

#### User Stories
- **US-01:** Kao administrator, želim kreirati korisnički račun za poštara unosom osnovnih podataka, emaila/korisničkog imena i inicijalne lozinke, kako bi poštar mogao pristupiti sistemu.
- **US-02:** Kao administrator, želim da sistem validira jedinstvenost emaila/korisničkog imena i snagu inicijalne lozinke, kako bi se spriječilo kreiranje neispravnih ili nesigurnih računa.
- **US-03:** Kao administrator, želim dobiti jasnu potvrdu o uspješnom kreiranju računa ili poruku o grešci (npr. zauzet email), kako bih mogao završiti unos i dostaviti kredencijale poštaru.

#### Poslovna vrijednost
Korisnički računi za poštare moraju biti kontrolisano kreirani od strane administratora kako bi pristup sistemu imali samo ovlašteni uposlenici. Time se zadržava sigurnost sistema i jasna veza između poštara, dodijeljenih ruta i aktivnosti na terenu.

#### Prioritet: High

---

#### Detaljna razrada Story-ja

##### ID storyja: US-01
**Naziv storyja:** Administratorsko kreiranje korisničkog računa poštara  
**Opis:** Kao **administrator**, želim **unijeti osnovne podatke poštara, email/korisničko ime i inicijalnu lozinku**, kako bih **kreirao korisnički račun koji poštaru omogućava pristup sistemu**.  
**Poslovna vrijednost:** Osigurava da korisničke račune može otvoriti samo ovlaštena osoba, čime se smanjuje rizik od neovlaštene registracije.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Administrator ima pristup modulu za upravljanje korisnicima i dodavanje novih poštara.
- *Otvoreno pitanje:* Da li se kao primarni identifikator za prijavu koristi email adresa ili posebno korisničko ime?  
**Veze sa drugim storyjima:** Direktna zavisnost od US-02 i US-03.

#### Acceptance criteria

- Kada administrator otvori formu za kreiranje računa, ako klikne na dugme "Kreiraj" bez unesenih podataka, tada sistem mora označiti polja Ime, Prezime, Email i Lozinka crvenom bojom i ispisati poruku "Ovo polje je obavezno" ispod svakog.
- Sistem mora automatski validirati format emaila (mora sadržavati @ i domenu) prije slanja podataka na server.
- Kada administrator unese ispravne podatke i klikne "Kreiraj", tada sistem mora kreirati novi zapis u tabeli Korisnici sa ulogom (role) postavljenom isključivo na "Poštar".
- Sistem ne smije u bazu spasiti lozinku u čitljivom formatu; lozinka mora biti hashirana prije spašavanja radi sigurnosti.
- Kada je račun uspješno kreiran, tada korisnik (administrator) treba dobiti zelenu obavijest u gornjem desnom uglu sa tekstom: "Račun za [Ime Prezime] je uspješno kreiran", a polja na formi se moraju isprazniti.
- Sistem mora automatski poslati sistemski email na adresu novog poštara sa linkom za prvu prijavu (poveznica sa US-06).

---

##### ID storyja: US-02
**Naziv storyja:** Validacija unosa pri kreiranju računa  
**Opis:** Kao **administrator**, želim da **sistem automatski provjerava jedinstvenost emaila/korisničkog imena i kompleksnost inicijalne lozinke**, kako bi se **smanjio rizik od grešaka i sigurnosnih propusta pri otvaranju računa**.  
**Poslovna vrijednost:** Povećanje sigurnosti sistema i integriteta podataka od samog početka korištenja.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Definisan je standard snage lozinke (npr. min. 8 karaktera, broj, veliko slovo).
- *Otvoreno pitanje:* Da li inicijalna lozinka treba biti ručno unesena od strane administratora ili automatski generisana od sistema?
**Veze sa drugim storyjima:** Dio je procesa US-01.


#### Acceptance criteria

- Kada administrator unese email koji već postoji u bazi podataka, ako pokuša spasiti račun, tada sistem mora spriječiti slanje forme i prikazati poruku: "Korisnik sa ovim emailom već postoji".
- Sistem mora provjeravati jedinstvenost emaila u realnom vremenu (asinhrono) ili najkasnije u trenutku klika na dugme "Kreiraj", prije nego što podaci budu procesuirani.
- Kada administrator unosi lozinku, tada sistem mora dinamički prikazivati indikator jačine lozinke koji postaje "Zelen" samo ako su ispunjeni sljedeći uslovi:
    - Minimalno 8 karaktera.
    - Barem jedno veliko slovo.
    - Barem jedan broj.
    - Barem jedan specijalni znak (npr. !, @, #, $).
- Sistem ne smije dozvoliti klik na dugme "Kreiraj" sve dok lozinka ne ispuni gore navedene kriterije (dugme treba biti sivo/onemogućeno).
- Kada administrator unese korisničko ime koje sadrži nedozvoljene karaktere (npr. razmake ili simbole poput / \ |), ako pokuša spasiti podatke, tada sistem mora izbaciti grešku: "Korisničko ime smije sadržavati samo slova, brojeve i donju crtu (_)".

---

##### ID storyja: US-03
**Naziv storyja:** Feedback o statusu kreiranja računa  
**Opis:** Kao **administrator**, želim **primiti vizuelnu potvrdu o uspjehu ili opisnu poruku o grešci**, kako bih **imao informaciju da li je korisnički račun uspješno kreiran i spreman za dodjelu poštaru**.  
**Poslovna vrijednost:** Poboljšanje administratorskog iskustva i smanjenje broja duplih ili neuspjelih pokušaja kreiranja računa.  
**Prioritet:** Medium  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Postoje predefinisane poruke za različite tipove grešaka (npr. "Email već postoji").  
- *Otvoreno pitanje:* Na koji način administrator uručuje inicijalne kredencijale poštaru (npr. usmeno, printano ili kroz interni kanal)?
**Veze sa drugim storyjima:** Uspješan ishod ovog story-ja je preduslov za **US-04** (Prijava korisnika).

#### Acceptance criteria

- Kada administrator klikne na dugme "Kreiraj", ako je server uspješno obradio zahtjev, tada sistem mora prikazati zeleni "Success" modal ili toast obavijest sa tekstom: "Račun za [Ime Prezime] je uspješno kreiran. Podaci su poslani na email korisnika."
- Kada dođe do prekida internet konekcije ili pada servera (greška 500), ako administrator pokuša spasiti podatke, tada sistem mora prikazati crvenu poruku upozorenja: "Sistem trenutno nije dostupan. Molimo pokušajte kasnije ili kontaktirajte IT podršku."
- Sistem mora onemogućiti dugme "Kreiraj" odmah nakon prvog klika, kako bi se spriječilo višestruko slanje istog zahtjeva dok traje obrada.
- Kada dođe do greške, sistem ne smije obrisati unesene podatke iz polja forme, kako bi administrator mogao ispraviti grešku (npr. promijeniti email) bez ponovnog kucanja svega.
- Korisnik treba imati mogućnost da zatvori poruku o uspjehu klikom na "X" ili na dugme "U redu", što ga automatski vraća na listu svih poštara (US-13).

---

### PBI-012 Prijava korisnika

#### User Stories
- **US-04:** Kao registrovani korisnik, želim se prijaviti na sistem koristeći kredencijale koje mi je dodijelio administrator, kako bih pristupio funkcionalnostima aplikacije.
- **US-05:** Kao korisnik, želim da me sistem obavijesti ako unesem pogrešne kredencijale, kako bih znao da trebam ponoviti unos ili resetovati inicijalnu lozinku.
- **US-06:** Kao poštar koji se prvi put prijavljuje, želim biti obavezan promijeniti inicijalnu lozinku prije nastavka rada, kako bih zaštitio svoj korisnički račun.

#### Poslovna vrijednost
Prijava osigurava da samo autentifikovani korisnici mogu manipulisati rutama i podacima o sandučićima. Obavezna promjena inicijalne lozinke pri prvoj prijavi dodatno štiti korisničke račune i smanjuje rizik od neovlaštenog pristupa nakon dodjele kredencijala.


#### Prioritet: High

---

#### Detaljna razrada Story-a

##### ID storyja: US-04
**Naziv storyja:** Osnovna prijava na sistem  
**Opis:** Kao **registrovani korisnik**, želim **unijeti svoje pristupne podatke u login formu**, kako bih **ostvario pristup radnom okruženju**.  
**Poslovna vrijednost:** Osnovni mehanizam autentifikacije i zaštite korisničkog profila.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Korisnik je prethodno dobio kredencijale od administratora kroz PBI-011.
- *Otvoreno pitanje:* Da li sistem treba ograničiti broj neuspješnih pokušaja prijave prije privremenog zaključavanja računa?
**Veze sa drugim storyjima:**
- **Striktna zavisnost:** Zavisi od **US-01** (Kreiranje korisničkog računa poštara).
- **Logički povezano:** Prethodi svim storyjima koji zahtijevaju autorizaciju.

#### Acceptance criteria

- Kada korisnik unese ispravan email i lozinku, ako klikne na dugme "Prijava", tada sistem mora generisati aktivnu sesiju i preusmjeriti korisnika na Dashboard u roku od 2 sekunde.
- Sistem ne smije dozvoliti klik na dugme "Prijava" ako su polja email ili lozinka prazna (dugme treba biti vizuelno onemogućeno).
- Kada korisnik unese nepostojeći email ili pogrešnu lozinku, ako pokuša prijavu, tada sistem mora prikazati generičku poruku o grešci: "Neispravni kredencijali. Pokušajte ponovo."
  
   *Napomena za developera:* Ne govoriti tačno šta je pogrešno (email ili lozinka) radi zaštite od hakerskih napada.

- Sistem mora zapamtiti sesiju korisnika tako da osvježavanje stranice ne izbacuje korisnika iz sistema, sve dok ne klikne na "Odjava" (US-07).
- Kada korisnik unese pogrešnu lozinku 5 puta zaredom, tada sistem mora privremeno zaključati račun na 15 minuta i prikazati toast obavijest: "Račun je privremeno zaključan zbog previše neuspješnih pokušaja."

---

##### ID storyja: US-05
**Naziv storyja:** Rukovanje neispravnim kredencijalima<br>
**Opis:** Kao **korisnik**, želim **jasnu poruku o grešci u slučaju pogrešnog emaila ili lozinke**, kako bih **znao da unos nije ispravan**. <br>
**Poslovna vrijednost:** Poboljšanje korisničkog iskustva i smanjenje broja upita podršci zbog nejasnoća pri prijavi. <br>
**Prioritet:** Medium  <br>
**Pretpostavke i otvorena pitanja:**<br>
- *Pretpostavka:* Poruka ne smije biti previše specifična radi sigurnosti (sprječavanje enumeration napada).<br>
**Veze sa drugim storyjima:** Dio je toka **US-04**.

#### Acceptance criteria
 
- Kada korisnik unese ispravan email, ali pogrešnu lozinku, ako klikne na prijavu, tada sistem mora obrisati sadržaj polja za lozinku i prikazati crvenu poruku ispod forme: "Neispravni kredencijali. Molimo pokušajte ponovo."
- Sistem ne smije dozvoliti slanje zahtjeva na server ako su polja prazna; umjesto toga, polja moraju dobiti crveni okvir čim korisnik klikne na "Prijava".
- Kada korisnik unese email u neispravnom formatu (npr. nedostaje mu "@"), ako pokuša prijavu, tada korisnik treba dobiti toast obavijest ili natpis: "Unesite ispravnu email adresu".
- Sistem mora onemogućiti dugme "Prijava" na 3 sekunde nakon svakog neuspješnog pokušaja, kako bi se spriječilo "brzinsko" pogađanje lozinke.
- Kada korisnik unese kredencijale računa koji je deaktiviran od strane administratora, ako pokuša prijavu, tada mora dobiti poruku: "Vaš račun je deaktiviran. Kontaktirajte administratora."

---

##### ID storyja: US-06
**Naziv storyja:** Obavezna promjena lozinke pri prvoj prijavi  <br>
**Opis:** Kao **poštar koji se prvi put prijavljuje**, želim da me **sistem odmah nakon uspješne autentifikacije preusmjeri na promjenu inicijalne lozinke**, kako bi **moj račun bio zaštićen prije daljnjeg korištenja sistema**. <br>
**Poslovna vrijednost:** Smanjuje rizik da inicijalna lozinka ostane poznata drugim osobama i povećava sigurnost pristupa sistemu.<br>
**Prioritet:** High <br>
**Pretpostavke i otvorena pitanja:**<br>
- *Pretpostavka:* Sistem može prepoznati da li korisnik koristi privremenu/inicijalnu lozinku.<br>
- *Otvoreno pitanje:* Da li nakon prve promjene lozinke korisnik odmah ulazi u sistem ili se mora ponovo prijaviti?<br>
**Veze sa drugim storyjima:** Nadovezuje se na **US-04** i zavisi od **US-02**.

#### Acceptance criteria

- Kada se korisnik (poštar) prvi put prijavi sa inicijalnim kredencijalima, ako je prijava uspješna, tada sistem ne smije otvoriti Dashboard, već mora automatski prikazati ekran "Promjena lozinke".
- Sistem mora onemogućiti navigaciju na bilo koju drugu stranicu (putem menija ili direktnog URL-a) sve dok se proces promjene lozinke uspješno ne završi.
- Kada korisnik unese novu lozinku, ako je ona identična inicijalnoj lozinki koju je dobio od administratora, tada sistem mora odbiti spašavanje i prikazati poruku: "Nova lozinka ne smije biti ista kao privremena."
- Sistem mora primijeniti ista pravila jačine lozinke kao u US-02 (min. 8 karaktera, broj, simbol).
- Kada korisnik uspješno unese i potvrdi novu lozinku, ako klikne na "Spasi", tada sistem mora:
   - Ažurirati lozinku u bazi (hashiranu).
   - Prikazati zelenu toast obavijest: "Lozinka uspješno promijenjena. Dobrodošli!".
   - Preusmjeriti korisnika na Dashboard.
- Sistem ne smije dozvoliti da polje "Nova lozinka" i "Potvrdi lozinku" ostanu prazni prilikom klika na dugme.

---

### PBI-013 Odjava korisnika

#### User Stories
- **US-07:** Kao prijavljeni korisnik, želim se moći odjaviti iz sistema u bilo kojem trenutku, kako bih osigurao da niko drugi ne može pristupiti mojim podacima nakon završetka rada.

#### Poslovna vrijednost
Osigurava sigurnost korisničkih profila i sprečava neovlašteno korištenje sesije. Ovo je kritično u radnim okruženjima gdje više uposlenika može koristiti isti uređaj.

#### Prioritet: Medium

---

#### Detaljna razrada Story-ja 

##### ID storyja: US-07
**Naziv storyja:** Sigurna odjava iz sistema<br>
**Opis:** Kao **prijavljeni korisnik**, želim **klikom na dugme za odjavu prekinuti aktivnu sesiju**, kako bi **sistem zahtijevao ponovnu prijavu za dalji rad**.<br>
**Poslovna vrijednost:** Zaštita integriteta podataka i sprječavanje zloupotrebe sesije od strane trećih lica.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:**<br>
- *Pretpostavka:* Korisnik se nalazi na bilo kojoj stranici unutar sistema gdje je vidljiv navigacijski meni.<br>
- *Otvoreno pitanje:* Da li sistem treba automatski odjaviti korisnika nakon određenog perioda neaktivnosti (npr. 30 minuta) radi dodatne sigurnosti?<br>
**Veze sa drugim storyjima:**<br>
- **Striktna zavisnost:** Direktno zavisi od **US-04** (Prijava korisnika) – ne može se odjaviti neko ko nije prijavljen.<br>
- **Logički slijed:** Nakon odjave, korisnik se preusmjerava na početnu stranicu ili formu za ponovnu prijavu.

#### Acceptance criteria

- Kada korisnik klikne na dugme "Odjava" u navigacionom meniju, ako potvrdi akciju (opcionalno), tada sistem mora momentalno uništiti aktivnu sesiju na serveru i preusmjeriti korisnika na početni ekran za prijavu.
- Sistem mora prikazati toast obavijest nakon preusmjeravanja sa tekstom: "Uspješno ste se odjavili iz sistema."
- Sistem ne smije dozvoliti korisniku da se vrati na Dashboard ili bilo koju internu stranicu klikom na dugme "Back" u browseru nakon odjave; sistem ga u tom slučaju mora ponovo vratiti na Login.
- Kada je korisnik odjavljen, ako pokuša ručno unijeti URL neke interne stranice (npr. /dashboard ili /postari), tada sistem mora automatski odbiti pristup i preusmjeriti ga na prijavu uz poruku: "Pristup odbijen. Molimo prijavite se."
- Sistem mora osigurati da se pri odjavi obrišu svi lokalno sačuvani podaci o sesiji (npr. cookies ili local storage) kako bi se spriječilo neovlašteno korištenje istog računara.

---

### PBI-014 Uloge i pristup po ulozi

#### User Stories
- **US-08:** Kao administrator, želim definisati različite nivoe pristupa (Administrator, Dispečer, Poštar), kako bih osigurao da svaki korisnik vidi samo relevantne podatke.
- **US-09:** Kao prijavljeni korisnik, želim da me sistem preusmjeri na radnu površinu (dashboard) prilagođenu mojoj ulozi, kako bih odmah mogao započeti sa svojim specifičnim zadacima.
- **US-10:** Kao korisnik sa ograničenim pravima, želim dobiti poruku o zabrani pristupa ako pokušam otvoriti stranicu za koju nemam ovlaštenje, kako bi se spriječila neovlaštena manipulacija podacima.

#### Poslovna vrijednost
Implementacija uloga sprječava ljudske greške i zloupotrebu sistema. Poštari na terenu trebaju jednostavan interfejs za rad, dok dispečeri trebaju kompleksne alate za planiranje. Razgraničenje pristupa štiti bazu podataka od neovlaštenih izmjena.

#### Prioritet: High

---

#### Detaljna razrada Story-ja 

##### ID storyja: US-08
**Naziv storyja:** Definisanje sistemskih uloga<br>
**Opis:** Kao **administrator**, želim **dodijeliti specifičnu ulogu svakom korisničkom računu**, kako bi **sistem mogao kontrolisati dostupne funkcionalnosti**.<br>
**Poslovna vrijednost:** Osnovna kontrola pristupa (Access Control) koja omogućava skalabilnost tima.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Korisnički račun je već kreiran u sistemu.
- *Otvoreno pitanje:* Da li jedan korisnik može imati više uloga istovremeno (npr. dispečer koji je ujedno i administrator)?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-01 (Kreiranje korisničkog računa poštara).
- **Osnova za:** Sve funkcionalnosti koje slijede (dodavanje sandučića, generisanje ruta).

#### Acceptance criteria

- Kada administrator otvori formu za kreiranje ili uređivanje korisnika, ako klikne na polje "Uloga", tada sistem mora ponuditi striktan izbor između dvije opcije: Administrator i Poštar.
- Sistem mora onemogućiti spašavanje računa ako uloga nije odabrana, uz prikaz crvene poruke: "Uloga je obavezno polje".
- Kada je korisniku dodijeljena uloga "Poštar", ako se on prijavi na sistem, tada mu sistem mora sakriti sve administrativne funkcionalnosti (npr. meni za upravljanje korisnicima, dugmad za brisanje sandučića, postavke sistema).
- Sistem ne smije dozvoliti korisniku sa ulogom "Poštar" pristup URL-ovima namijenjenim administratorima (npr. /admin/settings); ako korisnik to pokuša, sistem ga mora preusmjeriti na njegov dashboard uz toast obavijest: "Nemate ovlaštenje za ovu akciju".
- Sistem mora u bazi podataka uz svaki korisnički račun sačuvati ID uloge (Role ID) koji je povezan sa tabelom privilegija, osiguravajući da se prava pristupa provjeravaju pri svakom zahtjevu na server (API level validation).
- Kada administrator promijeni ulogu korisniku, tada ta promjena mora stupiti na snagu odmah (pri sljedećem osvježavanju stranice ili navigaciji korisnika).


---

##### ID storyja: US-09
**Naziv storyja:** Personalizovani dashboard prema ulozi<br>
**Opis:** Kao **prijavljeni korisnik**, želim da **nakon prijave vidim meni i opcije specifične za moju ulogu**, kako bih **brže obavljao posao bez suvišnih informacija**.<br>
**Poslovna vrijednost:** Povećanje efikasnosti uposlenika kroz pojednostavljen i relevantan korisnički interfejs.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**<br>
- *Pretpostavka:* Sistem prepoznaje ID uloge pri svakom učitavanju stranice.
**Veze sa drugim storyjima:**
- **Zavisi od:** US-04 (Prijava korisnika) i US-08 (Definisanje uloga).

#### Acceptance criteria

- Kada se korisnik prijavi sa ulogom "Administrator", ako je preusmjeren na dashboard, tada sistem u bočnom meniju (sidebar) mora prikazati opcije: Upravljanje korisnicima, Pregled sandučića, Statistika sistema i Postavke.
- Kada se korisnik prijavi sa ulogom "Poštar", ako je preusmjeren na dashboard, tada sistem u meniju smije prikazati isključivo: Moja današnja ruta, Mapa sandučića i Prijava problema na terenu.
- Sistem mora na vrhu dashboarda ispisati personalizovanu poruku dobrodošlice koja sadrži ime korisnika i naziv njegove uloge (npr. "Dobrodošli, [Ime], (Poštar)").
- Sistem ne smije iscrtati administrativne grafikone ili brojače (npr. "Ukupan broj poštara") ako je prijavljeni korisnik u ulozi Poštara.
- Kada administrator klikne na opciju "Upravljanje korisnicima", tada mu sistem mora omogućiti pristup svim CRUD operacijama (US-01, US-11), dok ta opcija za poštara mora biti potpuno nevidljiva.
- Sistem mora provjeravati autorizaciju pri svakom učitavanju komponente; ako korisnik bez uloge pokuša pristupiti dashboardu, sistem ga mora vratiti na login ekran uz toast obavijest: "Sesija istekla ili nemate pristup".

---

##### ID storyja: US-10
**Naziv storyja:** Restrikcija neovlaštenog pristupa<br>
**Opis:** Kao **korisnik sa ograničenim pravima**, želim da mi **sistem onemogući direktan pristup URL-ovima koji nisu namijenjeni mojoj ulozi**, kako bi **sigurnost podataka ostala netaknuta**.<br>
**Poslovna vrijednost:** Sprječavanje sigurnosnih propusta (tzv. „Insecure Direct Object References“).<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:**<br>
- *Pretpostavka:* Čak i ako korisnik zna tačan link (npr. `/admin/delete`), sistem mu ne smije dozvoliti izvršenje akcije.
- *Otvoreno pitanje:* Da li sistem treba logirati svaki neuspješan pokušaj pristupa zabranjenim stranicama radi sigurnosne analize?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-08 (Definisanje uloga).

#### Acceptance criteria

- Kada korisnik sa ulogom "Poštar" pokuša ručno unijeti URL u browser koji pripada administratorskom modulu (npr. /admin/postari, /admin/settings ili /admin/delete-box/1), tada sistem mora spriječiti učitavanje stranice i automatski ga preusmjeriti na njegov Dashboard.
- Sistem mora u trenutku preusmjeravanja prikazati crvenu toast obavijest sa tekstom: "Pristup odbijen: Nemate potrebne privilegije za pregled ove stranice."
- Sistem ne smije izvršiti bilo koju pozadinsku (API) akciju ako token/sesija korisnika ne sadrži ulogu "Administrator", čak i ako je zahtjev poslan direktno preko alata kao što je Postman.
- Kada dođe do pokušaja neovlaštenog pristupa, sistem mora u bazi podataka (Security Log) zabilježiti: ID korisnika, pokušani URL, tačno vrijeme i IP adresu radi sigurnosne analize.
- Sistem mora sakriti sve linkove u navigaciji koji vode ka administratorskim stranicama za svakog korisnika koji nema "Administrator" ulogu, tako da korisnik nema vizuelni put do zabranjenih stranica.


---
## Sprint 6 (PBI-015, PBI-016, PBI-017, PBI-018, PBI-019)
**Cilj:** Napuniti bazu podacima s kojima će sistem raditi i omogućiti osnovne operacije nad tim podacima.

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

---

## Sprint 7 (PBI-020, PBI-021) 
**Cilj:** Definisati logičke parametre sistema kroz određivanje važnosti sandučića i njihovih vremenskih ograničenja.


### PBI-020 Definisanje prioriteta sandučića

#### User Stories
- **US-18:** Kao administrator, želim postaviti ili izmijeniti prioritet za pražnjenje/punjenje sandučića, kako bi sistem znao koje lokacije imaju veći operativni značaj.

#### Poslovna vrijednost
Prioriteti omogućavaju da sistem i dispečer razlikuju kritične od manje kritičnih lokacija, što direktno utiče na kvalitet planiranja i redoslijed obilaska.

#### Prioritet: High

---

##### ID storyja: US-18
**Naziv storyja:** Postavljanje prioriteta sandučića <br>
**Opis:** Kao **administrator**, želim **dodijeliti ili izmijeniti nivo prioriteta za pojedini sandučić**, kako bi **sistem mogao uzeti u obzir njegov značaj pri planiranju pražnjenja i punjenja**.<br>
**Poslovna vrijednost:** Diferencijacija usluge prema važnosti lokacije i bolja podrška algoritmu za planiranje.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Postoji jasan skup vrijednosti prioriteta (npr. nizak, srednji, visok).
- *Otvoreno pitanje:* Da li se prioritet treba mijenjati isključivo ručno ili može biti i automatski predložen na osnovu pravila sistema?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-14 i US-15.
- **Utiče na:** US-22 (Generisanje dnevne rute).

#### Acceptance criteria

- Kada administrator otvori formu za sandučić (unos ili uređivanje), tada sistem mora ponuditi padajući meni "Prioritet" sa tri fiksne opcije: Visok (High), Srednji (Medium) i Nizak (Low).
- Sistem mora vizuelno označiti nivoe prioriteta u svim tabelama i mapama koristeći kodiranje bojama:
  - Visok: Crveni indikator (Mora se isprazniti svakodnevno).
  - Srednji: Žuti/Narandžasti indikator (Pražnjenje svaka 2-3 dana).
  - Nizak: Zeleni indikator (Pražnjenje po potrebi ili jednom sedmično).
- Sistem mora omogućiti "Automatski prioritet"; ako je ova opcija uključena, tada sistem automatski postavlja prioritet na "Visok" za sve sandučiće tipa "Specijalni/Prioritetni" (iz US-15) ili one locirane u zoni centra grada.
- Kada administrator ručno promijeni prioritet, tada sistem mora tražiti kratko obrazloženje (npr. "Povećan volumen pošte zbog praznika"), koje se čuva u bazi.
- Sistem ne smije dozvoliti da sandučić ostane bez dodijeljenog prioriteta; inicijalna vrijednost pri kreiranju mora biti "Srednji".
- Kada se prioritet promijeni, tada se ta promjena mora odmah odraziti na listu prioriteta za generisanje rute u US-22 (sandučići sa Visokim prioritetom se automatski pomjeraju na vrh liste za obilazak).

---
### PBI-021 Evidencija radnih pravila sandučića

#### User Stories
- **US-32:** Kao administrator ili dispečer, želim definisati vremenske okvire  unutar kojih je sandučić dostupan za pražnjenje, kako bi algoritam planirao rute u skladu sa radnim vremenom.
- **US-33:** Kao administrator ili dispečer, želim odrediti specifične radne dane za svaki sandučić, kako bi se izbjeglo planiranje obilazaka u danima kada sandučić nije dostupan.

#### Poslovna vrijednost
Uvođenje radnih pravila osigurava da generisane rute budu operativno izvodljive u stvarnim uslovima. Time se eliminišu situacije u kojima poštar dolazi do zaključanog objekta, čime se direktno štedi vrijeme, smanjuju troškovi goriva i povećava ukupna efikasnost logističke mreže.

#### Prioritet: High

---

#### Detaljna razrada Story-ja

##### ID storyja: US-32
**Naziv storyja:** Definisanje vremenskih okvira dostupnosti sandučića <br>
**Opis:** Kao **administrator**, želim **unijeti vrijeme početka i kraja dostupnosti za svaki sandučić**, kako bi **sistem mogao izračunati optimalno vrijeme dolaska poštara**. <br>
**Poslovna vrijednost:** Osigurava da poštar stigne na lokaciju isključivo tokom njenog radnog vremena. <br>
**Prioritet:** High <br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Vrijeme se unosi u formatu HH:mm (24h format).
- *Otvoreno pitanje:* Da li sistem treba dozvoliti unos više različitih termina unutar jednog dana?
**Veze sa drugim storyjima:**
- **Utiče na:** US-19 i US-20 (Algoritam za generisanje ruta).

**Acceptance criteria:**
- Kada administrator otvori formu za sandučić, tada sistem mora prikazati sekciju "Dostupnost" sa poljima: Početak (Time picker) i Kraj (Time picker) u 24-satnom formatu (npr. 08:00 - 16:00).
- Sistem mora onemogućiti spašavanje ako je "Vrijeme do" ranije ili jednako "Vremenu od" (npr. od 12:00 do 10:00), uz prikaz poruke: "Krajnje vrijeme mora biti nakon početnog".
- Sistem mora omogućiti unos do dva odvojena termina dnevno (npr. za objekte koji imaju pauzu, tipa 08:00-12:00 i 14:00-18:00).
- Kada se unose dva termina, sistem mora validirati da se oni ne preklapaju (npr. termin 1: 08:00-12:00, termin 2: 11:00-14:00 mora biti odbijen).
- Sistem mora imati predefinisano polje "24/7 dostupnost" (checkbox); ako je označeno, tada polja za vrijeme postaju neaktivna, a sistem tretira sandučić kao stalno dostupan.
- Kada algoritam za rute (US-20) izračuna da poštar stiže izvan definisanog okvira, tada sistem mora označiti tu tačku na ruti crvenom bojom i prikazati upozorenje administratoru prije finalnog slanja rute poštaru.
- Sistem ne smije dozvoliti unos nepostojećeg vremena (npr. 25:61).

---

##### ID storyja: US-33
**Naziv storyja:** Definisanje radnih dana sandučića <br>
**Opis:** Kao **administrator**, želim **označiti dane u sedmici kada je sandučić dostupan**, kako bi **sistem isključio te lokacije iz ruta tokom neradnih dana**. <br>
**Poslovna vrijednost:** Sprečava greške u planiranju obilazaka vikendom ili specifičnim danima kada sandučići nisu u funkciji. <br>
**Prioritet:** High <br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Defaultna postavka za nove sandučiće je radna sedmica (Pon-Pet).
**Veze sa drugim storyjima:**
- **Zavisi od:** US-15 (Pregled liste sandučića).

**Acceptance criteria:**
- Kada administrator otvori formu za sandučić, tada sistem mora prikazati sekciju "Radni dani" sa sedam checkbox kontrola (Ponedjeljak – Nedjelja).
- Sistem mora pri kreiranju novog sandučića automatski označiti dane od Ponedjeljka do Petka, dok Subota i Nedjelja moraju biti inicijalno odznačeni.
- Sistem ne smije dozvoliti spašavanje sandučića ako nijedan dan nije označen; u tom slučaju mora prikazati poruku: "Sandučić mora imati barem jedan definisan radni dan".
- Kada administrator označi ili odznači dan, tada sistem mora momentalno ažurirati bazu podataka tako da se promjena uzme u obzir pri sljedećem generisanju rute (US-20).
- Sistem mora omogućiti opciju "Označi sve / Odznači sve" radi bržeg unosa podataka za sandučiće koji su dostupni 24/7.
- Kada algoritam (US-20) generiše rutu za npr. Subotu, ako sandučić nema označenu subotu kao radni dan, tada taj sandučić mora biti potpuno izostavljen sa mape obilaska, bez obzira na njegov prioritet.



---
## Sprint 8 (PBI-022, PBI-023, PBI-024, PBI-025)
**Cilj:** Implementirati algoritam za optimizaciju koji na osnovu unesenih parametara generiše najefikasnije rute.

### PBI-022 Generisanje dnevne rute

#### User Stories
- **US-22:** Kao dispečer, želim pokrenuti algoritam za automatsko generisanje dnevne rute za odabranog poštara, kako bih dobio prijedlog obilaska zasnovan na lokacijama i prioritetima sandučića.

#### Poslovna vrijednost
Ovo je srce sistema. Automatizacija rute smanjuje manuelni rad dispečera, štedi vrijeme i osigurava da ključne lokacije ne budu zaboravljene.

#### Prioritet: High 

---

#### Detaljna razrada Story-ja 

##### ID storyja: US-22 
**Naziv storyja:** Automatizovani proračun dnevne rute<br>
**Opis:** Kao **dispečer**, želim **klikom na dugme "Generiši" aktivirati algoritam**, koji će **na osnovu GPS koordinata i prioriteta sandučića kreirati prijedlog dnevne rute za odabranog poštara**.<br>
**Poslovna vrijednost:** Eliminacija manuelnog planiranja i smanjenje ljudske greške.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem ima pristup koordinatama i prioritetima svih relevantnih sandučića.
- *Otvoreno pitanje:* Koji algoritam koristiti u MVP-u s obzirom na broj tačaka i performanse sistema?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-14 i US-18.
- **Osnova za:** US-23, US-24 i US-25.

#### Acceptance criteria

- Kada dispečer klikne na dugme Generiši, tada sistem mora u obzir uzeti isključivo sandučiće koji su: Aktivni (US-13), imaju označen današnji radni dan (US-33) i čiji se vremenski okvir dostupnosti (US-32) podudara sa planiranim vremenom obilaska.
- Sistem mora primijeniti prioritetno ponderisanje tako da sandučići sa statusom Visok prioritet (US-18) imaju prednost u redoslijedu obilaska u odnosu na one sa nižim prioritetom.
- Kada se proces proračuna završi, tada sistem mora prikazati vizuelni prijedlog rute na interaktivnoj mapi (povezana linija između pinova) i hronološku listu adresa sa procijenjenim vremenom dolaska za svaku tačku.
- Sistem mora izvršiti proračun unutar maksimalno 5 sekundi za rute do 50 tačaka; u suprotnom, mora prikazati indikator učitavanja (loader).
- Kada algoritam izračuna da ukupno trajanje rute premašuje 8 sati rada, tada sistem mora prikazati narandžastu toast obavijest: Upozorenje: Ruta premašuje standardno radno vrijeme.
- Sistem mora za MVP verziju koristiti algoritam zasnovan na Euklidskoj udaljenosti $$d = \sqrt{(x_2-x_1)^2 + (y_2-y_1)^2}$$ kako bi osigurali brzinu proračuna.
- Kada u sistemu nema dostupnih sandučića za odabrane parametre, tada sistem mora onemogućiti dugme Generiši i prikazati poruku: Nema dostupnih lokacija za generisanje rute.
---

### PBI-023 Dodjela rute poštaru

#### User Stories
- **US-25:** Kao dispečer, želim dodijeliti generisanu rutu konkretnom poštaru, kako bi on dobio svoj dnevni zadatak za izvršenje.

#### Poslovna vrijednost
Uspostavlja jasnu odgovornost za izvršenje rute i omogućava da poštar na vrijeme dobije plan rada.

#### Prioritet: High

---

##### ID storyja: US-25
**Naziv storyja:** Dodjela rute poštaru<br>
**Opis:** Kao **dispečer**, želim **izabrati poštara iz liste dostupnih radnika i dodijeliti mu generisanu rutu**, kako bi **on mogao započeti sa radnim zadatkom**.<br>
**Poslovna vrijednost:** Uspostavljanje jasne odgovornosti za izvršenje posla.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Ruta je prethodno generisana i spremna za dodjelu.
- *Otvoreno pitanje:* Na koji način poštar dobija obavijest o dodijeljenoj ruti?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-13 (Pregled liste poštara) i US-22 (Generisanje dnevne rute).

#### Acceptance criteria

- Kada je ruta generisana, sistem mora prikazati padajući meni sa listom svih poštara koji imaju status Dostupan (US-13).
- Kada dispečer odabere poštara i klikne na dugme Potvrdi dodjelu, sistem mora u bazi podataka povezati ID poštara sa ID-om rute i promijeniti status poštara u Zauzet.
- Sistem mora u trenutku dodjele poslati push obavijest ili internu poruku na korisnički račun poštara sa tekstom: Nova ruta vam je dodijeljena. Kliknite za pregled.
- Sistem mora onemogućiti dodjelu iste rute više puta ili dodjelu rute poštaru koji već ima aktivan zadatak.
- Kada se proces završi, sistem mora prikazati zelenu toast obavijest: Ruta je uspješno dodijeljena poštaru [Ime i prezime].
- Sistem mora automatski osvježiti tabelarni pregled poštara (US-13) kako bi status novozauzetog radnika odmah bio vidljiv dispečeru.
- Kada dispečer pokuša dodijeliti rutu bez odabranog poštara, sistem mora prikazati poruku upozorenja: Molimo odaberite poštara sa liste prije potvrde.

---

### PBI-024 Pregled detalja rute

#### User Stories
- **US-23:** Kao dispečer, želim pregledati detalje generisane rute, uključujući redoslijed obilaska, uključene sandučiće i osnovne informacije o ruti, kako bih mogao provjeriti njenu logičnost prije dodjele.

#### Poslovna vrijednost
Omogućava provjeru kvaliteta prijedloga rute prije nego što ruta bude poslana poštaru na izvršenje.

#### Prioritet: Medium

---

##### ID storyja: US-23
**Naziv storyja:** Pregled detalja generisane rute<br>
**Opis:** Kao **dispečer**, želim **vidjeti redoslijed obilaska, uključene sandučiće i osnovne detalje rute**, kako bih **mogao provjeriti da li prijedlog odgovara stvarnoj operativnoj potrebi**.<br>
**Poslovna vrijednost:** Bolja kontrola i lakše uočavanje nelogičnosti u predloženoj ruti.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem može prikazati rutu kroz listu i/ili mapu kao dodatni prikaz detalja.
- *Otvoreno pitanje:* Koji skup osnovnih detalja rute mora biti prikazan u MVP verziji?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-22 (Generisanje dnevne rute).

#### Acceptance criteria

- Kada dispečer klikne na generisanu rutu, sistem mora prikazati detaljan hronološki spisak svih sandučića uključenih u taj obilazak prema redoslijedu koji je odredio algoritam.
- Sistem mora za svaki sandučić u listi prikazati osnovne identifikatore: redni broj u ruti, adresu lokacije i serijski broj sandučića.
- Sistem mora u zaglavlju detalja rute prikazati sumarne podatke: ukupan broj tačaka koje treba obići, procijenjenu ukupnu kilometražu i očekivano trajanje obilaska u satima i minutama.
- Sistem mora omogućiti interaktivni pregled rute na mapi gdje su tačke obilaska numerisane (1, 2, 3...) kako bi dispečer vizuelno potvrdio logiku kretanja.
- Kada korisnik klikne na pojedinačni sandučić unutar liste detalja, sistem mora automatski centrirati mapu na tu lokaciju i prikazati dodatne informacije poput prioriteta i radnog vremena sandučića.
- Sistem mora sadržavati opciju Nazad koja omogućava dispečeru povratak na listu svih generisanih ruta bez gubitka trenutnih podataka proračuna.
- Kada ruta sadrži više od 20 tačaka, sistem mora omogućiti pregled liste kroz skrolovanje uz fiksirano zaglavlje sa ukupnim statistikama rute.

---

### PBI-025 Ručna izmjena redoslijeda obilaska

#### User Stories
- **US-24:** Kao dispečer, želim imati mogućnost ručne izmjene redoslijeda obilaska unutar generisane rute, kako bih uvažio nepredviđene okolnosti na terenu.

#### Poslovna vrijednost
Daje dispečeru neophodnu fleksibilnost u situacijama kada automatski prijedlog ne odražava u potpunosti stvarne uslove rada.

#### Prioritet: Medium

---

##### ID storyja: US-24
**Naziv storyja:** Ručna izmjena redoslijeda obilaska <br>
**Opis:** Kao **dispečer**, želim **promijeniti redoslijed tačaka unutar generisane rute**, kako bih **prilagodio plan trenutnim operativnim okolnostima**. <br>
**Poslovna vrijednost:** Fleksibilnost sistema u realnim situacijama. <br>
**Prioritet:** Medium <br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem nakon izmjene čuva novi redoslijed obilaska.
- *Otvoreno pitanje:* Da li sistem treba automatski preračunavati procijenjenu dužinu ili trajanje rute nakon ručne promjene?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-22 i US-23.

#### Acceptance criteria

- Kada dispečer otvori detalje rute (US-23), sistem mora omogućiti interaktivnu promjenu redoslijeda tačaka koristeći drag and drop metodu unutar hronološke liste sandučića.
- Sistem mora dinamički ažurirati numeraciju tačaka na listi i na mapi odmah nakon svake pojedinačne promjene pozicije sandučića u nizu.
- Sistem mora nakon svake ručne izmjene automatski ponovo izračunati i prikazati ažurirane vrijednosti za ukupnu kilometražu i očekivano trajanje rute (ETA).
- Kada dispečer završi sa pomjeranjem tačaka, sistem mora zahtijevati klik na dugme Sačuvaj promjene kako bi novi redoslijed postao trajan u bazi podataka.
- Sistem mora prikazati potvrdu u obliku zelene toast obavijesti: Redoslijed rute je uspješno promijenjen, nakon što se izmjene uspješno zapišu u bazu.
- Kada dispečer pokuša napustiti stranicu bez spašavanja napravljenih izmjena, sistem mora prikazati upozoravajući modal sa pitanjem: "Imate nesačuvane promjene. Želite li ih sačuvati prije odlaska?"
- Sistem ne smije dozvoliti brisanje tačaka iz rute unutar ove funkcionalnosti, već isključivo promjenu njihovog međusobnog redoslijeda.

---
## Sprint 9 (PBI-026, PBI-027, PBI-028, PBI-029, PBI-030)
**Cilj:** Omogućiti poštarima digitalni pristup dodijeljenim rutama putem mobilnog interfejsa, bilježenje statusa sandučića u realnom vremenu.

### PBI-026 Mobilni prikaz dodijeljene rute

#### User Stories
- **US-26:** Kao poštar, želim vidjeti svoju dodijeljenu rutu preko responzivnog web interfejsa, kako bih na mobilnom uređaju imao jasan pregled dnevnog zadatka.

#### Poslovna vrijednost
Povećava brzinu rada na terenu i eliminiše potrebu za papirnim spiskovima ili dodatnim neformalnim kanalima komunikacije.

#### Prioritet: High

---

#### Detaljna razrada Story-ja 

##### ID storyja: US-26
**Naziv storyja:** Mobilni pregled dodijeljene rute <br>
**Opis:** Kao **poštar**, želim **na svom uređaju vidjeti dodijeljenu rutu, redoslijed obilaska i osnovne informacije o tačkama**, kako bih **imao jasan uvid u obim posla za taj dan**. <br>
**Poslovna vrijednost:** Transparentnost rada i bolja organizacija vremena uposlenika. <br>
**Prioritet:** High <br>
**Pretpostavke i otvorena pitanja:** <br>
- *Pretpostavka:* Poštar je prijavljen na sistem i dodijeljena mu je aktivna ruta (US-25).
- *Otvoreno pitanje:* Da li poštar treba vidjeti i procjenu vremena potrebnog za završetak cijele rute?<br>
**Veze sa drugim storyjima:**<br>
- **Direktna zavisnost:** US-25 (Dodjela rute poštaru).

#### Acceptance criteria

- Kada se poštar prijavi na mobilnu aplikaciju, sistem mora automatski učitati i prikazati aktivnu rutu dodijeljenu tom korisniku na početnom ekranu.
- Sistem mora prikazati listu svih sandučića za taj dan poredanih po hronološkom redoslijedu obilaska sa adresom i trenutnom udaljenošću od lokacije poštara.
- Sistem mora u vrhu ekrana prikazati sumarne podatke za poštara: ukupan broj sandučića, preostali broj sandučića za obilazak i procijenjeno vrijeme potrebno za završetak cijele rute.
- Sistem mora omogućiti prebacivanje između tabelarnog prikaza liste i prikaza rute na mapi, pri čemu se prikaz na mapi mora prilagođavati trenutnoj GPS lokaciji poštara.
- Kada poštar klikne na pojedinačnu tačku u listi, sistem mora prikazati detalje sandučića uključujući serijski broj, tip sandučića i polje za bilješke dispečera ako postoji.
- Sistem mora biti u potpunosti responzivan i optimizovan za rad na mobilnim uređajima, sa dugmadi koja su dovoljno velika za laganu navigaciju prstom.
- Kada poštar nema dodijeljenu aktivnu rutu, sistem mora na početnom ekranu prikazati poruku: Trenutno nemate dodijeljenih ruta za danas.
- Sistem mora omogućiti navigaciju do prve ili sljedeće tačke u ruti otvaranjem eksterne mape na uređaju (npr. Google Maps) klikom na adresu sandučića.

---

### PBI-027 Ažuriranje statusa sandučića

#### User Stories
- **US-28:** Kao poštar, želim promijeniti status sandučića tokom obilaska, kako bi dispečer imao informaciju o progresu rute u realnom vremenu.

#### Poslovna vrijednost
Omogućava zatvaranje petlje povratnih informacija između poštara i dispečera i daje uvid u napredak rada na terenu.

#### Prioritet: High

---

##### ID storyja: US-28
**Naziv storyja:** Ažuriranje statusa sandučića tokom obilaska <br>
**Opis:** Kao **poštar**, želim **jednim klikom promijeniti status sandučića tokom obilaska**, kako bi **sistem evidentirao napredak i prikazao ažurno stanje rute dispečeru**. <br>
**Poslovna vrijednost:** Real-time praćenje progresa rada. <br>
**Prioritet:** High <br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem bilježi tačno vrijeme promjene statusa radi kasnije analize.
- *Otvoreno pitanje:* Koji minimalni skup statusa mora postojati u MVP-u?
**Veze sa drugim storyjima:**<br>
- **Zavisi od:** US-26.
- **Utiče na:** US-29 i US-30.

#### Acceptance criteria

- Kada poštar odabere sandučić na mobilnom uređaju, sistem mora prikazati jasno vidljivo dugme za promjenu statusa (npr. Potvrdi pražnjenje).
- Sistem mora u MVP verziji podržavati minimalno tri statusa: Na čekanju (inicijalno stanje), Završeno (uspješno ispražnjeno) i Problem (nemogućnost pristupa ili kvar).
- Kada poštar klikne na dugme za promjenu statusa u Završeno, sistem mora automatski zabilježiti tačno vrijeme (timestamp) i GPS koordinate poštara u trenutku klika radi potvrde lokacije.
- Sistem mora odmah po promjeni statusa vizuelno izmijeniti prikaz te tačke u listi i na mapi (npr. promjena boje pina iz plave u zelenu) kako bi poštar vidio napredak.
- Kada se odabere status Problem, sistem mora otvoriti obavezno polje za kratak unos teksta ili odabir tipa problema (npr. Oštećena brava, Blokiran prilaz) prije čuvanja promjene.
- Sistem mora u realnom vremenu sinhronizovati ove promjene sa dispečerskom konzolom (US-29) kako bi dispečer imao uvid u progres rute bez osvježavanja stranice.
- Kada je status sandučića postavljen na Završeno, sistem mora onemogućiti ponovnu promjenu statusa za taj sandučić unutar iste radne smjene, osim ako administrator ne odobri resetovanje.
- Sistem mora omogućiti rad u offline režimu; ako poštar nema internet konekciju, status se čuva lokalno na uređaju i automatski šalje na server čim se veza uspostavi.

---

### PBI-028 Označavanje nedostupne lokacije

#### User Stories
- **US-29:** Kao poštar, želim evidentirati da određena lokacija nije bila dostupna tokom obilaska, kako bi problem bio zabilježen i vidljiv dispečeru.

#### Poslovna vrijednost
Omogućava da sistem zabilježi operativne izuzetke i da dispečer ima tačnu sliku o tome zašto pojedina tačka nije obrađena.

#### Prioritet: Low

---

##### ID storyja: US-29
**Naziv storyja:** Evidentiranje nedostupne lokacije <br>
**Opis:** Kao **poštar**, želim **označiti lokaciju kao nedostupnu i po potrebi ostaviti kratku napomenu**, kako bi **problem bio evidentiran i mogao se uzeti u obzir u daljem radu**.<br>
**Poslovna vrijednost:** Brže reagovanje na probleme na terenu i kvalitetnija evidencija izuzetaka.<br>
**Prioritet:** Low <br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Nedostupna lokacija predstavlja poseban status ili razlog unutar sistema.
- *Otvoreno pitanje:* Da li je u MVP-u dovoljan tekstualni opis ili je potrebna i fotografija?<br>
**Veze sa drugim storyjima:**<br>
- **Zavisi od:** US-26.<br>
- **Povezano sa:** US-28 i US-30.

#### Acceptance criteria

- Kada poštar unutar mobilne aplikacije označi status sandučića kao Nedostupno, sistem mora obavezno prikazati polje za unos tekstualne napomene.
- Sistem mora onemogućiti spašavanje statusa Nedostupno ukoliko polje Napomena ostane prazno, uz prikaz upozorenja: Molimo unesite razlog nedostupnosti lokacije.
- Sistem u MVP verziji mora podržavati tekstualni opis od minimalno 10 karaktera, dok će se opcija dodavanja fotografije ostaviti za kasniju nadogradnju sistema.
- Kada se potvrdi unos, sistem mora automatski dodijeliti oznaku Nedostupno tom sandučiću u listi zadataka poštara i promijeniti boju indikatora u crvenu.
- Sistem mora zabilježiti GPS lokaciju poštara u trenutku slanja prijave o nedostupnosti kako bi dispečer mogao potvrditi da se radnik zaista nalazi u blizini sandučića.
- Kada se status Nedostupno sačuva, sistem mora automatski poslati hitnu obavijest (alert) u dispečerski centar sa informacijama o sandučiću i razlogu problema.
- Sistem mora omogućiti poštaru da nastavi sa sljedećom tačkom u ruti bez blokiranja aplikacije nakon evidentiranja nedostupnosti.
- Kada dispečer pregleda rutu, sistem mu mora omogućiti direktan klik na napomenu poštara kako bi mogao odmah procijeniti da li je potrebna intervencija ili promjena prioriteta za naredni dan.


---

### PBI-029 Praćenje statusa rute od strane dispečera

#### User Stories
- **US-30:** Kao dispečer, želim imati uvid u to koji su sandučići obrađeni, preskočeni ili problematični, kako bih mogao pratiti status izvršenja dodijeljene rute.

#### Poslovna vrijednost
Dispečer dobija operativni pregled nad izvršenjem rute i može pravovremeno reagovati kada dođe do odstupanja ili problema na terenu.

#### Prioritet: High

---

##### ID storyja: US-30
**Naziv storyja:** Operativni pregled statusa rute<br>
**Opis:** Kao **dispečer**, želim **vidjeti status obrađenih, preskočenih ili problematičnih sandučića unutar rute**, kako bih **mogao pratiti realizaciju i reagovati na probleme**.<br>
**Poslovna vrijednost:** Bolja kontrola izvršenja dnevnih zadataka i pravovremeno donošenje operativnih odluka. <br>
**Prioritet:** High <br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Statusi se osvježavaju na osnovu akcija koje poštar radi na terenu.
- *Otvoreno pitanje:* Da li je u MVP-u dovoljan tabelarni pregled ili je potreban i vizuelni prikaz na mapi? <br>
**Veze sa drugim storyjima:** <br>
- **Zavisi od:** US-28 i US-29.

#### Acceptance criteria

- Kada dispečer otvori stranicu Operativni pregled, sistem mora prikazati listu svih aktivnih ruta sa imenom poštara i procentualnim indikatorom progresa (npr. 45% završeno).
- Sistem mora za svaku pojedinačnu rutu prikazati sumarnu statistiku: ukupan broj sandučića, broj uspješno ispražnjenih, broj nedostupnih i broj onih koji su još na čekanju.
- Sistem u MVP verziji mora ponuditi oba prikaza: tabelarni spisak sa statusima i interaktivnu mapu na kojoj se boja pinova mijenja u realnom vremenu (npr. zelena za završeno, crvena za nedostupno, plava za čekanje).
- Kada dispečer klikne na sandučić sa statusom Nedostupno, sistem mora u sklopu pregleda odmah prikazati napomenu koju je poštar unio na terenu (US-29).
- Sistem mora osigurati automatsko osvježavanje podataka na dispečerskoj konzoli (npr. svakih 60 sekundi) bez potrebe za ručnim ponovnim učitavanjem cijele stranice.
- Kada je ruta u potpunosti završena (svi sandučići imaju status Završeno ili Nedostupno), sistem mora označiti rutu statusom Arhivirana i zabilježiti ukupno vrijeme trajanja obilaska.
- Sistem mora omogućiti dispečeru filtriranje pregleda prema statusu (npr. prikaži samo rute koje imaju barem jednu problematičnu lokaciju) radi brže reakcije na terenu.


---

### PBI-030 Osnovni dnevni izvještaj

#### User Stories
- **US-31:** Kao administrator ili dispečer, želim generisati osnovni dnevni izvještaj o realizovanim i nerealizovanim obilascima, kako bih imao sažet pregled učinka za dati dan.

#### Poslovna vrijednost
Izvještaj daje pregled realizacije rada i predstavlja osnovu za internu analizu, evidenciju i kasnije unapređenje operativnih procesa.

#### Prioritet: Low

---

##### ID storyja: US-31
**Naziv storyja:** Generisanje osnovnog dnevnog izvještaja <br>
**Opis:** Kao **administrator ili dispečer**, želim **dobiti osnovni dnevni izvještaj o realizovanim i nerealizovanim obilascima**, kako bih **imao sažet pregled izvršenja rada za određeni dan**. <br>
**Poslovna vrijednost:** Omogućava osnovno operativno izvještavanje i pregled učinka. <br>
**Prioritet:** Low <br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem već raspolaže podacima o statusima obilazaka i sandučića za posmatrani dan. <br>
- *Otvoreno pitanje:* Koji minimum informacija treba ući u MVP izvještaj (broj završenih, broj nedostupnih, broj nezavršenih lokacija i sl.)? <br>
**Veze sa drugim storyjima:** <br>
- **Zavisi od:** US-28, US-29 i US-30.

#### Acceptance criteria

- Kada administrator odabere datum, sistem mora generisati pregled koji sadrži ukupne brojke za taj dan: broj planiranih, broj ispražnjenih i broj neuspješnih obilazaka.
- Sistem mora izlistati sve napomene o nedostupnim lokacijama grupisanu po poštarima radi lakše analize prepreka na terenu.
- Korisnik mora imati opciju da jednim klikom preuzme (download) ovaj izvještaj u PDF formatu.
- Sistem mora sadržavati i podatak o prosječnom vremenu zadržavanja poštara po jednom sandučiću za taj dan.
- Ukoliko za odabrani datum nema podataka, sistem mora prikazati poruku: Za traženi datum nisu pronađene zabilježene aktivnosti.

---
## Sprint 10 (PBI-049, PBI-050, PBI-051)
**Cilj:** Uvođenje arhive za retrospektivnu analizu ruta, izvještavanje o učinku po različitim parametrima, te pretragu i filtriranje.

### PBI-049 Historija obilazaka i arhiva ruta

#### User Stories
- **US-34:** Kao administrator ili dispečer, želim imati pristup arhivi svih realizovanih ruta, kako bih mogao pratiti radni učinak u prošlosti i vršiti retrospektivnu analizu.
- **US-35:** Kao administrator, želim vidjeti detaljne informacije o svakoj ruti iz arhive (tačno vrijeme pražnjenja svakog sandučića), kako bih mogao izvršiti reviziju u slučaju reklamacija ili provjere efikasnosti.

#### Poslovna vrijednost
Arhiviranje osigurava potpunu transparentnost operacija na terenu. Omogućava dispečerima da opravdaju resurse, analiziraju istorijske podatke za buduće planiranje i pruža neoboriv dokaz o izvršenim aktivnostima (audit trail), što je ključno za rješavanje eventualnih sporova ili žalbi građana.

#### Prioritet: Medium

---

#### Detaljna razrada Story-ja

##### ID storyja: US-34
**Naziv storyja:** Pregled arhive realizovanih ruta <br>
**Opis:** Kao **dispečer**, želim **vidjeti listu svih ruta koje su završene u prošlosti**, kako bih **imao uvid u istoriju aktivnosti**. <br>
**Poslovna vrijednost:** Centralizovan uvid u istorijske podatke bez miješanja sa trenutno aktivnim rutama. <br>
**Prioritet:** Medium <br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Ruta se automatski arhivira onog trenutka kada poštar na mobilnoj aplikaciji označi "Završi rutu".
- *Otvoreno pitanje:* Da li arhivirane rute treba omogućiti za ponovno pokretanje u slučaju greške?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-28 (Završetak rute).
- **Utiče na:** US-36 (Izvještaji o učinku).

#### Acceptance criteria

- Kada korisnik pristupi modulu Arhiva, sistem mora učitati listu svih ruta koje su dobile status Završeno ili Prekinuto, poredanih od najnovijih prema starijima.
- Sistem mora za svaku arhiviranu rutu u tabelarnom prikazu prikazati kolone: datum kreiranja, ime i prezime poštara, ukupan broj planiranih tačaka i finalni status rute.
- Sistem mora omogućiti brzo filtriranje arhive korištenjem kalendara (odabir perioda od-do) i padajućeg menija za izbor određenog poštara.
- Kada dispečer klikne na bilo koju rutu u arhivi, sistem mora otvoriti detaljan pregled te rute koji je identičan operativnom pregledu, ali u modu samo za čitanje.
- Sistem ne smije dozvoliti ponovno pokretanje arhivirane rute; u slučaju greške, administrator može samo dodati internu napomenu na arhiviranu stavku.
- Sistem mora omogućiti pretragu arhive putem polja za unos teksta koje pretražuje bazu po imenu poštara ili ID broju rute.

---

##### ID storyja: US-35
**Naziv storyja:** Detaljni uvid u arhiviranu rutu <br>
**Opis:** Kao **administrator**, želim **kliknuti na rutu iz arhive i vidjeti status svakog pojedinačnog sandučića**, kako bih **znao koji su sandučići ispražnjeni, a koji preskočeni**. <br>
**Poslovna vrijednost:** Detaljna kontrola kvaliteta obavljenog posla na nivou pojedinačne lokacije. <br>
**Prioritet:** Medium <br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Timestamp se generiše na strani servera u trenutku kada poštar potvrdi aktivnost.
**Veze sa drugim storyjima:**
- **Zavisi od:** US-34 (Pregled liste arhive).

#### Acceptance criteria

- Kada administrator odabere specifičnu rutu iz arhive, sistem mora prikazati kompletan spisak sandučića onim redoslijedom kojim su bili planirani za obilazak.
- Sistem mora pored svakog sandučića jasno prikazati njegov finalni status: Ispražnjeno, Nedostupno ili Nije posjećeno.
- Za svaki sandučić koji je obrađen na terenu, sistem mora prikazati tačno vrijeme potvrde aktivnosti (timestamp) u formatu HH:mm:ss.
- Kada je sandučić bio označen kao Nedostupno, sistem mora u detaljnom prikazu arhive prikazati i tekstualno obrazloženje koje je poštar unio u trenutku prijave problema.
- Sistem mora prikazati mapu sa ucrtanom putanjom i pinovima sandučića čije se boje razlikuju na osnovu statusa koji su dobili tokom tog specifičnog obilaska.
- Sistem mora omogućiti administratoru da izveze (export) ovaj detaljni prikaz u Excel formatu radi dalje analize učinka po lokacijama.
- Kada administrator pregleda arhiviranu rutu, sistem mora onemogućiti bilo kakve izmjene statusa ili vremena, osiguravajući integritet istorijskih podataka.

---

### PBI-050 Prošireno operativno izvještavanje

#### User Stories
- **US-36:** Kao dispečer, želim generisati sumarne izvještaje o realizaciji po poštaru za proizvoljan period, kako bih analizirao individualnu efikasnost i produktivnost tima.
- **US-37:** Kao administrator, želim vidjeti izvještaj o uspješnosti pražnjenja prema tipu sandučića, kako bih identifikovao kritične kategorije koje se najčešće ne isprazne.

#### Poslovna vrijednost
Prošireni izvještaji transformišu sirove podatke u korisne poslovne informacije, omogućavajući menadžmentu da donosi odluke zasnovane na realnim podacima (Data-driven decisions). Ovo pomaže u boljoj raspodjeli ljudskih resursa i identifikaciji tehničkih problema na specifičnim tipovima lokacija.

#### Prioritet: Medium

---

#### Detaljna razrada Story-ja

##### ID storyja: US-36
**Naziv storyja:** Izvještaj o učinku poštara <br>
**Opis:** Kao **dispečer**, želim **dobiti sumarni prikaz broja planiranih naspram realizovanih obilazaka**, kako bih **izvršio evaluaciju rada uposlenika**.<br>
**Poslovna vrijednost:** Transparentno praćenje KPI-jeva (Key Performance Indicators) za svakog poštara.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem koristi podatke iz baze arhiviranih ruta (PBI-049).
- *Otvoreno pitanje:* Da li izvještaj treba uključivati i vizuelne grafikone ili samo tabelarni prikaz?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-34 (Arhiva ruta).

#### Acceptance criteria:

- Kada dispečer otvori modul za izvještaje o učinku, sistem mora prikazati tabelu sa kolonama: ime poštara, ukupan broj dodijeljenih sandučića, broj uspješno ispražnjenih lokacija i broj nerealizovanih lokacija.
- Sistem mora automatski izračunati procenat uspješnosti za svakog poštara koristeći formulu (Ispražnjeno / Planirano) * 100 i prikazati tu vrijednost u posebnoj koloni.
- Sistem mora omogućiti filtriranje podataka prema specifičnom vremenskom periodu (npr. sedmični ili mjesečni izvještaj) kako bi se pratio učinak kroz vrijeme.
- Sistem u MVP verziji mora prikazati tabelarni izvještaj, ali i jednostavan stubni grafikon koji vizuelno poredi učinak različitih poštara radi lakše evaluacije.
- Sistem mora omogućiti sortiranje tabele prema procentu uspješnosti, od najvećeg ka najmanjem, kako bi se odmah identifikovali najefikasniji radnici.
- Kada korisnik klikne na ime poštara u izvještaju, sistem mora otvoriti dodatni detaljni prikaz sa listom svih ruta tog radnika koje su ušle u trenutni obračun.
- Sistem mora omogućiti izvoz sumarnog izvještaja o učinku u CSV formatu za potrebe daljeg procesiranja u ljudskim resursima.
- Kada u odabranom periodu nema završenih ruta za određenog poštara, sistem ga ne smije uključiti u obračun prosječne uspješnosti tima.
---

##### ID storyja: US-37
**Naziv storyja:** Analiza realizacije po tipu sandučića <br>
**Opis:** Kao **administrator**, želim **vidjeti statistiku pražnjenja grupisanu po kategorijama sandučića**, kako bih **otkrio sistemske probleme na terenu**.<br>
**Poslovna vrijednost:** Optimizacija opreme i resursa na osnovu učestalosti problema na određenim tipovima lokacija.<br>
**Prioritet:** Medium<br>
**Veze sa drugim storyjima:**
- **Zavisi od:** US-15 (Podaci o sandučićima).

#### Acceptance criteria:

- Kada administrator pokrene analizu realizacije, sistem mora prikazati zbirni tabelarni izvještaj gdje su podaci grupisani prema tipu sandučića (npr. Zidni, Samostojeći, Ugradbeni).
- Sistem mora za svaki tip sandučića prikazati ukupan broj planiranih pražnjenja, broj uspješno realizovanih i broj prijavljenih problema u odabranom periodu.
- Sistem mora automatski izračunati stopu kvarova ili nedostupnosti za svaki tip sandučića kako bi se identifikovalo da li je neki specifičan model skloniji problemima na terenu.
- Kada se u izvještaju klikne na određeni tip sandučića, sistem mora izlistati sve povezane napomene poštara (US-29) koje se odnose isključivo na taj model opreme.
- Sistem mora omogućiti filtriranje izvještaja po gradskim zonama kako bi se utvrdilo da li tip sandučića utiče na realizaciju samo u određenim okruženjima (npr. uska grla u starom gradu).
- Korisnik mora imati mogućnost da rezultate analize prikaže u obliku kružnog dijagrama (pie chart) koji pokazuje udio pojedinačnih tipova sandučića u ukupnom broju neuspješnih obilazaka.
- Sistem mora omogućiti upoređivanje realizacije između dva različita tipa sandučića za isti vremenski period radi donošenja odluka o nabavci nove opreme.
- Izvještaj mora sadržavati opciju za izvoz podataka u Excel formatu, uključujući sve pojedinačne lokacije koje su ušle u statistiku za odabrani tip sandučića.

---

### PBI-051 Pretraga i filtriranje sandučića

#### User Stories
- **US-38:** Kao administrator ili dispečer, želim pretraživati bazu sandučića po adresi ili ID-u, kako bih brzo pronašao specifičnu lokaciju.
- **US-39:** Kao administrator, želim filtrirati sandučiće prema statusu ili prioritetu, kako bih lakše upravljao održavanjem mreže.

#### Poslovna vrijednost
Brza pretraga i filtriranje štede vrijeme administratorima pri radu sa velikim brojem podataka. Ovo smanjuje operativne zastoje pri ručnom traženju lokacija i minimizira mogućnost greške pri odabiru sandučića za izmjene ili dodjelu ruti.

#### Prioritet: Medium

---

#### Detaljna razrada Story-ja

##### ID storyja: US-38
**Naziv storyja:** Brza pretraga sandučića <br>
**Opis:** Kao **administrator**, želim **unijeti dio adrese ili ID u polje za pretragu**, kako bi se **lista trenutno suzila na tražene objekte**.<br>
**Poslovna vrijednost:** Brži pristup informacijama i efikasnije upravljanje bazom podataka.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Pretraga se vrši nad postojećom listom sandučića u bazi.
**Veze sa drugim storyjima:**
- **Zavisi od:** US-15 (Prikaz liste sandučića).

#### Acceptance criteria

- Kada administrator unese najmanje tri karaktera u polje za pretragu, sistem mora automatski osvježiti listu i prikazati samo one sandučiće koji sadrže taj niz u ID-u ili adresi.
- Sistem mora osigurati da pretraga bude neosjetljiva na velika i mala slova (case-insensitive) kako bi se olakšao unos podataka.
- Pretraga mora funkcionisati u realnom vremenu bez potrebe da korisnik pritisne tipku Enter ili dugme za potvrdu.
- Kada se polje za pretragu isprazni (obriše se sav tekst), sistem mora trenutno vratiti prikaz kompletne liste svih sandučića bez kašnjenja.
- Sistem mora podržavati pretragu po parcijalnim vrijednostima, što znači da unos dijela naziva ulice mora vratiti sve sandučiće koji se nalaze u toj ulici.
- Kada pretraga ne vrati nijedan rezultat, sistem mora unutar tabele prikazati jasnu informaciju: "Nema pronađenih sandučića za uneseni pojam."
- Brzina filtriranja liste ne smije prelaziti jednu sekundu za baze do 1000 sandučića kako bi se osiguralo glatko korisničko iskustvo.
- Sistem mora zadržati funkcionalnost straničenja čak i nad filtriranim rezultatima ukoliko broj pronađenih objekata prelazi standardni broj stavki po stranici.
---

##### ID storyja: US-39
**Naziv storyja:** Filtriranje po atributima <br>
**Opis:** Kao **administrator**, želim **izabrati jedan ili više filtera (npr. Status: Neaktivan)**, kako bih **izdvojio specifičnu grupu lokacija**.<br>
**Poslovna vrijednost:** Omogućava ciljano upravljanje grupama sandučića (npr. pregled svih oštećenih ili neaktivnih lokacija).<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:**
- *Otvoreno pitanje:* Da li se filteri trebaju resetovati nakon napuštanja stranice?
**Veze sa drugim storyjima:**
- **Proširuje:** US-15 (Interfejs za upravljanje sandučićima).

#### Acceptance criteria

- Kada administrator otvori interfejs za upravljanje, sistem mora prikazati dostupne filtere za Tip sandučića, Status i Prioritet u formi padajućih menija ili grupisanih polja za odabir.
- Sistem mora omogućiti kombinovanje više različitih filtera istovremeno (npr. prikaži sve sandučiće koji su Neaktivni i imaju Visok prioritet).
- Kada korisnik odabere ili promijeni vrijednost filtera, sistem mora trenutno ažurirati prikazanu tabelu koristeći asinhrono učitavanje podataka bez osvježavanja cijelog prozora preglednika.
- Sistem mora sadržavati jasno vidljivo dugme Resetuj filtere koje jednim klikom vraća sve parametre na početne vrijednosti i prikazuje kompletnu listu sandučića.
- Sistem mora automatski resetovati sve filtere nakon što korisnik napusti stranicu ili se odjavi, kako bi se spriječila zabuna pri sljedećem pristupu podacima.
- Pored svakog aktivnog filtera sistem treba prikazati mali indikator (npr. ikonu x) koji omogućava uklanjanje samo tog specifičnog kriterija bez uticaja na ostale izabrane filtere.
- Sistem mora ispravno uskladiti funkciju filtriranja sa funkcijom brze pretrage (US-38), tako da rezultati pretrage budu ograničeni isključivo na skup podataka koji zadovoljava aktivne filtere.
- Kada primijenjeni filteri ne daju nijedan rezultat, sistem mora unutar prostora za tabelu prikazati poruku: "Nema sandučića koji odgovaraju odabranim kriterijima filtriranja."
