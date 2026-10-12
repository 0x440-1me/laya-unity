# Architecture

The **laya-unity** core is a backend-independent integration SDK for arbitrary Unity systems. It owns structured request/response plumbing, not NPCs, simulation policy, or application behavior.

```text
UI   Scene/Cameras   Animation   Simulation   Game Director   NPCs   Other
                           │
                state + allowed outcomes
                           │
                           ▼
                     LayaClient
           Choice / Score / Noul + typed results
                           │
                           ▼
                   ISystemOneBackend
                           │
            ┌──────────────┴──────────────┐
            ▼                             ▼
 RemoteSystemOneBackend         Future embedded backend
     (implemented)                  (research only)
            │
       ILayaTransport
            │
   UnityWebRequestTransport
            │
  Laya-compatible / TypeSafe-compatible endpoint
```

`LayaClient` depends only on `ISystemOneBackend`; transport and inference remain separable. The current concrete backend is **remote**, using HTTP transport. Neither embedded inference nor ready-made Unity Inspector components ship in 0.1.

## Core invariants

1. **No NPC dependency:** runtime/protocol contracts do not require characters, agents, memory, dialogue, or gameplay-specific state.
2. **Caller-owned policy:** Unity defines valid choices, validates outputs and performs all state changes. The SDK never directly moves a camera, modifies UI, animates a character, or commands a game entity.
3. **Backend independence:** state/questions/results remain the same whether the transport is a supported remote service or a future locally embedded backend.
4. **Composable Unity integration:** developers can use the plain C# API from MonoBehaviours, services, simulations or editor tooling; optional convenience components must not become core requirements.
5. **Explicit lifecycle and security:** bounded timeouts, cancellation, endpoint policy, credential handling and clear failure states are relevant to *every* caller, not just game AI.

## Compatibility boundary

The public protocol implements `choice`, ordered `score`, and `noul`. The parser requires fields needed to use each typed result, but preserves unknown properties at result and answer levels, so compatible services can return additional routing, action, or latency metadata.

Version 0.1 performs **one** request attempt. A bounded, explicitly configured retry policy is proposed for 0.2; silent or unbounded retries are inappropriate because retrying a POST can duplicate processing or cost.

## Errors

Configuration, protocol, connection, timeout, authentication, rate-limit, and other HTTP request failures have distinct exception types. HTTP status and sanitized endpoint details are retained. Authorization headers are never attached to exceptions.

## Samples vs. engine features

UI Presentation and Scene Direction demonstrate **consumer-owned** state and outputs. NPC Decisions remains an optional consumer example, not a dependency or architectural template. The SDK does not itself implement a UI layout system, a camera controller, an agent brain, an emotion system or memory.

See the [general-purpose roadmap](roadmap.md) for prioritized follow-up and explicit non-goals.
