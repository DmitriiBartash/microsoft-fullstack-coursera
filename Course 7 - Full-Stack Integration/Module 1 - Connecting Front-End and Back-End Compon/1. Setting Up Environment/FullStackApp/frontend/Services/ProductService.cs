using System.Net.Http.Json;
using frontend.Models;

namespace frontend.Services;

public class ProductService
{
    private readonly HttpClient _http;
    public ProductService(HttpClient http) => _http = http;

    public async Task<List<Product>?> GetAll() =>
        await _http.GetFromJsonAsync<List<Product>>("api/products");

    public async Task<Product?> Add(Product product)
    {
        var response = await _http.PostAsJsonAsync("api/products", product);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Product>()
            : null;
    }

    public async Task<Product?> Update(Product product)
    {
        var response = await _http.PutAsJsonAsync($"api/products/{product.Id}", product);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Product>()
            : null;
    }

    public async Task<bool> Delete(int id)
    {
        var response = await _http.DeleteAsync($"api/products/{id}");
        return response.IsSuccessStatusCode;
    }
}
