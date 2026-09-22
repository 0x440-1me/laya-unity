# Release and versioning notes

The package uses Semantic Versioning. While the major version is zero, public API changes may occur in minor releases and will be documented in `CHANGELOG.md`.

## 0.1 scope

- Protocol and builder types for Choice, Score, and Noul.
- Backend-independent client.
- One-attempt UnityWebRequest transport with timeout and cancellation.
- HTTPS and loopback-only HTTP defaults.
- Fixture-based Edit Mode tests.

Not included: registry publishing, production-readiness guarantees, automatic retries, enum convenience helpers, embedded inference, tokenizers, ONNX graphs, or model weights.

The initial source repository is the installation channel. A future release process should create an annotated tag only after Unity tests pass on the supported baseline.
