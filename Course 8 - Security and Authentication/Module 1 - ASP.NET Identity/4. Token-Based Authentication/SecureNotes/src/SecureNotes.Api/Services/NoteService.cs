using Microsoft.EntityFrameworkCore;
using SecureNotes.Api.Contracts.Notes;
using SecureNotes.Api.Domain.Entities;
using SecureNotes.Api.Infrastructure.Data;

namespace SecureNotes.Api.Services;

public class NoteService(ApplicationDbContext dbContext) : INoteService
{
    public async Task<List<NoteResponse>> GetAllAsync(string userId)
    {
        return await dbContext.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.UpdatedAt)
            .Select(n => ToResponse(n))
            .ToListAsync();
    }

    public async Task<NoteResponse> GetByIdAsync(Guid id, string userId)
    {
        var note = await dbContext.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId)
            ?? throw new KeyNotFoundException("Note not found.");

        return ToResponse(note);
    }

    public async Task<NoteResponse> CreateAsync(CreateNoteRequest request, string userId)
    {
        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            IsPinned = request.IsPinned,
            UserId = userId
        };

        dbContext.Notes.Add(note);
        await dbContext.SaveChangesAsync();

        return ToResponse(note);
    }

    public async Task<NoteResponse> UpdateAsync(Guid id, UpdateNoteRequest request, string userId)
    {
        var note = await dbContext.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId)
            ?? throw new KeyNotFoundException("Note not found.");

        note.Title = request.Title;
        note.Content = request.Content;
        note.IsPinned = request.IsPinned;
        note.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return ToResponse(note);
    }

    public async Task DeleteAsync(Guid id, string userId)
    {
        var note = await dbContext.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId)
            ?? throw new KeyNotFoundException("Note not found.");

        dbContext.Notes.Remove(note);
        await dbContext.SaveChangesAsync();
    }

    private static NoteResponse ToResponse(Note note) => new(
        note.Id,
        note.Title,
        note.Content,
        note.IsPinned,
        note.CreatedAt,
        note.UpdatedAt);
}
