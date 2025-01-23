﻿namespace Cookbook.Domain.Recipe;

public sealed class Recipe
{
    public int Id { get; init; }
    public string Name { get; set; }
    public Dictionary<int, string> Instructions { get; set; }
    public IEnumerable<Ingredient> Ingredients { get; set; }

    public Recipe(int id, string name, Dictionary<int, string> instructions, IEnumerable<Ingredient> ingredients)
    {
        Id = id;
        Name = name;
        Instructions = instructions;
        Ingredients = ingredients;
    }
}