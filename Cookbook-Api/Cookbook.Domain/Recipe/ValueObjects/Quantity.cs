namespace Cookbook.Domain.Recipe.ValueObjects;

public record Quantity{
    public double Amount { get; }
    public MeasurementUnit Unit { get; }

    public Quantity(double amount, MeasurementUnit unit)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        Amount = amount;
        Unit = unit;
    }

    public override string ToString() => $"{Amount} {Unit}";
}
