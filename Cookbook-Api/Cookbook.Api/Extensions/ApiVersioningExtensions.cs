using Microsoft.AspNetCore.Mvc;

namespace Cookbook.Api.Extensions;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddVersioning(this IServiceCollection services) =>
        services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
        });
}