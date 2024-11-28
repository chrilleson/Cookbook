using Cookbook.Domain.Units;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Cookbook.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services) =>
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Cookbook.Api", Version = "v1" });
            options.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("00:00:00")
            });
            options.MapType<Fluid>(() => new OpenApiSchema
            {
                Type = "string",
                Enum = Enum.GetNames<Fluid>().Select(name => new OpenApiString(name)).ToList<IOpenApiAny>()
            });
            options.MapType<Weight>(() => new OpenApiSchema
            {
                Type = "string",
                Enum = Enum.GetNames<Weight>().Select(name => new OpenApiString(name)).ToList<IOpenApiAny>()
            });
        });
}