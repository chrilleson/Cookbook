using System.Reflection;
using Cookbook.Common.Json;

namespace Cookbook.Api.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services
            .AddVersioning()
            .AddHttpClient()
            .AddHttpContextAccessor()
            .AddHsts()
            .AddCors()
            .AddEndpoints(Assembly.GetExecutingAssembly())
            .ConfigureHttpJsonOptions(options => CustomJsonOptions.Configure(options.SerializerOptions))
            .AddEndpointsApiExplorer()
            .AddSwagger();
        services.AddHealthChecks();

        return services;
    }
}
