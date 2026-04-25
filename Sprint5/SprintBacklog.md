## Sprint 5 (PBI-011, PBI-012, PBI-013, PBI-014) 

**Cilj:** Omogućiti kreiranje profila i osigurati zaštitu podataka.

## Tabela sprint backloga
 
| ID | Naziv stavke / zadatka | Odgovorna osoba | Procjena | Status | Napomena |
|---|---|---|---|---|---|
| PBI-011 / US-01 | Administratorsko kreiranje korisničkog računa poštara | [Rubina] | 2h | To Do | Forma za unos podataka, uloga "Poštar" |
| PBI-011 / US-02 | Validacija unosa pri kreiranju računa | [Kerim] | 2h | To Do | Jedinstvenost emaila, jačina lozinke, indikator |
| PBI-011 / US-03 | Feedback o statusu kreiranja računa | [Emrah] | 1h | To Do | Toast obavijesti, greške servera, zaštita od duplog slanja |
| PBI-012 / US-04 | Osnovna prijava na sistem | [Nejla] | 1h 30min | To Do | Login forma, sesija, redirect na dashboard, zaključavanje nakon 5 pokušaja |
| PBI-012 / US-05 | Rukovanje neispravnim kredencijalima | [Aldin] | 1h | To Do | Generička poruka greške, validacija emaila, 3s blokada |
| PBI-012 / US-06 | Obavezna promjena lozinke pri prvoj prijavi | [Ibrahim] | 1h 30min | To Do | Blokada navigacije, provjera da nova ≠ inicijalna |
| PBI-013 / US-07 | Sigurna odjava iz sistema | [Faruk] | 1h 30min | To Do | Uništavanje sesije, brisanje cookieja, zaštita back dugmeta |
| PBI-014 / US-08 | Definisanje sistemskih uloga | [Rubina] | 1h 30min | To Do | Administrator / Poštar, Role ID u bazi, API level validation |
| PBI-014 / US-09 | Personalizovani dashboard prema ulozi | [Kerim] | 2h | To Do | Različit sidebar meni, personalizovana dobrodošlica |
| PBI-014 / US-10 | Restrikcija neovlaštenog pristupa | [Emrah] | 2h | To Do | Blokada URL-a, Security Log zapis, API provjera tokena |

---

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


