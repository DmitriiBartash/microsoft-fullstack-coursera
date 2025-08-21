using FeedbackApp.Models;

namespace FeedbackApp.Services;

public interface IFeedbackService
{
    void Add(FeedbackItem item);
    IReadOnlyList<FeedbackItem> GetAll();
}
