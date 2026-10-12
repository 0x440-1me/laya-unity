# Scene Direction sample

This sample demonstrates using Laya/System One with a **camera or scene presentation subsystem**, not an NPC.

1. Import **Scene Direction** from the Laya Unity Package Manager samples.
2. Add `SceneDirectionExample` to a GameObject.
3. Set `Endpoint` to a running Laya-compatible service.
4. Enter Play Mode and choose **Request Scene Direction** from the component context menu.
5. Read the proposed `wide` / `medium` / `close` shot and ordered transition score in the Console.

The example **does not control a Unity Camera, change scenes, or choose arbitrary commands**. In a real project, implement an allowlisted mapping from the typed result to a camera system you own. The model can only select among options presented in `Choice`.

The component cancels requests on disable and refuses a second in-flight request.
