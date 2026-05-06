# Decision Log - PostRoute Project

## PBI-017: Unos tipa i osnovnih podataka sandučića

### Datum: 07.05.2026
### Status: COMPLETED ✅

---

### 1. Tehnološke Odluke

#### 1.1 Frontend Framework
- **Odluka:** React sa TypeScript
- **Razlog:** Type safety, bolji developer experience, skalabilnost
- **Alternativa:** Vue.js, Angular
- **Justifikacija:** React ima najveću community podršku i biblioteke

#### 1.2 Mapa Integracija
- **Odluka:** OpenStreetMap sa react-leaflet
- **Razlog:** Google Maps API problemi, besplatna alternativa
- **Alternativa:** Google Maps API
- **Justifikacija:** OpenStreetMap je open-source, besplatan, i lakši za integraciju

#### 1.3 Form Validacija
- **Odluka:** React Hook Form + Zod
- **Razlog:** Performanse, type safety, re-render optimizacija
- **Alternativa:** Formik, Yup
- **Justifikacija:** React Hook Form je brži i ima bolju TypeScript integraciju

#### 1.4 HTTP Klijent
- **Odluka:** Axios sa custom wrapperom
- **Razlog:** Fleksibilnost, interceptors, error handling
- **Alternativa:** Fetch API
- **Justifikacija:** Axios ima bolju error handling i interceptors podršku

---

### 2. Arhitektonske Odluke

#### 2.1 Komponenta Struktura
- **Odluka:** Feature-based organizacija
- **Struktura:**
  ```
  src/
  ├── ui/
  │   ├── pages/
  │   │   └── admin/
  │   │       ├── CreateMailboxPage.tsx
  │   │       └── MailboxListPage.tsx
  │   └── components/
  │       └── common/
  │           └── OpenStreetMapPicker.tsx
  └── infrastructure/
      └── api/
          └── mailboxes/
              └── mailboxesApi.ts
  ```
- **Razlog:** Lakše održavanje i skaliranje
- **Alternativa:** Domain-driven design
- **Justifikacija:** Za ovu veličinu projekta, feature-based je jednostavniji

#### 2.2 State Management
- **Odluka:** Lokalni state sa React hooks
- **Razlog:** Jednostavnost, performanse
- **Alternativa:** Redux, Zustand
- **Justifikacija:** Nema potrebe za globalnim state-om za ovu funkcionalnost

---

### 3. API Design Odluke

#### 3.1 API Rute
- **Odluka:** RESTful API sa `/api/[controller]` konvencijom
- **Rute:**
  - `POST /api/mailboxes` - Kreiranje sandučića
  - `GET /api/mailboxes` - Lista sandučića
  - `GET /api/mailboxes/{id}` - Detalji sandučića
  - `GET /api/mailboxes/check-serial-number/{serialNumber}` - Provera jedinstvenosti
- **Razlog:** Standard RESTful prakse
- **Alternativa:** GraphQL
- **Justifikacija:** REST je jednostavniji i adekvatan za ovaj use case

#### 3.2 Data Transfer Objects (DTOs)
- **Odluka:** Strongly typed DTOs sa validacijom
- **Implementacija:** DataAnnotations za validaciju
- **Razlog:** Type safety, input validacija
- **Alternativa:** Anonimni objekti
- **Justifikacija:** DTOs pružaju bolju dokumentaciju i validaciju

---

### 4. Database Odluke

#### 4.1 Entity Design
- **Odluka:** Mailbox entitet sa decimal koordinatama
- **Polja:**
  - Id (Guid)
  - SerialNumber (string, unique)
  - Address (string)
  - Latitude (decimal(9,6))
  - Longitude (decimal(9,6))
  - Type (MailboxType enum)
  - Capacity (int)
  - InstallationYear (int)
  - Notes (string, nullable)
  - CreatedAt/UpdatedAt (DateTime)
- **Razlog:** Preciznost koordinata, integritet podataka
- **Alternativa:** float koordinate
- **Justifikacija:** decimal(9,6) pruža dovoljnu preciznost za GPS koordinate

#### 4.2 Enum Design
- **Odluka:** MailboxType enum sa brojevima
- **Vrednosti:**
  - WallSmall = 1
  - StandaloneLarge = 2
  - IndoorResidential = 3
  - SpecialPriority = 4
- **Razlog:** Stabilnost, baza podataka integritet
- **Alternativa:** String enum
- **Justifikacija:** Brojevi su efikasniji i stabilniji

---

### 5. UI/UX Odluke

#### 5.1 Design System
- **Odluka:** TailwindCSS sa custom komponentama
- **Komponente:** FormCard, FormField, Button
- **Razlog:** Brži development, consistency
- **Alternativa:** Material-UI, Bootstrap
- **Justifikacija:** TailwindCSS je fleksibilniji i customizabilniji

#### 5.2 User Experience
- **Odluka:** Progressive disclosure sa mapom
- **Flow:** Form → Mapa → Review → Save
- **Razlog:** Intuitivno korisničko iskustvo
- **Alternativa:** Sve na jednoj stranici
- **Justifikacija:** Progressive disclosure smanjuje kognitivno opterećenje

#### 5.3 Error Handling
- **Odluka:** Toast notifikacije sa detaljnim porukama
- **Implementacija:** Sonner library
- **Razlog:** Non-intrusive feedback
- **Alternativa:** Modal dialogs
- **Justifikacija:** Toast je manje disruptivan za korisnika

---

### 6. Security Odluke

#### 6.1 Authorization
- **Odluka:** Role-based access control (RBAC)
- **Implementacija:** `[RequiredRole("Administrator")]` attribute
- **Razlog:** Jednostavnost, jasne dozvole
- **Alternativa:** Claims-based authorization
- **Justifikacija:** RBAC je adekvatan za ovaj sistem

#### 6.2 Input Validation
- **Odluka:** Client-side + Server-side validacija
- **Client:** React Hook Form + Zod
- **Server:** DataAnnotations
- **Razlog:** Defense in depth
- **Alternativa:** Samo server-side
- **Justifikacija:** Client-side pruža bolji UX, server-side garantira sigurnost

---

### 7. Performance Odluke

#### 7.1 Frontend Optimization
- **Odluka:** Lazy loading za mapu
- **Implementacija:** Conditional rendering
- **Razlog:** Smanjenje initial load time
- **Alternativa:** Sve odmah
- **Justifikacija:** Mapa nije potrebna odmah

#### 7.2 API Optimization
- **Odluka:** Pagination za listu sandučića
- **Implementacija:** Skip/Take parameters
- **Razlog:** Smanjenje network load-a
- **Alternativa:** Sve odjednom
- **Justifikacija:** Prevencija performance problema sa velikim dataset-om

---

### 8. Testing Odluke

#### 8.1 Test Strategy
- **Odluka:** Unit + Integration + E2E testovi
- **Framework:** xUnit, Playwright
- **Pokriće:** Minimum 80% code coverage
- **Razlog:** Kompletna validacija funkcionalnosti
- **Alternativa:** Samo unit testovi
- **Justifikacija:** Različiti nivoi testiranja hvataju različite probleme

#### 8.2 Test Data
- **Odluka:** In-memory database za testove
- **Implementacija:** Entity Framework InMemory
- **Razlog:** Izolacija, brzina
- **Alternativa:** Real database
- **Justifikacija:** InMemory je brži i ne utiče na production podatke

---

### 9. Deployment Odluke

#### 9.1 Environment Setup
- **Odluka:** Docker Compose za development
- **Servisi:** PostgreSQL, Backend, Frontend
- **Razlog:** Consistency, portability
- **Alternativa:** Manual setup
- **Justifikacija:** Docker garantuje identična okruženja

#### 9.2 Configuration
- **Odluka:** Environment variables + appsettings.json
- **Razlog:** Security, flexibility
- **Alternativa:** Hardcoded values
- **Justifikacija:** Environment variables su standard praksa

---

### 10. Future Considerations

#### 10.1 Skalabilnost
- **Odluka:** Horizontal scaling sa load balancer-om
- **Razlog:** Povećani traffic
- **Implementacija:** Docker Swarm / Kubernetes

#### 10.2 Monitoring
- **Odluka:** Application logging + monitoring
- **Tools:** Serilog, Application Insights
- **Razlog:** Proactive problem detection

---

### 11. Lessons Learned

#### 11.1 Tehnički
- **Google Maps → OpenStreetMap:** API problemi su realni, imati alternativu
- **Type safety:** TypeScript je worth the effort
- **Error handling:** Detaljni logging je ključan za debug

#### 11.2 Procesni
- **Iterativni development:** Mali koraci su efikasniji
- **Testiranje:** Rano testiranje štedi vreme
- **Dokumentacija:** AI-assisted dokumentacija je efikasna

---

### 12. Zaključak

PBI-017 je uspešno implementiran sa svim planiranim funkcionalnostima:

- ✅ Frontend React komponente sa TypeScript
- ✅ OpenStreetMap integracija
- ✅ Backend API sa validacijom
- ✅ Database design sa proper enum-ima
- ✅ UI/UX sa TailwindCSS
- ✅ Security sa RBAC
- ✅ Error handling sa detaljnim logging

Implementacija je prošla kroz sistematičan proces odlučivanja sa fokusom na quality, performance, i maintainability. Sve odluke su dokumentirane sa justifikacijama za buduće reference.
