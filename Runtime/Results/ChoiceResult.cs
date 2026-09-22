using System.Collections.Generic;

namespace Laya.Unity.Results
{
    public sealed class ChoiceResult : Answer
    {
        public string Choice { get; internal set; }
        public IReadOnlyDictionary<string, double> Probabilities { get; internal set; }
        public double Confidence { get; internal set; }
    }
}
