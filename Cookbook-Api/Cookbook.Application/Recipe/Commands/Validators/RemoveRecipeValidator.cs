using FluentValidation;

namespace Cookbook.Application.Recipe.Commands.Validators;

public class RemoveRecipeValidator : AbstractValidator<RemoveRecipeCommand>
{
    public RemoveRecipeValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}
