using System.Collections.ObjectModel;
using FeedbackApp.Models;

namespace FeedbackApp.Services;

public class FeedbackService : IFeedbackService
{
    private readonly ObservableCollection<FeedbackItem> _items = new();

    public void Add(FeedbackItem item)
    {
        if (item is null) return;
        _items.Add(new FeedbackItem
        {
            Name = item.Name,
            Email = item.Email,
            Comment = item.Comment
        });
    }

    public IReadOnlyList<FeedbackItem> GetAll()
        => new ReadOnlyCollection<FeedbackItem>(_items);
}
