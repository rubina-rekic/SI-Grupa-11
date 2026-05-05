# Sprint Retrospective Summary — Sprint 5

## Šta je išlo dobro
- Tim je uspješno isporučio sve planirane stavke unutar jednog sprinta
- Koordinacija između članova tima bila je efikasna — PR procesi su poštovani i branching strategija se primjenjivala dosljedno
- Postavljanje CI/CD pipelinea u ovom sprintu omogućit će brži i pouzdaniji razvoj u narednim sprintovima
- Decision Log i AI Usage Log su uspostavljeni i redovno ažurirani tokom sprinta

## Šta nije išlo dobro
- Postavljanje produkcijskog deploymenta oduzelo je više vremena nego što je planirano — konfiguracija Neon, Render i Netlify servisa zahtijevala je dodatno usklađivanje između članova tima
- Migracije na produkcijskoj bazi nisu bile pokrenute odmah nakon prvog deploymenta, što je uzrokovalo kratkoročne probleme s funkcionalnošću online aplikacije

## Šta treba promijeniti
- Deployment zadatke planirati ranije u sprintu, a ne ostavljati za kraj sedmice
- Bolje dokumentovati setup korake za nove environment-e kako bi svi članovi tima mogli samostalno pristupiti bazi i provjeriti stanje aplikacije

## Konkretne akcije koje tim uvodi u Sprint 6
- Svaki član tima provjerava da li mu lokalno okruženje radi ispravno na početku sprinta prije nego počne implementaciju
- Deployment provjera se planira kao posebna stavka u sprint backlogu, ne kao usputni zadatak