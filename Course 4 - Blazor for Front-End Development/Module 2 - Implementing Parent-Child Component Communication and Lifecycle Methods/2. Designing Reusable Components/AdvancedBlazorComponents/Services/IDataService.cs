namespace AdvancedBlazorComponents.Services;

public interface IDataService
{
    Task<List<string>> GetData();
}
