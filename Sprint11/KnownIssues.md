# Known Issues / Limitations

## 1. Poznati bugovi

U trenutnoj verziji nisu evidentirani poznati bugovi koji blokiraju osnovno korištenje sistema. Ipak, prije finalne demonstracije preporučuje se kratka ručna provjera glavnih korisničkih tokova: prijava korisnika, rad sa rutama, prikaz mape i arhiviranje završenih ruta.

Map/routing dio aplikacije zavisi od browsera i vanjskih map servisa, pa ga je potrebno provjeriti u stvarnom okruženju nakon deploymenta.

## 2. Tehnička ograničenja

* Sistem nije predviđen za offline rad. Za normalno korištenje potrebno je da su dostupni frontend, backend, baza i vanjski map servisi.
* Lokalno pokretanje zahtijeva ispravnu konfiguraciju baze i development postavki, uključujući connection string koji se ne čuva direktno u repozitoriju.
* Frontend komunicira sa backendom preko konfigurirane API adrese. Ako `VITE_API_BASE_URL` nije pravilno postavljen u deployment okruženju, frontend se može učitati, ali neće pravilno komunicirati sa backendom.
* Prikaz promjena na dispečerskom dashboardu nije implementiran kao pravi real-time push sistem, nego se stanje periodično osvježava.
* Automatski testovi pokrivaju dio backend i frontend logike, ali kompletan end-to-end tok kroz browser, posebno dio sa mapama i rutama, nije potpuno pokriven automatizovanim testovima.

## 3. Sigurnosna ograničenja

* Sistem koristi demo korisnike za potrebe testiranja i prezentacije. Takav pristup je prihvatljiv za finalnu studentsku isporuku, ali nije dovoljan za produkcijsko okruženje bez dodatnog podešavanja korisnika i lozinki.
* Produkcijska sigurnost zavisi od ispravno postavljenih environment varijabli i hosting secret-a, posebno za connection string, CORS postavke i seeding.
* Potrebno je dodatno provjeriti da su svi endpointi zaštićeni očekivanim rolama i pravilima pristupa, posebno nakon izmjena ili budućeg proširenja sistema.

## 4. Nedovršene ili djelimično završene funkcionalnosti

* U navigaciji postoje ekrani koji su više pripremljeni kao osnova za budući rad nego kao potpuno završeni moduli, npr. statistika, podešavanja i dodatni radnički pregledi.
* Prijava problema na terenu postoji u ograničenom obliku kroz postojeći tok rada, ali nije razvijena kao potpuno zaseban i detaljan modul.
* Statusi ruta podržavaju više mogućih stanja, ali nisu svi statusni tokovi jednako dostupni kroz korisnički interfejs.
* Optimizacija rute je prilagođena MVP verziji sistema. Koristi jednostavniji pristup i ne uzima u obzir realne saobraćajne uslove, gužve ili radove na putu.

## 5. Pretpostavke sistema

* Pretpostavlja se da su backend, frontend i PostgreSQL baza pravilno pokrenuti i povezani.
* Pretpostavlja se da su migracije uspješno primijenjene prije korištenja sistema.
* Pretpostavlja se da postoje seed/demo podaci potrebni za testiranje i prezentaciju.
* Pretpostavlja se da su vanjski servisi za mapu, geocoding i rutiranje dostupni u trenutku korištenja.
* Pretpostavlja se korištenje modernog browsera i ispravna konfiguracija CORS-a između frontend i backend aplikacije.

## 6. Šta ne treba predstavljati kao potpuno završeno

* Deployment ne treba predstavljati kao potpuno nezavisan od konfiguracije, jer zavisi od ispravno postavljenih varijabli i hosting postavki.
* Map/routing dio ne treba predstavljati kao potpuno automatizovano testiran, jer dio ponašanja zavisi od browsera i vanjskih servisa.
* Dashboard osvježavanje ne treba predstavljati kao pravi real-time sistem, nego kao periodično osvježavanje podataka.
* Placeholder ili djelimično pripremljene ekrane ne treba predstavljati kao završene funkcionalnosti.

Ova lista ne znači da sistem nije upotrebljiv. Ona predstavlja realno stanje finalne isporuke i stvari koje treba imati na umu pri evaluaciji ili nastavku razvoja.
::: 
