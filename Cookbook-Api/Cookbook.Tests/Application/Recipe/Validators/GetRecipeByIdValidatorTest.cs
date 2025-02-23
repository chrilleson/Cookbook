using Cookbook.Application.Recipe.Queries;
using Cookbook.Application.Recipe.Queries.Validators;

namespace Cookbook.Tests.Application.Recipe.Validators;

public class GetRecipeByIdValidatorTest
{
    [Fact]
    public async Task Validate_CommandIsValid_ShouldNotHaveValidationError()
    {
        var command = new GetRecipeByIdQuery(1);
        var sut = CreateSut();

        var result = await sut.ValidateAsync(command, CancellationToken.None);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_CommandIsInvalid_ShouldHaveValidationError()
    {
        var command = new GetRecipeByIdQuery(0);
        var sut = CreateSut();

        var result = await sut.ValidateAsync(command, CancellationToken.None);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("Id is required");
    }

    private static GetRecipeByIdValidator CreateSut() =>
        new();
}