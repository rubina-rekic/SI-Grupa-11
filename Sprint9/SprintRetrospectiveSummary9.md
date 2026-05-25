# Sprint Retrospective Summary — Sprint 9

## Šta je išlo dobro

- Tim je uspješno isporučio svih pet planiranih stavki sprinta (PBI-026, PBI-027, PBI-028, PBI-029, PBI-030), čime je zatvorena puna operativna petlja od planiranja rute do pisane evidencije realizacije
- Mobilni prikaz rute (PBI-026) i ažuriranje statusa sandučića (PBI-027) implementirani su s real-time ažuriranjem i zaštitom pristupa po ulozi, bez zastoja — zahvaljujući jasnim acceptance kriterijima iz prethodnih sprintova
- Dispečerski dashboard za praćenje ruta (PBI-029) uspješno integriše real-time notifikacije i filtere, čime se značajno smanjuje potreba za telefonskom koordinacijom između poštara i dispečera
- Dnevni izvještaj (PBI-030) implementiran je s export funkcionalnosti u PDF i vizualnim upozorenjima za realizaciju ispod 80%, što zatvara traceability operativnih aktivnosti
- Deployment je uspješno popravljen — aplikacija je premještena na novi URL postrouteapp.netlify.com nakon što je istekao token za prethodni deployment, čime je osiguran stabilan pristup svim korisnicima

## Šta nije išlo dobro

- Frontend komponente koje ovise o Leaflet mapama i DOM API-ju (interaktivna mapa, pin promjena boje, real-time prikaz) ostaju bez automatskih testova zbog ograničenja jsdom okruženja
- Sprint je bio tehnički zahtjevniji od prethodnih (mobilni prikaz, real-time komunikacija, PDF export) što je povećalo pritisak na tim, posebno prema kraju sprinta

## Šta treba promijeniti

- Pokrenuti istraživanje i prototip Playwright e2e testova za frontend scenarije koji uključuju Leaflet i DOM interakcije
- Za buduće sprintove s višim tehničkim rizikom (real-time funkcionalnosti, integracije) uvesti kraće interne provjere napretka (mid-sprint sync) kako bi se blokeri otklonili ranije

## Konkretne akcije koje tim uvodi u Sprint 10

- Posvetiti dedicirani task unutar Sprint 10 backloga postavljanju Api.Tests projekta s minimalnim pokrivanjem kontrolera za sve PBI-je implementirane do sada
- Istražiti i dokumentovati Playwright setup za e2e testiranje frontend komponenti zavisnih od DOM API-ja — cilj je imati funkcionalni prototip do kraja sprinta
- Sprint 10 fokusirati na historiju obilazaka i arhivu ruta (PBI-049) te prošireno operativno izvještavanje (PBI-050), uz osiguranje da svaki novi PBI dolazi s unit i integracijskim testovima od prvog dana implementacije
