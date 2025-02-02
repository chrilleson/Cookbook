namespace Cookbook.Domain.Recipe;

public sealed class Recipe
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Dictionary<int, string> Instructions { get; set; }
    public IEnumerable<Ingredient> Ingredients { get; set; }
    public byte[] RowVersion { get; set; }

    public Recipe(int id, string name, string? description, Dictionary<int, string> instructions, IEnumerable<Ingredient> ingredients)
    {
        Id = id;
        Name = name;
        Description = description;
        Instructions = instructions;
        Ingredients = ingredients;
    }
}