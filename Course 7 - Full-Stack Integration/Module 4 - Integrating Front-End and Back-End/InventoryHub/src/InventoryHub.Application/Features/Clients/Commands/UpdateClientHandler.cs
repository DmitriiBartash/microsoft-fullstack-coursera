using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Clients.Commands;

/// <summary>
/// Handler for updating an existing client.
/// </summary>
public class UpdateClientHandler : IRequestHandler<UpdateClientCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public UpdateClientHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the command to update a client.
    /// </summary>
    public async Task<Result<Unit>> Handle(
        UpdateClientCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<Unit>.Failure("Client.InvalidName", "Client name is required.");
        }

        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (client is null)
        {
            return Result<Unit>.Failure("Client.NotFound", $"Client with ID {request.Id} not found.");
        }

        var nameExists = await _context.Clients
            .AnyAsync(c => c.Id != request.Id && c.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (nameExists)
        {
            return Result<Unit>.Failure("Client.DuplicateName", $"Client with name '{request.Name}' already exists.");
        }

        client.Update(
            request.Name,
            request.ContactPerson,
            request.Email,
            request.Phone,
            request.Address,
            request.ShippingAddress);

        if (request.IsActive && !client.IsActive)
        {
            client.Activate();
        }
        else if (!request.IsActive && client.IsActive)
        {
            client.Deactivate();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
