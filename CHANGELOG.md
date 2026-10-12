# Changelog

All notable changes to this project will be documented here. The project follows [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Documentation and samples

- Clarify that the SDK integrates Laya/System One decisions into any Unity subsystem, not only NPCs.
- Replace the NPC-only quickstart with a UI presentation example and document core/client boundaries.
- Add optional UI Presentation and Scene Direction sample components; retain NPC Decisions as a separate optional sample.
- Add a domain-neutral v0.2 developer-experience roadmap, with embedded inference explicitly still research-only.
- No core runtime API, protocol, model distribution, or production-readiness change in this documentation/sample update.

## [0.1.0] - 2026-09-22

### Added

- Unity-native System One client and protocol types.
- Choice, Score, and Noul question builders.
- Backend-independent `ISystemOneBackend` boundary.
- `UnityWebRequest` transport with cancellation and timeout support.
- Endpoint security policy that permits HTTPS and loopback HTTP by default.
- Typed result accessors, usage metadata, and tolerant extension data.
- Editor tests, golden JSON fixtures, documentation, and NPC sample.

[Unreleased]: https://github.com/0x440-1me/laya-unity/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/0x440-1me/laya-unity/releases/tag/v0.1.0
