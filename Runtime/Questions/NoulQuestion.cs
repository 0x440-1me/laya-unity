using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Laya.Unity.Questions
{
    public sealed class NoulQuestion : Question
    {
        [JsonProperty("criteria", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyDictionary<string, JToken> Criteria { get; private set; }

        internal NoulQuestion(object instructions, object trueDescription, object falseDescription)
            : base("noul", instructions)
        {
            if (trueDescription == null && falseDescription == null) return;

            var criteria = new Dictionary<string, JToken>();
            if (trueDescription != null) criteria["true"] = JToken.FromObject(trueDescription);
            if (falseDescription != null) criteria["false"] = JToken.FromObject(falseDescription);
            Criteria = criteria;
        }

        internal override void Validate(string id) { }
    }

    public static class Noul
    {
        public static NoulQuestion Create(
            object instructions,
            object trueDescription = null,
            object falseDescription = null)
        {
            return new NoulQuestion(instructions, trueDescription, falseDescription);
        }
    }
}
