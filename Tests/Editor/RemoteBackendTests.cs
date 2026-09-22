using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laya.Unity.Backends;
using Laya.Unity.Errors;
using Laya.Unity.Protocol;
using Laya.Unity.Questions;
using Laya.Unity.Transport;
using NUnit.Framework;

namespace Laya.Unity.Tests
{
    public sealed class RemoteBackendTests
    {
        private static SystemOneRequest Request()
        {
            return new SystemOneRequest("state", QuestionSet.Create(("q", Noul.Create("Is it true?"))));
        }

        [Test]
        public async Task SuccessSendsJsonAndAuthorization()
        {
            var transport = new FakeTransport(new LayaTransportResponse(
                200,
                "{\"model\":\"fixture\",\"answers\":{\"q\":{\"type\":\"noul\",\"noul\":0.75}},\"usage\":{\"input_tokens\":2,\"output_tokens\":0}}"));
            var backend = Backend(transport, "dev-key");

            var result = await backend.SystemOneAsync(Request());

            Assert.That(result.GetNoul("q").Noul, Is.EqualTo(0.75));
            Assert.That(transport.LastRequest.Headers["Authorization"], Is.EqualTo("Bearer dev-key"));
            Assert.That(transport.LastRequest.Endpoint.AbsolutePath, Is.EqualTo("/v1/systemone"));
        }

        [TestCase(400, typeof(LayaRequestException))]
        [TestCase(401, typeof(LayaAuthenticationException))]
        [TestCase(403, typeof(LayaAuthenticationException))]
        [TestCase(408, typeof(LayaRequestException))]
        [TestCase(429, typeof(LayaRateLimitException))]
        [TestCase(500, typeof(LayaRequestException))]
        public void MapsHttpErrors(int status, Type expected)
        {
            var backend = Backend(new FakeTransport(new LayaTransportResponse(
                status,
                "{\"detail\":\"fixture failure\"}",
                new Dictionary<string, string> { ["Retry-After"] = "2", ["x-request-id"] = "req_fixture" })));

            var exception = Assert.ThrowsAsync(expected, async () => await backend.SystemOneAsync(Request()));
            Assert.That(exception.Message, Does.Contain("fixture failure"));
            Assert.That(exception.Message, Does.Not.Contain("dev-key"));
        }

        [Test]
        public void ParsesRateLimitDelay()
        {
            var backend = Backend(new FakeTransport(new LayaTransportResponse(
                429,
                "{}",
                new Dictionary<string, string> { ["retry-after-ms"] = "1250" })));

            var exception = Assert.ThrowsAsync<LayaRateLimitException>(async () => await backend.SystemOneAsync(Request()));
            Assert.That(exception.RetryAfter.Value.TotalMilliseconds, Is.EqualTo(1250));
        }

        [Test]
        public void PropagatesTimeout()
        {
            var backend = Backend(new FakeTransport(new LayaTimeoutException("timed out")));
            Assert.ThrowsAsync<LayaTimeoutException>(async () => await backend.SystemOneAsync(Request()));
        }

        [Test]
        public void HonorsCancellation()
        {
            var source = new CancellationTokenSource();
            source.Cancel();
            var backend = Backend(new FakeTransport(new OperationCanceledException(source.Token)));
            Assert.ThrowsAsync<OperationCanceledException>(async () => await backend.SystemOneAsync(Request(), source.Token));
        }

        [Test]
        public void RejectsInvalidJsonSuccess()
        {
            var backend = Backend(new FakeTransport(new LayaTransportResponse(200, "<html>not json</html>")));
            Assert.ThrowsAsync<LayaProtocolException>(async () => await backend.SystemOneAsync(Request()));
        }

        private static RemoteSystemOneBackend Backend(FakeTransport transport, string apiKey = null)
        {
            return new RemoteSystemOneBackend(transport, new LayaClientOptions
            {
                Endpoint = new Uri("https://example.test"),
                ApiKey = apiKey,
                Timeout = TimeSpan.FromSeconds(1)
            });
        }

        private sealed class FakeTransport : ILayaTransport
        {
            private readonly LayaTransportResponse _response;
            private readonly Exception _exception;
            public LayaTransportRequest LastRequest { get; private set; }

            public FakeTransport(LayaTransportResponse response) { _response = response; }
            public FakeTransport(Exception exception) { _exception = exception; }

            public Task<LayaTransportResponse> SendAsync(
                LayaTransportRequest request,
                CancellationToken cancellationToken = default(CancellationToken))
            {
                LastRequest = request;
                if (_exception != null) return Task.FromException<LayaTransportResponse>(_exception);
                return Task.FromResult(_response);
            }
        }
    }
}
