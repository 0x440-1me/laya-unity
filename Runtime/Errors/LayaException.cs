using System;

namespace Laya.Unity.Errors
{
    public class LayaException : Exception
    {
        public LayaException(string message) : base(message) { }

        public LayaException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public sealed class LayaConfigurationException : LayaException
    {
        public LayaConfigurationException(string message) : base(message) { }
    }

    public sealed class LayaProtocolException : LayaException
    {
        public string Context { get; private set; }

        public LayaProtocolException(string message, string context = null, Exception innerException = null)
            : base(message, innerException)
        {
            Context = context;
        }
    }

    public class LayaConnectionException : LayaException
    {
        public LayaConnectionException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }

    public sealed class LayaTimeoutException : LayaConnectionException
    {
        public LayaTimeoutException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }

    public class LayaRequestException : LayaException
    {
        public long StatusCode { get; private set; }
        public string RequestContext { get; private set; }
        public string RequestId { get; private set; }

        public LayaRequestException(
            string message,
            long statusCode,
            string requestContext,
            string requestId = null,
            Exception innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            RequestContext = requestContext;
            RequestId = requestId;
        }
    }

    public sealed class LayaAuthenticationException : LayaRequestException
    {
        public LayaAuthenticationException(string message, long statusCode, string requestContext, string requestId = null)
            : base(message, statusCode, requestContext, requestId) { }
    }

    public sealed class LayaRateLimitException : LayaRequestException
    {
        public TimeSpan? RetryAfter { get; private set; }

        public LayaRateLimitException(
            string message,
            long statusCode,
            string requestContext,
            TimeSpan? retryAfter,
            string requestId = null)
            : base(message, statusCode, requestContext, requestId)
        {
            RetryAfter = retryAfter;
        }
    }
}
