using SecureNotes.Api.Contracts.Notes;

namespace SecureNotes.Api.Services;

public interface INoteService
{
    Task<List<NoteResponse>> GetAllAsync(string userId);
    Task<NoteResponse> GetByIdAsync(Guid id, string userId);
    Task<NoteResponse> CreateAsync(CreateNoteRequest request, string userId);
    Task<NoteResponse> UpdateAsync(Guid id, UpdateNoteRequest request, string userId);
    Task DeleteAsync(Guid id, string userId);
}
