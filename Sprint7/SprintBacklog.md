# Sprint 7 Backlog

**Sprint cilj:** Definisati logičke parametre sistema kroz određivanje važnosti sandučića i njihovih vremenskih ograničenja — implementirati prioritete sandučića i evidenciju radnih pravila kako bi sistem imao sve potrebne podatke za generisanje operativno izvodivih ruta.

## Tabela sprint backloga

| ID | Naziv stavke / zadatka | Odgovorna osoba | Procjena | Status | Napomena |
|---|---|---|---|---|---|
| PBI-020 / US-18 | Postavljanje prioriteta sandučića | Rubina | 3h 30min | Done | Dropdown Visok/Srednji/Nizak, kodiranje bojama, automatski prioritet, obrazloženje promjene, inicijalna vrijednost Srednji |
| PBI-021 / US-32 | Definisanje vremenskih okvira dostupnosti sandučića | -| 4h | Done | Time picker 24h format, validacija vremena, dva termina dnevno, checkbox 24/7, upozorenje pri konfliktu s rutom |
| PBI-021 / US-33 | Definisanje radnih dana sandučića | - | 2h | To Do | Sedam checkbox kontrola, default Pon-Pet, validacija min. jedan dan, Označi sve / Odznači sve |

---

## PBI-020 Definisanje prioriteta sandučića

#### User Stories
- **US-18:** Kao administrator, želim postaviti ili izmijeniti prioritet za pražnjenje/punjenje sandučića, kako bi sistem znao koje lokacije imaju veći operativni značaj.

#### Poslovna vrijednost
Prioriteti omogućavaju da sistem i dispečer razlikuju kritične od manje kritičnih lokacija, što direktno utiče na kvalitet planiranja i redoslijed obilaska.

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
- **Utiče na:** US-22 (Generisanje dnevne rute).

#### Acceptance criteria

- Kada administrator otvori formu za sandučić (unos ili uređivanje), tada sistem mora ponuditi padajući meni "Prioritet" sa tri fiksne opcije: Visok (High), Srednji (Medium) i Nizak (Low).
- Sistem mora vizuelno označiti nivoe prioriteta u svim tabelama i mapama koristeći kodiranje bojama: Visok — crveni indikator, Srednji — žuti/narandžasti indikator, Nizak — zeleni indikator.
- Sistem mora omogućiti "Automatski prioritet"; ako je ova opcija uključena, tada sistem automatski postavlja prioritet na "Visok" za sve sandučiće tipa "Specijalni/Prioritetni" ili one locirane u zoni centra grada.
- Kada administrator ručno promijeni prioritet, tada sistem mora tražiti kratko obrazloženje koje se čuva u bazi.
- Sistem ne smije dozvoliti da sandučić ostane bez dodijeljenog prioriteta; inicijalna vrijednost pri kreiranju mora biti "Srednji".
- Kada se prioritet promijeni, tada se ta promjena mora odmah odraziti na listu prioriteta za generisanje rute u US-22 — sandučići sa Visokim prioritetom automatski se pomjeraju na vrh liste za obilazak.

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
**Poslovna vrijednost:** Sprečava greške u planiranju obilazaka vikendom ili specifičnim danima kada sandučići nisu u funkciji.  
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