using EventEase.Models;

namespace EventEase.Services;

public sealed class EventStore : IEventStore
{
    private readonly List<Event> _events = new();
    private int _nextId;

    public EventStore()
    {
        _events.AddRange(new[]
        {
            new Event { Id = 1, Name = "Tech Summit",   Date = DateTime.Today.AddDays(7),  Location = "Chicago, IL", Description="Corporate tech conference." },
            new Event { Id = 2, Name = "Gala Night",    Date = DateTime.Today.AddDays(21), Location = "New York, NY", Description="Fundraising gala." },
            new Event { Id = 3, Name = "Sales Kickoff", Date = DateTime.Today.AddDays(35), Location = "Austin, TX",   Description="Q4 sales kickoff." },
        });
        _nextId = _events.Max(e => e.Id) + 1;
    }

    public IReadOnlyList<Event> All => _events;

    public Event? Get(int id) => _events.FirstOrDefault(e => e.Id == id);

    public Event Add(Event e)
    {
        e.Id = _nextId++;
        _events.Add(e);
        return e;
    }
}
