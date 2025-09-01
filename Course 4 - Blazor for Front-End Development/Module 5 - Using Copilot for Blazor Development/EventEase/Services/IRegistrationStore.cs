namespace EventEase.Services;

public interface IRegistrationStore
{
    IEnumerable<EventEase.Models.Registration> All { get; }
    EventEase.Models.Registration Add(EventEase.Models.Registration r);
    bool Exists(int eventId, string email);
    int CountFor(int eventId);
    IEnumerable<EventEase.Models.Registration> ForEvent(int eventId);
}
