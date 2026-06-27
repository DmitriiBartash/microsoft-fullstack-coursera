using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace JwtSecurityCore.Services;

/// <summary>A refresh token record: an opaque high-entropy string bound to a user, with an expiry and a revoked flag.</summary>
public sealed record RefreshToken(string Token, string Username, DateTime ExpiresUtc, bool Revoked);

/// <summary>
/// In-memory store of refresh tokens (a real system would persist these). Implements
/// <b>rotation with reuse detection</b>: every successful refresh atomically consumes (revokes) the
/// presented token and issues a brand-new one, so a refresh token is single-use. If an already-revoked
/// token is presented again — the signature of a stolen, replayed token — the whole token family for
/// that user is revoked, forcing a fresh login. Expired entries are pruned so the store stays bounded.
/// </summary>
public class RefreshTokenStore
{
    private readonly ConcurrentDictionary<string, RefreshToken> _store = new();
    private readonly JwtSettings _settings;

    public RefreshTokenStore(JwtSettings settings) => _settings = settings;

    /// <summary>Mints and stores a new refresh token for the user, pruning any expired entries first.</summary>
    public RefreshToken Issue(string username)
    {
        PruneExpired();
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');
        var entry = new RefreshToken(token, username, DateTime.UtcNow.AddDays(_settings.RefreshTokenDays), Revoked: false);
        _store[token] = entry;
        return entry;
    }

    /// <summary>Returns the live token entry, or <c>null</c> when it is unknown, revoked or expired.</summary>
    public RefreshToken? Validate(string? token)
    {
        if (token is null || !_store.TryGetValue(token, out var entry)) return null;
        if (entry.Revoked || entry.ExpiresUtc <= DateTime.UtcNow) return null;
        return entry;
    }

    /// <summary>
    /// Rotates a valid token: atomically revokes it and returns a fresh replacement. The revoke uses a
    /// compare-and-swap, so two concurrent rotations of the same token cannot both succeed — the loser is
    /// treated as reuse. Re-presenting an already-revoked token also trips reuse detection and revokes the
    /// whole family. Returns <c>null</c> when the token cannot be rotated.
    /// </summary>
    public RefreshToken? Rotate(string? token)
    {
        if (token is null || !_store.TryGetValue(token, out var entry)) return null;

        if (entry.Revoked)
        {
            RevokeAllFor(entry.Username);
            return null;
        }
        if (entry.ExpiresUtc <= DateTime.UtcNow) return null;

        if (!_store.TryUpdate(token, entry with { Revoked = true }, entry))
        {
            RevokeAllFor(entry.Username);
            return null;
        }
        return Issue(entry.Username);
    }

    /// <summary>Revokes a single token (e.g. on logout).</summary>
    public void Revoke(string? token)
    {
        if (token is not null && _store.TryGetValue(token, out var entry))
            _store.TryUpdate(token, entry with { Revoked = true }, entry);
    }

    private void RevokeAllFor(string username)
    {
        foreach (var kv in _store)
            if (kv.Value.Username == username && !kv.Value.Revoked)
                _store.TryUpdate(kv.Key, kv.Value with { Revoked = true }, kv.Value);
    }

    private void PruneExpired()
    {
        var now = DateTime.UtcNow;
        foreach (var kv in _store)
            if (kv.Value.ExpiresUtc <= now)
                _store.TryRemove(kv.Key, out _);
    }
}
