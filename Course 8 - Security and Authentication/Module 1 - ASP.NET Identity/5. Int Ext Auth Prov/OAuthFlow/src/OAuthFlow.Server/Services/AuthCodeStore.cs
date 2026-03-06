using System.Collections.Concurrent;
using OAuthFlow.Server.Domain.Entities;

namespace OAuthFlow.Server.Services;

public class AuthCodeStore : IAuthCodeStore
{
    private readonly ConcurrentDictionary<string, AuthorizationCode> _codes = new();

    public void Store(AuthorizationCode code)
    {
        _codes[code.Code] = code;
    }

    public AuthorizationCode? RetrieveAndDelete(string code)
    {
        if (!_codes.TryRemove(code, out var authCode))
            return null;

        return authCode.ExpiresAt > DateTime.UtcNow ? authCode : null;
    }
}
