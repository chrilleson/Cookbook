namespace Cookbook.Domain.Recipe;

public class Recipe
{
    public int Id { get; init; }
    public string Name { get; init; }
    public Dictionary<int, string> Instructions { get; init; }
    public IEnumerable<Ingredient> Ingredients { get; init; }

    public Recipe(string name, Dictionary<int, string> instructions, IEnumerable<Ingredient> ingredients)
    {
        Name = name;
        Instructions = instructions;
        Ingredients = ingredients;
    }
}