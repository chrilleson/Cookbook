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
        services.AddControllers().AddJsonOptions(options => CustomJsonOptions.Configure(options.JsonSerializerOptions));
        services.AddEndpointsApiExplorer();
        services.AddSwagger();
    }
}