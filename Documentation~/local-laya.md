# Local Laya-compatible server

```csharp
var client = LayaClient.CreateLocal("http://127.0.0.1:8000");
```

The client posts to `/v1/systemone`. You can also pass a complete URI ending in `/v1/systemone`.

Loopback HTTP is allowed for `localhost`, `127.0.0.1`, and IPv6 loopback. Other HTTP hosts are denied unless explicitly enabled:

```csharp
var client = LayaClient.CreateLocal(new LayaClientOptions
{
    Endpoint = new Uri("http://development-lan-host:8000"),
    AllowInsecureHttp = true
});
```

Use that opt-in only on a trusted development network. Prefer HTTPS everywhere else.

The SDK does not install Python, start a server, download a model, or bundle weights. Consult your chosen Laya-compatible server for its setup and model license.
