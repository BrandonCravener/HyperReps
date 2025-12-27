# ADR 005: Object Storage Strategy for User Content

* **Status:** Accepted
* **Date:** 2025-12-27

## Context
Users need to upload custom images (thumbnails/cover art) for the Mixes they create. 
* Storing binary image data (BLOBs) in the primary SQL database causes "database bloat," slows down backups, and increases costs significantly.
* Self-hosting a file storage cluster (e.g., generic NFS, MinIO on-prem) introduces significant operational overhead (disk management, redundancy, backups) and potential security risks if not managed correctly.
* Serving images directly from the Application Server consumes valuable CPU and bandwidth that should be reserved for API logic.

## Decision
We will use an **S3-Compatible Object Storage** solution with the **Presigned URL** pattern.

1.  **Interface:** The application will use the standard AWS S3 SDK for .NET. This decouples the code from the specific provider.
2.  **Provider (Production):** We will use **Cloudflare R2** (or AWS S3). R2 is preferred for this use case due to its zero-egress-fee policy, which is critical for image assets that are downloaded frequently.
3.  **Provider (Development):** Developers will run a local **rustfs** container to simulate the S3 API locally.
4.  **Upload Mechanism:** We will strictly use **Presigned URLs**. The client (Frontend) requests a temporary upload URL from the API, then uploads the binary file *directly* to the Object Storage. The API never handles the file stream.

## Consequences
* **Positive:** **Zero Server Load:** The API server does not process image bytes, saving CPU and RAM.
* **Positive:** **Cost:** Offloads storage to cheap commodity cloud storage; eliminates egress fees (if using R2).
* **Positive:** **Simplicity:** No need to manage physical disks or file system permissions on the server.
* **Negative:** **Complexity:** The frontend requires a two-step upload process (Get URL -> Put File).
* **Negative:** **Consistency:** We must ensure the database `thumbnail_url` is updated only *after* a successful upload to S3.