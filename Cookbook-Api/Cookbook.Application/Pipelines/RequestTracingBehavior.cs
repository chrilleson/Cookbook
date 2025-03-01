using System.Diagnostics;
using Ardalis.Result;
using MediatR;
using OpenTelemetry.Trace;

namespace Cookbook.Application.Pipelines;

public class RequestTracingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private static readonly ActivitySource ActivitySource = new("Cookbook.Application");

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        if (!requestName.EndsWith("Query") && !requestName.EndsWith("Command"))
        {
            return await next();
        }

        using var activity = ActivitySource.StartActivity(requestName);
        activity?.SetStartTime(DateTime.UtcNow);
        try
        {
            AddRequestTags(activity, request);
            var response = await next();

            return response;
        }
        catch (Exception ex)
        {
            activity?.SetEndTime(DateTime.UtcNow);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            throw;
        }
    }

    private static void AddRequestTags(Activity? activity, TRequest request)
    {
        if (activity is null)
        {
            return;
        }

        activity.SetTag("request.type", typeof(TRequest).Name);

        var commonProperties = new[] { "Id", "Name", "Title" };
        foreach (var propertyName in commonProperties)
        {
            if (request?.GetType().GetProperty(propertyName)?.GetValue(request) is { } value)
            {
                activity.SetTag($"request.{propertyName.ToLower()}", value);
            }
        }
    }
}