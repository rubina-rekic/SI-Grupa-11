# Sprint Review Summary — Sprint 4

## Sprint broj
4

## Period sprinta
15. april 2026 – 21. april 2026

## Planirani sprint goal
Završna validacija human-first faze projekta i uspostavljanje tehničkih temelja za nadolazeću implementacijsku fazu. Tim definiše Definition of Done kao zajednički standard kvaliteta, izrađuje Initial Release Plan koji povezuje backlog sa sprintovima do kraja semestra, te postavlja tehnički skeleton sistema i inicijalnu strukturu repozitorija.

---

## Šta je završeno

| PBI ID | Naziv | Odgovorna osoba/e | Napomena |
|--------|-------|-------------------|----------|
| PBI-036 | Definition of Done | Nejla Karalić, Rubina Rekić | Definirano 7 kriterija završenosti; primjenjuje se na sve User Storije u svim sprintovima |
| PBI-037 | Initial Release Plan | Rubina Rekić | Mapiranje svih PBI-a na sprintove 5–13; definirani 3 release kandidata (Sprint 8, 9, 12) |
| PBI-038 | Tehnički skeleton projekta | Emrah Žunić i Kerim Šikalo  | Folder PROJEKAT u repozitoriju sa PostRoute.Api (Controllers, Contracts, Configuration), PostRoute.BLL (Services, Models, DependencyInjection) i PostRoute.DAL (Repositories, Entities, DependencyInjection); frontend sa Vite + React + TypeScript strukturom |
| — | Struktura repozitorija i tehnički setup | Faruk Avdegić, Emrah Žunić i Ibrahim Tabaković | Dokumentovana GitLab Flow branching strategija, tehnički stack (React 19, ASP.NET Core .NET 9, PostgreSQL), CI/CD pipeline koraci, deployment strategija (Ubuntu VM + Docker + Nginx) i lokalni razvojni setup u TechnicalSetup.md |

## Šta nije završeno

Sve planirane stavke su završene.

---

## Demonstrirani artefakti

- **Definition of Done** — dokument s 7 kriterija završenosti primjenjivih na sve User Storije
- **Initial Release Plan** — plan s 9 inkremenata, zavisnostima, rizicima i 3 eksplicitna release kandidata
- **TechnicalSetup.md** — branching strategija, tehnički stack, CI/CD osnove
- **Repozitorij (GitHub)** — demonstrirana struktura foldera PROJEKAT/backend (API/BLL/DAL) i PROJEKAT/frontend u repozitoriju SI-Grupa-11

---

## Glavni problemi i blokeri

Nije bilo blokatora. 

---

## Ključne odluke donesene u sprintu

- Odabran GitLab Flow kao branching strategija
- Definiran tehnički stack: React 19 + TypeScript 6 (frontend), ASP.NET Core .NET 9 + C# (backend), PostgreSQL 16 (baza podataka)
- Backend strukturiran po slojevima: API / BLL / DAL — u skladu sa Architecture Overview dokumentom
- Definirani release kandidati: Release 1 nakon Sprinta 8, Release 2 nakon Sprinta 9, Release 3 (finalni) nakon Sprinta 12
- Definisana deployment strategija: Ubuntu 22.04 LTS VM + Docker Compose + Nginx reverse proxy + Kestrel

---

## Povratna informacija Product Ownera

Initial releease plan je detaljan i realističan. Tehnički skeleton i struktura repozitorija su dobro osmišljeni, što će olakšati razvoj u narednim sprintovima.
U Definition of Done potrebno je bilo dodati ko vrši testiranje (konkretno, developer koji implementira funkcionalnost). Ostatak sprint reviewa je prošao bez primjedbi.
---

## Zaključak za naredni sprint

U sprintu 5 osnovni zadatak bit će omogućiti kreiranje profila i osigurati zaštitu podataka.
Ovo obuhvata implementaciju korisničkih računa, autentifikacije i autorizacije, kao i osiguravanje sigurnosnih protokola.
Svi nedostaci prethodnog sprinta su adresirani, kako se isti ne bi propagirali u naredne faze razvoja.
