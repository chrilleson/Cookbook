using System.Reflection;
using Cookbook.Application.Pipelines;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Application;

public static class ApplicationExtensions
{
    private static Assembly TargetAssembly => typeof(ApplicationExtensions).Assembly;

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(TargetAssembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationPipelineBehavior<,>));
        services.AddValidatorsFromAssembly(TargetAssembly);

        return services;
    }
}