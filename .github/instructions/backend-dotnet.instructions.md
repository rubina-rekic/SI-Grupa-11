---
applyTo: "PROJEKAT/backend/**/*.cs,PROJEKAT/backend/**/*.csproj,PROJEKAT/backend/**/*.sln"
---

# Backend .NET Instructions

## Scope
These instructions apply to backend .NET code in this repository.

Always follow repository-wide guidance from `.github/copilot-instructions.md`, then apply these backend-specific rules.

---

## Current backend structure
Backend root: `PROJEKAT/backend`

Current solution and projects:
- `PostRoute.sln`
- `src/PostRoute.Api`
- `src/PostRoute.BLL`
- `src/PostRoute.DAL`

Current folder intent:
- `src/PostRoute.Api/Configuration`: API composition and registration extensions.
- `src/PostRoute.Api/Controllers`: thin HTTP controllers only.
- `src/PostRoute.Api/Contracts`: request/response DTOs used at API boundary.
- `src/PostRoute.BLL/Services`: business orchestration services and interfaces.
- `src/PostRoute.BLL/Models`: business-facing models used by services.
- `src/PostRoute.BLL/DependencyInjection`: BLL service registration.
- `src/PostRoute.DAL/Entities`: persistence entities.
- `src/PostRoute.DAL/Repositories`: repository interfaces and temporary placeholder implementations.
- `src/PostRoute.DAL/DependencyInjection`: DAL registration.

Key API folders and current contents:
- `src/PostRoute.Api/Controllers/UsersController.cs`: user CRUD-lite + auth/session endpoints.
- `src/PostRoute.Api/Contracts/Users`: `CreateUserRequest`, `LoginRequest`, `ChangePasswordRequest`, `UserResponse`.
- `src/PostRoute.Api/Middleware/RoleAuthorizationMiddleware.cs`: session + role checks via `RequiredRoleAttribute`.

Key BLL folders and current contents:
- `src/PostRoute.BLL/Services/IUserService.cs`, `UserService.cs`: user management, login, password change.
- `src/PostRoute.BLL/Services/IUserSeedService.cs`, `UserSeedService.cs`: startup seeding for default accounts.
- `src/PostRoute.BLL/Commands/CreateUserCommand.cs`: create-user input contract.
- `src/PostRoute.BLL/Exceptions`: `AccountLockedException`, `InvalidCredentialsException`.
- `src/PostRoute.BLL/Models/UserModel.cs`: business user projection.

Key DAL folders and current contents:
- `AppDbContext.cs`: EF Core DbContext with `Users` and `SecurityLogs`.
- `Entities`: `User`, `SecurityLog`.
- `Repositories`: `IUserRepository`, `ISecurityLogRepository` + EF Core implementations.
- `Migrations`: initial schema + lockout/security log migrations.

When adding files, place them in the matching layer folder above instead of creating ad-hoc locations.

---

## Layer boundaries and dependencies
The backend uses strict layering:
- API
- BLL
- DAL

Dependency direction:
- API can depend on BLL.
- BLL can depend on DAL abstractions.
- API must not call DAL directly.

Rules:
- Keep controllers thin and focused on HTTP concerns.
- Keep business rules and orchestration in BLL.
- Keep persistence and repository concerns in DAL.

---

## Repository pattern direction (explicit)
Repository pattern is the expected persistence approach in this codebase.

Use these rules:
- Define repository abstractions in `PostRoute.DAL/Repositories`.
- Keep repository implementations in DAL (currently placeholder/in-memory style until DB is introduced).
- Inject repositories into BLL services through interfaces.
- Do not query persistence directly from controllers.

Until database integration starts, placeholder repository implementations are acceptable, but they must stay in DAL.

---

## Current minimal example flow
The current reference flow is for `User` and demonstrates intended architecture:
- Controller: `src/PostRoute.Api/Controllers/UsersController.cs`
- API DTO: `src/PostRoute.Api/Contracts/Users/UserResponse.cs`
- Service interface + implementation: `src/PostRoute.BLL/Services`
- Business model: `src/PostRoute.BLL/Models/UserModel.cs`
- Entity: `src/PostRoute.DAL/Entities/User.cs`
- Repository interface + placeholder implementation: `src/PostRoute.DAL/Repositories`

Use this pattern for future entities, but keep changes incremental and avoid premature complexity.

---

## Authentication and session flow (current)
Auth is session-based (cookies) and not JWT-based yet.

Backend flow:
- `POST /api/users/login`: validates credentials, sets session (`UserId`, `UserRole`, `Username`, `Email`), logs success/failure to `SecurityLog`.
- `GET /api/users/current-user`: reads session and returns current user or `401`.
- `POST /api/users/change-password`: requires session email match; updates hash and clears `MustChangePassword`.
- `POST /api/users/logout`: clears session.
- `POST /api/users`: admin-only create; uses `RequiredRoleAttribute`.
- `GET /api/users/{userId}`: fetch user by id.

Role checks:
- `RequiredRoleAttribute` + `RoleAuthorizationMiddleware` use `UserRole` values and deny access with `403` while logging `SecurityLog` entries.

Default seeded accounts:
- Admin: `admin@mail.com` / `Admin123!` (`Administrator` role).
- Postal workers: `postar@mail.com` / `Postar123!`, `postar1@mail.com` / `Postar123!` (`PostalWorker` role).

---

## Coding guidance
Prefer code that is simple, explicit, and easy to maintain by a student team.

- Use `async/await` for I/O style flows.
- Propagate `CancellationToken` through public async methods where appropriate.
- Keep method names clear and behavior predictable.
- Avoid speculative abstractions and unnecessary interfaces outside clear boundaries.

---

## Output expectations
When changing backend code:
- briefly explain what changed
- briefly explain why
- mention affected layer boundaries
- call out assumptions instead of inventing missing business rules
