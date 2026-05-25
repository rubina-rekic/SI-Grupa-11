# Sprint 9 Backlog

**Sprint cilj:** Operacionalizovati sistem na terenu — omogućiti poštaru pristup dodijeljenoj ruti putem responzivnog mobilnog prikaza i ažuriranje statusa sandučića tokom obilaska, uz istovremeni real-time uvid dispečera u napredak rute i generisanje osnovnog dnevnog izvještaja, čime se zatvara puna operativna petlja od planiranja do realizacije.

## Tabela sprint backloga

| ID | Naziv stavke / zadatka | Odgovorna osoba | Procjena | Status | Napomena |
|---|---|---|---|---|---|
| PBI-026 / US-26 | Mobilni prikaz dodijeljene rute | Rubina, Nejla | 5h 30min | Done | Responzivni web prikaz rute za poštara; hronološka lista sandučića, interaktivna mapa, status badge za svaki sandučić, zaštita pristupa po ulozi |
| PBI-027 / US-27 | Ažuriranje statusa sandučića | Ibrahim, Emrah | 4h | Done | Dugmad za promjenu statusa (Napunjen, Ispraznjen) unutar mobilnog prikaza, evidentiranje timestampa i autora, real-time ažuriranje |
| PBI-028 / US-28 | Označavanje nedostupne lokacije | Faruk | 2h 30min | To Do | Dugme "Nedostupno" s obaveznim razlogom (padajući meni), notifikacija dispečeru, vizuelno označavanje preskočene tačke na mapi |
| PBI-029 / US-29 | Praćenje statusa rute od strane dispečera | Kerim, Aldin | 5h | Done | Dashboard za dispečera s real-time pregledom statusa po ruti i sandučiću, filtri po statusu, naglašavanje problematičnih lokacija |
| PBI-030 / US-30 | Osnovni dnevni izvještaj | Nejla, Ibrahim | 4h | Done | Generisanje izvještaja za odabrani datum i poštara u dashboardu "Praćenje ruta"; agregacija obrađenih, neposjećenih i nedostupnih sandučića, upozorenje ispod 80% i export u PDF |

---

## PBI-026 Mobilni prikaz dodijeljene rute

#### User Stories
- **US-26:** Kao poštar, želim vidjeti svoju dodijeljenu dnevnu rutu putem responzivnog web interfejsa, kako bih na terenu znao koji su sandučići na mom planu i kojim redoslijedom ih treba obići.

#### Poslovna vrijednost
Spaja dispečerovo planiranje sa terenskim izvršenjem — poštar ne mora čekati papirni nalog niti kontaktirati dispečera, već direktno iz telefona vidi šta treba uraditi i kojim redom.

#### Prioritet: High

---

##### ID storyja: US-26
**Naziv storyja:** Mobilni prikaz dodijeljene rute  
**Opis:** Kao **poštar**, želim **otvoriti svoju dodijeljenu rutu na mobilnom uređaju**, koja prikazuje **interaktivnu mapu s pinovima i hronološku listu sandučića s procijenjenim vremenima dolaska**, kako bih **mogao efikasno obilaziti lokacije bez papirnog naloga**.  
**Poslovna vrijednost:** Digitalizacija terenskog rada poštara i eliminacija papirnih naloga.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Poštar ima pametni telefon s pristupom internetu i korisničkim računom u sistemu.
- *Pretpostavka:* Ruta je prethodno dodijeljena poštaru od strane dispečera (US-23) i ima status "Dodijeljena" ili "U toku".
- *Otvoreno pitanje:* Da li poštar treba imati mogućnost kontaktiranja dispečera direktno iz aplikacije?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-23 (Dodjela rute poštaru) — prikaz rute dostupan je isključivo za rute dodijeljene poštaru.
- **Osnova za:** US-27 (Ažuriranje statusa sandučića), US-28 (Označavanje nedostupne lokacije).
- **Povezano sa:** US-24 (Pregled detalja rute) — poštar vidi iste podatke kao dispečer, ali u mobilno optimizovanom prikazu.

#### Acceptance criteria

- Kada poštar pristupi sistemu s ulogom "Poštar", tada sistem mora prikazati njegovu dodijeljenu rutu za tekući dan ako postoji ruta u statusu "Dodijeljena" ili "U toku".
- Sistem mora prikazati hronološku listu sandučića sa sljedećim podacima za svaki unos: redni broj obilaska, adresa lokacije, prioritet (vizuelno kodiran bojom: crvena — Visok, žuta — Srednji, zelena — Nizak), procijenjeno vrijeme dolaska i trenutni status obilaska (Čeka / Obrađen / Nedostupan).
- Sistem mora prikazati interaktivnu mapu s numeriranim pinovima za svaki sandučić i linijom rute koja ih spaja redoslijedom obilaska.
- Prikaz mora biti optimizovan za mobilne uređaje — dovoljno velik tekst, dugmad prilagođena dodirom, bez horizontalnog skrolanja.
- Sistem mora zaštititi pristup — poštar smije vidjeti isključivo rute koje su mu dodijeljene; pokušaj pristupa tuđoj ruti mora rezultirati porukom: "Nemate ovlaštenje za pristup ovoj ruti."
- Kada poštar nema dodijeljenu rutu za tekući dan, sistem mora prikazati poruku: "Nema dodijeljene rute za danas."
- Sistem mora prikazati sumarni blok na vrhu s: ukupnim brojem sandučića, brojem obrađenih, brojem preskočenih i procijenjenim preostalim vremenom obilaska.
- Kada status rute pređe u "U toku" (prvi sandučić označen kao obrađen), sistem mora to evidentirati u bazi s tačnim timestampom.

---

## PBI-027 Ažuriranje statusa sandučića

#### User Stories
- **US-27:** Kao poštar, želim promijeniti status sandučića tokom obilaska, kako bi sistem evidentirano bilježio napredak i dispečer mogao pratiti realizaciju rute u realnom vremenu.

#### Poslovna vrijednost
Digitalizira terenski rad — svaki obavljeni posao se automatski bilježi, eliminišući naknadnu papirnu evidenciju i grešku pri ručnom prepisivanju rezultata obilaska.

#### Prioritet: High

---

##### ID storyja: US-27
**Naziv storyja:** Ažuriranje statusa sandučića  
**Opis:** Kao **poštar**, želim **klikom na odgovarajuće dugme označiti sandučić kao obrađen (ispraznjen ili napunjen)**, kako bi **sistem evidentirao ovaj podatak s vremenskim pečatom i dispečer vidio napredak u realnom vremenu**.  
**Poslovna vrijednost:** Automatsko vođenje evidencije terenskih aktivnosti smanjuje administraciju i povećava transparentnost prema dispečeru i administratoru.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Poštar je na terenu i pristupa sistemu putem mobilnog uređaja.
- *Pretpostavka:* Ruta je u statusu "U toku".
- *Otvoreno pitanje:* Da li poštar treba imati mogućnost poništavanja pogrešno postavljenog statusa?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-26 (Mobilni prikaz dodijeljene rute) — ažuriranje statusa dostupno je isključivo unutar mobilnog prikaza.
- **Utiče na:** US-28 (Označavanje nedostupne lokacije), US-29 (Praćenje statusa rute od strane dispečera), US-30 (Osnovni dnevni izvještaj).

#### Acceptance criteria

- Sistem mora pored svakog sandučića u hronološkoj listi (US-26) prikazati dugme **"Označi kao obrađen"** koje postaje aktivno tek kada poštar stigne na redoslijedno ispravnu lokaciju.
- Kada poštar klikne na "Označi kao obrađen", tada sistem mora: promijeniti vizuelni status sandučića u "Obrađen" (zelena kvačica), evidentirati tačno datum i vrijeme akcije, evidentirati ID poštara koji je napravio promjenu i odmah ažurirati prikaz bez ponovnog učitavanja stranice.
- Sistem mora podržavati dva tipa statusa obrađenosti: **Ispraznjen** (sandučić je bio pun, poštar ga je ispraznio) i **Napunjen** (sandučić je prazan, poštar je ubacio poštu); poštar odabire odgovarajući tip pri označavanju.
- Sistem mora onemogućiti promjenu statusa sandučića koji je već označen kao "Obrađen" bez odobravajuće akcije dispečera — u tom slučaju prikazati poruku: "Status je već evidentiran. Kontaktirajte dispečera za ispravku."
- Kada je sandučić označen kao obrađen, tada odgovarajući pin na mapi mora promijeniti boju u zelenu kako bi poštar imao vizuelni pregled napretka.
- Kada poštar označi posljednji sandučić kao obrađen, sistem mora automatski promijeniti status rute u **"Završena"** i evidentirati tačno vrijeme završetka.
- Sve promjene statusa moraju biti pohranjene u bazi s vremenskim pečatom i dostupne dispečeru u realnom vremenu (US-29).

---

## PBI-028 Označavanje nedostupne lokacije

#### User Stories
- **US-28:** Kao poštar, želim evidentirati da određena lokacija nije bila dostupna tokom obilaska, kako bi dispečer bio odmah obaviješten i mogao preduzeti odgovarajuće mjere.

#### Poslovna vrijednost
Smanjuje operativne gubitke — dispečer pravovremeno zna o problematičnim lokacijama i može reagovati (prerasporediti, kontaktirati, evidentirati za sljedeći dan) bez čekanja na kraj smjene.

#### Prioritet: Low

---

##### ID storyja: US-28
**Naziv storyja:** Označavanje nedostupne lokacije  
**Opis:** Kao **poštar**, želim **označiti sandučić kao nedostupan uz navođenje razloga**, kako bi **dispečer bio obaviješten o problemu i mogao poduzeti odgovarajuće mjere**.  
**Poslovna vrijednost:** Pravovremena informacija o terenskim problemima omogućava dispečeru brzu reakciju i poboljšava planiranje za naredne rute.  
**Prioritet:** Low  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Poštar je fizički pokušao pristupiti lokaciji, ali to nije bilo moguće.
- *Otvoreno pitanje:* Da li sistem treba automatski predložiti isto sandučić za narednu rutu istog poštara?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-26 (Mobilni prikaz dodijeljene rute), US-27 (Ažuriranje statusa sandučića).
- **Utiče na:** US-29 (Praćenje statusa rute od strane dispečera), US-30 (Osnovni dnevni izvještaj).

#### Acceptance criteria

- Sistem mora pored svakog sandučića u hronološkoj listi (US-26) prikazati dugme **"Nedostupno"** kao alternativnu akciju umjesto "Označi kao obrađen".
- Kada poštar klikne na "Nedostupno", tada sistem mora prikazati padajući meni s predefinisanim razlozima: "Zaključan pristup", "Sandučić oštećen", "Privatni posjed nedostupan", "Prirodna prepreka" i "Ostalo (unesi napomenu)".
- Kada je odabran razlog "Ostalo", sistem mora prikazati tekstualno polje za slobodan unos napomene (maks. 200 znakova).
- Sistem ne smije dozvoliti označavanje lokacije kao nedostupne bez odabranog razloga — dugme za potvrdu mora biti onemogućeno dok razlog nije odabran.
- Kada poštar potvrdi nedostupnost, tada sistem mora: promijeniti vizuelni status sandučića u "Nedostupan" (crveni X), evidentirati razlog, timestamp i ID poštara te odmah poslati obavijest dispečeru: "Poštar [Ime Prezime] je označio sandučić [Adresa] kao nedostupan — razlog: [razlog]."
- Odgovarajući pin na mapi mora promijeniti boju u crvenu radi vizuelnog razlikovanja od obrađenih (zelenih) i preostalih (sivih) lokacija.
- Jednom označena nedostupna lokacija mora biti uključena u dnevni izvještaj (US-30) pod zasebnom sekcijom.

---

## PBI-029 Praćenje statusa rute od strane dispečera

#### User Stories
- **US-29:** Kao dispečer, želim u realnom vremenu pratiti napredak aktivnih ruta i vidjeti koji su sandučići obrađeni, koji su preskočeni i koji su evidentirani kao problematični, kako bih mogao pravovremeno reagovati na terenska odstupanja.

#### Poslovna vrijednost
Centralizovana kontrolna tabla za dispečera transformiše reaktivno upravljanje u proaktivno — problemi se otkrivaju čim nastanu, a ne tek na kraju smjene kada je kasno za intervenciju.

#### Prioritet: High

---

##### ID storyja: US-29
**Naziv storyja:** Praćenje statusa rute od strane dispečera  
**Opis:** Kao **dispečer**, želim **u realnom vremenu vidjeti status svake aktivne rute i pojedinih sandučića**, kako bih **mogao identificovati probleme, pratiti napredak poštara i po potrebi intervenisati**.  
**Poslovna vrijednost:** Povećava operativnu efikasnost i smanjuje potrebu za telefonskom koordinacijom između dispečera i poštara.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Poštar redovno ažurira statusove sandučića tokom obilaska (US-27, US-28).
- *Otvoreno pitanje:* Da li dispečer treba imati mogućnost direktnog kontaktiranja poštara unutar sistema?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-27 (Ažuriranje statusa sandučića), US-28 (Označavanje nedostupne lokacije).
- **Osnova za:** US-30 (Osnovni dnevni izvještaj).
- **Povezano sa:** US-24 (Pregled detalja rute) — praćenje se nadovezuje na prikaz detalja rute.

#### Acceptance criteria

- Sistem mora dispečeru prikazati pregled svih aktivnih ruta za tekući dan s agregiranim statusom svake rute: broj obrađenih sandučića, broj preskočenih, broj nedostupnih i ukupni broj u ruti.
- Za svaku rutu mora biti vidljivo: ime poštara, datum rute, trenutni status rute (Dodijeljena / U toku / Završena) i postotak napretka (npr. "12/20 sandučića obrađeno — 60%").
- Kada dispečer klikne na određenu rutu, sistem mora prikazati detaljni prikaz sa statusom svakog pojedinog sandučića, vremenskim pečatom posljednje akcije i navedenim razlogom za nedostupne lokacije.
- Sistem mora vizuelno istaknuti problematične rute — ruta koja ima barem jednu nedostupnu lokaciju mora biti označena narandžastom ikonom upozorenja u pregledu svih ruta.
- Sistem mora prikazati notifikacije u realnom vremenu kada poštar označi sandučić kao nedostupan (US-28) — notifikacija mora biti vidljiva dispečeru bez osvježavanja stranice.
- Dispečer mora imati mogućnost filtriranja prikaza po: statusu rute (U toku / Završena / Dodijeljena), imenu poštara i datumu.
- Kada nema aktivnih ruta za tekući dan, sistem mora prikazati poruku: "Nema aktivnih ruta za danas."
- Pregled praćenja mora biti dostupan isključivo korisnicima s ulogom dispečer ili administrator (US-14).

---

## PBI-030 Osnovni dnevni izvještaj

#### User Stories
- **US-30:** Kao dispečer ili administrator, želim generisati osnovni dnevni izvještaj o realizovanim i nerealizovanim obilascima za odabrani datum i poštara, kako bih imao pregled učinkovitosti terenskog rada.

#### Poslovna vrijednost
Zatvara operativni ciklus sistema — od planiranja rute do pisanog dokaza o njenoj realizaciji. Izvještaj služi kao osnova za ocjenu učinkovitosti, identifikaciju ponavljajućih problema i planiranje budućih ruta.

#### Prioritet: Low

---

##### ID storyja: US-30
**Naziv storyja:** Osnovni dnevni izvještaj  
**Opis:** Kao **dispečer ili administrator**, želim **generisati izvještaj za odabrani datum i poštara**, koji prikazuje **pregled svih sandučića iz rute s njihovim finalnim statusom**, kako bih **imao pisanu evidenciju dnevnih operacija**.  
**Poslovna vrijednost:** Osigurava traceability operativnih aktivnosti i pruža osnovu za analizu efikasnosti terenskog rada.  
**Prioritet:** Low  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Ruta za odabrani datum i poštara je u statusu "Završena" ili "U toku".
- *Otvoreno pitanje:* Da li je potreban automatski dnevni export izvještaja u dogovoreno odredište (email, folder)?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-27 (Ažuriranje statusa sandučića), US-28 (Označavanje nedostupne lokacije), US-29 (Praćenje statusa rute od strane dispečera).
- **Osnova za:** PBI-049 (Historija obilazaka i arhiva ruta) i PBI-050 (Prošireno operativno izvještavanje) u Sprintu 10.

#### Acceptance criteria

- Sistem mora dispečeru i administratoru omogućiti odabir datuma i poštara za koji se želi generisati izvještaj.
- Kada korisnik klikne "Generiši izvještaj", tada sistem mora prikazati izvještaj koji sadrži: zaglavlje (datum, ime poštara, naziv rute, status rute), sumarni blok (ukupno sandučića, obrađenih, nedostupnih, % realizacije) i detaljnu tabelu sandučića.
- Detaljna tabela mora sadržavati za svaki sandučić: redni broj, adresu, prioritet, finalni status (Obrađen / Nedostupan / Nije posjećen), razlog nedostupnosti (ako postoji) i timestamp akcije.
- Sistem mora vizuelno razlikovati obrađene (zelene), nedostupne (crvene) i neposjećene (sive) unose u tabeli.
- Kada ukupan postotak realizacije padne ispod 80%, sistem mora prikazati narandžasto upozorenje: "Upozorenje: Realizacija rute ispod standardnog praga (80%)."
- Sistem mora omogućiti export generisanog izvještaja u PDF format klikom na dugme "Preuzmi PDF".
- Kada za odabrani datum i poštara ne postoji ruta, sistem mora prikazati poruku: "Nema podataka za odabrane parametre."
- Pristup generisanju izvještaja mora biti ograničen na korisnike s ulogom dispečer ili administrator (US-14).
