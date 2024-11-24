using System.Reflection;
using Cookbook.Common.Json;

namespace Cookbook.Api.Extensions;

public static class ApiExtensions
{
    public static void AddApi(this IServiceCollection services)
    {
        services.AddVersioning();
        services.AddHealthChecks();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddHsts();
        services.AddCors();
        services.AddEndpoints(Assembly.GetExecutingAssembly());
        services.ConfigureHttpJsonOptions(options => CustomJsonOptions.Configure(options.SerializerOptions));
        services.AddEndpointsApiExplorer();
        services.AddSwagger();
    }
}