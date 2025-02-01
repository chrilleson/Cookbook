using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Cookbook.Application.Pipelines;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var (resultErrors, failures) = await ValidateAsync(request, cancellationToken);

        if (resultErrors is { Count: 0 } && failures is { Count: 0 }) return await next();

        return typeof(TResponse) switch
        {
            { IsGenericType: true } when typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>) => GetGenericResult(resultErrors),
            { IsGenericType: false } when typeof(TResponse) == typeof(Result) => (TResponse)(object)Result.Invalid(resultErrors),
            _ => throw new FluentValidation.ValidationException(failures)
        };
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
        if (invalidMethod != null)
        {
            return (TResponse)invalidMethod.Invoke(null, new object[] { resultErrors });
        }
        throw new InvalidOperationException("Invalid method not found");
    }
}