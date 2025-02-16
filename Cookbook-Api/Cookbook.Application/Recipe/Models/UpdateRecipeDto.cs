namespace Cookbook.Application.Recipe.Models;

public record UpdateRecipeDto(string? Name, string? Description, IEnumerable<string>? Instructions, IEnumerable<IngredientDto>? Ingredients);