# Getting started

## Requirements

- Unity 2022.3 LTS or newer.
- A running Laya/System One-compatible HTTP endpoint. The SDK does not install or start one.
- For TypeSafe, a development API key or a developer-controlled production proxy.

Install the package from its Git URL. Unity resolves the pinned Newtonsoft JSON dependency automatically.

## A generic integration flow

Any Unity component or plain C# service can supply:
1. **State** — a JSON-compatible string, anonymous object, array, dictionary or null.
2. **Questions** — permitted options (`Choice`), ordered levels (`Score`) or a yes/no-like recommendation (`Noul`).
3. **Application policy** — what, if anything, to do with the returned decision.

For example, a UI layout selection (no NPC involved):

```csharp
using System.Threading;
using Laya.Unity;
using Laya.Unity.Questions;

var client = LayaClient.CreateLocal("http://127.0.0.1:8000");
var questions = QuestionSet.Create(
    ("layout", Choice.Create(
        "Which allowed presentation mode fits the current screen?",
        ("compact", "Use minimal controls"),
        ("expanded", "Use detailed panels"))),
    ("urgency", Score.Create(
        "How urgent is this UI notification?",
        "low", "medium", "high")),
    ("showHint", Noul.Create("Should a contextual hint be considered?")));

var result = await client.SystemOneAsync(
    new { viewportWidth = 1280, pendingNotifications = 3, focusMode = true },
    questions,
    cancellationToken: CancellationToken.None);

var layout = result.GetChoice("layout").Choice;
var urgency = result.GetScore("urgency").Score;
var hintProbability = result.GetNoul("showHint").Noul;
```

These values are **recommendations**. Your own application checks allowed labels, thresholds and constraints before executing any behavior. The SDK never executes Unity actions for you.

## Cancellable requests and lifecycle

Use a cancellation token tied to the owning component's lifetime (see the sample MonoBehaviours). Cancellation aborts the underlying Unity web request instead of blocking the main thread; handle `OperationCanceledException` as an expected lifecycle event. Avoid overlapping requests in a single component unless explicitly intended.

## Read typed results

`ChoiceResult`, `ScoreResult`, and `NoulResult` expose the appropriate result types; malformed JSON, missing required properties and mismatched typed accessors cause a `LayaProtocolException` with field context. Unknown response properties remain in `ExtensionData`.

## Import optional Unity samples

In Unity Package Manager, select **Laya Unity** and import any of:

- **UI Presentation:** read Inspector fields describing presentation state, request `Choice`/`Score`/`Noul` and log results without automatically editing the UI.
- **Scene Direction:** request a constrained camera-shot suggestion and log it; your camera subsystem controls actual camera transitions.
- **NPC Decisions:** the older gameplay example; useful but not required for any of the APIs.

For each example, add its component to a GameObject, provide a compatible server endpoint, enter Play Mode, then choose the component's **Request ...** context-menu action. Scripts report decisions only; they do not implicitly control the scene.

## See also

[Architecture](architecture.md), [backends](local-laya.md), and [roadmap](roadmap.md).
