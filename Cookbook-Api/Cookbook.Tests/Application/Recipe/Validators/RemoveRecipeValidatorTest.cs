using Cookbook.Application.Recipe.Commands;
using Cookbook.Application.Recipe.Commands.Validators;
using Cookbook.Repositories;
using Cookbook.Tests.Application.Recipe.Models;
using FluentAssertions;
using NSubstitute;

namespace Cookbook.Tests.Application.Recipe.Validators;

public class RemoveRecipeValidatorTest
{
    [Fact]
    public async Task Validate_CommandIsValid_ShouldNotHaveValidationError()
    {
        var command = new RemoveRecipeCommand(1);
        var (sut, recipeRepository) = CreateSut();
        recipeRepository.GetById(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(TestRecipe.CreateRecipe());

        var result = await sut.ValidateAsync(command, CancellationToken.None);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_CommandIsInvalid_ShouldHaveValidationError()
    {
        var command = new RemoveRecipeCommand(1);
        var (sut, recipeRepository) = CreateSut();
        recipeRepository.GetById(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(null as Domain.Recipe.Recipe);

        var result = await sut.ValidateAsync(command, CancellationToken.None);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("Recipe with id 1 not found");
    }

    private static (RemoveRecipeValidator, IRecipeRepository) CreateSut()
    {
        var recipeRepository = Substitute.For<IRecipeRepository>();
        var sut = new RemoveRecipeValidator(recipeRepository);

        return (sut, recipeRepository);
    }
}