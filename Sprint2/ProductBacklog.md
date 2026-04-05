# Product Backlog

**Projekat:** Sistem za optimizaciju ruta punjenja i pražnjenja poštanskih sandučića  

| ID | Naziv stavke | Kratak opis | Tip stavke | Prioritet | Procjena napora | Status | Veza sa sprintom / planom | Napomena |
|---|---|---|---|---|---|---|---|---|
| PBI-001 | Definisati problem i ciljeve sistema | Razraditi problem koji sistem rješava, očekivanu vrijednost i osnovne ciljeve proizvoda. | research | High | 3h 30min | Done | Sprint 1 |  |
| PBI-002 | Izraditi Product Vision | Pripremiti i finalizirati Product Vision dokument na osnovu dogovorenog scope-a i potreba korisnika. | documentation | High | 4h | Done | Sprint 1 |  |
| PBI-003 | Identifikovati stakeholdere sistema | Prepoznati glavne stakeholder grupe, njihove interese, očekivanja i uticaj na sistem. | research | High | 2h 30min | Done | Sprint 1 |  |
| PBI-004 | Izraditi Stakeholder Map | Dokumentovati stakeholdere kroz preglednu mapu sa ulogama i prioritetom komunikacije. | documentation | High | 2h | Done | Sprint 1 |  |
| PBI-005 | Napraviti početni Product Backlog | Sastaviti početni backlog na osnovu Product Visiona, scope-a i početnih pretpostavki tima. | documentation | High | 3h | Done | Sprint 1 | Živi dokument |
| PBI-006 | Razraditi prioritetne user storyje | Definisati najvažnije user storyje za osnovne MVP funkcionalnosti. | documentation | High | 4h 30min | To Do | Sprint 2 |  |
| PBI-007 | Definisati acceptance criteria | Napisati mjerljive i provjerljive acceptance kriterije za prioritetne storyje. | documentation | High | 4h | To Do | Sprint 2 |  |
| PBI-008 | Uraditi prioritizaciju backloga | Razvrstati stavke po poslovnoj vrijednosti, zavisnostima i značaju za MVP. | research | High | 2h | To Do | Sprint 2 |  |
| PBI-009 | Definisati početne NFR zahtjeve | Identifikovati početne nefunkcionalne zahtjeve poput sigurnosti, upotrebljivosti i pouzdanosti. | documentation | Medium | 3h | To Do | Sprint 2 |  |
| PBI-031 | Izraditi početni Risk Register | Identifikovati glavne projektne, tehničke i operativne rizike sistema, procijeniti njihov uticaj i vjerovatnoću, te definisati početne mjere mitigacije. | documentation | High | 3h 30min | To Do | Sprint 3 |  |
| PBI-033 | Izraditi Domain Model i/ili Use Case Model | Dokumentovati model domene i/ili ključne use-case scenarije na osnovu prethodno razrađenih zahtjeva i korisničkih uloga. | documentation | High | 4h | To Do | Sprint 3 |  |
| PBI-034 | Definisati Architecture Overview | Pripremiti početni pregled arhitekture sistema, glavnih komponenti, njihovih odgovornosti i osnovnog toka podataka. | documentation | High | 3h 30min | To Do | Sprint 3 | Osnovni arhitektonski pravac |
| PBI-035 | Definisati Test Strategy | Odrediti ciljeve testiranja, nivoe testiranja, vezu sa acceptance kriterijima i način evidentiranja rezultata testiranja. | documentation | High | 3h | To Do | Sprint 3 |  |
| PBI-011 | Registracija korisnika | Omogućiti kreiranje korisničkog računa za korisnike koji trebaju pristup sistemu. | feature | High | 6h | To Do | MVP | Osnovna autentikacija |
| PBI-012 | Prijava korisnika | Omogućiti korisniku prijavu u sistem putem emaila/korisničkog imena i lozinke. | feature | High | 5h | To Do | MVP |  |
| PBI-013 | Odjava korisnika | Omogućiti sigurno odjavljivanje iz sistema. | feature | Medium | 1h 30min | To Do | MVP |  |
| PBI-014 | Uloge i pristup po ulozi | Ograničiti pristup funkcionalnostima na osnovu uloge: administrator, dispečer, poštar. | feature | High | 5h 30min | To Do | MVP |  |
| PBI-015 | Dodavanje poštara | Omogućiti administratoru unos novog poštara u sistem. | feature | Medium | 3h | To Do | MVP |  |
| PBI-016 | Pregled liste poštara | Omogućiti pregled svih poštara sa osnovnim podacima i statusom aktivnosti. | feature | Medium | 2h 30min | To Do | MVP |  |
| PBI-017 | Dodavanje poštanskog sandučića | Omogućiti unos novog sandučića sa lokacijom, tipom i osnovnim podacima. | feature | High | 4h | To Do | MVP |  |
| PBI-018 | Izmjena podataka o sandučiću | Omogućiti administratoru izmjenu lokacije, tipa, prioriteta i drugih podataka o sandučiću. | feature | Medium | 4h | To Do | MVP |  |
| PBI-019 | Pregled sandučića na listi | Omogućiti pregled svih evidentiranih sandučića kroz jednostavnu tabelu/listu. | feature | Medium | 3h | To Do | MVP |  |
| PBI-020 | Definisanje prioriteta sandučića | Omogućiti postavljanje ili izmjenu prioriteta za pražnjenje/punjenje sandučića. | feature | High | 3h 30min | To Do | MVP |  |
| PBI-022 | Generisanje dnevne rute | Omogućiti dispečeru automatsko generisanje dnevne rute za odabranog poštara. | feature | High | 8h | To Do | MVP | Osnovna optimizacija |
| PBI-023 | Dodjela rute poštaru | Omogućiti dispečeru da dodijeli generisanu rutu konkretnom poštaru. | feature | High | 3h | To Do | MVP |  |
| PBI-024 | Pregled detalja rute | Omogućiti pregled redoslijeda obilaska, uključenih sandučića i osnovnih detalja rute. | feature | Medium | 3h 30min | To Do | MVP |  |
| PBI-025 | Ručna izmjena redoslijeda obilaska | Omogućiti dispečeru ručno prilagođavanje redoslijeda obilaska unutar rute. | feature | Medium | 5h | To Do | MVP |  |
| PBI-026 | Mobilni prikaz dodijeljene rute | Omogućiti poštaru da preko responzivnog web interfejsa vidi svoju dodijeljenu rutu. | feature | High | 5h 30min | To Do | MVP | Responzivni web |
| PBI-027 | Ažuriranje statusa sandučića | Omogućiti poštaru da promijeni status sandučića tokom obilaska. | feature | High | 4h | To Do | MVP | Npr. ispražnjen, napunjen |
| PBI-028 | Označavanje nedostupne lokacije | Omogućiti poštaru da evidentira da lokacija nije bila dostupna tokom obilaska. | feature | Low | 2h 30min | To Do | MVP |  |
| PBI-029 | Praćenje statusa rute od strane dispečera | Omogućiti dispečeru uvid u to koji su sandučići obrađeni, preskočeni ili problematični. | feature | High | 5h | To Do | MVP |  |
| PBI-030 | Osnovni dnevni izvještaj | Omogućiti generisanje osnovnog dnevnog izvještaja o realizovanim i nerealizovanim obilascima. | feature | Low | 4h | To Do | MVP |  |

> **Napomena:** Od sprinta 5 nadalje, evidencija implementacijskog dijela Product Backloga vodit će se i kroz [JIRA](https://si-grupa-11.atlassian.net/jira/software/projects/SG1/boards/1/backlog). Product Backlog će se redovno ažurirati i u ovom `.md` dokumentu i na JIRA stranici, kako bi sve stavke ostale usklađene i pregledne.


