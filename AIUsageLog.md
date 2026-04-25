# AI Usage Log

Ovaj log je obavezan u AI-enabled fazi projekta. Svrha dokumenta je osiguravanje transparentnosti, praćenje načina na koji AI pomaže u radu, te procjena zrelosti tima u korištenju ovih alata.

| Datum | Sprint | Alat | Svrha korištenja | Opis zadatka/upita | AI Prijedlog | Prihvaćeno | Izmijenjeno | Odbačeno | Rizici/Greške | Korisnik |
|:---|:---:|:---|:---|:---|:---|:---|:---|:---|:---|:---|
| 2026-04-25 | 5 | Claude Code | Popravka .gitignore | Analiza i poboljšanje sva tri .gitignore fajla (root, backend, frontend) | Dodati `.vs/`, `*.user`, `appsettings.Development.json`, `.env`, OS fajlovi | Da | - | - | - | Kerim |
| 2026-04-25 | 5 | Claude Code | Analiza projekta | Kompletna analiza arhitekture, stack-a, strukture i trenutnog stanja projekta | Pregled svih slojeva (API/BLL/DAL, UI/Application/Infrastructure), identifikacija skeleton faze | Da | - | - | - | Kerim |
| 2026-04-25 | 5 | Claude Code | Analiza preduslova za PBI-011 | Provjera TechnicalSetup.md i koda, identifikacija blokatora za US-01/02/03 | 4 kritična blokatora: frontend paketi nisu instalirani, httpClient stub (501), nema BCrypt, nema CORS | Da | - | - | - | Kerim |
| 2026-04-25 | 5 | Claude Code | Infra setup za PBI-011 | Instalacija frontend paketa (axios, react-router-dom, react-hook-form, zod, sonner), implementacija httpClient sa Axios + JWT interceptorom, BCrypt.Net-Next na BLL, CORS politika u Program.cs | Kompletna infra priprema — sve 4 blokade uklonjene | Da | - | - | VS locka obj/ fajlove pri paralelnom buildu; rješenje: zatvoriti VS pa pokrenuti dotnet build | Kerim |