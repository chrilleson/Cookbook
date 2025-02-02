using Ardalis.Result;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.Recipe.Queries;
using Cookbook.Domain.Units;
using Cookbook.Repositories;
using Cookbook.Tests.Application.Recipe.Models;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace Cookbook.Tests.Application.Recipe.Queries;

public class GetRecipeByIdQueryTest
{
    [Fact]
    public async Task GetRecipeByIdQuery_RecipeExists_ReturnsSuccessWithRecipe()
    {
        var recipe = TestRecipe.CreateRecipe(instructions: new Dictionary<int, string> { [1] = "First Instruction" }, ingredients: [TestRecipe.CreateIngredient(weight: Weight.G)]);
        var (sut, recipeRepository) = CreateSut();
        recipeRepository.GetById(1, Arg.Any<CancellationToken>()).Returns(recipe);

        var actual = await sut.Handle(new GetRecipeByIdQuery(1), CancellationToken.None);

        actual.IsSuccess.Should().BeTrue();
        actual.SuccessMessage.Should().Be("Recipe found");
        actual.Value.Name.Should().Be("My favourite recipe");
        actual.Value.Instructions.Should().HaveCount(1);
        actual.Value.Instructions.First().Should().Be("First Instruction");
        actual.Value.Ingredients.Should().HaveCount(1);
        actual.Value.Ingredients.First().Name.Should().Be("Beef");
        actual.Value.Ingredients.First().Amount.Should().Be(500);
        actual.Value.Ingredients.First().Unit.Should().Be(Unit.FromWeight(Weight.G));
    }

    [Fact]
    public async Task GetRecipeByIdQuery_RecipeDoesNotExist_ReturnsNotFound()
    {
        var (sut, recipeRepository) = CreateSut();
        recipeRepository.GetById(1, Arg.Any<CancellationToken>()).ReturnsNull();

        var actual = await sut.Handle(new GetRecipeByIdQuery(1), CancellationToken.None);

        actual.IsNotFound().Should().BeTrue();
        actual.Errors.Should().HaveCount(1);
        actual.Errors.First().Should().Be("Recipe not found");
    }

    [Fact]
    public async Task GetRecipeByIdQuery_ThrowsException_ReturnsError()
    {
        var (sut, recipeRepository) = CreateSut();
        recipeRepository.GetById(1, Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("Test"));

        var actual = await sut.Handle(new GetRecipeByIdQuery(1), CancellationToken.None);

        actual.IsError().Should().BeTrue();
        actual.Errors.Should().HaveCount(1);
        actual.Errors.First().Should().Be("Something went wrong while fetching recipe");
    }

    private static (GetRecipeByIdQueryHandler, IRecipeRepository) CreateSut()
    {
        var recipeRepository = Substitute.For<IRecipeRepository>();
        var logger = Substitute.For<ILogger<GetRecipeByIdQueryHandler>>();
        var sut = new GetRecipeByIdQueryHandler(recipeRepository, logger);

        return (sut, recipeRepository);
    }
}