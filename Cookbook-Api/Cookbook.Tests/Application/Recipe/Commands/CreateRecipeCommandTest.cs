using Ardalis.Result;
using Cookbook.Application.Recipe.Commands;
using Cookbook.Domain.Units;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Repositories;
using Cookbook.Tests.Application.Recipe.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NSubstitute.ExceptionExtensions;
using Unit = Cookbook.Application.Recipe.Models.Unit;

namespace Cookbook.Tests.Application.Recipe.Commands;

public class CreateRecipeCommandTest
{
    [Fact]
    public async Task CreateRecipeCommand_RecipeIsCreated_ReturnsSuccessWithMessage()
    {
        var command = new CreateRecipeCommand(TestRecipe.CreateRecipeDto(ingredients: [TestRecipe.CreateIngredientDto(unit: Unit.FromWeight(Weight.G))], instructions: ["test"]));
        var (sut, recipeRepository, unitOfWork) = CreateSut();

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.Created);
        actual.Value.Name.Should().Be("My favourite recipe");
        actual.Location.Should().Be("/api/recipes/1");
        await recipeRepository.Received(1).Add(Arg.Is<Domain.Recipe.Recipe>(x => x.Name == "My favourite recipe"), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateRecipeCommand_ThrowsException_ReturnsError()
    {
        var command = new CreateRecipeCommand(TestRecipe.CreateRecipeDto(ingredients: [TestRecipe.CreateIngredientDto(unit: Unit.FromWeight(Weight.G))], instructions: ["test"]));
        var (sut, recipeRepository, unitOfWork) = CreateSut();
        recipeRepository.Add(Arg.Any<Domain.Recipe.Recipe>(), Arg.Any<CancellationToken>()).ThrowsAsyncForAnyArgs(new Exception("Test"));

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.Error);
        actual.Errors.Should().HaveCount(1);
        actual.Errors.First().Should().Be("Test");
        actual.IsError().Should().BeTrue();
        unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task CreateRecipeCommand_RecipeAlreadyExists_ReturnsConflict()
    {
        var command = new CreateRecipeCommand(TestRecipe.CreateRecipeDto(ingredients: [TestRecipe.CreateIngredientDto(unit: Unit.FromWeight(Weight.G))], instructions: ["test"]));
        var (sut, recipeRepository, unitOfWork) = CreateSut();
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsyncForAnyArgs(new DbUpdateException("Test", new PostgresException(default, default, default, "23505", default)));

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.Conflict);
        actual.Errors.Should().HaveCount(1);
        actual.Errors.First().Should().Be("Recipe already exists");
        actual.IsConflict().Should().BeTrue();
        await recipeRepository.Received(1).Add(Arg.Is<Domain.Recipe.Recipe>(r => r.Name == "My favourite recipe"), Arg.Any<CancellationToken>());
        unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task CreateRecipeCommand_ThrowsDbUpdateException_ReturnsError()
    {
        var command = new CreateRecipeCommand(TestRecipe.CreateRecipeDto(ingredients: [TestRecipe.CreateIngredientDto(unit: Unit.FromWeight(Weight.G))], instructions: ["test"]));
        var (sut, _, unitOfWork) = CreateSut();
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsyncForAnyArgs(new DbUpdateException("Test"));

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.Error);
        actual.Errors.Should().HaveCount(1);
        actual.Errors.First().Should().Be("Test");
        actual.IsError().Should().BeTrue();
        unitOfWork.Received(1).Rollback();
    }

    private static (CreateRecipeCommandHandler, IRecipeRepository, IUnitOfWork) CreateSut()
    {
        var recipeRepository = Substitute.For<IRecipeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<CreateRecipeCommandHandler>>();
        var sut = new CreateRecipeCommandHandler(recipeRepository, unitOfWork, logger);

        return (sut, recipeRepository, unitOfWork);
    }
}