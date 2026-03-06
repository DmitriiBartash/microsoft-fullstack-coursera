using OAuthFlow.Server.Contracts;
using OAuthFlow.Server.Services;

namespace OAuthFlow.Server.Endpoints;

public static class TokenEndpoints
{
    public static RouteGroupBuilder MapTokenEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Token");

        group.MapPost("/token", async (HttpContext context, IClientStore clientStore, IAuthCodeStore authCodeStore, ITokenService tokenService, IUserStore userStore) =>
        {
            var form = await context.Request.ReadFormAsync();
            var grantType = form["grant_type"].ToString();
            var code = form["code"].ToString();
            var redirectUri = form["redirect_uri"].ToString();
            var clientId = form["client_id"].ToString();
            var clientSecret = form["client_secret"].ToString();

            if (grantType != "authorization_code")
                return Results.BadRequest(new { error = "unsupported_grant_type" });

            if (!clientStore.ValidateCredentials(clientId, clientSecret))
                return Results.BadRequest(new { error = "invalid_client" });

            var authCode = authCodeStore.RetrieveAndDelete(code);
            if (authCode is null)
                return Results.BadRequest(new { error = "invalid_grant" });

            if (authCode.ClientId != clientId || authCode.RedirectUri != redirectUri)
                return Results.BadRequest(new { error = "invalid_grant" });

            var user = userStore.FindById(authCode.UserId);
            if (user is null)
                return Results.BadRequest(new { error = "invalid_grant" });

            var jwtExpirationMinutes = context.RequestServices.GetRequiredService<IConfiguration>()
                .GetValue<int>("Jwt:ExpirationMinutes", 30);

            var accessToken = tokenService.GenerateAccessToken(user, authCode.Scope);

            return Results.Ok(new TokenResponse
            {
                AccessToken = accessToken,
                ExpiresIn = jwtExpirationMinutes * 60,
                Scope = authCode.Scope
            });
        }).AllowAnonymous().DisableAntiforgery();

        return group;
    }
}
