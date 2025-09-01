using EventEase.Models;

namespace EventEase.Services;

public interface IEventStore
{
    IReadOnlyList<Event> All { get; }
    Event? Get(int id);
    Event Add(Event e);
}
