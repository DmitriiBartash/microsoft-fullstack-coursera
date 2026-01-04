using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Clients.Queries;

/// <summary>
/// Query to retrieve a list of clients with optional search filter.
/// </summary>
public record GetClientsQuery : IRequest<Result<List<ClientDto>>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
}
