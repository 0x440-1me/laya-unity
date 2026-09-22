using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Laya.Unity.Results
{
    public abstract class Answer
    {
        public string Type { get; internal set; }
        public IReadOnlyDictionary<string, JToken> ExtensionData { get; internal set; }
    }
}
