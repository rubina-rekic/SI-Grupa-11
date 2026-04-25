# Sprint Goal – Sprint 5

## Sprint broj
5

## Sprint cilj
Implementirati kompletan autentifikacijski modul — kreiranje korisničkih računa,
prijavu, odjavu i kontrolu pristupa po ulozi — kako bi sistem bio zaštićen i
spreman za dalji razvoj funkcionalnosti u narednim sprintovima. Pored toga,
uspostaviti Decision Log i AI Usage Log kao obavezne evidencije AI-enabled faze.

## Ključne stavke koje tim želi završiti
- PBI-011: Kreiranje korisničkog računa poštara (US-01, US-02, US-03)
- PBI-012: Prijava korisnika — osnovna prijava, rukovanje greškama, obavezna
  promjena lozinke pri prvoj prijavi (US-04, US-05, US-06)
- PBI-013: Sigurna odjava iz sistema (US-07)
- PBI-014: Uloge i pristup po ulozi — definisanje uloga, personalizovani
  dashboard, restrikcija neovlaštenog pristupa (US-08, US-09, US-10)
- PBI-040: Uspostava Decision Loga
- PBI-041: Uspostava AI Usage Loga

## Rizici i zavisnosti
- US-04 (prijava) striktno zavisi od US-01 (kreiranje računa) — mora biti
  implementiran prvi
- US-06 (promjena lozinke pri prvoj prijavi) zavisi od US-02 (validacija lozinke)
  — ista pravila jačine lozinke moraju biti konzistentna
- US-09 i US-10 (dashboard i restrikcija pristupa) zavise od US-08 (definisanje
  uloga) — Role ID mora biti implementiran u bazi prije ovih stavki