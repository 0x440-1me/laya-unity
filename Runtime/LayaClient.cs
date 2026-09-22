using System;
using System.Threading;
using System.Threading.Tasks;
using Laya.Unity.Backends;
using Laya.Unity.Protocol;
using Laya.Unity.Questions;
using Laya.Unity.Transport;

namespace Laya.Unity
{
    public sealed class LayaClient
    {
        private static readonly Uri TypeSafeEndpoint = new Uri("https://api.typesafe.ai");
        private readonly ISystemOneBackend _backend;
        private readonly string _defaultModel;

        public LayaClient(ISystemOneBackend backend, string defaultModel = null)
        {
            _backend = backend ?? throw new ArgumentNullException(nameof(backend));
            _defaultModel = defaultModel;
        }

        public static LayaClient CreateLocal(string endpoint)
        {
            Uri uri;
            if (string.IsNullOrWhiteSpace(endpoint) || !Uri.TryCreate(endpoint, UriKind.Absolute, out uri))
            {
                throw new Errors.LayaConfigurationException("Endpoint must be a valid absolute HTTP or HTTPS URI.");
            }
            return Create(new LayaClientOptions { Endpoint = uri });
        }

        public static LayaClient CreateLocal(LayaClientOptions options)
        {
            return Create(options);
        }

        public static LayaClient CreateTypeSafe(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Errors.LayaConfigurationException("A TypeSafe API key is required for direct development access.");
            }
            return Create(new LayaClientOptions
            {
                Endpoint = TypeSafeEndpoint,
                ApiKey = apiKey,
                DefaultModel = "jev-latest"
            });
        }

        public static LayaClient Create(LayaClientOptions options, ILayaTransport transport = null)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.Validate();
            var backend = new RemoteSystemOneBackend(transport ?? new UnityWebRequestTransport(), options);
            return new LayaClient(backend, options.DefaultModel);
        }

        public Task<SystemOneResult> SystemOneAsync(
            object state,
            QuestionSet questions,
            string model = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var resolvedModel = model ?? _defaultModel;
            var request = new SystemOneRequest(state, questions, resolvedModel);
            return _backend.SystemOneAsync(request, cancellationToken);
        }
    }
}
