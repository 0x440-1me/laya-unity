using System.Collections.Generic;
using Laya.Unity.Errors;
using Laya.Unity.Protocol;
using Laya.Unity.Questions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Laya.Unity.Tests
{
    public sealed class ProtocolTests
    {
        [Test]
        public void SerializesChoiceRequestWithStructuredState()
        {
            var request = new SystemOneRequest(
                new { health = 25, inventory = new[] { "potion", "key" } },
                QuestionSet.Create(("action", Choice.Create(
                    "What should the NPC do?",
                    ("attack", "Engage"),
                    ("wait", (string)null)))),
                "laya");

            var json = JObject.Parse(ProtocolSerializer.Serialize(request));
            Assert.That((int)json["state"]["health"], Is.EqualTo(25));
            Assert.That((string)json["state"]["inventory"][1], Is.EqualTo("key"));
            Assert.That((string)json["questions"]["action"]["type"], Is.EqualTo("choice"));
            Assert.That(json["questions"]["action"]["criteria"]["wait"].Type, Is.EqualTo(JTokenType.Null));
            Assert.That((string)json["model"], Is.EqualTo("laya"));
        }

        [Test]
        public void SerializesScoreNoulMultipleQuestionsAndUnicode()
        {
            var request = new SystemOneRequest(
                "敵が近いです",
                QuestionSet.Create(
                    ("danger", Score.Create("危険度は？", "安全", "危険")),
                    ("flee", Noul.Create("逃げるべき？", "はい", "いいえ"))));

            var json = JObject.Parse(ProtocolSerializer.Serialize(request));
            Assert.That((string)json["state"], Is.EqualTo("敵が近いです"));
            Assert.That(json["questions"].Children<JProperty>(), Has.Exactly(2).Items);
            Assert.That((string)json["questions"]["danger"]["criteria"][1], Is.EqualTo("危険"));
            Assert.That((string)json["questions"]["flee"]["criteria"]["false"], Is.EqualTo("いいえ"));
            Assert.That(json["model"], Is.Null);
        }

        [Test]
        public void SupportsArrayAndNullState()
        {
            var questions = QuestionSet.Create(("ok", Noul.Create("Is it valid?")));
            var array = JObject.Parse(ProtocolSerializer.Serialize(new SystemOneRequest(new object[] { 1, "two", null }, questions)));
            var nil = JObject.Parse(ProtocolSerializer.Serialize(new SystemOneRequest(null, questions)));

            Assert.That(array["state"].Type, Is.EqualTo(JTokenType.Array));
            Assert.That(array["state"][2].Type, Is.EqualTo(JTokenType.Null));
            Assert.That(nil["state"].Type, Is.EqualTo(JTokenType.Null));
        }

        [Test]
        public void DeserializesChoiceAndPreservesExtensionsAndUsage()
        {
            var result = ProtocolSerializer.DeserializeResult(FixtureLoader.Read("choice-response.json"));
            var choice = result.GetChoice("action");

            Assert.That(choice.Choice, Is.EqualTo("retreat"));
            Assert.That(choice.Probabilities["retreat"], Is.EqualTo(0.8).Within(0.0001));
            Assert.That(choice.ExtensionData.ContainsKey("action"), Is.True);
            Assert.That(result.ExtensionData.ContainsKey("routing"), Is.True);
            Assert.That(result.Usage.InputTokens, Is.EqualTo(42));
            Assert.That(result.Usage.OutputTokens, Is.Zero);
        }

        [Test]
        public void DeserializesScore()
        {
            var result = ProtocolSerializer.DeserializeResult(FixtureLoader.Read("score-response.json"));
            var score = result.GetScore("danger");

            Assert.That(score.Score, Is.EqualTo(1.75));
            Assert.That((string)score.Legend["2"], Is.EqualTo("extreme danger"));
            Assert.That(score.Probabilities["2"], Is.EqualTo(0.8));
            Assert.That(result.Usage.OutputTokens, Is.EqualTo(3));
        }

        [Test]
        public void DeserializesNoulWithOptionalConfidence()
        {
            var result = ProtocolSerializer.DeserializeResult(FixtureLoader.Read("noul-response.json"));
            var noul = result.GetNoul("flee");

            Assert.That(noul.Noul, Is.EqualTo(0.92));
            Assert.That(noul.Confidence, Is.EqualTo(0.92));
            Assert.That(noul.ExtensionData.ContainsKey("vendor_extension"), Is.True);
        }

        [Test]
        public void AllowsNoulWithoutConfidence()
        {
            const string json = "{\"answers\":{\"q\":{\"type\":\"noul\",\"noul\":0.4}}}";
            Assert.That(ProtocolSerializer.DeserializeResult(json).GetNoul("q").Confidence, Is.Null);
        }

        [Test]
        public void RejectsMalformedAnswersWithContext()
        {
            var exception = Assert.Throws<LayaProtocolException>(() =>
                ProtocolSerializer.DeserializeResult(FixtureLoader.Read("malformed-response.json")));
            Assert.That(exception.Context, Is.EqualTo("answers.action.probabilities.wait"));
        }

        [Test]
        public void RejectsInvalidJson()
        {
            var exception = Assert.Throws<LayaProtocolException>(() => ProtocolSerializer.DeserializeResult("not json"));
            Assert.That(exception.Context, Is.EqualTo("response"));
        }

        [Test]
        public void ResultAccessorRejectsWrongType()
        {
            var result = ProtocolSerializer.DeserializeResult(FixtureLoader.Read("choice-response.json"));
            Assert.Throws<LayaProtocolException>(() => result.GetScore("action"));
        }

        [Test]
        public void ValidatesQuestionSemantics()
        {
            Assert.Throws<LayaConfigurationException>(() => new SystemOneRequest("state", new QuestionSet()));
            Assert.Throws<LayaConfigurationException>(() => QuestionSet.Create((" ", Noul.Create("x"))));
            Assert.Throws<LayaConfigurationException>(() =>
                new SystemOneRequest("state", QuestionSet.Create(("score", Score.Create("x", "only")))));
            Assert.Throws<LayaConfigurationException>(() =>
                new SystemOneRequest("state", QuestionSet.Create(("choice", Choice.Create("x")))));
            Assert.Throws<LayaConfigurationException>(() =>
                new SystemOneRequest("state", QuestionSet.Create(("q", Noul.Create("x"))), " "));
        }
    }
}
