using System.ComponentModel.DataAnnotations;
using Cookbook.Domain.Recipe.ValueObjects;

namespace Cookbook.Domain.Recipe.Entities;

public sealed class Recipe
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required Dictionary<int, string> Instructions { get; set; }
    public required IEnumerable<Ingredient> Ingredients { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
