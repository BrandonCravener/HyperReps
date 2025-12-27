# ADR 003: Asynchronous Playlist Synchronization

* **Status:** Accepted
* **Date:** 2025-12-26

## Context
Users import playlists that may contain hundreds of tracks. Fetching metadata for all these tracks from Spotify takes time and is subject to rate limiting.

Processing this import synchronously (during the HTTP Request) would result in poor user experience, browser timeouts, and potential data inconsistency if the user closes the tab.

## Decision
We will implement an **Asynchronous Job Queue** architecture.

1.  **Redis** will be used as the backing store for the queue.
2.  **BullMQ** (or a similar .NET compatible library like Hangfire/MassTransit) will manage the job lifecycle.
3.  The SQL Database (`PLAYLIST` table) will only store the *status* of the sync (`QUEUED`, `SYNCING`, `COMPLETED`, `FAILED`) to inform the UI.
4.  A separate "Worker Service" will consume jobs, handle Spotify API rate limits (exponential backoff), and upsert data to the SQL database.

## Consequences
* **Positive:** Decouples the UI from long-running processes; the user gets immediate feedback ("Import Started").
* **Positive:** Robustness; failed jobs can be retried automatically without user intervention.
* **Positive:** Scalability; we can increase the number of worker instances independently of the API server to handle high load.
* **Negative:** Adds an infrastructure dependency (Redis) which must be managed and monitored.