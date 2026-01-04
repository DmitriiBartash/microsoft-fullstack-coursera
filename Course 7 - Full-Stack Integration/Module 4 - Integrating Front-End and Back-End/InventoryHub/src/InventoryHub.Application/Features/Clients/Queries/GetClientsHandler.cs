using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Clients.Queries;

/// <summary>
/// Handler for retrieving clients with optional filtering and order count.
/// </summary>
public class GetClientsHandler : IRequestHandler<GetClientsQuery, Result<List<ClientDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetClientsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the query to retrieve clients.
    /// </summary>
    public async Task<Result<List<ClientDto>>> Handle(
        GetClientsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Clients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(searchLower) ||
                (c.ContactPerson != null && c.ContactPerson.ToLower().Contains(searchLower)) ||
                (c.Email != null && c.Email.ToLower().Contains(searchLower)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        var clients = await query
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
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return Result<List<ClientDto>>.Success(clients);
    }
}
