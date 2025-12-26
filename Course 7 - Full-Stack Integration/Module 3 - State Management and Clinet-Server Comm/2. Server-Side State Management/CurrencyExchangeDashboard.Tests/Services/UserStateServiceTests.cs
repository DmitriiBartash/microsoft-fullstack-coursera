using Blazored.SessionStorage;
using CurrencyExchangeDashboard.Models;
using CurrencyExchangeDashboard.Services;
using Moq;

namespace CurrencyExchangeDashboard.Tests.Services;

public class UserStateServiceTests
{
    private readonly Mock<ISessionStorageService> _sessionStorageMock;
    private readonly UserStateService _sut;

    public UserStateServiceTests()
    {
        _sessionStorageMock = new Mock<ISessionStorageService>();
        _sut = new UserStateService(_sessionStorageMock.Object);
    }

    #region Preferences Tests

    [Fact]
    public async Task GetPreferencesAsync_WhenNoPreferences_ReturnsDefault()
    {
        // Arrange
#pragma warning disable CS8620
        _sessionStorageMock
            .Setup(s => s.GetItemAsync<UserPreferences>("user_preferences", default))
            .ReturnsAsync((UserPreferences?)null);
#pragma warning restore CS8620

        // Act
        var result = await _sut.GetPreferencesAsync();

        // Assert
        Assert.Equal("USD", result.BaseCurrency);
        Assert.Contains("EUR", result.FavoriteCurrencies);
    }

    [Fact]
    public async Task GetPreferencesAsync_WhenPreferencesExist_ReturnsStoredPreferences()
    {
        // Arrange
        var storedPreferences = new UserPreferences("EUR", ["EUR", "GBP"]);

        _sessionStorageMock
            .Setup(s => s.GetItemAsync<UserPreferences>("user_preferences", default))
            .ReturnsAsync(storedPreferences!);

        // Act
        var result = await _sut.GetPreferencesAsync();

        // Assert
        Assert.Equal("EUR", result.BaseCurrency);
        Assert.Equal(2, result.FavoriteCurrencies.Count);
    }

    [Fact]
    public async Task GetPreferencesAsync_WhenStorageThrows_ReturnsDefault()
    {
        // Arrange
        _sessionStorageMock
            .Setup(s => s.GetItemAsync<UserPreferences>("user_preferences", default))
            .ThrowsAsync(new Exception("Storage error"));

        // Act
        var result = await _sut.GetPreferencesAsync();

        // Assert
        Assert.Equal(UserPreferences.Default.BaseCurrency, result.BaseCurrency);
    }

    [Fact]
    public async Task SavePreferencesAsync_StoresPreferences()
    {
        // Arrange
        var preferences = new UserPreferences("GBP", ["GBP", "USD"]);

        // Act
        await _sut.SavePreferencesAsync(preferences);

        // Assert
        _sessionStorageMock.Verify(
            s => s.SetItemAsync("user_preferences", preferences, default),
            Times.Once);
    }

    #endregion

    #region Wallet Tests

    [Fact]
    public async Task GetWalletAsync_WhenNoWallet_ReturnsEmptyList()
    {
        // Arrange
#pragma warning disable CS8620
        _sessionStorageMock
            .Setup(s => s.GetItemAsync<List<WalletEntry>>("user_wallet", default))
            .ReturnsAsync((List<WalletEntry>?)null);
#pragma warning restore CS8620

        // Act
        var result = await _sut.GetWalletAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetWalletAsync_WhenWalletExists_ReturnsStoredWallet()
    {
        // Arrange
        var wallet = new List<WalletEntry>
        {
            new("USD", 1000m),
            new("EUR", 500m)
        };

        _sessionStorageMock
            .Setup(s => s.GetItemAsync<List<WalletEntry>>("user_wallet", default))
            .ReturnsAsync(wallet!);

        // Act
        var result = await _sut.GetWalletAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, w => w.Currency == "USD" && w.Amount == 1000m);
    }

    [Fact]
    public async Task GetWalletAsync_WhenStorageThrows_ReturnsEmptyList()
    {
        // Arrange
        _sessionStorageMock
            .Setup(s => s.GetItemAsync<List<WalletEntry>>("user_wallet", default))
            .ThrowsAsync(new Exception("Storage error"));

        // Act
        var result = await _sut.GetWalletAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SaveWalletAsync_StoresWallet()
    {
        // Arrange
        var wallet = new List<WalletEntry> { new("USD", 1000m) };

        // Act
        await _sut.SaveWalletAsync(wallet);

        // Assert
        _sessionStorageMock.Verify(
            s => s.SetItemAsync("user_wallet", wallet, default),
            Times.Once);
    }

    #endregion

    #region Transactions Tests

    [Fact]
    public async Task GetTransactionsAsync_WhenNoTransactions_ReturnsEmptyList()
    {
        // Arrange
#pragma warning disable CS8620
        _sessionStorageMock
            .Setup(s => s.GetItemAsync<List<Transaction>>("user_transactions", default))
            .ReturnsAsync((List<Transaction>?)null);
#pragma warning restore CS8620

        // Act
        var result = await _sut.GetTransactionsAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddTransactionAsync_AddsToBeginningOfList()
    {
        // Arrange
        var existingTransactions = new List<Transaction>
        {
            new(TransactionType.Buy, "EUR", 100m, 0.85m, 85m, DateTime.UtcNow.AddHours(-1))
        };

        _sessionStorageMock
            .Setup(s => s.GetItemAsync<List<Transaction>>("user_transactions", default))
            .ReturnsAsync(existingTransactions!);

        var newTransaction = new Transaction(TransactionType.Sell, "GBP", 50m, 0.73m, 68.5m, DateTime.UtcNow);
        List<Transaction>? savedTransactions = null;

        _sessionStorageMock
            .Setup(s => s.SetItemAsync("user_transactions", It.IsAny<List<Transaction>>(), default))
            .Callback<string, List<Transaction>, CancellationToken>((key, value, ct) => savedTransactions = value);

        // Act
        await _sut.AddTransactionAsync(newTransaction);

        // Assert
        Assert.NotNull(savedTransactions);
        Assert.Equal(2, savedTransactions.Count);
        Assert.Equal(TransactionType.Sell, savedTransactions[0].Type);
        Assert.Equal("GBP", savedTransactions[0].Currency);
    }

    [Fact]
    public async Task AddTransactionAsync_LimitsTo10Transactions()
    {
        // Arrange
        var existingTransactions = Enumerable.Range(1, 10)
            .Select(i => new Transaction(TransactionType.Buy, "USD", i * 10m, 1m, i * 10m, DateTime.UtcNow.AddMinutes(-i)))
            .ToList();

        _sessionStorageMock
            .Setup(s => s.GetItemAsync<List<Transaction>>("user_transactions", default))
            .ReturnsAsync(existingTransactions!);

        var newTransaction = new Transaction(TransactionType.Buy, "EUR", 999m, 0.85m, 849.15m, DateTime.UtcNow);
        List<Transaction>? savedTransactions = null;

        _sessionStorageMock
            .Setup(s => s.SetItemAsync("user_transactions", It.IsAny<List<Transaction>>(), default))
            .Callback<string, List<Transaction>, CancellationToken>((key, value, ct) => savedTransactions = value);

        // Act
        await _sut.AddTransactionAsync(newTransaction);

        // Assert
        Assert.NotNull(savedTransactions);
        Assert.Equal(10, savedTransactions.Count);
        Assert.Equal(999m, savedTransactions[0].Amount);
    }

    [Fact]
    public async Task GetTransactionsAsync_WhenStorageThrows_ReturnsEmptyList()
    {
        // Arrange
        _sessionStorageMock
            .Setup(s => s.GetItemAsync<List<Transaction>>("user_transactions", default))
            .ThrowsAsync(new Exception("Storage error"));

        // Act
        var result = await _sut.GetTransactionsAsync();

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region Conversions Tests

    [Fact]
    public async Task GetConversionHistoryAsync_WhenNoConversions_ReturnsEmptyList()
    {
        // Arrange
#pragma warning disable CS8620
        _sessionStorageMock
            .Setup(s => s.GetItemAsync<List<ConversionRecord>>("user_conversions", default))
            .ReturnsAsync((List<ConversionRecord>?)null);
#pragma warning restore CS8620

        // Act
        var result = await _sut.GetConversionHistoryAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddConversionAsync_AddsToBeginningOfList()
    {
        // Arrange
        var existingConversions = new List<ConversionRecord>
        {
            new("USD", "EUR", 100m, 85m, DateTime.UtcNow.AddHours(-1))
        };

        _sessionStorageMock
            .Setup(s => s.GetItemAsync<List<ConversionRecord>>("user_conversions", default))
            .ReturnsAsync(existingConversions!);

        var newConversion = new ConversionRecord("EUR", "GBP", 50m, 43m, DateTime.UtcNow);
        List<ConversionRecord>? savedConversions = null;

        _sessionStorageMock
            .Setup(s => s.SetItemAsync("user_conversions", It.IsAny<List<ConversionRecord>>(), default))
            .Callback<string, List<ConversionRecord>, CancellationToken>((key, value, ct) => savedConversions = value);

        // Act
        await _sut.AddConversionAsync(newConversion);

        // Assert
        Assert.NotNull(savedConversions);
        Assert.Equal(2, savedConversions.Count);
        Assert.Equal("EUR", savedConversions[0].FromCurrency);
        Assert.Equal("GBP", savedConversions[0].ToCurrency);
    }

    [Fact]
    public async Task AddConversionAsync_LimitsTo10Conversions()
    {
        // Arrange
        var existingConversions = Enumerable.Range(1, 10)
            .Select(i => new ConversionRecord("USD", "EUR", i * 10m, i * 8.5m, DateTime.UtcNow.AddMinutes(-i)))
            .ToList();

        _sessionStorageMock
            .Setup(s => s.GetItemAsync<List<ConversionRecord>>("user_conversions", default))
            .ReturnsAsync(existingConversions!);

        var newConversion = new ConversionRecord("GBP", "JPY", 100m, 18500m, DateTime.UtcNow);
        List<ConversionRecord>? savedConversions = null;

        _sessionStorageMock
            .Setup(s => s.SetItemAsync("user_conversions", It.IsAny<List<ConversionRecord>>(), default))
            .Callback<string, List<ConversionRecord>, CancellationToken>((key, value, ct) => savedConversions = value);

        // Act
        await _sut.AddConversionAsync(newConversion);

        // Assert
        Assert.NotNull(savedConversions);
        Assert.Equal(10, savedConversions.Count);
        Assert.Equal("GBP", savedConversions[0].FromCurrency);
    }

    [Fact]
    public async Task GetConversionHistoryAsync_WhenStorageThrows_ReturnsEmptyList()
    {
        // Arrange
        _sessionStorageMock
            .Setup(s => s.GetItemAsync<List<ConversionRecord>>("user_conversions", default))
            .ThrowsAsync(new Exception("Storage error"));

        // Act
        var result = await _sut.GetConversionHistoryAsync();

        // Assert
        Assert.Empty(result);
    }

    #endregion
}
