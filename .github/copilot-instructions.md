# GitHub Copilot Instructions

## Purpose of this file
This file is the persistent repository context for GitHub Copilot and other coding agents working in this codebase.

Treat this as a living repository policy, not a static template.

- Update it whenever architecture, folder structure, coding conventions, business rules, workflows, tooling, testing approach, or other cross-cutting decisions change.
- If you discover a stable repository convention that appears repeatedly in code or documentation and is missing here, update this file as part of your task.
- Keep this file practical, high-signal, and repository-specific.

---
## Additional instruction files
This repository also uses path-specific instruction files under `.github/instructions/`.

When working in matching parts of the repository, agents must read and follow:
- `.github/instructions/backend-dotnet.instructions.md` for backend .NET / C# work
- `.github/instructions/frontend-react.instructions.md` for frontend React / TypeScript work

These files complement this repository-wide instructions file.
If there is no conflict, apply both:
- repository-wide instructions from `.github/copilot-instructions.md`
- path-specific instructions from `.github/instructions/*.instructions.md`

If a stable convention becomes specific to backend or frontend work, update the corresponding path-specific instructions file as well as this root file when appropriate.

## Project context
This repository contains a monolithic web application for optimizing routes for filling and emptying postal mailboxes.

Business purpose:
- reduce manual dispatch planning effort
- improve field execution visibility
- support faster reaction to incidents
- improve operational reporting for daily decisions

Primary user roles:
- Administrator: manages users and core system data
- Dispatcher: generates, assigns, monitors, and adjusts routes
- Postman: executes assigned routes in the field and updates statuses
- Management/supervision: consumes reports and operational overview

Key domain concepts:
- users
- delivery zones
- host venues
- working schedules
- non-working days
- mailboxes
- vehicles and vehicle assignments
- routes and route items
- collection records and mail items
- incidents and notifications
- daily reports and audit logs

Repository and process context:
- student team project delivered iteratively through sprint artifacts (Sprint1..Sprint4 so far)
- documentation is split across product vision, backlog, user stories, architecture, domain, risks, test strategy, release plan, and technical setup
- preserve traceability from requirements to implementation and tests

MVP constraints reflected in docs:
- web application (responsive), no native mobile app
- no full offline mode in MVP
- route optimization starts with nearest-neighbor heuristic in backend service
- dispatcher monitoring uses periodic HTTP polling in MVP

---

## Architecture boundaries

### Backend architecture (strict)
Backend is organized into three layers:
- API
- BLL
- DAL

Responsibilities:
- API: HTTP endpoints, request validation, authentication/authorization, delegating to BLL
- BLL: business rules, use-case orchestration, route optimization logic
- DAL: persistence and data access only (EF Core, repositories, mappings, migrations)

Rules:
- no business logic in controllers
- no business logic in DAL/repositories
- no direct DB access from API layer

### Frontend architecture (strict)
Frontend is a React SPA organized into:
- UI/Presentation
- Application
- Infrastructure

Responsibilities:
- UI/Presentation: rendering and user interactions
- Application: hooks, orchestration, feature state, view models, client flow coordination
- Infrastructure: API client, interceptors, routing, storage, technical integration

Rules:
- presentational components do not contain direct HTTP logic
- avoid mixing routing, networking, and rendering in one component unless existing pattern already does that

### Frontend-backend communication
- frontend and backend communicate only via REST API contracts
- OpenAPI/Swagger contract is the integration source of truth
- never bypass architectural layers unless explicitly requested and justified

### Default stack assumptions (from current docs)
- backend: ASP.NET Core Web API, C#, ASP.NET Identity, JWT RBAC, EF Core, PostgreSQL
- frontend: React 18, TypeScript (strict), Vite, React Router v6, React Hook Form + Zod, Tailwind CSS, Axios interceptors

### Repository structure baseline
- project code root: `PROJEKAT/`
- backend root: `PROJEKAT/backend`
- frontend root: `PROJEKAT/frontend`
- backend solution/project naming: `PostRoute*`
- frontend naming baseline: `postroute-frontend`

### `PROJEKAT/` structure (current)
- `PROJEKAT/README.md`: top-level project setup notes.
- `PROJEKAT/create-admin.ps1`: helper script for creating an admin user via API.
- `PROJEKAT/docker-compose.dev.yml`: dev stack (backend + dependencies) wiring.
- `PROJEKAT/backend/`: backend solution root.
  - `PostRoute.sln`: backend solution.
  - `README.md`: backend setup notes.
  - `src/PostRoute.Api`: ASP.NET Core Web API (controllers, middleware, API DTOs).
  - `src/PostRoute.BLL`: business logic layer (services, commands, models).
  - `src/PostRoute.DAL`: data access layer (EF Core context, repositories, migrations, entities).
  - `src/PostRoute.Domain`: domain-level shared types (e.g., `UserRole`).
- `PROJEKAT/frontend/`: React + Vite frontend.
  - `package.json`, `vite.config.ts`, `index.html`: frontend tooling/entry.
  - `src/`: layered frontend structure (`app`, `ui`, `application`, `infrastructure`, `shared`, `styles`).

---

## Core engineering principles
Optimize for:
- simplicity
- readability
- maintainability
- predictability
- low cognitive load for a student team

Prefer:
- explicit solutions over clever ones
- small, focused, low-risk changes
- local clarity over premature abstraction
- existing repository patterns over new abstractions

Avoid:
- unnecessary frameworks
- speculative abstractions and overengineering
- hidden side effects
- mixing responsibilities across architectural layers

Business-rule discipline:
- do not guess unresolved rules
- if a rule is unclear, preserve current behavior and call out uncertainty
- use documented constraints from sprint artifacts before introducing new behavior

---

## Domain and business-rule awareness
When a change touches routing, field execution, incidents, notifications, or reporting, inspect related domain and use-case docs first.

Important rule-driven characteristics:
- routes are composed of route items
- route items target specific mailboxes and status transitions matter
- mailbox availability depends on working rules, host venue schedules, and non-working days
- realized route items can require collection records
- incident handling affects operations and reporting
- auditability and server-side timestamps matter

Do not flatten domain behavior into generic CRUD where the module models richer operational logic.

---

## .NET / C# guidance
For backend code:
- keep controllers thin
- keep business logic in BLL services/use-case handlers
- keep DAL focused on persistence and querying
- use async/await for I/O-bound work
- pass CancellationToken through public async flows where appropriate
- use dependency injection consistently
- keep DTOs, entities, and view models separated where appropriate
- prefer composition over inheritance unless clearly justified
- keep naming explicit and methods easy to follow
- preserve predictable API behavior and stable contracts

When adding backend code:
- mirror existing folder structure and naming
- reuse existing abstractions where idiomatic
- do not create interfaces by default without a real boundary/need
- keep route optimization logic isolated in BLL service boundaries

Security and access expectations:
- enforce role checks at API level, not only in frontend UI guards
- never expose sensitive data in logs or error responses

---

## React / TypeScript guidance
For frontend code:
- use functional components and hooks
- keep presentational components focused on UI and interaction
- keep orchestration/state flow in hooks/application layer
- do not place direct HTTP calls inside presentational components
- reuse centralized infrastructure/API layer
- prefer strong typing and avoid any
- keep prop interfaces explicit and readable
- reuse shared UI components/utilities where appropriate
- preserve responsive behavior for field/mobile flows
- preserve accessibility and straightforward flows

Current frontend conventions from docs:
- role-based views (admin, dispatcher, postman) in one SPA
- protected routes and role guards
- axios interceptors for auth token handling
- forms validated via React Hook Form + Zod

---

## Testing and quality expectations
Follow Sprint3 Test Strategy, Sprint4 Definition of Done, and NFR quality constraints.

Minimum expectations when changing behavior:
- update or add tests where feasible, especially for business logic
- keep acceptance criteria and implementation aligned
- keep changes easy to verify and review

High-priority verification areas:
- authentication and RBAC correctness
- route generation correctness (priority + availability constraints)
- route status updates and dispatcher visibility timing
- critical security checks and logging behavior

Performance and reliability guardrails from docs:
- standard operations should remain responsive under expected MVP load
- route generation performance target must remain within documented constraints

Do not claim correctness without checking affected flows and nearby conventions.

---

## Repository workflow and working style
Before editing:
1. inspect nearby files
2. identify local structure and naming patterns
3. match existing style first

When changing code:
- make minimal diffs
- avoid broad refactors unless explicitly requested
- keep changes localized to relevant feature/layer
- do not silently rewrite unrelated files
- preserve backward compatibility unless task explicitly allows breaking changes

Branching and collaboration conventions (from TechnicalSetup):
- GitLab-flow style on GitHub: main, develop, short-lived feature/fix/docs branches
- no direct pushes to protected main/develop
- merge through PR with review and passing CI
- use conventional commit prefixes (feat, fix, docs, refactor, test, chore)

Generated output should align with repository style and existing conventions.

---
## Living project artifacts
This project explicitly defines several artifacts as continuously maintained documents. Implementation changes can require artifact updates, not only code updates.

### Artifacts that are explicitly living
- Product Backlog (`Sprint*/ProductBacklog.md` + JIRA backlog): marked as a living document and explicitly expected to be regularly updated in both places.
- For Product Backlog maintenance, keep status transitions (for example To Do to Done), sprint mapping, and JIRA parity synchronized.
- Risk Register (`Sprint3/RiskRegister.md`): explicitly defined as a living artifact; update at sprint start and whenever new risks/incidents appear.
- Architecture Overview (`Sprint3/ArchitectureOverview.md`): explicitly updated when a sprint introduces an architectural change or resolves an open question.
- Use Case Model (`Sprint3/UseCaseModel.md`): explicitly defined as a living artifact, to be refined with requirement evolution and PO feedback.
- Initial Release Plan (`Sprint4/InitialReleasePlan.md`): explicitly non-static and expected to evolve during the semester.

### Artifacts with explicit ongoing maintenance rules
- Decision Log (planned from PBI-040): track key technical/architectural/scope decisions and significant plan changes.
- AI Usage Log (planned from PBI-041): maintain ongoing evidence of AI usage (prompts, purpose, modifications).
- Test evidence set (from `Sprint3/TestStrategy.md`): keep test case results, defect records, and phase test reports current as testing progresses.
- Release and final documentation set (from Sprint 12 plan): keep Release Notes, user documentation, technical documentation, and artifact-completeness updates aligned with delivered scope and known limitations.
- Definition of Done (`Sprint4/DefinitionOfDone.md`): may be extended during the project when agreed by the whole team.
- Team Google Docs decision and incident records (Sprint1/TeamCharter.md and Sprint3/RiskRegister.md): keep external decision and incident/problem records current when project changes affect them.

### Agent update policy for living artifacts
- For any meaningful change, check whether one or more living artifacts are materially affected.
- Update artifacts only when task impact is real and specific; do not perform blanket or cosmetic updates.
- Keep cross-document consistency when updating: backlog status/priority, plan assumptions, decision rationale, and test evidence should not drift.
- If an expected artifact lives outside this repository (for example, team Google Docs logs), call out the required follow-up explicitly in your summary.

### Trigger-to-artifact guidance
- Requirements or scope change: update Product Backlog; if release sequencing changes, update Initial Release Plan; if behavior/contracts changed, update Use Case Model and release docs.
- Backlog priority/structure or sprint mapping change: update Product Backlog and any affected Release Plan sections.
- Sprint outcome or sprint review feedback changes sequencing: update Initial Release Plan and Product Backlog, and record rationale in Decision Log.
- Architecture or technical decision change: update Architecture Overview and Decision Log together.
- Risk or assumption change: update Risk Register; if decision/assumption changed, also update Decision Log.
- Testing approach or evidence change: update Test Strategy-related evidence artifacts (test results, defects, reports).
- Release scope or known limitation change: update Release Notes and user/technical documentation accordingly.

## Open-question awareness
Some decisions are intentionally unresolved and should not be hard-coded as settled architecture.

Known unresolved areas include:
- login identifier strategy (email vs username)
- initial credential/bootstrap flow details
- offline support strategy for field work
- future server-state management approach on frontend
- post-MVP routing algorithm upgrades
- deployment/hosting specifics

If a task depends on these, follow current implemented behavior and mark assumptions clearly.

Role-model consistency rule:
- if documentation conflicts on role details, align with architecture/NFR baseline of three operational roles (Administrator, Dispatcher, Postman) unless task explicitly states otherwise.

---

## Living instructions maintenance rule
Whenever a meaningful change affects any of the following, review and update this file in the same task:
- architecture
- folder/repository structure
- coding conventions
- major business rules
- workflows
- tooling
- testing approach
- important cross-cutting decisions

This file must stay synchronized with the real repository and sprint documentation.

---

## Output expectations for agents
When making changes:
- briefly explain what changed
- briefly explain why it changed
- mention architectural reasoning when it affects the decision
- call out assumptions when assumptions were necessary

Do not produce filler, generic tutorials, or broad rewrites when a focused repository-specific change is enough.









