using System;
using System.Threading;
using Laya.Unity.Questions;
using UnityEngine;

namespace Laya.Unity.Samples
{
    /// <summary>
    /// Example of using Laya from a UI subsystem. This logs recommendations;
    /// it does not change the UI or execute any model-selected action.
    /// </summary>
    public sealed class UiPresentationExample : MonoBehaviour
    {
        [SerializeField] private string endpoint = "http://127.0.0.1:8000";
        [SerializeField] private int pendingNotifications = 3;
        [SerializeField] private bool focusMode = true;

        private CancellationTokenSource _lifetime;
        private bool _requestInFlight;

        private void OnEnable()
        {
            _lifetime = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            var lifetime = _lifetime;
            _lifetime = null;
            if (lifetime == null) return;
            lifetime.Cancel();
            lifetime.Dispose();
        }

        [ContextMenu("Request UI Decision")]
        public async void RequestUiDecision()
        {
            if (_lifetime == null || _requestInFlight) return;
            _requestInFlight = true;
            var token = _lifetime.Token;
            try
            {
                var client = LayaClient.CreateLocal(endpoint);
                var result = await client.SystemOneAsync(
                    new
                    {
                        viewportWidth = Screen.width,
                        viewportHeight = Screen.height,
                        pendingNotifications,
                        focusMode
                    },
                    QuestionSet.Create(
                        ("layout", Choice.Create(
                            "Which permitted UI presentation mode best suits this state?",
                            ("compact", "Show essential information only"),
                            ("expanded", "Show detailed panels and controls"))),
                        ("urgency", Score.Create(
                            "How urgent is this information?",
                            "low", "medium", "high")),
                        ("showHint", Noul.Create(
                            "Should the interface consider a contextual hint?"))),
                    cancellationToken: token);

                var layout = result.GetChoice("layout");
                var urgency = result.GetScore("urgency");
                var showHint = result.GetNoul("showHint");

                // The application must decide whether and how to apply these results.
                Debug.Log(
                    "Laya UI recommendation: layout=" + layout.Choice
                    + " (confidence=" + layout.Confidence
                    + "), urgency score=" + urgency.Score
                    + ", hint probability=" + showHint.Noul,
                    this);
            }
            catch (OperationCanceledException)
            {
                // Expected if the component is disabled or its scene unloads.
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
            finally
            {
                _requestInFlight = false;
            }
        }
    }
}
