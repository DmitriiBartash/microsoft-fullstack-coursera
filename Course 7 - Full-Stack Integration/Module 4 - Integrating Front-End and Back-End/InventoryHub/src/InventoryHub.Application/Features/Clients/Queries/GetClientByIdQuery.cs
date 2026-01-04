using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Clients.Queries;

/// <summary>
/// Query to retrieve a single client by ID.
/// </summary>
public record GetClientByIdQuery(Guid Id) : IRequest<Result<ClientDto>>;
