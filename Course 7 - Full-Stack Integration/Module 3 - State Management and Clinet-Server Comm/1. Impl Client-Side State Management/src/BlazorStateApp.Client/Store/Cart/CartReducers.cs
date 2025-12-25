using Fluxor;
using BlazorStateApp.Shared.Models;

namespace BlazorStateApp.Client.Store.Cart;

/// <summary>
/// Pure functions for cart state updates.
/// </summary>
public static class CartReducers
{
    [ReducerMethod(typeof(LoadCartAction))]
    public static CartState ReduceLoadCart(CartState state) =>
        state with { IsLoading = true, ErrorMessage = null };

    [ReducerMethod]
    public static CartState ReduceLoadCartSuccess(CartState state, LoadCartSuccessAction action) =>
        state with { Items = action.Items, IsLoading = false, LastSyncedAt = DateTime.UtcNow };

    [ReducerMethod]
    public static CartState ReduceLoadCartFailure(CartState state, LoadCartFailureAction action) =>
        state with { IsLoading = false, ErrorMessage = action.Error };

    [ReducerMethod]
    public static CartState ReduceAddToCartSuccess(CartState state, AddToCartSuccessAction action)
    {
        var items = new List<CartItem>(state.Items.Count + 1);
        items.AddRange(state.Items);
        items.Add(action.Item);
        return state with { Items = items, LastSyncedAt = DateTime.UtcNow };
    }

    [ReducerMethod]
    public static CartState ReduceAddToCartFailure(CartState state, AddToCartFailureAction action) =>
        state with { ErrorMessage = action.Error };

    [ReducerMethod]
    public static CartState ReduceRemoveFromCartSuccess(CartState state, RemoveFromCartSuccessAction action)
    {
        var items = new List<CartItem>(state.Items.Count);
        foreach (var item in state.Items)
        {
            if (item.Id != action.ItemId)
                items.Add(item);
        }
        return state with { Items = items, LastSyncedAt = DateTime.UtcNow };
    }

    [ReducerMethod]
    public static CartState ReduceUpdateQuantitySuccess(CartState state, UpdateQuantitySuccessAction action)
    {
        if (action.Quantity <= 0)
        {
            var filtered = new List<CartItem>(state.Items.Count);
            foreach (var item in state.Items)
            {
                if (item.Id != action.ItemId)
                    filtered.Add(item);
            }
            return state with { Items = filtered, LastSyncedAt = DateTime.UtcNow };
        }

        var items = new List<CartItem>(state.Items.Count);
        foreach (var item in state.Items)
        {
            items.Add(item.Id == action.ItemId ? item with { Quantity = action.Quantity } : item);
        }
        return state with { Items = items, LastSyncedAt = DateTime.UtcNow };
    }

    [ReducerMethod(typeof(ClearCartSuccessAction))]
    public static CartState ReduceClearCartSuccess(CartState state) =>
        state with { Items = Array.Empty<CartItem>(), LastSyncedAt = DateTime.UtcNow };
}
