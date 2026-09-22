using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laya.Unity.Errors;
using Laya.Unity.Protocol;
using Laya.Unity.Transport;
using Newtonsoft.Json.Linq;

namespace Laya.Unity.Backends
{
    public sealed class RemoteSystemOneBackend : ISystemOneBackend
    {
        private readonly ILayaTransport _transport;
        private readonly LayaClientOptions _options;
        private readonly Uri _systemOneEndpoint;

        public RemoteSystemOneBackend(ILayaTransport transport, LayaClientOptions options)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _options = options != null ? options.Clone() : throw new ArgumentNullException(nameof(options));
            _options.Validate();
            _systemOneEndpoint = EndpointPolicy.ResolveSystemOneEndpoint(_options.Endpoint);
        }

        public async Task<SystemOneResult> SystemOneAsync(
            SystemOneRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                headers["Authorization"] = "Bearer " + _options.ApiKey;
            }

            var transportRequest = new LayaTransportRequest
            {
                Endpoint = _systemOneEndpoint,
                Body = ProtocolSerializer.Serialize(request),
                Headers = headers,
                Timeout = _options.Timeout
            };

            var response = await _transport.SendAsync(transportRequest, cancellationToken);
            if (response == null)
            {
                throw new LayaConnectionException("The transport returned no response.");
            }

            if (response.StatusCode < 200 || response.StatusCode >= 300)
            {
                throw CreateRequestException(response);
            }

            try
            {
                return ProtocolSerializer.DeserializeResult(response.Body);
            }
            catch (LayaProtocolException exception)
            {
                throw new LayaProtocolException(
                    exception.Message + " Response from " + EndpointPolicy.SafeContext(_systemOneEndpoint) + ".",
                    exception.Context,
                    exception);
            }
        }

        private LayaRequestException CreateRequestException(LayaTransportResponse response)
        {
            var context = EndpointPolicy.SafeContext(_systemOneEndpoint);
            var requestId = Header(response.Headers, "x-request-id") ?? Header(response.Headers, "request-id");
            var detail = SafeServerDetail(response.Body);
            var message = context + " returned HTTP " + response.StatusCode.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(detail)) message += ": " + detail;

            if (response.StatusCode == 401 || response.StatusCode == 403)
            {
                return new LayaAuthenticationException(message, response.StatusCode, context, requestId);
            }
            if (response.StatusCode == 408)
            {
                return new LayaRequestException(message, response.StatusCode, context, requestId);
            }
            if (response.StatusCode == 429)
            {
                return new LayaRateLimitException(
                    message,
                    response.StatusCode,
                    context,
                    ParseRetryAfter(response.Headers),
                    requestId);
            }
            return new LayaRequestException(message, response.StatusCode, context, requestId);
        }

        private static string Header(IReadOnlyDictionary<string, string> headers, string name)
        {
            if (headers == null) return null;
            var pair = headers.FirstOrDefault(item => string.Equals(item.Key, name, StringComparison.OrdinalIgnoreCase));
            return pair.Key == null ? null : pair.Value;
        }

        private static TimeSpan? ParseRetryAfter(IReadOnlyDictionary<string, string> headers)
        {
            double milliseconds;
            var ms = Header(headers, "retry-after-ms");
            if (double.TryParse(ms, NumberStyles.Float, CultureInfo.InvariantCulture, out milliseconds) && milliseconds >= 0)
            {
                return TimeSpan.FromMilliseconds(milliseconds);
            }

            double seconds;
            var raw = Header(headers, "retry-after");
            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out seconds) && seconds >= 0)
            {
                return TimeSpan.FromSeconds(seconds);
            }

            DateTimeOffset date;
            if (DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date))
            {
                var delay = date - DateTimeOffset.UtcNow;
                return delay < TimeSpan.Zero ? TimeSpan.Zero : delay;
            }
            return null;
        }

        private static string SafeServerDetail(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return null;

            try
            {
                var root = JToken.Parse(body);
                var detail = root["detail"] ?? root["message"] ?? root["error"];
                if (detail != null)
                {
                    var value = detail.Type == JTokenType.String ? detail.Value<string>() : detail.ToString(Newtonsoft.Json.Formatting.None);
                    return Truncate(value, 512);
                }
            }
            catch (Newtonsoft.Json.JsonException)
            {
                // A short plain-text server error is still useful.
            }
            return Truncate(body.Trim(), 512);
        }

        private static string Truncate(string value, int length)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= length) return value;
            return value.Substring(0, length) + "…";
        }
    }
}
