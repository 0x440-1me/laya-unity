using System;
using Laya.Unity.Errors;
using NUnit.Framework;

namespace Laya.Unity.Tests
{
    public sealed class EndpointPolicyTests
    {
        [TestCase("https://example.com")]
        [TestCase("http://localhost:8000")]
        [TestCase("http://127.0.0.1:8000")]
        [TestCase("http://[::1]:8000")]
        public void AllowsSecureAndLoopbackEndpoints(string endpoint)
        {
            Assert.DoesNotThrow(() => new LayaClientOptions { Endpoint = new Uri(endpoint) }.Validate());
        }

        [Test]
        public void DeniesRemotePlainHttpByDefault()
        {
            Assert.Throws<LayaConfigurationException>(() =>
                new LayaClientOptions { Endpoint = new Uri("http://example.com") }.Validate());
        }

        [Test]
        public void ExplicitlyAllowsRemotePlainHttp()
        {
            Assert.DoesNotThrow(() => new LayaClientOptions
            {
                Endpoint = new Uri("http://example.com"),
                AllowInsecureHttp = true
            }.Validate());
        }

        [Test]
        public void FactoriesRejectInvalidEndpointAndMissingTypeSafeKey()
        {
            Assert.Throws<LayaConfigurationException>(() => LayaClient.CreateLocal("not a uri"));
            Assert.Throws<LayaConfigurationException>(() => LayaClient.CreateTypeSafe(" "));
        }

        [Test]
        public void AppendsProtocolPathOnce()
        {
            Assert.That(
                EndpointPolicy.ResolveSystemOneEndpoint(new Uri("https://example.com")).AbsolutePath,
                Is.EqualTo("/v1/systemone"));
            Assert.That(
                EndpointPolicy.ResolveSystemOneEndpoint(new Uri("https://example.com/v1/systemone")).AbsolutePath,
                Is.EqualTo("/v1/systemone"));
        }
    }
}
