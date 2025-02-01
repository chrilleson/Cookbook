using System.Reflection;
using Cookbook.Application.Pipelines;
using Cookbook.Application.Repositories;
using Cookbook.Repositories;
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
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });
        services.AddValidatorsFromAssembly(TargetAssembly);

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRecipeRepository, RecipeRepository>();

        return services;
    }
}