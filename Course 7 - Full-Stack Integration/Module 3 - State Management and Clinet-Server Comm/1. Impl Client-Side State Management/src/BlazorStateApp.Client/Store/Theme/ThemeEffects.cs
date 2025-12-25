using Fluxor;
using BlazorStateApp.Client.Services.Storage;
using Microsoft.JSInterop;

namespace BlazorStateApp.Client.Store.Theme;

/// <summary>
/// Side effects for theme operations (storage, DOM manipulation).
/// </summary>
public class ThemeEffects
{
    private readonly IStorageService _storage;
    private readonly IJSRuntime _jsRuntime;
    private const string ThemeKey = "app_theme";

    public ThemeEffects(IStorageService storage, IJSRuntime jsRuntime)
    {
        _storage = storage;
        _jsRuntime = jsRuntime;
    }

    [EffectMethod(typeof(LoadThemeAction))]
    public async Task HandleLoadTheme(IDispatcher dispatcher)
    {
        try
        {
            var theme = await _storage.GetAsync<string>(ThemeKey) ?? Themes.Light;
            await ApplyThemeToDom(theme);
            dispatcher.Dispatch(new LoadThemeSuccessAction(theme));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new SetThemeFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleSetTheme(SetThemeAction action, IDispatcher dispatcher)
    {
        try
        {
            if (!Themes.IsValid(action.Theme))
            {
                dispatcher.Dispatch(new SetThemeFailureAction($"Invalid theme: {action.Theme}"));
                return;
            }

            await _storage.SetAsync(ThemeKey, action.Theme);
            await ApplyThemeToDom(action.Theme);
            dispatcher.Dispatch(new SetThemeSuccessAction(action.Theme, DateTime.UtcNow));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new SetThemeFailureAction(ex.Message));
        }
    }

    private async Task ApplyThemeToDom(string theme) =>
        await _jsRuntime.InvokeVoidAsync(
            "eval",
            $"document.documentElement.setAttribute('data-bs-theme', '{theme}')"
        );
}
