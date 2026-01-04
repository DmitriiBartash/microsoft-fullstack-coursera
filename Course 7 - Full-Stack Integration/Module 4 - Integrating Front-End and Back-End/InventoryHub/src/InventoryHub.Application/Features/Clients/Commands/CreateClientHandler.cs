using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Clients.Commands;

/// <summary>
/// Handler for creating a new client.
/// </summary>
public class CreateClientHandler : IRequestHandler<CreateClientCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateClientHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the command to create a new client.
    /// </summary>
    public async Task<Result<Guid>> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<Guid>.Failure("Client.InvalidName", "Client name is required.");
        }

        var nameExists = await _context.Clients
            .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (nameExists)
        {
            return Result<Guid>.Failure("Client.DuplicateName", $"Client with name '{request.Name}' already exists.");
        }

        var client = Client.Create(
            request.Name,
            request.ContactPerson,
            request.Email,
            request.Phone,
            request.Address,
            request.ShippingAddress);

        _context.Clients.Add(client);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(client.Id);
    }
}
