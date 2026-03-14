# ADR 008: CQRS Implementation via Wolverine

* **Status:** Accepted
* **Date:** 2026-01-01
* **Supersedes:** [Draft] CQRS Implementation via MediatR

## Context
In traditional N-Tier architectures, "Services" often become "God Classes" that violate the Single Responsibility Principle. While the **CQRS (Command Query Responsibility Segregation)** pattern solves this by splitting operations into distinct Request/Handler pairs, the traditional library for this in .NET (MediatR) relies heavily on reflection and lacks built-in support for transactional durability.

Additionally, our "Playlist Synchronization" feature (ADR 003) requires robust event publishing and background processing. Implementing this with MediatR requires stitching together third-party libraries for "Outbox" patterns, increasing complexity and fragility. Furthermore, recent licensing changes to MediatR (moving to a commercial model) pose a long-term risk for the project.

## Decision
We will implement the **CQRS** pattern using **Wolverine**.

1.  **Message Bus:** Wolverine will act as the in-process mediator for Commands and Queries.
2.  **No Reflection:** Wolverine uses **Source Generators** to compile handler invocation code at build time, eliminating the runtime performance cost associated with reflection-based mediators.
3.  **Durability:** We will leverage Wolverine's built-in **Transactional Outbox** implementation. When a handler modifies a `Mix` and publishes a `MixCreated` event, Wolverine ensures the database change and the event message are committed atomically.
4.  **Compound Handlers:** We will utilize Wolverine's "Compound Handler" capability to inject pure Domain entities directly into handler methods, reducing the boilerplate of fetching data in every handler.

## Consequences
* **Positive:** **Performance:** Faster startup and runtime execution due to pre-compiled source generation.
* **Positive:** **Robustness:** Native support for retries, circuit breakers, and the Outbox pattern eliminates the "Distributed Transaction" problem for our sync jobs.
* **Positive:** **Licensing:** Wolverine is free and open-source (MIT), mitigating commercial dependency risks.
* **Negative:** **Learning Curve:** Developers must learn Wolverine-specific conventions (e.g., static `Handle` methods, side effects) which differ slightly from the standard MediatR `IRequestHandler<T>` interfaces.
* **Negative:** **Ecosystem:** While powerful, the community ecosystem is smaller than MediatR's, potentially yielding fewer StackOverflow answers for edge cases.