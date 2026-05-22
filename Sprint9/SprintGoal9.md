# Sprint Goal — Sprint 9

## Sprint broj
9

## Sprint cilj
Operacionalizovati sistem na terenu — omogućiti poštaru pristup dodijeljenoj ruti putem responzivnog mobilnog prikaza i ažuriranje statusa sandučića tokom obilaska, uz istovremeni real-time uvid dispečera u napredak rute i generisanje osnovnog dnevnog izvještaja, čime se zatvara puna operativna petlja od planiranja do realizacije.

## Ključne stavke koje tim želi završiti
- PBI-026: Mobilni prikaz dodijeljene rute (US-26)
- PBI-027: Ažuriranje statusa sandučića (US-27)
- PBI-028: Označavanje nedostupne lokacije (US-28)
- PBI-029: Praćenje statusa rute od strane dispečera (US-29)
- PBI-030: Osnovni dnevni izvještaj (US-30)

## Rizici i zavisnosti
- US-26 zavisi od završenih PBI-023 (dodjela rute) i PBI-024 (pregled detalja rute) iz Sprinta 8 — poštar može pristupiti samo ruti koja mu je dodijeljena
- US-27 zavisi od US-26 — ažuriranje statusa sandučića dostupno je isključivo unutar mobilnog prikaza dodijeljene rute
- US-28 zavisi od US-26 i US-27 — označavanje nedostupne lokacije nadograđuje funkcionalnost ažuriranja statusa
- US-29 zavisi od US-27 i US-28 — dispečer prati samo one statuse koje je poštar evidentirao
- US-30 zavisi od US-27, US-28 i US-29 — izvještaj agregira podatke iz evidentiranih statusa sandučića i rute
- Kompleksnost ovog sprinta leži u real-time sinhronizaciji između pogleda poštara i dispečera — potrebno osigurati ažurnost podataka bez prevelikog opterećenja servera
