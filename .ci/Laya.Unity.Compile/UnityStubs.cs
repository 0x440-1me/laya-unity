using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public class AsyncOperation { }
}

namespace UnityEngine.Networking
{
    public class UploadHandler : IDisposable
    {
        public void Dispose() { }
    }

    public sealed class UploadHandlerRaw : UploadHandler
    {
        public UploadHandlerRaw(byte[] bytes) { }
    }

    public class DownloadHandler : IDisposable
    {
        public string text { get; set; }
        public void Dispose() { }
    }

    public sealed class DownloadHandlerBuffer : DownloadHandler { }

    public sealed class UnityWebRequestAsyncOperation : UnityEngine.AsyncOperation
    {
        public event Action<UnityEngine.AsyncOperation> completed;
        public void Complete() { completed?.Invoke(this); }
    }

    public sealed class UnityWebRequest : IDisposable
    {
        public const string kHttpVerbPOST = "POST";

        public enum Result
        {
            InProgress,
            Success,
            ConnectionError,
            ProtocolError,
            DataProcessingError
        }

        public UploadHandler uploadHandler { get; set; }
        public DownloadHandler downloadHandler { get; set; }
        public int timeout { get; set; }
        public long responseCode { get; set; }
        public string error { get; set; }
        public Result result { get; set; }

        public UnityWebRequest(Uri uri, string method) { }
        public void SetRequestHeader(string name, string value) { }
        public UnityWebRequestAsyncOperation SendWebRequest() => new UnityWebRequestAsyncOperation();
        public Dictionary<string, string> GetResponseHeaders() => new Dictionary<string, string>();
        public void Abort() { }
        public void Dispose() { }
    }
}
