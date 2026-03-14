# HyperReps

**HyperReps** is a precision music mixing application that allows users to import Spotify playlists, define millisecond-accurate segments ("cuts"), and create gapless playback mixes.

This repository is a **Monorepo** containing the complete solution: the .NET Clean Architecture backend, the Angular frontend, and comprehensive documentation.

---

## 🏗 Architecture Overview

The solution follows **Enterprise .NET Clean Architecture** principles with a strict dependency rule: *inner layers define interfaces, outer layers implement them.*

### High-Level Structure

The repository is organized into four distinct roots:

| Folder | Description |
| :--- | :--- |
| **`/src`** | The **Backend** solution (.NET Core). Contains the 4 layers of Clean Architecture. |
| **`/client`** | The **Frontend** Single Page Application (Angular). Physically decoupled from the API. |
| **`/tests`** | **Unit and Integration tests** (xUnit) mirroring the source structure. |
| **`/docs`** | **Architecture documentation**, including ADRs and Mermaid.js diagrams. |

---

## 🔌 Backend Layering (`/src`)

The backend logic is split into four projects to ensure separation of concerns.

### 1. `HyperReps.Domain` (Core)
* **Role:** The enterprise business rules. Contains Entities, Value Objects, and Domain Exceptions.
* **Key Entities:** `User`, `Mix`, `MixSegment`, `Track`, `Playlist`.
* **Dependencies:** None (Pure C#).

### 2. `HyperReps.Application` (Orchestration)
* **Role:** Defines *what* the system does. Contains CQRS Handlers, DTOs, and Interfaces.
* **Patterns:**
    * **CQRS:** Uses **Wolverine** to handle Commands (Writes) and Queries (Reads).
    * **Validation:** Uses **FluentValidation** via Wolverine Middleware.
* **Dependencies:** `Domain`.

### 3. `HyperReps.Infrastructure` (Implementation)
* **Role:** Implements interfaces defined in the *Application* layer. Connects to external I/O.
* **Key Components:**
    * **Persistence:** Entity Framework Core (PostgreSQL).
    * **Background Jobs:** Redis integration (BullMQ/Hangfire) for playlist synchronization.
    * **Storage:** S3-compatible implementation (Cloudflare R2) for user uploads.
    * **Identity:** Native ASP.NET Spotify OAuth integration.
* **Dependencies:** `Application`, `Domain`.

### 4. `HyperReps.API` (Presentation)
* **Role:** The entry point. Contains "Thin Controllers" that delegate requests to local bus (Wolverine).
* **Dependencies:** `Application`, `Infrastructure`.

---

## 💻 Frontend Strategy (`/client`)

The frontend is a standalone **Angular** application that consumes the API as a completely separate product.

* **Audio Engine:** Uses a **Web Worker "Conductor"** pattern to handle millisecond-accurate scheduling and gapless transitions, bypassing main-thread throttling on mobile devices.
* **State Management:** Reactive stores handling the complex state of the Mix Editor.

---

## 🛠 Tech Stack

* **Backend:** .NET 10, ASP.NET Core Web API
* **Database:** PostgreSQL (Core Data), Redis (Cache & Queues)
* **Storage:** Cloudflare R2 / AWS S3 (Presigned URLs)
* **Frontend:** Angular, TypeScript, Spotify Web Playback SDK
* **Testing:** xUnit, Moq, Testcontainers

---

## 🚀 Getting Started

### Prerequisites
* .NET 10 SDK
* Node.js (LTS) & NPM
* Docker Desktop (for Postgres & Redis)

### 1. Infrastructure Setup
Spin up the local database and Redis containers:
```bash
docker-compose up -d
```

### 2. Backend Setup
Navigate to the backend source and apply migrations:
```bash
cd src/HyperReps.API
dotnet ef database update
dotnet run
```
The API will be available at https://localhost:5001 (Swagger at /swagger).

3. Frontend Setup
Install dependencies and run the client:
```bash
cd client
npm install
npm start
```
The UI will be available at http://localhost:4200.

## 🧪 Testing (`/tests`)
Tests are separated to ensure fast feedback loops:
* **Unit Tests:** Located in `/tests/Unit`, covering individual components.
* **Integration Tests:** Located in `/tests/Integration`, testing end-to-end scenarios with Testcontainers.

Run all tests using:
```bash
dotnet test
```

## 📚 Documentation (`/docs`)

We maintain strict architectural records. Please review the `/docs` folder before contributing.
* **ADRs:** Architectural Decision Records documenting key decisions.
* **Diagrams:** Mermaid.js diagrams illustrating system architecture and workflows.
* **Guidelines:** Coding standards and best practices.
