using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Clients.Queries;

/// <summary>
/// Handler for retrieving a single client by ID.
/// </summary>
public class GetClientByIdHandler : IRequestHandler<GetClientByIdQuery, Result<ClientDto>>
{
    private readonly IApplicationDbContext _context;

    public GetClientByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the query to retrieve a client by ID.
    /// </summary>
    public async Task<Result<ClientDto>> Handle(
        GetClientByIdQuery request,
        CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .Where(c => c.Id == request.Id)
            .Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                ContactPerson = c.ContactPerson,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                ShippingAddress = c.ShippingAddress,
                IsActive = c.IsActive,
                OrderCount = c.Orders.Count,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (client is null)
        {
            return Result<ClientDto>.Failure("Client.NotFound", $"Client with ID {request.Id} not found.");
        }

        return Result<ClientDto>.Success(client);
    }
}
