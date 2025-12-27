# ADR 002: Authentication Provider Strategy

* **Status:** Accepted
* **Date:** 2025-12-26

## Context
The application requires persistent access to Spotify APIs to perform background tasks (playlist synchronization) even when the user is offline. This requires storing and refreshing OAuth Access Tokens.

We evaluated using an external Identity Provider (Keycloak) versus a native ASP.NET Core implementation. Keycloak introduces a "Token Hop" issue where the Spotify tokens are stored inside Keycloak, requiring complex API calls to extract them for our background workers.

## Decision
We will use **Native ASP.NET Core OAuth** via the `AspNet.Security.OAuth.Spotify` package.

1.  We will handle the OAuth handshake directly in the backend.
2.  We will encrypt and store the `access_token` and `refresh_token` directly in our own `Users` table upon a successful `OnTokenValidated` event.
3.  We will implement a `TokenService` to handle decryption and refreshing of tokens for background jobs.

## Consequences
* **Positive:** Significantly simplifies the infrastructure by removing the need for a dedicated Identity container (JVM/Keycloak).
* **Positive:** Provides immediate, direct access to the tokens needed for the core "Sync" feature without extra API calls.
* **Positive:** Improves User Experience by removing the intermediate "Login to Keycloak" redirection step.
* **Negative:** We assume full responsibility for the security (encryption at rest) of the tokens.
* **Negative:** Tighter coupling between the application code and the specific OAuth provider (Spotify).