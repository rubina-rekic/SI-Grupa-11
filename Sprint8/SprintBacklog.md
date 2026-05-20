# Sprint 8 Backlog

**Sprint cilj:** Implementirati kompletno upravljanje rutama — automatski generisati optimiziranu dnevnu rutu na osnovu GPS koordinata i prioriteta sandučića, dodijeliti je konkretnom poštaru te osigurati dispečeru pregled i mogućnost ručnog prilagođavanja redoslijeda obilaska, kako bi sistem bio spreman za operativnu upotrebu na terenu.

## Tabela sprint backloga

| ID | Naziv stavke / zadatka | Odgovorna osoba | Procjena | Status | Napomena |
|---|---|---|---|---|---|
| PBI-022 / US-22 | Generisanje dnevne rute | Emrah, Kerim | 8h | Done | Na osnovu GPS koordinata i prioriteta sandučića kreirati prijedlog dnevne rute; Euklidska udaljenost, prioritetno ponderisanje, loader za duže proračune |
| PBI-023 / US-23 | Dodjela rute poštaru | Ibrahim, Faruk | 3h | Done | Odabir poštara iz padajućeg menija, promjena statusa rute u "Dodijeljena", evidencija autora i vremena dodjele |
| PBI-024 / US-24 | Pregled detalja rute | Nejla, Aldin | 3h 30min | Done | Interaktivna mapa s pinovima i linijom rute, hronološka lista sandučića, procijenjeno trajanje, sinhronizacija mapa–lista |
| PBI-025 / US-25 | Ručna izmjena redoslijeda obilaska | Rubina, Emrah | 5h | Done | Dugmad ↑/↓ ili drag-and-drop, ponovna kalkulacija vremena, označavanje izmijenjenih tačaka, resetovanje na originalni redoslijed |

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
- **Zavisi od:** US-14 (Uloge i pristup), US-18 (Postavljanje prioriteta sandučića), US-32 i US-33 (Radna pravila sandučića).
- **Osnova za:** US-23 (Dodjela rute poštaru), US-24 (Pregled detalja rute), US-25 (Ručna izmjena redoslijeda obilaska).

#### Acceptance criteria

- Kada administrator ili dispečer klikne na dugme "Generiši", tada sistem mora u obzir uzeti isključivo sandučiće koji su: aktivni (US-13), imaju označen današnji radni dan (US-33) i čiji se vremenski okvir dostupnosti (US-32) podudara sa planiranim vremenom obilaska.
- Sistem mora primijeniti prioritetno ponderisanje tako da sandučići sa statusom Visok prioritet (US-18) imaju prednost u redoslijedu obilaska u odnosu na one sa nižim prioritetom.
- Kada se proces proračuna završi, tada sistem mora prikazati vizuelni prijedlog rute na interaktivnoj mapi (povezana linija između pinova) i hronološku listu adresa sa procijenjenim vremenom dolaska za svaku tačku.
- Sistem mora izvršiti proračun unutar maksimalno 5 sekundi za rute do 50 tačaka; u suprotnom, mora prikazati indikator učitavanja (loader).
- Kada algoritam izračuna da ukupno trajanje rute premašuje 8 sati rada, tada sistem mora prikazati narandžastu toast obavijest: "Upozorenje: Ruta premašuje standardno radno vrijeme."
- Sistem mora za MVP verziju koristiti algoritam zasnovan na Euklidskoj udaljenosti $d = \sqrt{(x_2-x_1)^2 + (y_2-y_1)^2}$ kako bi osigurali brzinu proračuna.
- Kada u sistemu nema dostupnih sandučića za odabrane parametre, tada sistem mora onemogućiti dugme "Generiši" i prikazati poruku: "Nema dostupnih lokacija za generisanje rute."

---

## PBI-023 Dodjela rute poštaru

#### User Stories
- **US-23:** Kao dispečer, želim dodijeliti generisanu rutu konkretnom poštaru, kako bi poštar znao koje sandučiće treba obići tog dana.

#### Poslovna vrijednost
Dodjela rute zatvara petlju između planiranja i izvršenja — generisani prijedlog postaje operativni nalog koji poštar može primiti i preuzeti na terenu.

#### Prioritet: High

---

##### ID storyja: US-23
**Naziv storyja:** Dodjela rute poštaru  
**Opis:** Kao **dispečer**, želim **odabrati aktivnog poštara i dodijeliti mu generisanu rutu**, kako bi **sistem evidentirao dodjelu i poštar mogao pristupiti svojoj ruti**.  
**Poslovna vrijednost:** Operacionalizacija planiranja — ruta prelazi iz statusa prijedloga u konkretan zadatak za poštara.  
**Prioritet:** High  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Postoji najmanje jedna generisana ruta u statusu "Prijedlog" i bar jedan aktivan poštar u sistemu.
- *Pretpostavka:* Svaki poštar može imati najviše jednu aktivnu rutu po datumu.
- *Otvoreno pitanje:* Da li dispečer treba primiti potvrdu od poštara da je preuzeo rutu?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-22 (Generisanje dnevne rute) — dodjela je moguća samo za postoje u statusu "Prijedlog".
- **Utiče na:** US-24 (Pregled detalja rute), US-25 (Ručna izmjena redoslijeda), US-26 (Mobilni prikaz dodijeljene rute).

#### Acceptance criteria

- Kada dispečer otvori prijedlog generisane rute, tada sistem mora prikazati dugme **"Dodijeli poštaru"**.
- Sistem mora prikazati padajući meni sa listom svih aktivnih poštara koji nemaju dodijeljenu rutu za odabrani datum.
- Kada dispečer odabere poštara i klikne na dugme za potvrdu, tada sistem mora promijeniti status rute iz "Prijedlog" u **"Dodijeljena"** i evidentirati datum, vrijeme i ime dispečera koji je napravio dodjelu.
- Sistem mora prikazati obavijest o uspješnoj dodjeli: "Ruta je uspješno dodijeljena poštaru **[Ime Prezime]**."
- Sistem ne smije dozvoliti dodjelu iste rute dvoma poštarima za isti datum — ako je poštar već ima aktivnu rutu, njegovo ime mora biti onemogućeno u padajućem meniju uz tooltip: "Poštar već ima dodijeljenu rutu za ovaj datum."
- Dispečer mora imati mogućnost preraspodjele rute (promjene poštara) sve dok poštar nije počeo obilazak (status rute nije "U toku").
- Jednom dodijeljena ruta mora biti dostupna poštaru u njegovom mobilnom prikazu (US-26).
- Kada na listi nema nijednog slobodnog poštara, sistem mora prikazati poruku: "Nema dostupnih poštara za odabrani datum."

---

## PBI-024 Pregled detalja rute

#### User Stories
- **US-24:** Kao dispečer ili administrator, želim pregledati kompletan redoslijed obilaska, uključene sandučiće i osnovne detalje rute, kako bih imao potpun uvid u plan obilaska prije i nakon dodjele.

#### Poslovna vrijednost
Transparentnost planiranja — dispečer može verificirati ispravnost generisane rute, uočiti potencijalne probleme i donijeti operativne odluke bez otvaranja drugih dijelova sistema.

#### Prioritet: Medium

---

##### ID storyja: US-24
**Naziv storyja:** Pregled detalja rute  
**Opis:** Kao **dispečer ili administrator**, želim **otvoriti stranicu s detaljnim prikazom rute**, koja prikazuje **interaktivnu mapu s tačkama obilaska i hronološku listu sandučića s procijenjenim vremenima**, kako bih **mogao vizuelno provjeriti plan obilaska i identifikovati eventualne probleme**.  
**Poslovna vrijednost:** Smanjuje operativne greške — dispečer vidi rutu onako kako će je poštar obilaziti, pa može ispraviti probleme prije nego što ruta bude poslana na teren.  
**Prioritet:** Medium  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Ruta sadrži bar jednu tačku obilaska.
- *Pretpostavka:* GPS koordinate svih sandučića u ruti su dostupne i validne.
- *Otvoreno pitanje:* Da li pregled detalja treba biti dostupan i poštaru, ili isključivo dispečeru i administratoru?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-22 (Generisanje dnevne rute) — detalji pretpostavljaju postoje generisane rute.
- **Utiče na:** US-25 (Ručna izmjena redoslijeda obilaska) — izmjena se pokreće iz prikaza detalja.
- **Povezano sa:** US-23 (Dodjela rute poštaru) — status rute vidljiv je u pregledu detalja.

#### Acceptance criteria

- Sistem mora prikazati detalje rute na stranici koja sadrži: interaktivnu mapu s numeriranim pinovima za svaki sandučić i linijom koja ih spaja u redoslijedu obilaska, i hronološku listu sandučića ispod mape.
- Za svaki sandučić u hronološkoj listi mora biti prikazano: redni broj obilaska, naziv/adresa lokacije, prioritet (vizuelno kodiran bojom: crvena — Visok, žuta — Srednji, zelena — Nizak) i procijenjeno vrijeme dolaska.
- Sistem mora prikazati sumarni blok na vrhu stranice sa: ukupnim brojem sandučića u ruti, ukupnim procijenjenim trajanjem obilaska i datumom za koji je ruta generisana.
- Kada ukupno trajanje rute premašuje 8 radnih sati, sumarni blok mora istaknuti tu vrijednost narandžastom bojom uz napomenu: "Ruta premašuje standardno radno vrijeme."
- Sistem mora sinhronizovati prikaz mape i liste — klik na sandučić u listi mora istaknuti odgovarajući pin na mapi, i obrnuto.
- Status rute (Prijedlog / Dodijeljena / U toku / Završena) i ime dodijeljenog poštara (ako je dodjela obavljena) moraju biti vidljivi na vrhu stranice.
- Kada ruta ne sadrži nijednu tačku obilaska, sistem mora prikazati poruku: "Ruta ne sadrži sandučiće."
- Pregled detalja mora biti dostupan isključivo korisnicima s ulogom administrator ili dispečer (US-14).

---

## PBI-025 Ručna izmjena redoslijeda obilaska

#### User Stories
- **US-25:** Kao dispečer, želim ručno prilagoditi redoslijed obilaska sandučića unutar rute, kako bih mogao ispraviti ili optimizirati automatski generisani prijedlog prema operativnim potrebama.

#### Poslovna vrijednost
Algoritam daje optimalan prijedlog, ali dispečer poznaje terenske specifičnosti koje sistem ne može predvidjeti (radovi, gužve, posebni zahtjevi). Ručno prilagođavanje osigurava da ruta bude i teorijski optimalna i praktično izvodljiva.

#### Prioritet: Medium

---

##### ID storyja: US-25
**Naziv storyja:** Ručna izmjena redoslijeda obilaska  
**Opis:** Kao **dispečer**, želim **pomjerati sandučiće gore ili dolje u listi obilaska unutar prikaza detalja rute**, kako bi **redoslijed obilaska odražavao operativne prioritete i terenske specifičnosti koje algoritam nije uzeo u obzir**.  
**Poslovna vrijednost:** Povećava prihvatljivost sistema na terenu — dispečer ne mora slijepo prihvatiti automatski prijedlog, već ga može prilagoditi bez ponovnog pokretanja generisanja.  
**Prioritet:** Medium  
**Pretpostavke i otvorena pitanja:**
- *Pretpostavka:* Ruta mora biti u statusu "Prijedlog" ili "Dodijeljena" da bi izmjena bila moguća.
- *Pretpostavka:* Ponovna kalkulacija vremena dolaska vrši se isključivo na osnovu Euklidske udaljenosti, isto kao pri generisanju (US-22).
- *Otvoreno pitanje:* Da li je potrebna mogućnost dodavanja ili uklanjanja sandučića iz rute, ili isključivo izmjena redoslijeda?

**Veze sa drugim storyjima:**
- **Zavisi od:** US-24 (Pregled detalja rute) — izmjena redoslijeda dostupna je iz prikaza detalja rute.
- **Utiče na:** US-26 (Mobilni prikaz dodijeljene rute) — poštar vidi finalni redoslijed koji uključuje i ručne izmjene.
- **Povezano sa:** US-22 (Generisanje dnevne rute) — izmjenom se nadograđuje, a ne zamjenjuje, automatski generisani prijedlog.

#### Acceptance criteria

- Sistem mora u prikazu detalja rute (US-24) prikazati dugmad **↑** i **↓** pored svakog sandučića u hronološkoj listi, kojima dispečer može pomjeriti sandučić jedno mjesto prema gore ili dolje.
- Kada dispečer pomjeri sandučić, sistem mora automatski ponovo izračunati procijenjeno vrijeme dolaska za sve sandučiće koji slijede iza izmijenjene pozicije i odmah ažurirati prikaz bez ponovnog učitavanja stranice.
- Sistem mora vizuelno označiti svaki ručno premješteni sandučić (npr. ikonom olovke ✎) kako bi dispečer mogao razlikovati automatski generisani redoslijed od ručno izmijenjenog.
- Sistem mora prikazati dugme **"Resetuj na originalni redoslijed"** koje vraća raspored na stanje koje je algoritam originalno generisao; sve ručne izmjene moraju biti poništene.
- Kada dispečer klikne na "Sačuvaj izmjene", sistem mora trajno pohraniti novi redoslijed i prikazati poruku: "Izmjene redoslijeda su uspješno sačuvane."
- Sistem mora onemogućiti dugmad ↑ i ↓ i prikazati obavijest: "Izmjena redoslijeda nije dostupna za rute u toku ili završene rute." kada je status rute "U toku" ili "Završena".
- Sve izmjene redoslijeda moraju biti evidentirane u bazi s vremenskim pečatom i imenom dispečera koji je napravio izmjenu, za potrebe revizije.
- Linija rute na interaktivnoj mapi mora se ažurirati u realnom vremenu kako dispečer mijenja redoslijed, odražavajući novi put obilaska.
