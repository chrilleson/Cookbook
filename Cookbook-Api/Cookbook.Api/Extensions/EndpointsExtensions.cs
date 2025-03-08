using System.Reflection;
using Cookbook.Api.Endpoints;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cookbook.Api.Extensions;

public static class EndpointsExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var serviceDescriptors = assembly
            .DefinedTypes
            .Where(x => x is { IsAbstract: false, IsInterface: false } && x.ImplementedInterfaces.Contains(typeof(IEndpoint)))
            .Select(x => ServiceDescriptor.Transient(typeof(IEndpoint), x))
            .ToList();

        services.TryAddEnumerable(serviceDescriptors);
        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
        IEndpointRouteBuilder routeBuilder = routeGroupBuilder is null ? app : routeGroupBuilder;
        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(routeBuilder);
        }

        return app;
    }
}
