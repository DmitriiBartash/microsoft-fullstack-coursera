namespace BlazorStateApp.Client.Services.Storage;

/// <summary>
/// Abstraction for persistent storage operations.
/// </summary>
public interface IStorageService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value);

    Task<IEnumerable<T>> GetAllAsync<T>(string storeName);
    Task<T> AddAsync<T>(string storeName, T item);
    Task UpdateAsync<T>(string storeName, int id, Func<T, T> updateFunc);
    Task DeleteAsync(string storeName, int id);
    Task ClearStoreAsync(string storeName);
}
