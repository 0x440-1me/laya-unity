using System;
using System.Collections.Generic;
using Laya.Unity.Errors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Laya.Unity.Questions
{
    public sealed class ChoiceQuestion : Question
    {
        [JsonProperty("criteria", Required = Required.Always)]
        public IReadOnlyDictionary<string, JToken> Criteria { get; private set; }

        internal ChoiceQuestion(object instructions, IEnumerable<KeyValuePair<string, object>> criteria)
            : base("choice", instructions)
        {
            if (criteria == null) throw new ArgumentNullException(nameof(criteria));

            var values = new Dictionary<string, JToken>(StringComparer.Ordinal);
            foreach (var pair in criteria)
            {
                if (string.IsNullOrWhiteSpace(pair.Key))
                {
                    throw new LayaConfigurationException("Choice labels cannot be empty.");
                }

                if (values.ContainsKey(pair.Key))
                {
                    throw new LayaConfigurationException("Choice labels must be unique: " + pair.Key);
                }

                values.Add(pair.Key, pair.Value == null ? JValue.CreateNull() : JToken.FromObject(pair.Value));
            }

            Criteria = values;
        }

        internal override void Validate(string id)
        {
            if (Criteria.Count == 0)
            {
                throw new LayaConfigurationException("Choice question '" + id + "' must contain at least one option.");
            }
        }
    }

    public static class Choice
    {
        public static ChoiceQuestion Create(object instructions, params (string Label, string Description)[] criteria)
        {
            if (criteria == null) throw new ArgumentNullException(nameof(criteria));

            var mapped = new List<KeyValuePair<string, object>>(criteria.Length);
            foreach (var criterion in criteria)
            {
                mapped.Add(new KeyValuePair<string, object>(criterion.Label, criterion.Description));
            }
            return new ChoiceQuestion(instructions, mapped);
        }

        public static ChoiceQuestion Create(object instructions, IEnumerable<KeyValuePair<string, object>> criteria)
        {
            return new ChoiceQuestion(instructions, criteria);
        }
    }
}
