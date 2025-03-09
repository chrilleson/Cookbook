using System.Reflection;
using Cookbook.Application.Pipelines;
using Cookbook.Application.Recipe.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Application;

public static class ApplicationExtensions
{
    private static Assembly TargetAssembly => typeof(ApplicationExtensions).Assembly;

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(TargetAssembly);
            cfg.AddOpenBehavior(typeof(RequestTracingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            cfg.AddOpenBehavior(typeof(HandlerTracingBehavior<,>));
        });
        services.AddValidatorsFromAssembly(TargetAssembly);

        services.AddScoped<IRecipeService, RecipeService>();

        return services;
    }
}
