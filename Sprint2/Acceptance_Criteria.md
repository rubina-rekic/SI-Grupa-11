# Acceptance criteria

---

## PBI-011 Kreiranje korisničkog računa poštara

### US-01
*Kao administrator, želim kreirati korisnički račun za poštara unosom osnovnih podataka, emaila/korisničkog imena i inicijalne lozinke, kako bi poštar mogao pristupiti sistemu.*

- **Kada** administrator unese sve obavezne podatke, **ako** klikne na dugme za kreiranje računa, **tada** sistem mora kreirati korisnički račun.
- **Sistem mora** sačuvati podatke u bazi.
- **Sistem ne smije** dozvoliti kreiranje računa ako nedostaju obavezna polja.
- **Korisnik treba** dobiti potvrdu o uspješnom kreiranju.

### US-02
*Kao administrator, želim da sistem validira jedinstvenost emaila/korisničkog imena i snagu inicijalne lozinke, kako bi se spriječilo kreiranje neispravnih ili nesigurnih računa.*

- **Kada** administrator unese već postojeći email/username, **ako** pokuša kreirati račun, **tada** sistem mora odbiti unos i prikazati poruku o grešci.
- **Sistem mora** provjeravati jedinstvenost emaila/username-a.
- **Kada** lozinka ne zadovoljava sigurnosne kriterije, **ako** administrator pokuša sačuvati podatke, **tada** sistem mora odbiti unos.
- **Sistem ne smije** dozvoliti slabu lozinku.

### US-03
*Kao administrator, želim dobiti jasnu potvrdu o uspješnom kreiranju računa ili poruku o grešci (npr. zauzet email), kako bih mogao završiti unos i dostaviti kredencijale poštaru.*

- **Kada** administrator potvrdi kreiranje, **ako** je račun uspješno kreiran, **tada** sistem mora prikazati potvrdu.
- **Kada** dođe do greške prilikom obrade, **ako** sistem ne može izvršiti akciju, **tada** mora prikazati odgovarajuću poruku.
- **Sistem ne smije** ostaviti korisnika bez feedbacka.

---

## PBI-012 Prijava korisnika

### US-04
*Kao registrovani korisnik, želim se prijaviti na sistem koristeći kredencijale koje mi je dodijelio administrator, kako bih pristupio funkcionalnostima aplikacije.*

- **Kada** korisnik unese ispravne kredencijale, **ako** klikne na prijavu, **tada** sistem mora omogućiti pristup.
- **Sistem mora** preusmjeriti korisnika na dashboard.
- **Sistem ne smije** dozvoliti prijavu bez unosa podataka.

### US-05
*Kao korisnik, želim da me sistem obavijesti ako unesem pogrešne kredencijale, kako bih znao da trebam ponoviti unos ili resetovati inicijalnu lozinku.*

- **Kada** korisnik unese pogrešne kredencijale, **ako** klikne na prijavu, **tada** sistem mora prikazati poruku o grešci.
- **Sistem ne smije** dozvoliti prijavu sa praznim poljima.

### US-06
*Kao poštar koji se prvi put prijavljuje, želim biti obavezan promijeniti inicijalnu lozinku prije nastavka rada, kako bih zaštitio svoj korisnički račun.*

- **Kada** se korisnik prvi put prijavi, **ako** unese inicijalnu lozinku, **tada** sistem mora zahtijevati promjenu lozinke.
- **Sistem ne smije** dozvoliti nastavak rada bez promjene lozinke.

---

## PBI-013 Odjava korisnika

### US-07
*Kao prijavljeni korisnik, želim se moći odjaviti iz sistema u bilo kojem trenutku, kako bih osigurao da niko drugi ne može pristupiti mojim podacima nakon završetka rada.*

- **Kada** korisnik klikne na odjavu, **ako** potvrdi akciju, **tada** sistem mora prekinuti sesiju.
- **Sistem mora** preusmjeriti korisnika na login stranicu.
- **Sistem ne smije** dozvoliti pristup aplikaciji bez ponovne prijave.

---

## PBI-014 Uloge i pristup

### US-08
*Kao administrator, želim definisati različite nivoe pristupa (Administrator, Dispečer, Poštar), kako bih osigurao da svaki korisnik vidi samo relevantne podatke.*

- **Kada** administrator odabere nivo pristupa, **ako** dodijeli ulogu korisniku, **tada** sistem mora sačuvati ulogu.
- **Sistem mora** omogućiti različite nivoe pristupa.
- **Sistem ne smije** dozvoliti neautorizovan pristup funkcijama drugih uloga.

### US-09
*Kao prijavljeni korisnik, želim da me sistem preusmjeri na radnu površinu (dashboard) prilagođenu mojoj ulozi, kako bih odmah mogao započeti sa svojim specifičnim zadacima.*

- **Kada** se korisnik prijavi, **ako** sistem prepozna ulogu, **tada** sistem mora prikazati odgovarajući dashboard.
- **Sistem mora** omogućiti prikaz funkcionalnosti isključivo po ulozi.
- **Korisnik treba** vidjeti samo relevantne opcije.

### US-10
*Kao korisnik sa ograničenim pravima, želim dobiti poruku o zabrani pristupa ako pokušam otvoriti stranicu za koju nemam ovlaštenje, kako bi se spriječila neovlaštena manipulacija podacima.*

- **Kada** korisnik pokuša pristupiti zabranjenoj stranici, **ako** sistem detektuje nedostatak dozvole, **tada** sistem mora odbiti pristup.
- **Sistem mora** prikazati poruku o zabrani.
- **Sistem ne smije** dozvoliti izvršenje akcije na neovlaštenoj stranici.

---

## PBI-015 Dodavanje poštara

### US-11
*Kao administrator, želim unijeti podatke o novom poštaru (ime, prezime, kontakt telefon, ID broj), kako bih ga uključio u bazu aktivnih uposlenika na terenu.*

- **Kada** administrator unese lične podatke, **ako** klikne na dugme za spasavanje, **tada** sistem mora sačuvati poštara.
- **Sistem mora** omogućiti unos svih polja (ime, prezime, telefon, ID).
- **Sistem ne smije** dozvoliti prazna polja pri registraciji poštara.

### US-12
*Kao administrator, želim da sistem provjeri da li poštar sa istim ID brojem već postoji, kako bi se izbjegli dupli unosi i konfuzija pri dodjeli ruta.*

- **Kada** administrator unese ID, **ako** ID već postoji u bazi, **tada** sistem mora odbiti unos.
- **Sistem mora** provjeravati jedinstvenost ID-a.
- **Korisnik treba** dobiti poruku o grešci koja objašnjava da je ID zauzet.

---

## PBI-016 Pregled poštara

### US-13
*Kao administrator ili dispečer, želim vidjeti listu svih registrovanih poštara sa njihovim trenutnim statusom, kako bih znao koga mogu zadužiti za nove zadatke.*

- **Kada** korisnik otvori listu poštara, **ako** sistem učita podatke, **tada** sistem mora prikazati sve poštare.
- **Sistem mora** omogućiti pregled osnovnih podataka (ime, prezime, kontakt telefon i status poštara).
- **Korisnik treba** vidjeti status poštara (dostupan ili zauzet).

---

## PBI-017 Dodavanje sandučića

### US-14
*Kao administrator ili dispečer, želim dodati novi poštanski sandučić u sistem unoseći njegovu adresu i precizne GPS koordinate, kako bi on bio vidljiv u evidenciji i mogao biti korišten za planiranje ruta.*

- **Kada** administrator unese geografsku lokaciju, **ako** se unesu koordinate, **tada** sistem mora sačuvati lokaciju sandučića.
- **Sistem mora** omogućiti unos GPS podataka.
- **Sistem ne smije** dozvoliti unos nevalidnih koordinata.

### US-15
*Kao administrator ili dispečer, želim pri unosu definisati tip sandučića i osnovne podatke o njemu, kako bi zapis bio potpun i spreman za dalju obradu u sistemu.*

- **Kada** administrator popunjava formu, **ako** se odabere tip sandučića, **tada** sistem mora sačuvati podatak o tipu.
- **Sistem mora** omogućiti izbor tipa iz predefinisanih opcija.
- **Sistem ne smije** dozvoliti nepostojeći tip.

---

## PBI-018 Izmjena sandučića

### US-16
*Kao administrator, želim izmijeniti lokaciju, tip, prioritet i druge podatke postojećeg sandučića, kako bih osigurao da baza podataka odgovara stvarnom stanju na terenu.*

- **Kada** administrator otvori formu za uređivanje, **ako** se izmjene podaci, **tada** sistem mora sačuvati promjene.
- **Sistem mora** omogućiti uređivanje svih podataka o sandučiću.
- **Korisnik treba** vidjeti ažurirane informacije odmah nakon spasavanja.

---

## PBI-019 Pregled sandučića

### US-17
*Kao administrator ili dispečer, želim vidjeti listu svih evidentiranih sandučića kroz jednostavnu tabelu ili listu, kako bih imao pregled nad stanjem i raspoloživim tačkama za planiranje.*

- **Kada** se otvori lista sandučića, **ako** sistem dohvati podatke, **tada** sistem mora prikazati sandučiće.
- **Sistem mora** prikazati adresu, tip i prioritet sandučića.
- **Korisnik treba** vidjeti listu svih evidentiranih sandučića u bazi.

---

## PBI-020 Prioritet sandučića

### US-18
*Kao administrator, želim postaviti ili izmijeniti prioritet za pražnjenje/punjenje sandučića, kako bi sistem znao koje lokacije imaju veći operativni značaj.*

- **Kada** administrator pristupi opcijama sandučića, **ako** se odabere prioritet, **tada** sistem mora sačuvati tu vrijednost.
- **Sistem mora** omogućiti izbor prioriteta (npr. Visok, Srednji, Nizak).
- **Korisnik treba** vidjeti trenutni prioritet na listi i u detaljima.

---

## PBI-022 Generisanje rute

### US-22
*Kao dispečer, želim pokrenuti algoritam za automatsko generisanje dnevne rute za odabranog poštara, kako bih dobio prijedlog obilaska zasnovan na lokacijama i prioritetima sandučića.*

- **Kada** dispečer odabere parametre, **ako** se pokrene generisanje, **tada** sistem mora kreirati rutu.
- **Sistem mora** uzeti u obzir koordinate i prioritete pri računanju redoslijeda.
- **Kada** ne postoje sandučići u sistemu, **ako** se pokuša kreiranje, **tada** sistem ne smije generisati rutu.

---

## PBI-023 Dodjela rute

### US-25
*Kao dispečer, želim dodijeliti generisanu rutu konkretnom poštaru, kako bi on dobio svoj dnevni zadatak za izvršenje.*

- **Kada** je ruta spremna, **ako** se ruta dodijeli odabranom radniku, **tada** sistem mora povezati poštara i rutu.
- **Sistem mora** omogućiti izbor poštara sa liste dostupnih radnika.
- **Korisnik treba** dobiti potvrdu o uspješno izvršenoj dodjeli.

---

## PBI-024 Pregled rute

### US-23
*Kao dispečer, želim pregledati detalje generisane rute, uključujući redoslijed obilaska, uključene sandučiće i osnovne informacije o ruti, kako bih mogao provjeriti njenu logičnost prije dodjele.*

- **Kada** se u pregledu odabere ruta, **ako** se otvori ruta, **tada** sistem mora prikazati detalje.
- **Sistem mora** prikazati redoslijed obilaska i listu uključenih sandučića.
- **Korisnik treba** vidjeti osnovne informacije o ruti (dužina, broj tačaka) prije dodjele.

---

## PBI-025 Izmjena rute

### US-24
*Kao dispečer, želim imati mogućnost ručne izmjene redoslijeda obilaska unutar generisane rute, kako bih uvažio nepredviđene okolnosti na terenu.*

- **Kada** dispečer mijenja raspored, **ako** se promijeni redoslijed tačaka, **tada** sistem mora sačuvati izmjene.
- **Sistem mora** omogućiti ručnu izmjenu redoslijeda tačaka unutar rute (npr. drag & drop).
- **Kada** dispečer sačuva izmjene, **ako** potvrdi akciju, **tada** sistem mora prikazati novi redoslijed obilaska.

---

## PBI-026 Mobilni prikaz

### US-26
*Kao poštar, želim vidjeti svoju dodijeljenu rutu preko responzivnog web interfejsa, kako bih na mobilnom uređaju imao jasan pregled dnevnog zadatka.*

- **Kada** se poštar prijavi, **ako** se otvori mobilna aplikacija, **tada** sistem mora prikazati aktivnu rutu.
- **Sistem mora** biti responzivan (prilagođen ekranu telefona).
- **Korisnik treba** vidjeti sve tačke obilaska na mapi ili listi.

---

## PBI-027 Status sandučića

### US-28
*Kao poštar, želim promijeniti status sandučića tokom obilaska, kako bi dispečer imao informaciju o progresu rute u realnom vremenu.*

- **Kada** poštar dođe do lokacije, **ako** se promijeni status sandučića (npr. u "Ispražnjeno"), **tada** sistem mora sačuvati promjenu.
- **Sistem mora** omogućiti promjenu statusa jednim klikom.
- **Korisnik treba** vidjeti ažuriran status na svom interfejsu.

---

## PBI-028 Nedostupna lokacija

### US-29
*Kao poštar, želim evidentirati da određena lokacija nije bila dostupna tokom obilaska, kako bi problem bio zabilježen i vidljiv dispečeru.*

- **Kada** poštar ne može pristupiti tački, **ako** se označi lokacija kao nedostupna, **tada** sistem mora evidentirati problem.
- **Sistem mora** omogućiti unos napomene (razlog nedostupnosti).
- **Korisnik treba** vidjeti status "Nedostupno" u svojoj listi zadataka.

---

## PBI-029 Praćenje rute

### US-30
*Kao dispečer, želim imati uvid u to koji su sandučići obrađeni, preskočeni ili problematični, kako bih mogao pratiti status izvršenja dodijeljene rute.*

- **Kada** dispečer prati rad na terenu, **ako** se učita progres, **tada** sistem mora prikazati statuse u realnom vremenu.
- **Sistem mora** omogućiti vizuelni pregled progresa rute.
- **Korisnik treba** vidjeti tačno koji su sandučići obrađeni, preskočeni ili problematični.

---

## PBI-030 Izvještaj

### US-31
*Kao administrator ili dispečer, želim generisati osnovni dnevni izvještaj o realizovanim i nerealizovanim obilascima, kako bih imao sažet pregled učinka za dati dan.*

- **Kada** se odabere opcija za izvještavanje, **ako** se generiše izvještaj za odabrani datum, **tada** sistem mora prikazati rezultate.
- **Sistem mora** prikazati broj realizovanih i nerealizovanih obilazaka.
- **Korisnik treba** dobiti sažetak sa osnovnim statistikama za odabrani dan.
