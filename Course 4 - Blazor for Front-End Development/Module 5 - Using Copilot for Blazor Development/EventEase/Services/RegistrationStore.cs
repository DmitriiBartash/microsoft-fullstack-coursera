using System.Collections.Concurrent;
using EventEase.Models;

namespace EventEase.Services;

public sealed class RegistrationStore : IRegistrationStore
{
    private readonly ConcurrentDictionary<(int,string), Registration> _set = new();
    private readonly ConcurrentDictionary<int, List<Registration>> _byEvent = new();

    public IEnumerable<Registration> All => _set.Values;

    public Registration Add(Registration r)
    {
        var key = (r.EventId, (r.Email ?? "").Trim().ToLowerInvariant());
        _set.TryAdd(key, r);
        _byEvent.AddOrUpdate(r.EventId,
            _ => new List<Registration> { r },
            (_, list) => { lock (list) list.Add(r); return list; });
        return r;
    }

    public bool Exists(int eventId, string email)
        => _set.ContainsKey((eventId, (email ?? "").Trim().ToLowerInvariant()));

    public int CountFor(int eventId)
        => _byEvent.TryGetValue(eventId, out var list) ? list.Count : 0;

    public IEnumerable<Registration> ForEvent(int eventId)
        => _byEvent.TryGetValue(eventId, out var list) ? list : Enumerable.Empty<Registration>();
}
