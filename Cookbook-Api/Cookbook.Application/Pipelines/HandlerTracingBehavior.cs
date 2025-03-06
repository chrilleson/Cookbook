using System.Diagnostics;
using Ardalis.Result;
using MediatR;

namespace Cookbook.Application.Pipelines;

public class HandlerTracingBehavior <TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private static readonly ActivitySource ActivitySource = new("Cookbook.Application");

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!ResponseIsGenericResult() && !ResponseIsResult())
        {
            return await next();
        }

        var requestName = $"Handler {typeof(TRequest).Name}";
        using var activity = ActivitySource.StartActivity($"Handler {requestName}");

        activity?.SetStartTime(DateTime.UtcNow);
        try
        {
            activity?.SetTag("handler.status", "processing");

            var response = await next();
            activity?.SetEndTime(DateTime.UtcNow);

            if (response is not IResult result) return response;

            activity?.SetTag("handler.status", result.Status.ToString());
            var activityStatus = result switch
            {
                _ when result.IsOk() => ActivityStatusCode.Ok,
                _ when result.IsCreated() => ActivityStatusCode.Ok,
                _ when result.IsNoContent() => ActivityStatusCode.Ok,
                _ when result.IsError() => ActivityStatusCode.Error,
                _ when result.IsConflict() => ActivityStatusCode.Error,
                _ => ActivityStatusCode.Unset
            };

            activity?.SetStatus(activityStatus);

            if (result.IsOk() || result.Errors == null) return response;

            foreach (var error in result.Errors)
            {
                activity?.AddEvent(new ActivityEvent("HandlerError",
                    tags: new ActivityTagsCollection { { "error.message", error } }));
            }

            return response;
        }
        catch (Exception ex)
        {
            activity?.SetEndTime(DateTime.UtcNow);
            activity?.SetTag("handler.status", "error");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }
    }

    private static bool ResponseIsGenericResult() => typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>);

    private static bool ResponseIsResult() => !typeof(TResponse).IsGenericType && typeof(TResponse) == typeof(Result);
}