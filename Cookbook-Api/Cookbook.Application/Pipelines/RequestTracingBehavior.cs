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
            AddResponseTags(activity, response);

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

    private static void AddResponseTags(Activity? activity, TResponse response)
    {
        if (activity == null || response == null) return;

        if (response is not IResult result) return;

        activity.SetTag("response.status", result.Status.ToString());

        if (result.IsOk())
        {
            activity.SetStatus(ActivityStatusCode.Ok);
        }
        else
        {
            activity.SetStatus(ActivityStatusCode.Error,
                result.Errors?.FirstOrDefault() ?? "Operation failed");

            if (result.Errors == null) return;

            foreach (var error in result.Errors)
            {
                activity.AddEvent(new ActivityEvent("Error",
                    tags: new ActivityTagsCollection { { "error.message", error } }));
            }
        }
    }
}