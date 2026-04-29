---
applyTo: "PROJEKAT/frontend/**/*.ts,PROJEKAT/frontend/**/*.tsx,PROJEKAT/frontend/**/package.json,PROJEKAT/frontend/**/vite.config.*,PROJEKAT/frontend/**/src/**/*"
---

# Frontend React Instructions

## Scope
These instructions apply to the React/TypeScript frontend.

Always follow repository-wide guidance from `.github/copilot-instructions.md`, then apply these frontend-specific rules.

---

## Current frontend structure
Frontend root: `PROJEKAT/frontend`

Current structure:
- `src/main.tsx`: React bootstrap entry.
- `src/vite-env.d.ts`: local `ImportMeta` typing for Vite env variables.
- `src/app`: top-level app composition.
- `src/ui`: presentation layer (pages, layouts, presentational components).
- `src/application`: hooks and feature orchestration models.
- `src/infrastructure`: API client wrappers, config, routing, storage adapters.
- `src/shared`: shared types and utilities.
- `src/styles`: global styling.
- `public`: optional static assets only when truly needed.

When adding new files, prefer these existing folders before introducing new top-level frontend directories.

---

## Current frontend routes and auth flow
Routing lives in `src/infrastructure/routing/AppRouter.tsx` and uses a `PrivateRoute` wrapper.

Key routes:
- `/login`: `LoginPage`.
- `/change-password`: `ChangePasswordPage`.
- `/dashboard`: shared dashboard, role-based content.
- `/admin/*`: admin-only placeholders and `CreatePostalWorkerPage` (`Administrator` role required).
- `/worker/*`: postal worker-only placeholders (`PostalWorker` role required).

Auth flow (client):
- `useAuth` hook (`src/application/hooks/useAuth.ts`) calls `GET /api/users/current-user` on load.
- `login(email, password)` posts to `POST /api/users/login`, then redirects to `/change-password` if `mustChangePassword` is true, otherwise `/dashboard`.
- `logout()` posts to `POST /api/users/logout` then clears client state.

HTTP access:
- `httpClient` wraps Axios with `withCredentials: true` to send session cookies to the backend.

---

## Layer boundaries
The frontend follows a layered SPA structure:
- UI / Presentation
- Application
- Infrastructure

Rules:
- Keep rendering and interaction in `ui`.
- Keep flow coordination in `application`.
- Keep HTTP, routing, storage, and environment wiring in `infrastructure`.
- Keep shared primitives in `shared`.
- Do not place direct HTTP calls inside presentational components.

---

## TypeScript and environment typing
- Prefer strong typing and avoid `any`.
- Keep Vite environment typing in `src/vite-env.d.ts`.
- Do not reintroduce hard dependency on `types: ["vite/client"]` in `tsconfig.app.json`; use local `ImportMetaEnv` typing and keep it in sync with used env vars.

---

## Current frontend baseline behavior
The current frontend is an initial skeleton with structure-demonstration UI only.

- It is not a business feature implementation.
- Keep additions small and architecture-driven.
- Reuse the existing `app -> ui/application/infrastructure` flow as the baseline pattern.

---

## Change strategy
Before making frontend changes:
1. Inspect nearby feature files and follow existing naming.
2. Keep diffs small and local.
3. Preserve responsive behavior and clear interaction patterns.
4. Avoid broad refactors unless explicitly requested.

---

## Output expectations
When changing frontend code:
- briefly explain what changed
- briefly explain why
- mention affected layer(s)
- call out assumptions instead of inventing missing product rules
