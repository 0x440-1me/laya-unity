# Laya Unity

**A general-purpose Unity SDK for Laya and compatible System One decision models.**

```text
Any Unity subsystem (UI, camera, simulation, game director, NPC, ...)
         │  provides state + allowed outcomes
         ▼
    LayaClient  —  Choice / Score / Noul
         │
         ▼
Laya / compatible System One backend
         │  structured, typed result
         ▼
    Calling Unity subsystem decides what to do
```

Laya Unity provides a single Unity-native, backend-independent integration surface for **any system needing constrained decisions**. It is **not an NPC framework**: NPCs are one optional use case, alongside UI presentation, camera/scene direction, animation selection, simulation policies, and game/world direction. The SDK does not decide what Unity code is authorized to execute; the caller defines the options and applies the result.

Version **0.1** is a **transport-first SDK**, not an all-in-one model distribution: it currently connects to local Laya-compatible servers and TypeSafe-compatible System One endpoints. It does **not** bundle or run an embedded ONNX model, tokenizer, Python runtime, server, or model weights.

> [!IMPORTANT]
> This project is an early community SDK, not production-ready software and not affiliated with Laya or TypeSafe.

## Install

In Unity **2022.3 LTS or newer**, open **Window → Package Manager**, choose **Add package from git URL**, and enter:

```text
https://github.com/0x440-1me/laya-unity.git
```

The package depends on Unity's maintained `com.unity.nuget.newtonsoft-json` package, currently pinned to `3.2.2`. Configure a compatible server separately.

## Quick start: a UI presentation decision (no NPC required)

This example asks for a layout, a relative urgency score, and a yes/no-style recommendation. It can run inside an async method in any Unity component or service:

```csharp
using System.Threading;
using Laya.Unity;
using Laya.Unity.Questions;
using UnityEngine;

var client = LayaClient.CreateLocal("http://127.0.0.1:8000");

var result = await client.SystemOneAsync(
    state: new
    {
        screenWidth = Screen.width,
        pendingNotifications = 3,
        focusMode = true
    },
    questions: QuestionSet.Create(
        ("layout", Choice.Create(
            "Which allowed UI layout fits the current state?",
            ("compact", "Show essential controls only"),
            ("expanded", "Show the full information panel"))),
        ("urgency", Score.Create(
            "How urgent is the pending information?",
            "low", "medium", "high")),
        ("showHint", Noul.Create("Should the interface show a contextual hint?"))),
    cancellationToken: CancellationToken.None);

var layout = result.GetChoice("layout").Choice;
var urgency = result.GetScore("urgency").Score;
var showHintProbability = result.GetNoul("showHint").Noul;
Debug.Log($"Layout: {layout}; urgency score: {urgency}; hint probability: {showHintProbability}");
```

**Unity owns the resulting behavior.** Only values explicitly offered in a `Choice` question are valid, and any score/probability threshold must be chosen by the application. No actions are automatically executed by the SDK. Handle exceptions, cancellation and lifecycle in real components; see [Getting started](Documentation~/getting-started.md) and the samples.

The default local endpoint is `POST /v1/systemone`. An endpoint URI already ending in `/v1/systemone` is used as-is.

## Where it fits

| Consumer | Example use of typed decisions |
| --- | --- |
| UI / UX | Choose an allowed layout, prioritize notifications or assess hints |
| Camera / scene direction | Select a permitted shot or scene transition |
| Animation systems | Choose an authored transition or gesture |
| Game / world director | Choose pacing or encounter emphasis from safe options |
| Simulation / automation | Evaluate conditions and select an application-defined policy |
| NPCs (optional) | Choose behavior from a restricted action set |
| Your own subsystem | Send structured state and receive `Choice`, `Score` or `Noul` |

The same core API serves all of these. No character, agent, dialogue, memory, personality or emotion system is required.

## Why structured decisions instead of free-form text?

Laya and compatible System One models return constrained, typed decisions rather than unrestricted text. The caller supplies the legal choices and remains responsible for interpreting and executing results. A decision model is **not** an authority over game rules, application state, permissions, or validation.

## Backends

### Local Laya-compatible server

Loopback HTTP is permitted by default:

```csharp
var client = LayaClient.CreateLocal("http://localhost:8000");
```

HTTPS is allowed for any host. Plain HTTP to a non-loopback host is rejected unless `AllowInsecureHttp` is explicitly set.

### TypeSafe-compatible endpoint

```csharp
var client = LayaClient.CreateTypeSafe("development-api-key");
```

Direct API keys are suitable **only for local development**. A distributed Unity binary cannot safely hide an API key. For public applications, use:

```text
Unity application → developer-controlled backend/proxy → TypeSafe
```

Never commit keys or store production credentials in Unity assets.

## Supported protocol (v0.1)

- Requests: string or structured `state`, named `questions`, optional `model`.
- Questions: `choice`, ordered `score`, and `noul`.
- Answers: choice values, confidence/probabilities where present, scores, noul probabilities and legends.
- Metadata: model and input/output token usage.
- Compatibility: preserve unknown response properties in extension data rather than silently losing them.
- Runtime: backend abstraction, UnityWebRequest transport, cancellation, timeouts, structured errors and HTTPS/loopback endpoint policy.

## Unity samples

Available through Package Manager sample import:

- **UI Presentation** — generic UI layout / urgency / hint decisions.
- **Scene Direction** — an allowed camera-shot choice based on scene state.
- **NPC Decisions** — an *optional* gameplay-specific example retained for existing users.

Samples demonstrate decisions and may only log results; they are **not** complete UI, camera or NPC frameworks. See [samples and setup](Documentation~/getting-started.md).

## Status and roadmap

0.1 establishes protocol, backend, transport, validation, and error boundaries. The next priority is **general-purpose Unity integration quality**: backend fixtures, Unity lifecycle/cancellation tests, developer ergonomics and clear multi-subsystem samples. Bounded retries and optional enum helpers remain v0.2 candidates, not shipped guarantees.

An Inspector-first workflow and embedded Laya inference are **future work**. Embedded inference requires tokenizer and sequence parity, ONNX and Unity Inference Engine compatibility, numerical calibration, platform tests and separate model distribution. Model weights will not be committed here.

Read [the general-purpose roadmap](Documentation~/roadmap.md), [architecture](Documentation~/architecture.md), and [0.1 release notes](Documentation~/release-notes.md). Track the scope correction in [issue #12](https://github.com/0x440-1me/laya-unity/issues/12).

## Contributing

Issues and focused pull requests are welcome. Read [CONTRIBUTING.md](CONTRIBUTING.md), [SECURITY.md](SECURITY.md), and the [Code of Conduct](CODE_OF_CONDUCT.md).

## License

MIT. See [LICENSE.md](LICENSE.md) and [Third Party Notices.md](Third%20Party%20Notices.md).
