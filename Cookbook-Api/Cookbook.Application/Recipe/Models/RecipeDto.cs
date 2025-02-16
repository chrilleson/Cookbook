
namespace Cookbook.Application.Recipe.Models;

public record RecipeDto(
    int Id,
    string Name,
    string Description,
    IEnumerable<string> Instructions,
    IEnumerable<IngredientDto> Ingredients,
    uint RowVersion);