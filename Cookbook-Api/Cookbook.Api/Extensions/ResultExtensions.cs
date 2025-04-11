using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Cookbook.Application.Extensions;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Cookbook.Api.Extensions;

internal static class ResultExtensions
{
    internal static IResult MapResultToResponse<T>(this Result<T> result) =>
        result switch
        {
            { ValidationErrors: var errors, IsSuccess: false } when errors.Any() => Results.ValidationProblem(detail: "Validation failed", errors: result.ValidationErrors.AsDictionary()),
            _ => result.ToMinimalApiResult(),
        };
}
