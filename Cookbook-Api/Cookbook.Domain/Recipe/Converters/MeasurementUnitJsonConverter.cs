using System.Text.Json;
using System.Text.Json.Serialization;
using Cookbook.Domain.Recipe.ValueObjects;
using Cookbook.Domain.Shared.Enums;

namespace Cookbook.Domain.Recipe.Converters;

public sealed class MeasurementUnitJsonConverter : JsonConverter<MeasurementUnit>
{
    public override MeasurementUnit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;

            if (!root.TryGetProperty("unitType", out var unitTypeElement))
                throw new JsonException("Missing unitType property");

            if (!root.TryGetProperty("symbol", out var symbolElement))
                throw new JsonException("Missing symbol property");

            string unitType = unitTypeElement.GetString();
            string symbol = symbolElement.GetString();

            return unitType switch
            {
                "Fluid" => new FluidMeasurement((Fluid)Enum.Parse(typeof(Fluid), symbol)),
                "Weight" => new WeightMeasurement((Weight)Enum.Parse(typeof(Weight), symbol)),
                "Piece" => new PieceMeasurement(),
                _ => throw new JsonException($"Unknown unit type: {unitType}")
            };
        }
    }

    public override void Write(Utf8JsonWriter writer, MeasurementUnit value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("symbol", value.Symbol);
        writer.WriteString("unitType", value.UnitType);
        writer.WriteEndObject();
    }
}
