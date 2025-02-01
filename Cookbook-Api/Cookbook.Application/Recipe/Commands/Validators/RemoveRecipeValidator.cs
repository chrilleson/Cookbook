﻿using Cookbook.Repositories;
using FluentValidation;

namespace Cookbook.Application.Recipe.Commands.Validators;

public class RemoveRecipeValidator : AbstractValidator<RemoveRecipeCommand>
{
    public RemoveRecipeValidator(IRecipeRepository recipeRepository)
    {
        RuleFor(x => x.Id)
            .MustAsync(async (id, cancellationToken) =>
            {
                var recipe = await recipeRepository.AnyById(id, cancellationToken);
                return recipe;
            })
            .WithMessage("Recipe with id {PropertyValue} not found");
    }
}