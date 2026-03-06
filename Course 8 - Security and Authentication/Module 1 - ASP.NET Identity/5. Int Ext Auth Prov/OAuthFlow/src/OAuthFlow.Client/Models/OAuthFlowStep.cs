namespace OAuthFlow.Client.Models;

public class OAuthFlowStep
{
    public required int StepNumber { get; init; }
    public required string Title { get; init; }
    public required string Status { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? RequestInfo { get; set; }
    public string? ResponseInfo { get; set; }
}
