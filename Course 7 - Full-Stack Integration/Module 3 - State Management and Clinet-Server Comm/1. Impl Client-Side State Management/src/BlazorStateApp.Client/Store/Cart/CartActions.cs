using BlazorStateApp.Shared.Models;

namespace BlazorStateApp.Client.Store.Cart;

public record LoadCartAction;
public record LoadCartSuccessAction(IReadOnlyList<CartItem> Items);
public record LoadCartFailureAction(string Error);

public record AddToCartAction(string Name, decimal Price, string? ImageUrl = null, string? Category = null);
public record AddToCartSuccessAction(CartItem Item);
public record AddToCartFailureAction(string Error);

public record RemoveFromCartAction(int ItemId);
public record RemoveFromCartSuccessAction(int ItemId);

public record UpdateQuantityAction(int ItemId, int Quantity);
public record UpdateQuantitySuccessAction(int ItemId, int Quantity);

public record ClearCartAction;
public record ClearCartSuccessAction;
