using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Laya.Unity.Errors;
using Laya.Unity.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Laya.Unity.Protocol
{
    internal static class ProtocolSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            Formatting = Formatting.None,
            Culture = CultureInfo.InvariantCulture
        };

        internal static string Serialize(SystemOneRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            request.Validate();
            return JsonConvert.SerializeObject(request, Settings);
        }

        internal static SystemOneResult DeserializeResult(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new LayaProtocolException("The server returned an empty response.", "response");
            }

            JObject root;
            try
            {
                root = JObject.Parse(json);
            }
            catch (JsonException exception)
            {
                throw new LayaProtocolException("The server returned invalid JSON.", "response", exception);
            }

            var answersToken = root["answers"];
            if (answersToken == null || answersToken.Type != JTokenType.Object)
            {
                throw new LayaProtocolException("The response must contain an answers object.", "answers");
            }

            var answers = new Dictionary<string, Answer>(StringComparer.Ordinal);
            foreach (var property in ((JObject)answersToken).Properties())
            {
                answers.Add(property.Name, ParseAnswer(property.Name, property.Value));
            }

            var usage = ParseUsage(root["usage"]);
            return new SystemOneResult
            {
                Model = OptionalString(root["model"]),
                Usage = usage,
                Answers = answers,
                ExtensionData = Extensions(root, "model", "answers", "usage")
            };
        }

        private static Answer ParseAnswer(string id, JToken token)
        {
            var context = "answers." + id;
            var obj = token as JObject;
            if (obj == null) throw new LayaProtocolException("Answer '" + id + "' must be an object.", context);

            var type = RequiredString(obj["type"], context + ".type");
            switch (type)
            {
                case "choice":
                    return new ChoiceResult
                    {
                        Type = type,
                        Choice = RequiredString(obj["choice"], context + ".choice"),
                        Probabilities = NumberMap(obj["probabilities"], context + ".probabilities"),
                        Confidence = RequiredNumber(obj["confidence"], context + ".confidence"),
                        ExtensionData = Extensions(obj, "type", "choice", "probabilities", "confidence")
                    };
                case "score":
                    return new ScoreResult
                    {
                        Type = type,
                        Score = RequiredNumber(obj["score"], context + ".score"),
                        Legend = TokenMap(obj["legend"], context + ".legend"),
                        Probabilities = NumberMap(obj["probabilities"], context + ".probabilities"),
                        Confidence = RequiredNumber(obj["confidence"], context + ".confidence"),
                        ExtensionData = Extensions(obj, "type", "score", "legend", "probabilities", "confidence")
                    };
                case "noul":
                    return new NoulResult
                    {
                        Type = type,
                        Noul = RequiredNumber(obj["noul"], context + ".noul"),
                        Confidence = OptionalNumber(obj["confidence"], context + ".confidence"),
                        ExtensionData = Extensions(obj, "type", "noul", "confidence")
                    };
                default:
                    throw new LayaProtocolException("Answer '" + id + "' has unknown type '" + type + "'.", context + ".type");
            }
        }

        private static Usage ParseUsage(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null) return null;
            var obj = token as JObject;
            if (obj == null) throw new LayaProtocolException("usage must be an object when present.", "usage");

            return new Usage
            {
                InputTokens = OptionalInteger(obj["input_tokens"], "usage.input_tokens"),
                OutputTokens = OptionalInteger(obj["output_tokens"], "usage.output_tokens")
            };
        }

        private static string RequiredString(JToken token, string context)
        {
            if (token == null || token.Type != JTokenType.String || string.IsNullOrWhiteSpace(token.Value<string>()))
            {
                throw new LayaProtocolException(context + " must be a non-empty string.", context);
            }
            return token.Value<string>();
        }

        private static string OptionalString(JToken token)
        {
            return token != null && token.Type == JTokenType.String ? token.Value<string>() : null;
        }

        private static double RequiredNumber(JToken token, string context)
        {
            if (token == null || (token.Type != JTokenType.Float && token.Type != JTokenType.Integer))
            {
                throw new LayaProtocolException(context + " must be a number.", context);
            }
            var value = token.Value<double>();
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new LayaProtocolException(context + " must be finite.", context);
            }
            return value;
        }

        private static double? OptionalNumber(JToken token, string context)
        {
            if (token == null || token.Type == JTokenType.Null) return null;
            return RequiredNumber(token, context);
        }

        private static long OptionalInteger(JToken token, string context)
        {
            if (token == null || token.Type == JTokenType.Null) return 0;
            if (token.Type != JTokenType.Integer)
            {
                throw new LayaProtocolException(context + " must be an integer.", context);
            }
            return token.Value<long>();
        }

        private static IReadOnlyDictionary<string, double> NumberMap(JToken token, string context)
        {
            var obj = token as JObject;
            if (obj == null) throw new LayaProtocolException(context + " must be an object.", context);

            var values = new Dictionary<string, double>(StringComparer.Ordinal);
            foreach (var property in obj.Properties())
            {
                values.Add(property.Name, RequiredNumber(property.Value, context + "." + property.Name));
            }
            return values;
        }

        private static IReadOnlyDictionary<string, JToken> TokenMap(JToken token, string context)
        {
            var obj = token as JObject;
            if (obj == null) throw new LayaProtocolException(context + " must be an object.", context);
            return obj.Properties().ToDictionary(
                property => property.Name,
                property => property.Value.DeepClone(),
                StringComparer.Ordinal);
        }

        private static IReadOnlyDictionary<string, JToken> Extensions(JObject obj, params string[] known)
        {
            var knownNames = new HashSet<string>(known, StringComparer.Ordinal);
            return obj.Properties()
                .Where(property => !knownNames.Contains(property.Name))
                .ToDictionary(
                    property => property.Name,
                    property => property.Value.DeepClone(),
                    StringComparer.Ordinal);
        }
    }
}
