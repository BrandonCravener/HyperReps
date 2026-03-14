# ADR 007: Frontend Framework Selection (Angular)

* **Status:** Accepted
* **Date:** 2025-12-27

## Context
The HyperReps frontend requires complex state management for the "Mix Editor" (handling audio timing, waveforms, and drag-and-drop sequencing). It also requires strict integration with the backend API contracts.
We evaluated React vs. Angular.

## Decision
We will use **Angular** (latest stable version).

1.  **Opinionated Structure:** Angular's module/component system enforces a consistent structure that scales better for complex "Enterprise-grade" applications compared to React's flexibility.
2.  **TypeScript First:** Angular's strict adherence to TypeScript aligns with our strongly-typed .NET backend, allowing for easier sharing of DTO patterns/types.
3.  **RxJS Integration:** The "Mix Editor" involves complex asynchronous event streams (audio progress, user clicks, websocket updates). Angular's native integration with RxJS makes handling these streams significantly more robust than React's `useEffect` hooks.

## Consequences
* **Positive:** Powerful DI (Dependency Injection) system mirrors the backend architecture.
* **Positive:** Robust handling of async streams via RxJS is ideal for the Audio Engine logic.
* **Negative:** Higher initial boilerplate compared to simple React apps.
* **Negative:** steeper learning curve for developers not familiar with RxJS.