using System.Diagnostics;
using Ardalis.Result;
using MediatR;
using OpenTelemetry.Trace;

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
            activity?.SetStatus(result.IsOk() ? ActivityStatusCode.Ok : ActivityStatusCode.Error);

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
            activity?.RecordException(ex);
            throw;
        }
    }

    private static bool ResponseIsGenericResult() => typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>);

    private static bool ResponseIsResult() => !typeof(TResponse).IsGenericType && typeof(TResponse) == typeof(Result);
}