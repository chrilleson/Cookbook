using System.Linq.Expressions;
using Cookbook.Application.Recipe.Commands;
using Cookbook.Application.Recipe.Commands.Validators;
using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Units;
using Cookbook.Tests.Application.Recipe.Models;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Cookbook.Tests.Application.Recipe.Validators;

public class CreateRecipeValidatorTest
{
    [Fact]
    public void CommandIsValid_ShouldNotHaveError()
    {
        IngredientDto[] ingredients = [TestRecipe.CreateIngredientDto("Beef", 500, Unit.FromWeight(Weight.G)), TestRecipe.CreateIngredientDto("Milk", 2, Unit.FromFluid(Fluid.Dl))];
        var recipe = TestRecipe.CreateRecipeDto(
            name: "My favourite recipe",
            instructions: ["Do this", "Do that"],
            ingredients: ingredients);
        var sut = CreateSut();

        var actual = sut.TestValidate(new CreateRecipeCommand(recipe));

        actual.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public void ModelIsInvalid_ShouldHaveErrors(RecipeDto recipe, string property, string errorMessage)
    {
        var sut = CreateSut();

        var actual = sut.TestValidate(new CreateRecipeCommand(recipe));

        actual.ShouldHaveValidationErrorFor(property).WithErrorMessage(errorMessage);
    }

    public static TheoryData<RecipeDto, string, string> InvalidData = new()
    {
        { TestRecipe.CreateRecipeDto(name: "", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto()]), "Recipe.Name", "Name is required" },
        { TestRecipe.CreateRecipeDto(name: "My favourite recipe", instructions: Array.Empty<string>(), ingredients: [TestRecipe.CreateIngredientDto()]), "Recipe.Instructions", "Instructions are required" },
        { TestRecipe.CreateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: Array.Empty<IngredientDto>()), "Recipe.Ingredients", "Ingredients are required" },
        { TestRecipe.CreateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto(name: "", amount: 500, unit: Unit.FromWeight(Weight.G))]), "Recipe.Ingredients[0].Name", "Ingredient name is required" },
        { TestRecipe.CreateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto(name: "Beef", amount: default, unit: Unit.FromWeight(Weight.G))]), "Recipe.Ingredients[0].Amount", "Ingredient amount is required" },
        { TestRecipe.CreateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto(name: "Beef", amount: 500, unit: new Unit())]), "Recipe.Ingredients[0].Unit", "Ingredient unit must be either fluid or weight" },
        { TestRecipe.CreateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto(name: "Beef", amount: 500, unit: Unit.FromFluid((Fluid)999))]), "Recipe.Ingredients[0].Unit.Fluid", "Fluid unit is invalid" },
        { TestRecipe.CreateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto(name: "Beef", amount: 500, unit: Unit.FromWeight((Weight)999))]), "Recipe.Ingredients[0].Unit.Weight", "Weight unit is invalid" },
    };

    private static CreateRecipeValidator CreateSut() => new();
}