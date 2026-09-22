using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laya.Unity.Errors;
using Laya.Unity.Protocol;
using Laya.Unity.Questions;
using Laya.Unity.Transport;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Laya.Unity.DotNet.Tests
{
    public sealed class ProtocolSmokeTests
    {
        [Test]
        public void SerializesProtocolAndUnicode()
        {
            var request = new SystemOneRequest(
                new { text = "敵が近い" },
                QuestionSet.Create(
                    ("action", Choice.Create("行動は？", ("wait", (string)null), ("attack", "攻撃"))),
                    ("danger", Score.Create("危険度", "安全", "危険")),
                    ("flee", Noul.Create("逃げる？"))));

            var json = JObject.Parse(ProtocolSerializer.Serialize(request));
            Assert.That((string)json["state"]["text"], Is.EqualTo("敵が近い"));
            Assert.That(json["questions"]["action"]["criteria"]["wait"].Type, Is.EqualTo(JTokenType.Null));
            Assert.That(json["questions"].Children<JProperty>(), Has.Exactly(3).Items);
        }

        [Test]
        public async Task ClientParsesExtensionsAndMetadata()
        {
            var transport = new FakeTransport(new LayaTransportResponse(
                200,
                "{\"model\":\"fixture\",\"answers\":{\"q\":{\"type\":\"noul\",\"noul\":0.9,\"confidence\":0.9,\"extra\":true}},\"usage\":{\"input_tokens\":8,\"output_tokens\":0},\"routing\":{}}"));
            var client = LayaClient.Create(new LayaClientOptions
            {
                Endpoint = new Uri("https://example.test"),
                ApiKey = "test-only"
            }, transport);

            var result = await client.SystemOneAsync("state", QuestionSet.Create(("q", Noul.Create("true?"))));

            Assert.That(result.GetNoul("q").Noul, Is.EqualTo(0.9));
            Assert.That(result.GetNoul("q").ExtensionData.ContainsKey("extra"), Is.True);
            Assert.That(result.ExtensionData.ContainsKey("routing"), Is.True);
            Assert.That(result.Usage.InputTokens, Is.EqualTo(8));
            Assert.That(transport.LastRequest.Headers["Authorization"], Is.EqualTo("Bearer test-only"));
        }

        [Test]
        public void SecurityPolicyRejectsRemoteHttp()
        {
            Assert.Throws<LayaConfigurationException>(() => LayaClient.Create(new LayaClientOptions
            {
                Endpoint = new Uri("http://example.test")
            }, new FakeTransport(null)));
        }

        [Test]
        public void MalformedAnswersFailExplicitly()
        {
            var exception = Assert.Throws<LayaProtocolException>(() =>
                ProtocolSerializer.DeserializeResult("{\"answers\":{\"q\":{\"type\":\"choice\",\"choice\":\"a\",\"probabilities\":{\"a\":\"bad\"},\"confidence\":1}}}"));
            Assert.That(exception.Context, Is.EqualTo("answers.q.probabilities.a"));
        }

        private sealed class FakeTransport : ILayaTransport
        {
            private readonly LayaTransportResponse _response;
            public LayaTransportRequest LastRequest { get; private set; }

            public FakeTransport(LayaTransportResponse response) { _response = response; }

            public Task<LayaTransportResponse> SendAsync(
                LayaTransportRequest request,
                CancellationToken cancellationToken = default)
            {
                LastRequest = request;
                return Task.FromResult(_response);
            }
        }
    }
}
