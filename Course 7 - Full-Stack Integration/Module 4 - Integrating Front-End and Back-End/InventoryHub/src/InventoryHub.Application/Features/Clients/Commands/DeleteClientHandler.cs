using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Clients.Commands;

/// <summary>
/// Handler for deleting a client.
/// </summary>
public class DeleteClientHandler : IRequestHandler<DeleteClientCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public DeleteClientHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the command to delete a client.
    /// </summary>
    public async Task<Result<Unit>> Handle(
        DeleteClientCommand request,
        CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (client is null)
        {
            return Result<Unit>.Failure("Client.NotFound", $"Client with ID {request.Id} not found.");
        }

        var hasOrders = await _context.SalesOrders
            .AnyAsync(o => o.ClientId == request.Id, cancellationToken);

        if (hasOrders)
        {
            return Result<Unit>.Failure("Client.HasOrders", "Cannot delete client with existing orders.");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
