using System;
using System.Collections.Generic;
using Laya.Unity.Errors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Laya.Unity.Questions
{
    public sealed class ScoreQuestion : Question
    {
        [JsonProperty("criteria", Required = Required.Always)]
        public IReadOnlyList<JToken> Criteria { get; private set; }

        internal ScoreQuestion(object instructions, IEnumerable<object> criteria)
            : base("score", instructions)
        {
            if (criteria == null) throw new ArgumentNullException(nameof(criteria));

            var values = new List<JToken>();
            foreach (var value in criteria)
            {
                values.Add(value == null ? JValue.CreateNull() : JToken.FromObject(value));
            }
            Criteria = values;
        }

        internal override void Validate(string id)
        {
            if (Criteria.Count < 2)
            {
                throw new LayaConfigurationException(
                    "Score question '" + id + "' must contain at least two ordered levels.");
            }
        }
    }

    public static class Score
    {
        public static ScoreQuestion Create(object instructions, params object[] orderedCriteria)
        {
            return new ScoreQuestion(instructions, orderedCriteria);
        }
    }
}
