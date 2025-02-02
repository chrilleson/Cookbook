using Ardalis.Result;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.Recipe.Queries;
using Cookbook.Domain.Units;
using Cookbook.Repositories;
using Cookbook.Tests.Application.Recipe.Models;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace Cookbook.Tests.Application.Recipe.Queries;

public class GetAllRecipesTest
{
    [Fact]
    public async Task GetAllRecipesQuery_RecipesExist_ReturnsSuccessWithRecipes()
    {
        IEnumerable<Domain.Recipe.Recipe> recipes =
        [
            TestRecipe.CreateRecipe(instructions: new Dictionary<int, string> { [1] = "First Instruction" }, ingredients: [TestRecipe.CreateIngredient(weight: Weight.G)]),
            TestRecipe.CreateRecipe(id: 2, name: "Another recipe", instructions: new Dictionary<int, string> { [1] = "Pour the milk in the glass" }, ingredients: [TestRecipe.CreateIngredient(name: "Milk", amount: 2, fluid: Fluid.Dl)])
        ];
        var (sut, recipeRepository) = CreateSut();
        recipeRepository.GetAll(Arg.Any<CancellationToken>()).Returns(recipes);

        var actual = await sut.Handle(new GetAllRecipesQuery(), CancellationToken.None);

        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().HaveCount(2);
        actual.Value.First().Name.Should().Be("My favourite recipe");
        actual.Value.First().Instructions.Should().HaveCount(1);
        actual.Value.First().Instructions.First().Should().Be("First Instruction");
        actual.Value.First().Ingredients.Should().HaveCount(1);
        actual.Value.First().Ingredients.First().Name.Should().Be("Beef");
        actual.Value.First().Ingredients.First().Amount.Should().Be(500);
        actual.Value.First().Ingredients.First().Unit.Should().Be(Unit.FromWeight(Weight.G));
        actual.Value.Last().Name.Should().Be("Another recipe");
        actual.Value.Last().Instructions.Should().HaveCount(1);
        actual.Value.Last().Instructions.First().Should().Be("Pour the milk in the glass");
        actual.Value.Last().Ingredients.Should().HaveCount(1);
        actual.Value.Last().Ingredients.First().Name.Should().Be("Milk");
        actual.Value.Last().Ingredients.First().Amount.Should().Be(2);
        actual.Value.Last().Ingredients.First().Unit.Should().Be(Unit.FromFluid(Fluid.Dl));
    }

    [Fact]
    public async Task GetAllRecipesQuery_ThrowsException_ReturnsError()
    {
        var (sut, recipeRepository) = CreateSut();
        recipeRepository.GetAll(Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("Test"));

        var actual = await sut.Handle(new GetAllRecipesQuery(), CancellationToken.None);

        actual.IsError().Should().BeTrue();
        actual.Errors.Should().HaveCount(1);
        actual.Errors.First().Should().Be("Test");
    }

    private static (GetAllRecipesQueryHandler, IRecipeRepository) CreateSut()
    {
        var recipeRepository = Substitute.For<IRecipeRepository>();
        var logger = Substitute.For<ILogger<GetAllRecipesQueryHandler>>();
        var sut = new GetAllRecipesQueryHandler(recipeRepository, logger);

        return (sut, recipeRepository);
    }
}