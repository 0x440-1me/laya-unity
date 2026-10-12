# UI Presentation sample

This is a **general-purpose (non-NPC)** example of asking Laya/System One for constrained UI recommendations.

1. Import **UI Presentation** from the Laya Unity Package Manager samples.
2. Add `UiPresentationExample` to any GameObject.
3. Set `Endpoint` to a running Laya-compatible service (default `http://127.0.0.1:8000`).
4. Enter Play Mode, then use the component context menu **Request UI Decision**.
5. Inspect the Console for the suggested layout, urgency score, and hint probability.

This sample **logs recommendations only**. It does not alter GameObjects, switch layouts, or implement notifications. Your application must define thresholds and decide what to do with the typed results. The SDK never executes actions automatically.

The sample cancels an in-flight request on component disable and ignores a duplicate invocation while awaiting the current response.
