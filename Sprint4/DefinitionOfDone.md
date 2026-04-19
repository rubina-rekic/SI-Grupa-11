# Definition of Done (DoD)
---
## Scope

DoD se primjenjuje na **sve User Storije u svim sprintovima** do kraja projekta. Svi članovi tima su dužni poznavati i primjenjivati isti standard bez izuzetaka, osim u slučajevima definisanim u sekciji *Izuzeci*.

---

## Proces provjere

Prije nego što developer pomjeri User Story u status **Done**, dužan je samostalno proći kroz svaku stavku ovog dokumenta i potvrditi da je ispunjena.

Ukoliko neka stavka nije zadovoljena, Story ostaje u statusu **In Progress** dok se uvjet ne ispuni.

---

## Stavka se smatra završenom kada:

### 1. Implementirana / dokumentovana prema dogovoru
Funkcionalnost ili dokumentacija opisana u User Storiju je realizirana u skladu sa zadatkom i dogovorom unutar tima. Nema nedovršenih dijelova koji su direktno vezani za dati Story.

### 2. Zadovoljeni acceptance kriteriji
Sve stavke navedene u acceptance kriterijima User Storija su provjerene i potvrđene. Story ne može biti označen kao završen ukoliko i jedan acceptance kriterij nije ispunjen.

### 3. Urađen code review
Kod je pregledan od strane najmanje jednog drugog člana tima. Eventualni komentari su razriješeni prije merganja.

### 4. Testirano na odgovarajući način
Implementacija je testirana – ručno ili automatski – i potvrđeno je da radi ispravno. Nije pronađen kritičan bug koji blokira funkcionalnost.

### 5. Push na repozitorij i merge na glavni branch
Promjene su commitovane i pushane na repozitorij. Build se izvršava bez grešaka.

### 6. Evidentirana u relevantnim artefaktima
Promjene koje zahtijevaju ažuriranje projektnih artefakata (Product Backlog, Decision Log,
Sprint Review Summary ili drugi relevantni dokumenti) su evidentirane i reflektuju stvarno
stanje stavke.

### 7. Spremno za demonstraciju na Sprint Reviewu
User Story je u stanju u kojem može biti prikazan na Sprint Reviewu – funkcionalan, pregledan i stabilan.

---

## Izuzeci

Ukoliko određena stavka DoD-a nije primjenjiva za konkretan User Story, izuzetak mora biti:

- eksplicitno dogovoren unutar tima,
- zabilježen u komentaru na samom Story-ju u alatu za praćenje.

O izuzetku odlučuje tim zajednički, ne individualno.

---

> **Napomena:** Tim može proširiti ovaj dokument tokom projekta, ali sve izmjene moraju biti dogovorene i poznate svim članovima tima. Jedna zajednička definicija vrijedi za cijeli tim.
