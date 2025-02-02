using Ardalis.Result;
using Cookbook.Application.Recipe.Commands;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Repositories;
using Cookbook.Tests.Application.Recipe.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace Cookbook.Tests.Application.Recipe.Commands;

public class RemoveRecipeCommandTest
{
    [Fact]
    public async Task RemoveRecipeCommand_RecipeIsRemoved_ReturnsNoContent()
    {
        var command = new RemoveRecipeCommand(1);
        var (sut, recipeRepository, unitOfWork) = CreateSut();
        recipeRepository.GetById(1, Arg.Any<CancellationToken>()).Returns(TestRecipe.CreateRecipe());

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.NoContent);
        recipeRepository.Received(1).Remove(Arg.Is<Domain.Recipe.Recipe>(x => x.Name == "My favourite recipe"));
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveRecipeCommand_RecipeNotFound_ReturnsNotFound()
    {
        var command = new RemoveRecipeCommand(1);
        var (sut, recipeRepository, unitOfWork) = CreateSut();
        recipeRepository.GetById(1, Arg.Any<CancellationToken>()).ReturnsNull();

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.NotFound);
        actual.IsNotFound().Should().BeTrue();
        recipeRepository.DidNotReceive().Remove(Arg.Any<Domain.Recipe.Recipe>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveRecipeCommand_ThrowsException_ReturnsError()
    {
        var command = new RemoveRecipeCommand(1);
        var (sut, recipeRepository, unitOfWork) = CreateSut();
        recipeRepository.GetById(1, Arg.Any<CancellationToken>()).ThrowsAsyncForAnyArgs(new Exception("Test"));

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.Error);
        actual.Errors.Should().HaveCount(1);
        actual.Errors.First().Should().Be("Test");
        actual.IsError().Should().BeTrue();
        unitOfWork.Received(1).Rollback();
    }

    [Fact]
    public async Task RemoveRecipeCommand_ThrowsDbUpdateException_ReturnsError()
    {
        var command = new RemoveRecipeCommand(1);
        var (sut, recipeRepository, unitOfWork) = CreateSut();
        recipeRepository.GetById(1, Arg.Any<CancellationToken>()).Returns(TestRecipe.CreateRecipe());
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new DbUpdateException("Test"));

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.Should().Be(ResultStatus.Error);
        actual.Errors.Should().HaveCount(1);
        actual.Errors.First().Should().Be("Test");
        actual.IsError().Should().BeTrue();
        recipeRepository.Received(1).Remove(Arg.Is<Domain.Recipe.Recipe>(r => r.Id == 1));
        unitOfWork.Received(1).Rollback();
    }

    private static (RemoveRecipeCommandHandler, IRecipeRepository, IUnitOfWork) CreateSut()
    {
        var recipeRepository = Substitute.For<IRecipeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<RemoveRecipeCommandHandler>>();
        var sut = new RemoveRecipeCommandHandler(recipeRepository, unitOfWork, logger);

        return (sut, recipeRepository, unitOfWork);
    }
}