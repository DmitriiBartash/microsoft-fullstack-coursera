using System.Text.Json;
using TG.Blazor.IndexedDB;

namespace BlazorStateApp.Client.Services.Storage;

/// <summary>
/// IStorageService implementation via IndexedDB.
/// Uses TG.Blazor.IndexedDB for browser interaction.
/// </summary>
public class IndexedDbStorageService : IStorageService
{
    private readonly IndexedDBManager _dbManager;
    private const string SettingsStore = "Settings";

    public IndexedDbStorageService(IndexedDBManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var record = await _dbManager.GetRecordById<string, SettingsRecord>(SettingsStore, key);

            if (record == null)
                return default;

            return JsonSerializer.Deserialize<T>(record.Value);
        }
        catch
        {
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var record = new SettingsRecord
        {
            Id = key,
            Value = JsonSerializer.Serialize(value),
            UpdatedAt = DateTime.UtcNow
        };

        try
        {
            var existing = await _dbManager.GetRecordById<string, SettingsRecord>(SettingsStore, key);

            var storeRecord = new StoreRecord<SettingsRecord>
            {
                Storename = SettingsStore,
                Data = record
            };

            if (existing != null)
            {
                await _dbManager.UpdateRecord(storeRecord);
            }
            else
            {
                await _dbManager.AddRecord(storeRecord);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"IndexedDB SetAsync error: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(string storeName)
    {
        try
        {
            var records = await _dbManager.GetRecords<T>(storeName);
            return records ?? Enumerable.Empty<T>();
        }
        catch
        {
            return Enumerable.Empty<T>();
        }
    }

    public async Task<T> AddAsync<T>(string storeName, T item)
    {
        var storeRecord = new StoreRecord<T>
        {
            Storename = storeName,
            Data = item
        };
        await _dbManager.AddRecord(storeRecord);
        return item;
    }

    public async Task UpdateAsync<T>(string storeName, int id, Func<T, T> updateFunc)
    {
        var existing = await _dbManager.GetRecordById<int, T>(storeName, id);

        if (existing != null)
        {
            var updated = updateFunc(existing);
            var storeRecord = new StoreRecord<T>
            {
                Storename = storeName,
                Data = updated
            };
            await _dbManager.UpdateRecord(storeRecord);
        }
    }

    public async Task DeleteAsync(string storeName, int id)
    {
        await _dbManager.DeleteRecord(storeName, id);
    }

    public async Task ClearStoreAsync(string storeName) =>
        await _dbManager.ClearStore(storeName);
}

/// <summary>
/// Internal record for storing settings in IndexedDB
/// </summary>
internal record SettingsRecord
{
    public string Id { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; }
}
