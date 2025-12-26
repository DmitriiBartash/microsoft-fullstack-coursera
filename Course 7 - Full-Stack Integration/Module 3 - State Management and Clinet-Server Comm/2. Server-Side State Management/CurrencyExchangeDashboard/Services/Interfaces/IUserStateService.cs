using CurrencyExchangeDashboard.Models;

namespace CurrencyExchangeDashboard.Services.Interfaces;

public interface IUserStateService
{
    Task<UserPreferences> GetPreferencesAsync();
    Task SavePreferencesAsync(UserPreferences preferences);

    Task<List<WalletEntry>> GetWalletAsync();
    Task SaveWalletAsync(List<WalletEntry> wallet);

    Task<List<Transaction>> GetTransactionsAsync();
    Task AddTransactionAsync(Transaction transaction);

    Task<List<ConversionRecord>> GetConversionHistoryAsync();
    Task AddConversionAsync(ConversionRecord record);
}
