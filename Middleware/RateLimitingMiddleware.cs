namespace codebridgeTEST.Middleware
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, RateLimitInfo> _clients = new();
        private readonly int _maxRequestsPerSecond;
        private readonly TimeSpan _timeSpan = TimeSpan.FromSeconds(1);

        public RateLimitingMiddleware(RequestDelegate next, int maxRequestsPerSecond)
        {
            _next = next;
            _maxRequestsPerSecond = maxRequestsPerSecond;
        }

        public async Task Invoke(HttpContext context)
        {
            var clientId = context.Connection.RemoteIpAddress.ToString();

            var rateLimitInfo = _clients.GetOrAdd(clientId, _ => new RateLimitInfo(_timeSpan));

            if (!rateLimitInfo.AllowRequest(_maxRequestsPerSecond))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too Many Requests");
                return;
            }

            await _next(context);
        }

        private class RateLimitInfo
        {
            private readonly TimeSpan _timeSpan;
            private int _requestCount;
            private DateTime _lastRequestTime;

            public RateLimitInfo(TimeSpan timeSpan)
            {
                _timeSpan = timeSpan;
                _lastRequestTime = DateTime.UtcNow;
            }

            public bool AllowRequest(int max)
            {
                var currentTime = DateTime.UtcNow;

                if (currentTime - _lastRequestTime > _timeSpan)
                {
                    _requestCount = 0;
                    _lastRequestTime = currentTime;
                }

                _requestCount++;

                return _requestCount <= max;
            }
        }
    }

}
