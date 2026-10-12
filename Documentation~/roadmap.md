# General-purpose Laya Unity roadmap

**Vision:** One coherent Unity-facing SDK for Laya and compatible System One decision models, usable by any Unity subsystem. **NPCs are one optional application, not the SDK's architecture.** Track the scope correction in [issue #12](https://github.com/0x440-1me/laya-unity/issues/12).

The intended **all-in-one integration experience** means a consistent API and eventual easy installation/configuration across remote and, if proven feasible, embedded backends. It does **not** mean that v0.1 already bundles a model, tokenizer, runtime, Unity Inspector setup, or cloud account.

## Non-negotiable design boundaries

- The core uses domain-neutral state, questions and typed results. Do **not** add NPC, character, dialogue, memory or emotion concepts to required core types.
- `Choice` offers **explicit allowed labels**, `Score` evaluates ordered levels, `Noul` returns a boolean-like prediction/probability. Unity code owns policy, validation, thresholds and execution.
- Backend-specific deployment and credentials remain separate from the caller API. Never ship credentials in a public Unity binary.
- Optional high-level integration components should be *thin adapters*; plain C# access stays first class and the wire protocol stays backend-independent.
- Demos should be varied (UI, cameras, simulation, NPC) rather than treated as architectural requirements.
- Do not claim production readiness, embedded parity, platform support or benchmarks before actual tests.

## Milestone A — Solidify the v0.1 foundation (first)

- [ ] Expand sanitized real-backend golden fixtures (structured/string/array/null state; mixed Choice/Score/Noul; malformed and extension fields).
- [ ] Verify Editor tests in **Unity 2022.3 LTS** and test the actual local Laya + TypeSafe-compatible paths without committing keys.
- [ ] Verify cancellation at scene unload/domain lifecycle, timeout errors, and WebGL/platform limitations.
- [ ] Audit public naming, exceptions, response validation, security/credential policy and AOT compatibility.
- [ ] Document one fully reproducible local-server walkthrough, setup prerequisites and troubleshooting.

Existing issues: [#1–#6](https://github.com/0x440-1me/laya-unity/issues).

## Milestone B — Unity developer ergonomics (v0.2 candidate)

- [ ] Provide a **plain C#** non-NPC integration recipe that works with any subsystem.
- [ ] Design optional Unity Inspector configuration/serialized question presets, diagnostics and attachable MonoBehaviour helpers without requiring them for basic usage.
- [ ] Specify task lifecycle, request cancellation, concurrency and observability for UI, camera, simulation and gameplay callers.
- [ ] Keep state serialization and backend selection flexible; do not hard-code a single NPC input schema.
- [ ] Make the default samples UI Presentation and Scene Direction; retain NPC Decisions as explicitly optional.
- [ ] Add README/API documentation explaining model outputs are *recommendations*, not Unity actions.

**Acceptance:** a new Unity project can import Laya Unity, connect to a compatible backend, and request an arbitrary typed decision **without an NPC GameObject or character framework**.

## Milestone C — Reliability and convenience (v0.2 candidates)

- [ ] [Issue #7](https://github.com/0x440-1me/laya-unity/issues/7): opt-in, bounded retry/429 rate-limit policy respecting Retry-After, cancellation and non-idempotent POST risks.
- [ ] [Issue #8](https://github.com/0x440-1me/laya-unity/issues/8): optional enum-based Choice convenience for AOT-safe applications; keep canonical wire labels string-keyed.
- [ ] Expand generic consumer examples to explicit application-owned camera transition/UI behavior only with safe allowlist and lifecycle tests.

**Acceptance:** existing v0.1 call sites and protocol fixtures continue to work; new convenience APIs are opt-in and backend-agnostic.

## Milestone D — Embedded/all-in-one runtime (research, NOT promised for v0.2)

- [ ] [Issue #9](https://github.com/0x440-1me/laya-unity/issues/9): evaluate Unity Inference Engine/Sentis and ONNX compatibility, threading, startup, target platforms and memory.
- [ ] [Issue #10](https://github.com/0x440-1me/laya-unity/issues/10): byte-identical tokenizer and sequence construction fixtures across Python, Node/ONNX and Unity.
- [ ] [Issue #11](https://github.com/0x440-1me/laya-unity/issues/11): evaluate FP16/INT8 memory/latency/precision/calibration vs FP32.
- [ ] Design legally distributable, separately downloaded model artifacts with integrity checks and versioned compatibility. No weights in this repository.
- [ ] Integrate any verified embedded backend through `ISystemOneBackend`, leaving callers and typed protocol unchanged.

**Acceptance before any shipping claim:** demonstrated parity thresholds, Unity target-platform benchmarks and compliant model distribution. See [embedded Laya roadmap](embedded-laya-roadmap.md).

## Use-case portfolio

| Example | Scope | SDK dependency |
| --- | --- | --- |
| UI Presentation | layout / urgency / contextual hint | `Choice`, `Score`, `Noul` |
| Scene Direction | constrained shot / transition style | `Choice`, `Score` |
| Animation selection | authored gesture / transition labels | `Choice` |
| Simulation/world director | score conditions / choose policy | `Choice`, `Score` |
| NPC Decisions | behavior-selection demo | `Choice`, `Score`, `Noul` |

Examples are **consumers**. No example's domain vocabulary should leak into required runtime classes.

## Release discipline

0.1.0 remains the current package version; no tagged release or Unity Test Runner pass is claimed by this roadmap. Documentation/sample changes alone do not establish a new production-ready runtime version. Merge reviewed documentation/samples after package checks and CI; tag a later runtime release only after matching Unity and backend verification. Distinguish **implemented**, **tests observed**, **planned** and **research-only** in the changelog.
