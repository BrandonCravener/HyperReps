# HyperReps Implementation Guidelines

This document outlines the strict rules for contributing to the HyperReps backend. Follow these guidelines to maintain the integrity of the Clean Architecture.

## 1. The Golden Rule of Dependency
**Dependencies only point INWARDS.**
* `API` -> `Infrastructure` & `Application`
* `Infrastructure` -> `Application` & `Domain`
* `Application` -> `Domain`
* `Domain` -> **NOTHING**

❌ **NEVER** reference `Entity Framework`, `DbContext`, or `HTTP` classes inside the `Domain` or `Application` layers.

## 2. Layer-Specific Rules

### 🏛 Domain Layer (The Core)
* **Entities:** Must be "Rich", not "Anemic".
    * ❌ Bad: `public void SetDuration(int ms) { Duration = ms; }`
    * ✅ Good: `public void UpdateDuration(int ms) { if(ms < 0) throw new DomainException(...); Duration = ms; }`
* **Exceptions:** Define custom exceptions here (e.g., `MixValidationException`). Do not throw generic `System.Exception`.

### 🧠 Application Layer (The Logic)
* **CQRS:** Use Commands for writes, Queries for reads.
* **DTOs:** Always return DTOs, never Domain Entities.
    * *Why?* It prevents the API from accidentally exposing internal state or creating circular references (JSON serialization issues).
* **Validation:** Use `FluentValidation` validators for every Command.
    * Validation logic belongs here, NOT in the Controller.
* **Interfaces:** Define `IRepository` or `ISpotifyService` interfaces here. Do not implement them here.

### 🏗 Infrastructure Layer (The Plumbing)
* **Repositories:** Implement the interfaces from *Application*.
    * This is the ONLY place where `ApplicationDbContext` is allowed.
* **External APIs:** Implement wrappers for Spotify/S3 here.
* **Configuration:** Inject `IConfiguration` here to read connection strings/keys.

### 🔌 API Layer (The Interface)
* **Controllers:** Must be "Thin".
    * They should strictly handle HTTP concerns (Status Codes, Headers) and delegate work to `_mediator`.
    * ❌ **No business logic in Controllers.**

## 3. Workflow: Adding a New Feature

When adding a feature (e.g., "Rename Mix"), follow this order:

1.  **Domain:** Add the `Rename()` method to the `Mix` entity. Ensure it enforces invariants (e.g., name cannot be empty).
2.  **Application:**
    * Create `RenameMixCommand` (DTO).
    * Create `RenameMixCommandValidator` (FluentValidation).
    * Create `RenameMixCommandHandler` (Logic: Load Entity -> Call Domain Method -> Save).
3.  **API:** Add the endpoint `PATCH /mixes/{id}/name` to the Controller.

## 4. Testing Strategy

* **Unit Tests (`tests/UnitTests`):**
    * Test `Domain` entities for logic correctness.
    * Test `Application` handlers by mocking dependencies (`Mock<IMixRepository>`).
* **Integration Tests (`tests/IntegrationTests`):**
    * Use **Testcontainers** to spin up a real PostgreSQL Docker container.
    * Test the full `Infrastructure` layer (Repositories) to ensure SQL queries actually work.

## 5. Common Pitfalls

* **"I need the current user's ID in the Domain."**
    * **Solution:** Pass it as an argument to the method. Do not inject `IHttpContextAccessor` into the Domain.
* **"I need to map Entities to DTOs."**
    * **Solution:** Use Mapster or AutoMapper in the `Application` layer handlers.