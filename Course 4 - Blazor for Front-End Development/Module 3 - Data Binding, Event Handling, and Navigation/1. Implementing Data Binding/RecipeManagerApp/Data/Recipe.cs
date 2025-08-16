using System.ComponentModel.DataAnnotations;

namespace RecipeManagerApp.Data;

public class Recipe
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required, StringLength(500)]
    public string Description { get; set; } = "";
}
