# 📋 Product Vision 

---

## Naziv Projekta

Sistem za optimizaciju ruta punjenja i pražnjenja poštanskih sandučića

---

## Problem Koji sistem rješava

Trenutni proces obilazaka poštanskih sandučića (za njihovo punjenje i pražnjenje) u velikoj mjeri se oslanja na iskustvo i lično poznavanje terena od strane poštara ili dispečera. Rute se rijetko (gotovo nikada) prilagođavaju stvarnim potrebama na terenu, što dovodi do neefikasnog korištenja vremena i resursa. Ne uzimaju se u obzir faktori poput prioriteta sandučića, vremenskih ograničenja ili specifičnih radnih pravila, pa može doći do kašnjenja ili nepotrebnih obilazaka.

Dodatni problem predstavlja to što ne postoji jednostavan način za brzu promjenu planiranih ruta u slučaju nepredviđenih situacija, kao što su nedostupne lokacije ili promjene prioriteta. Također, praćenje realizacije obilazaka nije dovoljno pregledno, što otežava pravovremeno reagovanje i donošenje operativnih odluka.


## Ciljni korisnici

* **Dispečeri / Operateri:** Planiraju obilazak sandučića, prave dnevne rute i prate šta se dešava na terenu. U slučaju problema, oni reaguju i prilagođavaju plan.
* **Poštari / Terenski radnici:** Izvršavaju obilazak na terenu. Kroz sistem dobijaju isplanirane rute i informacije o prioritetima, te bilježe status (da li je sandučić obrađen, preskočen itd.).
* **Sistemski administratori:** Zaduženi za održavanje sistema i podataka — unose i ažuriraju informacije o sandučićima, njihovim lokacijama, pravilima rada i ostale tehničke parametre.
* **Menadžment / Nadzor:** Ne učestvuju direktno u planiranju operacija, ali koriste sistem za pregled operativnih izvještaja, praćenje efikasnosti resursa i donošenje strateških odluka.

## Vrijednost sistema

Sistem štedi vrijeme i smanjuje operativne troškove tako što zamjenjuje neefikasno manuelno planiranje pametnom optimizacijom ruta, uzimajući u obzir geografsku lokaciju sandučića i prioritete. Operativnom osoblju omogućava brzo prilagođavanje ruta u slučaju nepredviđenih situacija na terenu. Menadžment dobija jasnu sliku o statusu obilazaka i detaljne izvještaje, što rezultira efikasnijim korištenjem resursa, smanjenjem zakašnjelih ili propuštenih lokacija i poboljšanjem kvaliteta usluge.


## Scope MVP verzije

* **Evidencija poštanskih sandučića:** Unos, izmjena i pregled lokacija sandučića sa radnim pravilima i prioritetima.
* **Planiranje i optimizacija obilazaka:** Mogućnost da dispečer automatski generiše dnevnu rutu za poštara. Brza optimizacija će se vršiti na serverskoj strani (.NET) korištenjem osnovnih algoritama teorije grafova primijenjenih na putnu mrežu.
* **Upravljanje rutama:** Dispečer može dodijeliti rutu određenom poštaru i po potrebi ručno prilagoditi redoslijed obilaska kroz brzi web panel.
* **Mobilni prikaz za terenski rad:** Prilagodljiva (responzivna) web aplikacija preko koje poštar, koristeći mobilni browser, prati redoslijed obilaska i mijenja status sandučića (recimo "ispražnjen", "nedostupna lokacija" itd.).
* **Praćenje i izvještavanje:** Dispečerski pregled statusa obilazaka u realnom vremenu i generisanje osnovnih dnevnih izvještaja o realizaciji na terenu.

## Šta ne ulazi u MVP

* **Nativna mobilna aplikacija:** Ne razvija se specifična iOS ili Android aplikacija, već se sistem isključivo oslanja na responzivni web interfejs prilagođen mobilnim uređajima.
* **Hardversko GPS praćenje kretanja:** Obzirom da poštari koriste web aplikaciju umjesto nativne, sistem ne prati fizičko kretanje poštara uživo putem senzora na uređaju, već evidentira isključivo trenutak promjene statusa na samoj lokaciji sandučića.
* **Dinamičko re-rutiranje uživo:** Sistem u MVP fazi ne uzima u obzir trenutno stanje u saobraćaju (gužve, radovi na putu) za re-kalkulaciju ruta u realnom vremenu.
* **Napredni algoritmi rutiranja:** Upotreba složenih metaheurističkih algoritama ostaje za nekad kasnije.
* **Integracija sa vanjskim sistemima:** Nema povezivanja sa eksternim sistemima (za obračun plata) ili naprednim GIS servisima.

## Ključna ograničenja i pretpostavke

### 1. Tehnička ograničenja
* **Web-bazirani pristup:** Sistem zahtijeva web preglednik za rad na svim uređajima.
* **Zavisnost o internet konekciji:** Podrazumijeva se da poštari na terenu imaju stalnu internet konekciju jer se ne razvija offline mod.
* **Preciznost mapa:** Sistem se oslanja na besplatne podatke servisa OpenStreetMap, te preciznost zavisi od ažurnosti tog servisa.

### 2. Poslovna i funkcionalna ograničenja
* **Statističko praćenje:** Progres se bilježi ručnom promjenom statusa, a ne automatskim trackingom.
* **Fiksne rute:** Planirane rute su statične za jedan radni dan nakon što se jednom izgenerišu.

### 3. Pretpostavke
* **Dostupnost hardvera:** Svaki poštar posjeduje pametni telefon sa pristupom internetu.
* **Ispravnost podataka:** Početni podaci o lokacijama sandučića koje unosi administrator moraju biti u ispravnom formatu.
* **Tehnička obučenost:** Korisnici posjeduju osnovnu pismenost za rad sa web interfejsima.
