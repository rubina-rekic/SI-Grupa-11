# Sprint Review Summary — Sprint 9

## Sprint broj
9

## Planirani sprint goal
Operacionalizovati sistem na terenu — omogućiti poštaru pristup dodijeljenoj ruti putem responzivnog mobilnog prikaza i ažuriranje statusa sandučića tokom obilaska, uz istovremeni real-time uvid dispečera u napredak rute i generisanje osnovnog dnevnog izvještaja, čime se zatvara puna operativna petlja od planiranja do realizacije.

## Šta je završeno

Tokom Sprinta 9 uspješno su realizovani svi planirani Product Backlog Item-i i pripadajući user storyji. Implementiran je responzivni mobilni prikaz dodijeljene rute za poštare, uključujući interaktivnu mapu, hronološki pregled sandučića i prikaz statusa obilaska.

Omogućeno je ažuriranje statusa sandučića direktno sa terena, pri čemu sistem evidentira vrijeme izmjene, tip izvršene akcije i identitet poštara. Također je implementirana funkcionalnost označavanja nedostupnih lokacija uz navođenje razloga i automatsko obavještavanje dispečera.

Za dispečere je razvijen dashboard za praćenje statusa ruta u realnom vremenu, sa filtriranjem po statusu, poštaru i datumu, kao i pregledom problematičnih lokacija.

Na kraju, implementirano je generisanje osnovnog dnevnog izvještaja sa pregledom realizovanih i nerealizovanih obilazaka te mogućnošću exporta izvještaja u PDF format.

## Šta nije završeno

Svi planirani zadaci za Sprint 9 su uspješno završeni u okviru predviđenog vremenskog perioda. Nije bilo preostalih nezavršenih user storyja niti prenesenih zadataka u naredni sprint.

## Demonstrirane funkcionalnosti ili artefakti

- Responzivni mobilni prikaz dodijeljene rute za poštara
- Interaktivna mapa sa prikazom sandučića i statusa obilaska
- Evidentiranje statusa sandučića („Ispraznjen“, „Napunjen“, „Nedostupan“)
- Real-time ažuriranje statusa ruta i sandučića za dispečera
- Dashboard za praćenje aktivnih ruta i problematičnih lokacija
- Notifikacije za nedostupne lokacije
- Generisanje dnevnog izvještaja za odabrani datum i poštara
- Export izvještaja u PDF format

## Glavni problemi i blokeri

Najveći izazov tokom sprinta odnosio se na sinhronizaciju real-time ažuriranja između poštara i dispečerskog dashboarda, kako bi promjene statusa bile trenutno vidljive bez osvježavanja stranice.

Dodatni izazovi uključivali su prilagođavanje korisničkog interfejsa mobilnim uređajima različitih dimenzija, kao i pravilno upravljanje pristupnim pravima kako bi poštari mogli pristupiti isključivo vlastitim rutama.

## Ključne odluke donesene u sprintu

- Odlučeno je da se mobilni prikaz implementira kao responzivni web interfejs umjesto zasebne mobilne aplikacije, kako bi razvoj bio brži i jednostavniji za održavanje.
- Uvedeno je vizuelno razlikovanje statusa sandučića korištenjem boja i ikona radi lakšeg praćenja napretka obilaska.
- Definisano je da se status već obrađenog sandučića ne može mijenjati bez intervencije dispečera, čime se osigurava integritet podataka.
- Usvojena je odluka da se problematične i nedostupne lokacije odmah prikazuju dispečeru kroz real-time notifikacije.

## Povratna informacija Product Ownera

Product Owner je pohvalio kvalitet implementiranih funkcionalnosti i uspješnu realizaciju kompletnog sprint cilja, te je timu dodijelio maksimalan broj bodova. Posebno je istaknuta vrijednost povezivanja planiranja ruta sa stvarnim terenskim izvršenjem i mogućnost praćenja rada u realnom vremenu.

Ipak, naglašeno je da se od tima u narednom sprintu očekuje veći obim realizovanog posla, uz preporuku da se implementira najmanje 10 user storyja.

## Zaključak za naredni sprint

Sprint 9 uspješno je zatvorio osnovni operativni tok sistema — od planiranja i dodjele ruta do realizacije obilaska i izvještavanja. U narednom sprintu fokus će biti na proširenju funkcionalnosti izvještavanja, unapređenju analitike i historije obilazaka, kao i dodatnoj optimizaciji korisničkog iskustva za dispečere i poštare.