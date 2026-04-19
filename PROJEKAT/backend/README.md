# PostRoute Backend

Minimalni backend skeleton sa slojevima API/BLL/DAL.

## Trenutna struktura
- `PostRoute.sln`
- `src/PostRoute.Api`
- `src/PostRoute.BLL`
- `src/PostRoute.DAL`

## Referentni primjer
Trenutno postoji jedan minimalni primjer za `User`:
- API endpoint: `GET /api/users/{userId}`
- Controller delegira poziv na BLL servis
- BLL servis koristi repository interfejs iz DAL sloja
- DAL koristi placeholder repository implementaciju (bez baze)

Ovaj primjer je obrazac za naredne entitete, ne poslovna funkcionalnost.
