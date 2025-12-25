using Fluxor;

namespace BlazorStateApp.Client.Store.Theme;

/// <summary>
/// Pure functions for theme state updates.
/// </summary>
public static class ThemeReducers
{
    [ReducerMethod(typeof(LoadThemeAction))]
    public static ThemeState ReduceLoadTheme(ThemeState state) =>
        state with { IsLoading = true };

    [ReducerMethod]
    public static ThemeState ReduceLoadThemeSuccess(ThemeState state, LoadThemeSuccessAction action) =>
        state with { CurrentTheme = action.Theme, IsLoading = false };

    [ReducerMethod]
    public static ThemeState ReduceSetTheme(ThemeState state, SetThemeAction action) =>
        Themes.IsValid(action.Theme)
            ? state with { IsLoading = true, ErrorMessage = null }
            : state with { ErrorMessage = $"Invalid theme: {action.Theme}" };

    [ReducerMethod]
    public static ThemeState ReduceSetThemeSuccess(ThemeState state, SetThemeSuccessAction action) =>
        state with { CurrentTheme = action.Theme, IsLoading = false, ErrorMessage = null };

    [ReducerMethod]
    public static ThemeState ReduceSetThemeFailure(ThemeState state, SetThemeFailureAction action) =>
        state with { IsLoading = false, ErrorMessage = action.ErrorMessage };
}
