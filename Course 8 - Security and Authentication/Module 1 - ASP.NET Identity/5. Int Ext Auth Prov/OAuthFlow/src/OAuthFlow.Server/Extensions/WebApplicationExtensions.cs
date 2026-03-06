using OAuthFlow.Server.Endpoints;
using OAuthFlow.Server.Middleware;

namespace OAuthFlow.Server.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapAuthorizeEndpoints();
        app.MapTokenEndpoints();
        app.MapUserInfoEndpoints();

        return app;
    }
}
