# Architecture

```text
Unity game
   │
   ▼
LayaClient ── protocol questions and typed answers
   │
   ▼
ISystemOneBackend
   │
   ▼
RemoteSystemOneBackend ── serialization, status/error mapping
   │
   ▼
ILayaTransport
   │
   ▼
UnityWebRequestTransport
```

The HTTP transport and inference backend are separate abstractions. `LayaClient` only depends on `ISystemOneBackend`, so a future embedded implementation can use the same questions and results without pretending local inference is HTTP.

## Compatibility boundary

The public protocol implements `choice`, ordered `score`, and `noul`. The parser requires the typed fields needed to use an answer but retains unknown fields at the result and answer levels. This allows compatible Laya servers to return routing, action, latency, or other extensions.

Version 0.1 performs one request attempt. The transport boundary is intentionally ready for an explicit retry policy in 0.2. Silent or infinite retries are not acceptable for gameplay, and retrying a POST may duplicate processing or cost.

## Errors

Configuration, protocol, connection, timeout, authentication, rate-limit, and other HTTP request failures have distinct exception types. HTTP status and a sanitized endpoint are retained. Authorization headers are never attached to exceptions.
