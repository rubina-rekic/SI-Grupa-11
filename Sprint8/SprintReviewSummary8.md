# Sprint Review Summary — Sprint 8

## Sprint broj
8

## Planirani sprint goal
Implementirati kompletno upravljanje rutama — automatski generisati optimiziranu dnevnu rutu na osnovu GPS koordinata i prioriteta sandučića, dodijeliti je konkretnom poštaru te osigurati dispečeru pregled i mogućnost ručnog prilagođavanja redoslijeda obilaska, kako bi sistem bio spreman za operativnu upotrebu na terenu.

## Šta je završeno
- PBI-022: Generisanje dnevne rute sa uključenim filtriranjem po danu dostupnosti
- PBI-023: Dodjela rute poštaru
- PBI-024: Pregled detalja rute uz vizualizaciju na Leaflet mapi
- PBI-025: Ručna izmjena redoslijeda obilaska s live preračunavanjem vremena dolaska

## Šta nije završeno
Nije uspješno obavljen deployment aplikacije na produkcijsko/testno okruženje zbog objektivnih prepreka, pa stoga pregled nije mogao biti održan na live verziji aplikacije.

## Demonstrirane funkcionalnosti ili artefakti
- **Generisanje rute:** Kreiranje dnevne rute koja uzima u obzir radne dane i prioritete.
- **Dodjela rute poštaru:** Interfejs za dispečera gdje odabire dostupnog poštara iz liste za taj dan.
- **Pregled detalja i ručno preuređivanje rute:** Tabela s podrškom za pomjeranje stavki (gore/dolje), koja u realnom vremenu ažurira Leaflet mapu s prikazanim putanjama i preračunava vremena dolaska.

## Glavni problemi i blokeri
Razvoj funkcionalnosti tekao je prilično glatko, a implementacija Euklidske udaljenosti i integracija mapa završeni su u skladu s očekivanjima. Glavni bloker ticao se dostupnosti ključnih članova tima odgovornih za infrastrukturu, što je direktno onemogućilo postavljanje aplikacije (deployment) na server na kraju sprinta.

## Ključne odluke donesene u sprintu
(Dokumentovano u Decision Logu):
- **DEC-040:** Korištenje switch expression-a za sigurno mapiranje .NET `DayOfWeek` u enum vrijednosti bit flagova prilikom generisanja rute.
- **DEC-041 & DEC-042:** Evidentiranje izmjena redoslijeda putem direktnih polja u `Route` entitetu te dvostruka kalkulacija vremena dolaska (na frontendu za responzivan UX, te re-kalkulacija na backendu iz sigurnosnih razloga).
- **DEC-043:** Korištenje `useRef` za pohranu originalnog redoslijeda na frontendu, izbjegavajući nepotrebne cikluse renderisanja.

## Povratna informacija Product Ownera
Product Owner nije prihvatio demonstraciju na lokalnom okruženju te je, zbog izostanka deploymenta na produkcijsko/testno okruženje, timu dodijeljeno 30% od ukupnih bodova za ovu isporuku. Kao mjera za naredni period, tim će prioritizirati uspostavljanje stabilnog hosting okruženja koje nije ovisno o besplatnim tierovima s ograničenim trajanjem, kako slična situacija ne bi blokirala isporuku u budućnosti.

## Zaključak za naredni sprint
Sprint 8 je uspješno zaokružio cjelokupnu logiku kreiranja i modifikacije rute iz perspektive razvoja. Bez obzira na smanjeno bodovanje zbog izostanka deploymenta, izgrađen je čvrst tehnički temelj. U Sprintu 9 fokus prebacujemo na operativnu primjenu — razvoj mobilnog prikaza i ažuriranja statusa sandučića direktno sa terena od strane poštara (US-26, US-27). Također, prvi prioritet tima će biti kompletiranje deploymenta i uspostavljanje boljeg protokola dijeljenja znanja za infrastrukturne procese.
