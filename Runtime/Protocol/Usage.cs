using Newtonsoft.Json;

namespace Laya.Unity.Protocol
{
    public sealed class Usage
    {
        [JsonProperty("input_tokens")]
        public long InputTokens { get; internal set; }

        [JsonProperty("output_tokens")]
        public long OutputTokens { get; internal set; }
    }
}
