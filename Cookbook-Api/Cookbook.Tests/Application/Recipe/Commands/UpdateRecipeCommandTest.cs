using Ardalis.Result;
using Cookbook.Application.Recipe.Commands;
using Cookbook.Application.Repositories;
using Cookbook.Application.UnitOfWork;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Tests.Application.Recipe.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;
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

        actual.Status.ShouldBe(ResultStatus.NotFound);
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

        actual.Status.ShouldBe(ResultStatus.Ok);
        actual.SuccessMessage.ShouldBe("Recipe updated");
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

        actual.Status.ShouldBe(ResultStatus.Error);
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
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsyncForAnyArgs(new DbUpdateException("Error", new PostgresException(null!, null!, null!, "23505", null)));

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.ShouldBe(ResultStatus.Conflict);
        actual.IsConflict().ShouldBeTrue();
        actual.Errors.ShouldContain("Recipe already exists");
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

        actual.Status.ShouldBe(ResultStatus.Error);
        actual.Errors.ShouldContain("Error");
        actual.IsError().ShouldBeTrue();
        unitOfWork.Received(1).Rollback();
        recipeRepository.Received(1).Update(Arg.Is<Domain.Recipe.Recipe>(r => r.Id == command.Id));
    }

    [Theory]
    [InlineData(true, "The recipe was modified by another user. Please refresh and try again.")]
    [InlineData(false, "The recipe has been deleted by another user.")]
    public async Task UpdateRecipeCommand_ThrowsDbConcurrencyException_ReturnsAppropriateError(bool entityExists, string expectedError)
    {
        var propertyValues = entityExists ? new TestPropertyValues() : null;
        var updateEntry = new TestUpdateEntry(propertyValues);
        var concurrencyException = new DbUpdateConcurrencyException("Concurrency exception", [updateEntry]);
        const int id = 1;
        var command = new UpdateRecipeCommand(id, TestRecipe.CreateUpdateRecipeDto());
        var existingRecipe = TestRecipe.CreateRecipe();
        var (sut, recipeRepository, unitOfWork) = CreateSut();
        recipeRepository.GetById(command.Id, Arg.Any<CancellationToken>()).Returns(existingRecipe);
        unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsyncForAnyArgs(concurrencyException);

        var actual = await sut.Handle(command, CancellationToken.None);

        actual.Status.ShouldBe(ResultStatus.Error);
        actual.IsError().ShouldBeTrue();
        actual.Errors.First().ShouldBe(expectedError);
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

// Custom test implementation of IUpdateEntry
internal class TestUpdateEntry : IUpdateEntry
{
    private readonly TestEntityEntry _entityEntry;

    public TestUpdateEntry(PropertyValues? databaseValues)
    {
        _entityEntry = new TestEntityEntry(databaseValues);
    }

    public EntityEntry ToEntityEntry() => _entityEntry;
    public void SetOriginalValue(IProperty property, object? value) => throw new NotImplementedException();
    public void SetPropertyModified(IProperty property) => throw new NotImplementedException();
    bool IUpdateEntry.IsModified(IProperty property) => throw new NotImplementedException();
    bool IUpdateEntry.HasTemporaryValue(IProperty property) => throw new NotImplementedException();
    public bool HasExplicitValue(IProperty property) => throw new NotImplementedException();
    public bool HasStoreGeneratedValue(IProperty property) => throw new NotImplementedException();
    public bool IsStoreGenerated(IProperty property) => throw new NotImplementedException();
    public object? GetCurrentValue(IPropertyBase propertyBase) => throw new NotImplementedException();
    public object? GetOriginalValue(IPropertyBase propertyBase) => throw new NotImplementedException();
    public bool CanHaveOriginalValue(IPropertyBase propertyBase) => throw new NotImplementedException();
    public TProperty GetCurrentValue<TProperty>(IPropertyBase propertyBase) => throw new NotImplementedException();
    public TProperty GetOriginalValue<TProperty>(IProperty property) => throw new NotImplementedException();
    public void SetStoreGeneratedValue(IProperty property, object? value, bool setModified = true) => throw new NotImplementedException();
    public object? GetRelationshipSnapshotValue(IPropertyBase propertyBase) => throw new NotImplementedException();
    public object? GetPreStoreGeneratedCurrentValue(IPropertyBase propertyBase) => throw new NotImplementedException();
    public EntityState EntityState => throw new NotImplementedException();
    public IUpdateEntry SharedIdentityEntry => throw new NotImplementedException();
    public IEntityType EntityType => throw new NotImplementedException();
    EntityState IUpdateEntry.EntityState { get; set; }
    public bool IsConceptualNull(IProperty property) => throw new NotImplementedException();
    public DbContext Context { get; } = null!;
}

// Custom EntityEntry implementation
internal class TestEntityEntry : EntityEntry
{
    private readonly PropertyValues? _databaseValues;

#pragma warning disable EF1001 // Microsoft. EntityFrameworkCore. ChangeTracking. EntityEntry is an internal API that supports the Entity Framework Core infrastructure and not subject to the same compatibility standards as public APIs. It may be changed or removed without notice in any release.
    public TestEntityEntry(PropertyValues? databaseValues) : base(null!)
    {
        _databaseValues = databaseValues;
    }

    public override Task<PropertyValues?> GetDatabaseValuesAsync(CancellationToken cancellationToken = default) => Task.FromResult(_databaseValues);
}

// Custom PropertyValues implementation
internal class TestPropertyValues : PropertyValues
{
    public TestPropertyValues() : base(null!) { }

    public override IReadOnlyList<IProperty> Properties { get; } = null!;

    public override object? this[string propertyName]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    public override object? this[IProperty property]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    public override object ToObject() => throw new NotImplementedException();
    public override void SetValues(object obj) => throw new NotImplementedException();
    public override PropertyValues Clone() => throw new NotImplementedException();
    public override void SetValues(PropertyValues propertyValues) => throw new NotImplementedException();
    public override TValue GetValue<TValue>(string propertyName) => throw new NotImplementedException();
    public override TValue GetValue<TValue>(IProperty property) => throw new NotImplementedException();
}