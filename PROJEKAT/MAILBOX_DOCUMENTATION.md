# Dokumentacija: Unos tipa i osnovnih podataka sandučića (US-15)

## Pregled

Ova funkcionalnost omogućava administratorima i dispečerima da unose nove poštanske sandučiće u sistem sa svim potrebnim podacima.

## Implementirane funkcionalnosti

### Backend

#### Entiteti
- **Mailbox**: Glavni entitet za sandučiće
- **MailboxType**: Enum za tipove sandučića

#### API Endpointi
- `POST /api/mailboxes` - Kreiranje novog sandučića
- `GET /api/mailboxes/{id}` - Dobavljanje sandučića po ID
- `GET /api/mailboxes` - Lista svih sandučića
- `GET /api/mailboxes/check-serial-number/{serialNumber}` - Provjera jedinstvenosti serijskog broja

#### Validacija
- Serijski broj mora biti jedinstven
- Latitude: -90 do 90
- Longitude: -180 do 180
- Kapacitet: > 0
- Godina instalacije: 1900 do trenutna godina + 10

### Frontend

#### Stranica
- **CreateMailboxPage**: `/admin/mailboxes/new`
- Dostupna samo za Administrator rolu

#### Polja forme
1. **Tip sandučića** (dropdown)
   - Zidni (mali)
   - Samostojeći (veliki)
   - Unutrašnji (stambene zgrade)
   - Specijalni (prioritetni)

2. **Serijski broj** (text)
   - Obavezno polje
   - Validacija jedinstvenosti u realnom vremenu

3. **Adresa** (text)
   - Obavezno polje
   - Maksimalno 200 karaktera

4. **Koordinate**
   - Latitude (decimal, -90 do 90)
   - Longitude (decimal, -180 do 180)

5. **Kapacitet** (number)
   - Obavezno polje
   - Mora biti > 0

6. **Godina instalacije** (number)
   - Obavezno polje
   - 1900 do trenutna godina + 10

7. **Napomene** (textarea, opcionalno)
   - Maksimalno 500 karaktera

#### Vizuelne indikacije
- Boja pine na mapi se mijenja prema tipu sandučića:
  - Plava: Zidni (mali)
  - Zelena: Samostojeći (veliki)
  - Žuta: Unutrašnji (stambene zgrade)
  - Crvena: Specijalni (prioritetni)

## Acceptance Criteria

✅ **Dropdown tip sandučića**
- Implementirane sve 4 opcije
- Polje je obavezno
- Nema mogućnost ručnog unosa

✅ **Obavezna polja**
- Serijski broj, Adresa, Koordinate, Tip, Kapacitet, Godina instalacije
- Sva polja su validirana

✅ **Jedinstvenost serijskog broja**
- Backend validacija
- Frontend validacija u realnom vremenu
- Toast obavijest pri duplikatu

✅ **Vizuelna indikacija na mapi**
- Promjena boje pine prema tipu
- Prikaz u formi za potvrdu

## Korištenje

### 1. Pristup stranici
```
URL: /admin/mailboxes/new
Potrebna uloga: Administrator
```

### 2. Unos podataka
1. Odaberite tip sandučića iz dropdown menija
2. Unesite serijski broj (sistem će provjeriti jedinstvenost)
3. Unesite adresu
4. Unesite GPS koordinate
5. Postavite kapacitet
6. Odaberite godinu instalacije
7. Dodajte napomene (opciono)

### 3. Snimanje
- Kliknite "Sačuvaj sandučić"
- Uspješno snimanje prikazuje green toast obavijest
- Forma se automatski čisti nakon snimanja

## Error Handling

### Frontend validacija
- Sva polja su validirana prije slanja
- Prikaz grešaka ispod polja
- Onemogućeno dugme snimanja dok su greške prisutne

### Backend validacija
- Provjera jedinstvenosti serijskog broja
- Validacija raspona koordinata
- Vraćanje HTTP 409 za duplikate

### Toast obavijesti
- **Success**: "Sandučić [serijski broj] uspješno dodan!"
- **Error**: "Sandučić sa ovim serijskim brojem već postoji"
- **Error**: "Greška pri kreiranju sandučića"

## Tehnički detalji

### Backend stack
- .NET 9
- Entity Framework Core
- PostgreSQL
- ASP.NET Core Web API

### Frontend stack
- React 19
- TypeScript
- Vite
- Tailwind CSS
- React Hook Form
- Zod validacija
- Sonner za toast obavijesti

### Baza podataka
- Tabela: `Mailboxes`
- Indeksi: `IX_Mailboxes_SerialNumber` (jedinstven)
- Migration: `AddMailboxEntity`

## Buduća poboljšanja

1. **Integracija sa mapom**
   - Interaktivni odabir lokacije
   - Automatsko popunjavanje koordinata

2. **Bulk unos**
   - Import iz CSV/Excel fajlova
   - Masovno kreiranje sandučića

3. **Slike**
   - Upload fotografija sandučića
   - Vizuelna dokumentacija

4. **QR kodovi**
   - Generisanje QR kodova za sandučiće
   - Brza identifikacija na terenu
