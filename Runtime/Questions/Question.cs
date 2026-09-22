using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Laya.Unity.Questions
{
    public abstract class Question
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; private set; }

        [JsonProperty("instructions", NullValueHandling = NullValueHandling.Include)]
        public JToken Instructions { get; private set; }

        protected Question(string type, object instructions)
        {
            Type = type;
            Instructions = instructions == null ? JValue.CreateNull() : JToken.FromObject(instructions);
        }

        internal abstract void Validate(string id);
    }
}
