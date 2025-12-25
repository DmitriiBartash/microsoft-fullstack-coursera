using Fluxor;

namespace BlazorStateApp.Client.Store.Theme;

/// <summary>
/// Immutable state for application theme.
/// </summary>
[FeatureState]
public record ThemeState
{
    public string CurrentTheme { get; init; } = Themes.Light;
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }

    private ThemeState() { }
}

/// <summary>
/// Available theme constants.
/// </summary>
public static class Themes
{
    public const string Light = "light";
    public const string Dark = "dark";

    private static readonly HashSet<string> ValidThemes = new(StringComparer.OrdinalIgnoreCase) { Light, Dark };

    public static bool IsValid(string theme) => ValidThemes.Contains(theme);
}
