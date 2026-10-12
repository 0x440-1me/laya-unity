# NPC Decisions sample (optional)

`NpcDecisionExample.cs` demonstrates one request containing Choice, Score, and Noul questions. Set `Endpoint` to a running local Laya-compatible server, then call **Request Decision** from the component context menu while the game is running.

The sample offers legal actions to the model; it does not let the model execute gameplay actions directly.

This sample is one **optional application** of the general-purpose Laya Unity SDK. Other Unity systems (UI, cameras, simulations, editors) use the same `LayaClient` and question/result APIs without requiring NPCs or characters. See [general-purpose examples](../../Documentation~/getting-started.md).
