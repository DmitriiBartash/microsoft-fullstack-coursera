using Fluxor;
using BlazorStateApp.Client.Services.Storage;
using BlazorStateApp.Shared.Models;

namespace BlazorStateApp.Client.Store.Cart;

/// <summary>
/// Side effects for cart operations (IndexedDB persistence).
/// </summary>
public class CartEffects
{
    private readonly IStorageService _storage;
    private const string CartStoreName = "CartItems";

    public CartEffects(IStorageService storage)
    {
        _storage = storage;
    }

    [EffectMethod(typeof(LoadCartAction))]
    public async Task HandleLoadCart(IDispatcher dispatcher)
    {
        try
        {
            var items = await _storage.GetAllAsync<CartItem>(CartStoreName);
            dispatcher.Dispatch(new LoadCartSuccessAction(items.ToList()));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadCartFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleAddToCart(AddToCartAction action, IDispatcher dispatcher)
    {
        try
        {
            var existingItems = await _storage.GetAllAsync<CartItem>(CartStoreName);
            var existing = existingItems.FirstOrDefault(x =>
                x.Name.Equals(action.Name, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                await _storage.UpdateAsync<CartItem>(CartStoreName, existing.Id,
                    item => item with { Quantity = item.Quantity + 1 });
                dispatcher.Dispatch(new UpdateQuantitySuccessAction(existing.Id, existing.Quantity + 1));
            }
            else
            {
                var item = new CartItem
                {
                    Name = action.Name,
                    Price = action.Price,
                    Quantity = 1,
                    AddedAt = DateTime.UtcNow,
                    ImageUrl = action.ImageUrl,
                    Category = action.Category
                };

                await _storage.AddAsync(CartStoreName, item);
                var items = await _storage.GetAllAsync<CartItem>(CartStoreName);
                var savedItem = items.First(x =>
                    x.Name.Equals(action.Name, StringComparison.OrdinalIgnoreCase));
                dispatcher.Dispatch(new AddToCartSuccessAction(savedItem));
            }
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new AddToCartFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleRemoveFromCart(RemoveFromCartAction action, IDispatcher dispatcher)
    {
        try
        {
            await _storage.DeleteAsync(CartStoreName, action.ItemId);
            dispatcher.Dispatch(new RemoveFromCartSuccessAction(action.ItemId));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to remove from cart: {ex.Message}");
        }
    }

    [EffectMethod]
    public async Task HandleUpdateQuantity(UpdateQuantityAction action, IDispatcher dispatcher)
    {
        try
        {
            if (action.Quantity <= 0)
                await _storage.DeleteAsync(CartStoreName, action.ItemId);
            else
                await _storage.UpdateAsync<CartItem>(CartStoreName, action.ItemId,
                    item => item with { Quantity = action.Quantity });

            dispatcher.Dispatch(new UpdateQuantitySuccessAction(action.ItemId, action.Quantity));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to update quantity: {ex.Message}");
        }
    }

    [EffectMethod(typeof(ClearCartAction))]
    public async Task HandleClearCart(IDispatcher dispatcher)
    {
        try
        {
            await _storage.ClearStoreAsync(CartStoreName);
            dispatcher.Dispatch(new ClearCartSuccessAction());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to clear cart: {ex.Message}");
        }
    }
}
