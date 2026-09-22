using System.Collections.Generic;

namespace Laya.Unity.Transport
{
    public sealed class LayaTransportResponse
    {
        public long StatusCode { get; private set; }
        public string Body { get; private set; }
        public IReadOnlyDictionary<string, string> Headers { get; private set; }

        public LayaTransportResponse(long statusCode, string body, IReadOnlyDictionary<string, string> headers = null)
        {
            StatusCode = statusCode;
            Body = body;
            Headers = headers ?? new Dictionary<string, string>();
        }
    }
}
