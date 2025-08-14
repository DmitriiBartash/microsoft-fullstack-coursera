namespace AdvancedBlazorComponents.Services;

public class DataService : IDataService
{
    private readonly Random _rng = new();

    public Task<List<string>> GetData()
    {
        var techs = new List<string>
        {
            "Blazor", "C#", "Dependency Injection", ".NET 9", "WebAssembly",
            "Reusable UI", "Razor Components", "CSS Animations",
            "Parent-Child Communication", "Bootstrap 5"
        };

        return Task.FromResult(techs.OrderBy(_ => _rng.Next()).ToList());
    }
}
