# TypeSafe-compatible endpoints

For local development:

```csharp
var client = LayaClient.CreateTypeSafe("development-api-key");
```

This selects `https://api.typesafe.ai/v1/systemone` and defaults to `jev-latest`.

## Shipping games

A public Unity binary cannot safely hide an API key. Obfuscation and ScriptableObjects do not turn a client secret into a server secret. For released games, use:

```text
Unity Game → your authenticated backend → TypeSafe
```

Your backend can enforce player identity, rate limits, request bounds, and key rotation. Never commit production keys or put them in Unity assets.

The SDK redacts credentials by construction: request exceptions include a sanitized method and endpoint, status, request ID, and bounded server detail, but never request headers.
