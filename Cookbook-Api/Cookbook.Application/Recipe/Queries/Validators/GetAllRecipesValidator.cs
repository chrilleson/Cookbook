using FluentValidation;

namespace Cookbook.Application.Recipe.Queries.Validators;

public class GetAllRecipesValidator : AbstractValidator<GetAllRecipesQuery>
{
    public GetAllRecipesValidator()
    {
    }
}