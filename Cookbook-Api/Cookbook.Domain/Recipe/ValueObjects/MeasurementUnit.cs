namespace Cookbook.Domain.Recipe.ValueObjects;

public record MeasurementUnit
{
    public string Symbol { get; }

    protected MeasurementUnit(string symbol)
    {
        Symbol = symbol;
    }

    public override string ToString() => Symbol;

    public static MeasurementUnit Fluid(FluidUnit unit) => new FluidMeasurement(unit);
    public static MeasurementUnit Weight(WeightUnit unit) => new WeightMeasurement(unit);
    public static MeasurementUnit Piece() => new PieceMeasurement();
}

public record FluidMeasurement : MeasurementUnit
{
    public FluidUnit Unit { get; }

    public FluidMeasurement(FluidUnit unit)
        : base(unit.ToString())
    {
        Unit = unit;
    }
}

public record WeightMeasurement : MeasurementUnit
{
    public WeightUnit Unit { get; }

    public WeightMeasurement(WeightUnit unit)
        : base(unit.ToString())
    {
        Unit = unit;
    }
}

public record PieceMeasurement : MeasurementUnit
{
    public PieceMeasurement()
        : base("pc")
    {
    }
}

public enum FluidUnit
{
    L,
    Dl,
    Cl,
    Ml
}

public enum WeightUnit
{
    Kg,
    G,
    Mg
}
