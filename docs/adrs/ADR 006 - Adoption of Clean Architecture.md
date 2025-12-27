# ADR 006: Adoption of Clean Architecture

* **Status:** Accepted
* **Date:** 2025-12-27

## Context
As HyperReps grows, tight coupling between business logic, database access, and the UI (API controllers) will lead to a rigid system that is difficult to test and refactor.
We need an architectural style that prioritizes:
1.  **Testability:** Business logic must be testable without spinning up a database or web server.
2.  **Independence:** The UI (Web API) and Database (EF Core) should be plugins to the business logic, not the other way around.
3.  **Maintainability:** Clear boundaries where specific types of logic reside.

## Decision
We will adopt **Clean Architecture** (also known as Onion/Hexagonal Architecture) with strict layer separation:

1.  **Domain (Inner Circle):** Pure C# entities and business rules. No dependencies.
2.  **Application (Use Cases):** orchestrates data flow using Interfaces, DTOs, and CQRS commands. Depends only on Domain.
3.  **Infrastructure (Gateways):** Implements interfaces (Repositories, External Services). Depends on Application/Domain.
4.  **API (Presentation):** Entry point. Depends on Application/Infrastructure.

## Consequences
* **Positive:** High testability. We can unit test the `Domain` and `Application` layers in isolation using mocks.
* **Positive:** Technology agnostic core. We can swap the database (PostgreSQL) or external providers (Spotify) without touching the core business rules.
* **Negative:** Increased boilerplate. Simple CRUD operations require creating files in multiple layers (Entity, DTO, Interface, Implementation, Controller).
* **Negative:** Learning curve. Developers must understand "Dependency Inversion" to avoid referencing Infrastructure types (like `DbContext`) directly in the Core.