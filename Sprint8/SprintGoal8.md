# Sprint Goal — Sprint 8

## Sprint broj
8

## Sprint cilj
Implementirati kompletno upravljanje rutama — automatski generisati optimiziranu dnevnu rutu na osnovu GPS koordinata i prioriteta sandučića, dodijeliti je konkretnom poštaru te osigurati dispečeru pregled i mogućnost ručnog prilagođavanja redoslijeda obilaska, kako bi sistem bio spreman za operativnu upotrebu na terenu.

## Ključne stavke koje tim želi završiti
- PBI-022: Generisanje dnevne rute (US-22)
- PBI-023: Dodjela rute poštaru (US-23)
- PBI-024: Pregled detalja rute (US-24)
- PBI-025: Ručna izmjena redoslijeda obilaska (US-25)

## Rizici i zavisnosti
- US-22 zavisi od završenih PBI-020 (prioriteti sandučića) i PBI-021 (radna pravila) iz Sprinta 7
- US-23 zavisi od US-22 — dodjela rute moguća je tek nakon uspješnog generisanja
- US-24 zavisi od US-22 i US-23 — pregled detalja pretpostavlja postojanje generisane rute
- US-25 zavisi od US-24 — ručna izmjena redoslijeda dostupna je isključivo na prikazu postojeće rute
- Sve četiri stavke čine osnovu za US-26 (Mobilni prikaz dodijeljene rute) i US-27 (Ažuriranje statusa sandučića) u Sprintu 9
