# 2. Deployment Procedura — PostRoute

### Sadržaj
1. [Naziv aplikacije i opis arhitekture](#1-naziv-aplikacije-i-opis-arhitekture)
2. [Tehnologije i verzije](#2-tehnologije-i-verzije)
3. [Potrebni alati](#3-potrebni-alati)
4. [Environment varijable](#4-environment-varijable)
5. [Lokalno pokretanje backenda](#5-lokalno-pokretanje-backenda)
6. [Lokalno pokretanje frontenda](#6-lokalno-pokretanje-frontenda)
7. [Baza podataka](#7-baza-podataka)
8. [Migracije i seed podaci](#8-migracije-i-seed-podaci)
9. [Pokretanje testova](#9-pokretanje-testova)
10. [Produkcijski deployment (Netlify & Render)](#10-produkcijski-deployment-netlify--render)
11. [Link na deployment](#11-link-na-deployment)
12. [Poznata ograničenja deploymenta](#12-poznata-ograničenja-deploymenta)
13. [Najčešći problemi i rješenja](#13-najčešći-problemi-i-rješenja)

---

### 1. Naziv aplikacije i opis arhitekture
**Naziv:** PostRoute — Sistem za optimizaciju ruta punjenja i pražnjenja poštanskih sandučića

**Arhitektura:** Sistem je organizovan kao troslojna aplikacija deployovana na odvojenim servisima:

```text
Browser
  └── React SPA (Netlify)
         │ HTTPS /api/*
         ▼
    ASP.NET Core Web API (Render Docker)
         │ SSL / TCP
         ▼
    PostgreSQL (Render Managed DB)
