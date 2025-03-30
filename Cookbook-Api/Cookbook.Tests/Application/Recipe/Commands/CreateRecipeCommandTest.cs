using Ardalis.Result;
using Cookbook.Application.Recipe.Commands;
using Cookbook.Application.Recipe.Services;
using Cookbook.Application.UnitOfWork;
using Cookbook.Domain.Recipe.Repositories;
using Cookbook.Domain.Shared.Enums;
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
        var command = new CreateRecipeCommand(TestRecipe.CreateCreateRecipeDto(ingredients: [TestRecipe.CreateIngredientDto(unit: Unit.FromWeight(Weight.G))], instructions: ["test"]));
        var (sut, idGenerator, recipeRepository, unitOfWork) = CreateSut();
        idGenerator.GenerateNextId().Returns(1);

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.ShouldBe(ResultStatus.Created);
        actual.Value.Name.ShouldBe("My favourite recipe");
        actual.Location.ShouldBe("/api/recipes/1");
        await recipeRepository.Received(1).Add(Arg.Is<Domain.Recipe.Entities.Recipe>(x => x.Name == "My favourite recipe"), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateRecipeCommand_ThrowsException_ReturnsError()
    {
        var command = new CreateRecipeCommand(TestRecipe.CreateCreateRecipeDto(ingredients: [TestRecipe.CreateIngredientDto(unit: Unit.FromWeight(Weight.G))], instructions: ["test"]));
        var (sut, idGenerator, recipeRepository, unitOfWork) = CreateSut();
        idGenerator.GenerateNextId().Returns(1);
        recipeRepository.Add(Arg.Any<Domain.Recipe.Entities.Recipe>(), Arg.Any<CancellationToken>()).ThrowsAsyncForAnyArgs(new Exception("Test"));

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.ShouldBe(ResultStatus.Error);
        actual.Errors.Count().ShouldBe(1);
        actual.Errors.First().ShouldBe("Test");
        actual.IsError().ShouldBeTrue();
        unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task CreateRecipeCommand_RecipeAlreadyExists_ReturnsConflict()
    {
        var command = new CreateRecipeCommand(TestRecipe.CreateCreateRecipeDto(ingredients: [TestRecipe.CreateIngredientDto(unit: Unit.FromWeight(Weight.G))], instructions: ["test"]));
        var (sut, idGenerator, recipeRepository, unitOfWork) = CreateSut();
        idGenerator.GenerateNextId().Returns(1);
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsyncForAnyArgs(new DbUpdateException("Test", new PostgresException(null!, null!, null!, "23505", null)));

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.ShouldBe(ResultStatus.Conflict);
        actual.Errors.Count().ShouldBe(1);
        actual.Errors.First().ShouldBe("Recipe already exists");
        actual.IsConflict().ShouldBeTrue();
        await recipeRepository.Received(1).Add(Arg.Is<Domain.Recipe.Entities.Recipe>(r => r.Name == "My favourite recipe"), Arg.Any<CancellationToken>());
        unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task CreateRecipeCommand_ThrowsDbUpdateException_ReturnsError()
    {
        var command = new CreateRecipeCommand(TestRecipe.CreateCreateRecipeDto(ingredients: [TestRecipe.CreateIngredientDto(unit: Unit.FromWeight(Weight.G))], instructions: ["test"]));
        var (sut, idGenerator, _, unitOfWork) = CreateSut();
        idGenerator.GenerateNextId().Returns(1);
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsyncForAnyArgs(new DbUpdateException("Test"));

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.ShouldBe(ResultStatus.Error);
        actual.Errors.Count().ShouldBe(1);
        actual.Errors.First().ShouldBe("Test");
        actual.IsError().ShouldBeTrue();
        unitOfWork.Received(1).Rollback();
    }

    private static (CreateRecipeCommandHandler, IIdGenerator, IRecipeRepository, IUnitOfWork) CreateSut()
    {
        var idGenerator = Substitute.For<IIdGenerator>();
        var recipeRepository = Substitute.For<IRecipeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<CreateRecipeCommandHandler>>();
        var sut = new CreateRecipeCommandHandler(idGenerator, recipeRepository, unitOfWork, logger);

        return (sut, idGenerator, recipeRepository, unitOfWork);
    }
}
