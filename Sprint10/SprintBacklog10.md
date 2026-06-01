## Sprint 10 (PBI-049, PBI-050, PBI-051)
**Cilj:** Uvođenje arhive za retrospektivnu analizu ruta, izvještavanje o učinku po različitim parametrima, te pretragu i filtriranje.

## Tabela sprint backloga
| ID | User Story | Naziv stavke / zadatka | Odgovorna osoba | Procjena | Status | Napomena |
|---|---|---|---|---|---|---|
| PBI-049 / US-34 | Pregled arhive realizovanih ruta | Historija obilazaka i arhiva ruta | Kerim | 4h | Done | Arhiva realizovanih ruta; tabelarni prikaz, filtriranje po datumu i poštaru, read-only pregled arhive |
| PBI-049 / US-35 | Detaljni uvid u arhiviranu rutu | Historija obilazaka i arhiva ruta | Kerim | 4h | Done | Detaljan pregled statusa sandučića, timestamp aktivnosti, mapa obilaska, export u Excel, audit trail |
| PBI-050 / US-36 | Izvještaj o učinku poštara | Prošireno operativno izvještavanje | Ibrahim | 4h | Done | KPI tabela, procenat uspješnosti, stubni grafikon, filtriranje po periodu, export u CSV |
| PBI-050 / US-37 | Analiza realizacije po tipu sandučića | Prošireno operativno izvještavanje | Aldin | 3h | To Do | Statistika po tipu sandučića, pie chart, analiza problema, poređenje tipova, export u Excel |
| PBI-051 / US-38 | Brza pretraga sandučića | Pretraga i filtriranje sandučića | Faruk | 2h | Done | Real-time pretraga po adresi/ID-u, case-insensitive pretraga, parcijalno pretraživanje |
| PBI-051 / US-39 | Filtriranje po atributima | Pretraga i filtriranje sandučića | Emrah | 3h | Done | Filteri po tipu/statusu/prioritetu, kombinovani filteri, reset dugme, integracija sa pretragom |
| PBI-052 / US-40 | Pregled detalja problematične lokacije | Upravljanje problematičnim lokacijama | Rubina | 3h | Done | Detaljan pregled problema, timeline aktivnosti, prikaz razloga nedostupnosti, read-only pregled incidenta |
| PBI-052 / US-41 | Komentarisanje problema između dispečera i poštara | Upravljanje problematičnim lokacijama | Rubina | 3h | Done | Komentari sa timestampom i autorom, conversation prikaz, real-time osvježavanje |
| PBI-052 / US-42 | Dodjela akcije za problematičnu lokaciju | Upravljanje problematičnim lokacijama | Rubina | 2h | Done | Dodjela akcije (ponovni pokušaj, drugi poštar, odgoda), evidencija akcije, prikaz trenutnog statusa |
| PBI-052 / US-43 | Evidencija statusa rješavanja problema | Upravljanje problematičnim lokacijama | Nejla| 2h | Done | Statusi problema (Otvoren, U obradi, Riješen), filtriranje problema, audit trail |
| PBI-052 / US-44 | Notifikacije za ažuriranje problema | Upravljanje problematičnim lokacijama | Nejla | 2h | Done | Real-time notifikacije poštaru, indikator nepročitanih obavijesti, historija notifikacija |

---

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

---

### PBI-052 Upravljanje problematičnim lokacijama

#### User Stories
- **US-40:** Kao dispečer, želim otvoriti detalje problematične lokacije direktno iz aktivne rute, kako bih mogao analizirati problem i odlučiti o narednim koracima.
- **US-41:** Kao dispečer ili poštar, želim ostaviti komentar vezan za problematičnu lokaciju, kako bi komunikacija ostala evidentirana unutar sistema.
- **US-42:** Kao dispečer, želim dodijeliti akciju za problematičnu lokaciju (ponovni pokušaj, dodjela drugom poštaru ili odgoda), kako bih organizovao dalje izvršenje zadatka.
- **US-43:** Kao administrator ili dispečer, želim pratiti status rješavanja problema, kako bih imao pregled svih aktivnih i zatvorenih problema na terenu.
- **US-44:** Kao poštar, želim dobiti obavijest kada dispečer ažurira problem ili ostavi komentar, kako bih znao koje dalje korake trebam poduzeti.

#### Poslovna vrijednost
Uvođenje upravljanja problematičnim lokacijama zatvara operativni ciklus između poštara i dispečera. Sistem više ne služi samo za evidentiranje problema, već omogućava aktivno rješavanje terenskih poteškoća, bolju koordinaciju tima i potpunu evidenciju svih odluka i komunikacije vezanih za problematične lokacije.

#### Prioritet: Medium

---

#### Detaljna razrada Story-ja

##### ID storyja: US-40
**Naziv storyja:** Pregled detalja problematične lokacije  
**Opis:** Kao **dispečer**, želim **kliknuti na problematičnu lokaciju unutar aktivne rute**, kako bih **vidio detaljne informacije o problemu i historiju aktivnosti vezanu za tu lokaciju**.  
**Poslovna vrijednost:** Brži pregled i analiza problema bez potrebe za dodatnom komunikacijom izvan sistema.  
**Prioritet:** Medium  

**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Problematična lokacija je prethodno označena kao "Nedostupna" od strane poštara.
- *Otvoreno pitanje:* Da li detaljni prikaz treba sadržavati i prethodne historijske incidente za istu lokaciju?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-28 (Označavanje nedostupne lokacije), US-29 (Praćenje statusa rute od strane dispečera).
- **Osnova za:** US-41 (Komentarisanje problema), US-42 (Dodjela akcije).

#### Acceptance criteria

- Kada dispečer klikne na problematičnu lokaciju unutar detalja rute, sistem mora otvoriti poseban prikaz problema.
- Sistem mora prikazati adresu sandučića, ID lokacije, vrijeme prijave problema, ime poštara i razlog nedostupnosti.
- Sistem mora prikazati sve prethodne aktivnosti vezane za problem kroz vremensku liniju (timeline).
- Problematična lokacija mora biti vizuelno označena unutar mape i liste sandučića.
- Sistem mora omogućiti pristup detaljima problema isključivo korisnicima sa ulogom dispečer ili administrator.

---

##### ID storyja: US-41
**Naziv storyja:** Komentarisanje problema između dispečera i poštara  
**Opis:** Kao **dispečer ili poštar**, želim **ostaviti komentar vezan za problematičnu lokaciju**, kako bi **komunikacija ostala evidentirana unutar sistema**.  
**Poslovna vrijednost:** Centralizovana komunikacija smanjuje potrebu za telefonskim pozivima i omogućava audit trail svih odluka i instrukcija.  
**Prioritet:** Medium  

**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Svaki komentar se evidentira sa autorom i timestampom.
- *Otvoreno pitanje:* Da li komentari trebaju podržavati prilaganje slika oštećenja ili prepreka?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-40 (Pregled detalja problematične lokacije).

#### Acceptance criteria

- Sistem mora omogućiti unos tekstualnog komentara unutar detalja problematične lokacije.
- Svaki komentar mora sadržavati ime autora, ulogu korisnika i tačno vrijeme objave.
- Novi komentari moraju biti prikazani hronološkim redoslijedom unutar conversation prikaza.
- Poštar mora imati mogućnost pregleda komentara vezanih isključivo za svoje rute.
- Sistem ne smije dozvoliti unos praznog komentara.
- Kada novi komentar bude dodan, sistem mora automatski osvježiti prikaz bez reload-a stranice.

---

##### ID storyja: US-42
**Naziv storyja:** Dodjela akcije za problematičnu lokaciju  
**Opis:** Kao **dispečer**, želim **odabrati narednu akciju za problematičnu lokaciju**, kako bih **organizovao dalje rješavanje problema na terenu**.  
**Poslovna vrijednost:** Omogućava strukturisano upravljanje incidentima i smanjuje mogućnost zaboravljenih ili neriješenih problema.  
**Prioritet:** Medium  

**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Jedna problematična lokacija može imati samo jednu aktivnu akciju u datom trenutku.

**Veze sa drugim storyjima:**
- **Zavisi od:** US-40 (Pregled detalja problematične lokacije).

#### Acceptance criteria

- Sistem mora omogućiti dispečeru odabir jedne od akcija: "Ponovni pokušaj", "Dodijeli drugom poštaru" ili "Ostavi za naredni dan".
- Kada dispečer odabere akciju "Dodijeli drugom poštaru", sistem mora prikazati listu dostupnih poštara.
- Sistem mora evidentirati ko je dodijelio akciju i kada je akcija postavljena.
- Trenutno aktivna akcija mora biti jasno prikazana unutar detalja problema.
- Sistem mora omogućiti promjenu akcije sve dok problem nije označen kao riješen.

---

##### ID storyja: US-43
**Naziv storyja:** Evidencija statusa rješavanja problema  
**Opis:** Kao **administrator ili dispečer**, želim **pratiti status problema**, kako bih **imao pregled svih aktivnih i zatvorenih problema na terenu**.  
**Poslovna vrijednost:** Omogućava praćenje toka rješavanja problema i identifikaciju lokacija koje često uzrokuju operativne poteškoće.  
**Prioritet:** Medium  

**Veze sa drugim storyjima:**
- **Zavisi od:** US-40 (Pregled detalja problematične lokacije), US-42 (Dodjela akcije).

#### Acceptance criteria

- Sistem mora za svaki problem prikazati jedan od statusa: "Otvoren", "U obradi" ili "Riješen".
- Kada poštar prvi put označi lokaciju kao nedostupnu, sistem mora automatski postaviti status problema na "Otvoren".
- Kada dispečer dodijeli akciju ili ostavi komentar, sistem mora automatski promijeniti status u "U obradi".
- Administrator ili dispečer mora imati mogućnost ručnog označavanja problema kao "Riješen".
- Sistem mora omogućiti filtriranje problema prema statusu i datumu prijave.
- Zatvoreni problemi moraju ostati dostupni unutar arhive sistema radi audit trail-a.

---

##### ID storyja: US-44
**Naziv storyja:** Notifikacije za ažuriranje problema  
**Opis:** Kao **poštar**, želim **dobiti obavijest kada dispečer odgovori ili ažurira problem**, kako bih **znao koje dalje korake trebam poduzeti**.  
**Poslovna vrijednost:** Poboljšava koordinaciju između terena i dispečera te ubrzava reakciju na operativne probleme.  
**Prioritet:** Medium  

**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem već podržava osnovne real-time notifikacije (US-29).

**Veze sa drugim storyjima:**
- **Zavisi od:** US-41 (Komentarisanje problema), US-42 (Dodjela akcije).

#### Acceptance criteria

- Kada dispečer doda novi komentar ili promijeni akciju problema, sistem mora poštaru prikazati real-time notifikaciju.
- Notifikacija mora sadržavati naziv lokacije i kratki opis promjene.
- Klikom na notifikaciju poštar mora biti preusmjeren na detalje problema.
- Sistem mora označiti nepročitane notifikacije vizuelnim indikatorom.
- Nakon otvaranja detalja problema, notifikacija mora biti označena kao pročitana.
- Sistem mora čuvati historiju notifikacija najmanje za tekući radni dan.

---
