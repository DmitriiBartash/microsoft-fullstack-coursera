namespace BlazorStateApp.Client.Store.Theme;

public record LoadThemeAction;
public record LoadThemeSuccessAction(string Theme);
public record SetThemeAction(string Theme);
public record SetThemeSuccessAction(string Theme, DateTime Timestamp);
public record SetThemeFailureAction(string ErrorMessage);
