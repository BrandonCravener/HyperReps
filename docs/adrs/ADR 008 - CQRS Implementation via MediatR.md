# ADR 008: CQRS Implementation via MediatR

* **Status:** Accepted
* **Date:** 2025-12-27

## Context
In traditional N-Tier architectures, "Services" often become "God Classes" (e.g., a 2000-line `MixService.cs`) that handle validation, mapping, and database logic for every operation related to an entity. This violates the Single Responsibility Principle.

## Decision
We will implement the **CQRS (Command Query Responsibility Segregation)** pattern using the **MediatR** library.

1.  **Requests:** Every action is distinct (e.g., `CreateMixCommand`, `GetMixByIdQuery`).
2.  **Handlers:** Each request has exactly one Handler (e.g., `CreateMixCommandHandler`).
3.  **Pipeline Behaviors:** Cross-cutting concerns like Validation (`FluentValidation`) and Logging will be handled by MediatR pipeline behaviors, keeping handlers pure.

## Consequences
* **Positive:** **Single Responsibility:** Each class does exactly one thing.
* **Positive:** **No Fat Controllers:** API Controllers simply `_mediator.Send(command)` and return the result.
* **Positive:** **Testability:** It is trivial to test a single handler in isolation.
* **Negative:** **Explosion of Files:** A simple feature might result in 3-4 files (Command, Handler, Validator, DTO).
* **Negative:** **Indirection:** It can be harder to "Go to Definition" to find the logic compared to a simple Service method call.