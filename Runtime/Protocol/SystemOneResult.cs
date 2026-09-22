using System;
using System.Collections.Generic;
using Laya.Unity.Errors;
using Laya.Unity.Results;
using Newtonsoft.Json.Linq;

namespace Laya.Unity.Protocol
{
    public sealed class SystemOneResult
    {
        public string Model { get; internal set; }
        public Usage Usage { get; internal set; }
        public IReadOnlyDictionary<string, Answer> Answers { get; internal set; }
        public IReadOnlyDictionary<string, JToken> ExtensionData { get; internal set; }

        public ChoiceResult GetChoice(string id)
        {
            return Get<ChoiceResult>(id, "choice");
        }

        public ScoreResult GetScore(string id)
        {
            return Get<ScoreResult>(id, "score");
        }

        public NoulResult GetNoul(string id)
        {
            return Get<NoulResult>(id, "noul");
        }

        private T Get<T>(string id, string expectedType) where T : Answer
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Answer ID cannot be empty.", nameof(id));

            Answer answer;
            if (Answers == null || !Answers.TryGetValue(id, out answer))
            {
                throw new LayaProtocolException("The response does not contain answer '" + id + "'.", "answers." + id);
            }

            var typed = answer as T;
            if (typed == null)
            {
                throw new LayaProtocolException(
                    "Answer '" + id + "' is type '" + answer.Type + "', not '" + expectedType + "'.",
                    "answers." + id);
            }
            return typed;
        }
    }
}
