using System.Text.Json;
using Cookbook.Common.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cookbook.Infrastructure.Persistence.Configurations.Converters;

public class JsonArrayConverter<T> : ValueConverter<IReadOnlyCollection<T>, string> where T : class
{
    private static readonly JsonSerializerOptions s_options = CustomJsonOptions.Configure(new JsonSerializerOptions());

    public JsonArrayConverter() : base(
        items => JsonSerializer.Serialize(items, s_options),
        json => JsonSerializer.Deserialize<List<T>>(json, s_options) ?? new List<T>())
    {
    }
}
