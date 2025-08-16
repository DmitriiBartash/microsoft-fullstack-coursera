namespace RecipeManagerApp.Data;

public class RecipeService
{
    private readonly List<Recipe> _recipes = new()
    {
        new Recipe { Id = 1, Name = "Pasta Carbonara", Description = "Creamy sauce with bacon and parmesan." },
        new Recipe { Id = 2, Name = "Greek Salad", Description = "Tomatoes, cucumbers, olives, feta." },
        new Recipe { Id = 3, Name = "Banana Bread", Description = "Moist, sweet loaf with ripe bananas." }
    };

    public IReadOnlyList<Recipe> GetAll() => _recipes;

    public Recipe? GetById(int id) => _recipes.FirstOrDefault(r => r.Id == id);

    public void Add(Recipe recipe)
    {
        recipe.Id = (_recipes.LastOrDefault()?.Id ?? 0) + 1;
        _recipes.Add(recipe);
    }
}
