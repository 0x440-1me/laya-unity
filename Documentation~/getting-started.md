# Getting started

## Requirements

- Unity 2022.3 LTS or newer.
- A Laya/System One-compatible HTTP endpoint.
- For TypeSafe, a development API key or a developer-controlled proxy.

Install the package from its Git URL. Unity resolves the pinned Newtonsoft JSON dependency automatically.

## Create questions

```csharp
var questions = QuestionSet.Create(
    ("action", Choice.Create(
        "What should the NPC do?",
        ("attack", "Engage the enemy"),
        ("retreat", "Move toward safety"),
        ("wait", "Hold position"))),
    ("danger", Score.Create(
        "How dangerous is the situation?",
        "safe", "moderate", "extreme")),
    ("flee", Noul.Create("Should the NPC flee?")));
```

Score criteria are ordered from index zero and need at least two levels. Choice labels must be non-empty. Noul can optionally include descriptions for true and false.

## Send state

State can be a string, anonymous object, array, dictionary, or null. JSON-compatible values are serialized by Newtonsoft.Json.

```csharp
var result = await client.SystemOneAsync(
    new { health = 25, ammo = 2 },
    questions,
    cancellationToken: destroyCancellationToken);
```

Cancellation aborts the underlying Unity web request and does not block the main thread.

## Read typed results

```csharp
ChoiceResult action = result.GetChoice("action");
ScoreResult danger = result.GetScore("danger");
NoulResult flee = result.GetNoul("flee");
```

Malformed JSON, missing required answer properties, or mismatched accessors throw `LayaProtocolException` with a field context. Unknown response properties are preserved in `ExtensionData`.
