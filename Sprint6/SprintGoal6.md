# Sprint Goal — Sprint 6

## Sprint broj
6

## Sprint cilj
Omogućiti upravljanje poštarima i poštanskim sandučićima — dodavanje, pregled i izmjenu podataka — kako bi sistem imao operativnu osnovu za planiranje ruta u narednim sprintovima.

## Ključne stavke koje tim želi završiti
- PBI-015: Dodavanje poštara — unos i evidencija osnovnih podataka o novom poštaru
- PBI-016: Pregled liste poštara — pregled svih poštara s osnovnim podacima i statusom
- PBI-017: Dodavanje poštanskog sandučića — unos novog sandučića s lokacijom, tipom i osnovnim podacima
- PBI-018: Izmjena podataka o sandučiću — izmjena lokacije, tipa, prioriteta i drugih podataka
- PBI-019: Pregled sandučića na listi — pregled svih evidentiranih sandučića kroz tabelu

## Rizici i zavisnosti
- PBI-015 i PBI-016 zavise od završenog autentifikacijskog modula iz Sprinta 5
- PBI-017, PBI-018 i PBI-019 su međusobno zavisni — dodavanje mora biti završeno prije pregleda i izmjene
- Geolokacijski podaci za sandučiće zahtijevaju odluku o map biblioteci (Leaflet vs MapLibre) — evidentirati u Decision Logu na početku sprinta