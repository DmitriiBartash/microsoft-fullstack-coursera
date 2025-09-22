using System.Diagnostics;

namespace UserManagementApp.Middleware
{
    public class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? string.Empty;

            await _next(context);

            sw.Stop();
            var status = context.Response?.StatusCode;
            _logger.LogInformation("{Method} {Path} -> {Status} ({Elapsed} ms)", method, path, status, sw.ElapsedMilliseconds);
        }
    }
}
