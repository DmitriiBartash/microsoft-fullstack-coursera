using NSwag;
using NSwag.CodeGeneration.CSharp;

public class ClientGenerator
{
    private readonly string _baseUrl;

    public ClientGenerator(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task GenerateClient()
    {
        using var httpClient = new HttpClient();
        var swaggerUrl = $"{_baseUrl}/swagger/v1/swagger.json";

        var swaggerJson = await httpClient.GetStringAsync(swaggerUrl);
        var document = await OpenApiDocument.FromJsonAsync(swaggerJson);

        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = "CustomApiClient",
            CSharpGeneratorSettings = { Namespace = "CustomNamespace" }
        };

        var generator = new CSharpClientGenerator(document, settings);
        var code = generator.GenerateFile();

        await File.WriteAllTextAsync("GeneratedApiClient.cs", code);
        Console.WriteLine($"Client code generated from {swaggerUrl}");
    }
}
