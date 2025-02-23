using FluentValidation;

namespace Cookbook.Application.Recipe.Queries.Validators;

public class GetRecipeByIdValidator : AbstractValidator<GetRecipeByIdQuery>
{
    public GetRecipeByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}