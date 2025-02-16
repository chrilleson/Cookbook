using Cookbook.Application.Recipe.Commands;
using Cookbook.Application.Recipe.Commands.Validators;
using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Units;
using Cookbook.Repositories;
using Cookbook.Tests.Application.Recipe.Models;
using FluentValidation.TestHelper;

namespace Cookbook.Tests.Application.Recipe.Validators;

public class UpdateRecipeValidatorTest
{
    [Fact]
    public async Task CommandIsValid_ShouldNotHaveError()
    {
        const int id = 1;
        IngredientDto[] ingredients = [TestRecipe.CreateIngredientDto("Beef", 500, Unit.FromWeight(Weight.G)), TestRecipe.CreateIngredientDto("Milk", 2, Unit.FromFluid(Fluid.Dl))];
        var recipe = TestRecipe.CreateUpdateRecipeDto(
            name: "My favourite recipe",
            instructions: ["Do this", "Do that"],
            ingredients: ingredients);
        var (sut, recipeRepository) = CreateSut();
        recipeRepository.AnyById(Arg.Any<int>(), CancellationToken.None).ReturnsForAnyArgs(true);

        var actual = await sut.TestValidateAsync(new UpdateRecipeCommand(id, recipe));

        actual.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task CommandIsInvalid_ShouldHaveErrors(int id, UpdateRecipeDto recipe, string property, string errorMessage)
    {
        var (sut, recipeRepository) = CreateSut();
        recipeRepository.AnyById(Arg.Any<int>(), CancellationToken.None).ReturnsForAnyArgs(id == 1);

        var actual = await sut.TestValidateAsync(new UpdateRecipeCommand(id, recipe));

        actual.ShouldHaveValidationErrorFor(property).WithErrorMessage(errorMessage);
    }

    public static TheoryData<int, UpdateRecipeDto, string, string> InvalidData = new()
    {
        { 0, TestRecipe.CreateUpdateRecipeDto(name: "", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto()]), "Id", "Recipe with id 0 not found" },
        { 1, TestRecipe.CreateUpdateRecipeDto(name: "", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto()]), "Recipe.Name", "Name is required" },
        { 1, TestRecipe.CreateUpdateRecipeDto(name: "My favourite recipe", instructions: Array.Empty<string>(), ingredients: [TestRecipe.CreateIngredientDto()]), "Recipe.Instructions", "Instructions are required" },
        { 1, TestRecipe.CreateUpdateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: Array.Empty<IngredientDto>()), "Recipe.Ingredients", "Ingredients are required" },
        { 1, TestRecipe.CreateUpdateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto(name: "", amount: 500, unit: Unit.FromWeight(Weight.G))]), "Recipe.Ingredients[0].Name", "Ingredient name is required" },
        { 1, TestRecipe.CreateUpdateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: Enumerable.Repeat(TestRecipe.CreateIngredientDto(name: "Test", amount: 500, unit: Unit.FromWeight(Weight.G)), 2)), "", "Ingredient names must be unique" },
        { 1, TestRecipe.CreateUpdateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto(name: "Beef", amount: default, unit: Unit.FromWeight(Weight.G))]), "Recipe.Ingredients[0].Amount", "Ingredient amount is required" },
        { 1, TestRecipe.CreateUpdateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto(name: "Beef", amount: 500, unit: new Unit())]), "Recipe.Ingredients[0].Unit", "Ingredient unit must be either fluid, weight, or piece" },
        { 1, TestRecipe.CreateUpdateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto(name: "Beef", amount: 500, unit: Unit.FromFluid((Fluid)999))]), "Recipe.Ingredients[0].Unit.Fluid", "Fluid unit is invalid" },
        { 1, TestRecipe.CreateUpdateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto(name: "Beef", amount: 500, unit: Unit.FromWeight((Weight)999))]), "Recipe.Ingredients[0].Unit.Weight", "Weight unit is invalid" },
        { 1, TestRecipe.CreateUpdateRecipeDto(name: "My favourite recipe", instructions: ["Do this", "Do that"], ingredients: [TestRecipe.CreateIngredientDto(name: "Beef", amount: 500, unit: Unit.FromPiece((Piece)999))]), "Recipe.Ingredients[0].Unit.Piece", "Piece unit is invalid" },
    };

    private static (UpdateRecipeValidator, IRecipeRepository) CreateSut()
    {
        var recipeRepository = Substitute.For<IRecipeRepository>();
        var sut = new UpdateRecipeValidator(recipeRepository);

        return (sut, recipeRepository);
    }
}