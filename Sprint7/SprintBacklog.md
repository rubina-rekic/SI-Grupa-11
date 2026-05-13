# Sprint 7 Backlog

**Sprint cilj:** Definisati logičke parametre sistema kroz određivanje važnosti sandučića i njihovih vremenskih ograničenja — implementirati prioritete sandučića i evidenciju radnih pravila kako bi sistem imao sve potrebne podatke za generisanje operativno izvodivih ruta.

## Tabela sprint backloga

| ID | Naziv stavke / zadatka | Odgovorna osoba | Procjena | Status | Napomena |
|---|---|---|---|---|---|
| PBI-020 / US-18 | Postavljanje prioriteta sandučića | Rubina | 3h 30min | Done | Dropdown Visok/Srednji/Nizak, kodiranje bojama, automatski prioritet, obrazloženje promjene, inicijalna vrijednost Srednji |
| PBI-020 / US-19 | Sortiranje liste sandučića po prioritetu | Ibrahim | 2h | To Do | Dugme "Sortiraj po prioritetu", jednim klikom, desc redoslijed (Visok→Srednji→Nizak), vizuelni indikator aktivnog sortiranja |
| PBI-020 / US-20 | Pregled historije promjena prioriteta sandučića | Faruk | 3h | To Do | Tabela s kolonama: Datum/Vrijeme, Administrator, Stari prioritet, Novi prioritet, Obrazloženje; dostupna adminu iz forme sandučića |
| PBI-021 / US-32 | Definisanje vremenskih okvira dostupnosti sandučića | Rubina | 4h | Done | Time picker 24h format, validacija vremena, dva termina dnevno, checkbox 24/7, upozorenje pri konfliktu s rutom |
| PBI-021 / US-33 | Definisanje radnih dana sandučića | Nejla, Aldin | 2h | Done | Sedam checkbox kontrola, default Pon-Pet, validacija min. jedan dan, Označi sve / Odznači sve |
| PBI-022 / US-22 | Generisanje dnevne rute | Emrah, Kerim | 4h | Done | Na osnovu GPS koordinata i prioriteta sandučića kreirati prijedlog dnevne rute za odabranog poštara |

---

## PBI-020 Definisanje prioriteta sandučića

#### User Stories
- **US-18:** Kao administrator, želim postaviti ili izmijeniti prioritet za pražnjenje/punjenje sandučića, kako bi sistem znao koje lokacije imaju veći operativni značaj.
- **US-19:** Kao administrator, želim sortirati listu sandučića po prioritetu jednim klikom, kako bih brzo vidio najkritičnije lokacije na vrhu liste.
- **US-20:** Kao administrator, želim pregledati historiju promjena prioriteta sandučića, kako bih znao ko je, kada i zbog čega izmijenio prioritet.

#### Poslovna vrijednost
Prioriteti omogućavaju da sistem i dispečer razlikuju kritične od manje kritičnih lokacija, što direktno utiče na kvalitet planiranja i redoslijed obilaska. Sortiranje i historija promjena dodatno osiguravaju operativnu preglednost i revizijsku sljedivost.

#### Prioritet: High

---

##### ID storyja: US-18
**Naziv storyja:** Postavljanje prioriteta sandučića  
**Opis:** Kao **administrator**, želim **dodijeliti ili izmijeniti nivo prioriteta za pojedini sandučić**, kako bi **sistem mogao uzeti u obzir njegov značaj pri planiranju pražnjenja i punjenja**.  
**Poslovna vrijednost:** Diferencijacija usluge prema važnosti lokacije i bolja podrška algoritmu za planiranje.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Postoji jasan skup vrijednosti prioriteta (npr. nizak, srednji, visok).
- *Otvoreno pitanje:* Da li se prioritet treba mijenjati isključivo ručno ili može biti i automatski predložen na osnovu pravila sistema?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-14 i US-15.
- **Utiče na:** US-19 (Sortiranje liste sandučića po prioritetu), US-20 (Pregled historije promjena prioriteta), US-22 (Generisanje dnevne rute).

#### Acceptance criteria

- Kada administrator otvori formu za sandučić (unos ili uređivanje), tada sistem mora ponuditi padajući meni "Prioritet" sa tri fiksne opcije: Visok (High), Srednji (Medium) i Nizak (Low).
- Sistem mora vizuelno označiti nivoe prioriteta u svim tabelama i mapama koristeći kodiranje bojama: Visok — crveni indikator, Srednji — žuti/narandžasti indikator, Nizak — zeleni indikator.
- Sistem mora omogućiti "Automatski prioritet"; ako je ova opcija uključena, tada sistem automatski postavlja prioritet na "Visok" za sve sandučiće tipa "Specijalni/Prioritetni" ili one locirane u zoni centra grada.
- Kada administrator ručno promijeni prioritet, tada sistem mora tražiti kratko obrazloženje koje se čuva u bazi.
- Sistem ne smije dozvoliti da sandučić ostane bez dodijeljenog prioriteta; inicijalna vrijednost pri kreiranju mora biti "Srednji".
- Kada se prioritet promijeni, tada se ta promjena mora odmah odraziti na listu prioriteta za generisanje rute u US-22 — sandučići sa Visokim prioritetom automatski se pomjeraju na vrh liste za obilazak.

---

##### ID storyja: US-19
**Naziv storyja:** Sortiranje liste sandučića po prioritetu  
**Opis:** Kao **administrator**, želim **klikom na dugme "Sortiraj po prioritetu" sortirati listu sandučića silaznim redoslijedom (Visok → Srednji → Nizak)**, kako bih **brzo stekao pregled nad najkritičnijim lokacijama i mogao donositi operativne odluke bez ručnog pretražavanja**.  
**Poslovna vrijednost:** Smanjuje kognitivno opterećenje administratora — kritične lokacije su odmah vidljive na vrhu liste bez potrebe za filtriranjem ili skrolanjem.  
**Prioritet:** Medium  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Lista sandučića prikazana je u tabličnom prikazu s kojeg je dugme dostupno.
- *Pretpostavka:* Sandučići istog prioriteta zadržavaju međusobni poredak iz prethodnog sortiranja (stabilan sort).
- *Otvoreno pitanje:* Da li dugme treba omogućiti i obrnutu akciju (toggle Visok→Nizak / Nizak→Visok), ili je dovoljan samo jedan smjer?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-15 (Pregled liste sandučića), US-18 (Postavljanje prioriteta sandučića).
- **Utiče na:** US-22 (Generisanje dnevne rute) — administrator može vizuelno provjeriti prioritete prije pokretanja generisanja.

#### Acceptance criteria

- Kada administrator otvori listu sandučića, tada sistem mora prikazati dugme **"Sortiraj po prioritetu"** iznad tabele.
- Kada administrator klikne na dugme, tada sistem mora **odmah** (bez ponovnog učitavanja stranice) preurediti redoslijed prikazanih redova: sandučići s prioritetom **Visok** prikazuju se prvi, zatim **Srednji**, a na kraju **Nizak**.
- Sistem mora vizuelno označiti da je aktivno sortiranje po prioritetu (npr. ikonom strelice prema dolje pored naziva kolone "Prioritet" ili istaknutim stanjem dugmeta).
- Sandučići unutar istog nivoa prioriteta moraju zadržati stabilan međusobni poredak.
- Kada administrator klikne dugme drugi put, sortiranje se **invertira** (Nizak → Srednji → Visok), a vizuelni indikator se ažurira (strelica prema gore).
- Kada se prioritet bilo kojeg sandučića promijeni (US-18), tada lista mora automatski ažurirati prikaz ako je sortiranje po prioritetu trenutno aktivno — novi poredak mora biti primijenjen odmah.
- Sistem mora omogućiti kombinovanje sortiranja po prioritetu s postojećim filterima (npr. aktivni/neaktivni sandučići) — sortiranje se primjenjuje samo na filtrirani skup.

---

##### ID storyja: US-20
**Naziv storyja:** Pregled historije promjena prioriteta sandučića  
**Opis:** Kao **administrator**, želim **pregledati kompletnu historiju promjena prioriteta za određeni sandučić**, koja prikazuje **ko je, kada i zbog čega izmijenio prioritet**, kako bih **imao punu revizijsku sljedivost i mogao razumjeti zašto je lokacija dobila trenutni prioritet**.  
**Poslovna vrijednost:** Osigurava transparentnost i odgovornost u upravljanju podacima — administrator može otkriti neočekivane promjene prioriteta, razumjeti poslovni kontekst odluka i po potrebi ispraviti grešku.  
**Prioritet:** Medium  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Svaka ručna promjena prioriteta (US-18) automatski kreira zapis u bazi s metapodacima (autor, timestamp, obrazloženje).
- *Pretpostavka:* Automatske promjene prioriteta (US-18 — opcija "Automatski prioritet") također se bilježe, ali s autorom označenim kao "Sistem".
- *Otvoreno pitanje:* Da li je potrebna mogućnost eksporta historije u CSV ili PDF format?
- *Otvoreno pitanje:* Koliko dugo se čuvaju zapisi historije (retention policy)?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-18 (Postavljanje prioriteta sandučića) — historija postoji samo ako US-18 bilježi promjene.
- **Povezano sa:** US-15 (Pregled liste sandučića) — historija je dostupna iz detalja sandučića.

#### Acceptance criteria

- Kada administrator otvori formu za uređivanje sandučića, tada sistem mora prikazati sekciju ili tab **"Historija promjena prioriteta"**.
- Sekcija mora prikazivati sve zapise u tabeli sa sljedećim kolonama:
  - **Datum i vrijeme** — u formatu DD.MM.YYYY HH:mm
  - **Administrator** — puno ime korisnika koji je napravio promjenu (ili "Sistem" za automatske promjene)
  - **Stari prioritet** — vrijednost prije promjene, vizuelno označena bojom (crvena/žuta/zelena prema US-18)
  - **Novi prioritet** — vrijednost nakon promjene, vizuelno označena bojom
  - **Obrazloženje** — tekst koji je administrator unio pri promjeni (US-18)
- Zapisi moraju biti sortirani **od najnovijeg prema najstarijem** (desc po datumu).
- Sistem mora prikazati i automatske promjene (pokrenute opcijom "Automatski prioritet" iz US-18), s autorom "Sistem" i obrazloženjem koje navodi primijenjeno pravilo (npr. *"Automatski prioritet: sandučić tipa Specijalni"*).
- Kada sandučić još nema nijednu promjenu prioriteta (npr. tek kreiran), tada sistem mora prikazati poruku: **"Nema evidentirane historije promjena prioriteta."**
- Sistem mora onemogućiti brisanje ili uređivanje zapisa historije — svi zapisi su isključivo za čitanje.
- Kada lista ima više od 20 zapisa, sistem mora primijeniti paginaciju ili lazy load — maksimalno 20 zapisa po stranici.

---

## PBI-021 Evidencija radnih pravila sandučića

#### User Stories
- **US-32:** Kao administrator ili dispečer, želim definisati vremenske okvire unutar kojih je sandučić dostupan za pražnjenje, kako bi algoritam planirao rute u skladu sa radnim vremenom.
- **US-33:** Kao administrator ili dispečer, želim odrediti specifične radne dane za svaki sandučić, kako bi se izbjeglo planiranje obilazaka u danima kada sandučić nije dostupan.

#### Poslovna vrijednost
Uvođenje radnih pravila osigurava da generisane rute budu operativno izvodljive u stvarnim uslovima. Time se eliminišu situacije u kojima poštar dolazi do zaključanog objekta, čime se direktno štedi vrijeme, smanjuju troškovi goriva i povećava ukupna efikasnost logističke mreže.

#### Prioritet: High

---

##### ID storyja: US-32
**Naziv storyja:** Definisanje vremenskih okvira dostupnosti sandučića  
**Opis:** Kao **administrator**, želim **unijeti vrijeme početka i kraja dostupnosti za svaki sandučić**, kako bi **sistem mogao izračunati optimalno vrijeme dolaska poštara**.  
**Poslovna vrijednost:** Osigurava da poštar stigne na lokaciju isključivo tokom njenog radnog vremena.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Vrijeme se unosi u formatu HH:mm (24h format).
- *Otvoreno pitanje:* Da li sistem treba dozvoliti unos više različitih termina unutar jednog dana?

**Veze sa drugim storyjima:**
- **Utiče na:** US-19 i US-20 (Algoritam za generisanje ruta).

#### Acceptance criteria

- Kada administrator otvori formu za sandučić, tada sistem mora prikazati sekciju "Dostupnost" sa poljima: Početak (Time picker) i Kraj (Time picker) u 24-satnom formatu (npr. 08:00 - 16:00).
- Sistem mora onemogućiti spašavanje ako je "Vrijeme do" ranije ili jednako "Vremenu od", uz prikaz poruke: "Krajnje vrijeme mora biti nakon početnog".
- Sistem mora omogućiti unos do dva odvojena termina dnevno (npr. za objekte koji imaju pauzu, tipa 08:00-12:00 i 14:00-18:00).
- Kada se unose dva termina, sistem mora validirati da se oni ne preklapaju.
- Sistem mora imati predefinisano polje "24/7 dostupnost" (checkbox); ako je označeno, tada polja za vrijeme postaju neaktivna, a sistem tretira sandučić kao stalno dostupan.
- Kada algoritam za rute izračuna da poštar stiže izvan definisanog okvira, tada sistem mora označiti tu tačku na ruti crvenom bojom i prikazati upozorenje administratoru prije finalnog slanja rute poštaru.
- Sistem ne smije dozvoliti unos nepostojećeg vremena (npr. 25:61).

---

##### ID storyja: US-33
**Naziv storyja:** Definisanje radnih dana sandučića  
**Opis:** Kao **administrator**, želim **označiti dane u sedmici kada je sandučić dostupan**, kako bi **sistem isključio te lokacije iz ruta tokom neradnih dana**.  
**Poslovna vrijednost:** Sprječava greške u planiranju obilazaka vikendom ili specifičnim danima kada sandučići nisu u funkciji.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Defaultna postavka za nove sandučiće je radna sedmica (Pon-Pet).

**Veze sa drugim storyjima:**
- **Zavisi od:** US-15 (Pregled liste sandučića).

#### Acceptance criteria

- Kada administrator otvori formu za sandučić, tada sistem mora prikazati sekciju "Radni dani" sa sedam checkbox kontrola (Ponedjeljak – Nedjelja).
- Sistem mora pri kreiranju novog sandučića automatski označiti dane od Ponedjeljka do Petka, dok Subota i Nedjelja moraju biti inicijalno odznačeni.
- Sistem ne smije dozvoliti spašavanje sandučića ako nijedan dan nije označen; u tom slučaju mora prikazati poruku: "Sandučić mora imati barem jedan definisan radni dan".
- Kada administrator označi ili odznači dan, tada sistem mora momentalno ažurirati bazu podataka tako da se promjena uzme u obzir pri sljedećem generisanju rute.
- Sistem mora omogućiti opciju "Označi sve / Odznači sve" radi bržeg unosa podataka.
- Kada algoritam generiše rutu za npr. Subotu, ako sandučić nema označenu subotu kao radni dan, tada taj sandučić mora biti potpuno izostavljen sa mape obilaska, bez obzira na njegov prioritet.

---

## PBI-022 Generisanje dnevne rute

#### User Stories
- **US-22:** Kao administrator ili dispečer, želim pokrenuti algoritam za automatsko generisanje dnevne rute za odabranog poštara, kako bih dobio prijedlog obilaska zasnovan na lokacijama i prioritetima sandučića.

#### Poslovna vrijednost
Ovo je srce sistema. Automatizacija rute smanjuje manuelni rad administratora i dispečera, štedi vrijeme i osigurava da ključne lokacije ne budu zaboravljene.

#### Prioritet: High

---

##### ID storyja: US-22
**Naziv storyja:** Automatizovani proračun dnevne rute  
**Opis:** Kao **administrator ili dispečer**, želim **klikom na dugme "Generiši" aktivirati algoritam**, koji će **na osnovu GPS koordinata i prioriteta sandučića kreirati prijedlog dnevne rute za odabranog poštara**.  
**Poslovna vrijednost:** Eliminacija manuelnog planiranja i smanjenje ljudske greške.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Sistem ima pristup koordinatama i prioritetima svih relevantnih sandučića.
- *Otvoreno pitanje:* Koji algoritam koristiti u MVP-u s obzirom na broj tačaka i performanse sistema?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-14 i US-18.
- **Osnova za:** US-23, US-24 i US-25.

#### Acceptance criteria

- Kada administrator ili dispečer klikne na dugme Generiši, tada sistem mora u obzir uzeti isključivo sandučiće koji su: Aktivni (US-13), imaju označen današnji radni dan (US-33) i čiji se vremenski okvir dostupnosti (US-32) podudara sa planiranim vremenom obilaska.
- Sistem mora primijeniti prioritetno ponderisanje tako da sandučići sa statusom Visok prioritet (US-18) imaju prednost u redoslijedu obilaska u odnosu na one sa nižim prioritetom.
- Kada se proces proračuna završi, tada sistem mora prikazati vizuelni prijedlog rute na interaktivnoj mapi (povezana linija između pinova) i hronološku listu adresa sa procijenjenim vremenom dolaska za svaku tačku.
- Sistem mora izvršiti proračun unutar maksimalno 5 sekundi za rute do 50 tačaka; u suprotnom, mora prikazati indikator učitavanja (loader).
- Kada algoritam izračuna da ukupno trajanje rute premašuje 8 sati rada, tada sistem mora prikazati narandžastu toast obavijest: Upozorenje: Ruta premašuje standardno radno vrijeme.
- Sistem mora za MVP verziju koristiti algoritam zasnovan na Euklidskoj udaljenosti $$d = \sqrt{(x_2-x_1)^2 + (y_2-y_1)^2}$$ kako bi osigurali brzinu proračuna.
- Kada u sistemu nema dostupnih sandučića za odabrane parametre, tada sistem mora onemogućiti dugme Generiši i prikazati poruku: Nema dostupnih lokacija za generisanje rute.