using Cookbook.Application.Recipe.Queries;
using Cookbook.Application.Recipe.Queries.Validators;
using Cookbook.Repositories;

namespace Cookbook.Tests.Application.Recipe.Validators;

public class GetRecipeByIdValidatorTest
{
    [Fact]
    public async Task Validate_CommandIsValid_ShouldNotHaveValidationError()
    {
        var command = new GetRecipeByIdQuery(1);
        var (sut, recipeRepository) = CreateSut();
        recipeRepository.AnyById(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(true);

        var result = await sut.ValidateAsync(command, CancellationToken.None);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_CommandIsInvalid_ShouldHaveValidationError()
    {
        var command = new GetRecipeByIdQuery(1);
        var (sut, recipeRepository) = CreateSut();
        recipeRepository.AnyById(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await sut.ValidateAsync(command, CancellationToken.None);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("Recipe with id 1 not found");
    }

    private static (GetRecipeByIdValidator, IRecipeRepository) CreateSut()
    {
        var recipeRepository = Substitute.For<IRecipeRepository>();
        var sut = new GetRecipeByIdValidator(recipeRepository);

        return (sut, recipeRepository);
    }
}