using Ardalis.Result;

namespace Cookbook.Application.Extensions;

public static class ValidationErrorExtensions
{
    public static IEnumerable<KeyValuePair<string, string[]>> AsDictionary(this IEnumerable<ValidationError> errors) =>
        errors.ToDictionary(x => x.Identifier, x => new[] { x.ErrorMessage });
}
