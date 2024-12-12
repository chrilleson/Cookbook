using Cookbook.Application.Recipe.Models;
using FluentValidation;

namespace Cookbook.Application.Recipe.Commands.Validators;

public class CreateRecipeValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeValidator()
    {
        RuleFor(x => x.Recipe.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Recipe.Instructions).NotEmpty().WithMessage("Instructions are required");
        RuleFor(x => x.Recipe.Ingredients).NotEmpty().WithMessage("Ingredients are required");
        When(x => x.Recipe.Ingredients.Any(), () =>
        {
            RuleForEach(x => x.Recipe.Ingredients).SetValidator(new IngredientDtoValidator());
            RuleFor(x => x.Recipe.Ingredients.Select(x => x.Name))
                .Must(x => x.Distinct().Count() == x.Count())
                .WithMessage("Ingredient names must be unique");
        });
    }
}

public class IngredientDtoValidator : AbstractValidator<IngredientDto>
{
    public IngredientDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Ingredient name is required");
        RuleFor(x => x.Amount).NotEmpty().WithMessage("Ingredient amount is required");
        RuleFor(x => x.Unit).Must(x => x.Fluid is not null || x.Weight is not null).WithMessage("Ingredient unit must be either fluid or weight");
        RuleFor(x => x.Unit.Fluid)
            .IsInEnum()
            .When(x => x.Unit.Fluid is not null)
            .WithMessage("Fluid unit is invalid");
        RuleFor(x => x.Unit.Weight)
            .IsInEnum()
            .When(x => x.Unit.Weight is not null)
            .WithMessage("Weight unit is invalid");
    }
}