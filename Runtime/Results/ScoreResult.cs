using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Laya.Unity.Results
{
    public sealed class ScoreResult : Answer
    {
        public double Score { get; internal set; }
        public IReadOnlyDictionary<string, JToken> Legend { get; internal set; }
        public IReadOnlyDictionary<string, double> Probabilities { get; internal set; }
        public double Confidence { get; internal set; }
    }
}
