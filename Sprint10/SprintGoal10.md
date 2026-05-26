# Sprint Goal — Sprint 10

## Sprint broj
10

## Sprint cilj
Uvođenje arhive za retrospektivnu analizu ruta, izvještavanje o učinku po različitim parametrima, te pretraga i filtriranje sandučića — čime se zatvara analitički sloj sistema i administratorima omogućava donošenje odluka zasnovanih na realnim historijskim podacima.

## Ključne stavke koje tim želi završiti
- **PBI-049** — Historija obilazaka i arhiva ruta (US-34, US-35)
- **PBI-050** — Prošireno operativno izvještavanje (US-36, US-37)
- **PBI-051** — Pretraga i filtriranje sandučića (US-38, US-39)

## Rizici i zavisnosti
- PBI-049 zavisi od **US-28** (Završetak rute) — arhiviranje se pokreće tek po finalizaciji rute iz Sprinta 9.
- PBI-050 zavisi od **PBI-049** — izvještaji o učinku koriste podatke iz arhive; bez završene arhive izvještaji nisu mogući.
- PBI-051 zavisi od **US-15** (Prikaz liste sandučića) — pretraga i filtriranje rade nad postojećom bazom sandučića.
- *Otvoreno pitanje:* Da li arhivirane rute trebaju podržati ponovno pokretanje — potrebna odluka product ownera prije implementacije US-34.
- *Otvoreno pitanje:* Da li izvještaj o učinku poštara (US-36) treba uključivati i vizuelne grafikone — u MVP verziji implementiran stubni grafikon, moguće proširenje.