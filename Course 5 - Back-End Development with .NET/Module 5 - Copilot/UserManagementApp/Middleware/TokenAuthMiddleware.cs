using Microsoft.AspNetCore.Mvc;

namespace UserManagementApp.Middleware
{
    public class TokenAuthMiddleware(RequestDelegate next, IConfiguration config, ILogger<TokenAuthMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly IConfiguration _config = config;
        private readonly ILogger<TokenAuthMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;
            var isApi = path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
            var isSwagger = path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase);
            var isStatic = path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) ||
                           path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) ||
                           path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase);

            if (isApi && !isSwagger && !isStatic)
            {
                var expected = _config["Auth:Token"];
                var auth = context.Request.Headers["Authorization"].ToString();
                var valid = !string.IsNullOrEmpty(expected) &&
                            auth.StartsWith("Bearer ", StringComparison.Ordinal) &&
                            string.Equals(auth.Substring("Bearer ".Length), expected, StringComparison.Ordinal);

                if (!valid)
                {
                    var problem = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/401",
                        Title = "Unauthorized",
                        Status = StatusCodes.Status401Unauthorized,
                        Instance = context.Request.Path
                    };
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json; charset=utf-8";
                    await context.Response.WriteAsJsonAsync(problem);
                    _logger.LogWarning("Unauthorized request to {Path}", path);
                    return;
                }
            }

            await _next(context);
        }
    }
}
