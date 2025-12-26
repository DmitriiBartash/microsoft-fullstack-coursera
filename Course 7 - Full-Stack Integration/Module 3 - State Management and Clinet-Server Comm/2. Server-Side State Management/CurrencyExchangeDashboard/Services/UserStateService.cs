using Blazored.SessionStorage;
using CurrencyExchangeDashboard.Models;
using CurrencyExchangeDashboard.Services.Interfaces;

namespace CurrencyExchangeDashboard.Services;

public class UserStateService : IUserStateService
{
    private readonly ISessionStorageService _sessionStorage;

    private const string PreferencesKey = "user_preferences";
    private const string WalletKey = "user_wallet";
    private const string TransactionsKey = "user_transactions";
    private const string ConversionsKey = "user_conversions";
    private const int MaxTransactions = 10;
    private const int MaxConversions = 10;

    public UserStateService(ISessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public async Task<UserPreferences> GetPreferencesAsync()
    {
        try
        {
            var preferences = await _sessionStorage.GetItemAsync<UserPreferences>(PreferencesKey);
            return preferences ?? UserPreferences.Default;
        }
        catch
        {
            return UserPreferences.Default;
        }
    }

    public async Task SavePreferencesAsync(UserPreferences preferences)
    {
        await _sessionStorage.SetItemAsync(PreferencesKey, preferences);
    }

    public async Task<List<WalletEntry>> GetWalletAsync()
    {
        try
        {
            var wallet = await _sessionStorage.GetItemAsync<List<WalletEntry>>(WalletKey);
            return wallet ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task SaveWalletAsync(List<WalletEntry> wallet)
    {
        await _sessionStorage.SetItemAsync(WalletKey, wallet);
    }

    public async Task<List<Transaction>> GetTransactionsAsync()
    {
        try
        {
            var transactions = await _sessionStorage.GetItemAsync<List<Transaction>>(TransactionsKey);
            return transactions ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        var transactions = await GetTransactionsAsync();
        transactions.Insert(0, transaction);

        if (transactions.Count > MaxTransactions)
        {
            transactions = transactions.Take(MaxTransactions).ToList();
        }

        await _sessionStorage.SetItemAsync(TransactionsKey, transactions);
    }

    public async Task<List<ConversionRecord>> GetConversionHistoryAsync()
    {
        try
        {
            var conversions = await _sessionStorage.GetItemAsync<List<ConversionRecord>>(ConversionsKey);
            return conversions ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task AddConversionAsync(ConversionRecord record)
    {
        var conversions = await GetConversionHistoryAsync();
        conversions.Insert(0, record);

        if (conversions.Count > MaxConversions)
        {
            conversions = conversions.Take(MaxConversions).ToList();
        }

        await _sessionStorage.SetItemAsync(ConversionsKey, conversions);
    }
}
