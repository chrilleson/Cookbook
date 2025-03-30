using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cookbook.Common.Json;

public class CustomJsonOptions
{
    private static readonly JsonSerializerOptions _options = Configure(new JsonSerializerOptions());

    public static JsonSerializerOptions Value => _options;

    public static JsonSerializerOptions Configure(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.Converters.Add(new JsonStringEnumConverter());

        options.PropertyNameCaseInsensitive = true;
        options.IncludeFields = true;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        return options;
    }
}
