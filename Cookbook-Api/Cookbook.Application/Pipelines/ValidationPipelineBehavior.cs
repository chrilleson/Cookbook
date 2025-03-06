using System.Diagnostics;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Cookbook.Application.Pipelines;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private static readonly ActivitySource ActivitySource = new("Cookbook.Application");

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        using var activity = ActivitySource.StartActivity($"Validate {requestName}");

        activity?.SetStartTime(DateTime.UtcNow);
        try
        {
            var (resultErrors, failures) = await ValidateAsync(request, cancellationToken);

            activity?.SetEndTime(DateTime.UtcNow);
            activity?.SetTag("validation.error_count", resultErrors.Count);
            activity?.SetTag("validation.failure_count", failures.Count);

            if (resultErrors is { Count: 0 } && failures is { Count: 0 })
            {
                activity?.SetTag("validation.status", "success");
                activity?.SetStatus(ActivityStatusCode.Ok);
                return await next();
            }

            foreach (var error in resultErrors)
            {
                activity?.AddEvent(new ActivityEvent("ValidationError",
                    tags: new ActivityTagsCollection {
                        { "error.property", error.Identifier },
                        { "error.message", error.ErrorMessage }
                    }));
            }

            activity?.SetTag("validation.status", "failed");
            activity?.SetStatus(ActivityStatusCode.Error);

            return typeof(TResponse) switch
            {
                { IsGenericType: true } when typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>) => GetGenericResult(resultErrors),
                { IsGenericType: false } when typeof(TResponse) == typeof(Result) => (TResponse)(object)Result.Invalid(resultErrors),
                _ => throw new ValidationException(failures)
            };
        }
        catch (Exception ex)
        {
            activity?.SetEndTime(DateTime.UtcNow);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }
    }

    private async Task<(List<ValidationError> resultErrors, List<ValidationFailure> failures)> ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return ([], []);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var resultErrors = validationResults.SelectMany(r => r.AsErrors()).ToList();
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(failure => failure is not null)
            .ToList();

        return (resultErrors, failures);
    }

    private static TResponse GetGenericResult(List<ValidationError> resultErrors)
    {
        var resultType = typeof(TResponse).GetGenericArguments()[0];
        var invalidMethod = typeof(Result<>)
            .MakeGenericType(resultType)
            .GetMethod(nameof(Result<int>.Invalid), [typeof(List<ValidationError>)]);

        if (invalidMethod == null)
        {
            throw new InvalidOperationException("Invalid method not found");
        }

        return (TResponse)invalidMethod.Invoke(null, [resultErrors])!;
    }
}
