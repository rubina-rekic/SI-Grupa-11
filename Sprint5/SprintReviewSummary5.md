# Sprint Review Summary — Sprint 5

## Sprint broj
5

## Planirani sprint goal
Implementirati autentifikacijski modul — kreiranje korisničkih računa, prijavu, odjavu i kontrolu pristupa po ulozi, te uspostaviti Decision Log i AI Usage Log.

## Šta je završeno
- PBI-011: Kreiranje korisničkog računa poštara (US-01, US-02, US-03)
- PBI-012: Prijava korisnika (US-04, US-05, US-06)
- PBI-013: Sigurna odjava iz sistema (US-07)
- PBI-014: Uloge i pristup po ulozi (US-08, US-09, US-10)
- PBI-040: Uspostava Decision Loga
- PBI-041: Uspostava AI Usage Loga

## Šta nije završeno
Sve planirane stavke su završene.

## Demonstrirane funkcionalnosti ili artefakti
- Kreiranje korisničkog računa poštara s validacijom i indikatorom jačine lozinke
- Prijava sa JWT autentifikacijom i obaveznom promjenom lozinke pri prvoj prijavi
- Sigurna odjava s brisanjem sesije i zaštitom ruta
- Personalizovani dashboard prema ulozi korisnika
- CI/CD pipeline sa deploymentom na Netlify i Render

## Glavni problemi i blokeri
- Usklađivanje Docker porta (5432 vs 5433) između članova tima
- Postavljanje produkcijskog deploya na Render + Netlify + Neon zahtijevalo je dodatno vrijeme
- Migracije nisu bile odmah pokrenute na produkcijskoj bazi nakon prvog deploymenta

## Ključne odluke donesene u sprintu
- JWT stateless autentifikacija umjesto session-based (DEC-013)
- BCrypt za hashiranje lozinki umjesto punog ASP.NET Identity (DEC-002)
- Docker Compose za lokalni razvoj radi konzistentnosti između članova tima (DEC-011)

## Povratna informacija Product Ownera
Pozitivna — sve planirane funkcionalnosti demonstrirane i prihvaćene.

## Zaključak za naredni sprint
Tim prelazi na Sprint 6 — dodavanje poštara, pregled liste poštara i upravljanje poštanskim sandučićima (PBI-015, PBI-016, PBI-017, PBI-018, PBI-019).