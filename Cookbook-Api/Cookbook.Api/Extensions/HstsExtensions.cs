namespace Cookbook.Api.Extensions;

public static class HstsExtensions
{
    public static IServiceCollection AddHsts(this IServiceCollection services) =>
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });
}