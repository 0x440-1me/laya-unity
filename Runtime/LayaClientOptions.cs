using System;
using Laya.Unity.Errors;

namespace Laya.Unity
{
    public sealed class LayaClientOptions
    {
        public Uri Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string DefaultModel { get; set; }
        public bool AllowInsecureHttp { get; set; }
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

        public LayaClientOptions Clone()
        {
            return new LayaClientOptions
            {
                Endpoint = Endpoint,
                ApiKey = ApiKey,
                DefaultModel = DefaultModel,
                AllowInsecureHttp = AllowInsecureHttp,
                Timeout = Timeout
            };
        }

        internal void Validate()
        {
            EndpointPolicy.Validate(Endpoint, AllowInsecureHttp);

            if (Timeout <= TimeSpan.Zero)
            {
                throw new LayaConfigurationException("Timeout must be greater than zero.");
            }

            if (DefaultModel != null && string.IsNullOrWhiteSpace(DefaultModel))
            {
                throw new LayaConfigurationException("DefaultModel must be non-empty when supplied.");
            }
        }
    }

    internal static class EndpointPolicy
    {
        internal static void Validate(Uri endpoint, bool allowInsecureHttp)
        {
            if (endpoint == null || !endpoint.IsAbsoluteUri)
            {
                throw new LayaConfigurationException("Endpoint must be an absolute HTTP or HTTPS URI.");
            }

            if (endpoint.Scheme == Uri.UriSchemeHttps)
            {
                return;
            }

            if (endpoint.Scheme != Uri.UriSchemeHttp)
            {
                throw new LayaConfigurationException("Endpoint must use HTTP or HTTPS.");
            }

            if (!endpoint.IsLoopback && !allowInsecureHttp)
            {
                throw new LayaConfigurationException(
                    "Plain HTTP is allowed only for loopback endpoints. Set AllowInsecureHttp=true to opt in for another host.");
            }
        }

        internal static Uri ResolveSystemOneEndpoint(Uri endpoint)
        {
            var trimmed = endpoint.AbsolutePath.TrimEnd('/');
            if (trimmed.EndsWith("/v1/systemone", StringComparison.OrdinalIgnoreCase))
            {
                return endpoint;
            }

            var builder = new UriBuilder(endpoint);
            builder.Path = trimmed + "/v1/systemone";
            return builder.Uri;
        }

        internal static string SafeContext(Uri endpoint)
        {
            var builder = new UriBuilder(endpoint)
            {
                Query = string.Empty,
                Fragment = string.Empty,
                UserName = string.Empty,
                Password = string.Empty
            };
            return "POST " + builder.Uri.GetLeftPart(UriPartial.Path);
        }
    }
}
