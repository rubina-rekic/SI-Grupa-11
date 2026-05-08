# Sprint Review Summary — Sprint 6

## Sprint broj
6

## Planirani sprint goal
Omogućiti upravljanje poštarima i poštanskim sandučićima — dodavanje, pregled i izmjenu podataka — kako bi sistem imao operativnu osnovu za planiranje ruta u narednim sprintovima.

## Šta je završeno
- PBI-015: Dodavanje poštara — unos i evidencija osnovnih podataka o novom poštaru
- PBI-016: Pregled liste poštara — pregled svih poštara s osnovnim podacima i statusom
- PBI-017: Dodavanje poštanskog sandučića — unos novog sandučića s lokacijom, tipom i osnovnim podacima
- PBI-018: Izmjena podataka o sandučiću — izmjena lokacije, tipa, prioriteta i drugih podataka
- PBI-019: Pregled sandučića na listi — pregled svih evidentiranih sandučića kroz tabelu


## Šta nije završeno
Tim je uspio uspio zavrsiti sve planirane stavke, tako da nema neispunjenih ciljeva iz sprinta.

## Demonstrirane funkcionalnosti ili artefakti
- Dodavanje poštara: Prikaz forme za unos podataka o poštaru i uspješno dodavanje u sistem
- Pregled poštara: Prikaz liste svih poštara s osnovnim informacijama i statusom
- Dodavanje sandučića: Prikaz forme za unos podataka o sandučiću i uspješno dodavanje u sistem
- Izmjena sandučića: Prikaz forme za izmjenu podataka o sandučiću i uspješno ažuriranje informacija
- Pregled sandučića: Prikaz tabele sa svim evidentiranim sandučićima i njihovim osnovnim podacima


## Glavni problemi i blokeri
Tim nije imao značajnih problema ili blokera tokom ovog sprinta, što je omogućilo uspješno završavanje svih planiranih stavki. Međutim, važno je napomenuti da su PBI-015 i PBI-016 zavisili od završenog autentifikacijskog modula iz Sprinta 5, što je zahtijevalo dobru koordinaciju između clanova tima kako bi se osiguralo da su svi potrebni moduli spremni na vrijeme. Takođe, tim je morao donijeti odluku o izboru map biblioteke (Leaflet vs MapLibre) za geolokacijske podatke sandučića, što je bilo ključno za dalji razvoj funkcionalnosti vezanih za lokaciju sandučića.


## Ključne odluke donesene u sprintu
- Odluka o izboru map biblioteke: Tim je odlučio koristiti Leaflet kao map biblioteku za geolokacijske podatke sandučića, zbog njegove jednostavnosti i široke podrške u zajednici. Ova odluka je zabilježena u Decision Logu na početku sprinta.

## Povratna informacija Product Ownera
Dobili smo povratnu informaciju u vezi Sprint Retrospective dokumenta, kako je isti trebo biti popunjen prije sastanka s njim, tj. trebao je biti dio odradjenog sprinta. Takodjer, receno nam je da je ui nedovoljno dobro dizajniran, te da bi trebalo poraditi na tome u narednim sprintovima.

## Zaključak za naredni sprint
UI dizajn će biti prioritet u narednom sprintu, s fokusom na poboljšanje korisničkog iskustva i vizualne privlačnosti aplikacije. Takođe, tim će nastaviti s implementacijom funkcionalnosti vezanih za planiranje ruta, koristeći podatke o poštarima i sandučićima koji su uneseni u ovom sprintu. Očekuje se da će se dodatno raditi na optimizaciji performansi i stabilnosti aplikacije, kako bi se osiguralo da je spremna za veće opterećenje kada se više funkcionalnosti implementira.
