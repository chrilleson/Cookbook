using FluentValidation;

namespace Cookbook.Application.Recipe.Commands.Validators;

public class UpdateRecipeValidator : AbstractValidator<UpdateRecipeCommand>
{
    public UpdateRecipeValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Recipe.Name)
            .NotEmpty()
            .When(x => x.Recipe.Name is not null)
            .WithMessage("Name is required");
        RuleFor(x => x.Recipe.Ingredients)
            .NotEmpty()
            .When(x => x.Recipe.Ingredients is not null)
            .WithMessage("Ingredients are required");
        RuleFor(x => x.Recipe.Instructions)
            .NotEmpty()
            .When(x => x.Recipe.Instructions is not null)
            .WithMessage("Instructions are required");
        When(x => x.Recipe.Ingredients?.Any() ?? false, () =>
        {
            RuleForEach(x => x.Recipe.Ingredients).SetValidator(new IngredientDtoValidator());
            RuleFor(x => x.Recipe.Ingredients!.Select(x => x.Name))
                .Must(x => x.Distinct().Count() == x.Count())
                .WithMessage("Ingredient names must be unique");
        });
        RuleFor(x => x.Recipe.RowVersion)
            .NotEmpty()
            .WithMessage("Row version is required");
    }
}
