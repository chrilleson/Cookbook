namespace Cookbook.Domain.Recipe.ValueObjects;

public record RecipeId
{
    public int Value { get; }

    public RecipeId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Recipe ID must be positive", nameof(value));

        Value = value;
    }

    public static implicit operator int(RecipeId id) => id.Value;

    public override string ToString() => Value.ToString();
}
