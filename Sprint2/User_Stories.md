# Dokumentacija Korisničkih Priča (User Stories)

---

## PBI-011 Kreiranje korisničkog računa poštara

### User Stories
- **US-01:** Kao administrator, želim kreirati korisnički račun za poštara unosom osnovnih podataka, emaila/korisničkog imena i inicijalne lozinke, kako bi poštar mogao pristupiti sistemu.
- **US-02:** Kao administrator, želim da sistem validira jedinstvenost emaila/korisničkog imena i snagu inicijalne lozinke, kako bi se spriječilo kreiranje neispravnih ili nesigurnih računa.
- **US-03:** Kao administrator, želim dobiti jasnu potvrdu o uspješnom kreiranju računa ili poruku o grešci (npr. zauzet email), kako bih mogao završiti unos i dostaviti kredencijale poštaru.

### Poslovna vrijednost
Korisnički računi za poštare moraju biti kontrolisano kreirani od strane administratora kako bi pristup sistemu imali samo ovlašteni uposlenici. Time se zadržava sigurnost sistema i jasna veza između poštara, dodijeljenih ruta i aktivnosti na terenu.

### Prioritet: High

---

### Detaljna razrada Story-ja

#### ID storyja: US-01
**Naziv storyja:** Administratorsko kreiranje korisničkog računa poštara  
**Opis:** Kao **administrator**, želim **unijeti osnovne podatke poštara, email/korisničko ime i inicijalnu lozinku**, kako bih **kreirao korisnički račun koji poštaru omogućava pristup sistemu**.  
**Poslovna vrijednost:** Osigurava da korisničke račune može otvoriti samo ovlaštena osoba, čime se smanjuje rizik od neovlaštene registracije.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Administrator ima pristup modulu za upravljanje korisnicima i dodavanje novih poštara.
- *Otvoreno pitanje:* Da li se kao primarni identifikator za prijavu koristi email adresa ili posebno korisničko ime?  
**Veze sa drugim storyjima:** Direktna zavisnost od US-02 i US-03.

### Acceptance criteria

- **Kada** administrator unese sve unosne podatke (email/username i lozinka), **ako** klikne na dugme za kreiranje računa, **tada** sistem mora kreirati korisnički račun.
- **Sistem mora** sačuvati podatke u bazi.
- **Sistem ne smije** dozvoliti kreiranje računa ako nedostaju obavezna polja.
- **Korisnik treba** dobiti potvrdu o uspješnom kreiranju.

---

#### ID storyja: US-02
**Naziv storyja:** Validacija unosa pri kreiranju računa  
**Opis:** Kao **administrator**, želim da **sistem automatski provjerava jedinstvenost emaila/korisničkog imena i kompleksnost inicijalne lozinke**, kako bi se **smanjio rizik od grešaka i sigurnosnih propusta pri otvaranju računa**.  
**Poslovna vrijednost:** Povećanje sigurnosti sistema i integriteta podataka od samog početka korištenja.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Definisan je standard snage lozinke (npr. min. 8 karaktera, broj, veliko slovo).
- *Otvoreno pitanje:* Da li inicijalna lozinka treba biti ručno unesena od strane administratora ili automatski generisana od sistema?
**Veze sa drugim storyjima:** Dio je procesa US-01.


### Acceptance criteria

- **Kada** administrator unese već postojeći email/username, **ako** pokuša kreirati račun, **tada** sistem mora odbiti unos i prikazati poruku o grešci.
- **Sistem mora** provjeravati jedinstvenost emaila/username-a.
- **Kada** lozinka ne zadovoljava sigurnosne kriterije, **ako** administrator pokuša sačuvati podatke, **tada** sistem mora odbiti unos.
- **Sistem ne smije** dozvoliti slabu lozinku.

---

#### ID storyja: US-03
**Naziv storyja:** Feedback o statusu kreiranja računa  
**Opis:** Kao **administrator**, želim **primiti vizuelnu potvrdu o uspjehu ili opisnu poruku o grešci**, kako bih **imao informaciju da li je korisnički račun uspješno kreiran i spreman za dodjelu poštaru**.  
**Poslovna vrijednost:** Poboljšanje administratorskog iskustva i smanjenje broja duplih ili neuspjelih pokušaja kreiranja računa.  
**Prioritet:** Medium  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Postoje predefinisane poruke za različite tipove grešaka (npr. "Email već postoji").  
- *Otvoreno pitanje:* Na koji način administrator uručuje inicijalne kredencijale poštaru (npr. usmeno, printano ili kroz interni kanal)?
**Veze sa drugim storyjima:** Uspješan ishod ovog story-ja je preduslov za **US-04** (Prijava korisnika).

### Acceptance criteria

- **Kada** administrator potvrdi kreiranje, **ako** je račun uspješno kreiran, **tada** sistem mora prikazati potvrdu.
- **Kada** dođe do greške prilikom obrade, **ako** sistem ne može izvršiti akciju, **tada** mora prikazati odgovarajuću poruku.
- **Sistem ne smije** ostaviti korisnika bez feedbacka.

---

## PBI-012 Prijava korisnika

### User Stories
- **US-04:** Kao registrovani korisnik, želim se prijaviti na sistem koristeći kredencijale koje mi je dodijelio administrator, kako bih pristupio funkcionalnostima aplikacije.
- **US-05:** Kao korisnik, želim da me sistem obavijesti ako unesem pogrešne kredencijale, kako bih znao da trebam ponoviti unos ili resetovati inicijalnu lozinku.
- **US-06:** Kao poštar koji se prvi put prijavljuje, želim biti obavezan promijeniti inicijalnu lozinku prije nastavka rada, kako bih zaštitio svoj korisnički račun.

### Poslovna vrijednost
Prijava osigurava da samo autentifikovani korisnici mogu manipulisati rutama i podacima o sandučićima. Obavezna promjena inicijalne lozinke pri prvoj prijavi dodatno štiti korisničke račune i smanjuje rizik od neovlaštenog pristupa nakon dodjele kredencijala.


### Prioritet: High

---

### Detaljna razrada Story-a

#### ID storyja: US-04
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

### Acceptance criteria

- **Kada** korisnik unese ispravne kredencijale, **ako** klikne na prijavu, **tada** sistem mora omogućiti pristup.
- **Sistem mora** preusmjeriti korisnika na dashboard.
- **Sistem ne smije** dozvoliti prijavu bez unosa podataka.

---

#### ID storyja: US-05
**Naziv storyja:** Rukovanje neispravnim kredencijalima
**Opis:** Kao **korisnik**, želim **jasnu poruku o grešci u slučaju pogrešnog emaila ili lozinke**, kako bih **znao da unos nije ispravan**.
**Poslovna vrijednost:** Poboljšanje korisničkog iskustva i smanjenje broja upita podršci zbog nejasnoća pri prijavi.
**Prioritet:** Medium  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Poruka ne smije biti previše specifična radi sigurnosti (sprječavanje enumeration napada).
**Veze sa drugim storyjima:** Dio je toka **US-04**.

### Acceptance criteria
 
**Kada** korisnik unese pogrešne kredencijale, **ako** klikne na prijavu, **tada** sistem mora prikazati poruku o grešci.
- **Sistem ne smije** dozvoliti prijavu sa praznim poljima.

---

#### ID storyja: US-06
**Naziv storyja:** Obavezna promjena lozinke pri prvoj prijavi  
**Opis:** Kao **poštar koji se prvi put prijavljuje**, želim da me **sistem odmah nakon uspješne autentifikacije preusmjeri na promjenu inicijalne lozinke**, kako bi **moj račun bio zaštićen prije daljnjeg korištenja sistema**.
**Poslovna vrijednost:** Smanjuje rizik da inicijalna lozinka ostane poznata drugim osobama i povećava sigurnost pristupa sistemu.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem može prepoznati da li korisnik koristi privremenu/inicijalnu lozinku.
- *Otvoreno pitanje:* Da li nakon prve promjene lozinke korisnik odmah ulazi u sistem ili se mora ponovo prijaviti?
**Veze sa drugim storyjima:** Nadovezuje se na **US-04** i zavisi od **US-02**.

### Acceptance criteria

- **Kada** se korisnik prvi put prijavi, **ako** unese inicijalnu lozinku, **tada** sistem mora zahtijevati promjenu lozinke.
- **Sistem ne smije** dozvoliti nastavak rada bez promjene lozinke.

---

## PBI-013 Odjava korisnika

### User Stories
- **US-07:** Kao prijavljeni korisnik, želim se moći odjaviti iz sistema u bilo kojem trenutku, kako bih osigurao da niko drugi ne može pristupiti mojim podacima nakon završetka rada.

### Poslovna vrijednost
Osigurava sigurnost korisničkih profila i sprečava neovlašteno korištenje sesije. Ovo je kritično u radnim okruženjima gdje više uposlenika može koristiti isti uređaj.

### Prioritet: Medium

---

### Detaljna razrada Story-ja 

#### ID storyja: US-07
**Naziv storyja:** Sigurna odjava iz sistema
**Opis:** Kao **prijavljeni korisnik**, želim **klikom na dugme za odjavu prekinuti aktivnu sesiju**, kako bi **sistem zahtijevao ponovnu prijavu za dalji rad**.
**Poslovna vrijednost:** Zaštita integriteta podataka i sprječavanje zloupotrebe sesije od strane trećih lica.
**Prioritet:** Medium
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Korisnik se nalazi na bilo kojoj stranici unutar sistema gdje je vidljiv navigacijski meni.
- *Otvoreno pitanje:* Da li sistem treba automatski odjaviti korisnika nakon određenog perioda neaktivnosti (npr. 30 minuta) radi dodatne sigurnosti?
**Veze sa drugim storyjima:**
- **Striktna zavisnost:** Direktno zavisi od **US-04** (Prijava korisnika) – ne može se odjaviti neko ko nije prijavljen.
- **Logički slijed:** Nakon odjave, korisnik se preusmjerava na početnu stranicu ili formu za ponovnu prijavu.

### Acceptance criteria

- **Kada** korisnik klikne na odjavu, **ako** potvrdi akciju, **tada** sistem mora prekinuti sesiju.
- **Sistem mora** preusmjeriti korisnika na login stranicu.
- **Sistem ne smije** dozvoliti pristup aplikaciji bez ponovne prijave.

---

## PBI-014 Uloge i pristup po ulozi

### User Stories
- **US-08:** Kao administrator, želim definisati različite nivoe pristupa (Administrator, Dispečer, Poštar), kako bih osigurao da svaki korisnik vidi samo relevantne podatke.
- **US-09:** Kao prijavljeni korisnik, želim da me sistem preusmjeri na radnu površinu (dashboard) prilagođenu mojoj ulozi, kako bih odmah mogao započeti sa svojim specifičnim zadacima.
- **US-10:** Kao korisnik sa ograničenim pravima, želim dobiti poruku o zabrani pristupa ako pokušam otvoriti stranicu za koju nemam ovlaštenje, kako bi se spriječila neovlaštena manipulacija podacima.

### Poslovna vrijednost
Implementacija uloga sprječava ljudske greške i zloupotrebu sistema. Poštari na terenu trebaju jednostavan interfejs za rad, dok dispečeri trebaju kompleksne alate za planiranje. Razgraničenje pristupa štiti bazu podataka od neovlaštenih izmjena.

### Prioritet: High

---

### Detaljna razrada Story-ja 

#### ID storyja: US-08
**Naziv storyja:** Definisanje sistemskih uloga
**Opis:** Kao **administrator**, želim **dodijeliti specifičnu ulogu svakom korisničkom računu**, kako bi **sistem mogao kontrolisati dostupne funkcionalnosti**.
**Poslovna vrijednost:** Osnovna kontrola pristupa (Access Control) koja omogućava skalabilnost tima.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Korisnički račun je već kreiran u sistemu.
- *Otvoreno pitanje:* Da li jedan korisnik može imati više uloga istovremeno (npr. dispečer koji je ujedno i administrator)?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-01 (Kreiranje korisničkog računa poštara).
- **Osnova za:** Sve funkcionalnosti koje slijede (dodavanje sandučića, generisanje ruta).

### Acceptance criteria

- **Kada** administrator odabere nivo pristupa, **ako** dodijeli ulogu korisniku, **tada** sistem mora sačuvati ulogu.
- **Sistem mora** omogućiti različite nivoe pristupa.
- **Sistem ne smije** dozvoliti neautorizovan pristup funkcijama drugih uloga.


---

#### ID storyja: US-09
**Naziv storyja:** Personalizovani dashboard prema ulozi
**Opis:** Kao **prijavljeni korisnik**, želim da **nakon prijave vidim meni i opcije specifične za moju ulogu (npr. poštar vidi samo „Moja ruta“)**, kako bih **brže obavljao posao bez suvišnih informacija**.
**Poslovna vrijednost:** Povećanje efikasnosti uposlenika kroz pojednostavljen i relevantan korisnički interfejs.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem prepoznaje ID uloge pri svakom učitavanju stranice.
**Veze sa drugim storyjima:**
- **Zavisi od:** US-04 (Prijava korisnika) i US-08 (Definisanje uloga).

### Acceptance criteria

- **Kada** se korisnik prijavi, **ako** sistem prepozna ulogu, **tada** sistem mora prikazati odgovarajući dashboard.
- **Sistem mora** omogućiti prikaz funkcionalnosti isključivo po ulozi.
- **Korisnik treba** vidjeti samo relevantne opcije.

---

#### ID storyja: US-10
**Naziv storyja:** Restrikcija neovlaštenog pristupa
**Opis:** Kao **korisnik sa ograničenim pravima**, želim da mi **sistem onemogući direktan pristup URL-ovima koji nisu namijenjeni mojoj ulozi**, kako bi **sigurnost podataka ostala netaknuta**.
**Poslovna vrijednost:** Sprječavanje sigurnosnih propusta (tzv. „Insecure Direct Object References“).
**Prioritet:** Medium
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Čak i ako korisnik zna tačan link (npr. `/admin/delete`), sistem mu ne smije dozvoliti izvršenje akcije.
- *Otvoreno pitanje:* Da li sistem treba logirati svaki neuspješan pokušaj pristupa zabranjenim stranicama radi sigurnosne analize?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-08 (Definisanje uloga).

### Acceptance criteria

- **Kada** korisnik pokuša pristupiti zabranjenoj stranici, **ako** sistem detektuje nedostatak dozvole, **tada** sistem mora odbiti pristup.
- **Sistem mora** prikazati poruku o zabrani.
- **Sistem ne smije** dozvoliti izvršenje akcije na neovlaštenoj stranici.


---

## PBI-015 Dodavanje poštara

### User Stories
- **US-11:** Kao administrator, želim unijeti podatke o novom poštaru (ime, prezime, kontakt telefon, ID broj), kako bih ga uključio u bazu aktivnih uposlenika na terenu.
- **US-12:** Kao administrator, želim da sistem provjeri da li poštar sa istim ID brojem već postoji, kako bi se izbjegli dupli unosi i konfuzija pri dodjeli ruta.

### Poslovna vrijednost
Ovaj modul je ključan za operativno planiranje. Bez tačne baze poštara, dispečer ne može vršiti dodjelu ruta, a sistem ne može pratiti ko je odgovoran za koji sandučić.

### Prioritet: High

---

### Detaljna razrada Story-ja

#### ID storyja: US-11
**Naziv storyja:** Unos novog poštara u sistem
**Opis:** Kao **administrator**, želim **popuniti formu sa podacima o novom uposleniku**, kako bi **on postao vidljiv u listi za dodjelu dnevnih ruta**.
**Poslovna vrijednost:** Digitalizacija evidencije terenskih radnika.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Administrator ima pristup panelu za upravljanje osobljem.
- *Pretpostavka:* Kreiranje login naloga i dodjela početnih kredencijala pokriveni su kroz PBI-011.
**Veze sa drugim storyjima:**
- **Osnova za:** US-13 (Pregled liste poštara) i US-23 (Dodjela rute poštaru).

### Acceptance criteria

- **Kada** administrator unese lične podatke, **ako** klikne na dugme za spasavanje, **tada** sistem mora sačuvati poštara.
- **Sistem mora** omogućiti unos svih polja (ime, prezime, telefon, ID).
- **Sistem ne smije** dozvoliti prazna polja pri registraciji poštara.

---

#### ID storyja: US-12
**Naziv storyja:** Validacija duplog ID broja
**Opis:** Kao **administrator**, želim **da sistem spriječi unos novog poštara ukoliko se njegov ID broj već nalazi u bazi**, kako bi se **izbjegli dupli unosi i konfuzija pri dodjeli ruta**.
**Poslovna vrijednost:** Održavanje integriteta baze podataka i sprečavanje operativnih grešaka prilikom vođenja evidencije.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* ID broj radnika je striktno jedinstven identifikator na nivou cijelog sistema.
- *Otvoreno pitanje:* Da li sistem, u slučaju unosa duplog ID-a, treba odmah ponuditi link za pregled profila postojećeg poštara sa tim ID brojem?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-11 (Unos novog poštara u sistem).

### Acceptance criteria

- **Kada** administrator unese ID, **ako** ID već postoji u bazi, **tada** sistem mora odbiti unos.
- **Sistem mora** provjeravati jedinstvenost ID-a.
- **Korisnik treba** dobiti poruku o grešci koja objašnjava da je ID zauzet.


---

## PBI-016 Pregled liste poštara

### User Stories
- **US-13:** Kao administrator ili dispečer, želim vidjeti listu svih registrovanih poštara sa njihovim trenutnim statusom, kako bih znao koga mogu zadužiti za nove zadatke.

### Poslovna vrijednost
Omogućava brz pregled dostupnih ljudskih resursa, što direktno utiče na brzinu reagovanja dispečera pri planiranju vanrednih ili redovnih pražnjenja sandučića.

### Prioritet: Medium

---

#### ID storyja: US-13
**Naziv storyja:** Tabelarni pregled poštara
**Opis:** Kao **administrator ili dispečer**, želim **imati pregled svih poštara u obliku tabele sa osnovnim podacima i statusom aktivnosti**, kako bih **brzo pronašao određenog uposlenika i procijenio njegovu raspoloživost**.
**Poslovna vrijednost:** Transparentnost i lakša navigacija kroz bazu uposlenika.
**Prioritet:** Medium
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Lista prikazuje samo relevantne podatke (npr. bez lozinki).
- *Otvoreno pitanje:* Da li u listi treba biti vidljiva i zadnja poznata lokacija poštara radi bolje koordinacije?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-11 (Unos poštara).

### Acceptance criteria

- **Kada** korisnik otvori listu poštara, **ako** sistem učita podatke, **tada** sistem mora prikazati sve poštare.
- **Sistem mora** omogućiti pregled osnovnih podataka (ime, prezime, kontakt telefon i status poštara).
- **Korisnik treba** vidjeti status poštara (dostupan ili zauzet).

---

## PBI-017 Dodavanje poštanskog sandučića

### User Stories
- **US-14:** Kao administrator ili dispečer, želim dodati novi poštanski sandučić u sistem unoseći njegovu adresu i precizne GPS koordinate, kako bi on bio vidljiv u evidenciji i mogao biti korišten za planiranje ruta.
- **US-15:** Kao administrator ili dispečer, želim pri unosu definisati tip sandučića i osnovne podatke o njemu, kako bi zapis bio potpun i spreman za dalju obradu u sistemu.

### Poslovna vrijednost
Precizna evidencija sandučića je temelj optimizacije. Unos GPS koordinata eliminiše nagađanje na terenu, dok evidentiranje tipa i osnovnih podataka omogućava kvalitetnije planiranje obilazaka.

### Prioritet: High

---

### Detaljna razrada Story-ja 

#### ID storyja: US-14
**Naziv storyja:** Unos lokacije sandučića putem koordinata
**Opis:** Kao **administrator ili dispečer**, želim **unijeti adresu, geografsku širinu (Latitude) i dužinu (Longitude) za svaki sandučić**, kako bi **sistem mogao evidentirati lokaciju i koristiti je pri generisanju ruta**.
**Poslovna vrijednost:** Omogućava matematičku preciznost pri generisanju dnevnih ruta i smanjuje potrošnju vremena na terenu.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Korisnik ima pristup Google Maps ili sličnom servisu za očitavanje koordinata.
- *Otvoreno pitanje:* Da li sistem treba omogućiti „pinovanje“ lokacije direktno na mapi unutar aplikacije umjesto ručnog kucanja brojeva?
**Veze sa drugim storyjima:**
- **Osnova za:** US-22 (Generisanje dnevne rute).

### Acceptance criteria

- **Kada** administrator unese geografsku lokaciju, **ako** se unesu koordinate, **tada** sistem mora sačuvati lokaciju sandučića.
- **Sistem mora** omogućiti unos GPS podataka.
- **Sistem ne smije** dozvoliti unos nevalidnih koordinata.

---

#### ID storyja: US-15
**Naziv storyja:** Unos tipa i osnovnih podataka sandučića
**Opis:** Kao **administrator ili dispečer**, želim **prilikom kreiranja sandučića definisati njegov tip i osnovne podatke**, kako bi **zapis bio kompletan i spreman za dalje upravljanje**.
**Poslovna vrijednost:** Omogućava konzistentnu evidenciju sandučića i lakše upravljanje njihovim karakteristikama.
**Prioritet:** Medium
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Postoji unaprijed definisan skup tipova sandučića.
- *Otvoreno pitanje:* Koji skup osnovnih podataka je obavezan pored adrese i koordinata?
**Veze sa drugim storyjima:**
- **Nadovezuje se na:** US-14.

### Acceptance criteria

- **Kada** administrator popunjava formu, **ako** se odabere tip sandučića, **tada** sistem mora sačuvati podatak o tipu.
- **Sistem mora** omogućiti izbor tipa iz predefinisanih opcija.
- **Sistem ne smije** dozvoliti nepostojeći tip.

---

## PBI-018 Izmjena podataka o sandučiću

### User Stories
- **US-16:** Kao administrator, želim izmijeniti lokaciju, tip, prioritet i druge podatke postojećeg sandučića, kako bih osigurao da baza podataka odgovara stvarnom stanju na terenu.

### Poslovna vrijednost
Održavanje tačnosti baze podataka. Pogrešne ili zastarjele informacije dovode do gubitka vremena poštara na terenu i neispravnog planiranja rute.

### Prioritet: Medium

---

### Detaljna razrada Story-ja 

#### ID storyja: US-16
**Naziv storyja:** Ažuriranje podataka o sandučiću
**Opis:** Kao **administrator**, želim **otvoriti formu za uređivanje postojećeg sandučića i spasiti izmjene nad lokacijom, tipom, prioritetom i drugim podacima**, kako bi **promjene bile odmah vidljive svim korisnicima sistema**.
**Poslovna vrijednost:** Fleksibilnost sistema u slučaju urbanističkih promjena ili tehničkih grešaka pri prvom unosu.
**Prioritet:** Medium
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sandučić već postoji u bazi podataka.
- *Otvoreno pitanje:* Da li sistem treba čuvati historiju promjena (ko je i kada izmijenio podatke) radi interne kontrole?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-14 i US-15.
- **Povezano sa:** US-18 (Definisanje prioriteta sandučića).

### Acceptance criteria

- **Kada** administrator otvori formu za uređivanje, **ako** se izmjene podaci, **tada** sistem mora sačuvati promjene.
- **Sistem mora** omogućiti uređivanje svih podataka o sandučiću.
- **Korisnik treba** vidjeti ažurirane informacije odmah nakon spasavanja.

---

## PBI-019 Pregled sandučića na listi

### User Stories
- **US-17:** Kao administrator ili dispečer, želim vidjeti listu svih evidentiranih sandučića kroz jednostavnu tabelu ili listu, kako bih imao pregled nad stanjem i raspoloživim tačkama za planiranje.

### Poslovna vrijednost
Vizuelni pregled svih tačaka u sistemu omogućava brz uvid u evidenciju sandučića i olakšava operativni rad pri planiranju i kontroli.

### Prioritet: Medium

---

#### ID storyja: US-17
**Naziv storyja:** Pregled liste sandučića
**Opis:** Kao **administrator ili dispečer**, želim **pregledati sve evidentirane sandučiće kroz jednostavnu tabelu ili listu sa osnovnim podacima**, kako bih **brzo pronašao traženi sandučić i provjerio njegove informacije**.
**Poslovna vrijednost:** Brža navigacija i bolja organizacija resursa.
**Prioritet:** Medium
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Lista prikazuje osnovne podatke potrebne za rad (npr. lokaciju, tip i status/prioritet gdje je primjenjivo).
- *Otvoreno pitanje:* Da li u ovoj fazi treba omogućiti i pretragu ili filtriranje po tipu/naselju?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-14 i US-15.

### Acceptance criteria

- **Kada** se otvori lista sandučića, **ako** sistem dohvati podatke, **tada** sistem mora prikazati sandučiće.
- **Sistem mora** prikazati adresu, tip i prioritet sandučića.
- **Korisnik treba** vidjeti listu svih evidentiranih sandučića u bazi.

---

## PBI-020 Definisanje prioriteta sandučića

### User Stories
- **US-18:** Kao administrator, želim postaviti ili izmijeniti prioritet za pražnjenje/punjenje sandučića, kako bi sistem znao koje lokacije imaju veći operativni značaj.

### Poslovna vrijednost
Prioriteti omogućavaju da sistem i dispečer razlikuju kritične od manje kritičnih lokacija, što direktno utiče na kvalitet planiranja i redoslijed obilaska.

### Prioritet: High

---

#### ID storyja: US-18
**Naziv storyja:** Postavljanje prioriteta sandučića
**Opis:** Kao **administrator**, želim **dodijeliti ili izmijeniti nivo prioriteta za pojedini sandučić**, kako bi **sistem mogao uzeti u obzir njegov značaj pri planiranju pražnjenja i punjenja**.
**Poslovna vrijednost:** Diferencijacija usluge prema važnosti lokacije i bolja podrška algoritmu za planiranje.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Postoji jasan skup vrijednosti prioriteta (npr. nizak, srednji, visok).
- *Otvoreno pitanje:* Da li se prioritet treba mijenjati isključivo ručno ili može biti i automatski predložen na osnovu pravila sistema?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-14 i US-15.
- **Utiče na:** US-22 (Generisanje dnevne rute).

### Acceptance criteria

- **Kada** administrator pristupi opcijama sandučića, **ako** se odabere prioritet, **tada** sistem mora sačuvati tu vrijednost.
- **Sistem mora** omogućiti izbor prioriteta (npr. Visok, Srednji, Nizak).
- **Korisnik treba** vidjeti trenutni prioritet na listi i u detaljima.

---

## PBI-022 Generisanje dnevne rute

### User Stories
- **US-22:** Kao dispečer, želim pokrenuti algoritam za automatsko generisanje dnevne rute za odabranog poštara, kako bih dobio prijedlog obilaska zasnovan na lokacijama i prioritetima sandučića.

### Poslovna vrijednost
Ovo je srce sistema. Automatizacija rute smanjuje manuelni rad dispečera, štedi vrijeme i osigurava da ključne lokacije ne budu zaboravljene.

### Prioritet: High 

---

### Detaljna razrada Story-ja 

#### ID storyja: US-22
**Naziv storyja:** Automatizovani proračun dnevne rute
**Opis:** Kao **dispečer**, želim **klikom na dugme "Generiši" aktivirati algoritam**, koji će **na osnovu GPS koordinata i prioriteta sandučića kreirati prijedlog dnevne rute za odabranog poštara**.
**Poslovna vrijednost:** Eliminacija manuelnog planiranja i smanjenje ljudske greške.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem ima pristup koordinatama i prioritetima svih relevantnih sandučića.
- *Otvoreno pitanje:* Koji algoritam koristiti u MVP-u s obzirom na broj tačaka i performanse sistema?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-14 i US-18.
- **Osnova za:** US-23, US-24 i US-25.

### Acceptance criteria

- **Kada** dispečer odabere parametre, **ako** se pokrene generisanje, **tada** sistem mora kreirati rutu.
- **Sistem mora** uzeti u obzir koordinate i prioritete pri računanju redoslijeda.
- **Kada** ne postoje sandučići u sistemu, **ako** se pokuša kreiranje, **tada** sistem ne smije generisati rutu.

---

## PBI-023 Dodjela rute poštaru

### User Stories
- **US-25:** Kao dispečer, želim dodijeliti generisanu rutu konkretnom poštaru, kako bi on dobio svoj dnevni zadatak za izvršenje.

### Poslovna vrijednost
Uspostavlja jasnu odgovornost za izvršenje rute i omogućava da poštar na vrijeme dobije plan rada.

### Prioritet: High

---

#### ID storyja: US-25
**Naziv storyja:** Dodjela rute poštaru
**Opis:** Kao **dispečer**, želim **izabrati poštara iz liste dostupnih radnika i dodijeliti mu generisanu rutu**, kako bi **on mogao započeti sa radnim zadatkom**.
**Poslovna vrijednost:** Uspostavljanje jasne odgovornosti za izvršenje posla.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Ruta je prethodno generisana i spremna za dodjelu.
- *Otvoreno pitanje:* Na koji način poštar dobija obavijest o dodijeljenoj ruti?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-13 (Pregled liste poštara) i US-22 (Generisanje dnevne rute).

### Acceptance criteria

- **Kada** je ruta spremna, **ako** se ruta dodijeli odabranom radniku, **tada** sistem mora povezati poštara i rutu.
- **Sistem mora** omogućiti izbor poštara sa liste dostupnih radnika.
- **Korisnik treba** dobiti potvrdu o uspješno izvršenoj dodjeli.

---

## PBI-024 Pregled detalja rute

### User Stories
- **US-23:** Kao dispečer, želim pregledati detalje generisane rute, uključujući redoslijed obilaska, uključene sandučiće i osnovne informacije o ruti, kako bih mogao provjeriti njenu logičnost prije dodjele.

### Poslovna vrijednost
Omogućava provjeru kvaliteta prijedloga rute prije nego što ruta bude poslana poštaru na izvršenje.

### Prioritet: Medium

---

#### ID storyja: US-23
**Naziv storyja:** Pregled detalja generisane rute
**Opis:** Kao **dispečer**, želim **vidjeti redoslijed obilaska, uključene sandučiće i osnovne detalje rute**, kako bih **mogao provjeriti da li prijedlog odgovara stvarnoj operativnoj potrebi**.
**Poslovna vrijednost:** Bolja kontrola i lakše uočavanje nelogičnosti u predloženoj ruti.
**Prioritet:** Medium
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem može prikazati rutu kroz listu i/ili mapu kao dodatni prikaz detalja.
- *Otvoreno pitanje:* Koji skup osnovnih detalja rute mora biti prikazan u MVP verziji?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-22 (Generisanje dnevne rute).

### Acceptance criteria

- **Kada** se u pregledu odabere ruta, **ako** se otvori ruta, **tada** sistem mora prikazati detalje.
- **Sistem mora** prikazati redoslijed obilaska i listu uključenih sandučića.
- **Korisnik treba** vidjeti osnovne informacije o ruti (dužina, broj tačaka) prije dodjele.

---

## PBI-025 Ručna izmjena redoslijeda obilaska

### User Stories
- **US-24:** Kao dispečer, želim imati mogućnost ručne izmjene redoslijeda obilaska unutar generisane rute, kako bih uvažio nepredviđene okolnosti na terenu.

### Poslovna vrijednost
Daje dispečeru neophodnu fleksibilnost u situacijama kada automatski prijedlog ne odražava u potpunosti stvarne uslove rada.

### Prioritet: Medium

---

#### ID storyja: US-24
**Naziv storyja:** Ručna izmjena redoslijeda obilaska
**Opis:** Kao **dispečer**, želim **promijeniti redoslijed tačaka unutar generisane rute**, kako bih **prilagodio plan trenutnim operativnim okolnostima**.
**Poslovna vrijednost:** Fleksibilnost sistema u realnim situacijama.
**Prioritet:** Medium
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem nakon izmjene čuva novi redoslijed obilaska.
- *Otvoreno pitanje:* Da li sistem treba automatski preračunavati procijenjenu dužinu ili trajanje rute nakon ručne promjene?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-22 i US-23.

### Acceptance criteria

- **Kada** dispečer mijenja raspored, **ako** se promijeni redoslijed tačaka, **tada** sistem mora sačuvati izmjene.
- **Sistem mora** omogućiti ručnu izmjenu redoslijeda tačaka unutar rute (npr. drag & drop).
- **Kada** dispečer sačuva izmjene, **ako** potvrdi akciju, **tada** sistem mora prikazati novi redoslijed obilaska.

---

## PBI-026 Mobilni prikaz dodijeljene rute

### User Stories
- **US-26:** Kao poštar, želim vidjeti svoju dodijeljenu rutu preko responzivnog web interfejsa, kako bih na mobilnom uređaju imao jasan pregled dnevnog zadatka.

### Poslovna vrijednost
Povećava brzinu rada na terenu i eliminiše potrebu za papirnim spiskovima ili dodatnim neformalnim kanalima komunikacije.

### Prioritet: High

---

### Detaljna razrada Story-ja 

#### ID storyja: US-26
**Naziv storyja:** Mobilni pregled dodijeljene rute
**Opis:** Kao **poštar**, želim **na svom uređaju vidjeti dodijeljenu rutu, redoslijed obilaska i osnovne informacije o tačkama**, kako bih **imao jasan uvid u obim posla za taj dan**.
**Poslovna vrijednost:** Transparentnost rada i bolja organizacija vremena uposlenika.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Poštar je prijavljen na sistem i dodijeljena mu je aktivna ruta (US-25).
- *Otvoreno pitanje:* Da li poštar treba vidjeti i procjenu vremena potrebnog za završetak cijele rute?
**Veze sa drugim storyjima:**
- **Direktna zavisnost:** US-25 (Dodjela rute poštaru).

### Acceptance criteria

- **Kada** se poštar prijavi, **ako** se otvori mobilna aplikacija, **tada** sistem mora prikazati aktivnu rutu.
- **Sistem mora** biti responzivan (prilagođen ekranu telefona).
- **Korisnik treba** vidjeti sve tačke obilaska na mapi ili listi.

---

## PBI-027 Ažuriranje statusa sandučića

### User Stories
- **US-28:** Kao poštar, želim promijeniti status sandučića tokom obilaska, kako bi dispečer imao informaciju o progresu rute u realnom vremenu.

### Poslovna vrijednost
Omogućava zatvaranje petlje povratnih informacija između poštara i dispečera i daje uvid u napredak rada na terenu.

### Prioritet: High

---

#### ID storyja: US-28
**Naziv storyja:** Ažuriranje statusa sandučića tokom obilaska
**Opis:** Kao **poštar**, želim **jednim klikom promijeniti status sandučića tokom obilaska**, kako bi **sistem evidentirao napredak i prikazao ažurno stanje rute dispečeru**.
**Poslovna vrijednost:** Real-time praćenje progresa rada.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem bilježi tačno vrijeme promjene statusa radi kasnije analize.
- *Otvoreno pitanje:* Koji minimalni skup statusa mora postojati u MVP-u?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-26.
- **Utiče na:** US-29 i US-30.

### Acceptance criteria

- **Kada** poštar dođe do lokacije, **ako** se promijeni status sandučića (npr. u "Ispražnjeno"), **tada** sistem mora sačuvati promjenu.
- **Sistem mora** omogućiti promjenu statusa jednim klikom.
- **Korisnik treba** vidjeti ažuriran status na svom interfejsu.

---

## PBI-028 Označavanje nedostupne lokacije

### User Stories
- **US-29:** Kao poštar, želim evidentirati da određena lokacija nije bila dostupna tokom obilaska, kako bi problem bio zabilježen i vidljiv dispečeru.

### Poslovna vrijednost
Omogućava da sistem zabilježi operativne izuzetke i da dispečer ima tačnu sliku o tome zašto pojedina tačka nije obrađena.

### Prioritet: Low

---

#### ID storyja: US-29
**Naziv storyja:** Evidentiranje nedostupne lokacije
**Opis:** Kao **poštar**, želim **označiti lokaciju kao nedostupnu i po potrebi ostaviti kratku napomenu**, kako bi **problem bio evidentiran i mogao se uzeti u obzir u daljem radu**.
**Poslovna vrijednost:** Brže reagovanje na probleme na terenu i kvalitetnija evidencija izuzetaka.
**Prioritet:** Low
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Nedostupna lokacija predstavlja poseban status ili razlog unutar sistema.
- *Otvoreno pitanje:* Da li je u MVP-u dovoljan tekstualni opis ili je potrebna i fotografija?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-26.
- **Povezano sa:** US-28 i US-30.

### Acceptance criteria

- **Kada** poštar ne može pristupiti tački, **ako** se označi lokacija kao nedostupna, **tada** sistem mora evidentirati problem.
- **Sistem mora** omogućiti unos napomene (razlog nedostupnosti).
- **Korisnik treba** vidjeti status "Nedostupno" u svojoj listi zadataka.

---

## PBI-029 Praćenje statusa rute od strane dispečera

### User Stories
- **US-30:** Kao dispečer, želim imati uvid u to koji su sandučići obrađeni, preskočeni ili problematični, kako bih mogao pratiti status izvršenja dodijeljene rute.

### Poslovna vrijednost
Dispečer dobija operativni pregled nad izvršenjem rute i može pravovremeno reagovati kada dođe do odstupanja ili problema na terenu.

### Prioritet: High

---

#### ID storyja: US-30
**Naziv storyja:** Operativni pregled statusa rute
**Opis:** Kao **dispečer**, želim **vidjeti status obrađenih, preskočenih ili problematičnih sandučića unutar rute**, kako bih **mogao pratiti realizaciju i reagovati na probleme**.
**Poslovna vrijednost:** Bolja kontrola izvršenja dnevnih zadataka i pravovremeno donošenje operativnih odluka.
**Prioritet:** High
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Statusi se osvježavaju na osnovu akcija koje poštar radi na terenu.
- *Otvoreno pitanje:* Da li je u MVP-u dovoljan tabelarni pregled ili je potreban i vizuelni prikaz na mapi?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-28 i US-29.

### Acceptance criteria

- **Kada** dispečer prati rad na terenu, **ako** se učita progres, **tada** sistem mora prikazati statuse u realnom vremenu.
- **Sistem mora** omogućiti vizuelni pregled progresa rute.
- **Korisnik treba** vidjeti tačno koji su sandučići obrađeni, preskočeni ili problematični.

---

## PBI-030 Osnovni dnevni izvještaj

### User Stories
- **US-31:** Kao administrator ili dispečer, želim generisati osnovni dnevni izvještaj o realizovanim i nerealizovanim obilascima, kako bih imao sažet pregled učinka za dati dan.

### Poslovna vrijednost
Izvještaj daje pregled realizacije rada i predstavlja osnovu za internu analizu, evidenciju i kasnije unapređenje operativnih procesa.

### Prioritet: Low

---

#### ID storyja: US-31
**Naziv storyja:** Generisanje osnovnog dnevnog izvještaja
**Opis:** Kao **administrator ili dispečer**, želim **dobiti osnovni dnevni izvještaj o realizovanim i nerealizovanim obilascima**, kako bih **imao sažet pregled izvršenja rada za određeni dan**.
**Poslovna vrijednost:** Omogućava osnovno operativno izvještavanje i pregled učinka.
**Prioritet:** Low
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem već raspolaže podacima o statusima obilazaka i sandučića za posmatrani dan.
- *Otvoreno pitanje:* Koji minimum informacija treba ući u MVP izvještaj (broj završenih, broj nedostupnih, broj nezavršenih lokacija i sl.)?
**Veze sa drugim storyjima:**
- **Zavisi od:** US-28, US-29 i US-30.

### Acceptance criteria

- **Kada** se odabere opcija za izvještavanje, **ako** se generiše izvještaj za odabrani datum, **tada** sistem mora prikazati rezultate.
- **Sistem mora** prikazati broj realizovanih i nerealizovanih obilazaka.
- **Korisnik treba** dobiti sažetak sa osnovnim statistikama za odabrani dan.
