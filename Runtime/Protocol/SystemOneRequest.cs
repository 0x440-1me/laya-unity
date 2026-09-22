using Laya.Unity.Errors;
using Laya.Unity.Questions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Laya.Unity.Protocol
{
    public sealed class SystemOneRequest
    {
        [JsonProperty("state", Required = Required.Always)]
        public JToken State { get; private set; }

        [JsonProperty("questions", Required = Required.Always)]
        public QuestionSet Questions { get; private set; }

        [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; private set; }

        public SystemOneRequest(object state, QuestionSet questions, string model = null)
        {
            State = state == null ? JValue.CreateNull() : JToken.FromObject(state);
            Questions = questions;
            Model = model;
            Validate();
        }

        internal void Validate()
        {
            if (Questions == null)
            {
                throw new LayaConfigurationException("Questions are required.");
            }

            Questions.Validate();

            if (Model != null && string.IsNullOrWhiteSpace(Model))
            {
                throw new LayaConfigurationException("Model must be non-empty when supplied.");
            }
        }
    }
}
