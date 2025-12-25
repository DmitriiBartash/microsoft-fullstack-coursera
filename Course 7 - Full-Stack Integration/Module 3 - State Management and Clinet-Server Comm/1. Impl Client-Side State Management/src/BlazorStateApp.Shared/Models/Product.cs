namespace BlazorStateApp.Shared.Models;

/// <summary>
/// Product information model.
/// </summary>
public sealed record Product(string Name, decimal Price, string Category, string Icon);
