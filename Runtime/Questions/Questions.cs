using System;
using System.Collections;
using System.Collections.Generic;
using Laya.Unity.Errors;

namespace Laya.Unity.Questions
{
    public sealed class QuestionSet : IReadOnlyDictionary<string, Question>
    {
        private readonly Dictionary<string, Question> _values;

        public Question this[string key] => _values[key];
        public IEnumerable<string> Keys => _values.Keys;
        public IEnumerable<Question> Values => _values.Values;
        public int Count => _values.Count;

        public QuestionSet()
        {
            _values = new Dictionary<string, Question>(StringComparer.Ordinal);
        }

        public QuestionSet Add(string id, Question question)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new LayaConfigurationException("Question IDs cannot be empty.");
            }
            if (question == null) throw new ArgumentNullException(nameof(question));
            if (_values.ContainsKey(id))
            {
                throw new LayaConfigurationException("Question IDs must be unique: " + id);
            }

            _values.Add(id, question);
            return this;
        }

        public static QuestionSet Create(params (string Id, Question Question)[] entries)
        {
            if (entries == null) throw new ArgumentNullException(nameof(entries));

            var questions = new QuestionSet();
            foreach (var entry in entries) questions.Add(entry.Id, entry.Question);
            return questions;
        }

        internal void Validate()
        {
            if (_values.Count == 0)
            {
                throw new LayaConfigurationException("At least one question is required.");
            }

            foreach (var pair in _values) pair.Value.Validate(pair.Key);
        }

        public bool ContainsKey(string key) => _values.ContainsKey(key);
        public bool TryGetValue(string key, out Question value) => _values.TryGetValue(key, out value);
        public IEnumerator<KeyValuePair<string, Question>> GetEnumerator() => _values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
