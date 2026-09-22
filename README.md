# Laya Unity

Laya Unity is a community Unity SDK for System One decision models.

```text
game state
   ↓
Choice / Score / Noul
   ↓
Laya / compatible backend
   ↓
structured decision
```

It provides a Unity-native, backend-independent client for local Laya-compatible servers and TypeSafe-compatible System One endpoints. Version 0.1 is deliberately transport-first: it does not embed ONNX, a tokenizer, Python, or model weights.

> [!IMPORTANT]
> This project is an early community SDK, not production-ready software and not affiliated with Laya or TypeSafe.

## Install

In Unity 2022.3 LTS or newer, open **Window → Package Manager**, choose **Add package from git URL**, and enter:

```text
https://github.com/0x440-1me/laya-unity.git
```

The package depends on Unity's maintained `com.unity.nuget.newtonsoft-json` package, currently pinned to `3.2.2`.

## Quick start: local Laya

```csharp
using System.Threading;
using Laya.Unity;
using Laya.Unity.Questions;
using UnityEngine;

var client = LayaClient.CreateLocal("http://127.0.0.1:8000");

var result = await client.SystemOneAsync(
    state: new
    {
        health = 25,
        enemyDistance = 4.2f,
        ammo = 2
    },
    questions: QuestionSet.Create(
        ("action", Choice.Create(
            "What should the NPC do?",
            ("attack", "Engage the enemy"),
            ("retreat", "Move toward safety"),
            ("wait", "Hold position"))),
        ("danger", Score.Create(
            "How dangerous is the situation?",
            "safe", "moderate danger", "extreme danger")),
        ("flee", Noul.Create("Should the NPC flee?"))),
    cancellationToken: CancellationToken.None);

var action = result.GetChoice("action");
Debug.Log($"{action.Choice} ({action.Confidence:P0})");
```

The default local endpoint is `POST /v1/systemone`. If the supplied URI already ends in `/v1/systemone`, it is used as-is.

## Why a decision model instead of an LLM?

Laya and compatible System One models return constrained, typed decisions rather than free-form text. Unity supplies the legal actions and remains responsible for executing them.

If Unity offers `attack`, `retreat`, and `wait`, the model chooses among those options and returns probabilities. It cannot invent `teleport`, `spawn weapon`, or `delete enemy` unless the game explicitly offers those actions.

This makes the SDK useful as one decision layer inside game AI, not as an authority over game rules.

## Backends

### Local Laya-compatible server

Loopback HTTP is allowed by default:

```csharp
var client = LayaClient.CreateLocal("http://localhost:8000");
```

HTTPS is allowed for any host. Plain HTTP to a non-loopback host is rejected unless `AllowInsecureHttp` is explicitly set.

### TypeSafe-compatible endpoint

```csharp
var client = LayaClient.CreateTypeSafe("development-api-key");
```

Direct API keys are suitable only for local development. A released Unity binary cannot safely hide a TypeSafe API key. For public games, use:

```text
Unity Game → developer backend → TypeSafe
```

Never commit keys or store a production key in a Unity asset.

## Supported protocol

- Requests: structured or string `state`, named `questions`, and optional `model`.
- Questions: `choice`, ordered `score`, and `noul`.
- Answers: choice values, expected scores, noul probabilities, probability maps, legends, and confidence where present.
- Metadata: model and input/output token usage.
- Compatibility: unknown response properties are retained in extension data instead of rejected.

See [Getting started](Documentation~/getting-started.md), [architecture](Documentation~/architecture.md), and the [0.1 release notes](Documentation~/release-notes.md).

## Status and roadmap

0.1 establishes the protocol, backend, transport, validation, and error boundaries. Conservative retry support is deferred to 0.2 so gameplay code never receives an unreviewed retry policy.

Embedded Laya is a research milestone. It will require tokenizer, sequence construction, ONNX, calibration, model-distribution, and Python/Node/Unity parity work. Model weights will remain separate from this repository.

## Contributing

Issues and focused pull requests are welcome. Read [CONTRIBUTING.md](CONTRIBUTING.md), [SECURITY.md](SECURITY.md), and the [Code of Conduct](CODE_OF_CONDUCT.md).

## License

MIT. See [LICENSE.md](LICENSE.md) and [Third Party Notices.md](Third%20Party%20Notices.md).
