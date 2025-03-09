using Ardalis.GuardClauses;
using Cookbook.Application.Recipe.Models;
using Cookbook.Application.UnitOfWork;
using Cookbook.Domain.Recipe.Repositories;
using Cookbook.Domain.Recipe.ValueObjects;
using Cookbook.Domain.Shared.Enums;

namespace Cookbook.Application.Recipe.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdGenerator _idGenerator;

    public RecipeService(IRecipeRepository repository, IUnitOfWork unitOfWork, IIdGenerator idGenerator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _idGenerator = idGenerator;
    }

    public async Task<IEnumerable<RecipeDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        (await _repository.GetAll(cancellationToken)).Select(MapToDto);

    public async Task<RecipeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var recipe = await _repository.GetById(new RecipeId(id), cancellationToken);
        return recipe == null ? null : MapToDto(recipe);
    }

    public async Task<RecipeDto> CreateAsync(CreateRecipeDto dto, CancellationToken cancellationToken = default)
    {
        var id = new RecipeId(_idGenerator.GenerateNextId());
        var recipe = new Domain.Recipe.Entities.Recipe(id, dto.Name, dto.Description);

        foreach (var instruction in dto.Instructions)
        {
            recipe.AddInstruction(instruction);
        }

        foreach (var ingredientDto in dto.Ingredients)
        {
            var unit = ingredientDto switch
            {
                { Unit.Fluid: not null } => MeasurementUnit.Fluid(ingredientDto.Unit.Fluid.Value),
                { Unit.Weight: not null } => MeasurementUnit.Weight(ingredientDto.Unit.Weight.Value),
                { Unit.Piece: not null } => MeasurementUnit.Piece(),
                _ => throw new InvalidOperationException("Invalid unit")
            };

            recipe.AddIngredient(ingredientDto.Name, ingredientDto.Amount, unit);
        }

        await _repository.Add(recipe, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(recipe);
    }

    public async Task UpdateAsync(int id, UpdateRecipeDto dto, CancellationToken cancellationToken = default)
    {
        var recipeId = new RecipeId(id);
        var recipe = await _repository.GetById(recipeId, cancellationToken);
        if (recipe is null)
        {
            throw new NotFoundException(nameof(recipeId), nameof(recipe));
        }

        if (!string.IsNullOrEmpty(dto.Name))
        {
            recipe.UpdateName(dto.Name);
        }

        recipe.UpdateDescription(dto.Description);

        if (dto.Instructions != null)
        {
            recipe.ClearInstructions();

            foreach (var instruction in dto.Instructions)
            {
                recipe.AddInstruction(instruction);
            }
        }

        if (dto.Ingredients != null)
        {
            recipe.ClearIngredients();

            foreach (var ingredientDto in dto.Ingredients)
            {
                var unit = ingredientDto switch
                {
                    { Unit.Fluid: not null } => MeasurementUnit.Fluid(ingredientDto.Unit.Fluid.Value),
                    { Unit.Weight: not null } => MeasurementUnit.Weight(ingredientDto.Unit.Weight.Value),
                    { Unit.Piece: not null } => MeasurementUnit.Piece(),
                    _ => throw new InvalidOperationException("Invalid unit")
                };

                recipe.AddIngredient(ingredientDto.Name, ingredientDto.Amount, unit);
            }
        }

        _repository.Update(recipe);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var recipeId = new RecipeId(id);
        var recipe = await _repository.GetById(recipeId, cancellationToken);
        if (recipe is null)
        {
            throw new NotFoundException(nameof(recipeId), nameof(recipe));
        }

        _repository.Remove(recipe);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static RecipeDto MapToDto(Domain.Recipe.Entities.Recipe recipe)
    {
        return new RecipeDto(
            recipe.Id.Value,
            recipe.Name,
            recipe.Description,
            recipe.Instructions.OrderBy(i => i.StepNumber).Select(i => i.Text).ToList(),
            recipe.Ingredients.Select(i => new IngredientDto(
                i.Name,
                i.Quantity.Amount,
                new Unit(
                    Fluid: i.Quantity.Unit is FluidMeasurement fm ? fm.Unit : null,
                    Weight: i.Quantity.Unit is WeightMeasurement wm ? wm.Unit : null,
                    Piece: i.Quantity.Unit is PieceMeasurement ? Piece.Piece : null
                )
            )).ToList(),
            recipe.RowVersion ?? []
        );
    }
}
