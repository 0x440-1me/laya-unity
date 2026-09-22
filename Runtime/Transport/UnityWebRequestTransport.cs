using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laya.Unity.Errors;
using UnityEngine.Networking;

namespace Laya.Unity.Transport
{
    public sealed class UnityWebRequestTransport : ILayaTransport
    {
        public async Task<LayaTransportResponse> SendAsync(
            LayaTransportRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            cancellationToken.ThrowIfCancellationRequested();

            using (var webRequest = new UnityWebRequest(request.Endpoint, UnityWebRequest.kHttpVerbPOST))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(request.Body ?? string.Empty));
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Accept", "application/json");
                webRequest.timeout = Math.Max(1, (int)Math.Ceiling(request.Timeout.TotalSeconds));

                if (request.Headers != null)
                {
                    foreach (var header in request.Headers)
                    {
                        webRequest.SetRequestHeader(header.Key, header.Value);
                    }
                }

                var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                var operation = webRequest.SendWebRequest();
                operation.completed += _ => completion.TrySetResult(true);

                using (cancellationToken.Register(() =>
                {
                    webRequest.Abort();
                    completion.TrySetCanceled();
                }))
                {
                    try
                    {
                        await completion.Task;
                    }
                    catch (TaskCanceledException)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        throw;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    if (IsTimeout(webRequest.error))
                    {
                        throw new LayaTimeoutException("The System One request timed out.");
                    }
                    throw new LayaConnectionException("The System One endpoint could not be reached: " + SafeError(webRequest.error));
                }

                return new LayaTransportResponse(
                    webRequest.responseCode,
                    webRequest.downloadHandler != null ? webRequest.downloadHandler.text : string.Empty,
                    CopyHeaders(webRequest.GetResponseHeaders()));
            }
        }

        private static bool IsTimeout(string error)
        {
            return !string.IsNullOrEmpty(error) &&
                   (error.IndexOf("timed out", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    error.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static string SafeError(string error)
        {
            return string.IsNullOrWhiteSpace(error) ? "connection error" : error;
        }

        private static IReadOnlyDictionary<string, string> CopyHeaders(Dictionary<string, string> headers)
        {
            return headers == null
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(headers, StringComparer.OrdinalIgnoreCase);
        }
    }
}
