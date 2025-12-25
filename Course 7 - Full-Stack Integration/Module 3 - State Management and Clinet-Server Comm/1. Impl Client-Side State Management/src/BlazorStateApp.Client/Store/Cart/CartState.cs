using Fluxor;
using BlazorStateApp.Shared.Models;

namespace BlazorStateApp.Client.Store.Cart;

/// <summary>
/// Shopping cart state with computed properties.
/// </summary>
[FeatureState]
public record CartState
{
    public IReadOnlyList<CartItem> Items { get; init; } = Array.Empty<CartItem>();
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime? LastSyncedAt { get; init; }

    public int ItemCount => Items.Sum(x => x.Quantity);
    public int UniqueItemCount => Items.Count;
    public decimal TotalPrice => Items.Sum(x => x.Total);
    public bool IsEmpty => Items.Count == 0;
    public bool HasItems => Items.Count > 0;

    private CartState() { }
}
