# Završni izvještaj o radu tima

**Projekat:** Sistem za optimizaciju ruta punjenja i pražnjenja poštanskih sandučića — **PostRoute**  
**Tim:** Grupa 11  
**Trajanje projekta:** 10 razvojnih sprintova + završni sprint 
**Produkcija:** [https://postrouteapp.netlify.app](https://postrouteapp.netlify.app)

---

## 1. Svrha projekta

Sistem **PostRoute** razvijen je s primarnim ciljem digitalne transformacije, automatizacije i optimizacije procesa pražnjenja i punjenja mreže poštanskih sandučića. U tradicionalnim poštanskim sistemima, ovaj proces se primarno oslanja na rigidne, statične rute i subjektivno iskustvo terenskih radnika, što uzrokuje visoke operativne troškove, neefikasno trošenje vremena i resursa, te potpunu nemogućnost brze reakcije na promjene na terenu. PostRoute zamjenjuje ovaj zastarjeli i manuelni pristup inteligentnim softverskim rješenjem koje maksimizira efikasnost cjelokupne logističke mreže primjenom algoritama optimizacije.

Svrha projekta se može detaljnije raščlaniti na nekoliko ključnih dimenzija:

### 1.1. Optimizacija resursa i operativna efikasnost
Glavna svrha sistema je omogućiti dispečerima alate za inteligentno planiranje obilazaka. Umjesto da poštari svakodnevno obilaze fiksne lokacije bez obzira na njihovo stvarno stanje, PostRoute koristi matematičke modele kako bi izračunao matematički najpovoljniji redoslijed obilaska sandučića. Pri tome, algoritam ne računa samo čistu geografsku distancu, već dinamički ponderiše rute uzimajući u obzir:
* **Prioritete sandučića:** Sandučići visokog prioriteta imaju prednost pri odabiru sljedeće tačke unutar optimizacijske petlje.
* **Radna pravila i vremenske prozore:** Sistem striktno poštuje definisane vremenske intervale i radne dane u sedmici unutar kojih se obilazak mora izvršiti, sprečavajući situacije u kojima poštar stiže na lokaciju izvan radnog vremena objekta ili u periodu saobraćajnog špica.

### 1.2. Povećanje transparentnosti i real-time telemetrija
Jedan od ključnih ciljeva projekta je premostiti informacijski jaz između dispečerskog centra i radnika na terenu. Kroz implementaciju dispečerskog dashboarda koji koristi efikasan auto-refresh mehanizam (HTTP polling postavljen na 30 sekundi), sistem pruža trenutni uvid u operativni status terena. Svrha je omogućiti dispečeru da u svakom trenutku vidi tačan progres rute, vremenske oznake (*timestamps*) obrade svakog pojedinačnog sandučića, kao i vizuelno naglašene kritične probleme. Time se postiže potpuna operativna transparentnost i omogućava proaktivno donošenje odluka.

### 1.3. Unapređenje terenskog rada (Mobilni ekosistem)
Sistem ima za svrhu pružiti poštarima na terenu jednostavno, intuitivno i *mobile-first* programsko rješenje koje minimizira administrativni i kognitivni teret tokom vožnje ili hoda. Kroz responzivni web interfejs poštar dobija jasan linijski i kartografski prikaz svoje smjene. Svrha ovog interfejsa je da u par klikova omogući precizno elegantno evidentiranje stanja na terenu i, što je najvažnije, brzo prijavljivanje anomalija (npr. oštećen sandučić, neprohodan put) kroz ugrađeni audit log.

### 1.4. Akademski i metodološki ciljevi tima
Pored očigledne poslovne i logističke vrijednosti samog softverskog proizvoda, PostRoute ima izuzetno važnu akademsku i pedagošku svrhu. Razvijen od strane sedam članova tima (Grupa 11) u okviru jednog semestra, projekat je služio kao platforma za praktičnu primjenu i ovladavanje agilnim metodologijama u realističnom timskom okruženju.

Svrha u ovom kontekstu obuhvatala je:
* **Rotaciju uloga i timsku agilnost:** Članovi tima nisu imali strogo fiksirane pozicije, već su se kroz sprintove fleksibilno rotirali između razvoja backenda (.NET 9), frontenda (React 19 + TypeScript) i osiguranja kvaliteta (QA testiranje), čime je postignuta visoka unutar-timska kohezija.
* **Inženjersku disciplinu i CI/CD standarde:** Uspostavljanje *GitLab Flow* branching strategije i automatizovanog *GitHub Actions* pipeline-a osiguralo je da se svaki komad koda rigorozno i automatizovano testira prije spajanja na glavne grane.
* **Zrelo i transparentno korištenje AI alata:**  iTm je kroz *AI Usage Log* pratio i kritički analizirao greške koje AI pravi, demonstrirajući visok nivo inženerske zrelosti tima.

---

## 2. Problem koji sistem rješava

Tradicionalni proces upravljanja mrežom poštanskih sandučića i planiranja svakodnevnih obilazaka pati od više logističkih i tehnoloških nedostataka. Tradicionalni model se primarno oslanja na statične planove rada i decenijsko, subjektivno iskustvo pojedinačnih poštara i dispečera. Takav pristup u modernom logističkom okruženju više nije održiv i direktno uzrokuje niz operativnih problema koji su klasifikovani u nastavku.

### 2.1. Detaljna analiza operativnih problema

#### 1. Neefikasno korištenje vremena i ljudskih resursa
U konvencionalnim sistemima, rute su unaprijed definisane i rijetko se mijenjaju, bez obzira na stvarne potrebe terena ili fluktuaciju volumena pošiljki. Poštari troše radne sate vozeći se po fiksnom, suboptimalnom šablonu. To dovodi do neracionalne potrošnje goriva, povećanog habanja službenih vozila i prekovremenih radnih sati, jer rute nisu matematički proračunate da minimiziraju pređeni put.

#### 2. Nepotrebni obilasci i propuštanje kritičnih lokacija
Zbog nedostatka dinamičkog planiranja, poštari svakodnevno obilaze sandučiće koji su poluprazni ili uopšte nemaju pošiljki, što predstavlja čisti gubitak resursa. Sa druge strane, sandučići na visokofrekventnim lokacijama (koji imaju visok prioritet) često ostanu prepunjeni jer sistem ne prepoznaje potrebu za prioritetnijom obradom, što direktno narušava kvalitet usluge i povjerenje korisnika.

#### 3. Informacijsko sljepilo dispečera (Procesni "Black Box")
Onog trenutka kada poštar napusti distributivni centar i krene na teren, dispečerski centar gubi realni uvid u njegov rad. Sve informacije o progresu smjene, problemima na lokacijama ili kašnjenjima ostaju nepoznate dispečerima sve do kraja radnog dana kada se vrši manuelno razduživanje. Nedostatak centralizovane telemetrije onemogućava bilo kakav operativni nadzor.

#### 4. Nemogućnost brze reakcije na nepredviđene situacije
Terenski rad je podložan stalnim promjenama: zatvaranje ulica zbog radova, fizička oštećenja sandučića, gubitak ključeva ili vremenske nepogode. U tradicionalnom sistemu, ako poštar naiđe na blokiranu lokaciju, ta informacija se bilježi samo kao usmena opaska na kraju smjene. Ne postoji mehanizam da se ta lokacija trenutno označi kao nedostupna, niti da dispečer u hodu preusmjeri resurse ili ažurira planove za naredni dan.

#### 5. Nedostatak istorijskih podataka za analitiku menadžmenta
Bez digitalne evidencije posjeta, nemoguće je izračunati ključne indikatore učinka (KPI). Menadžment nema uvid u to kolika je stvarna stopa uspješnosti pražnjenja po tipovima sandučića, koji poštari konstantno premašuju vremenske okvire, te koje su lokacije statistički najproblematičnije. Odluke o širenju ili optimizaciji mreže donose se na osnovu nagađanja, a ne na osnovu egzaktnih podataka.

---

## 3. Glavne korisničke uloge

Sistem **PostRoute** implementira strogu i robusnu kontrolu pristupa zasnovanu na ulogama (**RBAC - Role-Based Access Control**). Unutar aplikacije su jasno definisane i izolovane tri korisničke uloge, pri čemu svaka uloga ima specifičan opseg odgovornosti, nivo autorizacije i prilagođen interfejs.

Autentifikacija za sve uloge se vrši centralizovano putem sigurnih JSON Web Tokena (JWT), dok su rute na frontendu zaštićene namjenskim omotačem `<RequireAuth roles={...}/>` kako bi se spriječio bilo kakav neovlašteni pristup podacima.

---

### 3.1. Administrator 
Administrator predstavlja vrhovni autoritet u sistemu i primarno je zadužen za konfiguraciju sistema, održavanje integriteta baze podataka i postavljanje sigurnosnih i operativnih osnova projekta. Ova uloga aplikaciju koristi isključivo putem desktop interfejsa.

**Ključne funkcionalnosti i odgovornosti:**
* **Upravljanje korisničkim računima i identitetima:** Kreiranje računa za dispečere i poštare. Administracija obuhvata unos ličnih podataka, dodjelu uloga i generisanje inicijalnih kredencijala za prvu prijavu.
* **Primjena sigurnosnih polisa:** Prilikom kreiranja novog korisnika (posebno za ulogu `PostalWorker`), poslovna logika (BLL) automatski forsira zastavicu `MustChangePassword = true`. Time se korisnik pri prvoj prijavi primorava na promjenu lozinke uz evaluaciju njene kompleksnosti preko komponente `PasswordStrengthIndicator`.
* **Upravljanje prostornim podacima (CRUD Mailboxes):** Unos i mapiranje lokacija poštanskih sandučića. Administrator koristi interaktivnu mapu baziranu na kombinaciji `Leaflet` i `OpenStreetMap` biblioteka, gdje klikom na mapu automatski preuzima i validira precizne geografske (GPS) koordinate (geografsku širinu i dužinu).
* **Definisanje operativnih metapodataka:** Za svaki sandučić administrator precizno definiše tip sandučića, nivo prioriteta (Visok, Srednji, Nizak) te stroga poslovna pravila koja uključuju dopuštene radne dane u sedmici i fiksne vremenske prozore unutar kojih se obilazak mora izvršiti.

---

### 3.2. Dispečer (Operativna i analitička uloga)
Dispečer je operativni menadžer sistema čiji je osnovni zadatak dnevno planiranje, alokacija resursa, donošenje ad-hoc odluka u hodu i evaluacija rada terenskih radnika. Ova uloga zahtijeva rad na desktop računaru zbog kompleksnosti dashboarda i analitičkih prikaza.

**Ključne funkcionalnosti i odgovornosti:**
* **Planiranje i automatska optimizacija ruta:** Pokretanje modula za automatsko generisanje dnevnih ruta. Dispečer bira poštara i sandučiće, a sistem proračunava matematički optimalan redoslijed obilaska, poštujući prioritete i vremenske prozore.
* **Ručna modifikacija i override sistema:** Prije nego što ruta postane aktivna i vidljiva poštaru, dispečer ima punu slobodu da ručno izmijeni redoslijed tačaka. To radi pomoću intuitivnog interfejsa za re-organizovanje (drag-and-drop ili gore/dolje kontrole), prilagođavajući rutu trenutnim vanrednim okolnostima na gradu.
* **Nadzor u realnom vremenu (Live Dashboard):** Praćenje realizacije svih aktivnih ruta u gradu kroz centralni dashboard. Zahvaljujući HTTP Polling mehanizmu koji automatski osvježava podatke na svakih 30 sekundi, dispečer ima živu telemetriju: vidi tačno vrijeme obrade pojedinih tačaka i trenutni progres.
* **Napredno izvještavanje i KPI analitika:** Generisanje naprednih operativnih izvještaja na kraju smjene. To uključuje izvještaj o učinku poštara (stope realizacije, timski prosjeci) i izvještaj po tipu sandučića (analiza anomalija i audit logovi problema) uz ugrađenu mogućnost eksporta podataka u standardni CSV format radi dalje eksterne obrade.

---

### 3.3. Poštar (Terenska i izvršna uloga)
Poštar (terenski radnik) je ključni izvršilac na terenu čiji je primarni zadatak fizička obrada poštanskih sandučića prema strogo definisanom planu. Ova uloga aplikaciju koristi **isključivo putem mobilnog web interfejsa**, koji je unutar koda rigorozno optimizovan za responsive prikaz na ekranima širine od minimalno 360px.

**Ključne funkcionalnosti i odgovornosti:**
* **Praćenje dodijeljene rute:** Nakon prijave na sistem, poštar dobija linearan i pregledan prikaz isključivo svoje aktivne rute za tekući dan. Sistem ga vodi korak-po-korak kroz tačke obilaska prema redoslijedu koji je optimizovao algoritam ili dispečer.
* **Evidencija statusa u realnom vremenu:** Prilikom dolaska na lokaciju sandučića, poštar u par klikova vrši promjenu stanja. Odabirom jedne od predefinisanih opcija (*Ispražnjen*, *Napunjen*, *Obrađen*) automatski se šalje mutacijski zahtjev prema backendu koji ažurira bazu i dispečerski dashboard.
* **Bezmodalno prijavljivanje anomalija i blokatora:** Ukoliko je lokacija fizički nedostupna (npr. radovi na cesti, oštećen brava, vremenske nepogode), poštar bira status *Nedostupna lokacija*. Unos problema je implementiran kroz inline panel unutar same stavke (izbjegnuti su modalni prozori koji narušavaju UI na mobilnim uređajima), gdje poštar bira jedan od 5 predefinisanih razloga. Ova akcija automatski aktivira alarm na dispečerskom dashboardu.

---

## 4. Glavne implementirane funkcionalnosti

### 4.1 Autentifikacija i upravljanje pristupom (Sprint 5)

- Prijava korisnika putem emaila/korisničkog imena i lozinke s JWT tokenom
- Obavezna promjena inicijalne lozinke pri prvoj prijavi (uz vizuelni indikator jačine lozinke s 4 nivoa)
- Sigurna odjava iz sistema
- Role-based access control (RBAC) s tri uloge: Administrator, Dispečer, Poštar
- Zaštita ruta na frontendu kroz `<RequireAuth roles={...}/>` wrapper komponentu i provjera na API razini (403 za neovlaštene zahtjeve)
- Lozinke hashirane BCrypt.Net-Next algoritmom s ugrađenim salt-om

### 4.2 Upravljanje korisnicima i sandučićima (Sprint 6–7)

- Kreiranje i pregled korisničkih računa poštara od strane administratora
- Unos novog sandučića s GPS koordinatama (interaktivni odabir lokacije na mapi), tipom, prioritetom i napomenom
- Izmjena svih podataka o sandučiću
- Pregled svih sandučića kroz tabelu/listu
- Definisanje prioriteta sandučića (Visok, Srednji, Nizak) — korišten kao ulaz za algoritam optimizacije
- Evidencija radnih pravila sandučića: radni dani u sedmici i vremenski prozor za obilazak (od–do)
- Pretraga sandučića s debounce mehanizmom (300ms, minimum 3 karaktera) i filtriranje po adresi, tipu, prioritetu i statusu

### 4.3 Generisanje i upravljanje rutama (Sprint 8)

- Automatsko generisanje dnevne rute korištenjem **nearest-neighbor heuristike** — algoritam iterativno bira najbliži neposjećeni sandučić na osnovu Haversine udaljenosti između GPS koordinata, uzimajući u obzir prioritet sandučića kao ponder pri odabiru
- Algoritam izoliran iza `IRouteOptimizationService` interfejsa — zamjena naprednijim algoritmom ne zahtijeva refaktoring ostatka sistema
- Dodjela generisane rute konkretnom poštaru od strane dispečera
- Pregled detalja rute: redoslijed obilaska, adrese sandučića, mapa s prikazom rute i svih lokacija
- Ručna izmjena redoslijeda obilaska unutar rute (drag-and-drop ili gore/dolje dugmad)

### 4.4 Terenski rad — mobilni prikaz (Sprint 9)

- Responzivni web interfejs optimiziran za mobilne uređaje (mobile-first pristup, minimalni breakpoint 360px)
- Prikaz dodijeljene rute poštaru s mapom, redoslijedom obilaska i statusom svake stavke
- Ažuriranje statusa sandučića (Ispraznjen, Napunjen, Obrađen) s evidencijom u audit logu
- Označavanje nedostupne lokacije: inline panel unutar stavke rute (ne modal) s padajućim menijem od 5 predefinisanih razloga + slobodan unos za „Ostalo"
- Jedan razlog nedostupnosti vidljiv dispečeru kroz audit log

### 4.5 Praćenje i izvještavanje (Sprint 9–10)

- **Dispečerski dashboard:** pregled svih ruta za odabrani datum s auto-refreshom svakih 30 sekundi (HTTP polling umjesto WebSocket-a — svjesna odluka za MVP)
- Vizualno naglašavanje problematičnih sandučića (označenih kao „Napunjen")
- **Upravljanje problematičnim lokacijama:** pregled svih nedostupnih i problematičnih lokacija, komentarisanje, dodjela akcija i praćenje statusa koordinacije između dispečera i poštara
- **Historija i arhiva ruta:** pregled svih realizovanih ruta po datumu i poštaru, s CSV eksportom
- **Izvještaj o učinku poštara:** KPI sumarni prikaz po poštaru za odabrani period (stopa realizacije, broj sandučića, prosječna uspješnost tima), sortiranje i detalji ruta koji ulaze u obračun — realizovano kroz zasebni backend endpoint s BLL agregacijom
- **Izvještaj po tipu sandučića:** stope realizacije i neuspjeha grupirane po tipu sandučića s evidencijom razloga neuspjeha

---

## 5. Pregled rada kroz sprintove

| Sprint | Fokus | Ključne isporuke |
|--------|-------|-----------------|
| **Sprint 1** | Definisanje projekta | Product Vision, identifikacija stakeholdera i Stakeholder Map, Team Charter, početni Product Backlog |
| **Sprint 2** | Razrada zahtjeva | User Stories, Acceptance Criteria, prioritizacija backlog-a, početni NFR zahtjevi |
| **Sprint 3** | Arhitektura i planiranje | Risk Register, Domain Model i Use Case Model, Architecture Overview, Test Strategy |
| **Sprint 4** | Tehnička osnova | Definition of Done, Initial Release Plan, tehnički skeleton — Git repozitorij, GitHub Actions CI/CD osnove, struktura .NET projekta i React SPA |
| **Sprint 5** | Autentifikacija i setup | Decision Log, AI Usage Log, kreiranje korisničkih računa, prijava/odjava, upravljanje ulogama i pristupom (RBAC) |
| **Sprint 6** | Korisnici i sandučići | Dodavanje i pregled poštara, unos, izmjena i pregled sandučića s interaktivnom mapom |
| **Sprint 7** | Napredne funkcije sandučića | Definisanje prioriteta sandučića, evidencija radnih pravila (radni dani, vremenski prozori) |
| **Sprint 8** | Generisanje i upravljanje rutama | Automatsko generisanje rute (nearest-neighbor), dodjela rute, pregled detalja, ručna izmjena redoslijeda |
| **Sprint 9** | Terenski rad i praćenje | Mobilni prikaz rute za poštara, ažuriranje statusa sandučića, označavanje nedostupnih lokacija, dispečerski dashboard s auto-refreshom, dnevni izvještaj |
| **Sprint 10** | Izvještavanje i završne funkcije | Historija i arhiva ruta, izvještaj o učinku poštara, izvještaj po tipu sandučića, pretraga i filtriranje sandučića, upravljanje problematičnim lokacijama |

---

## 6. Status isporuke

### 6.1 Završeno (Done)

Sve stavke označene kao **Done** u Product Backlogu su stvarno implementirane i verificirane od strane člana tima zaduženog za testiranje te stavke. To obuhvata kompletnu autentifikaciju i autorizaciju, upravljanje korisnicima i sandučićima, generisanje i upravljanje rutama, mobilni prikaz za poštara, praćenje statusa u realnom vremenu i kompletan sistem izvještavanja (historija, učinak poštara, realizacija po tipu sandučića).

### 6.2 Planirano za završni sprint (To Do / U toku)

- **PBI-052 — Stabilizacija sistema i regresijsko testiranje:** U toku — sistemsko testiranje svih ključnih korisničkih tokova i ispravka pronađenih grešaka
- **PBI-053 — Finalni inkrement i bug fixing:** U toku — završne korekcije projekta
- **PBI-054 do PBI-057 — Završna dokumentacija:** Release Notes, korisnička dokumentacija, tehnička dokumentacija, ažuriranje svih logova i artefakata
- **PBI-058 do PBI-061 — Individualne refleksije, peer evaluation, završni tim izvještaj, završna demonstracija**

### 6.3 Nije implementirano (svjesno izostavljeno iz MVP-a)

- **Nativna iOS/Android aplikacija** — sistem se oslanja isključivo na responzivni web interfejs
- **Hardversko GPS praćenje kretanja poštara** — fizičko kretanje se ne prati; evidencija se vrši ručnom promjenom statusa na lokaciji sandučića
- **Dinamičko re-rutiranje na osnovu trenutnog saobraćaja** — rute su statične za jedan radni dan nakon generisanja
- **Metaheuristički algoritmi optimizacije** (OR-Tools solver, genetski algoritmi) — ostavljeno za post-MVP fazu
- **Integracija s eksternim sistemima** — nema povezivanja s obračunom plata ili naprednim GIS servisima
- **Offline mod** — sistem zahtijeva stalnu internet konekciju; Service Worker / PWA strategija nije implementirana u MVP-u

---

## 7. Glavne tehničke odluke

### 7.1 Arhitektura sistema

Sistem je implementiran kao **monolitna web aplikacija** s jasnom troslojnom separacijom odgovornosti, što je odabrano zbog brzine razvoja unutar tima od 7 članova:

* **Backend (.NET 9):** Podijeljen na *Presentation* (REST API endpoints), *BLL* (poslovna logika i optimizacija) i *DAL* (EF Core pristup bazi).
* **Frontend (React 19 + TypeScript):** SPA arhitektura podijeljena na UI komponente, custom hooks (state) i infrastrukturu (Axios/Router).
* **Baza podataka:** PostgreSQL 16 (jedini izvor istine).
* **Komunikacija:** Isključivo REST API dokumentovan kroz Swagger/OpenAPI.

---

### 7.2 Tehnički stack (Tech Stack)

| Sloj | Tehnologija / Biblioteka | Verzija | Svrha / Uloga |
| :--- | :--- | :--- | :--- |
| **Backend** | .NET (C# Core API) | 9.0 | Primarni backend framework |
| **ORM** | Entity Framework Core | 9.0 | Code-First pristup i migracije |
| **Baza podataka**| PostgreSQL | 16.x | Relaciono skladište podataka |
| **Kriptografija**| BCrypt.Net-Next | 4.1.0 | Sigurno hashiranje lozinki u BLL-u |
| **Frontend** | React + TypeScript | 19.x / 5.x | Korisnički interfejs i tipska sigurnost |
| **Build Tool** | Vite | 6.x | Kompajliranje i bundling |
| **Stilizacija** | Tailwind CSS | 3.x | Utility-first UI dizajn |
| **HTTP Klijent** | Axios | Latest | API komunikacija s JWT presretačem |
| **Mape** | Leaflet + OpenStreetMap | Latest | Interaktivni prikaz i GPS koordinate |

---

### 7.3 Ključne odluke iz Zapisnika odluka (Decision Log)

* **DEC-001 (Sonner):** Odabran umjesto težih paketa zbog minimalne veličine (bundle size) i ugrađenog Promise-based API-ja za laganu izmjenu stanja notifikacije (loading $\rightarrow$ success).
* **DEC-002 (BCrypt.Net-Next):** Integrisan direktno u BLL umjesto punog `ASP.NET Identity` frameworka, čime je izbjegnuta nepotrebna kompleksnost u bazi za sistem koji koristi isključivo JWT.
* **DEC-003 (Axios wrapper):** Implementiran centralizovani HTTP klijent sa interceptorima koji automatski ubacuju JWT token u svako zaglavlje i globalno hendlaju 401 greške.
* **DEC-005 (BLL sigurnost):** Dodjela uloga i `MustChangePassword` zastavica fiksirani su u `UserService.CreateAsync` na backendu, čime je spriječena manipulacija privilegijama kroz presretanje API zahtjeva.
* **DEC-052 (HTTP Polling 30s):** Odabran na dispečerskom dashboardu umjesto WebSocketa (SignalR). Kašnjenje do 30s je zanemarivo u logističkom kontekstu pražnjenja sandučića, čime je ušteđeno razvojno vrijeme.
* **DEC-060 (Integer Enumi):** Statusi sandučića se u PostgreSQL čuvaju kao cijeli brojevi (`integer`). Ovo je omogućilo dodavanje novih statusa (npr. *Nedostupan*) bez pokretanja DB migracija.
* **DEC-064 (Backend KPI):** Proračun performansi radnika vrši se isključivo na backendu (`GET /api/routes/reports/postman-performance`), čime je spriječeno dupliranje kalkulacija na UI-u i osigurana tačnost podataka.

---

### 7.4 Branching strategija i CI/CD

Korišten je **GitLab Flow** na GitHub repozitoriju. Centralne grane su `main` (produkcija) i `develop` (integracija). Rad na funkcionalnostima izolovan je na `feature/PBI-xxx` granama, a spajanje u `develop` rađeno je isključivo putem Pull Requesta uz obavezan Code Review (minimalno 1 odobrenje).

Automatizovani **GitHub Actions CI pipeline** se okidao na svaki commit i PR, a provjeravao je frontend i backend servise.


---

## 8. Najveći problemi tokom razvoja i način rješavanja

### 8.1 Deployment pipeline i stabilizacija CI/CD

Jedan od najvećih praktičnih izazova bio je uspostavljanje stabilnog i ponovljivog deployment procesa. Sistem je deployovan na dvije odvojene platforme — **backend na Render, frontend na Netlify** — što je zahtijevalo pažljivu konfiguraciju:

- CORS politike na backendu (dozvoljena origin-a Netlify deployment URLa)
- Environment varijabli na obje platforme (database URL, JWT secret, API base URL)
- GitHub Actions workflowa koji koordinira build i deploy za obje platforme
- Inicijalno je dolazilo do problema s redoslijedom deploy koraka i neusklađenošću environment varijabli između okruženja

Rješenje je bilo iterativno — svaki neuspjeli deploy evidentiran je kao Problem, dokumentovano rješenje i dodano u deployment uputstvo kako bi sljedeći put bio brži. Na kraju je uspostavljen stabilan pipeline koji automatski deployuje svaki merge u `main`.

### 8.2 Praćenje prisustva poštara na lokaciji — svjesni kompromis

Sistem ne prati fizičko kretanje poštara GPS senzorom — ovo je svjesna odluka donesena na početku projekta (evidentirana u Product Visionu i Architecture Overviewu). Na web platformi u akademskom kontekstu implementacija background location trackinga nije izvodiva bez nativne aplikacije.

Rješenje: status posjete sandučiću evidentira se **ručnom promjenom statusa** od strane poštara. Ovaj kompromis je eksplicitno dokumentovan kao ograničenje sistema i nije predstavljan kao potpuno automatizovano rješenje. Dispečer dobija potvrdu da je poštar bio na lokaciji u trenutku promjene statusa, što je u praksi prihvatljivo za operativni kontekst.

### 8.3 Retroaktivno otkrivene funkcionalnosti

U kasnim sprintovima (9 i 10) tim je prepoznao nekoliko funkcionalnosti koje su logična i neophodna dopuna prethodno implementiranih dijelova, ali nisu bile identificirane u inicijalnoj analizi zahtjeva u Sprintu 2:

- **Hendlanje nedostupnih lokacija s razlogom** — poštar nije mogao samo "preskočiti" sandučić bez evidencije razloga
- **Komunikacija poštar–dispečer** o problematičnim lokacijama — dispečer je trebao mehanizam za praćenje i koordinaciju

Ove funkcionalnosti implementirane su u Sprintu 10 i zahtijevale su prilagodbu postojećeg API-ja (proširenje `MailboxStatus` enum-a, novi endpoint za problematične lokacije) i frontend komponenti. Retroaktivno su dodane i u Product Backlog.

**Naučena lekcija:** Korisničke tokove treba analizirati end-to-end u ranoj fazi, ne samo za happy path, nego i za rubne scenarije (što se dešava kad poštar ne može pristupiti lokaciji?).

### 8.4 Koordinacija paralelnog razvoja u timu od 7 članova

Tim je radio asinhrono s jednim sedmičnim Google Meet sastankom i Viber grupom za operativnu komunikaciju. Paralelni razvoj na različitim featurima povremeno je dovodio do merge konflikata i neusklađenosti u API ugovorima.

Mitigacije koje su se pokazale efektivnim:
- **Feature-based organizacija foldera** na frontendu (po use-caseu, ne po tipu fajla) — svaki developer radio u svom feature folderu s minimalnim preklapanjem
- **Decision Log** kao centralni dokument za sve arhitektonske odluke — sprječio je situacije gdje različiti dijelovi tima implementuju istu stvar na različit način
- **Obavezan code review** s minimalno jednim odobrenjem prije merge-a

---

## 9. Šta bi tim unaprijedio da se projekat nastavlja

### 9.1 Pametnija i automatizovana dodjela sandučića poštarima

Trenutni sistem zahtijeva da dispečer ručno doda sandučiće za svaku rutu. Logičan sljedeći korak bio bi automatska dodjela sandučića poštarima na osnovu geografskog sektora, kapaciteta poštara i radnog vremena. Sistem bi sam prepoznao koji poštar pokriva koji dio grada i generisao rute bez manuelne intervencije — dispečer bi samo potvrdio ili prilagodio prijedlog.

### 9.2 Navigacija do lokacije za poštara

Mobilni prikaz trenutno pokazuje redoslijed obilaska i kartu s lokacijama, ali ne pruža turn-by-turn navigaciju. Integracija s Google Maps Directions API-jem ili OpenRouteService-om omogućila bi poštaru pokretanje navigacije do sljedeće lokacije direktno unutar aplikacije, bez prebacivanja između PostRoute-a i vanjske navigacijske aplikacije.

### 9.3 Napredni algoritam optimizacije ruta

Nearest-neighbor heuristika dovoljna je za MVP s manjim skupovima sandučića (do ~50 po ruti), ali daje suboptimalne rute za veće skupove — greška može biti i do 20–25% u odnosu na optimalnu rutu. Algoritam je svjesno izoliran iza `IRouteOptimizationService` interfejsa upravo radi ove buduće zamjene. OR-Tools solver (Google) ili metaheuristički pristup (genetski algoritam, simulated annealing) bili bi logičan sljedeći korak bez ikakvih promjena u ostatku sistema.

### 9.4 Zamjena HTTP pollinga sa SignalR

HTTP polling s intervalom od 30 sekundi funkcionalno je rješenje za MVP, ali uvodi nepotrebnu mrežnu aktivnost i do 30 sekundi kašnjenja u prikazu statusa. WebSocket komunikacija putem SignalR (biblioteka koja je već dio .NET ekosistema) omogućila bi trenutna obavještenja dispečeru — posebno korisno pri velikom broju aktivnih poštara ili hitnim situacijama na terenu.

### 9.5 PWA i offline podrška za poštara

Poštari na terenu mogu imati nestabilnu ili odsutnu internet konekciju. Progressive Web App (PWA) strategija s Service Worker-om i lokalnim IndexedDB cacheom omogućila bi nastavak rada bez konekcije (preuzimanje rute unaprijed, lokalno snimanje promjena statusa) i automatsku sinkronizaciju čim se veza uspostavi.

### 9.6 Proširene analitike i izvještaji za menadžment

Trenutni sistem pruža operativne izvještaje (učinak poštara, realizacija po tipu sandučića). Za menadžersku razinu korisni bi bili trendovi kroz duži vremenski period, heatmape problematičnih lokacija na mapi, prediktivna upozorenja za sandučiće koji se redovno javljaju kao nedostupni, i export u Excel ili PDF format.

