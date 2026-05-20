# Sprint Review Summary — Sprint 7

## Sprint broj
7

## Planirani sprint goal
Definisati logičke parametre sistema kroz određivanje važnosti sandučića i njihovih vremenskih ograničenja — implementirati prioritete sandučića i evidenciju radnih pravila kako bi sistem imao sve potrebne podatke za generisanje operativno izvodivih ruta.

## Šta je završeno
- PBI-020: Definisanje prioriteta sandučića — postavljanje, sortiranje i historija promjena prioriteta
- PBI-021: Evidencija radnih pravila sandučića — vremenska dostupnost i radni dani
- PBI-022: Generisanje dnevne rute — bonus, urađeno prije plana (predviđeno za Sprint 8)

## Šta nije završeno
Tim je uspio završiti sve planirane stavke, uključujući i predviđenu stavku PBI-022 za naredni sprint. Nema neispunjenih ciljeva iz sprinta.

## Demonstrirane funkcionalnosti ili artefakti
- Definisanje prioriteta sandučića: Prikaz padajućeg menija s tri nivoa (Visok/Srednji/Nizak), vizuelno kodiranje bojama i mogućnost ažuriranja prioriteta s obrazloženjem
- Sortiranje liste sandučića po prioritetu: Klik na dugme koji sortira listu descenedentno (Visok → Srednji → Nizak) s vizuelnim indikatorom aktivnog sortiranja
- Historija promjena prioriteta: Tabela s detaljima o datumu, administratoru, starim i novim prioritetima i obrazloženjem promjene
- Vremenska dostupnost sandučića: Time picker 24h format s mogućnošću postavljanja dva termina dnevno ili 24/7 dostupnosti
- Radni dani sandučića: Sedam checkbox kontrola za izbor radnih dana s mogućnošću za "Označi sve / Odznači sve"
- Generisanje dnevne rute: Automatsko kreiranje prijedloga dnevne rute na osnovu GPS koordinata i prioriteta sandučića

## Glavni problemi i blokeri
Tim nije imao značajnih problema ili blokera tokom ovog sprinta, što je omogućilo uspješno završavanje svih planiranih stavki plus dodatnu stavku PBI-022. Koordinacija između članova tima je bila odličan, a svi radovi su odrađeni na vrijeme s dobrom kvalitetom. Također je postojana komunikacija s ostalim dijelovima sistema rezultirala sinhronizovanjem vremenske dostupnosti i radnih dana sa algoritmom za generisanje ruta.

## Ključne odluke donesene u sprintu
Tijekom sprinta donesene su sljedeće arhitektonske i tehnička odluke (dokumentovane u Decision Logu):
- **DEC-035:** Vremenska dostupnost modelirana kroz dva opcionalna termina + 24/7 flag — omogućava fleksibilan raspored dostupnosti za raznovrsne scenarije
- **DEC-036:** Ruta na mapi prikazana через Leaflet Routing Machine s OSRM routerom — omogućava prikaz rute koja prati ceste umjesto pravolinijskog prikaza
- **DEC-037:** Sakrivanje routing itinerarija i formatiranje vremena kao HH:MM — čitljivija mapa bez opterećenja detalja
- **DEC-038:** Responsive proširenje admin lista na puni content width — bolja iskorištenost prostora za tabele
- **DEC-039:** Radni dani sandučića modelirani kao bit flag enum — kompaktan i fleksibilan model za validaciju

## Povratna informacija Product Ownera
Product Owner je dao povratnu informaciju da je sve okej i da su svi radovi odrađeni prema specifikaciji. Nema kritičnih ili blokantnih napomena. 

## Zaključak za naredni sprint
Sa završenim prioritetima i vremenskim pravilima, sistem je gotovo potpuno pripremljen za operativno testiranje u narednim sprintovima. Sprint 8 će se fokusirati na finalnu optimizaciju algoritma za generisanje ruta i detaljniju integraciju svih parametara (prioriteti, vremenske dostupnosti, radni dani) u logiku planiranja. Očekuje se nastavak s implementacijom funkcionalnosti vezanih za dispečerski nadzor i izvršavanje ruta na terenu, koristeći sve parametre koji su definisani u ovom sprintu.
