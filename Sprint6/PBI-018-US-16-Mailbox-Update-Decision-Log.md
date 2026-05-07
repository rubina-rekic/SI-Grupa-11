# PBI-018/US-16: Ažuriranje podataka o sandučiću - Decision Log

## Datum implementacije
- **Datum**: 7. maj 2026.
- **Implementer**: AI Assistant (Cascade)
- **Vrijeme implementacije**: ~3 sata

## Opis zadatka
Implementirati kompletnu funkcionalnost za ažuriranje podataka o sandučiću (mailboxes) u ASP.NET Core aplikaciji sa React frontendom.

## Ključni zahtjevi
- ✅ Formular za uređivanje sa prepopuliranim podacima
- ✅ Validacija ista kao pri kreiranju
- ✅ Audit log za praćenje promjena
- ✅ Backend PUT endpoint
- ✅ Frontend edit stranica
- ✅ Integracija sa postojećim list view-om

## Tehničke odluke i razlozi

### 1. Authentication arhitektura
**Odluka**: Implementirana cookie-based authentication umjesto session-based
**Razlog**: Standardni ASP.NET Core authentication nije prepoznavao session kao validni authentication, što je dovelo do 401 Unauthorized grešaka
**Implementacija**:
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => { ... });
app.UseAuthentication();
```

### 2. Frontend/backend komunikacija
**Odluka**: Korišćen Vite proxy za rješavanje cross-origin problema
**Razlog**: Frontend (localhost:5173) i backend (localhost:5000) su na različitim portovima, što je cross-origin scenario
**Implementacija**:
```typescript
// vite.config.ts
server: {
  proxy: {
    '/api': {
      target: 'http://localhost:5000',
      changeOrigin: true,
      secure: false
    }
  }
}

// environment.ts
export const environment = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? ""
}
```

### 3. Database schema
**Odluka**: Kreirana MailboxAuditLog tabela za praćenje promjena
**Razlog**: Svaka promjena sandučića se logira sa detaljnim informacijama
**Implementacija**:
```csharp
public class MailboxAuditLog
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid MailboxId { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string FieldName { get; set; } = string.Empty;
    
    [Required]
    public string? OldValue { get; set; }
    
    [Required]
    public string? NewValue { get; set; }
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(20)]
    public string Action { get; set; } = string.Empty; // "UPDATE", "CREATE", "DELETE"
}
```

### 4. Frontend komponente
**Odluka**: Kreirana EditMailboxPage.tsx komponenta sa react-hook-form
**Razlog**: Korišćen zod za validaciju i react-hook-form za form handling
**Ključne osobine**:
- Read-only serijski broj
- Prepopulirani podaci
- Map integracija
- Toast notifikacije

### 5. API endpoint
**Odluka**: Dodan PUT /api/mailboxes/{id} endpoint
**Razlog**: Endpoint zahtijeva Administrator rolu i audit logging
**Implementacija**:
```csharp
[HttpPut("{id:guid}")]
[RequiredRole("Administrator")]
public async Task<ActionResult<MailboxResponse>> UpdateAsync(Guid id, UpdateMailboxRequest request, CancellationToken cancellationToken)
```

## Rezultat
- ✅ **Funkcionalnost**: Svi zahtjevi su uspješno implementirani
- ✅ **Authentication**: Riješen 401 Unauthorized problem
- ✅ **UI**: Moderniziran i profesionalan dizajn
- ✅ **Audit**: Kompletan audit trail za sve promjene

## Buduće preporuke
1. **Standardizacija**: Implementirati centralized authentication servis za cijelu aplikaciju
2. **Error handling**: Koristiti global error handler sa standardizovanim error kodovima
3. **Testing**: Osigurati integration testove za authentication i API endpointove

## Status
**Status**: ZAVRŠENO ✅

PBI-018/US-16 je uspješno implementiran sa svim zahtjevim funkcionalnostima i spreman za produkciju.
