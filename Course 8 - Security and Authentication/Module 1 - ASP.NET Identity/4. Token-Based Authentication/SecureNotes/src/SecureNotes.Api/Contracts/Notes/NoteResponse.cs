namespace SecureNotes.Api.Contracts.Notes;

public record NoteResponse(
    Guid Id,
    string Title,
    string Content,
    bool IsPinned,
    DateTime CreatedAt,
    DateTime UpdatedAt);
