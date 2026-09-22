# Contributing

Thanks for helping make Laya Unity dependable.

## Before opening a pull request

1. Open or find an issue for behavior changes larger than a typo.
2. Keep the public API backend-independent; HTTP and inference are separate abstractions.
3. Add or update protocol fixtures and tests.
4. Do not commit API keys, model weights, generated Unity folders, or third-party source without license review.
5. Update `CHANGELOG.md` for user-visible changes.

Use a focused feature branch and a clear commit message. Pull requests should explain the protocol or gameplay behavior affected and how it was tested.

## Compatibility principles

- Target Unity 2022.3 LTS and the .NET Standard 2.1-compatible API surface Unity provides.
- Preserve unknown response fields.
- Avoid validating rules that are not part of the public System One contract.
- Never log authorization headers or API keys.
- Normal tests must not require a live service.

## Testing

Editor tests live in `Tests/Editor`. Import this package into a Unity 2022.3+ test project, add `com.0x4401me.laya.unity` to the project's `testables`, and run Edit Mode tests from Test Runner.

CI performs license, manifest, fixture, secret-pattern, and repository-shape checks without Unity credentials. A licensed Unity test job may be added later, but it must not be represented as available until it is actually configured.
