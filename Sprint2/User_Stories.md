# Dokumentacija Korisničkih Priča (User Stories)

---

## PBI-011 Registracija korisnika

### User Stories
- **US-01:** Kao neregistrovani korisnik, želim kreirati korisnički račun unoseći email, lozinku i osnovne podatke, kako bih dobio pristup sistemu.
- **US-02:** Kao administrator, želim da sistem validira snagu lozinke i ispravnost email formata, kako bi se spriječilo kreiranje nesigurnih računa.
- **US-03:** Kao neregistrovani korisnik, želim dobiti jasnu poruku o uspješnoj registraciji ili grešci (npr. zauzet email), kako bih znao koji je sljedeći korak.

### Poslovna vrijednost
Registracija je ulaz u sistem. Bez nje je nemoguće osigurati sigurnost podataka, pratiti koji poštar je odgovoran za koju rutu, te spriječiti neovlašteni pristup osjetljivim informacijama o lokacijama sandučića.

### Prioritet: High

---

### Detaljna razrada Story-ja

#### ID storyja: US-01
**Naziv storyja:** Osnovna registracija korisnika  
**Opis:** Kao **neregistrovani korisnik**, želim **unijeti svoje ime, prezime, email i lozinku u formu za registraciju**, kako bih **postao evidentiran korisnik sistema**.  
**Poslovna vrijednost:** Osigurava bazu korisnika neophodnu za dalju dodjelu uloga i zadataka.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:** - *Pretpostavka:* Korisnik ima pristup internetu i važeću email adresu.
- *Otvoreno pitanje:* Da li sistem treba automatski dodijeliti rolu "Gost" dok administrator ne odobri višu rolu?  
**Veze sa drugim storyjima:** Direktna zavisnost od US-02.

---

#### ID storyja: US-02
**Naziv storyja:** Validacija unosa pri registraciji  
**Opis:** Kao **administrator**, želim da **sistem automatski provjerava kompleksnost lozinke i validnost emaila**, kako bi se **smanjio rizik od hakerskih napada i grešaka u bazi**.  
**Poslovna vrijednost:** Povećanje sigurnosti sistema i integriteta podataka od samog početka.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:** - *Pretpostavka:* Definisan je standard snage lozinke (npr. min 8 karaktera, broj, veliko slovo).
- *Otvoreno pitanje:* Treba li sistem ograničiti registraciju samo na određene domene (npr. @posta.ba)?
**Veze sa drugim storyjima:** Dio je procesa US-01.

---

#### ID storyja: US-03
**Naziv storyja:** Feedback o statusu registracije  
**Opis:** Kao **neregistrovani korisnik**, želim **primiti vizuelnu potvrdu o uspjehu ili opisnu poruku o grešci**, kako bih **imao informaciju da li je moj račun uspješno kreiran**.  
**Poslovna vrijednost:** Poboljšanje korisničkog iskustva i smanjenje broja duplih pokušaja registracije.  
**Prioritet:** Medium  
**Pretpostavke i otvorena pitanja:** - *Pretpostavka:* Postoje predefinisane poruke za različite tipove grešaka (npr. "Email već postoji").  
**Veze sa drugim storyjima:** Uspješan ishod ovog story-ja je preduslov za **US-04** (Prijava korisnika).

---

## PBI-012 Prijava korisnika

### User Stories
- **US-04:** Kao registrovani korisnik, želim se prijaviti na sistem koristeći svoj email i lozinku, kako bih pristupio funkcionalnostima aplikacije.
- **US-05:** Kao korisnik, želim da me sistem obavijesti ako unesem pogrešne kredencijale, kako bih znao da trebam ponoviti unos ili resetovati lozinku.
- **US-06:** Kao korisnik, želim ostati prijavljen u sistemu tokom trajanja sesije, kako ne bih morao ponavljati prijavu pri svakom osvježavanju stranice.

### Poslovna vrijednost
Osigurava da samo autentifikovani korisnici mogu manipulisati rutama i podacima o sandučićima. Ovo direktno štiti integritet poštanskih operacija i sprječava neovlaštene izmjene statusa na terenu.

### Prioritet: High

---

### Detaljna razrada Story-a

#### ID storyja: US-04
**Naziv storyja:** Osnovna prijava na sistem <br>
**Opis:** Kao **registrovani korisnik**, želim **unijeti svoje pristupne podatke u login formu**, kako bih **ostvario pristup radnom okruženju**. <br>
**Poslovna vrijednost:** Osnovni mehanizam autentifikacije i zaštite korisničkog profila. <br>
**Prioritet:** High <br>
**Pretpostavke i otvorena pitanja:**<br> - *Pretpostavka:* Korisnik je prethodno uspješno prošao proces registracije (PBI-011).<br>
- *Otvoreno pitanje:* Da li sistem treba ograničiti broj neuspješnih pokušaja prijave prije privremenog zaključavanja računa?<br>
**Veze sa drugim storyjima:** - **Striktna zavisnost:** Zavisi od **US-01** (Registracija).<br>
- **Logički povezano:** Prethodi svim storyjima koji zahtijevaju autorizaciju.

---

#### ID storyja: US-05
**Naziv storyja:** Rukovanje neispravnim kredencijalima<br>
**Opis:** Kao **korisnik**, želim **jasnu poruku o grešci u slučaju pogrešnog emaila ili lozinke**, kako bih **znao da unos nije ispravan**.<br>
**Poslovna vrijednost:** Poboljšanje korisničkog iskustva i smanjenje broja upita podršci zbog nejasnoća pri prijavi.<br>
**Prioritet:** Medium <br>
**Pretpostavke i otvorena pitanja:** - *Pretpostavka:* Poruka ne smije biti previše specifična radi sigurnosti (sprječavanje enumeration napada).<br>
**Veze sa drugim storyjima:** Dio je toka **US-04**.

---

#### ID storyja: US-06
**Naziv storyja:** Održavanje korisničke sesije <br>
**Opis:** Kao **korisnik**, želim da **sistem pamti moju prijavu dok ne zatvorim preglednik ili se odjavim**, kako bi **rad bio kontinuiran i bez prekida**.<br>
**Poslovna vrijednost:** Povećanje efikasnosti rada, posebno za dispečere koji sistem koriste duži vremenski period tokom dana.<br>
**Prioritet:** Low<br>
**Pretpostavke i otvorena pitanja:**<br> - *Otvoreno pitanje:* Koliko dugo sesija treba biti aktivna u slučaju neaktivnosti korisnika?<br>
**Veze sa drugim storyjima:** Nadovezuje se na **US-04**.

---

## PBI-013 Odjava korisnika

### User Stories
- **US-07:** Kao prijavljeni korisnik, želim se moći odjaviti iz sistema u bilo kojem trenutku, kako bih osigurao da niko drugi ne može pristupiti mojim podacima nakon završetka rada.

### Poslovna vrijednost
Osigurava sigurnost korisničkih profila i sprečava neovlašteno korištenje sesije. Ovo je kritično u radnim okruženjima gdje više uposlenika može koristiti isti uređaj.

### Prioritet: Medium

---

### Detaljna razrada Story-ja 

#### ID storyja: US-07<br>
**Naziv storyja:** Sigurna odjava iz sistema<br>
**Opis:** Kao **prijavljeni korisnik**, želim **klikom na dugme za odjavu prekinuti aktivnu sesiju**, kako bi **sistem zahtijevao ponovnu prijavu za dalji rad**.<br>
**Poslovna vrijednost:** Zaštita integriteta podataka i sprječavanje zloupotrebe sesije od strane trećih lica. <br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:** <br> - *Pretpostavka:* Korisnik se nalazi na bilo kojoj stranici unutar sistema gdje je vidljiv navigacijski meni.<br>
- *Otvoreno pitanje:* Da li sistem treba automatski odjaviti korisnika nakon određenog perioda neaktivnosti (npr. 30 minuta) radi dodatne sigurnosti?<br>
**Veze sa drugim storyjima:**<br> - **Striktna zavisnost:** Direktno zavisi od **US-04** (Prijava korisnika) – ne može se odjaviti neko ko nije prijavljen.<br>
- **Logički slijed:** Nakon odjave, korisnik se preusmjerava na početnu stranicu ili formu za ponovnu prijavu.

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
**Naziv storyja:** Definisanje sistemskih uloga<br>
**Opis:** Kao **administrator**, želim **dodijeliti specifičnu ulogu svakom registrovanom korisniku**, kako bi **sistem mogao kontrolisati dostupne funkcionalnosti**.<br>
**Poslovna vrijednost:** Osnovna kontrola pristupa (Access Control) koja omogućava skalabilnost tima.
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**<br> - *Pretpostavka:* Korisnik je već registrovan u sistemu.<br>
- *Otvoreno pitanje:* Da li jedan korisnik može imati više uloga istovremeno (npr. Dispečer koji je ujedno i Administrator)?
**Veze sa drugim storyjima:**<br> - **Zavisi od:** US-01 (Registracija).<br>
- **Osnova za:** Sve funkcionalnosti koje slijede (Dodavanje sandučića, generisanje ruta).

---

#### ID storyja: US-09
**Naziv storyja:** Personalizovani dashboard prema ulozi<br>
**Opis:** Kao **prijavljeni korisnik**, želim da **nakon prijave vidim meni i opcije specifične za moju ulogu (npr. poštar vidi samo 'Moja ruta')**, kako bih **brže obavljao posao bez suvišnih informacija**.<br>
**Poslovna vrijednost:** Povećanje efikasnosti uposlenika kroz pojednostavljen i relevantan korisnički interfejs.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**<br> - *Pretpostavka:* Sistem prepoznaje ID uloge pri svakom učitavanju stranice.<br>
**Veze sa drugim storyjima:** <br>- **Zavisi od:** US-04 (Prijava korisnika) i US-08 (Definisanje uloga).<br>

---

#### ID storyja: US-10
**Naziv storyja:** Restrikcija neovlaštenog pristupa<br>
**Opis:** Kao **korisnik sa ograničenim pravima**, želim da mi **sistem onemogući direktan pristup URL-ovima koji nisu namijenjeni mojoj ulozi**, kako bi **sigurnost podataka ostala netaknuta**.<br>
**Poslovna vrijednost:** Sprječavanje sigurnosnih propusta (tzv. "Insecure Direct Object References").<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:** <br>- *Pretpostavka:* Čak i ako korisnik zna tačan link (npr. /admin/delete), sistem mu ne smije dozvoliti izvršenje akcije.<br>
- *Otvoreno pitanje:* Da li sistem treba logirati svaki neuspješan pokušaj pristupa zabranjenim stranicama radi sigurnosne analize?<br>
**Veze sa drugim storyjima:**<br> - **Zavisi od:** US-08 (Definisanje uloga).

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

#### ID storyja: US-11<br>
**Naziv storyja:** Unos novog poštara u sistem<br>
**Opis:** Kao **administrator**, želim **popuniti formu sa podacima o novom uposleniku**, kako bi **on postao vidljiv u listi za dodjelu dnevnih ruta**.<br>
**Poslovna vrijednost:** Digitalizacija evidencije terenskih radnika.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:** <br>- *Pretpostavka:* Administrator ima pristup panelu za upravljanje osobljem.<br>
- *Otvoreno pitanje:* Da li poštar automatski dobija kredencijale za login čim ga admin doda, ili se to radi u odvojenom koraku?<br>
**Veze sa drugim storyjima:** <br>- **Osnova za:** US-13 (Pregled liste poštara) i US-22 (Dodjela rute).

---

## PBI-016 Pregled liste poštara

### User Stories
- **US-13:** Kao administrator ili dispečer, želim vidjeti listu svih registrovanih poštara sa njihovim trenutnim statusom, kako bih znao koga mogu zadužiti za nove zadatke.

### Poslovna vrijednost
Omogućava brz pregled dostupnih ljudskih resursa, što direktno utiče na brzinu reagovanja dispečera pri planiranju vanrednih ili redovnih pražnjenja sandučića.

### Prioritet: Medium

---

#### ID storyja: US-13
**Naziv storyja:** Tabelarni pregled poštara<br>
**Opis:** Kao **administrator ili dispečer**, želim **imati pregled svih poštara u obliku tabele sa opcijom pretrage po imenu**, kako bih **brzo pronašao određenog uposlenika**.<br>
**Poslovna vrijednost:** Transparentnost i lakša navigacija kroz bazu uposlenika.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:** <br>- *Pretpostavka:* Lista prikazuje samo relevantne podatke (npr. bez lozinki).<br>
- *Otvoreno pitanje:* Da li u listi treba biti vidljiva i zadnja poznata lokacija poštara radi bolje koordinacije?<br>
**Veze sa drugim storyjima:** <br> - **Zavisi od:** US-11 (Unos poštara).

---
## PBI-017 Dodavanje poštanskih sandučića

### User Stories
- **US-14:** Kao administrator ili dispečer, želim dodati novi poštanski sandučić u sistem unoseći njegovu adresu i precizne GPS koordinate, kako bi on bio vidljiv na mapi.
- **US-15:** Kao administrator, želim definisati tip sandučića (npr. standardni, prioritetni) i učestalost pražnjenja, kako bi sistem znao kada ga treba uključiti u rutu.

### Poslovna vrijednost
Precizna evidencija sandučića je temelj optimizacije. Unos GPS koordinata eliminiše nagađanje na terenu, dok definisanje prioriteta osigurava da se najbitniji sandučići prazne na vrijeme, čime se podiže kvalitet poštanske usluge.

### Prioritet: High

---

### Detaljna razrada Story-ja 

#### ID storyja: US-14
**Naziv storyja:** Unos lokacije sandučića putem koordinata<br>
**Opis:** Kao **administrator**, želim **unijeti geografsku širinu (Latitude) i dužinu (Longitude) za svaki sandučić**, kako bi **sistem mogao izračunati najkraću distancu između njih**.<br>
**Poslovna vrijednost:** Omogućava matematičku preciznost pri generisanju dnevnih ruta i smanjuje potrošnju goriva.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**<br> - *Pretpostavka:* Korisnik ima pristup Google Maps ili sličnom servisu za očitavanje koordinata.<br>
- *Otvoreno pitanje:* Da li sistem treba omogućiti "pinovanje" lokacije direktno na mapi unutar aplikacije umjesto ručnog kucanja brojeva?
**Veze sa drugim storyjima:**<br> - **Osnova za:** US-22 (Generisanje dnevne rute).

---

#### ID storyja: US-15
**Naziv storyja:** Konfiguracija prioriteta sandučića<br>
**Opis:** Kao **administrator**, želim **dodijeliti nivo hitnosti svakom sandučiću**, kako bi **algoritam znao koje lokacije ne smiju biti preskočene u kritičnim terminima**.<br>
**Poslovna vrijednost:** Diferencijacija usluge prema važnosti lokacije (npr. sandučići ispred bolnica ili državnih institucija).<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:** <br>- *Pretpostavka:* Postoje bar tri nivoa prioriteta (Nizak, Srednji, Visok).<br>
- *Otvoreno pitanje:* Da li se prioritet treba automatski podizati ako sandučić nije ispražnjen duže od 24 sata?<br>
**Veze sa drugim storyjima:**<br> - **Zavisi od:** US-14 (Postojanje lokacije).

---

## PBI-018 Pregled i pretraga sandučića

### User Stories
- **US-16:** Kao dispečer, želim vidjeti listu svih sandučića sa njihovim statusom (npr. pun/ispražnjen), kako bih imao kontrolu nad stanjem na terenu.

### Poslovna vrijednost
Vizuelni pregled svih tačaka u sistemu omogućava dispečeru da brzo uoči anomalije (npr. sandučić koji se predugo ne prazni).

### Prioritet: Medium

---

#### ID storyja: US-16
**Naziv storyja:** Pregled liste sandučića sa filterima<br>
**Opis:** Kao **dispečer**, želim **filtrirati sandučiće po naseljima ili po statusu zadnjeg pražnjenja**, kako bih **izdvojio samo one kritične za rad**. <br>
**Poslovna vrijednost:** Brža navigacija i bolja organizacija resursa. <br>
**Prioritet:** Medium <br>
**Pretpostavke i otvorena pitanja:** <br> - *Otvoreno pitanje:* Da li treba omogućiti izvoz ove liste u PDF format za potrebe arhive? <br>
**Veze sa drugim storyjima:** <br>- **Zavisi od:** US-14 (Dodavanje sandučića).

---
## PBI-019 Izmjena podataka o sandučiću

### User Stories
- **US-17:** Kao administrator, želim izmijeniti koordinate ili adresu postojećeg sandučića, kako bih osigurao da baza podataka odgovara stvarnom stanju na terenu nakon pomjeranja sandučića.

### Poslovna vrijednost
Održavanje tačnosti baze podataka. Pogrešne koordinate dovode do gubitka vremena poštara na terenu i neispravnog izračunavanja rute.

### Prioritet: Medium

---

### Detaljna razrada Story-ja 

#### ID storyja: US-17
**Naziv storyja:** Ažuriranje informacija o lokaciji<br>
**Opis:** Kao **administrator**, želim **otvoriti formu za uređivanje postojećeg sandučića i spasiti nove podatke**, kako bi **promjene bile odmah vidljive svim korisnicima sistema**.<br>
**Poslovna vrijednost:** Fleksibilnost sistema u slučaju urbanističkih promjena ili tehničkih grešaka pri prvom unosu.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:** <br>- *Pretpostavka:* Sandučić već postoji u bazi podataka.<br>
- *Otvoreno pitanje:* Da li sistem treba čuvati historiju promjena (ko je i kada izmijenio lokaciju) radi interne kontrole?<br>
**Veze sa drugim storyjima:** <br>- **Zavisi od:** US-14 (Dodavanje sandučića).

---

## PBI-020 Deaktivacija i brisanje sandučića

### User Stories
- **US-18:** Kao administrator, želim ukloniti sandučić iz aktivne baze, kako se on više ne bi pojavljivao u prijedlozima za dnevne rute.

### Poslovna vrijednost
Spriječavanje slanja poštara na lokacije koje više ne postoje, čime se direktno optimizuje radno vrijeme i resursi pošte.

### Prioritet: Medium

---

#### ID storyja: US-18
**Naziv storyja:** Uklanjanje sandučića iz sistema<br>
**Opis:** Kao **administrator**, želim **označiti sandučić kao neaktivan ili ga trajno obrisati**, kako bi **algoritam za rutiranje prestao uzimati tu tačku u obzir**.<br>
**Poslovna vrijednost:** Čišćenje baze od zastarjelih podataka.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:** <br>- *Pretpostavka:* Sandučić koji se briše ne smije biti dio rute koja je trenutno u toku (In Progress).<br>
- *Otvoreno pitanje:* Da li je bolje koristiti "Soft Delete" (samo sakriti sandučić) umjesto trajnog brisanja iz baze podataka zbog arhive raniijh izvještaja?<br>
**Veze sa drugim storyjima:**<br> - **Zavisi od:** US-14 (Dodavanje sandučića).<br>
- **Utiče na:** US-22 (Generisanje dnevne rute).

---
## PBI-021 Pregled mape sa svim sandučićima

### User Stories
- **US-19:** Kao dispečer, želim vidjeti sve registrovane sandučiće kao pinove na mapi, kako bih imao vizuelni pregled njihove prostorne raspoređenosti u gradu.
- **US-20:** Kao korisnik, želim klikom na pin sandučića na mapi vidjeti njegove detaljne informacije (adresa, status popunjenosti, zadnje pražnjenje), kako bih brzo dobio uvid u stanje te lokacije bez pretraživanja tabele.
- **US-21:** Kao dispečer, želim filtrirati prikaz na mapi prema statusu (npr. prikaži samo one koji zahtijevaju pražnjenje), kako bih vizuelno identifikovao kritične zone u gradu.

### Poslovna vrijednost
Mapa je ključni alat za donošenje odluka. Umjesto listanja stotina redova u tabeli, dispečer može u sekundi prepoznati skupine sandučića koji su blizu jedan drugom, što mu pomaže u logističkom planiranju čak i prije pokretanja automatskog algoritma.

### Prioritet: High

---

### Detaljna razrada Story-ja 

#### ID storyja: US-19
**Naziv storyja:** Vizuelni prikaz lokacija na mapi<br>
**Opis:** Kao **dispečer**, želim **otvoriti mapu grada na kojoj su označeni svi aktivni sandučići**, kako bih **imao globalni pregled poštanske mreže**.<br>
**Poslovna vrijednost:** Olakšava prostornu orijentaciju i planiranje ljudskih resursa.<br>
**Prioritet:** High <br>
**Pretpostavke i otvorena pitanja:**<br> - *Pretpostavka:* Sistem koristi eksterni API za renderovanje mape.<br>
- *Otvoreno pitanje:* Da li mapa treba imati različite ikone (boje pinova) zavisno od prioriteta sandučića?<br>
**Veze sa drugim storyjima:**<br> - **Zavisi od:** US-14 (GPS koordinate sandučića).

---

#### ID storyja: US-20
**Naziv storyja:** Brzi uvid u detalje sandučića (Popup)<br>
**Opis:** Kao **korisnik**, želim **kliknuti na bilo koji pin na mapi i vidjeti "pop-up" prozor sa podacima**, kako bih **izbjegao prebacivanje između mape i tabelarnog pregleda**.<br>
**Poslovna vrijednost:** Povećanje efikasnosti rada kroz smanjenje broja klikova potrebnih za dobijanje informacija.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:** <br>- *Pretpostavka:* Podaci u prozorčiću se osvježavaju u realnom vremenu (ako je to moguće).<br>
**Veze sa drugim storyjima:**<br> - **Zavisi od:** US-19 (Prikaz pinova).

---

#### ID storyja: US-21
**Naziv storyja:** Filtriranje prikaza na mapi<br>
**Opis:** Kao **dispečer**, želim **isključiti ili uključiti slojeve na mapi (npr. samo prazni, samo puni)**, kako bih **fokusirao pažnju na problematične tačke**.<br>
**Poslovna vrijednost:** Brža identifikacija prioriteta na terenu.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:** <br> - *Otvoreno pitanje:* Da li treba omogućiti čuvanje filtera kao podrazumijevani prikaz za određenog dispečera? <br>
**Veze sa drugim storyjima:** <br> - **Zavisi od:** US-19 i US-16 (Logika filtriranja).

---
## PBI-022 Generisanje dnevne rute

### User Stories
- **US-22:** Kao dispečer, želim pokrenuti algoritam za automatsko generisanje rute, kako bih dobio najkraći put koji povezuje sve kritične sandučiće.
- **US-23:** Kao dispečer, želim vidjeti iscrtanu putanju na mapi sa redoslijedom obilaska tačaka, kako bih vizuelno provjerio logičnost predložene rute.
- **US-24:** Kao dispečer, želim imati mogućnost ručne izmjene redoslijeda tačaka na generisanoj ruti, kako bih uvažio nepredviđene okolnosti na terenu (npr. radovi na putu).
- **US-25:** Kao dispečer, želim dodijeliti finalizovanu rutu specifičnom poštaru, kako bi on dobio instrukcije za rad na svoj mobilni uređaj.

### Poslovna vrijednost
Ovo je srce sistema. Automatizacija rute smanjuje pređenu kilometražu za 15-30%, štedi gorivo i osigurava da nijedan kritični sandučić ne bude zaboravljen. Ručna korekcija dodaje neophodnu fleksibilnost koju automatika nekada ne može predvidjeti.

### Prioritet: High 

---

### Detaljna razrada Story-ja 

#### ID storyja: US-22
**Naziv storyja:** Automatizovani proračun optimalne putanje<br>
**Opis:** Kao **dispečer**, želim **klikom na dugme "Generiši" aktivirati algoritam**, koji će **na osnovu GPS koordinata i prioriteta sandučića kreirati listu obilaska**.<br>
**Poslovna vrijednost:** Eliminacija manuelnog planiranja i smanjenje ljudske greške.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:** <br> - *Pretpostavka:* Sistem ima pristup koordinatama svih sandučića koji su označeni za pražnjenje.<br>
- *Otvoreno pitanje:* Koji algoritam koristiti (npr. Dijkstra ili Nearest Neighbor) s obzirom na broj tačaka i performanse sistema?<br>
**Veze sa drugim storyjima:** <br>- **Zavisi od:** US-14 (Koordinate) i US-15 (Prioriteti).<br>
- **Osnova za:** US-23 (Prikaz na mapi).

---

#### ID storyja: US-23
**Naziv storyja:** Vizuelni prikaz rute na mapi<br>
**Opis:** Kao **dispečer**, želim da se **generisana ruta prikaže kao povezana linija na interaktivnoj mapi**, kako bih **jasno vidio planirano kretanje poštara**. <br>
**Poslovna vrijednost:** Bolja kontrola i lakše uočavanje nelogičnosti u putanji.<br>
**Prioritet:** High <br>
**Pretpostavke i otvorena pitanja:** <br>- *Otvoreno pitanje:* Da li ruta treba pratiti stvarne ceste (Road-snapping) ili prikazivati "zračnu liniju" između tačaka?<br>
**Veze sa drugim storyjima:**<br> - **Zavisi od:** US-19 (Osnovna mapa) i US-22 (Proračun).

---

#### ID storyja: US-24
**Naziv storyja:** Ručna modifikacija rute (Drag & Drop)<br>
**Opis:** Kao **dispečer**, želim **promijeniti redoslijed tačaka u listi ili na mapi**, kako bih **prilagodio rutu trenutnim uslovima u saobraćaju**.<br>
**Poslovna vrijednost:** Fleksibilnost sistema u realnim situacijama.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:**<br> - *Pretpostavka:* Sistem automatski preračunava ukupnu dužinu rute nakon svake ručne promjene.<br>
**Veze sa drugim storyjima:** <br>- **Nadovezuje se na:** US-22.

---

#### ID storyja: US-25
**Naziv storyja:** Dodjela rute izvršiocu<br>
**Opis:** Kao **dispečer**, želim **izabrati poštara iz liste dostupnih radnika i poslati mu rutu**, kako bi on **mogao započeti sa radnim zadatkom**.<br>
**Poslovna vrijednost:** Uspostavljanje jasne odgovornosti za izvršenje posla.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:**<br> - *Otvoreno pitanje:* Na koji način poštar dobija obavijest (npr. In-app notifikacija ili SMS)?<br>
**Veze sa drugim storyjima:** <br>- **Zavisi od:** US-13 (Lista poštara) i US-22 (Postojanje rute).

---

## PBI-026 Pregled i navigacija dodijeljenom rutom

### User Stories
- **US-26:** Kao poštar, želim vidjeti listu svih sandučića koje trebam isprazniti u sklopu moje trenutne rute, kako bih planirao svoje kretanje.
- **US-27:** Kao poštar, želim pokrenuti navigaciju do sljedećeg sandučića na ruti koristeći GPS lokaciju, kako bih najbržim putem stigao do cilja.

### Poslovna vrijednost
Povećava brzinu rada na terenu, posebno za nove poštare ili one koji mijenjaju rejone. Digitalni plan rada eliminiše potrebu za papirnim spiskovima i smanjuje mogućnost da se neka ulica ili sandučić slučajno preskoče.

### Prioritet: High

---

### Detaljna razrada Story-ja 

#### ID storyja: US-26
**Naziv storyja:** Mobilni pregled dnevnog zadatka<br>
**Opis:** Kao **poštar**, želim **na svom uređaju vidjeti hronološki poredane adrese sandučića**, kako bih **imao jasan uvid u obim posla za taj dan**.<br>
**Poslovna vrijednost:** Transparentnost rada i bolja organizacija vremena uposlenika.<br>
**Prioritet:** High <br>
**Pretpostavke i otvorena pitanja:** <br> - *Pretpostavka:* Poštar je prijavljen na sistem i dodijeljena mu je aktivna ruta (US-25).<br>
- *Otvoreno pitanje:* Da li poštar treba vidjeti procjenu vremena potrebnog za završetak cijele rute?<br>
**Veze sa drugim storyjima:** <br> - **Direktna zavisnost:** US-25 (Dodjela rute).

---

#### ID storyja: US-27
**Naziv storyja:** Navigacija do specifičnog sandučića (OSM integracija)<br>
**Opis:** Kao **poštar**, želim **klikom na adresu otvoriti upute za navigaciju zasnovane na OpenStreetMap podacima**, kako bi me **sistem vodio tačno do koordinata sandučića najefikasnijom rutom**.<br>
**Poslovna vrijednost:** Smanjenje vremena provedenog u saobraćaju i precizno pronalaženje sandučića na nepoznatom terenu bez zavisnosti od komercijalnih provajdera mapa.<br>
**Prioritet:** Medium<br>
**Pretpostavke i otvorena pitanja:** <br>- *Pretpostavka:* Mobilna aplikacija koristi OSM podlogu. <br>
- *Otvoreno pitanje:* Da li navigacija treba raditi u "offline" režimu u slučaju slabog signala na određenim lokacijama?<br>
**Veze sa drugim storyjima:** <br>- **Zavisi od:** US-14 (GPS koordinate sandučića).

---

## PBI-027 Ažuriranje statusa i potvrda pražnjenja

### User Stories
- **US-28:** Kao poštar, želim označiti sandučić kao "ispražnjen" odmah nakon obavljenog posla, kako bi dispečer imao informaciju o progresu rute u realnom vremenu.
- **US-29:** Kao poštar, želim prijaviti problem sa sandučićem (npr. oštećena brava, pun sandučić, nedostupan prilaz) putem komentara ili fotografije, kako bi se kvar mogao sanirati.

### Poslovna vrijednost
Zatvaranje petlje povratnih informacija. Ovo omogućava menadžmentu da prati efikasnost u realnom vremenu i da brzo reaguje na tehničke probleme na terenu bez čekanja da se poštar vrati u bazu.

### Prioritet: High

---

#### ID storyja: US-28
**Naziv storyja:** Potvrda izvršenja zadatka<br>
**Opis:** Kao **poštar**, želim **jednim klikom promijeniti status sandučića u "Završeno"**, kako bi **sistem automatski ažurirao mapu i prebacio fokus na sljedeću tačku**.<br>
**Poslovna vrijednost:** Real-time praćenje progresa rada.<br>
**Prioritet:** High<br>
**Pretpostavke i otvorena pitanja:** <br>- *Pretpostavka:* Sistem bilježi tačno vrijeme (Timestamp) klika radi kasnije analize.<br>
- *Otvoreno pitanje:* Da li treba uvesti geofencing (da se status može promijeniti samo ako je poštar fizički blizu sandučića)?<br>
**Veze sa drugim storyjima:**<br> - **Utiče na:** US-16 (Statusi sandučića).

---

#### ID storyja: US-29
**Naziv storyja:** Prijava incidenta na terenu<br>
**Opis:** Kao **poštar**, želim **poslati kratku poruku ili upozorenje dispečeru ako sandučić nije moguće isprazniti**, kako bi **problem bio evidentiran**.<br>
**Poslovna vrijednost:** Brže održavanje infrastrukture i smanjenje broja pritužbi građana na nefunkcionalne sandučiće.<br>
**Prioritet:** Low<br>
**Pretpostavke i otvorena pitanja:** <br>- *Otvoreno pitanje:* Da li je u ovoj fazi potreban upload fotografije oštećenja ili je tekstualni opis dovoljan?<br>
**Veze sa drugim storyjima:**<br> - **Utiče na:** US-30 (Završni izvještaj rute).

---

## PBI-028 Završetak rute i sumarni izvještaj

### User Stories
- **US-30:** Kao poštar, želim označiti cijelu rutu kao "Završenu" nakon pražnjenja zadnjeg sandučića, kako bi dispečer dobio automatsku potvrdu da je moj rejon pokriven.
- **US-31:** Kao korisnik, želim vidjeti kratak rezime rute na kraju rada (broj ispražnjenih sandučića, vrijeme početka i kraja), kako bih imao uvid u svoju dnevnu produktivnost.

### Poslovna vrijednost
Završetak rute omogućava sistemu da oslobodi poštara za nove zadatke i arhivira podatke o dnevnim aktivnostima. Ovo je ključno za obračun radnih sati i praćenje ukupne efikasnosti poštanske mreže.

### Prioritet: Medium

---

### Detaljna razrada Story-ja

#### ID storyja: US-30
**Naziv storyja:** Finalizacija dnevnog zadatka  
**Opis:** Kao **poštar**, želim **kliknuti na dugme 'Završi rutu'**, čime se **status rute mijenja u 'Completed' i šalje notifikacija dispečeru**.  
**Poslovna vrijednost:** Trenutna informacija menadžmentu da je proces pražnjenja u tom rejonu uspješno okončan.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:** - *Pretpostavka:* Sistem ne dozvoljava završetak rute ako bar jedan sandučić nije dobio status (ispražnjen ili prijavljen kvar).
- *Otvoreno pitanje:* Da li sistem treba onemogućiti završetak rute ako poštar nije fizički blizu baze/pošte?  
**Veze sa drugim storyjima:** - **Zavisi od:** US-28 (Potvrda pražnjenja).

---

## PBI-029 Statistika i arhiva ruta (OpenStreetMap integracija)

### User Stories
- **US-32:** Kao administrator, želim pregledati arhivirane rute na OpenStreetMap podlozi, kako bih analizirao historijsko kretanje poštara i optimizovao buduće rejone.
- **US-33:** Kao administrator, želim izvesti statistički izvještaj o broju ispražnjenih sandučića po danima/mjesecima, kako bih donio odluke o potrebi za novim sandučićima ili radnicima.

### Poslovna vrijednost
Arhiva i statistika pretvaraju sirove podatke u poslovnu inteligenciju. Analizom starih ruta na OpenStreetMap mapi, uprava može uočiti uska grla u saobraćaju ili neravnomjerno raspoređen teret posla među poštarima.

### Prioritet: Low

---

#### ID storyja: US-32
**Naziv storyja:** Pregled historije kretanja na OSM mapi  
**Opis:** Kao **administrator**, želim **učitati putanju bilo koje stare rute na OpenStreetMap interfejs**, kako bih **vizuelno uporedio planiranu i stvarnu putanju poštara**.  
**Poslovna vrijednost:** Kontrola kvaliteta i optimizacija logističkih procesa.  
**Prioritet:** Low  
**Pretpostavke i otvorena pitanja:** - *Pretpostavka:* Koristimo OpenStreetMap podlogu sa Leaflet bibliotekom za renderovanje historijskih linija kretanja.
- *Otvoreno pitanje:* Koliko dugo (mjeseci/godina) sistem treba čuvati detaljne GPS tragove starih ruta prije brisanja radi uštede prostora?  
**Veze sa drugim storyjima:** - **Zavisi od:** US-22 (Logika rute) i integracije sa OSM.
