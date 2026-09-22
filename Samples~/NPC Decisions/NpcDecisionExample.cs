using System;
using System.Threading;
using Laya.Unity.Questions;
using UnityEngine;

namespace Laya.Unity.Samples
{
    public sealed class NpcDecisionExample : MonoBehaviour
    {
        [SerializeField] private string endpoint = "http://127.0.0.1:8000";
        [SerializeField] private int health = 25;
        [SerializeField] private float enemyDistance = 4.2f;
        [SerializeField] private int ammo = 2;

        private CancellationTokenSource _lifetime;

        private void OnEnable()
        {
            _lifetime = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
            _lifetime = null;
        }

        [ContextMenu("Request Decision")]
        public async void RequestDecision()
        {
            try
            {
                var client = LayaClient.CreateLocal(endpoint);
                var result = await client.SystemOneAsync(
                    new { health, enemyDistance, ammo },
                    QuestionSet.Create(
                        ("action", Choice.Create(
                            "What should the NPC do?",
                            ("attack", "Engage the enemy"),
                            ("retreat", "Move toward safety"),
                            ("wait", "Hold position"))),
                        ("danger", Score.Create(
                            "How dangerous is the situation?",
                            "safe", "moderate danger", "extreme danger")),
                        ("flee", Noul.Create("Should the NPC flee?"))),
                    cancellationToken: _lifetime.Token);

                var action = result.GetChoice("action");
                Debug.Log("Laya chose " + action.Choice + " with confidence " + action.Confidence);
            }
            catch (OperationCanceledException)
            {
                // Expected when the component is disabled or the scene unloads.
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }
    }
}
