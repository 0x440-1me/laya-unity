using System;
using System.Threading;
using Laya.Unity.Questions;
using UnityEngine;

namespace Laya.Unity.Samples
{
    /// <summary>
    /// General-purpose scene/camera direction sample. Requests only an allowed
    /// shot suggestion; the host Unity camera system owns transitions.
    /// </summary>
    public sealed class SceneDirectionExample : MonoBehaviour
    {
        [SerializeField] private string endpoint = "http://127.0.0.1:8000";
        [SerializeField] private bool cinematicMode = false;
        [SerializeField] private float sceneIntensity = 0.5f;
        [SerializeField] private bool subjectInFrame = true;

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

        [ContextMenu("Request Scene Direction")]
        public async void RequestSceneDirection()
        {
            if (_lifetime == null || _requestInFlight) return;
            _requestInFlight = true;
            var token = _lifetime.Token;
            try
            {
                var client = LayaClient.CreateLocal(endpoint);
                var result = await client.SystemOneAsync(
                    new { cinematicMode, sceneIntensity, subjectInFrame },
                    QuestionSet.Create(
                        ("shot", Choice.Create(
                            "Select an allowed camera framing style for this scene.",
                            ("wide", "Show the environment and subjects"),
                            ("medium", "Balance context and focal subject"),
                            ("close", "Emphasize the focal subject"))),
                        ("transitionIntensity", Score.Create(
                            "What level of visual transition is appropriate?",
                            "subtle", "moderate", "strong"))),
                    cancellationToken: token);

                var shot = result.GetChoice("shot");
                var intensity = result.GetScore("transitionIntensity");
                // Do not apply arbitrary camera commands from a model response.
                Debug.Log(
                    "Laya scene recommendation: shot=" + shot.Choice
                    + " (confidence=" + shot.Confidence
                    + "), transition score=" + intensity.Score,
                    this);
            }
            catch (OperationCanceledException)
            {
                // Normal when the component is disabled or its scene unloads.
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
