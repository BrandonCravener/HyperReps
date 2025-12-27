# ADR 004: Caching Strategy for External Metadata

* **Status:** Accepted
* **Date:** 2025-12-26

## Context
HyperReps mixes are composed of tracks sourced from Spotify. If the application relied solely on real-time calls to Spotify for track metadata (Title, Artist, Album Art), the application would be fragile.

* If a user deletes the source playlist on Spotify, the Mix would break.
* If Spotify changes metadata, the Mix history would change.
* Loading a mix would trigger hundreds of API calls, hitting rate limits immediately.

## Decision
We will **Aggressively Cache** track metadata into a local `TRACKS` table.

1.  When a playlist is imported, we copy the relevant metadata (Title, Artist, Duration, Images) to our database.
2.  The application will treat our local database as the **Source of Truth** for all visual display logic.
3.  We will only use the Spotify API for the actual *audio playback stream* (using the stored `spotify_track_id`).

## Consequences
* **Positive:** **Data Integrity:** Mixes remain valid and viewable even if the original Spotify content is moved or deleted.
* **Positive:** **Performance:** Reading mix data becomes a fast SQL join rather than multiple HTTP calls to Spotify.
* **Negative:** **Stale Data:** If an artist updates their name or album art on Spotify, our local copy will be outdated until a re-sync is triggered.
* **Negative:** **Storage:** We are duplicating data that exists elsewhere, increasing storage requirements.