using Ardalis.Result;
using Cookbook.Application.Recipe.Commands;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Repositories;
using Cookbook.Tests.Application.Recipe.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NSubstitute.ExceptionExtensions;

namespace Cookbook.Tests.Application.Recipe.Commands;

public class UpdateRecipeCommandTest
{
    [Fact]
    public async Task UpdateRecipeCommand_RecipeDoesNotExist_ReturnsNotFound()
    {
        const int id = 1;
        var command = new UpdateRecipeCommand(id, TestRecipe.CreateUpdateRecipeDto());
        var (sut, recipeRepository, _) = CreateSut();
        recipeRepository.GetById(command.Id, Arg.Any<CancellationToken>()).Returns((Domain.Recipe.Recipe?)null);

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task UpdateRecipeCommand_RecipeIsUpdated_ReturnsSuccessWithMessage()
    {
        const int id = 1;
        var command = new UpdateRecipeCommand(id, TestRecipe.CreateUpdateRecipeDto());
        var (sut, recipeRepository, unitOfWork) = CreateSut();
        var existingRecipe = TestRecipe.CreateRecipe();
        recipeRepository.GetById(command.Id, Arg.Any<CancellationToken>()).Returns(existingRecipe);

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.Ok);
        actual.SuccessMessage.Should().Be("Recipe updated");
        recipeRepository.Received(1).Update(Arg.Is<Domain.Recipe.Recipe>(r => r.Id == command.Id));
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateRecipeCommand_ThrowsException_ReturnsError()
    {
        const int id = 1;
        var command = new UpdateRecipeCommand(id, TestRecipe.CreateUpdateRecipeDto());
        var (sut, recipeRepository, unitOfWork) = CreateSut();
        var existingRecipe = TestRecipe.CreateRecipe();
        recipeRepository.GetById(command.Id, Arg.Any<CancellationToken>()).Returns(existingRecipe);
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsyncForAnyArgs(new Exception());

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.Error);
        unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task UpdateRecipeCommand_RecipeAlreadyExists_ReturnsConflict()
    {
        const int id = 1;
        var command = new UpdateRecipeCommand(id, TestRecipe.CreateUpdateRecipeDto());
        var (sut, recipeRepository, unitOfWork) = CreateSut();
        var existingRecipe = TestRecipe.CreateRecipe();
        recipeRepository.GetById(command.Id, Arg.Any<CancellationToken>()).Returns(existingRecipe);
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsyncForAnyArgs(new DbUpdateException("Error", new PostgresException(default, default, default, "23505", default)));

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.Conflict);
        actual.IsConflict().Should().BeTrue();
        actual.Errors.Should().Contain("Recipe already exists");
        unitOfWork.Received(1).Rollback();
        recipeRepository.Received(1).Update(Arg.Is<Domain.Recipe.Recipe>(r => r.Id == command.Id));
    }

    [Fact]
    public async Task UpdateRecipeCommand_ThrowsDbUpdateException_ReturnsError()
    {
        const int id = 1;
        var command = new UpdateRecipeCommand(id, TestRecipe.CreateUpdateRecipeDto());
        var (sut, recipeRepository, unitOfWork) = CreateSut();
        var existingRecipe = TestRecipe.CreateRecipe();
        recipeRepository.GetById(command.Id, Arg.Any<CancellationToken>()).Returns(existingRecipe);
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsyncForAnyArgs(new DbUpdateException("Error"));

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.Error);
        actual.Errors.Should().Contain("Error");
        actual.IsError().Should().BeTrue();
        unitOfWork.Received(1).Rollback();
        recipeRepository.Received(1).Update(Arg.Is<Domain.Recipe.Recipe>(r => r.Id == command.Id));
    }

    private static (UpdateRecipeCommandHandler, IRecipeRepository, IUnitOfWork) CreateSut()
    {
        var recipeRepository = Substitute.For<IRecipeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<UpdateRecipeCommandHandler>>();
        var sut = new UpdateRecipeCommandHandler(recipeRepository, unitOfWork, logger);

        return (sut, recipeRepository, unitOfWork);
    }
}