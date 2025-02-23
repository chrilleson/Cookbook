﻿using Cookbook.Application.Recipe.Commands;
using Cookbook.Application.Recipe.Commands.Validators;
using Cookbook.Repositories;
using Cookbook.Tests.Application.Recipe.Models;

namespace Cookbook.Tests.Application.Recipe.Validators;

public class RemoveRecipeValidatorTest
{
    [Fact]
    public async Task Validate_CommandIsValid_ShouldNotHaveValidationError()
    {
        var command = new RemoveRecipeCommand(1);
        var sut = CreateSut();

        var result = await sut.ValidateAsync(command, CancellationToken.None);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_CommandIsInvalid_ShouldHaveValidationError()
    {
        var command = new RemoveRecipeCommand(0);
        var sut = CreateSut();

        var result = await sut.ValidateAsync(command, CancellationToken.None);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("Id is required");
    }

    private static RemoveRecipeValidator CreateSut() =>
        new();
}