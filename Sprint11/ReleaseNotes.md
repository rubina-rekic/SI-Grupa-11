# Release Notes – PostRoute

---

## Šta smo napravili

PostRoute je web aplikacija koja poštanskim dispečerima pomaže da planiraju dnevne rute punjenja i pražnjenja sandučića, a poštarima da prate i ažuriraju rad na terenu. Ovo je prva funkcionalna verzija sistema, razvijena tokom šest sprintova.

---

## Šta je uključeno u finalnu verziju

**Prijava i upravljanje korisnicima** – Tri uloge (Administrator, Dispečer, Poštar) sa potpuno odvojenim interfejsima. Lozinke su hashirane, neuspjeli pokušaji prijave se bilježe, a korisnik se zaključava nakon pet uzastopnih grešaka. Administrator može kreirati nove poštare i pregledati listu korisnika.

**Upravljanje sandučićima** – Administrator dodaje sandučiće sa GPS koordinatama putem interaktivne mape, postavlja tip, prioritet i radne dane. Svaka izmjena se automatski logira sa informacijom ko je šta i kada promijenio. Postoji pretraga i filtriranje po više kriterija.

**Generisanje i dodjela ruta** – Dispečer odabira datum i jednim klikom dobija optimizovanu rutu koja uzima u obzir prioritete sandučića, radne dane i vremenske prozore dostupnosti. Rutu može ručno preurediti, a sistem automatski ažurira procijenjene dolaske. Kad je zadovoljan, dodjeljuje rutu poštaru.

**Rad na terenu** – Poštar vidi svoju dnevnu rutu sa mapom i tabelom sandučića. Tokom obilaska bilježi status svakog sandučića (obrađen, napunjen, ispraznjen) ili prijavljuje nedostupnost sa razlogom. Sve promjene su vidljive dispečeru.

**Praćenje i izvještaji** – Dispečer prati napredak svih ruta za odabrani dan kroz dashboard koji se osvježava svakih 30 sekundi. Problematične lokacije su naglašene. Završene rute idu u arhivu sa mogućnošću CSV exporta. Dostupna su tri osnovna izvještaja: dnevna realizacija, učinak po poštaru i statistika po tipu sandučića.

---

## Najvažnije funkcionalnosti

Tri stvari koje sistem donosi a prije nisu postojale:

Dispečer više ne planira rute ručno. Algoritam za sekunde generiše redoslijed obilaska koji uzima u obzir gdje su sandučići, koji su prioritetniji i koji dani su radni za svaki sandučić.

Poštar više ne čeka kraj dana da prijavi probleme. Nedostupna lokacija se odmah vidi na dashboardu dispečera, koji može reagovati u realnom vremenu.

Sve ostaje zabilježeno: ko je šta promijenio, kad je koji sandučić bio nedostupan i zašto, koji poštar ima bolji procenat realizacije. Podaci su tu za buduće planiranje.

---

## Poznata ograničenja

Sistem zahtijeva aktivnu internet vezu. Offline rad nije moguć. Prikaz ruta na mapi ovisi o vanjskom OSRM servisu; ako taj servis nije dostupan, mapa neće prikazivati rute. Dashboard se ne osvježava u stvarnom vremenu već svakih 30 sekundi, što znači da promjene na terenu mogu kasniti do pola minute. Algoritam za optimizaciju ruta je heuristički. Za mreže do stotinjak sandučića daje dobre rezultate, ali nije matematički optimalan i ne uzima u obzir saobraćaj.

Na sigurnosnoj strani: demo nalozi (`admin@mail.com`, `dispatcher@mail.com`, `postar@mail.com`) su aktivni i namijenjeni su isključivo demonstraciji. Sistem radi preko HTTP lokalno; za produkcijsku upotrebu obavezan je HTTPS.

---

## Poznati bugovi

Nema bugova koji blokiraju osnovnu upotrebu. Manje anomalije koje smo zabilježili:

Na nekim preglednicima mapa se ne učita na prvom otvaranju. Osvježavanje stranice rješava problem. CSV exporti sadrže sirove vrijednosti za datume i vremena, pa ih ponekad treba ručno formatirati u Excelu. Za rute sa više od stotinjak sandučića, prikaz na mapi može potrajati nekoliko sekundi.

---

## Šta nije dio finalne isporuke

Planirali smo, ali nismo završili proširene filtere u izvještaju o učinku poštara (PBI-050). Osnovni izvještaj postoji, ali napredni parametri filtriranja nisu implementirani. Kompletan QA pass i regresijsko testiranje (PBI-052) su u toku.

Funkcionalnosti koje nisu bile dio ovog MVP-a: mobilna nativna aplikacija, rad bez interneta, push notifikacije, otkazivanje dodijeljene rute, dodavanje sandučića u već aktivnu rutu i integracija sa vanjskim sistemima.

---

## Napomena za demonstraciju

Sistem je spreman za demonstraciju i pilot testiranje na manjoj mreži sandučića. Nije spreman za produkcijsku upotrebu bez dodatne sigurnosne konfiguracije, konkretno: uklanjanje demo naloga, postavljanje HTTPS-a i konfiguracija backup strategije za bazu.
