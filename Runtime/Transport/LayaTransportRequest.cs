using System;
using System.Collections.Generic;

namespace Laya.Unity.Transport
{
    public sealed class LayaTransportRequest
    {
        public Uri Endpoint { get; set; }
        public string Body { get; set; }
        public IReadOnlyDictionary<string, string> Headers { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}
