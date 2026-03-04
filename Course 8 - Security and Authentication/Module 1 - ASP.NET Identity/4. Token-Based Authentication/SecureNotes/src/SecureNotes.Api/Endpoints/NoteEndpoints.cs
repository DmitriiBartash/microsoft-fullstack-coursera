using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SecureNotes.Api.Contracts.Notes;
using SecureNotes.Api.Services;

namespace SecureNotes.Api.Endpoints;

public static class NoteEndpoints
{
    public static RouteGroupBuilder MapNoteEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/notes")
            .WithTags("Notes")
            .RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal user, INoteService noteService) =>
        {
            var userId = GetUserId(user);
            var notes = await noteService.GetAllAsync(userId);
            return Results.Ok(notes);
        });

        group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal user, INoteService noteService) =>
        {
            var userId = GetUserId(user);
            var note = await noteService.GetByIdAsync(id, userId);
            return Results.Ok(note);
        });

        group.MapPost("/", async (CreateNoteRequest request, ClaimsPrincipal user, INoteService noteService) =>
        {
            var userId = GetUserId(user);
            var note = await noteService.CreateAsync(request, userId);
            return Results.Created($"/api/notes/{note.Id}", note);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateNoteRequest request, ClaimsPrincipal user, INoteService noteService) =>
        {
            var userId = GetUserId(user);
            var note = await noteService.UpdateAsync(id, request, userId);
            return Results.Ok(note);
        });

        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, INoteService noteService) =>
        {
            var userId = GetUserId(user);
            await noteService.DeleteAsync(id, userId);
            return Results.NoContent();
        });

        return group;
    }

    private static string GetUserId(ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? throw new UnauthorizedAccessException("User ID not found in token.");
}
