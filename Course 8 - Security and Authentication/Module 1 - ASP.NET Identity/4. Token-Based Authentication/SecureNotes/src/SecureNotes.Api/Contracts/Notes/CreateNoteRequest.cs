using System.ComponentModel.DataAnnotations;

namespace SecureNotes.Api.Contracts.Notes;

public record CreateNoteRequest(
    [Required, MaxLength(200)] string Title,
    [Required, MaxLength(10000)] string Content,
    bool IsPinned = false);
